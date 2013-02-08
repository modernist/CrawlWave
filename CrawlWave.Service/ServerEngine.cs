using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using System.Text;
using System.Threading;
using CrawlWave.Common;
using CrawlWave.ServerCommon;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

namespace CrawlWave.Service
{
	/// <summary>
	/// ServerEngine acts as a front-end to the system's database. It contains all the 
	/// methods that have some kind of interaction with the database so that they can be
	/// used by the <see cref="CrawlWaveServer"/> Web Service.
	/// </summary>
	public class ServerEngine
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
		public ServerEngine()
		{
			settings = ServerSettings.Instance();
			dbcon = null;
			ConnectToDatabase();
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Updates the computer hardware info related to a client.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client.</param>
		/// <param name="info">The <see cref="CWComputerInfo"/> of the client computer.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException StoreNewClientComputerInfo(ClientInfo ci, CWComputerInfo info)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				SqlCommand cmd = new SqlCommand("cw_update_client", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@client_id", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@user_id", SqlDbType.Int);
				cmd.Parameters.Add("@info_cpu", SqlDbType.NVarChar, 50);
				cmd.Parameters.Add("@info_ram", SqlDbType.SmallInt);
				cmd.Parameters.Add("@info_hdd", SqlDbType.Int);
				cmd.Parameters.Add("@info_net", SqlDbType.TinyInt);
				cmd.Parameters[0].Value = ci.ClientID;
				cmd.Parameters[1].Value = ci.UserID;
				cmd.Parameters[2].Value = info.CPUType;
				cmd.Parameters[3].Value = info.RAMSize;
				cmd.Parameters[4].Value = info.HDDSpace;
				cmd.Parameters[5].Value = (byte)info.ConnectionSpeed;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				if (!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("StoreNewClientComputerInfo failed: " + e.ToString());
				}
			}
			finally
			{
				LogClientAction(ci, CWClientActions.LogGetClientComputerInfo);
			}
			return sx;
		}

		/// <summary>
		/// Stores the results that the clients return after crawling a set of Urls.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client returning the data.</param>
		/// <param name="data">An array of <see cref="UrlCrawlData"/> objects containing the data of the crawled urls.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException StoreCrawlResults(ClientInfo ci, UrlCrawlData[] data)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				try
				{
					//store the new robots.txt files in the database, nothing else needs to
					//be done since the urls will be marked as not assigned when their data
					//is processed by DBUpdater
					if ((data != null) && (data.Length > 0))
					{
						SqlCommand cmd = new SqlCommand("cw_update_or_insert_robot", dbcon);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
						cmd.Parameters.Add("@disallowed", SqlDbType.NVarChar, 1000);
						foreach (UrlCrawlData urlData in data)
						{
							if ((urlData.FlagFetchRobots) || (urlData.Redirected))
							{
								string url = urlData.Url;
								cmd.Parameters[0].Value = new Guid(MD5Hash.md5(InternetUtils.HostName(url)));
								cmd.Parameters[1].Value = urlData.RobotsDisallowedPaths;
								try
								{
									cmd.ExecuteNonQuery();
								}
								catch
								{
									continue;
								}
							}
						}
						cmd.Dispose();
						SqlCommand statscmd = new SqlCommand("cw_update_client_statistics", dbcon);
						statscmd.CommandType = CommandType.StoredProcedure;
						statscmd.Parameters.Add("@client_id", SqlDbType.UniqueIdentifier);
						statscmd.Parameters.Add("@assigned", SqlDbType.BigInt);
						statscmd.Parameters.Add("@returned", SqlDbType.BigInt);
						statscmd.Parameters.Add("@type", SqlDbType.TinyInt);
						statscmd.Parameters[0].Value = ci.ClientID;
						statscmd.Parameters[1].Value = DBNull.Value;
						statscmd.Parameters[2].Value = data.Length;
						statscmd.Parameters[3].Value = 1;
						statscmd.ExecuteNonQuery();
						statscmd.Dispose();
					}
				}
				catch (Exception ex)
				{
					if (settings.LogLevel <= CWLogLevel.LogWarning)
					{
						settings.Log.LogWarning("StoreCrawlResults failed: " + ex.ToString());
					}
					throw ex;
				}
				finally
				{
					//save xml file on disk
					try
					{
						SaveXMLFile(ci, data);
					}
					catch (Exception se)
					{
						sx = new SerializedException(se.GetType().ToString(), se.Message, se.ToString());
						if (settings.LogLevel <= CWLogLevel.LogWarning)
						{
							settings.Log.LogWarning("StoreCrawlResults failed to save XML data on disk: " + se.ToString());
						}
					}
				}
				if (!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
			}
			finally
			{
				UpdateClientLastActive(ci);
				LogClientAction(ci, CWClientActions.LogGetCrawlResults);
			}
			return sx;
		}

		/// <summary>
		/// Performs the registration of a new client for a registered user of the system.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client that wishes to be
		/// registered to the system.</param>
		/// <param name="info">The <see cref="CWComputerInfo"/> of the computer running the
		/// client.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException StoreClientRegistrationInfo(ref ClientInfo ci, CWComputerInfo info)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				ci.ClientID = Guid.NewGuid();
				SqlCommand cmd = new SqlCommand("cw_insert_client", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@client_id", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@user_id", SqlDbType.Int);
				cmd.Parameters.Add("@info_cpu", SqlDbType.NVarChar, 50);
				cmd.Parameters.Add("@info_ram", SqlDbType.SmallInt);
				cmd.Parameters.Add("@info_hdd", SqlDbType.Int);
				cmd.Parameters.Add("@info_net", SqlDbType.TinyInt);
				cmd.Parameters[0].Value = ci.ClientID;
				cmd.Parameters[1].Value = ci.UserID;
				cmd.Parameters[2].Value = info.CPUType;
				cmd.Parameters[3].Value = info.RAMSize;
				cmd.Parameters[4].Value = info.HDDSpace;
				cmd.Parameters[5].Value = (byte)info.ConnectionSpeed;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				if (!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("StoreClientRegistrationInfo failed: " + e.ToString());
				}
			}
			finally
			{
				LogClientAction(ci, CWClientActions.LogRegisterClient);
			}
			return sx;
		}

		/// <summary>
		/// Performs the registration of a new user by storing his info in the database.
		/// </summary>
		/// <param name="ID">The ID that will be assigned to the new user, passed by reference.</param>
		/// <param name="username">The username requested from the new user.</param>
		/// <param name="password">The hash of the new user's password.</param>
		/// <param name="email">The user's email address.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException StoreUserRegistrationInfo(ref int ID, string username, byte[] password, string email)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				SqlCommand cmd = new SqlCommand("cw_insert_user", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@username", SqlDbType.NVarChar, 20);
				cmd.Parameters.Add("@password", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@email", SqlDbType.NVarChar, 50);
				cmd.Parameters.Add("@user_id", SqlDbType.Int);
				cmd.Parameters[3].Direction = ParameterDirection.ReturnValue;
				cmd.Parameters[0].Value = username;
				cmd.Parameters[1].Value = new Guid(password);
				cmd.Parameters[2].Value = email;
				try
				{
					cmd.ExecuteNonQuery();
					ID = (int)cmd.Parameters[3].Value;
					ClientInfo ci;
					ci.UserID = ID;
					ci.ClientID = Guid.Empty;
					ci.Version = "0.0.0.0";
					LogClientAction(ci, CWClientActions.LogRegisterUser);
				}
				catch (Exception se)
				{
					//the user already exists, throw an appropriate exception
					throw new CWUserExistsException("User registration failed: " + se.Message);
				}
				finally
				{
					cmd.Dispose();
					if (!DisconnectFromDatabase())
					{
						throw new CWDBConnectionFailedException("Disconnect from database failure.");
					}
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("StoreUserRegistrationInfo failed: " + e.ToString());
				}
			}
			return sx;
		}

		/// <summary>
		/// Selects and returns a list of all the banned hosts.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting the data.</param>
		/// <param name="data">A <see cref="DataSet"/> that will contain the list of banned hosts.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException SelectBannedHosts(ClientInfo ci, ref DataSet data)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				SqlCommand cmd = new SqlCommand("cw_select_banned_hosts", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				data = new DataSet();
				da.Fill(data);
				da.Dispose();
				cmd.Dispose();
				if (!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("SelectBannedHosts failed: " + e.ToString());
				}
			}
			finally
			{
				UpdateClientLastActive(ci);
				LogClientAction(ci, CWClientActions.LogSendBannedHosts);
			}
			return sx;
		}

		/// <summary>
		/// Selects and returns the latest version of the client updates available.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the call.</param>
		/// <param name="version">The latest version update available.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException SelectLatestVersion(ClientInfo ci, ref string version)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				//Load the values from the database
				SqlCommand cmd = new SqlCommand("cw_select_client_versions", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				Version latestVersion = new Version(0, 0, 0, 0);
				Version currentVersion;
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					try
					{
						currentVersion = new Version(((string)dr[0]).Trim());
						if (currentVersion > latestVersion)
						{
							latestVersion = currentVersion;
						}
					}
					catch
					{
						continue;
					}
				}
				ds.Dispose();
				version = latestVersion.ToString();
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("SelectBannedHosts failed: " + e.ToString());
				}
			}
			finally
			{
				UpdateClientLastActive(ci);
			}
			return sx;
		}

		/// <summary>
		/// Selects and returns a list of all the CrawlWave Servers.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting the data.</param>
		/// <param name="data">A <see cref="DataSet"/> that will contain the list of servers.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException SelectServers(ClientInfo ci, ref DataSet data)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				SqlCommand cmd = new SqlCommand("cw_select_servers", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				data = new DataSet();
				da.Fill(data);
				da.Dispose();
				cmd.Dispose();
				if (!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("SelectServers failed: " + e.ToString());
				}
			}
			finally
			{
				UpdateClientLastActive(ci);
				LogClientAction(ci, CWClientActions.LogSendServers);
			}
			return sx;
		}

		/// <summary>
		/// Selects and returns a byte array containing a Client Update version.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting the data.</param>
		/// <param name="version">The requested version.</param>
		/// <param name="data">A byte array that will contain the binary update file.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException SelectUpdatedVersion(ClientInfo ci, string version, byte[] data)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				SqlCommand cmd = new SqlCommand("cw_select_updated_version", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@version", SqlDbType.NChar, 15);
				cmd.Parameters[0].Value = version;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				if (!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
				if (ds.Tables[0].Rows.Count == 0)
				{
					throw new CWException("Version unavailable or not recognized.");
				}
				else
				{
					data = (byte[])ds.Tables[0].Rows[0][0];
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("SelectUpdatedVersion failed: " + e.ToString());
				}
			}
			finally
			{
				UpdateClientLastActive(ci);
				LogClientAction(ci, CWClientActions.LogSendUpdatedVersion);
			}
			return sx;
		}

		/// <summary>
		/// Selects and returns a set of urls that are ready to be crawled.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting urls to crawl.</param>
		/// <param name="data">An array of <see cref="InternetUrlToCrawl"/> objects containing the selected urls.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException SelectUrlsToCrawl(ClientInfo ci, ref InternetUrlToCrawl[] data)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				//we must use a transaction to make sure that if something goes wrong the
				//changes to the database will be rolled back.
				SqlTransaction transaction = dbcon.BeginTransaction(IsolationLevel.Serializable);//perhaps | repeatableread
				try
				{
					//first select the urls to crawl
					SqlCommand cmd = new SqlCommand("cw_select_urls_to_crawl", dbcon, transaction);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 120;
					SqlDataAdapter da = new SqlDataAdapter(cmd);
					DataSet ds = new DataSet();
					da.Fill(ds);
					da.Dispose();
					cmd.Dispose();
					//now delete them from the table of urls to crawl
					data = new InternetUrlToCrawl[ds.Tables[0].Rows.Count];
					if (data.Length > 0)
					{
						int i = 0;
						foreach (DataRow dr in ds.Tables[0].Rows)
						{
							try
							{
								InternetUrlToCrawl url = new InternetUrlToCrawl((int)dr[0], (string)dr[1]);
								if (dr[2] != DBNull.Value)
								{
									url.CRC = (long)dr[2];
								}
								if (dr[3] != DBNull.Value)
								{
									url.FlagDomain = (DomainFlagValue)((byte)dr[3]);
								}
								if (dr[4] != DBNull.Value)
								{
									url.RobotsDisallowedPaths = (string)dr[4];
								}
								else
								{
									RobotsTxtEntry entry = settings.Robots.GetEntry(InternetUtils.HostName(url));
									if (entry != null)
									{
										url.RobotsDisallowedPaths = ConcatenatePaths(entry.DisallowedPaths);
									}
									else
									{
										url.FlagFetchRobots = true;
									}
								}
								data[i++] = url;
							}
							catch
							{
								continue;
							}
						}
						SqlCommand statscmd = new SqlCommand("cw_update_client_statistics", dbcon, transaction);
						statscmd.CommandType = CommandType.StoredProcedure;
						statscmd.CommandTimeout = 120;
						statscmd.Parameters.Add("@client_id", SqlDbType.UniqueIdentifier);
						statscmd.Parameters.Add("@assigned", SqlDbType.BigInt);
						statscmd.Parameters.Add("@returned", SqlDbType.BigInt);
						statscmd.Parameters.Add("@type", SqlDbType.TinyInt);
						statscmd.Parameters[0].Value = ci.ClientID;
						statscmd.Parameters[1].Value = data.Length;
						statscmd.Parameters[2].Value = DBNull.Value;
						statscmd.Parameters[3].Value = 0;
						statscmd.ExecuteNonQuery();
						statscmd.Dispose();
						transaction.Commit();
					}
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					if (settings.LogLevel <= CWLogLevel.LogWarning)
					{
						settings.Log.LogWarning("SelectUrlsToCrawl failed, Transaction was rolled back: " + ex.ToString());
					}
					throw ex;
				}
				finally
				{
					UpdateClientLastActive(ci);
					LogClientAction(ci, CWClientActions.LogSendUrlsToCrawl);
					if (!DisconnectFromDatabase())
					{
						throw new CWDBConnectionFailedException("Disconnect from database failure.");
					}
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("SelectUrlsToCrawl failed: " + e.ToString());
				}
			}
			return sx;
		}

		/// <summary>
		/// Selects and returns the statistics for a certain user.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client requesting the statistics.</param>
		/// <param name="stats">The <see cref="UserStatistics"/> of the user.</param>
		/// <returns>Null if the operation succeeds, or <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException SelectUserStatistics(ClientInfo ci, ref UserStatistics stats)
		{
			SerializedException sx = null;
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				SqlCommand cmd = new SqlCommand("cw_select_user_statistic", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@user_id", SqlDbType.Int);
				cmd.Parameters[0].Value = ci.UserID;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				if (ds.Tables[0].Rows.Count > 0)
				{
					stats.RegistrationDate = (DateTime)ds.Tables[0].Rows[0][2];
					stats.LastActive = (DateTime)ds.Tables[0].Rows[0][6];

					foreach (DataRow dr in ds.Tables[0].Rows)
					{
						stats.NumClients++;
						stats.UrlsAssigned += (long)dr[4];
						stats.UrlsReturned += (long)dr[5];
						DateTime la = (DateTime)dr[6];
						if (la > stats.LastActive)
						{
							stats.LastActive = la;
						}
					}
				}
				ds.Dispose();
				if (!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
			}
			catch (Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.ToString());
				if (settings.LogLevel <= CWLogLevel.LogWarning)
				{
					settings.Log.LogWarning("SelectUserStatistics failed for user " + ci.UserID.ToString() + ":" + e.ToString());
				}
			}
			finally
			{
				UpdateClientLastActive(ci);
				LogClientAction(ci, CWClientActions.LogSendUserStatistics);
			}
			return sx;
		}

		/// <summary>
		/// Logs a client's activity in the Client History table of the system's database.
		/// It silently ignores errors.
		/// </summary>
		/// <param name="ci">The <see cref="ClientInfo"/> of the client performing the action.</param>
		/// <param name="action">The <see cref="CWClientActions"/> action performed by the client.</param>
		public void LogClientAction(ClientInfo ci, CWClientActions action)
		{
			try
			{
				if ((settings.LogOptions & action) == action)
				{
					if (!ConnectToDatabase())
					{
						throw new CWDBConnectionFailedException();
					}
					SqlCommand cmd = new SqlCommand("cw_insert_client_history", dbcon);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@client_id", SqlDbType.UniqueIdentifier);
					cmd.Parameters.Add("@event_type", SqlDbType.Int);
					cmd.Parameters[0].Value = ci.ClientID;
					cmd.Parameters[1].Value = (int)action;
					cmd.ExecuteNonQuery();
					cmd.Dispose();
					if (!DisconnectFromDatabase())
					{
						throw new CWDBConnectionFailedException("Disconnect from database failure.");
					}
				}
			}
			catch
			{ }
		}

		/// <summary>
		/// Updates a client's last activity date in the system's database.
		/// </summary>
		/// <param name="ci">The info of the client performing an action.</param>
		public void UpdateClientLastActive(ClientInfo ci)
		{
			try
			{
				if (!ConnectToDatabase())
				{
					throw new CWDBConnectionFailedException();
				}
				SqlCommand cmd = new SqlCommand("cw_update_client_last_active", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@client_id", SqlDbType.UniqueIdentifier);
				cmd.Parameters[0].Value = ci.ClientID;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				if (!DisconnectFromDatabase())
				{
					throw new CWDBConnectionFailedException("Disconnect from database failure.");
				}
			}
			catch
			{ }
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
				if (dbcon == null)
				{
					dbcon = new SqlConnection(settings.ProvideConnectionString());
					dbcon.Open();
				}
				else
				{
					if (dbcon.State != ConnectionState.Open)
					{
						dbcon.Open();
					}
				}
				return true;
			}
			catch (Exception e)
			{
				if (settings.LogLevel <= CWLogLevel.LogError)
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
				if (dbcon != null)
				{
					if (dbcon.State != ConnectionState.Closed)
					{
						dbcon.Close();
					}
				}
				return true;
			}
			catch (Exception e)
			{
				if (settings.LogLevel <= CWLogLevel.LogError)
				{
					settings.Log.LogError("CrawlWave Server failed to disconnect from the database: " + e.ToString());
				}
				return false;
			}
		}

		/// <summary>
		/// Stores an array of <see cref="UrlCrawlData"/> objects and the <see cref="ClientInfo"/>
		/// of the client who returned them on a compressed file on disk.
		/// </summary>
		/// <param name="info">The <see cref="ClientInfo"/> of the client who returned the data.</param>
		/// <param name="data">An array of <see cref="UrlCrawlData"/> objects containing the
		/// data returned by the client.</param>
		private void SaveXMLFile(ClientInfo info, UrlCrawlData[] data)
		{
			UrlCrawlDataFile udf = new UrlCrawlDataFile(info, data);
			string id = Guid.NewGuid().ToString();
			//serialize the object into a memory stream
			MemoryStream ms = new MemoryStream();
			//this may need to use SoapFormatter
			//XmlSerializer xml = new XmlSerializer(typeof(UrlCrawlDataFile));
			SoapFormatter xml = new SoapFormatter();
			xml.Serialize(ms, udf);
			byte[] buffer = ms.ToArray();
			ms.Close();
			string fileName = settings.DataFilesPath + id + ".zip";
			Crc32 crc = new Crc32();
			ZipOutputStream zs = new ZipOutputStream(File.Create(fileName));
			ZipEntry entry = new ZipEntry(id);
			entry.DateTime = DateTime.Now;
			entry.Size = buffer.Length;
			crc.Update(buffer);
			entry.Crc = crc.Value;
			zs.PutNextEntry(entry);
			zs.Write(buffer, 0, buffer.Length);
			zs.Finish();
			zs.Close();
		}

		/// <summary>
		/// Concatenates an array of strings each containing a disallowed path into a string
		/// separated by spaces.
		/// </summary>
		/// <param name="paths">An array of strings each containing a disallowed path.</param>
		/// <returns>A string containing all the paths concatenated and separated by spaces.</returns>
		private string ConcatenatePaths(string[] paths)
		{
			if ((paths == null) || (paths.Length == 0))
			{
				return String.Empty;
			}
			else
			{
				StringBuilder sb = new StringBuilder(paths[0]);
				for (int i = 1; i < paths.Length; i++)
				{
					sb.Append(' ');
					sb.Append(paths[i]);
				}
				return sb.ToString();
			}
		}

		#endregion
	}
}
