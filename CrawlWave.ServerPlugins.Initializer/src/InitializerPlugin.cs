using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerPlugins.Initializer
{
	/// <summary>
	/// InitializerPlugin is a Plugin that allows the administrator of the system to start
	/// the crawling process by initializing the database using a plain text file containing
	/// a list of Urls.
	/// </summary>
	/// <remarks>
	/// The <see cref="InitializerPlugin"/> is a class derived from <see cref="PluginBase"/>
	/// that implements <see cref="IPlugin"/>. It can be dynamically loaded by an <see cref="IPluginHost"/>
	/// and is capable of storing and loading its settings. It requires access to the system's
	/// database. If Url Cleaning is enabled it can store a list of the active Urls to a text
	/// file selected by the administrator. If Url Checking is enabled it automatically stores
	/// the active Urls in the database thus initializing it and allowing the crawling process
	/// to begin.
	/// </remarks>
	[CrawlWavePlugin]
	public class InitializerPlugin : PluginBase, IPlugin
	{
		#region Private variables
		
		private PluginSettings settings;
		private BannedHostsCache banned;
		private Thread [] pluginThreads;
		private StreamWriter outputStream;
		private Queue urlsToVisit;
		private bool mustStop;
		private int runningThreads;
		private int totalDomains;
		private int processedDomains;
		private static string UserAgent = String.Intern("CrawlWave/1.2 (crawlwave[at]spiderwave.aueb.gr http://www.spiderwave.aueb.gr/");
		private static string domain = ".gr";
		private static string http = "http";
		private static string httpscheme = "http://";
		private static string httpMethod = "HEAD";
		private SqlConnection dbcon;
		private SqlCommand urlcmd;
		private SqlCommand hostcmd;
		private object sync;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructs a new instance of the <see cref="CrawlWave.ServerPlugins.Initializer.InitializerPlugin"/> class.
		/// </summary>
		public InitializerPlugin()
		{
			dataDependent = true;
			description = "CrawlWave Initializer Plugin";
			enabled = true;
			host = null;
			name = "CrawlWave.ServerPlugins.Initializer";
			percent = 0;
			settingsPath = String.Empty;
			state = PluginState.Stopped;
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			settings = PluginSettings.Instance();
			pluginThreads = null;
			outputStream = null;
			urlsToVisit = new Queue();
			mustStop = false;
			runningThreads = 0;
			totalDomains = 0;
			processedDomains = 0;
			banned = BannedHostsCache.Instance();
			dbcon = null;
			urlcmd = null;
			hostcmd = null;
			sync = new object();
		}

		#endregion

		#region IPlugin Members

		/// <summary>
		/// Starts the process of the cleaning and checking of urls the urls contained in
		/// the input file. If the plugin is already in the <see cref="PluginState.Running"/>
		/// state then it has no effect.
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
				AddToReportQueue(CWLoggerEntryType.Info, "CrawlWave Initializer Plugin is starting.");
				mustStop = false;
				if(!ConnectToDatabase())
				{
					throw new ApplicationException();
				}
				if(!InitializeCommands())
				{
					throw new ApplicationException();
				}
				Thread loadThread = new Thread(new ThreadStart(LoadFile));
				loadThread.Priority = ThreadPriority.Lowest; //set this to normal if the
				//input file must be loaded completely before visiting/checking any urls
				loadThread.IsBackground = true;
				loadThread.Name = "Initializer Plugin Input File Loader thread";
				loadThread.Start();
				//if necessary open the output file
				if(settings.CleanUrls)
				{
					outputStream = new StreamWriter(settings.OutpufFile);
				}
				//create the threads that will be visiting hosts
				pluginThreads = new Thread[settings.Threads];
				for(int i = 0; i < settings.Threads; i++)
				{
					pluginThreads[i] = new Thread(new ThreadStart(PerformInitialization));
					pluginThreads[i].IsBackground = true;
					pluginThreads[i].Priority = ThreadPriority.Lowest;
					pluginThreads[i].Name = "Initialization Plugin Worker Thread " + i.ToString();
					pluginThreads[i].Start();
				}
				//Notify the clients that the plugin's process has started
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Error, "The Initializer Plugin failed to start: " + ex.ToString());
				mustStop = true;
				state = PluginState.Stopped;
				try
				{
					StopThreads();
				}
				catch(Exception exc)
				{
					ReportImmediately(CWLoggerEntryType.Error, "The Initializer Plugin failed to stop all threads: " + exc.ToString());
				}
			}
			finally
			{
				Report();
			}
		}

		/// <summary>
		/// Pauses the database initialization process by calling <see cref="Thread.Suspend"/>
		/// on the running threads. If the plugin is already in the <see cref="PluginState.Paused"/>
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
				AddToReportQueue(CWLoggerEntryType.Info, "The Initializer Plugin is attempting to pause.");
				state = PluginState.Paused;
				SuspendThreads();
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception e)
			{
				//if something goes wrong return to the Running state
				ReportImmediately(CWLoggerEntryType.Error, "The Initializer Plugin failed to pause: " + e.ToString());
				state = PluginState.Running;
			}
			finally
			{
				//hmmm....
				Report();
			}
		}

		/// <summary>
		/// Resumes the database initialization process if it has been paused.
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
				AddToReportQueue(CWLoggerEntryType.Info, "The Initializer Plugin is attempting to resume.");
				state = PluginState.Running;
				ResumeThreads();
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception e)
			{
				//if something goes wrong return to the Paused state
				ReportImmediately(CWLoggerEntryType.Error, "The Initializer Plugin failed to resume: " + e.ToString());
				state = PluginState.Paused;
			}
			finally
			{
				//hmmm....
				Report();
			}
		}

		/// <summary>
		/// Stops the database initialization process. If the plugin is already in the 
		/// <see cref="PluginState.Stopped"/> state it has no effect. If the database
		/// initialization is in progress it is not stopped abruptly but the method waits
		/// until the process of the currently running threads is complete.
		/// </summary>
		public void Stop()
		{
			try
			{
				mustStop = true;
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Info, DateTime.Now, "Stopping Initializer plugin."));
				//wait for all the threads to finish
				while(runningThreads > 0)
				{
					Thread.Sleep(1000); //TODO: Backoff
				}
				//Stop all threads
				StopThreads();
				//close the output stream if it is open
				if(settings.CleanUrls)
				{
					outputStream.Close();
				}
				//close the connection to the database
				DisposeCommands();
				DisconnectFromDatabase();
				state = PluginState.Stopped;
				//notify the other classes
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Warning, "The Initializer plugin failed to stop: " + ex.ToString());
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
		/// Performs the initialization process
		/// </summary>
		private void PerformInitialization()
		{
			Interlocked.Increment(ref runningThreads);
			try
			{
				while(!mustStop)
				{
					try
					{
						string url = GetUrlToVisit();
						if(url!=String.Empty)
						{
							VisitUrl(url);
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
					catch(Exception e)
					{
						if((e is ThreadAbortException)||(e is ThreadInterruptedException))
						{
							throw e;
						}
					}
					Report();
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
				Interlocked.Decrement(ref runningThreads);
			}
		}

		/// <summary>
		/// Dequeues a url from the queue in a thread-safe manner
		/// </summary>
		/// <returns>The url to be visited or an empty string if something goes wrong.</returns>
		private string GetUrlToVisit()
		{
			string currentUrl=String.Empty;
			try
			{
				//try to get a url from the queue
				try
				{
					lock(urlsToVisit.SyncRoot)
					{
						if (urlsToVisit.Count>0)
						{
							currentUrl=(string)urlsToVisit.Dequeue();
						}
						else
						{
							//if the queue is empty then the plugin must stop
							mustStop = true;
						}
					}
				}
				catch
				{
					//if something goes wrong a null reference will be returned
				}
			}
			catch
			{
				//Any Threading Exception will be caught here
			}
			return currentUrl;
		}

		/// <summary>
		/// Visits a url and if necessary stores it in the output file and the database
		/// </summary>
		/// <param name="url">The url to visit</param>
		private void VisitUrl(string url)
		{
			try
			{
				if (url==String.Empty)
				{
					throw new Exception("VisitUrl received an empty url!");
				}
				Uri uri = null;
				try
				{
					uri=new Uri(url);
					if(banned.IsBanned(uri.Host))
					{
						throw new UriFormatException("Banned host encountered");
					}
				}
				catch(UriFormatException e)
				{
					Interlocked.Increment(ref processedDomains);
					throw e; //cause the function to exit
				}
				//create the Web Request and get the header
				HttpWebRequest pageRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
				pageRequest.UserAgent = UserAgent;
				pageRequest.Timeout=10000; //don't wait more than 10 seconds for a page
				pageRequest.Method=httpMethod;
				HttpWebResponse pageResponse=null;
				try
				{
					pageResponse = (HttpWebResponse)pageRequest.GetResponse();
				}
				catch //either WebException or UriFormatException
				{
					Interlocked.Increment(ref processedDomains);
				}
				if(pageResponse!=null)
				{
					if (pageResponse.ResponseUri.AbsoluteUri!=url)
					{
						if(!pageResponse.ResponseUri.AbsoluteUri.StartsWith(url))
						{
							url=pageResponse.ResponseUri.AbsoluteUri;
						}
					}
					pageResponse.Close();
					//We must store the Url in the database and if necessary in the output file
					lock(sync)
					{
						InsertUrlInDatabase(url);
					}
					if(settings.CleanUrls)
					{
						lock(outputStream)
						{
							outputStream.WriteLine(url);
							outputStream.Flush();
						}
					}
					Interlocked.Increment(ref processedDomains);
				}
			}
			catch(Exception ex)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "Initializer Plugin VisitUrl failed: " + ex.ToString());
			}
			finally
			{
				percent = (int)(((long)processedDomains*100)/totalDomains);
			}
		}

		/// <summary>
		/// Inserts a new Url in the database
		/// </summary>
		/// <param name="url">The url to be inserted in the database.</param>
		private void InsertUrlInDatabase(string url)
		{
			try
			{
				Uri uri = new Uri(url);
				byte flagDomain = 0;
				string hostname = uri.Host;
				if(!hostname.EndsWith(domain))
				{
					flagDomain = (byte)DomainFlagValue.Unknown;
				}
				Guid host_id = new Guid(MD5Hash.md5(hostname));
				hostcmd.Parameters[0].Value = host_id;
				hostcmd.Parameters[1].Value = hostname;
				try
				{
					hostcmd.ExecuteNonQuery();
					//an exception will occur if the host already exists
				}
				catch(Exception e)
				{
					AddToReportQueue(CWLoggerEntryType.Warning, "CrawlWave Initializer Plugin failed to insert host " + hostname +": " + e.ToString());
				}

				urlcmd.Parameters[0].Value = url;
				urlcmd.Parameters[1].Value = new Guid(MD5Hash.md5(url));
				urlcmd.Parameters[2].Value = host_id;
				urlcmd.Parameters[3].Value = 1;
				urlcmd.Parameters[4].Value = flagDomain;
				urlcmd.Parameters[5].Value = 0;
				try
				{
					urlcmd.ExecuteNonQuery();
				}
				catch(Exception e)
				{
					AddToReportQueue(CWLoggerEntryType.Warning, "CrawlWave Initializer Plugin failed to insert url " + url + ": " + e.ToString()); 
				}
			}
			catch(Exception ex)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "CrawlWave Initializer plugin failed to insert a url to the database: " + ex.ToString());
			}
		}

		/// <summary>
		/// Attempts to open a connection to the database.
		/// </summary>
		/// <returns>True if it succeeds, false otherwise.</returns>
		private bool ConnectToDatabase()
		{
			try
			{
				dbcon = new SqlConnection(settings.ProvideDBConnectionString());
				dbcon.Open();
				return true;
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "CrawlWave Initializer plugin failed to connect to the database: " + e.ToString());
				return false;
			}
		}

		/// <summary>
		/// Attempts to create and initialize the SqlCommands needed by the plugin.
		/// </summary>
		/// <returns>True it it succeeds, false otherwise.</returns>
		private bool InitializeCommands()
		{
			try
			{
				//initialize the commands
				urlcmd = new SqlCommand("cw_insert_url", dbcon);
				urlcmd.CommandType = CommandType.StoredProcedure;
				urlcmd.Parameters.Add("@url", SqlDbType.NVarChar, 500);
				urlcmd.Parameters.Add("@url_md5", SqlDbType.UniqueIdentifier);
				urlcmd.Parameters.Add("@url_host_id", SqlDbType.UniqueIdentifier);
				urlcmd.Parameters.Add("@url_priority", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@flag_domain", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@flag_robots", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@id", SqlDbType.Int);
				urlcmd.Parameters["@id"].Direction = ParameterDirection.Output;

				hostcmd = new SqlCommand("cw_insert_host", dbcon);
				hostcmd.CommandType = CommandType.StoredProcedure;
				hostcmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
				hostcmd.Parameters.Add("@host_name", SqlDbType.NVarChar, 500);

				return true;
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Error, "CrawlWave Initializer plugin failed to create the SqlCommands: " + e.ToString());
				return false;
			}
		}

		/// <summary>
		/// Attempts to dispose the SqlCommands used by the plugin.
		/// </summary>
		/// <returns>True on success, false otherwise.</returns>
		private bool DisposeCommands()
		{
			bool retVal = true;
			try
			{
				if(urlcmd != null)
				{
					urlcmd.Dispose();
				}
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Error, "CrawlWave Initializer plugin failed to dispose an SqlCommand: " + e.ToString());
				retVal = false;
			}
			try
			{
				if(hostcmd!=null)
				{
					hostcmd.Dispose();
				}
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Error, "CrawlWave Initializer plugin failed to dispose an SqlCommand: " + e.ToString());
				retVal = false;
			}
			return retVal;
		}

		/// <summary>
		/// Attempts to close the connection to the database.
		/// </summary>
		/// <returns>True on success, false if it fails.</returns>
		private bool DisconnectFromDatabase()
		{
			bool retVal = true;
			if(dbcon!=null)
			{
				try
				{
					dbcon.Close();
					dbcon.Dispose();
				}
				catch(Exception e)
				{
					AddToReportQueue(CWLoggerEntryType.Error, "CrawlWave Initializer plugin failed to disconnect from the Database: " + e.ToString());
					retVal = false;
				}
				GC.Collect();
			}
			return retVal;
		}

		/// <summary>
		/// Loads the input file and initializes the queue. Runs on a separate thread.
		/// </summary>
		private void LoadFile()
		{
			try
			{
				StreamReader input = new StreamReader(settings.InputFile);
				string url = String.Empty;
				while(input.Peek() > -1 )
				{
					url = input.ReadLine().Trim();
					if(!url.StartsWith(http))
					{
						url = httpscheme + url;
					}
					if(!url.EndsWith("/"))
					{
						try
						{
							url = url + "/";
							lock(urlsToVisit.SyncRoot)
							{
								if(!urlsToVisit.Contains(url))
								{
									urlsToVisit.Enqueue(url);
								}
							}
							Interlocked.Increment(ref totalDomains);
						}
						catch
						{
							continue;
						}
					}
					else
					{
						try
						{
							lock(urlsToVisit.SyncRoot)
							{
								if(!urlsToVisit.Contains(url)) //this introduces much delay
								{
									urlsToVisit.Enqueue(url);
								}
							}
							Interlocked.Increment(ref totalDomains);
						}
						catch
						{
							continue;
						}
					}
				}
				input.Close();
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "CrawlWave Initializer Plugin failed to load the input file: " + e.ToString());
			}
			finally
			{
				AddToReportQueue(CWLoggerEntryType.Info, Thread.CurrentThread.Name + " has stopped.");
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
			for(int i = 0; i< settings.Threads; i++)
			{
				if(pluginThreads[i].IsAlive)
				{
					pluginThreads[i].Join(15000);
				}
			}
		}

		/// <summary>
		/// Suspends the plugin's threads
		/// </summary>
		private void SuspendThreads()
		{
			for(int i = 0; i< settings.Threads; i++)
			{
				if(pluginThreads[i].IsAlive)
				{
					pluginThreads[i].Suspend();
				}
			}
		}

		/// <summary>
		/// Resumes the plugin's suspended threads
		/// </summary>
		private void ResumeThreads()
		{
			for(int i=0; i<settings.Threads; i++)
			{
				if((pluginThreads[i].ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested)) >0)
				{
					pluginThreads[i].Resume();
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
