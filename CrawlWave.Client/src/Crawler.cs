using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using CrawlWave.Common;
using CrawlWave.Client.Common;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using CrawlWave.ServerCommon;

namespace CrawlWave.Client
{
	/// <summary>
	/// Crawler is the class that performs the crawling of web pages. It is a multithreaded
	/// class that provides a control interface so that the scheduler can start or stop him
	/// at desired times. It also publishes some events that allow the easy notification of
	/// other classes, and are more useful in updating the User Interface. It also collects
	/// some statistics about the crawling process that are exposed as public properties.
	/// </summary>
	/// <remarks>
	/// The Crawler class communicates asynchronously with <c>CrawlWave.Server</c>,
	/// and the results are stored on disk in case the server is unable to respond in time
	/// and timeouts occur. The results are sent by a thread that is dedicated to that task
	/// so that the crawling process will not be affected.<br/><br/>
	/// <b>Update History:</b>
	/// <list type="table">
	///   <listheader>
	///		<term>Date</term>
	///		<description>Description</description>
	///   </listheader>
	///   <item>
	///		<term>03/08/05</term>
	///		<description>Fixed a bug that would cause Crawling threads to die unexpectedly
	///		because gzip compressed streams would not be handled properly. This revision
	///		uses SharpZipLib to decompress the gzipped streams and then performs standard
	///		processing on their content.
	///		</description>
	///   </item>
	///   <item>
	///		<term>09/02/05</term>
	///		<description>General revision. Changes include improvements in url filtering,
	///		automatic detection of the charset/encoding of the crawled data, limiting of
	///		the amount of data that can be returned by each url, added InstanceExists, etc.
	///		</description>
	///   </item>
	///   <item>
	///		<term>16/10/04</term>
	///		<description>Wired up the client with the Web Service (CrawlWave.Server).
	///		</description>
	///   </item>
	///   <item>
	///		<term>22/09/04</term>
	///		<description>Initial release. The class has been rewritten from scratch in order
	///		to achieve better performance and reliability. The whole class architecture has
	///		been revised and the synchronization mechanisms are employed a lot more heavily.
	///		Added support for asynchronous sending of results to server, control interface,
	///		statistics interface, events for client notification. The communication with the
	///		server is performed using Web Service Enhancements 2.0.
	///		</description>
	///   </item>
	///   <item>
	///		<term>06/09/03</term>
	///		<description>Fixed a bug that caused the HtmlParser to create wrong urls if a
	///		redirection had occured when crawling a page.
	///		</description>
	///   </item>
	///   <item>
	///		<term>25/08/03</term>
	///		<description>Added support for host anti-slammering protection and logging.
	///		</description>
	///   </item>
	///   <item>
	///		<term>18/08/03</term>
	///		<description>Updated Url Crawling method so that it can obtain the HTTP Status
	///		Code even when an exception occurs while crawling a Url.
	///		</description>
	///   </item>
	/// </list>
	/// </remarks>
	public class Crawler
	{
		#region Private variables

		// The single instance of the Crawler class.
		private static Crawler instance;
		// Indicates that the crawler must stop visiting urls
		private bool mustStop;
		// Indicates that the crawler is in the process of stopping
		private bool stopping;
		// Indicates the crawler's current state
		private CrawlerState state;
		// Holds various crawling-related statistics. Position 0: Total Urls Crawled,
		// Pos 1: HTTP 200 OK, Pos 2: HTTP 404 Not Found, Pos 3: 302 Redirect, Pos 4:
		// Unauthorized, Pos 5: Forbidden, Pos 6: HTTP request timeout, Pos 7: Service
		// Unavailable or Server Error, Pos 8: Bad Request (other), Pos 9: Total bytes.
		private long [] stats;
		// The number of crawling threads that will be used by the crawler
		private int numThreads;
		// The number of crawling threads that are currently active
		private int runningThreads;
		// The thread that will periodically try to send the crawl results to the server
		//private Thread sendResultsThread;
		//This thread will periodically donwload new sets of Urls to be crawled.
		private Thread synchronizeThread;
		// The threads that will be used to perform the crawling
		private Thread []crawlingThreads;
		// Will be used to determine the delay between requests after a failure
		private ExponentialBackoff syncBackOff, downloadBackOff;
		// The queue that will hold the list of InternetUrlToCrawl objects to be crawled
		private Queue urlsToCrawl;
		// The queue that will hold the names of the files containing the crawl results 
		// that must be sent to the server
		private Queue resultFileNames;
		// The list of crawled Urls that must be sent to the server
		private ArrayList crawledUrls;
		// The size of the queue, used for synchronization when a set of urls is dowloaded
		private int queueSize;
		// The name of the file holding the downloaded set of urls to be crawled
		private string dataFileName;
		// Provides access to the global variables and application settings
		private Globals globals;
		// Provides access to the CrawlWave Server Web Sevice
		//private WebServiceProxy proxy;
		// Used to interpret the data received
		private Encoding defaultEncoding;
		private Encoding defaultGreekEncoding;
		//Regular Expression for content-type & charset extraction
		private Regex contentRegex;
		// Used for the processing of HTML documents
		private HtmlParser htmlParser;
		// Used for the processing of text documents
		private TextParser textParser;
		// Used for the processing of PDF documents
		private PdfParser pdfParser;
		// Used for the processing of SWF documents
		private SwfParser swfParser;
		// Used for processing documents of unsupported mime types
		private NullParser nullParser;
		// Used for Robots filtering if a redirection occurs
		private RobotsFilter robotsFilter;
		// Used for Domain filtering if a redirection occurs
		private DomainFilter domainFilter;
		// Used to synchronize the requests to hosts and avoid slammering
		private HostRequestFilter hostRequestFilter;
		// Used to avoid visiting any banned hosts
		private HostBanFilter hostBanFilter;
		//the list of extensions that are not allowed in a url (because they are of no interest to us)
		private static string []SIDVarNames={"PHPSESSID","sid","jsessionid","sessionid","session","sess_id","MSCSID"};

		private ICrawlWaveServer proxy;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// Constructs a new istance of the <see cref="Crawler"/> class and initializes its
		/// properties with the default values. The constructor is private so that only the
		/// class itself can create an instance.
		/// </summary>
		private Crawler()
		{
			//first of all get a reference to the global variables because they are needed
			//in order to initialize some variables.
			globals = Globals.Instance();
			mustStop = false;
			stopping = false;
			state = CrawlerState.Stopped;
			stats = new long[10] {0,0,0,0,0,0,0,0,0,0};
			numThreads = (int)globals.Settings.ConnectionSpeed;
			runningThreads = 0;
			//sendResultsThread = null;
			synchronizeThread = null;
			crawlingThreads = null;
			syncBackOff = new ExponentialBackoff(BackoffSpeed.Declining, 30000);
			downloadBackOff = new ExponentialBackoff(BackoffSpeed.Fast);
			urlsToCrawl = new Queue();
			resultFileNames = new Queue();
			crawledUrls = new ArrayList();
			queueSize = 0;
			dataFileName = String.Empty;
			defaultEncoding = Encoding.GetEncoding("ISO-8859-7");
			defaultGreekEncoding = Encoding.GetEncoding(1253);
			contentRegex = new Regex("<meta\\s*http-equiv=([^>])*charset\\s*=\\s*([^>])*(utf-7|utf-8|utf-16|windows-1253)([^>])*>",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			htmlParser = HtmlParser.Instance();
			textParser = TextParser.Instance();
			pdfParser = PdfParser.Instance();
			swfParser = SwfParser.Instance();
			nullParser = NullParser.Instance();
			robotsFilter = RobotsFilter.Instance();
			robotsFilter.LoadEntries();
			domainFilter = DomainFilter.Instance();
			hostRequestFilter = HostRequestFilter.Instance();
			hostBanFilter = HostBanFilter.Instance();
			//proxy = WebServiceProxy.Instance();
			proxy = CrawlWaveServerProxy.Instance(globals);
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="Crawler"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="Crawler"/>.</returns>
		public static Crawler Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe.
				Mutex mutex=new Mutex();
				mutex.WaitOne();
				if( instance == null )
				{
					instance = new Crawler();
				}
				mutex.Close();
			}
			return instance;
		}

