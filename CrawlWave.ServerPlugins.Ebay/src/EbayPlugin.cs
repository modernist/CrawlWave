using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CrawlWave.Common;
using CrawlWave.ServerCommon;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using System.Text.RegularExpressions;

namespace CrawlWave.ServerPlugins.Ebay
{
	[CrawlWavePlugin]
	public class EbayPlugin : PluginBase, IPlugin 
	{
		#region Private variables

		private Mutex mutex;
		private PluginSettings settings;
		private Thread pluginThread;
		private DBConnectionStringProvider dbProvider;
		private bool mustStop;
		private SqlConnection dbcon;
		private Regex regUser, regFeedback;

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new instance of the <see cref="EbayPlugin"/> class.
		/// </summary>
		public EbayPlugin()
		{
			mutex = new Mutex();
			settings = PluginSettings.Instance();
			name = "CrawlWave.ServerPlugins.Ebay";
			description = "CrawlWave Ebay Plugin";
			dataDependent = true;
			state = PluginState.Stopped;
			enabled = true;
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			percent = 0;
			mustStop = false;
			pluginThread = null;
			dbProvider = DBConnectionStringProvider.Instance();
			settings.DBConnectionString = dbProvider.ProvideDBConnectionString(name);
			dbcon = new SqlConnection(settings.DBConnectionString);
			regUser = new Regex("<h1>eBay My World:\\s*(?<user>[^<]*)<img", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
			regFeedback = new Regex("Feedback score: <b>(?<feedback>\\d+)</b><span class=\"vSep\">\\|</span>Positive feedback: <b>(?<positive>\\d+\\.?\\d)%</b>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
			//eBay My World: c7gman<img
			//Feedback score: <b>285</b><span class="vSep">|</span>Positive feedback: <b>100%</b>

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
			if (state == PluginState.Running)
			{
				return;
			}
			if (state == PluginState.Paused)
			{
				throw new InvalidOperationException("The plugin is in the Paused state and cannot be started.");
			}
			try
			{
				mustStop = false;
				//Initialize the connection to the database
				//create the thread that will be selecting urls  to crawl from the databse.
				pluginThread = new Thread(new ThreadStart(PerformProcessing));
				pluginThread.IsBackground = true;
				pluginThread.Priority = ThreadPriority.Lowest;
				pluginThread.Name = "Ebay Plugin Thread";
				pluginThread.Start();
				//Notify the clients that the plugin's process has started
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
			}
			catch (Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Error, "The Ebay Plugin failed to start: " + ex.ToString());
				mustStop = true;
				state = PluginState.Stopped;
				try
				{
					StopThreads();
				}
				catch (Exception exc)
				{
					ReportImmediately(CWLoggerEntryType.Error, "The Ebay Plugin failed to stop all threads: " + exc.ToString());
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
			if (state == PluginState.Paused)
			{
				return;
			}
			if (state == PluginState.Stopped)
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
			if ((state == PluginState.Stopped) || (state == PluginState.Running))
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
				if (pluginThread != null)
				{
					while (pluginThread.IsAlive)
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
			catch (Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Warning, "Ebay plugin failed to stop: " + ex.ToString());
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
		/// Performs the processing of the ebay profile pages
		/// </summary>
		private void PerformProcessing()
		{
			try
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Info, DateTime.Now, "CrawlWave Ebay Plugin thread has started with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4")));
				while (!mustStop)
				{
					//Select a user profile page from the database, perform content extraction and store the
					//results back in the database
					int UrlID = 0;
					string data = String.Empty;
					int waitSeconds = PauseInSeconds();
					SelectProfileUrl(out UrlID, out data);

					if (UrlID != 0)
					{
						try
						{
							//extract the data
							UserProfile profile = ExtractUserProfile(UrlID, data);
							if(!string.IsNullOrEmpty(profile.Username))
								UpdateEbayProfile(profile);
							UpdateUrlDataLastProcess(UrlID);
						}
						catch (Exception e)
						{
							events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, "Ebay Plugin failed to extract information from Url with ID " + UrlID.ToString() + ": " + e.ToString()));
							continue;
						}
					}
					else
					{
						for (int i = 0; i < waitSeconds; i++)
						{
							Thread.Sleep(1000);
							if (mustStop)
							{
								break;
							}
						}
					}
					Report();
					if (settings.PauseBetweenOperations)
					{
						for (int i = 0; i < waitSeconds; i++)
						{
							Thread.Sleep(1000);
							if (mustStop)
							{
								break;
							}
						}
					}
				}
			}
			catch (ThreadAbortException)
			{
				//The thread was asked to abort, which means it must return at once
				return;
			}
			catch (ThreadInterruptedException)
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
		/// 
		/// </summary>
		/// <param name="UrlID"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		private UserProfile ExtractUserProfile(int UrlID, string data)
		{
			UserProfile result = new UserProfile();
			result.Username = string.Empty;
			result.UrlID = UrlID;

			Match userMatch = regUser.Match(data);
			if (userMatch.Success)
			{
				result.Username = userMatch.Groups["user"].Value;
			}

			Match feedbackMatch = regFeedback.Match(data);
			if (feedbackMatch.Success)
			{
				int.TryParse(feedbackMatch.Groups["feedback"].Value, out result.Feedback);
				float.TryParse(feedbackMatch.Groups["positive"].Value.Replace('.', ','), out result.Positive);
			}
			//eBay My World: c7gman<img
			//Feedback score: <b>285</b><span class="vSep">|</span>Positive feedback: <b>100%</b>
			return result;
		}

