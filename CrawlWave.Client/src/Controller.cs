using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using CrawlWave.Common;
using CrawlWave.Client.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.Client
{
	/// <summary>
	/// Controller is a remotable object that implements <see cref="ICrawlerController"/>.
	/// It is used as an interface to the Crawler and is able to control its operation and
	/// retrieve its status. It should be used as a Singleton Remoting Service.
	/// </summary>
	public class Controller : MarshalByRefObject, ICrawlerController
	{
		#region Private variables

		private Globals globals;
		private QueueEventLogger log;
		private Crawler crawler;
		private long [] stats;
		private ICrawlWaveServer proxy;

		#endregion

		#region Constructor and Lifetime Initialization methods

		/// <summary>
		/// Creates a new instance of the <see cref="Controller"/> class.
		/// </summary>
		public Controller()
		{
			globals = Globals.Instance();
			log = new QueueEventLogger(100);
			crawler = null;
			stats = new long[10];
			proxy = CrawlWaveServerProxy.Instance(globals);
		}

		/// <summary>
		/// Initializes the remotable object's LifeTimeService.
		/// </summary>
		/// <returns>Null, thus causing the object's lifetime never to expire.</returns>
		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		#region ICrawlerController Members

		/// <summary>
		/// Retrieves the crawler's statistics as an array of longs.
		/// </summary>
		/// <returns>An array of longs containing the crawler's statistics.</returns>
		public long[] GetStatistics()
		{
			if(Crawler.InstanceExists())
			{
				if(crawler == null)
				{
					crawler = Crawler.Instance();
					AttachObservers();
				}
				for(int i = 0; i<10; i++)
				{
					stats[i] = crawler.Statistics[i];
				}
			}
			return stats;
		}

		/// <summary>
		/// Provides access to the Client's settings.
		/// </summary>
		/// <returns>A <see cref="ClientSettings"/> object containing the Client's settings.</returns>
		public ClientSettings GetSettings()
		{
			return globals.Settings;
		}

		/// <summary>
		/// Attempts to retrieve the user's statistics from the server.
		/// </summary>
		/// <param name="stats">The statistics of the user.</param>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException GetUserStatistics(ref UserStatistics stats)
		{
			SerializedException sx = null;
			try
			{
				//WebServiceProxy proxy = WebServiceProxy.Instance();
				UserStatistics userstats = null;
				sx = proxy.SendUserStatistics(globals.Client_Info, out userstats);
				if(sx!=null)
				{
					log.LogError("An error occured while retrieving the statistics:" + sx.Message);
					globals.FileLog.LogWarning("CrawlWave.Client: Failed to retrieve user's statistics: " + sx.Message); 
					stats = userstats;
				}
			}
			catch(Exception e)
			{
				log.LogWarning("An error occured while retrieving the statistics: " + e.Message); 
				globals.FileLog.LogWarning("CrawlWave.Client: Failed to retrieve user's statistics: " + e.ToString());
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.StackTrace);
			}
			finally
			{
				GC.Collect();
			}
			return sx;
		}

		/// <summary>
		/// Gets the current state of the Crawler.
		/// </summary>
		/// <returns>The Crawler's <see cref="CrawlerState"/>.</returns>
		/// <remarks>If the crawler has not been initialized yet the method always returns
		/// <see cref="CrawlerState.Stopped"/>.</remarks>
		public CrawlerState GetState()
		{
			if(crawler!=null)
			{
				return crawler.State;
			}
			else
			{
				return CrawlerState.Stopped;
			}
		}

		/// <summary>
		/// Retrieves the amount of (virtual) memory consumed by the application.
		/// </summary>
		/// <returns>An integer value containing the amount of memory consumed by the
		/// application in KB.</returns>
		public int GetMemoryUsage()
		{
			int memory = 0;
			try
			{
				memory = (int)Process.GetCurrentProcess().PrivateMemorySize64/1024;
			}
			catch
			{}
			return memory;
		}

		/// <summary>
		/// Provides access to the Client's event log.
		/// </summary>
		/// <returns>A <see cref="Queue"/> containing <see cref="EventLoggerEntry"/> objects,
		/// holding the events logged by the Client.</returns>
		public Queue<EventLoggerEntry> GetEventQueue()
		{
			Queue<EventLoggerEntry> retVal = new Queue<EventLoggerEntry>();
			try
			{
				while(log.Count>0)
				{
					retVal.Enqueue(log.Dequeue());
				}
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Sets the Client's settings and stores them.
		/// </summary>
		/// <param name="settings">The new <see cref="ClientSettings"/> to be assigned to the Cleint.</param>
		public void SetSettings(ClientSettings settings)
		{
			try
			{
				globals.Settings.ConnectionSpeed = settings.ConnectionSpeed;
				globals.Settings.Email = settings.Email;
				globals.Settings.EnableScheduler = settings.EnableScheduler;
				globals.Settings.LoadAtStartup = settings.LoadAtStartup;
				globals.Settings.MinimizeOnExit = settings.MinimizeOnExit;
				globals.Settings.MinimizeToTray = settings.MinimizeToTray;
				globals.Settings.StartTime = settings.StartTime;
				globals.Settings.StopTime = settings.StopTime;
				globals.Settings.SaveSettings();
			}
			catch(Exception e)
			{
				log.LogWarning("Failed to save the application's settings: " + e.Message);
			}
		}

		/// <summary>
		/// Attempts to perform the registration of a new user.
		/// </summary>
		/// <param name="UserName">The user's username.</param>
		/// <param name="Password">The user's password.</param>
		/// <param name="Email">The user's email address.</param>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException RegisterUser(string UserName, string Password, string Email)
		{
			SerializedException sx = null;
			try
			{
				//WebServiceProxy proxy = WebServiceProxy.Instance();
				int ID = 0;
				byte [] password = MD5Hash.md5(Password);
				sx = proxy.RegisterUser(ref ID, UserName, password, Email);
				if(sx!=null)
				{
					if(sx.Type == "CrawlWave.Common.CWUserExistsException")
					{
						log.LogWarning("User already exists, attempting to register client.");
					}
				}
				globals.Settings.UserID = ID;
				globals.Settings.UserName = UserName;
				globals.Settings.Password = password;
				globals.Settings.Email = Email;
				CWComputerInfo info = ComputerInfo.GetComputerInfo();
				globals.Settings.HardwareInfo = ComputerInfo.GetSHA1HashCode(info);
				globals.Settings.SaveSettings();
				//proxy.ForceInitializeProxies();
				ClientInfo ci = new ClientInfo();
				ci.UserID = globals.Settings.UserID;
				sx = proxy.RegisterClient(ref ci, info);
				globals.Settings.ClientID = ci.ClientID;
				globals.Settings.SaveSettings();
			}
			catch(Exception ex)
			{
				sx = new SerializedException(ex.GetType().ToString(), ex.Message, ex.StackTrace);
			}
			return sx;
		}

		/// <summary>
		/// Attempts to start the Crawler and enable the logging of events.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public SerializedException Start()
		{
			SerializedException sx = null;
			try
			{
				if(crawler == null)
				{
					crawler = Crawler.Instance();
					AttachObservers();
				}
				crawler.Start();
			}
			catch(Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.StackTrace);
			}
			return sx;
		}

		/// <summary>
		/// Attempts to pause the Crawler.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the Crawler has not yet been initialized.</exception>
		public SerializedException Pause()
		{
			SerializedException sx = null;
			try
			{
				if(Crawler.InstanceExists())
				{
					if(crawler == null)
					{
						crawler = Crawler.Instance();
						AttachObservers();
					}
					crawler.Pause();
				}
				else
				{
					throw new InvalidOperationException("The Crawler has not been initialized and cannot be paused.");
				}
			}
			catch(Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.StackTrace);
			}
			return sx;
		}

		/// <summary>
		/// Attempts to resume the Crawler if if has been paused.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the Crawler has not yet been initialized.</exception>
		public SerializedException Resume()
		{
			SerializedException sx = null;
			try
			{
				if(Crawler.InstanceExists())
				{
					if(crawler == null)
					{
						crawler = Crawler.Instance();
						AttachObservers();
					}
					crawler.Resume();
				}
				else
				{
					throw new InvalidOperationException("The Crawler has not been initialized and cannot be resumed.");
				}
			}
			catch(Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.StackTrace);
			}
			return sx;
		}

		/// <summary>
		/// Attempts to stop the Crawler.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the Crawler has not yet been initialized.</exception>
		public SerializedException Stop()
		{
			SerializedException sx = null;
			try
			{
				if(Crawler.InstanceExists())
				{
					if(crawler == null)
					{
						crawler = Crawler.Instance();
						AttachObservers();
					}
					crawler.StopImmediately();
				}
				else
				{
					throw new InvalidOperationException("The Crawler has not been initialized and cannot be stopped.");
				}
			}
			catch(Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.StackTrace);
			}
			return sx;
		}

		/// <summary>
		/// Attempts to terminate the application.
		/// </summary>
		/// <returns>Null if the operation succeeds, or a <see cref="SerializedException"/> 
		/// encapsulating the error that occured if the operation fails.</returns>
		public void Terminate(ref SerializedException sx)
		{
			try
			{
				//System.Windows.Forms.Application.Exit();
				Client.Instance().MustTerminate = true;
			}
			catch(Exception e)
			{
				sx = new SerializedException(e.GetType().ToString(), e.Message, e.StackTrace);
			}
		}		

		#endregion

		#region Private Methods

		/// <summary>
		/// Attaches the observers needed in order to log the Crawler's events.
		/// </summary>
		private void AttachObservers()
		{
			try
			{
				crawler.UrlSetReceived += new EventHandler(crawler_UrlSetReceived);
				crawler.ResultsSent += new EventHandler(crawler_ResultsSent);
				crawler.StateChanged += new EventHandler(crawler_StateChanged);
				crawler.UrlProcessed += new ParserEventHandler(crawler_UrlProcessed);
			}
			catch
			{}
		}

		#endregion

		#region Event Handlers

		private void crawler_UrlSetReceived(object sender, EventArgs e)
		{
			log.LogInfo("Crawler received a new set of Urls to Crawl.");
		}

		private void crawler_ResultsSent(object sender, EventArgs e)
		{
            log.LogInfo("Crawler sent a packet of Crawl Results to the Server.");
		}

		private void crawler_StateChanged(object sender, EventArgs e)
		{
            log.LogInfo("Crawler's state changed to " + crawler.State.ToString());
		}

		private void crawler_UrlProcessed(object sender, ParserEventArgs e)
		{
            log.LogInfo("Crawler processed url " + e.Url);
		}

		#endregion
	}
}
