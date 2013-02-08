using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using CrawlWave.Common;
using CrawlWave.ServerCommon;
using SpiderWaveJobs.Classes;

namespace CrawlWave.ServerPlugins.PageRank
{
	/// <summary>
	/// Summary description for PageRankPlugin.
	/// </summary>
	[CrawlWavePlugin]
	public class PageRankPlugin : PluginBase, IPlugin
	{
		#region Private variables

		private SqlConnection dbcon;
		private Thread pluginThread;
		private bool mustStop;
		private SWPageRankVisitor pageRank=null;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructs a new instance of the <see cref="PageRankPlugin"/> class.
		/// </summary>
		public PageRankPlugin()
		{
			description = "CrawlWave PageRank Plugin";
			name = "CrawlWave.ServerPlugins.PageRank";
			percent = 0;
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			string connectionString = DBConnectionStringProvider.Instance().ProvideDBConnectionString(name);
			dbcon = new SqlConnection(connectionString);
			pluginThread = null;
			mustStop = false;
			dataDependent = true;
			enabled = true;
		}

		#endregion

		#region IPlugin Members

		/// <summary>
		/// 
		/// </summary>
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
				//create the thread that will be processing urls.
				pluginThread = new Thread(new ThreadStart(PerformPageRank));
				pluginThread.IsBackground = true;
				pluginThread.Priority = ThreadPriority.Lowest;
				pluginThread.Name = "PageRank Plugin Thread";
				pluginThread.Start();
				//Notify the clients that the plugin's process has started
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Error, "The PageRank Plugin failed to start: " + ex.ToString());
				mustStop = true;
				state = PluginState.Stopped;
				try
				{
					StopThreads();
				}
				catch(Exception exc)
				{
					ReportImmediately(CWLoggerEntryType.Error, "The PageRank Plugin failed to stop all threads: " + exc.ToString());
				}
			}
			finally
			{
				Report();
			}
		}

		/// <summary>
		/// 
		/// </summary>
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
		///
		/// </summary>
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
		/// 
		/// </summary>
		public void Stop()
		{
			try
			{
				mustStop = true;
				AddToReportQueue(CWLoggerEntryType.Info, "Stopping PageRank plugin.");
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
				ReportImmediately(CWLoggerEntryType.Warning, "PageRank plugin failed to stop: " + ex.ToString());
			}
			finally
			{
				Report();
				mustStop = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler StateChanged;

		/// <summary>
		/// 
		/// </summary>
		public void ShowSettings()
		{
			frmPluginSettings frm = new frmPluginSettings();
			frm.Show();
			//on the form's OK button you must write code to save your settings persistently
			//and on the constructor of this class you must write code to read them
		}

		#endregion

		#region Private Methods


		public void OnReport(object UrlBaseVisitor, SWPageRankVisitor.ReportTriggerEventArgs e)
		{			
			ReportImmediately(CWLoggerEntryType.Info,e.strReport);									
		}



		/// <summary>
		/// Performs the PagrRank algorithm. It is the method that is running on the 
		/// pluginThread, so it must be able to handle ThreadAbortException and
		/// ThreadInterruptedException.
		/// </summary>
		private void PerformPageRank()
		{
			try
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Info, DateTime.Now, "CrawlWave Url Selection Plugin thread has started with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4")));
				
				//Select urls from the database, update the appropriate tables and stats				
				try
				{
					ReportImmediately(CWLoggerEntryType.Info,"Starting PageRank.");
					dbcon.Open();
					
					SWRankHandlerHash rankHandler = new SWRankHandlerHash(dbcon,1, 2);			
					pageRank = new SWPageRankVisitor(dbcon, 200000, 50,  rankHandler,0.85f);			
					pageRank.OnReportTrigger += (new SWUrlBaseVisitor.ReportTrigger(OnReport));

					if (pageRank.VisitAllURLs(pageRank))
					{
						ReportImmediately(CWLoggerEntryType.Info,"Succesfully calculated PageRank.");
					}
					//Report();
					state = PluginState.Stopped;
					OnStateChanged(EventArgs.Empty);
				}
				catch
				{}
				//do processing
				
				
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

		#region Event Invokers

		/// <summary>
		/// Raises the <see cref="StateChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnStateChanged(EventArgs e)
		{
			if(StateChanged != null)
			{
				StateChanged(this, e);
			}
		}

		#endregion
	}
}
