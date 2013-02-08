using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerPlugins.WordCleanup
{

	/// <summary>
	/// 
	/// </summary>
	[CrawlWavePlugin]
	public class WordCleanupPlugin : PluginBase, IPlugin
	{
		#region Private variables
		
		private PluginSettings settings;
		private SqlConnection dbcon;
		private Thread pluginThread;
		private int start;
		private bool mustStop;
		private static char []Chars = {'Á','Â','Ã','Ä','Å','Æ','Ç','È','É','Ê','Ë','Ì','Í','Î','Ï','Ð','Ñ','Ó','Ô','Õ','Ö','×','Ø','Ù','¢','¸','¹','º','¼','¾','¿','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
		private Hashtable validChars;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructs a new instance of the <see cref="CrawlWave.ServerPlugins.UrlSelection.UrlSelectorPlugin"/> class.
		/// </summary>
		public WordCleanupPlugin()
		{
			dataDependent = true;
			description = "CrawlWave WordCleanup Plugin";
			enabled = true;
			host = null;
			name = "CrawlWave.ServerPlugins.WordCleanup";
			percent = 0;
			settingsPath = String.Empty;
			state = PluginState.Stopped;
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			settings = PluginSettings.Instance();
			dbcon = new SqlConnection(settings.DBConnectionString);
			pluginThread = null;
			start = 0;
			validChars = new Hashtable();
			foreach(char c in Chars)
			{
				validChars.Add(c, null);
			}
			mustStop = false;
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
				pluginThread = new Thread(new ThreadStart(PerformCleanup));
				pluginThread.IsBackground = true;
				pluginThread.Priority = ThreadPriority.Lowest;
				pluginThread.Name = "Word Cleanup Plugin Thread";
				pluginThread.Start();
				//Notify the clients that the plugin's process has started
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Error, "The Word Cleanup Plugin failed to start: " + ex.ToString());
				mustStop = true;
				state = PluginState.Stopped;
				try
				{
					StopThreads();
				}
				catch(Exception exc)
				{
					ReportImmediately(CWLoggerEntryType.Error, "The Word Cleanup Plugin failed to stop all threads: " + exc.ToString());
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
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Info, DateTime.Now, "Stopping Word Cleanup plugin."));
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
				ReportImmediately(CWLoggerEntryType.Warning, "Word Cleanup plugin failed to stop: " + ex.ToString());
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
		/// 
		/// </summary>
		private void PerformCleanup()
		{
			try
			{
				ReportImmediately(CWLoggerEntryType.Info, "CrawlWave Word Cleanup Plugin thread has started with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4"));
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
							AddToReportQueue(CWLoggerEntryType.Info, "WordCleanupPlugin: Start is " + start.ToString());
							//Select the words to process
							DataSet words = SelectWords(transaction);
							if(words.Tables[0].Rows.Count>0)
							{
								//create the SqlCommand
								SqlCommand dcmd = new SqlCommand("cw_delete_word",dbcon, transaction);
								dcmd.CommandType = CommandType.StoredProcedure;
								dcmd.CommandTimeout = settings.DBTimeout;
								dcmd.Parameters.Add("@word_id", SqlDbType.Int);

								string word = String.Empty;
								foreach(DataRow dr in words.Tables[0].Rows)
								{
									word = (string)dr[1];

									if(!IsWordOK(word))
									{
										dcmd.Parameters[0].Value = dr[0];
										try
										{
											dcmd.ExecuteNonQuery();
										}
										catch
										{
											continue;
										}
									}
								}
								dcmd.Dispose();
							}
							else
							{
								mustStop = true;
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
								events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Error, DateTime.Now, "The Word Cleanup Plugin failed to connect to the database. Stopping..."));
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
		private DataSet SelectWords(SqlTransaction transaction)
		{
			DataSet data = new DataSet();
			try
			{
				SqlCommand cmd = new SqlCommand("cw_select_words_for_cleanup",dbcon, transaction);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBTimeout;
				cmd.Parameters.Add("@start", SqlDbType.Int);
				cmd.Parameters.Add("@count",SqlDbType.Int);
				cmd.Parameters[0].Value = start;
				cmd.Parameters[1].Value = settings.SelectionSize;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				try
				{
					da.Fill(data);
					start = (int)data.Tables[0].Rows[data.Tables[0].Rows.Count-1][0];
				}
				finally
				{
					da.Dispose();
					cmd.Dispose();
				}
			}
			catch(Exception e)
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, "Word Cleanup plugin failed to select words: " + e.ToString()));
				GC.Collect();
			}
			return data;
		}

		/// <summary>
		/// Delete Word
		/// </summary>
		/// <param name="transaction">The <see cref="SqlTransaction"/> currently active.</param>
		/// <param name="UrlID">The ID of the Word</param>
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
		/// 
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private bool IsWordOK(string word)
		{
			bool retVal = true;
			int valid = 0;
			for(int i = 0; i < word.Length; i++)
			{
				if(validChars.ContainsKey(word[i]))
				{
					valid++;
				}
			}
			if((valid / word.Length) < 0.5d)
			{
				retVal = false;
			}
			return retVal;
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