		/// <summary>
		/// Selects a random profile Url to extract data from
		/// </summary>
		/// <param name="UrlID">The ID of the selected Url.</param>
		/// <param name="data">The data of the selected Url.</param>
		private void SelectProfileUrl(out int UrlID, out string data)
		{
			UrlID = 0;
			data = String.Empty;
			try
			{
				try
				{
					dbcon.Open();
				}
				catch
				{ }
				if (dbcon.State == ConnectionState.Closed)
				{
					//log a message
					return;
				}
				SqlCommand cmd = new SqlCommand("cw_select_ebay_profile_url", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBActionTimeout;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				if (ds.Tables.Count == 0)
				{
					return;
				}
				UrlID = (int)ds.Tables[0].Rows[0][0];
				byte[] buffer = (byte[])ds.Tables[0].Rows[0][1];
				CompressionUtils.DecompressToString(buffer, out data);
				ds.Dispose();
			}
			catch
			{
				if (dbcon.State != ConnectionState.Closed)
				{
					try
					{
						dbcon.Close();
					}
					catch
					{ }
				}
				GC.Collect();
			}
		}

		/// <summary>
		/// Updates or inserts an ebay profile
		/// </summary>
		/// <param name="UrlID"></param>
		/// <param name="username"></param>
		/// <param name="feedback"></param>
		/// <param name="positive"></param>
		private void UpdateEbayProfile(UserProfile profile)
		{
			try
			{
				AddToReportQueue(CWLoggerEntryType.Info, "Ebay Plugin is updating User Profile with " + profile.ToString());
				try
				{
					dbcon.Open();
				}
				catch
				{ }
				if (dbcon.State == ConnectionState.Closed)
				{
					//log a message
					return;
				}
				SqlCommand cmd = new SqlCommand("cw_insert_update_ebay_profile", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBActionTimeout;
				cmd.Parameters.Add("@urlid", SqlDbType.Int);
				cmd.Parameters.Add("@username", SqlDbType.NVarChar, 50);
				cmd.Parameters.Add("@feedback", SqlDbType.Int);
				cmd.Parameters.Add("@positive", SqlDbType.Float);
				cmd.Parameters[0].Value = profile.UrlID;
				cmd.Parameters[1].Value = profile.Username;
				cmd.Parameters[2].Value = profile.Feedback;
				cmd.Parameters[3].Value = profile.Positive;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				dbcon.Close();
			}
			catch (Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "Ebay Plugin failed to update the profile of the user " + profile.Username + ": " + e.Message);
				if (dbcon.State != ConnectionState.Closed)
				{
					try
					{
						dbcon.Close();
					}
					catch
					{ }
				}
				GC.Collect();
			}
		}

		/// <summary>
		/// Updates a url's last process date after the data extraction is complete
		/// </summary>
		/// <param name="UrlID">The ID of the Url to update</param>
		private void UpdateUrlDataLastProcess(int UrlID)
		{
			try
			{
				try
				{
					dbcon.Open();
				}
				catch
				{ }
				if (dbcon.State == ConnectionState.Closed)
				{
					//log a message
					return;
				}
				SqlCommand cmd = new SqlCommand("cw_update_url_data_last_process", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBActionTimeout;
				cmd.Parameters.Add("@url_id", SqlDbType.Int);
				cmd.Parameters[0].Value = UrlID;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				dbcon.Close();
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "Ebay Plugin failed to update the Url with ID " + UrlID + ": " + e.Message);
				if (dbcon.State != ConnectionState.Closed)
				{
					try
					{
						dbcon.Close();
					}
					catch
					{ }
				}
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
			switch (settings.PauseDelay)
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
			if (pluginThread != null)
			{
				if (pluginThread.IsAlive)
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
			if (pluginThread != null)
			{
				if (pluginThread.IsAlive)
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
			if (pluginThread != null)
			{
				if ((pluginThread.ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested)) > 0)
				{
					pluginThread.Resume();
				}
			}
		}

		#endregion

		#region Event Invokers

		/// <summary>
		/// Raises the <see cref="StateChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnStateChanged(EventArgs e)
		{
			if (StateChanged != null)
			{
				StateChanged(this, e);
			}
		}

		#endregion

		private struct UserProfile
		{
			public int UrlID;
			public string Username;
			public int Feedback;
			public float Positive;

			public override string ToString()
			{
				return "Url ID: " + UrlID.ToString();
				//return string.Format("Url ID: {0), Username: {1}, Feedback: {2}, Positive: {3}", UrlID, string.IsNullOrEmpty(Username) ? "" : Username, Feedback, Positive);
			}
		}
	}
}
