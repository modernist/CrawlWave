using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.Security.Permissions;
using Microsoft.Web.Services2;
using Microsoft.Web.Services2.Security;
using Microsoft.Web.Services2.Security.Tokens;
using Microsoft.Web.Services2.Security.X509;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.Server
{
	/// <summary>
	/// Authentication is a <see cref="UsernameTokenManager"/> class that performs the
	/// authentication required for CrawlWave Clients. It performs lookup based on the
	/// username provided by the Client and it verifies it against the database.
	/// </summary>
	/// <remarks>
	/// This class includes the <see cref="SecurityPermissionFlag.UnmanagedCode"/> demand
	/// to ensure that any untrusted assemblies cannot invoke this code. This helps mitigate
	/// brute-force password discovery attacks. 
	///</remarks>
	[SecurityPermissionAttribute(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
	public class Authentication  : UsernameTokenManager
	{
		#region Private variables

		private ServerSettings settings;
		private SqlConnection dbcon;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructs a new instance of the <see cref="ServerEngine"/> class and opens a
		/// connection to the database.
		/// </summary>
		public Authentication()
		{
			settings = ServerSettings.Instance();
			dbcon = null;
			ConnectToDatabase();
		}

		#endregion

		#region Base Class method overrides
		/// <summary>
		/// Authenticates a <see cref="UsernameToken"/>.
		/// </summary>
		/// <param name="token">The <see cref="UsernameToken"/> to authenticate.</param>
		/// <returns>
		/// A string containing the password corresponding to the username of the provided
		/// token. If no username match occurs a null string is returned.
		/// </returns>
		protected override string AuthenticateToken(UsernameToken token)
		{
			if (token == null)					
				throw new ArgumentNullException();

			/*//perform a lookup in your database for the user name in 'token.Username'
			//and return the password as a string. If there is no match, return null.
			if(token.Username == "circular")
				return new Guid(MD5Hash.md5("mpamies")).ToString();
			return null;*/
			string retVal = null;
			try
			{
				if(!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				SqlCommand cmd = new SqlCommand("cw_select_user_authentication",dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@username", SqlDbType.NVarChar, 20);
				cmd.Parameters[0].Value = token.Username;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				if(ds.Tables[0].Rows.Count>0)
				{
					Guid password = (Guid)ds.Tables[0].Rows[0][0];
					retVal = password.ToString();
				}
				ds.Dispose();
				if(!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
			}
			catch
			{
				//log error
			}
			return retVal;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the connection to the database if necessary and opens it.
		/// </summary>
		/// <returns>True if the operation is successful, false otherwise.</returns>
		private bool ConnectToDatabase()
		{
			try
			{
				if(dbcon == null)
				{
					dbcon = new SqlConnection(settings.ProvideConnectionString());
					dbcon.Open();
				}
				else
				{
					if(dbcon.State != ConnectionState.Open)
					{
						dbcon.Open();
					}
				}
				return true;
			}
			catch(Exception e)
			{
				if(settings.LogLevel <= CWLogLevel.LogError)
				{
					settings.Log.LogError("CrawlWave Server failed to connect to the database: " + e.ToString());
				}
				return false;
			}
		}

		/// <summary>
		/// Disconeects the CrawlWave Server from the system's database.
		/// </summary>
		/// <returns>True if the operation is successful, false otherwise.</returns>
		private bool DisconnectFromDatabase()
		{
			try
			{
				if(dbcon != null)
				{
					if(dbcon.State != ConnectionState.Closed)
					{
						dbcon.Close();
					}
				}
				return true;
			}
			catch(Exception e)
			{
				if(settings.LogLevel <= CWLogLevel.LogError)
				{
					settings.Log.LogError("CrawlWave Server failed to disconnect from the database: " + e.ToString());
				}
				return false;
			}
		}

		#endregion
	}

	/// <summary>
	/// Contains static methods that allow common security functions, like Message Signature
	/// verification and Security Token extraction.
	/// </summary>
	public class SecurityUtils
	{
		/// <summary>
		/// This method basically checks if those required message parts exist. The required
		/// headers are Soap:body, To, Action, MessageID, and Timestamp
		/// </summary>
		/// <param name="context">The <see cref="SoapContext"/> to check.</param>
		/// <exception cref="ApplicationException">Thrown if the verification fails.</exception>
		public static void VerifyMessageParts(SoapContext context) 
		{
			// Body
			if (context.Envelope.Body == null)
			{
				throw new ApplicationException("The message must contain a soap:Body element");
			}
			if (context.Addressing.To == null || context.Addressing.To.TargetElement == null)
			{
				throw new ApplicationException("The message must contain a wsa:To header");
			}
			if (context.Addressing.Action == null || context.Addressing.Action.TargetElement == null)
			{
				throw new ApplicationException("The message must contain a wsa:Action header");
			}
			if (context.Addressing.MessageID == null || context.Addressing.MessageID.TargetElement == null)
			{
				throw new ApplicationException("The message must contain a wsa:To header");
			}
			if (context.Security.Timestamp == null)
			{
				throw new ApplicationException("The message must contain a wsu:Timestamp header");
			}
		}

		/// <summary>
		/// This method checks if the incoming message has signed all the important message
		/// parts such as soap:Body, all the addressing headers and timestamp.  
		/// </summary>
		/// <param name="context"></param>
		/// <returns>The signing token</returns>
		public static SecurityToken GetSigningToken(SoapContext context)
		{
			foreach (ISecurityElement element in context.Security.Elements)
			{
				if (element is MessageSignature)
				{
					// The given context contains a Signature element.
					MessageSignature sig = element as MessageSignature;
					if (CheckSignature(context, sig))
					{
						// The SOAP Body is signed.
						return sig.SigningToken;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// This method verifies an incoming message's signature by verifying the signature
		/// on those parts that were actually signed.
		/// </summary>
		/// <param name="context">The request's <see cref="SoapContext"/>.</param>
		/// <param name="signature">The <see cref="MessageSignature"/> to check.</param>
		/// <returns>True if the signature is verified, false otherwise.</returns>
		private static bool CheckSignature(SoapContext context, MessageSignature signature)
		{
			// Now verify which parts of the message were actually signed.
			SignatureOptions actualOptions   = signature.SignatureOptions;
			SignatureOptions expectedOptions = SignatureOptions.IncludeSoapBody;
          
			if (context.Security != null && context.Security.Timestamp != null) 
				expectedOptions |= SignatureOptions.IncludeTimestamp;  

			// The <Action> and <To> are required addressing elements.
			expectedOptions |= SignatureOptions.IncludeAction;
			expectedOptions |= SignatureOptions.IncludeTo;

			if (context.Addressing.FaultTo != null && context.Addressing.FaultTo.TargetElement != null)
			{
				expectedOptions |= SignatureOptions.IncludeFaultTo;
			}
			if (context.Addressing.From != null && context.Addressing.From.TargetElement != null)
			{
				expectedOptions |= SignatureOptions.IncludeFrom;
			}
			if (context.Addressing.MessageID != null && context.Addressing.MessageID.TargetElement != null)
			{
				expectedOptions |= SignatureOptions.IncludeMessageId;
			}
			if (context.Addressing.RelatesTo != null && context.Addressing.RelatesTo.TargetElement != null)
			{
				expectedOptions |= SignatureOptions.IncludeRelatesTo;
			}
			if (context.Addressing.ReplyTo != null && context.Addressing.ReplyTo.TargetElement != null)
			{
				expectedOptions |= SignatureOptions.IncludeReplyTo;
			}
			// Check if the all the expected options are the present.
			return ((expectedOptions & actualOptions) == expectedOptions);        
		}

		/// <summary>
		/// This method checks if the incoming message contains encrypted soap message body 
		/// </summary>
		/// <param name="context">The <see cref="SoapContext"/> to search for.</param>
		/// <returns>The encrypting token</returns>
		public static SecurityToken GetEncryptingToken(SoapContext context)
		{
			SecurityToken encryptingToken = null;

			foreach (ISecurityElement element in context.Security.Elements)
			{
				if (element is EncryptedData)
				{
					EncryptedData encryptedData = element as EncryptedData;
					System.Xml.XmlElement targetElement = encryptedData.TargetElement;										
							
					if ( SoapEnvelope.IsSoapBody(targetElement))
					{
						// The given context has the Body element Encrypted.
						encryptingToken = encryptedData.SecurityToken;
					}
				}
			}
			return encryptingToken;
		}

		/// <summary>
		/// This method checks if the incoming message contains a <see cref="UsernameToken"/>.
		/// </summary>
		/// <param name="context">The <see cref="SoapContext"/> to check.</param>
		/// <returns>The username token.</returns>
		public static UsernameToken GetUsernameToken(SoapContext context)
		{
			if (context == null)
				throw new Exception(
					"Only SOAP requests are permitted.");

			// Make sure there's a token
			if (context.Security.Tokens.Count == 0)
			{
				throw new SoapException("Missing security token", SoapException.ClientFaultCode);
			}
			else
			{
				foreach (UsernameToken tok in context.Security.Tokens)
					return tok;
				throw new Exception("UsernameToken not supplied");
			}
		}

	}
}