		/// <summary>
		/// Allows the clients to check if an instance of the <see cref="Crawler"/> has been
		/// created.
		/// </summary>
		/// <returns>True if an instance has been created, false otherwise.</returns>
		public static bool InstanceExists()
		{
			return (instance!=null);
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs when a change occurs to the statistics for which the UI must be notified.
		/// </summary>
		public event EventHandler StatisticsChanged;
		/// <summary>
		/// Occurs when the <see cref="Crawler"/>'s <see cref="CrawlerState"/> changes.
		/// </summary>
		public event EventHandler StateChanged;
		/// <summary>
		/// Occurs when the <see cref="Crawler"/> succeeds in returning crawled urls to the server.
		/// </summary>
		public event EventHandler ResultsSent;
		/// <summary>
		/// Occurs when the <see cref="Crawler"/> succeeds in receiving a list of urls to crawl.
		/// </summary>
		public event EventHandler UrlSetReceived;
		/// <summary>
		/// Occurs when the <see cref="Crawler"/> is done downloading and extracting links
		/// and content from a url.
		/// </summary>
		public event ParserEventHandler UrlProcessed;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a <see cref="CrawlerState"/> value indicating the <see cref="Crawler"/>'s
		/// internal state.
		/// </summary>
		public CrawlerState State
		{
			get { return state; }
		}

		/// <summary>
		/// Gets the number of currently running crawler threads
		/// </summary>
		public int RunningThreads
		{
			get { return runningThreads; }
		}

		/// <summary>
		/// Gets an array of long integers containing various statistics related to the 
		/// crawling process.
		/// </summary>
		public long [] Statistics
		{
			get { return stats; }
		}

		#endregion

		#region Public Interface

		/// <summary>
		/// Starts the crawling process. If the crawler is already in the <see cref="CrawlerState.Running"/>
		/// state it has no effect.
		/// </summary>
		/// <exception cref="CWException">Thrown if the crawler is in the <see cref="CrawlerState.Paused"/> state.</exception>
		public void Start()
		{
			if(state == CrawlerState.Running)
			{
				return;
			}
			if(state == CrawlerState.Paused)
			{
				throw new CWException("The Crawler is in the Pause mode and cannot be started.");
			}
			try
			{
				mustStop = false;
				stopping = false;
				syncBackOff.Reset();
				downloadBackOff.Reset();
				//Initialize the results queue
				InitializeResultsQueue();
				//Initialize the Urls queue
				InitializeUrlsQueue();
				//create the thread that will be downloading new urls to crawl
				synchronizeThread = new Thread(new ThreadStart(SynchronizeProcess));
				synchronizeThread.IsBackground = true;
				synchronizeThread.Priority = ThreadPriority.Lowest;
				synchronizeThread.Name = "Synchronization Thread";
				synchronizeThread.Start();
				/*//create the thread that will be sending the results to the server
				sendResultsThread = new Thread(new ThreadStart(SendResultsToServer));
				sendResultsThread.IsBackground = true;
				sendResultsThread.Priority = ThreadPriority.Lowest;
				sendResultsThread.Name = "Results Thread";
				sendResultsThread.Start();*/
				//create the threads that will perform the crawling
				crawlingThreads = new Thread[numThreads];
				for(int i=0; i<numThreads; i++)
				{
					crawlingThreads[i] = new Thread(new ThreadStart(PerformCrawling));
					crawlingThreads[i].IsBackground = true;
					crawlingThreads[i].Priority = ThreadPriority.Lowest;
					crawlingThreads[i].Name = "Crawler Thread " + i.ToString();
					crawlingThreads[i].Start();
				}
				//Notify the clients that the crawling process has started
				state = CrawlerState.Running;
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogError)
				{
					globals.FileLog.LogError("The Crawler failed to start: " + ex.ToString());
				}
				mustStop = true;
				state = CrawlerState.Stopped;
				try
				{
					StopAllThreads();
				}
				catch(Exception exc)
				{
					globals.FileLog.LogError("The Crawler failed to stop all the threads: " + exc.ToString());
				}
			}
			finally
			{
				
			}
		}

