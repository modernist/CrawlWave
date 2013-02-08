using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using CrawlWave.Common;

namespace CrawlWave.ServerCommon
{
	/// <summary>
	/// UrlDataProvider provides access to the urls' data to any class that needs it.
	/// </summary>
	public class UrlDataProvider
	{
		#region Private variables

		private static UrlDataProvider instance;
		private DBConnectionStringProvider dbProvider;
		private string connectionString;
		private SqlConnection dbcon;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private UrlDataProvider()
		{
			dbProvider = DBConnectionStringProvider.Instance();
			connectionString = dbProvider.ProvideDBConnectionString("CrawlWave.ServerCommon.UrlDataProvider");
			dbcon = new SqlConnection(connectionString);
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="UrlDataProvider"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="UrlDataProvider"/>.</returns>
		public static UrlDataProvider Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe.
				Mutex mutex=new Mutex();
				mutex.WaitOne();
				if( instance == null )
				{
					instance = new UrlDataProvider();
				}
				mutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Sets the Database Connection String that will be used by the class, in case the
		/// one provided by <see cref="DBConnectionStringProvider"/> can or must not be used.
		/// </summary>
		public string DBConnectionString
		{
			set
			{
				connectionString = value;
				DisconnectFromDatabase();
				if(dbcon != null)
				{
					dbcon.ConnectionString = connectionString; 
				}
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Provides the data of a Url given it's unique ID. It transparently undertakes the
		/// decompression of the data stored in the database for the given Url.
		/// </summary>
		/// <param name="urlID">The ID of the Url</param>
		/// <returns>The Url's data as a string, or an empty string if the operation fails.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if a negative value is given for Url ID.</exception>
		/// <exception cref="CWException">If the given Url's data cannot be found in the database.</exception>
		public string ProvideUrlData(int urlID)
		{
			if(urlID <= 0)
			{
				throw new ArgumentOutOfRangeException("urlID");
			}
			string retVal = string.Empty;
			try
			{
				if(!ConnectToDatabase())
				{
					if(!ConnectToDatabase())
					{
						throw new CWDBConnectionFailedException();
					}
				}
                SqlCommand cmd = new SqlCommand("cw_select_url_data", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@url_id", SqlDbType.Int);
				cmd.Parameters[0].Value = urlID;
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				if(ds.Tables[0].Rows.Count == 0)
				{
					ds.Dispose();
					throw new CWException("The data for the specified Url are not available.");
				}
				else
				{
                    DataRow dr = ds.Tables[0].Rows[0];
					byte [] data = (byte [])dr[1];
					int original_length = (int)dr[3];
					CompressionUtils.DecompressToString(data, out retVal);
					ds.Dispose();
					//if(retVal.Length != original_length)
					//{
					//	//log("Warning: Invalid string length of decompressed data.");
					//}
				}
			}
			catch(Exception e)
			{
				throw e;
			}
			finally
			{
				DisconnectFromDatabase();
			}
			return retVal;
		}

		#endregion

		#region Private methods

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
					dbcon = new SqlConnection(connectionString);
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
			catch
			{
				//if(settings.LogLevel <= CWLogLevel.LogError)
				//{
				//	settings.Log.LogError("CrawlWave Server failed to connect to the database: " + e.ToString());
				//}
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
			catch
			{
				//if(settings.LogLevel <= CWLogLevel.LogError)
				//{
				//	settings.Log.LogError("CrawlWave Server failed to disconnect from the database: " + e.ToString());
				//}
				return false;
			}
		}

		#endregion
	}
}
