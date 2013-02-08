using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerPlugins.UrlSelection
{
	/// <summary>
	/// UrlSelectorPlugin is a plugin that selects the most appropriate urls to be crawled
	/// next, combines them with the robots.txt information stored in the system's database
	/// at the time of the selection and marks them as assigned.
	/// </summary>
	/// <remarks>
	/// The UrlSelectionPlugin in effect allows the selection of the next Urls to be crawled
	/// to be performed in batches. Thus the server spends much less processing time, since
	/// the selection of ready Urls is perfomed only when the number of pending urls falls
	/// below an administrator-defined threshold. Thus the processing required in order to
	/// assign urls to clients is very little, since the Selection process does not have to
	/// be performed from scrath. In other words when it is activated it builds a queue of
	/// ready urls that can be assigned to the clients very fast and whose size can be easily
	/// adjusted according to the load on the server and the number of clients.<br/>
	/// A good policy for selecting the size of the cache is to multiply the number of active
	/// clients by 10 times the number of Urls per packet (work unit), whereas the selection
	/// size (the number of Urls selected and marked as ready to be crawled) is 10 times the
	/// threshold (cache) size.<br/>
	/// Any algorithms effecting the way the next urls to be crawled are selected should be
	/// applied by this plugin. In other words this plugin sould take into consideration the
	/// priority of Urls during the selection process.
	/// </remarks>
	[CrawlWavePlugin]
	public class UrlSelectorPlugin : PluginBase, IPlugin
	{
		#region Private variables
		
		private PluginSettings settings;
		private SqlConnection dbcon;
		private Thread pluginThread;
		private BannedHostsCache banned;
		private RobotsCache robots;
		private bool mustStop;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructs a new instance of the <see cref="CrawlWave.ServerPlugins.UrlSelection.UrlSelectorPlugin"/> class.
		/// </summary>
		public UrlSelectorPlugin()
		{
			dataDependent = true;
			description = "CrawlWave Url Selection Plugin";
			enabled = true;
			host = null;
			name = "CrawlWave.ServerPlugins.UrlSelection";
			percent = 0;
			settingsPath = String.Empty;
			state = PluginState.Stopped;
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			settings = PluginSettings.Instance();
			dbcon = new SqlConnection(settings.DBConnectionString);
			pluginThread = null;
			mustStop = false;
			banned = BannedHostsCache.Instance();
			robots = RobotsCache.Instance();
		}

		#endregion

		#region IPlugin Members

		/// <summary>
		/// Starts the process of the selection of urls to crawl from the database. If the 
		/// plugin is  already in the <see cref="PluginState.Running"/> state then it has
		/// no effect.
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
				//Initialize the connection to the database
				//create the thread that will be selecting urls  to crawl from the databse.
				pluginThread = new Thread(new ThreadStart(PerformSelection));
				pluginThread.IsBackground = true;
				pluginThread.Priority = ThreadPriority.Lowest;
				pluginThread.Name = "Url Selection Plugin Thread";
				pluginThread.Start();
				//Notify the clients that the plugin's process has started
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Error, "The UrlSelection Plugin failed to start: " + ex.ToString());
				mustStop = true;
				state = PluginState.Stopped;
				try
				{
					StopThreads();
				}
				catch(Exception exc)
				{
					ReportImmediately(CWLoggerEntryType.Error, "The UrlSelection Plugin failed to stop all threads: " + exc.ToString());
				}
			}
			finally
			{
				Report();
			}
		}

		/// <summary>
		/// Pauses the url selection process by calling <see cref="Thread.Suspend"/> on the 
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
		/// Resumes the url selection process if it has been paused.
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
		/// Stops the url selection process. If the plugin is already in the <see cref="PluginState.Stopped"/>
		/// state it has no effect. If the url selection is in progress it is not stopped
		/// abruptly but the method waits until the selection of urls is complete.
		/// </summary>
		public void Stop()
		{
			try
			{
				mustStop = true;
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Info, DateTime.Now, "Stopping Url Selection plugin."));
				//wait for all the threads to finish
				if(pluginThread != null)
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
				ReportImmediately(CWLoggerEntryType.Warning, "Url Selection plugin failed to stop: " + ex.ToString());
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
		/// Performs the selection of urls to crawl from the database. It is the method that
		/// is running on the pluginThread, so it must be able to handle ThreadAbortException
		/// and ThreadInterruptedException.
		/// </summary>
		private void PerformSelection()
		{
			try
			{
				ReportImmediately(CWLoggerEntryType.Info, "CrawlWave Url Selection Plugin thread has started with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4"));
				while(!mustStop)
				{
					//Select urls from the database, update the appropriate tables and stats
					try
					{
						try
						{
							dbcon.Open();
						}
						catch
						{}
						SqlTransaction transaction = null;
						if(settings.UseTransactions)
						{
							transaction = dbcon.BeginTransaction();
						}
						try
						{
							//check if the threshold criteria is met
							int count = SelectUrlCount(transaction);
							AddToReportQueue(CWLoggerEntryType.Info, "UrlSelectionPlugin: Threshold is " + settings.Threshold + " urls, current Url Queue Size is " + count.ToString());
							if(count < settings.Threshold)
							{
								AddToReportQueue(CWLoggerEntryType.Info, "UrlSelectionPlugin: Performing Url Selection, Selection size = " + settings.SelectionSize.ToString());
								//Refresh the banned hosts cache
								banned.RefreshCache();
								//and the robots cache
								robots.LoadCache();
								//Select the Urls to process
								DataSet urls = SelectUrls(transaction);
								//insert each one of them to the table of urls to be crawled and
								//mark them as assigned
								if(urls.Tables[0].Rows.Count>0)
								{
									//create the SqlCommands
									SqlCommand icmd = new SqlCommand("cw_insert_url_to_crawl",dbcon, transaction);
									icmd.CommandType = CommandType.StoredProcedure;
									icmd.CommandTimeout = 60;
									icmd.Parameters.Add("@url_id", SqlDbType.Int);
									icmd.Parameters.Add("@url", SqlDbType.NVarChar, 500);
									icmd.Parameters.Add("@crc", SqlDbType.BigInt);
									icmd.Parameters.Add("@domain", SqlDbType.TinyInt);
									icmd.Parameters.Add("@robots", SqlDbType.NVarChar, 1000);
									icmd.Parameters.Add("@robots_expiration", SqlDbType.SmallDateTime);
									
									SqlCommand mcmd = new SqlCommand("cw_mark_url_as_assigned",dbcon,transaction);
									mcmd.CommandType = CommandType.StoredProcedure;
									mcmd.CommandTimeout = 60;
									mcmd.Parameters.Add("@url_id", SqlDbType.Int);

									Guid hostID = new Guid();
									foreach(DataRow dr in urls.Tables[0].Rows)
									{
										hostID = (Guid)dr[2];
										byte [] hostIDbytes = hostID.ToByteArray();
										if(!banned.IsBanned(hostIDbytes))
										{
											icmd.Parameters[0].Value = dr[0];
											icmd.Parameters[1].Value = dr[1];
											icmd.Parameters[2].Value = dr[3];
											icmd.Parameters[3].Value = dr[4];
											RobotsTxtEntry entry = robots.GetEntry(hostIDbytes);
											if(entry == null)
											{
												icmd.Parameters[4].Value = DBNull.Value;
												icmd.Parameters[5].Value = DBNull.Value;
											}
											else
											{
												icmd.Parameters[4].Value = ConcatenatePaths(entry.DisallowedPaths);
												icmd.Parameters[5].Value = entry.ExpirationDate;
											}
											try
											{
												icmd.ExecuteNonQuery();
											}
											catch
											{
												continue;
											}
										}
										mcmd.Parameters[0].Value = (int)dr[0];
										try
										{
											mcmd.ExecuteNonQuery();
										}
										catch
										{
											continue;
										}
										//MarkUrlAsAssigned((int)dr[0],transaction); 
										//perhaps this should be inlined because calling the method n times
										//creates and destroys a lot of objects and spares cpu time
									}
									mcmd.Dispose();
									icmd.Dispose();
								}
							}
							try
							{
								if(settings.UseTransactions)
								{
									transaction.Commit();
									transaction.Dispose();
								}
							}
							catch(Exception ext)
							{
                                events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, ext.ToString()));
							}
							if(dbcon.State == ConnectionState.Closed)
							{
								events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Error, DateTime.Now, "The Url Selection Plugin failed to connect to the database. Stopping..."));
								return;
							}
							if(settings.PauseBetweenOperations)
							{
								int waitSeconds = PauseInSeconds();
								while(waitSeconds>0)
								{
									Thread.Sleep(1000);
									if(mustStop)
									{
										break;
									}
									waitSeconds--;
								}
							}
						}
						catch(ThreadInterruptedException)
						{
							if(settings.UseTransactions)
							{
								transaction.Rollback();
								transaction.Dispose();
							}
						}
						finally
						{
							if(dbcon.State != ConnectionState.Closed)
							{
								try
								{
									dbcon.Close();
								}
								catch
								{}
							}
							Report();
						}
					}
					catch(Exception ex)
					{
						events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, ex.ToString()));
					}
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
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Info, DateTime.Now, Thread.CurrentThread.Name + " has stopped."));
			}
		}

		/// <summary>
		/// Selects a set of Urls to be crawled and performs all the necessary processing on them.
		/// </summary>
		/// <param name="transaction">The <see cref="SqlTransaction"/> currently active.</param>
		/// <returns>A <see cref="DataSet"/> containing the selected Urls</returns>
		private DataSet SelectUrls(SqlTransaction transaction)
		{
			DataSet data = new DataSet();
			try
			{
				SqlCommand cmd;
				if(!settings.SelectionMode)
				{
					cmd = new SqlCommand("cw_select_ready_urls",dbcon, transaction);
				}
				else
				{
					cmd = new SqlCommand("cw_select_ready_urls_to_refresh", dbcon, transaction);
				}
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBTimeout;
				cmd.Parameters.Add("@count",SqlDbType.Int);
				cmd.Parameters[0].Value = settings.SelectionSize;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				try
				{
					da.Fill(data);
				}
				finally
				{
					da.Dispose();
					cmd.Dispose();
				}
			}
			catch(Exception e)
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, "Url Selection plugin failed to select ready Urls: " + e.ToString()));
				GC.Collect();
			}
			return data;
		}

		/// <summary>
		/// Checks how many Urls are left on the database, if this number is above a certain
		/// threshold no processing is performed and the plugin simply waits.
		/// </summary>
		/// <param name="transaction">The <see cref="SqlTransaction"/> currently active.</param>
		/// <returns></returns>
		private int SelectUrlCount(SqlTransaction transaction)
		{
			int retVal = 0;
			try
			{
				SqlCommand cmd = new SqlCommand("cw_select_url_to_crawl_count",dbcon, transaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 60;
				retVal = (int)cmd.ExecuteScalar();
				cmd.Dispose();
			}
			catch(Exception e)
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, "Url Selection plugin failed to select the count of ready Urls: " + e.ToString()));
				GC.Collect();
			}
			return retVal;
		}

		/// <summary>
		/// Mark the url as assigned
		/// </summary>
		/// <param name="transaction">The <see cref="SqlTransaction"/> currently active.</param>
		/// <param name="UrlID">The ID of the Url</param>
		/// <remarks>This method has been inlined in PerformSelection now (10/12/04) </remarks>
		private void MarkUrlAsAssigned(int UrlID, SqlTransaction transaction)
		{
			try
			{
				SqlCommand cmd = new SqlCommand("cw_mark_url_as_assigned",dbcon,transaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 60;
				cmd.Parameters.Add("@url_id", SqlDbType.Int);
				cmd.Parameters[0].Value = UrlID;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
			}
			catch(Exception e)
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, "Url Selection plugin failed to mark Url with ID " + UrlID.ToString() + " as assigned: " + e.ToString()));
				GC.Collect();
			}
		}

		/// <summary>
		/// Concatenates an array of strings each containing a disallowed path into a string
		/// separated by spaces.
		/// </summary>
		/// <param name="paths">An array of strings each containing a disallowed path.</param>
		/// <returns>A string containing all the paths concatenated and separated by spaces.</returns>
		private string ConcatenatePaths(string [] paths)
		{
			if((paths==null)||(paths.Length==0))
			{
				return String.Empty;
			}
			else
			{
				StringBuilder sb = new StringBuilder(paths[0]);
				for(int i=1; i<paths.Length; i++)
				{
					sb.Append(' ');
					sb.Append(paths[i]);
				}
				return sb.ToString();
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
			if(pluginThread!=null)
			{
				if(pluginThread.IsAlive)
				{
					pluginThread.Suspend();
				}
			}
		}

		/// <summary>
		/// Resumes the plugin's suspended threads
		/// </summary>
		private void ResumeThreads()
		{
			if(pluginThread!=null)
			{
				if((pluginThread.ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested)) >0)
				{
					pluginThread.Resume();
				}
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