		/// <summary>
		/// Stops the crawling process. If the crawler is already in the <see cref="CrawlerState.Stopped"/>
		/// state it has no effect. If the crawling is in progress it is not stopped abruptly
		/// but the method waits until the current working Url Set is processed.
		/// </summary>
		/// <remarks>This method will be removed and replaced by StopImmediately in future versions.</remarks>
		public void Stop()
		{
			try
			{
				stopping = true;
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo("Crawler.Stop is trying to stop all threads.");
				}
				//wait for all the threads to finish
				while(urlsToCrawl.Count>0)
				{
					Thread.Sleep(3000);//TODO ExponentialBackoff.DefaultBackoff);
				}
				mustStop = true;
				while(runningThreads > 0)
				{
					Thread.Sleep(3000);//TODO ExponentialBackoff.DefaultBackoff);
				}
				stopping = false;
				//store the crawl results on disk
				StoreCrawlResults();
				robotsFilter.SaveEntries();
				//Stop all threads
				StopAllThreads();
				state = CrawlerState.Stopped;
				//notify the other classes
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("Crawler.Stop failed: " + ex.ToString());
				}
			}
			finally
			{
				queueSize = 0;
				urlsToCrawl.Clear();
				crawledUrls.Clear();
				stopping = false;
				mustStop = false;
			}
		}

		/// <summary>
		/// Stops the crawling process immediately without waiting for the crawling threads
		/// to finish. If the crawler is already in the <see cref="CrawlerState.Stopped"/>
		/// state it has no effect.
		/// </summary>
		public void StopImmediately()
		{
			try
			{
				mustStop = true;
				//Kill all the threads without waiting
				KillAllThreads();
			}
			catch
			{
				//reset runningThreads to 0 if something goes wrong
				if(runningThreads >0)
				{
					runningThreads = 0;
				}
			}
			finally
			{
				queueSize = 0;
				urlsToCrawl.Clear();
				crawledUrls.Clear();
				robotsFilter.SaveEntries();
				mustStop = false;
				state = CrawlerState.Stopped;
				OnStateChanged(EventArgs.Empty);
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo("Crawler.StopImmediately stopped all threads.");
				}
			}
		}

		/// <summary>
		/// Pauses the crawling process by calling <see cref="Thread.Suspend"/> on all the
		/// crawling threads. If the crawler is already in the <see cref="CrawlerState.Paused"/>
		/// state it has no effect.
		/// </summary>
		/// <exception cref="CWException">Thrown if the crawler is in the <see cref="CrawlerState.Stopped"/> state.</exception>
		public void Pause()
		{
			if(state == CrawlerState.Paused)
			{
				return;
			}
			if(state == CrawlerState.Stopped)
			{
				throw new CWException("The crawler is in the Stopped state and cannot be paused.");
			}
			try
			{
				state = CrawlerState.Paused;
				SuspendAllThreads();
				OnStateChanged(EventArgs.Empty);
			}
			catch
			{
				//if something goes wrong return to the Running state
				state = CrawlerState.Running;
			}
			finally
			{
				//hmmm....
			}
		}

		/// <summary>
		/// Resumes the crawling process if it has been paused.
		/// </summary>
		/// <exception cref="CWException">
		/// Thrown if the crawler is in the <see cref="CrawlerState.Stopped"/> or 
		/// <see cref="CrawlerState.Running"/> state.
		/// </exception>
		public void Resume()
		{
			if((state == CrawlerState.Stopped)||(state == CrawlerState.Running))
			{
				throw new CWException("The crawler is not in the Paused state and cannot be resumed.");
			}
			try
			{
				state = CrawlerState.Running;
				ResumeAllThreads();
				OnStateChanged(EventArgs.Empty);
			}
			catch
			{
				//if something goes wrong return to the Paused state
				state = CrawlerState.Paused;
			}
			finally
			{
				//hmmm....
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Initializes the queue containing the names of the files that store the crawl
		/// data results.
		/// </summary>
		private void InitializeResultsQueue()
		{
			try
			{
				string [] FileNames = Directory.GetFiles(globals.AppWorkPath, "results????????-????-????-????-????????????.xml");
				lock(resultFileNames.SyncRoot)
				{
					for(int i = 0; i< FileNames.Length; i++)
					{
						resultFileNames.Enqueue(FileNames[i]);
					}
				}
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("Crawler failed to initialize the Results queue: " + e.ToString());
				}
			}
		}

		/// <summary>
		/// Initializes the queue of Urls from a file on disk containing the last set of
		/// urls that the crawler downloaded but didn't have the time to crawl.
		/// </summary>
		private void InitializeUrlsQueue()
		{
			try
			{
				//Check if there is a data file on disk
				string [] FileNames = Directory.GetFiles(globals.AppWorkPath, "data????????-????-????-????-????????????.xml");
				if(FileNames.Length!=0)
				{
					dataFileName = FileNames[0];
					Stream ReadStream=File.Open(dataFileName, FileMode.Open);
					//SoapFormatter serializer=new SoapFormatter();
					BinaryFormatter serializer = new BinaryFormatter();
					//lock the url queue, enqueue the urls
					lock(urlsToCrawl.SyncRoot)
					{
						try
						{
							urlsToCrawl = (Queue)serializer.Deserialize(ReadStream);
						}
						catch(Exception e)
						{
							globals.FileLog.LogWarning("The Crawler failed to deserialize the Urls Queue: " + e.ToString());
						}
					}
					ReadStream.Close();
					queueSize = urlsToCrawl.Count;
				}
			}
			catch(Exception e)
			{
				//Something went wrong - we must silently ignore the error but log it
				globals.FileLog.LogWarning("The Crawler failed to initialize the Urls Queue: " + e.ToString());
			}
			finally
			{
				GC.Collect();
			}
		}

		/// <summary>
		/// Synchronizes the processes by waiting for all the threads to finish downloading
		/// pages before downloading a new set of urls to crawl.
		/// </summary>
		private void SynchronizeProcess()
		{
			try
			{
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo("Started Synchronize Thread with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4"));
				}
				while(!InternetUtils.ConnectedToInternet())
				{
					Thread.Sleep(ExponentialBackoff.DefaultBackoff);
				}
				while((!mustStop)||(stopping))
				{
					//if the url queue is empty
					if(urlsToCrawl.Count==0)
					{
						//but there are some url crawl results (it's not the first loop)
						if(crawledUrls.Count > 0)
						{
							//wait until all the urls have been visited
							while(crawledUrls.Count < queueSize)
							{
								Thread.Sleep(syncBackOff.Next());
							}
							StoreCrawlResults();
							SendResultsSynchronously();
							if(resultFileNames.Count>0)
							{
								SendResultsSynchronously();
							}
						}
						if(!stopping)
						{
							if((stats[0]>0)&&(stats[0]%1000==0))
							{
								robotsFilter.SaveEntries();
							}
							DownloadUrlsToCrawl(); //this also saves them on disk
							queueSize = urlsToCrawl.Count;
							GC.Collect();
						}
						syncBackOff.Reset();
					}
					else
					{
						Thread.Sleep(syncBackOff.Next()); //TODO: check this out
					}
				}
			}
			catch(ThreadAbortException)
			{
				Thread.ResetAbort();
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("Crawler.SynchronizeProcess thread is being aborted.");
				}
				return;
			}
			catch(ThreadInterruptedException)
			{
				return;
			}
			catch(Exception e)
			{
				if (!((e is ThreadAbortException) || (e is ThreadInterruptedException)))
				{
					if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
					{
						globals.FileLog.LogWarning("Crawler.SynchronizeProcess caught an exception: " + e.ToString());
					}
				}
			}
			finally
			{
				//perform clean up - dont know what kind of clean up yet.
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogInfo(Thread.CurrentThread.Name + " terminated.");
				}
			}
		}

		/// <summary>
		/// Downloads a new set of Urls to crawl from the server. Since this method may be
		/// interrupted or aborted at any time it must handle ThreadInterruptedException
		/// and ThreadAbortException.
		/// </summary>
		private void DownloadUrlsToCrawl()
		{
			if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
			{
				globals.FileLog.LogInfo("Crawler is trying to donwload a new list of Urls to crawl.");
			}
			try
			{
				//communicate with the web service and download a set of urls to crawl.
				SerializedException sx = null;
				InternetUrlToCrawl [] urls = null;
				bool success = false;
				while(!success)
				{
					sx = null;
					try
					{
						sx = proxy.SendUrlsToCrawl(globals.Client_Info, out urls);
						success = true;
						if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
						{
							globals.FileLog.LogInfo("Crawler donwloaded a new list of Urls to crawl.");
						}
						OnUrlSetReceived(EventArgs.Empty);
					}
					catch(Exception e)
					{
						//an exception will be thrown if a timeout occurs
						globals.FileLog.LogInfo(e.ToString());
						if(InternetUtils.ConnectedToInternet())
						{
							//An internet connection is available, so the server encountered an error
							//proxy.IncrementFailures(); //TODO should this be used?
						}
						Thread.Sleep(downloadBackOff.Next());
					}
					if(sx!=null)
					{
						if((urls==null) || (urls.Length == 0))
						{
							Thread.Sleep(downloadBackOff.Next());
						}
					}
				}
				if(sx!=null)
				{
					//Something went wrong - we must silently ignore the error but log it
					globals.FileLog.LogWarning("The Crawler failed to download a set of Urls to crawl: " + sx.Type + ": " + sx.Message + "\n" + sx.StackTrace);
				}
				else
				{
					//lock the url queue, enqueue the urls and cleanup the hostRequestFilter
					lock(urlsToCrawl.SyncRoot)
					{
						urlsToCrawl.Clear();
						if(urls!=null)
						{
							foreach(InternetUrlToCrawl url in urls)
							{
								if(url!=null)
								{
									urlsToCrawl.Enqueue(url);
								}
							}
						}
						if(urlsToCrawl.Count>0)
						{
							downloadBackOff.Reset();
							Stream WriteStream = null;
							try
							{
								//Store the list of urls in a file on disk
								dataFileName=globals.AppWorkPath+"data" + Guid.NewGuid().ToString() + ".xml";
								WriteStream=File.Open(dataFileName, FileMode.Create);
								//SoapFormatter serializer=new SoapFormatter();
								BinaryFormatter serializer = new BinaryFormatter();
								serializer.Serialize(WriteStream,urlsToCrawl);
							}
							catch(Exception e)
							{
								//Something went wrong - we must silently ignore the error but log it
								globals.FileLog.LogWarning("The Crawler failed to store the downloaded set of Urls to disk: " + e.ToString());
							}
							finally
							{
								if(WriteStream != null)
								{
									WriteStream.Close();
									WriteStream = null; // TODO check if this is needed
								}
							}
						}
						else
						{
							//the queue is empty, cut the server some slack
							Thread.Sleep(downloadBackOff.Next());
						}
					}
					hostRequestFilter.Clear();
				}
			}
			catch(ThreadAbortException)
			{
				//The thread was asked to abort, which means it must return at once
				return;
			}
			catch(ThreadInterruptedException)
			{
				return;
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("An unexpected error occured in DownloadUrlsToCrawl: " + e.ToString());
				}
			}
			finally
			{
				GC.Collect();
			}
		}

		/// <summary>
		/// Performs the crawling process. This is the method that all the crawler threads
		/// are running throughout the application's operation. Since these threads may be
		/// aborted or interrupted at any time this method handles ThreadAbortException and
		/// ThreadInterruptedException.
		/// </summary>
		private void PerformCrawling()
		{
			Interlocked.Increment(ref runningThreads);
			InternetUrlToCrawl urlToCrawl = null;
			try
			{
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo("Started " + Thread.CurrentThread.Name + " with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4"));
				}
				while(!InternetUtils.ConnectedToInternet())
				{
					for(int i=0; i<ExponentialBackoff.DefaultBackoff; i+=1000)
					{
                        Thread.Sleep(1000);
					}
				}
				while((!mustStop)||(stopping))
				{
					urlToCrawl = SelectUrlToCrawl();
					if(urlToCrawl != null)
					{
						//Check if the url is on a banned host
						if(hostBanFilter.FilterUrl(ref urlToCrawl))
						{
							UpdateStatistics(HttpStatusCode.Forbidden, 0);
						}
						else
						{
							//Check if less than 30 seconds have passed since the last visit
							Thread.Sleep(hostRequestFilter.FilterUrl(ref urlToCrawl));
							CrawlUrl(ref urlToCrawl);
							//raise event to notify clients
							OnUrlProcessed(new ParserEventArgs(urlToCrawl.Url));
						}
					}
					else
					{
						if(stopping)
						{
							return;
						}
						Thread.Sleep(3000);
					}
				}
			}
			catch(ThreadAbortException)
			{
				//The thread was asked to abort, which means it must return at once
				Thread.ResetAbort();
				return;
			}
			catch(ThreadInterruptedException)
			{
				//The thread has been asked to Join. This means we must add the urlToCrawl
				//back in the queue and then return. perhaps this is unnecessary.
				if(urlToCrawl !=null)
				{
					lock(urlsToCrawl.SyncRoot)
					{
						urlsToCrawl.Enqueue(urlToCrawl);
					}
				}
				return;
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("Crawler.PerformCrawling caught an exception: " + e.ToString());
				}
			}
			finally
			{
				Interlocked.Decrement(ref runningThreads);
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogInfo(Thread.CurrentThread.Name + " terminated.");
				}
			}
		}

		/// <summary>
		/// Dequeues an <see cref="InternetUrlToCrawl"/> object from the queue of the urls
		/// that must be crawled in a thread-safe manner. If the queue is empty a null
		/// reference is returned. Called from <see cref="Crawler.PerformCrawling"/>.
		/// </summary>
		/// <returns>A <see cref="InternetUrlToCrawl"/> object to be crawled if the queue of
		/// urls contains items, otherwise a null reference.</returns>
		private InternetUrlToCrawl SelectUrlToCrawl()
		{
			InternetUrlToCrawl retVal = null;
			try
			{
				lock(urlsToCrawl.SyncRoot)
				{
					if(urlsToCrawl.Count>0)
					{
						retVal = (InternetUrlToCrawl)urlsToCrawl.Dequeue();
					}
				}
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Crawls a Url and creates a <see cref="UrlCrawlData"/> object that is stored in
		/// the internal crawledUrls <see cref="ArrayList"/>. Since it runs in one of the
		/// crawling threads that may be interrupted or aborted at any time it must be able
		/// to handle ThreadAbortException and ThreadInterruptedException.
 		/// </summary>
		/// <param name="urlToCrawl">A reference to the <see cref="InternetUrlToCrawl"/>
		/// object that encapsulates the url that must be crawled.</param>
		internal void CrawlUrl(ref InternetUrlToCrawl urlToCrawl)
		{
			try
			{
				UrlCrawlData urlData = new UrlCrawlData();
				HiResTimer timer = new HiResTimer();

				//create the web request and download the data
				HttpWebRequest pageRequest = null;
				try
				{
					pageRequest = (HttpWebRequest)HttpWebRequest.Create(urlToCrawl.Url);
				}
				catch
				{
					urlData.HttpStatusCode=HttpStatusCode.BadRequest;//TODO comment
					urlData.Updated=true;
					urlData.UrlToCrawl = urlToCrawl;
					urlData.OutLinks = new InternetUrlToIndex[0];
					urlData.Data = String.Empty;
					UpdateStatistics(HttpStatusCode.BadRequest, 0);
					lock(crawledUrls.SyncRoot)
					{
						crawledUrls.Add(urlData);
					}
					return;
				}
				pageRequest.UserAgent = globals.UserAgent;
				pageRequest.Timeout=ExponentialBackoff.DefaultBackoff; //page timeout = 30 seconds
				pageRequest.KeepAlive = false;
				HttpWebResponse pageResponse=null;
				try
				{
					timer.Start();
					pageResponse = (HttpWebResponse)pageRequest.GetResponse();
					//the above line might throw either WebException or UriFormatException
				}
				catch(WebException we)
				{
					HttpWebResponse response=(HttpWebResponse)we.Response;
					if (response!=null)
					{
						//although an exception occured we're able to get the Status Code
						urlData.HttpStatusCode=response.StatusCode;
						urlData.Updated=true;
						urlData.UrlToCrawl = urlToCrawl;
						urlData.Data = String.Empty;
						urlData.OutLinks = new InternetUrlToIndex[0];
						UpdateStatistics(response.StatusCode, response.ContentLength);
						response.Close();
					}
					else
					{
						urlData.HttpStatusCode=HttpStatusCode.BadRequest;//TODO comment
						urlData.Updated=true;
						urlData.UrlToCrawl = urlToCrawl;
						urlData.Data = String.Empty;
						urlData.OutLinks = new InternetUrlToIndex[0];
						UpdateStatistics(HttpStatusCode.BadRequest, 0);
					}
				}
				catch(UriFormatException)
				{
					//this will occur if the url is not valid
					urlData.HttpStatusCode=HttpStatusCode.BadRequest;
					urlData.Updated=false;
					urlData.Data = String.Empty;
					urlData.UrlToCrawl = urlToCrawl;
					urlData.OutLinks = new InternetUrlToIndex[0];
				}
				finally
				{
					timer.Stop();
					urlData.TimeStamp = DateTime.UtcNow;
				}
				if(pageResponse !=null)
				{
					//update the fields
					urlData.HttpStatusCode = pageResponse.StatusCode;
					//download and parse the contents of the url
					Stream receiveStream=pageResponse.GetResponseStream();
					/*StreamReader receivedBytes=new StreamReader(receiveStream,defaultEncoding);*/
					MemoryStream receivedBytes = new MemoryStream();
					byte [] buffer = new byte[4096];
					int read = 0;
					try
					{
						while((read=receiveStream.Read(buffer,0,4096))>0)
						{
							receivedBytes.Write(buffer,0,read);
						}
					}
					catch
					{
						//it should be response timeout not request timeout
						urlData.HttpStatusCode = HttpStatusCode.RequestTimeout;
						urlData.Updated = true;
						urlData.RetrievalTime = (int)timer.Duration;
						urlData.Data = String.Empty;
						urlData.OutLinks = new InternetUrlToIndex[0];
						urlData.UrlToCrawl = urlToCrawl;
						try
						{
							receivedBytes.Close();
							receiveStream.Close();
							pageResponse.Close();
						}
						catch
						{}
						lock(crawledUrls.SyncRoot)
						{
							crawledUrls.Add(urlData);
						}
						UpdateStatistics(HttpStatusCode.RequestTimeout, 0);
						return;
					}
					buffer = receivedBytes.ToArray();
					Parser parser = SelectParser(pageResponse.ContentType);
					string contents = String.Empty;
					if(parser == htmlParser)
					{
						Encoding encoding = null;
						switch(pageResponse.ContentEncoding)
						{
							case "":
							case "none":
								contents = this.defaultEncoding.GetString(buffer, 0, buffer.Length);
								//re-check the encoding
								encoding = DetectContentEncoding(ref contents);
								if(encoding != defaultEncoding)
								{
									contents = encoding.GetString(buffer, 0, buffer.Length);
								}
								break;

							case "gzip":
							case "x-gzip":
								//first decompress the stream and then re-check the encoding
								byte [] decompressed_buffer = new byte [0];
								DecompressGzippedContent(buffer, out decompressed_buffer);
								contents = this.defaultEncoding.GetString(decompressed_buffer, 0, decompressed_buffer.Length);
								//re-check the encoding
								encoding = DetectContentEncoding(ref contents);
								if(encoding != defaultEncoding)
								{
									contents = encoding.GetString(decompressed_buffer, 0, decompressed_buffer.Length);
								}
								break;

							default:
								try
								{
									encoding = Encoding.GetEncoding(pageResponse.ContentEncoding);
									contents = encoding.GetString(buffer, 0, buffer.Length);
								}
								catch//(NotSupportedException)
								{
									encoding = defaultEncoding;
									//the encoding specified is unsupported.
									contents = String.Empty;
								}
								break;
						}
					}
					else
					{
						if(parser == textParser)
						{
							try
							{
								contents = this.defaultEncoding.GetString(buffer, 0, buffer.Length);
							}
							catch
							{
								//something went seriously wrong here. The crawler got a header that says the server is
								//sending back a plain text document but for some reason we can't get the string contents.
								contents = String.Empty;
							}
						}
					}
					receiveStream.Close();
					receivedBytes.Close();
					UpdateStatistics(pageResponse.StatusCode, buffer.Length);
					string redirectUrl = string.Empty;
					if (pageResponse.ResponseUri.AbsoluteUri!=urlToCrawl.Url)
					{
						redirectUrl = pageResponse.ResponseUri.AbsoluteUri;
						urlData.RedirectedPriority = htmlParser.CleanUrlParams(ref redirectUrl);//CleanupRedirectUrl(ref redirectUrl);
						if(urlToCrawl.Url != redirectUrl)//now that was a bloody BUGBUG
						{
							urlData.Redirected=true;
							urlToCrawl.Url=redirectUrl;
						}
					}
					pageResponse.Close();
					long CRC = CompressionUtils.BufferCRC(buffer);
					if(CRC != urlToCrawl.CRC)
					{
						urlData.Updated = true;
						urlToCrawl.CRC = CRC;
					}
					if(urlData.Updated)
					{
						urlData.RetrievalTime = (int)timer.Duration;
						//if redirected, calculate robots, domain & priority for redirect url
						if(urlData.Redirected)
						{							
							urlData.RedirectedFlagDomain = domainFilter.FilterUrl(ref redirectUrl);
							urlData.RedirectedFlagRobots = robotsFilter.FilterUrl(redirectUrl, urlToCrawl, RobotsMetaTagValue.NoMeta);
						}
						//perform link extraction and content extraction
						ArrayList outlinks = null;
						try
						{
							if((parser == htmlParser)||(parser == textParser))
							{
								string clean = parser.ExtractContent(ref contents, false);
								if(clean.Length>1048576)
								{
									clean = clean.Substring(0,1048576);
								}
								urlData.Data = InternetUtils.Base64Encode(clean);
								if(urlData.Data == null)
								{
									urlData.Data = String.Empty;
								}
								outlinks = parser.ExtractLinks(ref contents, ref urlToCrawl);
								if(outlinks == null)
								{
									outlinks = new ArrayList();
								}
							}
							else
							{
								contents = parser.ExtractContent(buffer, false);
								if(contents.Length>1048576)
								{
									contents = contents.Substring(0,1048576);
								}
								urlData.Data = InternetUtils.Base64Encode(contents);
								if(urlData.Data == null)
								{
									urlData.Data = String.Empty;
								}
								if(parser == pdfParser)
								{
									outlinks = textParser.ExtractLinks(ref contents, ref urlToCrawl);
								}
								else
								{
									outlinks = htmlParser.ExtractLinks(ref contents, ref urlToCrawl);
								}
								if(outlinks == null)
								{
									outlinks = new ArrayList();
								}
							}
						}
						catch
						{
							if(outlinks == null)
							{
								outlinks = new ArrayList();
							}
						}
						urlData.OutLinks = new InternetUrlToIndex[outlinks.Count];
						for(int i = 0; i< outlinks.Count; i++)
						{
							urlData.OutLinks[i] = (InternetUrlToIndex)outlinks[i];
						}
						//finally update the urlData object with the modified UrlToCrawl
						urlData.UrlToCrawl = urlToCrawl;
					}
				}
				//lock and update CrawledUrls
				lock(crawledUrls.SyncRoot)
				{
					crawledUrls.Add(urlData);
				}
			}
			catch(ThreadAbortException tae)
			{
				//The thread has been asked to abort. Log information and return at once
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo(Thread.CurrentThread.Name + " has been asked to abort: " + tae.Message);
				}
				return;
			}
			catch(ThreadInterruptedException tie)
			{
				//The thread has been asked to join. Log information and return at once
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo(Thread.CurrentThread.Name + " has been interrupted: " + tie.Message);
				}
				return;
			}
			catch(Exception ex)
			{
				if(!(ex is ThreadAbortException)) // the ThreadAbortedException is rethrown
				{
					if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
					{
						globals.FileLog.LogWarning("CrawlUrl running in " + Thread.CurrentThread.Name + " encountered an unexpected exception: " + ex.ToString());
					}
				}
				throw ex; //PerformCrawling should catch this one ?????
			}
			finally
			{
				GC.Collect();
			}
		}

		/// <summary>
		/// Selects the appropriate parser according to the content type of the document.
		/// </summary>
		/// <param name="contentType">The content-type of the document.</param>
		/// <returns>A reference to a <see cref="Parser"/> object that can parse documents
		/// of the given content-type, or null if no appropriate parser exists.</returns>
		private Parser SelectParser(string contentType)
		{
			Parser retVal = nullParser;
			//switch cannot be used because it needs contant case expressions.
			if((contentType.IndexOf(htmlParser.ContentType)!=-1)||(contentType.IndexOf(htmlParser.AlternativeContentType)!=-1)||(contentType.IndexOf(htmlParser.AlternativeContentType2)!=-1))
			{
				retVal = htmlParser;
			}
			if(contentType.IndexOf(textParser.ContentType)!=-1)
			{
				retVal = textParser;
			}
			if(contentType.IndexOf(swfParser.ContentType)!=-1)
			{
				retVal = swfParser;
			}
			if((contentType.IndexOf(pdfParser.ContentType)!=-1)||(contentType.IndexOf(pdfParser.AlternativeContentType)!=-1))
			{
				retVal = pdfParser;
			}
			return retVal;
		}

		/// <summary>
		/// Updates the statistics table every time a Url is crawled, so that the client
		/// can be informed about the status of the last request.
		/// </summary>
		/// <param name="statusCode">The <see cref="HttpStatusCode"/> returned by the last <see cref="WebRequest"/></param>
		/// <param name="contentLength">The length of the content returned during the last <see cref="WebRequest"/></param>
		private void UpdateStatistics(HttpStatusCode statusCode, long contentLength)
		{
			//increment Total Urls by 1
			Interlocked.Increment(ref stats[0]);
			//Increment the appropriate statistic accoding to the statusCode
			switch(statusCode)
			{
				case HttpStatusCode.OK:
					Interlocked.Increment(ref stats[1]);
					break;

				case HttpStatusCode.NotFound:
					Interlocked.Increment(ref stats[2]);
					break;

				case HttpStatusCode.Redirect:
					Interlocked.Increment(ref stats[3]);
					break;

				case HttpStatusCode.Unauthorized:
					Interlocked.Increment(ref stats[4]);
					break;

				case HttpStatusCode.Forbidden:
					Interlocked.Increment(ref stats[5]);
					break;

				case HttpStatusCode.GatewayTimeout:
					Interlocked.Increment(ref stats[6]);
					break;

				case HttpStatusCode.ServiceUnavailable:
					Interlocked.Increment(ref stats[7]);
					break;

				case HttpStatusCode.InternalServerError:
					Interlocked.Increment(ref stats[7]);
					break;

				default:
					Interlocked.Increment(ref stats[8]);
					break;
			}
			//Update the total bytes
			lock(stats.SyncRoot)
			{
				stats[9] = stats[9] + contentLength;
			}
			//raise an event to notify the clients
			OnStatisticsChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Attempts to detect the document encoding of HTML documents 
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		private Encoding DetectContentEncoding(ref string content)
		{
			Encoding retVal = defaultEncoding;
			try
			{
				//attempt to perform a meta http-equiv tag matching
				Match match = contentRegex.Match(content);
				if(match.Success)
				{
					//a match was found, so we must get the exact encoding
					string charset = match.Groups[3].Value.ToUpper();
					if(charset.IndexOf("1253")!=-1)
					{
						retVal = defaultGreekEncoding;
					}
					else
					{
						try
						{
							retVal = Encoding.GetEncoding(charset);
						}
						catch
						{
							retVal = defaultEncoding;
						}
					}
				}
				//if no match was found no charset conversion must be performed
				match = null;
			}
			catch
			{
				retVal = defaultEncoding;
			}
			return retVal;
		}

		/// <summary>
		/// Decompresses an array of bytes that contains Gzipped content and stores the result into
		/// a new array of bytes. It is used in order to decompress the contents of urls that some
		/// servers send gzipped in order to save bandwidth.
		/// </summary>
		/// <param name="compressed">An array of bytes containing the compressed input.</param>
		/// <param name="decompressed">The buffer where the decompressed input will be returned.</param>
		/// <remarks>
		/// The decompressed buffer is passed back to the calling method as an <b>out</b>
		/// parameter. That means that the calling method doesn't need to initialize the
		/// decompressed buffer.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the compressed input buffer is empty or null.
		/// </exception>
		/// <exception cref="CWZipException">
		/// Thrown if a problem is encountered during the decompression process.
		/// </exception>
		private void DecompressGzippedContent(byte[] compressed, out byte [] decompressed)
		{
			if((compressed==null)||(compressed.Length==0))
			{
				throw new ArgumentNullException("compressed","The compressed input buffer cannot be null or empty.");
			}
			MemoryStream ms=null, output=null;
			try
			{
				ms=new MemoryStream(compressed);
				GZipInputStream s=new GZipInputStream(ms);
				int size;
				output=new MemoryStream();
				byte []data=new byte[4096];
				while(true)
				{
					size=s.Read(data,0,data.Length);
					if (size>0)
					{
						output.Write(data,0,size);
					}
					else
					{
						break;
					}
				}
				s.Close();
				ms.Close();
				decompressed=output.ToArray();
				output.Close();
			}
			catch(Exception e)
			{
				if(ms!=null)
				{
					ms.Close();
				}
				if(output!=null)
				{
					output.Close();
				}
				throw new CWZipException(e.Message);
			}
			finally
			{
				ms = null;
				output=null;
				GC.Collect();
			}
		}

		/// <summary>
		/// Performs a processing of the GET parameters of dynamic urls. It removes any
		/// session IDs and limits the number of parameters to 3, so as to avoid urls that
		/// act as "black holes". It also removes named anchors from the end of the urls
		/// for the same reason and performs a calculation of the url's 'importance', taking
		/// into account the length of the absolute path and the number of its parameters.
		/// </summary>
		/// <remarks>
		/// Works exactly the same way as <see cref="HtmlParser.CleanUrlParams"/>.
		/// </remarks>
		/// <param name="url">The url to be processed and whose priority is wanted.</param>
		/// <returns>An unsigned 8 bit integer indicating the Url's priority.</returns>
		private byte CleanupRedirectUrl(ref string url)
		{
			byte priority = 1;
			StringBuilder sb= new StringBuilder(url);
			int pos=url.IndexOf('#'), i=0;
			if (pos!=-1)
			{
				//the url contains an anchor, so it must be cleaned
				sb.Remove(pos,url.Length-pos);
				priority++;
			}
			Uri uri=new Uri(sb.ToString());
			//calculate the absolute path's depth  and increase the priority accordingly
			if(uri.AbsolutePath.Length>1)
			{
				for(i=1; i<uri.AbsolutePath.Length; i++)
				{
					if(uri.AbsolutePath[i]=='/')
					{
						priority++;
					}
				}
			}
			//now check for url parameters
			pos=url.IndexOf('?');
			if (pos!=-1)
			{
				//it's a dynamic url, so it must be processed.
				priority+=2;
				sb.Remove(pos, sb.Length-pos);
				int numParams=0; bool foundSID=false;
				string [] urlParams = uri.Query.Split('?','&');
				foreach (string urlParam in urlParams)
				{
					if(urlParam!="")
					{
						//check if the parameter is a session id, if so then skip it
						foundSID=false;
						for(i=0; i<SIDVarNames.Length; i++)
						{
							if(urlParam.IndexOf(SIDVarNames[i])!=-1)
							{
								foundSID=true;
								priority+=2; //Each SID variable has a penalty of 2 points
								break;
							}
						}
						//TODO session ID - perhaps use HtmlParser's CleanUrlParams
						if(!foundSID)
						{
							if(numParams==0)
							{
								sb.Append('?');
							}
							else
							{
								sb.Append('&');
							}
							sb.Append(urlParam);
							numParams++;
							priority++;
						}
					}
					if(numParams==3) //only keep as many as 3 parameters
					{
						break;
					}
				}
			}
			url=sb.ToString();
			return priority;
		}

		/// <summary>
		/// SendResultsToServer runs on a dedicated thread. It periodically attempts to send
		/// the data produced from the crawling of urls back to the server. It communicates
		/// with the CrawlWave.Server web service asynchronously.
		/// </summary>
		private void SendResultsToServer()
		{
			Interlocked.Increment(ref runningThreads);
			if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
			{
				globals.FileLog.LogInfo("Started Results Thread with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4"));
			}
			try
			{
				string FileName = String.Empty;
				UrlCrawlData [] urls = new UrlCrawlData[0];
				//SoapFormatter serializer = null;
				BinaryFormatter serializer = null;
				MemoryStream ms = null;
				Stream ReadStream = null;
				while(!mustStop)
				{
					try
					{
						FileName = String.Empty;
						lock(resultFileNames.SyncRoot)
						{
							if(resultFileNames.Count>0)
							{
								FileName = (string)resultFileNames.Dequeue();
							}
						}
						if(FileName != String.Empty)
						{
							//urls = new UrlCrawlData[0];
							ReadStream = null;
							serializer = null;
							try
							{
								ReadStream=File.Open(FileName, FileMode.Open);
								//serializer = new SoapFormatter();
								serializer = new BinaryFormatter();
								urls = (UrlCrawlData [] /*ArrayList*/)serializer.Deserialize(ReadStream);
							}
							catch(Exception e)
							{
								/*//something went wrong - put the filename back to the queue so that the
								//client will attempt to send it to the server later
								lock(resultFileNames.SyncRoot)
								{
									resultFileNames.Enqueue(FileName);
								}*/
								globals.FileLog.LogWarning("SendResults: could not deserialize data from " + FileName +". The file will be deleted. " + e.ToString());
								//the file must be deleted
								try
								{
									ReadStream.Close();
									serializer = null;
									File.Delete(FileName);
								}
								catch
								{}
							}
							finally
							{
								if(ReadStream != null)
								{
									try
									{
										ReadStream.Close();
										ReadStream = null; //TODO check if this is needed
									}
									catch
									{}
								}
							}
							if(urls.Length /*Count*/>0)
							{
//								UrlCrawlData [] data = new UrlCrawlData [urls.Count];
//								for(int i = 0; i < urls.Count; i++)
//								{
//									data [i] = (UrlCrawlData)urls[i];
//								}
//								urls.Clear();
								byte [] buffer = null;
								//TODO: should this be called asynchronously?
								//proxy.SecureServer.BeginGetCrawlResults(globals.Client_Info, data, new AsyncCallback(SendResultsToServerCallback), proxy.SecureServer);
								try
								{
									ms = new MemoryStream();
									serializer.Serialize(ms, urls/*data*/);
                                    buffer = ms.ToArray();
									ms.Close();
									//SerializedException sx = proxy.GetCrawlResultsRaw(globals.Client_Info, buffer);
									SerializedException sx = proxy.GetCrawlResults(globals.Client_Info, urls);
									if(sx!=null)
									{
										sx.ThrowException();
									}
									lock(resultFileNames.SyncRoot)
									{
										try
										{
											File.Delete(FileName);
										}
										catch
										{
											resultFileNames.Enqueue(FileName);
										}
									}
									OnResultsSent(EventArgs.Empty);
								}
								catch(Exception e)
								{
									lock(resultFileNames.SyncRoot)
									{
										resultFileNames.Enqueue(FileName);
									}
									if(ms!=null)
									{
										ms.Close();
									}
									throw e;
								}
								finally
								{
									//data = null;
									for(int i = 0; i<urls.Length; i++)
									{
										urls[i] = null;
									}
									buffer = null;
									ms=null;
								}
							}
						}
					}
					catch(Exception e)
					{
						if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
						{
							globals.FileLog.LogWarning("Crawler.SendResultsToServer failed: " + e.ToString());
						}
					}
					serializer = null;
					GC.Collect();
					Thread.Sleep(syncBackOff.Next());
				}
			}
			catch(ThreadAbortException tae)
			{
				//The thread has been asked to abort. Log information and return at once
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo(Thread.CurrentThread.Name + " has been asked to abort: " + tae.Message);
				}
				return;
			}
			catch(ThreadInterruptedException tie)
			{
				//The thread has been asked to join. Log information and return at once
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo(Thread.CurrentThread.Name + " has been interrupted: " + tie.Message);
				}
				return;
			}
			catch(Exception ex)
			{
				if(!(ex is ThreadAbortException)) // the ThreadAbortedException is rethrown
				{
					if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
					{
						globals.FileLog.LogWarning(Thread.CurrentThread.Name + " encountered an unexpected exception: " + ex.ToString());
					}
				}
			}
			finally
			{
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo(Thread.CurrentThread.Name + " has finished.");
				}
				GC.Collect();
				Interlocked.Decrement(ref runningThreads);
			}
		}

		/// <summary>
		/// Acts as an asynchronous callback for the SendResultsToServer method that sends
		/// the crawled urls data to the server by calling GetCrawlResults.
		/// </summary>
		/// <param name="result">The result of the asynchronous call.</param>
		/*private void SendResultsToServerCallback(IAsyncResult result)
		{
			try
			{
				CrawlWaveServer server = (CrawlWaveServer)result.AsyncState;
				SerializedException sx = server.EndGetCrawlResults(result);
				if(sx!=null)
				{
					sx.ThrowException();
				}
				lock(resultFileNames.SyncRoot)
				{
					string fileName = (string)resultFileNames.Dequeue();
					try
					{
						File.Delete(fileName);
					}
					catch
					{
						resultFileNames.Enqueue(fileName);
					}
				}
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("Crawler.SendResultsToServerCallback: " + e.ToString());
				}
			}
		}*/

		/// <summary>
		/// Sends the crawl results to the server in a synchronous mode, it reads one data
		/// file at a time.
		/// </summary>
		private void SendResultsSynchronously()
		{
			if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
			{
				globals.FileLog.LogInfo("SendResultsSynchronously is attempting to send results to server.");
			}
			try
			{
				string FileName = String.Empty;
				UrlCrawlData [] urls = new UrlCrawlData[0];
				//SoapFormatter serializer = null;
				BinaryFormatter serializer = null;
				MemoryStream ms = null;
				Stream ReadStream = null;
				
				lock(resultFileNames.SyncRoot)
				{
					if(resultFileNames.Count>0)
					{
						FileName = (string)resultFileNames.Dequeue();
					}
				}
				if(FileName != String.Empty)
				{
					ReadStream = null;
					serializer = null;
					try
					{
						ReadStream=File.Open(FileName, FileMode.Open);
						//serializer = new SoapFormatter();
						serializer = new BinaryFormatter();
						urls = (UrlCrawlData [] /*ArrayList*/)serializer.Deserialize(ReadStream);
					}
					catch(Exception e)
					{
						//something went wrong during deserialization
						globals.FileLog.LogWarning("SendResultsSynchronously: could not deserialize data from " + FileName +". The file will be deleted. " + e.ToString());
						//the file must be deleted
						try
						{
							ReadStream.Close();
							serializer = null;
							File.Delete(FileName);
						}
						catch
						{}
					}
					finally
					{
						if(ReadStream != null)
						{
							try
							{
								ReadStream.Close();
								ReadStream = null; //TODO check if this is needed
							}
							catch
							{}
						}
					}
					if(urls.Length /*Count*/>0)
					{
						byte [] buffer = null;
						//TODO: should this be called asynchronously?
						//proxy.SecureServer.BeginGetCrawlResults(globals.Client_Info, data, new AsyncCallback(SendResultsToServerCallback), proxy.SecureServer);
						try
						{
							ms = new MemoryStream();
							serializer.Serialize(ms, urls/*data*/);
							buffer = ms.ToArray();
							ms.Close();
							//proxy.Timeout = 600000;
//							SerializedException sx = proxy.GetCrawlResultsRaw(globals.Client_Info, buffer);
							SerializedException sx = proxy.GetCrawlResults(globals.Client_Info, urls);
							if(sx!=null)
							{
								sx.ThrowException();
							}
							lock(resultFileNames.SyncRoot)
							{
								try
								{
									File.Delete(FileName);
								}
								catch
								{
									resultFileNames.Enqueue(FileName);
								}
							}
							OnResultsSent(EventArgs.Empty);
						}
						catch(Exception e)
						{
							lock(resultFileNames.SyncRoot)
							{
								resultFileNames.Enqueue(FileName);
							}
							if(ms!=null)
							{
								ms.Close();
							}
							throw e;
						}
						finally
						{
							//data = null;
							//proxy.Timeout = 100000;
							for(int i = 0; i<urls.Length; i++)
							{
								urls[i] = null;
							}
							buffer = null;
							ms=null;
							serializer = null;
						}
					}
				}
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("Crawler.SendResultsSynchronously failed: " + e.ToString());
				}
			}
			finally
			{
				GC.Collect();
			}
		}

		/// <summary>
		/// Sends the crawl results currently in the results queue in a synchronous mode.
		/// </summary>
		/// <returns>True on success, false if the operation fails.</returns>
		private bool SendResultsInlined()
		{
			bool retVal = false;
			try
			{
				UrlCrawlData [] data = (UrlCrawlData [])crawledUrls.ToArray(typeof(UrlCrawlData));
				if(data.Length > 0)
				{
					try
					{
						//proxy.Timeout = 600000;
						SerializedException sx = proxy.GetCrawlResults(globals.Client_Info, data);
						if(sx == null)
						{
							retVal = true;
							OnResultsSent(EventArgs.Empty);
						}
						else
						{
							sx.ThrowException();
						}
					}
					catch(Exception e)
					{
						if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
						{
							globals.FileLog.LogWarning("Crawler.SendResultsInlined failed: " + e.ToString());
						}
					}
					finally
					{
						//proxy.Timeout = 300000;
					}
				}
			}
			catch
			{
				retVal = false;
			}
			finally
			{
				GC.Collect();
			}
			return retVal;
		}

		/// <summary>
		/// Stores the crawled urls to an xml file on disk and updates the internal queue
		/// of resultFileNames
		/// </summary>
		private void StoreCrawlResults()
		{
			//if there is no data to store just return
			if((crawledUrls == null)||(crawledUrls.Count==0))
			{
				return;
			}
			//save the data to disk
			string FileName=globals.AppWorkPath+"results" + Guid.NewGuid().ToString() + ".xml";
			Stream WriteStream = null;
			bool success = false;
			try
			{
				WriteStream=File.Open(FileName, FileMode.Create);
				//SoapFormatter serializer=new SoapFormatter();
				BinaryFormatter serializer = new BinaryFormatter();
				UrlCrawlData [] data = (UrlCrawlData [])crawledUrls.ToArray(typeof(UrlCrawlData));
				serializer.Serialize(WriteStream, data/*crawledUrls*/);
				WriteStream.Close();
				WriteStream = null; //TODO check if this is needed
				success = true;
			}
			catch(Exception e)
			{
				if(WriteStream != null)
				{
					try
					{
						WriteStream.Close();
						WriteStream = null; //TODO check if this is needed
					}
					catch
					{}
				}
				if(File.Exists(FileName))
				{
					try
					{
						File.Delete(FileName);
					}
					catch
					{}
				}
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("Crawler.StoreCrawlResults failed: " + e.ToString());
				}
			}
			finally
			{
				//attempt to send it at once
				if(SendResultsInlined())
				{
					if(success)
					{
						File.Delete(FileName);
					}
				}
				else
				{
					if(success)
					{
						//if it works enqueue the new file's name to the internal results queue
						lock(resultFileNames.SyncRoot)
						{
							resultFileNames.Enqueue(FileName);
						}
					}
				}
				//first of all empty the list of crawled urls
				crawledUrls.Clear();
				//We can now donwload a new set of Urls to crawl. If an old data file exists
				//try to delete it, since we're gonna download a new one
				if(dataFileName != String.Empty)
				{
					try
					{
						File.Delete(dataFileName);
						dataFileName = String.Empty;
					}
					catch
					{}
				}
				GC.Collect();
			}
		}

		/// <summary>
		/// Stops all the running threads. It calls Join on them to interrupt them and
		/// waits until they have finished executing.
		/// </summary>
		private void StopAllThreads()
		{
			/*if(sendResultsThread.IsAlive)
			{
				sendResultsThread.Join(ExponentialBackoff.DefaultBackoff);
			}*/
			if(synchronizeThread.IsAlive)
			{
				synchronizeThread.Join(ExponentialBackoff.DefaultBackoff);
			}
			for(int i = 0; i< numThreads; i++)
			{
				if(crawlingThreads[i].IsAlive)
				{
					crawlingThreads[i].Join(ExponentialBackoff.DefaultBackoff);
				}
			}
		}

		/// <summary>
		/// Kills all the running threads by calling Abort and then Join on them.
		/// </summary>
		private void KillAllThreads()
		{
			if(synchronizeThread.IsAlive)
			{
				try
				{
					synchronizeThread.Abort();
					synchronizeThread.Join(1000);
				}
				catch
				{}
			}
			/*if(sendResultsThread.IsAlive)
			{
				try
				{
					sendResultsThread.Abort();
					sendResultsThread.Join(ExponentialBackoff.DefaultBackoff);
				}
				catch
				{}
			}*/
			for(int i = 0; i<numThreads; i++)
			{
				try
				{
					crawlingThreads[i].Abort();
					crawlingThreads[i].Join(1000);
				}
				catch
				{
					continue;
				}
			}
		}

		/// <summary>
		/// Suspends the execution of all the running threads.
		/// </summary>
		private void SuspendAllThreads()
		{
			if(synchronizeThread.IsAlive)
			{
				synchronizeThread.Suspend();
			}
			/*if(sendResultsThread.IsAlive)
			{
				sendResultsThread.Suspend();
			}*/
			for(int i=0; i<numThreads; i++)
			{
				if(crawlingThreads[i].IsAlive)
				{
					crawlingThreads[i].Suspend();
				}
			}
		}

		/// <summary>
		/// Resumes the execution of all the running threads
		/// </summary>
		private void ResumeAllThreads()
		{
			if((synchronizeThread.ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested)) >0)
			{
				synchronizeThread.Resume();
			}
			/*if((sendResultsThread.ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested)) >0)
			{
				sendResultsThread.Resume();
			}*/
			for(int i=0; i<numThreads; i++)
			{
				if((crawlingThreads[i].ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested)) >0)
				{
					crawlingThreads[i].Resume();
				}
			}
		}

		#endregion

		#region Event Invokers

		/// <summary>
		/// Raises the <see cref="StatisticsChanged"/> event
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnStatisticsChanged(EventArgs e)
		{
			if(StatisticsChanged != null)
			{
				StatisticsChanged(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="StateChanged"/> event
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnStateChanged(EventArgs e)
		{
			if(StateChanged != null)
			{
				StateChanged(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="ResultsSent"/> event
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnResultsSent(EventArgs e)
		{
			if(ResultsSent != null)
			{
				ResultsSent(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="UrlSetReceived"/> event
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnUrlSetReceived(EventArgs e)
		{
			if(UrlSetReceived != null)
			{
				UrlSetReceived(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="UrlProcessed"/> event
		/// </summary>
		/// <param name="e">The <see cref="ParserEventArgs"/> related to the event.</param>
		private void OnUrlProcessed(ParserEventArgs e)
		{
			if(UrlProcessed != null)
			{
				UrlProcessed(this, e);
			}
		}

		#endregion
	}
}
