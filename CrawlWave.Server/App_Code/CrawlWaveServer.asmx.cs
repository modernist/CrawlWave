using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

namespace CrawlWave.Server
{
	/// <summary>
	/// Summary description for CrawlWaveServer.
	/// </summary>
	public class CrawlWaveServer : System.Web.Services.WebService
	{
		/// <summary>
		/// 
		/// </summary>
		public CrawlWaveServer()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		private void Authenticate(SoapContext context)
		{
			// Reject any requests which are not valid SOAP requests
			if (RequestSoapContext.Current == null)
				throw new ApplicationException("Only SOAP requests are permitted.");

			// Check if the soap message contains all the required message parts
			SecurityUtils.VerifyMessageParts(context);

			// Check if the Soap Message is Signed.
			UsernameToken usernameToken = SecurityUtils.GetSigningToken(context) as UsernameToken;
			if (usernameToken == null || usernameToken.PasswordOption == PasswordOption.SendHashed)
			{
				throw new SecurityFault(SecurityFault.FailedAuthenticationMessage, SecurityFault.FailedAuthenticationCode);
			}
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[WebMethod]
		public bool IsAlive()
		{
			//Authenticate(RequestSoapContext.Current);
			return true;
		}

		/// <summary>
		/// Updates the computer hardware info related to a client.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client.</param>
		/// <param name="info">The <see cref="CWComputerInfo"/> of the client computer.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <remarks>
		/// This method actually calls <see cref="ServerEngine.StoreNewClientComputerInfo"/>.
		/// All requests to this method are authenticated by <see cref="Authentication"/>.
		/// </remarks>
		[WebMethod]
		public SerializedException GetClientComputerInfo(ClientInfo ci, CWComputerInfo info)
		{
			//Authenticate(RequestSoapContext.Current);
			try
			{
				ServerEngine engine = new ServerEngine();
				engine.LogClientAction(ci, CWClientActions.LogGetClientComputerInfo);
				return engine.StoreNewClientComputerInfo(ci, info);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Stores the results that the clients return after crawling a set of Urls.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client returning the data.</param>
		/// <param name="data">An array of <see cref="UrlCrawlData"/> objects containing the data of the crawled urls.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <remarks>
		/// <para>This method actually calls <see cref="ServerEngine.StoreCrawlResults"/>.
		/// All requests to this method are authenticated by the <see cref="Authentication"/>
		/// class.</para>
		/// <para>
		/// This method does not actually update the database with the data of the crawled
		/// urls, since that is a very time-consuming process and the timeouts that occur
		/// would slow down the clients. Also this might lead to 'losing' urls because the
		/// database server will have to deal with great load if a big number of clients is
		/// returning data all the time. To avoid these problems the server simply updates
		/// the database with the fresh robots.txt files fetched by the clients and stores
		/// the data in a compressed zip file on disk. This allows their processing and the
		/// update of the database with the data of the urls and the links they contain to
		/// be performed at a later time by another process (the DBUpdater plugin).
		/// </para>
		/// </remarks>
		[XmlInclude(typeof(CrawlWave.Common.InternetUrlToIndex))]
		[WebMethod]
		public SerializedException GetCrawlResults(ClientInfo ci, UrlCrawlData [] data)
		{
			//Authenticate(RequestSoapContext.Current);
			try
			{
				ServerEngine engine = new ServerEngine();
				engine.LogClientAction(ci, CWClientActions.LogGetCrawlResults);
				return engine.StoreCrawlResults(ci, data);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Stores the results that the clients return after crawling a set of Urls.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client returning the data.</param>
		/// <param name="data">An array of bytes containing the data of the crawled urls as 
		/// an array of <see cref="UrlCrawlData"/> objects in serialized form.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <remarks>
		/// <para>This method actually calls <see cref="ServerEngine.StoreCrawlResults"/>.
		/// All requests to this method are authenticated by the <see cref="Authentication"/>
		/// class.</para>
		/// <para>
		/// This method does not actually update the database with the data of the crawled
		/// urls, since that is a very time-consuming process and the timeouts that occur
		/// would slow down the clients. Also this might lead to 'losing' urls because the
		/// database server will have to deal with great load if a big number of clients is
		/// returning data all the time. To avoid these problems the server simply updates
		/// the database with the fresh robots.txt files fetched by the clients and stores
		/// the data in a compressed zip file on disk. This allows their processing and the
		/// update of the database with the data of the urls and the links they contain to
		/// be performed at a later time by another process (the DBUpdater plugin).
		/// </para>
		/// <para>
		/// <b>Attention: </b> This method is slightly less efficient than its other overload
		/// and must only be used in special circumstances, e.g. when a SOAP Formatting error
		/// occurs during an attempt to return results.
		/// </para>
		/// </remarks>
		[WebMethod]
		public SerializedException GetCrawlResultsRaw(ClientInfo ci, byte [] data)
		{
			//Authenticate(RequestSoapContext.Current);
			try
			{
				ServerEngine engine = new ServerEngine();
				engine.LogClientAction(ci, CWClientActions.LogGetCrawlResults);
				return engine.StoreCrawlResults(ci, data);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Performs the registration of a new client for a registered user of the system.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> containing the information of the
		/// client that wishes to be registered to the system.</param>
		/// <param name="info">The <see cref="CWComputerInfo"/> containing information about
		/// the hardware of the computer running the client.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <remarks>
		/// The ClientInfo argument is passed by reference. After the registration process
		/// is complete the argument will be updated with the ID that has been assigned to
		/// the new client.
		/// </remarks>
		[WebMethod]
		public SerializedException RegisterClient(ref ClientInfo ci, CWComputerInfo info)
		{
			//This method will be called from a registered user, so it must be authenticated
			//Authenticate(RequestSoapContext.Current);
			try
			{
				ServerEngine engine = new ServerEngine();
				SerializedException sx = engine.StoreClientRegistrationInfo(ref ci, info);
				engine.LogClientAction(ci, CWClientActions.LogRegisterClient);
				return sx;
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Performs the registration of a new user to the system.
		/// </summary>
		/// <param name="ID">The ID that will be assigned to the new user, passed by reference.</param>
		/// <param name="username">The username requested from the new user.</param>
		/// <param name="password">The hash of the new user's password.</param>
		/// <param name="email">The user's email address.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <remarks>
		/// <para>This method actually calls <see cref="ServerEngine.StoreUserRegistrationInfo"/>.</para>
		/// <para>This method will not be called using WSE 2.0 (no authentication of the 
		/// requests will be performed) because when a client calls it his username will not
		/// have been assigned yet. It must be called by a non-WSE enabled proxy.</para>
		/// </remarks>
		[WebMethod]
		public SerializedException RegisterUser(ref int ID, string username, byte [] password, string email)
		{
			//This method will be called from an unregistered user, so it must not be authenticated
			try
			{
				ID = 0;
				ServerEngine engine = new ServerEngine();
				return engine.StoreUserRegistrationInfo(ref ID, username, password, email);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Selects and returns a list of all the banned hosts.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="data">A <see cref="DataSet"/> that will contain the list of banned hosts.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		[WebMethod]
		public SerializedException SendBannedHosts(ClientInfo ci, out DataSet data)
		{
			//This method will be called from a registered user, so it must be authenticated
			//Authenticate(RequestSoapContext.Current);
			data = null;
			try
			{
				ServerEngine engine = new ServerEngine();
				engine.LogClientAction(ci, CWClientActions.LogSendBannedHosts);
				return engine.SelectBannedHosts(ci, ref data);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Selects and returns the latest version of the client updates available.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="version">The latest version update available.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		[WebMethod]
		public SerializedException SendLatestVersion(ClientInfo ci, out string version)
		{
			//This method will be called from a registered user, so it must be authenticated
			//Authenticate(RequestSoapContext.Current);
			version = String.Empty;
			try
			{
				ServerEngine engine = new ServerEngine();
				return engine.SelectLatestVersion(ci, ref version);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Selects and returns a list of all the servers available.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="data">A <see cref="DataSet"/> that will contain the list of servers.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		[WebMethod]
		public SerializedException SendServers(ClientInfo ci, out DataSet data)
		{
			//This method will be called from a registered user, so it must be authenticated
			//Authenticate(RequestSoapContext.Current);
			data = null;
			try
			{
				ServerEngine engine = new ServerEngine();
				engine.LogClientAction(ci, CWClientActions.LogSendServers);
				return engine.SelectBannedHosts(ci, ref data);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Selects and returns the requested client update version.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="version">The requested version.</param>
		/// <param name="data">A <see cref="DataSet"/> that will contain the list of servers.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		[WebMethod]
		public SerializedException SendUpdatedVersion(ClientInfo ci, string version, out byte [] data)
		{
			//This method will be called from a registered user, so it must be authenticated
			//Authenticate(RequestSoapContext.Current);
			data = null;
			try
			{
				ServerEngine engine = new ServerEngine();
				engine.LogClientAction(ci, CWClientActions.LogSendUpdatedVersion);
				return engine.SelectUpdatedVersion(ci, version, data);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Selects and returns a set of urls that are ready to be crawled.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting urls to crawl.</param>
		/// <param name="data">An array of <see cref="InternetUrlToCrawl"/> objects containing the selected urls.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		[WebMethod]
		public SerializedException SendUrlsToCrawl(ClientInfo ci, out InternetUrlToCrawl [] data)
		{
			//This method will be called from a registered user, so it must be authenticated
			//Authenticate(RequestSoapContext.Current);
			data = null;
			try
			{
				ServerEngine engine = new ServerEngine();
				engine.LogClientAction(ci, CWClientActions.LogSendUrlsToCrawl);
				return engine.SelectUrlsToCrawl(ci, ref data);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}

		/// <summary>
		/// Selects and returns the statistics for a certain user.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting the statistics.</param>
		/// <param name="stats">The <see cref="UserStatistics"/> of the user.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		[WebMethod]
		public SerializedException SendUserStatistics(ClientInfo ci, out UserStatistics stats)
		{
			//This method will be called from a registered user, so it must be authenticated
			//Authenticate(RequestSoapContext.Current);
			stats = new UserStatistics();
			try
			{
				ServerEngine engine = new ServerEngine();
				engine.LogClientAction(ci, CWClientActions.LogSendUserStatistics);
				return engine.SelectUserStatistics(ci, ref stats);
			}
			catch(Exception e)
			{
				return new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
		}
	}
}
