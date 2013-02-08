using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using CrawlWave;
using CrawlWave.Client.Forms;
using CrawlWave.Common;

namespace CrawlWave.Client
{
	/// <summary>
	/// Client is a Singleton class that contains the application's Main method and startup
	/// code. It performs all the necessary initialization as well as the time scheduling
	/// of the CrawlWave Client, if the user has enabled it. It also makes sure that there
	/// is only one instance of the application running on the machine.
	/// </summary>
	public class Client
	{
		#region Private variables

		private static Client instance;
		private Globals globals;
		private Crawler crawler = null;
		private bool mustTerminate = false;
		private AlarmTimer startScheduler = null;
		private AlarmTimer stopScheduler = null;

		#endregion

		#region Public Properties

		/// <summary>
		/// Sets a <see cref="Boolean"/> value that determines whether the application must
		/// terminate its operation.
		/// </summary>
		public bool MustTerminate
		{
			set { mustTerminate = value; }
		}

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private Client()
		{
			globals = Globals.Instance();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="Client"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="Client"/>.</returns>
		public static Client Instance()
		{
			if (instance==null)
			{
				lock(typeof(Client))
				{
					if( instance == null )
					{
						instance = new Client();
					}
				}
			}
			return instance;
		}


		#endregion

		#region Main method

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[MTAThread]
		static void Main(string []args) 
		{
			//Attach the default Exception handlers
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ApplicationThreadExceptionHandler);
			try
			{
				if(CheckForRunningInstances(false))
				{
					//there is another instance of the application running so this one must exit
					return;
				}
				Client client = Client.Instance();
				//set the time scheduler to control the automatic start/stop of the crawling
				if(client.globals.Settings.EnableScheduler)
				{
					client.startScheduler = new AlarmTimer();
					client.startScheduler.AlarmTime = DateTime.Parse(client.globals.Settings.StartTime.ToShortTimeString());
					client.startScheduler.OnAlarmBell += new EventHandler(startScheduler_OnAlarmBell);
					client.startScheduler.Enabled = true;
					client.startScheduler.Start();
					client.stopScheduler = new AlarmTimer();
					client.stopScheduler.AlarmTime = DateTime.Parse(client.globals.Settings.StopTime.ToShortTimeString());
					client.stopScheduler.OnAlarmBell += new EventHandler(stopScheduler_OnAlarmBell);
					client.stopScheduler.Enabled = true;
					client.stopScheduler.Start();
					if((DateTime.Now > client.startScheduler.AlarmTime)&&(DateTime.Now<client.stopScheduler.AlarmTime))
					{
						client.startScheduler.AlarmTime.AddDays(1);
						client.stopScheduler.AlarmTime.AddDays(1);
						client.crawler = Crawler.Instance();
						if(client.globals.Client_Info.UserID!=0)
						{
							client.crawler.Start();
						}
					}
				}
				else
				{
					client.crawler = Crawler.Instance();
					if(client.globals.Client_Info.UserID!=0)
					{
						client.crawler.Start();
					}
				}
				//Attach the System Shutdown detection observer
				//SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
				//Allow the control of the client through Remoting
				RemotingConfiguration.Configure(client.globals.AppPath + "CrawlWave.Client.exe.config", true);
				while(!client.mustTerminate)
				{
					//Wait until it's time to exit
					Thread.Sleep(ExponentialBackoff.DefaultBackoff);
					if(Environment.HasShutdownStarted)
					{
						break;
					}
				}
			}
			catch(Exception e)
			{
				MessageBox.Show("CrawlWave Client failed to start:\n" + e.Message, "CrawlWave Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Checks if there is another instance of the Client running, either in GUI mode
		/// or in quiet mode.
		/// </summary>
		/// <param name="GUIMode">The mode in which to check if there are other instances running.</param>
		/// <returns>True if there is another instance of the Client running in the given mode.</returns>
		private static bool CheckForRunningInstances(bool GUIMode)
		{
			bool retVal=false;
			try
			{
				Process CurrentProcess=Process.GetCurrentProcess();
				Process []ClientProcesses=Process.GetProcessesByName("CrawlWave.Client");
				if (ClientProcesses.Length==1)
				{
					//there is no other instance of the Client running :)
					retVal=false;
				}
				if(GUIMode)
				{
					//try to see if the other instances are running in GUI mode
					foreach(Process ClientProcess in ClientProcesses)
					{
						try
						{
							if (ClientProcess.Id==CurrentProcess.Id)
							{
								//it's not meaningful to check the same instance!
								continue;
							}
							if (ClientProcess.MainWindowTitle!="")
							{
								//if there is another instance of the Client running in GUI mode this one must terminate
								MessageBox.Show("Another instance of CrawlWave Client is already running!\n This instance will now terminate.", "CrawlWave Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
								retVal=true;
								break;
							}
						}
						catch
						{
							//try checking the next process
							continue;
						}
					}
				}
				else //Quiet mode (no GUI)
				{
					//try to see if the other instances are running in quiet mode
					foreach(Process ClientProcess in ClientProcesses)
					{
						try
						{
							if (ClientProcess.Id==CurrentProcess.Id)
							{
								//it's not meaningful to check the same instance!
								continue;
							}
							if (ClientProcess.MainWindowTitle!="")
							{
								//if there is another instance of the Client running
								//in GUI mode wait for it to terminate
								ClientProcess.WaitForExit();
							}
							else
							{
								//if there is another instance of the client running
								//in quiet mode this instance must terminate
								System.Windows.Forms.MessageBox.Show("Another instance of CrawlWave Client is already running!\n This instance will now terminate.", "CrawlWave Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
								retVal=true;
							}
						}
						catch
						{
							//try checking the next process
							continue;
						}
					}
				}
			}
			catch
			{
				//assume that it's not safe to run
				retVal=true;
			}
			return retVal;
		}

		#endregion

		#region Event handlers

		/// <summary>
		/// Provides a default Exception Handler for unhandled exceptions.
		/// </summary>
		/// <param name="sender">The object related to the event.</param>
		/// <param name="e">The arguments related to the event.</param>
		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				MessageBox.Show("An error occured in CrawlWave Client:\n" + ((Exception)e.ExceptionObject).Message, "CrawlWave Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch
			{}
		}

		/// <summary>
		/// Provides an Exception Handler for ApplicationThreadExceptions.
		/// </summary>
		/// <param name="sender">The object related to the event.</param>
		/// <param name="e">The arguments related to the event.</param>
		private static void ApplicationThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			try
			{
				MessageBox.Show("An error occured in CrawlWave Client:\n" + ((Exception)e.Exception).Message, "CrawlWave Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch
			{}
		}

		/// <summary>
		/// Handles the SessionEnding events, allowing the client to terminate if the system
		/// is shutting down but not be affected if the user is simply logging off.
		/// </summary>
		/// <param name="sender">The object related to the event.</param>
		/// <param name="e">The arguments related to the event.</param>
		private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
		{
			try
			{
				if(e.Reason == SessionEndReasons.SystemShutdown)
				{
					Client client = Client.Instance();
					client.mustTerminate = true;
					e.Cancel = false;
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Handles the time scheduler events that start the crawler
		/// </summary>
		/// <param name="sender">The object related to the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private static void startScheduler_OnAlarmBell(object sender, EventArgs e)
		{
			instance.startScheduler.Enabled = false;
			instance.crawler = Crawler.Instance();
			instance.crawler.Start();
			instance.stopScheduler.Enabled = true;
		}

		/// <summary>
		/// Handles the time scheduler events that stop the crawler.
		/// </summary>
		/// <param name="sender">The object related to the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private static void stopScheduler_OnAlarmBell(object sender, EventArgs e)
		{
            instance.stopScheduler.Enabled = false;
			if(Crawler.InstanceExists())
			{
				instance.crawler = Crawler.Instance();
				instance.crawler.StopImmediately();
			}
			instance.startScheduler.Enabled = true;
		}

		#endregion

	}
}
