using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using CrawlWave.Common;
using CrawlWave.ServerCommon;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace CrawlWave.ServerPlugins.DBUpdater
{
	/// <summary>
	/// DBUpdaterPlugin is an <see cref="IPlugin"/> that performs the task of updating the
	/// system's database with the data returned by the clients after crawling urls. It can
	/// run while the crawling process is active or it can be enabled at a later time, thus
	/// updating the database in an asynchronous batch mode.
	/// </summary>
	[CrawlWavePlugin]
	public class DBUpdaterPlugin : PluginBase, IPlugin
	{
		#region Private variables
		
		private PluginSettings settings;
		private SqlConnection dbcon;
		private Thread pluginThread;
		private BannedHostsCache banned;
		private RobotsCache robots;
		private Random random;
		private bool mustStop;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructs a new instance of the <see cref="CrawlWave.ServerPlugins.DBUpdater.DBUpdaterPlugin"/> class.
		/// </summary>
		public DBUpdaterPlugin()
		{
			dataDependent = true;
			description = "CrawlWave Database Updater Plugin";
			enabled = true;
			host = null;
			name = "CrawlWave.ServerPlugins.DBUpdater";
			percent = 0;
			settingsPath = String.Empty;
			state = PluginState.Stopped;
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			settings = PluginSettings.Instance();
			dbcon = new SqlConnection(settings.DBConnectionString);
			pluginThread = null;
			random = new Random();
			mustStop = false;
			banned = BannedHostsCache.Instance();
			robots = RobotsCache.Instance();
		}

		#endregion

		#region IPlugin Members

		/// <summary>
		/// Starts the process of the database update. If the plugin is  already in the 
		/// <see cref="PluginState.Running"/> state then it has no effect.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the plugin is in the <see cref="PluginState.Paused"/> state.
		/// </exception>
		public void Start()
		{
			if(state == PluginState.Running)
			{
				return;
			}
			if(state == PluginState.Paused)
			{
				throw new InvalidOperationException("The plugin is in the Paused state and cannot be started.");
			}
			try
			{
				mustStop = false;
				AddToReportQueue(CWLoggerEntryType.Info, "Starting CrawlWave DBUpdater Plugin.");
				//create the thread that will be updating the databse.
				pluginThread = new Thread(new ThreadStart(PerformUpdate));
				pluginThread.IsBackground = true;
				pluginThread.Priority = ThreadPriority.Lowest;
				pluginThread.Name = "Database Updater Plugin Thread";
				pluginThread.Start();
				//Notify the clients that the plugin's process has started
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Error, "The DBUpdater Plugin failed to start: " + ex.ToString());
				mustStop = true;
				state = PluginState.Stopped;
				try
				{
					StopThreads();
				}
				catch(Exception exc)
				{
					ReportImmediately(CWLoggerEntryType.Error, "The DBUpdater Plugin failed to stop all threads: " + exc.ToString());
				}
			}
			finally
			{
				Report();
			}
		}

		/// <summary>
		/// Pauses the database update process by calling <see cref="Thread.Suspend"/> on the 
		/// running threads. If the plugin is already in the <see cref="PluginState.Paused"/>
		/// state it has no effect.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the plugin is in the <see cref="PluginState.Stopped"/> state.
		/// </exception>
		public void Pause()
		{
			if(state == PluginState.Paused)
			{
				return;
			}
			if(state == PluginState.Stopped)
			{
				throw new InvalidOperationException("The plugin is in the Stopped state and cannot be paused.");
			}
			try
			{
				state = PluginState.Paused;
				SuspendThreads();
				OnStateChanged(EventArgs.Empty);
			}
			catch
			{
				//if something goes wrong return to the Running state
				state = PluginState.Running;
			}
			finally
			{
				//hmmm....
			}
		}

		/// <summary>
		/// Resumes the database update process if it has been paused.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the plugin is in the <see cref="PluginState.Stopped"/> or 
		/// <see cref="PluginState.Running"/> state.
		/// </exception>
		public void Resume()
		{
			if((state == PluginState.Stopped)||(state == PluginState.Running))
			{
				throw new InvalidOperationException("The plugin is not in the Paused state and cannot be resumed.");
			}
			try
			{
				state = PluginState.Running;
				ResumeThreads();
				OnStateChanged(EventArgs.Empty);
			}
			catch
			{
				//if something goes wrong return to the Paused state
				state = PluginState.Paused;
			}
			finally
			{
				//hmmm....
			}
		}

		/// <summary>
		/// Stops the database update process. If the plugin is already in the <see cref="PluginState.Stopped"/>
		/// state it has no effect. If the update process is in progress it is not stopped
		/// abruptly but the method waits until the selection of urls is complete.
		/// </summary>
		public void Stop()
		{
			if(state == PluginState.Paused)
			{
				throw new InvalidOperationException("The plugin is in the Paused state and cannot be stopped.");
			}
			try
			{
				mustStop = true;
				AddToReportQueue(CWLoggerEntryType.Info, "Stopping DBUpdater plugin.");
				//wait for all the threads to finish
				if(pluginThread!=null)
				{
					while(pluginThread.IsAlive)
					{
						Thread.Sleep(1000);
					}
				}
				//Stop all threads
				StopThreads();
				state = PluginState.Stopped;
				//notify the other classes
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Warning, "The DBUpdater plugin failed to stop: " + ex.ToString());
			}
			finally
			{
				Report();
				mustStop = false;
			}
		}

		/// <summary>
		/// Displays a form with the plugin's settings.
		/// </summary>
		public void ShowSettings()
		{
			frmPluginSettings frm = new frmPluginSettings();
			frm.Show();
		}

		/// <summary>
		/// Occurs when the plugin's <see cref="PluginState"/> changes.
		/// </summary>
		public event System.EventHandler StateChanged;

		#endregion

		#region Private Methods

		/// <summary>
		/// Performs the database update. It is running on pluginThread, so it must be able
		/// to handle ThreadAbortException and ThreadInterruptedException.
		/// </summary>
		private void PerformUpdate()
		{
			try
			{
				ReportImmediately(CWLoggerEntryType.Info, "CrawlWave DBUpdater Plugin thread has started with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4"));
				while(!mustStop)
				{
					try
					{
						//Select one of the data files to process
						string fileName = SelectDataFileName();
						if(fileName!=String.Empty)
						{
							//Open a data file and extract the information it contains
							UrlCrawlDataFile udf = LoadDataFile(fileName);
							if(udf != null)
							{
								try
								{
									dbcon.Open();
								}
								catch(Exception e)
								{
									ReportImmediately(CWLoggerEntryType.Error, "DBUpdater plugin failed to connect to the database:" + e.ToString());
									throw e; //this will cause the plugin to pause if necessary
								}
								AddToReportQueue(CWLoggerEntryType.Info, "DBUpdater processing file " + fileName);
								//process each UrlCrawlData
								foreach(UrlCrawlData data in udf.Data)
								{
									SqlTransaction transaction = null;
									if(settings.UseTransactions)
									{
										transaction = dbcon.BeginTransaction();
									}
									int UrlID = 0;
									try
									{
										UrlID = UpdateUrl(data, transaction);
										if(UrlID!=0)
										{
											if(data.UrlToCrawl.FlagDomain==DomainFlagValue.MustVisit)
											{
												ClearUrlOutLinks(UrlID, transaction);
												InsertUrlOutLinks(UrlID, data, transaction);
											}
											if(settings.UseTransactions)
											{
												transaction.Commit();
											}
										}
										else
										{
											if(settings.UseTransactions)
											{
												transaction.Rollback();
											}
										}
									}
									catch(ThreadInterruptedException tie)
									{
										if(settings.UseTransactions)
										{
											transaction.Rollback();
										}
										throw tie;
									}
									finally
									{
										if(settings.UseTransactions)
										{
											transaction.Dispose();
										}
									}
								}
								//if everything succeeds delete the file
								File.Delete(fileName);
								try
								{
									if(dbcon!=null)
									{
										if(dbcon.State != ConnectionState.Closed)
										{
											dbcon.Close();
										}
									}
								}
								catch(Exception dce)
								{
									ReportImmediately(CWLoggerEntryType.Error, "DBUpdater Plugin failed to close the connection to the database: " + dce.ToString());
								}
							}
						}
					}
					catch(Exception e)
					{
						AddToReportQueue(CWLoggerEntryType.Warning, "DBUpdater encountered an unexpected exception: " + e.Message);
					}
					finally
					{
						Report();
						//pause if necessary
						if(settings.PauseBetweenOperations)
						{
							int waitSeconds = PauseInSeconds();
							for(int i =0; i< waitSeconds; i++)
							{
								Thread.Sleep(1000);
								if(mustStop)
								{
									break;
								}
							}
						}
					}
					GC.Collect();
				}
			}
			catch(ThreadAbortException)
			{
				//The thread was asked to abort, which means it must return at once
				return;
			}
			catch(ThreadInterruptedException)
			{
				//The thread has been asked to Join. We have nothing to do but return.
				return;
			}
			finally
			{
				AddToReportQueue(CWLoggerEntryType.Info, Thread.CurrentThread.Name + " has stopped.");
			}
		}

		/// <summary>
		/// Loads a data file from the disk, decompresses it and extracts the <see cref="UrlCrawlDataFile"/>
		/// it contains.
		/// </summary>
		/// <param name="fileName">The name of the file to load.</param>
		/// <returns>The <see cref="UrlCrawlDataFile"/> contained in the file or null if
		/// something goes wrong.</returns>
		private UrlCrawlDataFile LoadDataFile(string fileName)
		{
			UrlCrawlDataFile retVal = null;
			try
			{
				ZipInputStream zs = new ZipInputStream(File.Open(fileName, FileMode.Open));
				ZipEntry entry = zs.GetNextEntry();
				if(entry!=null)
				{
					MemoryStream ms = new MemoryStream();
					int size = 4096;
					byte [] data = new byte[4096];
					while(true)
					{
						size = zs.Read(data, 0, data.Length);
						if (size > 0) 
						{
							ms.Write(data, 0, size);
						}
						else 
						{
							break;
						}
					}
					ms.Position = 0;
					SoapFormatter xml = new SoapFormatter();
					retVal = (UrlCrawlDataFile)xml.Deserialize(ms);
					ms.Close();
				}
				zs.Close();
			}
			catch(Exception e)
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, "The DBUpdater plugin failed to load file " + fileName + ":" + e.ToString()));
			}
			return retVal;
		}

		/// <summary>
		/// Selects a file name of one of the available data files in order to process it.
		/// </summary>
		/// <returns>A string containing one of the available data files name.</returns>
		private string SelectDataFileName()
		{
			string retVal=String.Empty;
			//try to get the directory entries
			try
			{
				//the files we want have a guid as a filename, thus occurs the following pattern
				string []Files=Directory.GetFiles(settings.DataPath,"????????-????-????-????-????????????.zip");
				if (Files.Length>0)
				{
					retVal=Files[random.Next(Files.Length)];
				}
			}
			catch(Exception e)
			{
				//something went wrong - this will cause the method to return an empty string
				AddToReportQueue(CWLoggerEntryType.Warning, "The DBUpdater failed to select a file to process: " + e.ToString());
			}
			return retVal;
		}

		/// <summary>
		/// Updates the Url and the Url Data tables
		/// </summary>
		/// <param name="data">The UrlCrawlData containing the data of the crawled Url.</param>
		/// <param name="transaction">The currently active <see cref="SqlTransaction"/>.</param>
		/// <returns>The ID of the updated url or 0 of something goes wrong.</returns>
		private int UpdateUrl(UrlCrawlData data, SqlTransaction transaction)
		{
			int retVal = 0;
			try
			{
				//build the Sql Command for updating the url table
				SqlCommand urlcmd = new SqlCommand("cw_update_url", dbcon, transaction);
				urlcmd.CommandType = CommandType.StoredProcedure;
				urlcmd.CommandTimeout = settings.DBActionTimeout;
				urlcmd.Parameters.Add("@url_id",SqlDbType.Int);
				urlcmd.Parameters.Add("@url", SqlDbType.NVarChar, 500);
				urlcmd.Parameters.Add("@url_md5", SqlDbType.UniqueIdentifier);
				urlcmd.Parameters.Add("@url_host_id", SqlDbType.UniqueIdentifier);
				urlcmd.Parameters.Add("@url_priority", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@crc", SqlDbType.BigInt);
				urlcmd.Parameters.Add("@flag_domain", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@flag_robots", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@flag_updated", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@last_visited", SqlDbType.SmallDateTime);
				urlcmd.Parameters.Add("@flag_redirected", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@id", SqlDbType.Int);
				urlcmd.Parameters["@id"].Direction = ParameterDirection.Output;

				//Build the SQL Command for updating the hosts table
				SqlCommand hostcmd = new SqlCommand("cw_insert_host", dbcon, transaction);
				hostcmd.CommandType = CommandType.StoredProcedure;
				hostcmd.CommandTimeout = settings.DBActionTimeout;
				hostcmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
				hostcmd.Parameters.Add("@host_name", SqlDbType.NVarChar, 100);

				//set their parameters
				urlcmd.Parameters[0].Value = data.ID;
				urlcmd.Parameters[1].Value = data.Url;
				urlcmd.Parameters[2].Value = new Guid(data.MD5);
				Uri uri = new Uri(data.Url);
				string host_name = uri.Host;
				Guid host_id = new Guid(MD5Hash.md5(host_name));
				urlcmd.Parameters[3].Value = host_id;
				urlcmd.Parameters[5].Value = data.CRC;
				if(data.Redirected)
				{
					//we must first attempt to insert the host, otherwise the urlcmd will fail
					hostcmd.Parameters[0].Value = host_id;
					hostcmd.Parameters[1].Value = host_name;
					try
					{
						hostcmd.ExecuteNonQuery();
					}
					catch
					{
						//it probably exists already
					}

					urlcmd.Parameters[4].Value = (byte)data.RedirectedPriority;
					urlcmd.Parameters[6].Value = (byte)data.RedirectedFlagDomain;
					urlcmd.Parameters[7].Value = (data.RedirectedFlagRobots)?1:0;
					urlcmd.Parameters[8].Value = (data.Updated)?1:0;
					urlcmd.Parameters[9].Value = data.TimeStamp;
					urlcmd.Parameters[10].Value = 1;
				}
				else
				{
					urlcmd.Parameters[4].Value = DBNull.Value;
					urlcmd.Parameters[6].Value = (byte)data.UrlToCrawl.FlagDomain;
					if(data.FlagFetchRobots)
					{
						urlcmd.Parameters[7].Value = (data.RedirectedFlagRobots)?1:0;
					}
					else
					{
						urlcmd.Parameters[7].Value = 0;
					}
					urlcmd.Parameters[8].Value = (data.Updated)?1:0;
					urlcmd.Parameters[9].Value = data.TimeStamp;
					urlcmd.Parameters[10].Value = 0;
				}
				//retVal = data.ID;
				//make sure the host command is disposed
				hostcmd.Dispose();
				urlcmd.ExecuteNonQuery();
				retVal = (int)urlcmd.Parameters["@id"].Value;
				urlcmd.Dispose();
	
				if(data.Updated)
				{
					//if necessary build the sql command for updating the url data tables
					SqlCommand urldatacmd = new SqlCommand("cw_update_url_data", dbcon, transaction);
					urldatacmd.CommandType = CommandType.StoredProcedure;
					urldatacmd.CommandTimeout = settings.DBActionTimeout;
					urldatacmd.Parameters.Add("@url_id", SqlDbType.Int);
					urldatacmd.Parameters.Add("@data", SqlDbType.Image);
					urldatacmd.Parameters.Add("@length", SqlDbType.Int);
					urldatacmd.Parameters.Add("@original_length", SqlDbType.Int);
					urldatacmd.Parameters.Add("@http_code", SqlDbType.SmallInt);
					urldatacmd.Parameters.Add("@retrieval_time", SqlDbType.Int);

					urldatacmd.Parameters[0].Value = retVal; 
					//compress the url's data
					if(data.Data!= String.Empty)
					{
						byte [] compressed = null;
						string urldata = InternetUtils.Base64Decode(data.Data);
						CompressionUtils.CompressString(ref urldata, out compressed);
						urldatacmd.Parameters[1].Value = compressed;
						urldatacmd.Parameters[2].Value = compressed.Length;
						urldatacmd.Parameters[3].Value = data.Data.Length;
					}
					else
					{
						urldatacmd.Parameters[1].Value = new byte[0];
						urldatacmd.Parameters[2].Value = 0;
						urldatacmd.Parameters[3].Value = 0;
					}
					urldatacmd.Parameters[4].Value = (short)data.HttpStatusCode;
					urldatacmd.Parameters[5].Value = data.RetrievalTime;
					urldatacmd.ExecuteNonQuery();
					urldatacmd.Dispose();
				}
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "DBUpdater failed to update a Url in the database: " + e.ToString());
				retVal = 0;
			}
			return retVal;
		}

		/// <summary>
		/// Deletes all the edges of the Url Link Graph whose starting node is the given Url
		/// </summary>
		/// <param name="UrlID">The ID of the url whose all the out-links must be deleted.</param>
		/// <param name="transaction">The currently active <see cref="SqlTransaction"/>.</param>
		private void ClearUrlOutLinks(int UrlID, SqlTransaction transaction)
		{
			try
			{
				SqlCommand cmd = new SqlCommand("cw_delete_url_out_links", dbcon, transaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBActionTimeout;
				cmd.Parameters.Add("@url_id", SqlDbType.Int);
				cmd.Parameters[0].Value = UrlID;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "The DBUpdater plugin failed to delete the out links of the Url with ID " + UrlID.ToString() + " from the link graph: " + e.ToString());
				GC.Collect();
			}
		}

		/// <summary>
		/// Inserts the links contained in a url into the database and updates the link graph
		/// </summary>
		/// <param name="UrlID">The ID of the url.</param>
		/// <param name="data">The <see cref="UrlCrawlData"/> of the url.</param>
		/// <param name="transaction">The currently active <see cref="SqlTransaction"/>.</param>
		private void InsertUrlOutLinks(int UrlID, UrlCrawlData data, SqlTransaction transaction)
		{
			try
			{
				//Build the SQL Commands
				SqlCommand hostcmd = new SqlCommand("cw_insert_host", dbcon, transaction);
				hostcmd.CommandType = CommandType.StoredProcedure;
				hostcmd.CommandTimeout = settings.DBActionTimeout;
				hostcmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
				hostcmd.Parameters.Add("@host_name", SqlDbType.NVarChar, 100);

				SqlCommand urlcmd = new SqlCommand("cw_insert_url", dbcon, transaction);
				urlcmd.CommandType = CommandType.StoredProcedure;
				urlcmd.CommandTimeout = settings.DBActionTimeout;
				urlcmd.Parameters.Add("@url",SqlDbType.NVarChar, 500);
				urlcmd.Parameters.Add("@url_md5", SqlDbType.UniqueIdentifier);
				urlcmd.Parameters.Add("@url_host_id", SqlDbType.UniqueIdentifier);
				urlcmd.Parameters.Add("@url_priority", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@flag_domain", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@flag_robots", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@id", SqlDbType.Int);
				urlcmd.Parameters["@id"].Direction = ParameterDirection.Output;
				
				SqlCommand linkcmd = new SqlCommand("cw_insert_link_graph", dbcon, transaction);
				linkcmd.CommandType = CommandType.StoredProcedure;
				linkcmd.CommandTimeout = settings.DBActionTimeout;
				linkcmd.Parameters.Add("@from_url_id", SqlDbType.Int);
				linkcmd.Parameters.Add("@to_url_id", SqlDbType.Int);
	
				int new_id = 0;
				//insert each out link in the database
				foreach(InternetUrlToIndex url in data.OutLinks)
				{
					try
					{
						Uri uri = new Uri(url.Url);
						Guid host_id = new Guid(MD5Hash.md5(uri.Host));

						hostcmd.Parameters[0].Value = host_id;
						hostcmd.Parameters[1].Value = uri.Host;
						hostcmd.ExecuteNonQuery();

						urlcmd.Parameters[0].Value = url.Url;
						urlcmd.Parameters[1].Value = new Guid(url.MD5);
						urlcmd.Parameters[2].Value = host_id;
						urlcmd.Parameters[3].Value = (byte)url.Priority;
						urlcmd.Parameters[4].Value = (byte)url.FlagDomain;
						urlcmd.Parameters[5].Value = (byte)((url.FlagRobots)?1:0);
						urlcmd.ExecuteNonQuery();
						new_id = (int)urlcmd.Parameters["@id"].Value; //(int)urlcmd.ExecuteScalar();
						
						linkcmd.Parameters[0].Value = UrlID;
						linkcmd.Parameters[1].Value = new_id;
						linkcmd.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						AddToReportQueue(CWLoggerEntryType.Warning, "DBUpdater plugin failed to insert an edge to the link graph: " + e.ToString());
						continue;
					}
				}
				urlcmd.Dispose();
				linkcmd.Dispose();
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "DBUpdater plugin: an unexpected error occured when inserting the out links of url with ID " + UrlID.ToString() + " to the link graph: " + e.ToString());
				GC.Collect();
			}
		}

		/// <summary>
		/// Returns the number of seconds the thread must wait before continuing to the next loop
		/// </summary>
		/// <returns>The delay amount in seconds</returns>
		private int PauseInSeconds()
		{
			int retVal = 30;
			switch(settings.PauseDelay)
			{
				case 0:
					retVal = 15;
					break;

				case 1:
					retVal = 30;
					break;

				case 2:
					retVal = 60;
					break;

				case 3:
					retVal = 300;
					break;

				default:
					retVal = 30;
					break;
			}
			return retVal;
		}

		/// <summary>
		/// Stops the plugin's threads
		/// </summary>
		private void StopThreads()
		{
			if(pluginThread!=null)
			{
				if(pluginThread.IsAlive)
				{
					pluginThread.Join();
				}
			}
		}

		/// <summary>
		/// Suspends the plugin's threads
		/// </summary>
		private void SuspendThreads()
		{
			if(pluginThread.IsAlive)
			{
				pluginThread.Suspend();
			}
		}

		/// <summary>
		/// Resumes the plugin's suspended threads
		/// </summary>
		private void ResumeThreads()
		{
			if((pluginThread.ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested)) >0)
			{
				pluginThread.Resume();
			}
		}

		#endregion

		#region Base class override methods

		#endregion

		#region Event Invokers

		/// <summary>
		/// Raises the <see cref="StateChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnStateChanged(EventArgs e)
		{
			if(StateChanged != null)
			{
				StateChanged(this,e);
			}
		}

		#endregion
	}
}
