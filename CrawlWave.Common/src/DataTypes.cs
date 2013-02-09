/*========================================================================================
 * This file contains a number of definitions of data structures, classes and enumerations
 * that are too small to be put in a file of their own, so they are all gathered in here.
 * The data types defined here are used by more than one applications and many of them are
 * going to be passed as parameters over the network, so that's why they are defined here.
 *=======================================================================================*/

using System;

namespace CrawlWave.Common
{
	#region Enumerations

	/// <summary>
	/// CWLogLevel is an enumeration that defines the different error severity levels that
	/// can be logged using an <see cref="ILogger"/> implementation. This way it is easier
	/// to define what kinds of events will be stored in an Event Log.
	/// </summary>
	public enum CWLogLevel
	{
		/// <summary>
		/// Causes the <see cref="ILogger"/> to log Events of every type.
		/// </summary>
		LogInfo = 0,
		/// <summary>
		/// Causes the <see cref="ILogger"/> to log only events of type Warning and Error.
		/// </summary>
		LogWarning = 1,
		/// <summary>
		/// Causes the <see cref="ILogger"/> to log only events of type Error.
		/// </summary>
		LogError= 2 ,
		/// <summary>
		/// Causes the <see cref="ILogger"/> to log no Events at all.
		/// </summary>
		LogNothing = 3
	}

	/// <summary>
	/// CWClientActions is an enumeration that allows the CrawlWave Server applications to
	/// decide which client activities to log to the system's database.
	/// </summary>
	/// <remarks>
	/// In order to achieve maximum performance no activities must be logged at all because
	/// in that case the overall performance declines due to the bigger workload put on the
	/// system's Database Server backend.
	/// </remarks>
	[Flags]
	public enum CWClientActions
	{
		/// <summary>
		/// Disables logging (this is the fastest option).
		/// </summary>
		LogNothing = 0,
		/// <summary>
		/// Enables logging of attempts to update a client's computer info.
		/// </summary>
		LogGetClientComputerInfo = 1,
		/// <summary>
		/// Enables logging of clients' attempts to return crawled urls data to the server.
		/// </summary>
		LogGetCrawlResults = 2,
		/// <summary>
		/// Enables logging of client's attempts to be registered in the system. 
		/// </summary>
		LogRegisterClient = 4,
		/// <summary>
		/// Enables logging of users' attempts to be registered in the system.
		/// </summary>
		LogRegisterUser = 8,
		/// <summary>
		/// Enables logging of clients' attempts to download a list of banned hosts.
		/// </summary>
		LogSendBannedHosts = 16,
		/// <summary>
		/// Enables logging of clients' attempts to download a list of available servers.
		/// </summary>
		LogSendServers = 32,
		/// <summary>
		/// Enables logging of clients' attempts to download an updated client version.
		/// </summary>
		LogSendUpdatedVersion = 64,
		/// <summary>
		/// Enables logging of clients' attempts to download a set of urls to crawl.
		/// </summary>
		LogSendUrlsToCrawl = 128,
		/// <summary>
		/// Enables logging of clients' attempts to get their corresponding user statistics
		/// </summary>
		LogSendUserStatistics = 256,
		/// <summary>
		/// Enables logging of client's attempts to get their statistics
		/// </summary>
		LogSendClientStatistics = 512,
		/// <summary>
		/// Enables logging of clients' attempts to retrieve server statistics
		/// </summary>
		LogSendServerStatistics = 1024,
		/// <summary>
		/// Causes every action performed on the Server to be logged in the database. 
		/// </summary>
		LogAllActions = 2047
	}

	/// <summary>
	/// CWUrlActions is an enumeration that allows the CrawlWave Server applications to
	/// decide which Url tasks like Assignment or Update to log to the system's database.
	/// </summary>
	/// <remarks>
	/// In order to achieve maximum performance no activities must be logged at all because
	/// in that case the overall performance declines due to the bigger workload put on the
	/// system's Database Server backend.
	/// </remarks>
	[Flags]
	public enum CWUrlActions
	{
		/// <summary>
		/// Disables logging (this is the fastest option)
		/// </summary>
		LogNothing = 0,
		/// <summary>
		/// Enables logging of Url Assignments
		/// </summary>
		LogAssignment = 1,
		/// <summary>
		/// Enables logging of Url return / update
		/// </summary>
		LogUpdate = 2,
		/// <summary>
		/// Enables logging of Url Deletion
		/// </summary>
		LogDelete = 4,
		/// <summary>
		/// Enables logging of Url Domain Checking
		/// </summary>
		LogDomainCheck = 8,
		/// <summary>
		/// Enables logging of Url PageRank processing
		/// </summary>
		LogPageRank = 16,
		/// <summary>
		/// Enables logging of Url Word extraction
		/// </summary>
		LogWordExtraction = 32,
		/// <summary>
		/// Enables logging of all actions
		/// </summary>
		LogAllActions = 63
	}

	/// <summary>
	/// CWLoggerEntryType is an enumeration of the different types of events that are logged
	/// by the classes implementing the <see cref="ILogger"/> interface.
	/// </summary>
	public enum CWLoggerEntryType
	{
		/// <summary>
		/// Event of type information
		/// </summary>
		Info,
		/// <summary>
		/// Event of type warning (serious)
		/// </summary>
		Warning,
		/// <summary>
		/// Event of type error (more serious)
		/// </summary>
		Error
	}

	/// <summary>
	/// ConnectionSpeed is an enumeration of different Internet Access connection speeds.
	/// It is needed in order to determine the number of crawling threads that can visit
	/// pages simultaneously in the CrawlWave.Client.
	/// </summary>
	public enum CWConnectionSpeed
	{
		/// <summary>
		/// Undetermined connection speed
		/// </summary>
		Unknown = 1,
		/// <summary>
		/// 56Kbps Dial-Up Modem connection
		/// </summary>
		Modem56K = 2,
		/// <summary>
		/// ISDN 64Kbps connection
		/// </summary>
		ISDN64K = 3,
		/// <summary>
		/// ISDN 128Kbps connection
		/// </summary>
		ISDN128K = 4,
		/// <summary>
		/// DSL 256Kbps connection
		/// </summary>
		DSL256K = 5,
		/// <summary>
		/// DSL 384Kbps connection
		/// </summary>
		DSL384K = 6,
		/// <summary>
		/// DSL 512Kbps connection
		/// </summary>
		DSL512K = 8,
		/// <summary>
		/// DSL 1Mbps connection
		/// </summary>
		DSL1M = 10,
		/// <summary>
		/// T1: 1536Kbps (perhaphs should be replaced with E1 - we're in Europe, after all :)
		/// </summary>
		T1 = 15,
		/// <summary>
		/// T3: 45Mbps (perhaphs should be replaced with E3  - we're in Europe, after all :)
		/// </summary>
		T3 = 20,
		/// <summary>
		/// Lit with fiber! Any connection above 45Mbps and below 155Mbps
		/// </summary>
		Fiber = 25,
		/// <summary>
		/// ATM 155Mbps connection
		/// </summary>
		ATM = 30
	}

	/// <summary>
	/// RobotsMetaTagValue is an enumeration of the different options that can be set using
	/// the robots meta tag. It is useful when one wants to characterize a Url depending on
	/// whether it is allowed to be indexed and / or followed.
	/// </summary>
	[Flags]
	public enum RobotsMetaTagValue
	{
		/// <summary>
		/// No meta tag found - assumed to be equivalent to <see cref="IndexFollow"/>
		/// </summary>
		NoMeta = 5,
		/// <summary>
		/// Allowed to crawl and index the Url
		/// </summary>
		Index = 1,
		/// <summary>
		/// Not allowed to crawl or index the Url
		/// </summary>
		NoIndex = 2,
		/// <summary>
		/// Allowed to follow the links found in the contents of the Url
		/// </summary>
		Follow = 4,
		/// <summary>
		/// Index and Follow is allowed - equivalent to Index | Follow
		/// </summary>
		IndexFollow = 5,
		/// <summary>
		/// Not allowed to follow the links found in the contents of the Url
		/// </summary>
		NoFollow = 8
	}

	/// <summary>
	/// DomainFlagValue is an enumeration of the different domain flag values which are used
	/// to determine whether a Url belongs to the part of the web we wish to visit or not.
	/// </summary>
	/// <remarks>
	/// The values this enumeration can take are:
	/// <list type="table">
	///   <listheader>
	///		<term>Value</term>
	///		<description>Description</description>
	///   </listheader>
	///   <item>
	///		<term>0</term>
	///		<description>
	///		  Indicates that the Url belongs to the part of the web we wish to crawl
	///		</description>
	///   </item>
	///   <item>
	///		<term>1</term>
	///		<description>
	///		  Indicates that the Url doesn't belong to the part of the web we wish to crawl
	///		</description>
	///   </item>
	///   <item>
	///		<term>2</term>
	///		<description>
	///		  Indicates that it is not yet known whether this Url belongs to the part of
	///		  the web we wish to crawl
	///		</description>
	///   </item>
	/// </list>
	/// </remarks>
	public enum DomainFlagValue
	{
		/// <summary>
		/// Indicates that the Url belongs to the part of the web we wish to crawl.
		/// </summary>
		MustVisit = 0,
		/// <summary>
		/// Indicates that the Url doesn't belong to the part of the web we wish to crawl.
		/// </summary>
		MustNotVisit = 1,
		/// <summary>
		/// Indicates that it is not yet known whether this Url belongs to the part of the
		///	web we wish to crawl.
		/// </summary>
		Unknown = 2
	}

	/// <summary>
	/// PluginStatus specifies the different states in which a <see cref="IPlugin"/> object
	/// can be at any given time.
	/// </summary>
	public enum PluginState
	{
		/// <summary>
		/// Indicates that the plugin has paused its process
		/// </summary>
		Paused,
		/// <summary>
		/// Indicates that the plugin has been started and is active
		/// </summary>
		Running,
		/// <summary>
		/// Indicates that the plugin has just been initialized or stopped and is inactive.
		/// </summary>
		Stopped
	}

	#endregion

	#region Data Structures

	/// <summary>
	/// CWComputerInfo is a struct that stores the characteristics of a computer of the
	/// system. It is useful for statistic purposes for now, but in the future it could
	/// be used to assign more work load to clients running on fast systems in order to
	/// improve the overall performance of the system. 
	/// </summary>
	[Serializable]
	public struct CWComputerInfo
	{
		/// <summary>
		/// A <see cref="String"/> containing the CPU type and speed in MHz.
		/// </summary>
		public string CPUType;
		/// <summary>
		/// The system's RAM size in MB.
		/// </summary>
		public int RAMSize;
		/// <summary>
		/// The free hard disk space of the system's HDD in MB.
		/// </summary>
		public int HDDSpace;
		/// <summary>
		/// The <see cref="CWConnectionSpeed">Speed</see> of the system's Internet connection.
		/// </summary>
		public CWConnectionSpeed ConnectionSpeed;

		/// <summary>
		/// Constructs an instance of <see cref="CWComputerInfo"/> struct and initializes it
		/// with the supplied values.
		/// </summary>
		/// <param name="CPU">A string describibg the CPU type and speed in MHz.</param>
		/// <param name="RAM">The RAM size in MB.</param>
		/// <param name="HDD">The free HDD space in MB.</param>
		/// <param name="Speed">The <see cref="CWConnectionSpeed">Internet connection speed</see> of the system.</param>
		public CWComputerInfo(string CPU, int RAM, int HDD, CWConnectionSpeed Speed)
		{
			CPUType = CPU;
			if(RAM<0)
			{
				throw new ArgumentOutOfRangeException("RAM","The RAM size cannot be negative.");
			}
			RAMSize = RAM;
			if(HDD<0)
			{
				throw new ArgumentOutOfRangeException("HDD","The HDD space cannot be negative.");
			}
			HDDSpace = HDD;
			ConnectionSpeed = Speed;
		}
	}

	/// <summary>
	/// ClientInfo is a struct that holds information about a client, such as its uniqui ID,
	/// the ID of the user running it and its version.
	/// </summary>
	[Serializable]
	public struct ClientInfo
	{
		/// <summary>
		/// An integer containing the ID of the user running the client
		/// </summary>
		public int UserID;
		/// <summary>
		/// The unique identifier of the client. Each instance of the client has its own ID.
		/// </summary>
		public Guid ClientID;
		/// <summary>
		/// The client's version
		/// </summary>
		public string Version;
	}

	/// <summary>
	/// EventLoggerEntry is a struct that holds information related to an event that can or
	/// must be logged by a class implementing the <see cref="ILogger"/> interface. It is
	/// particularly useful for classes implementing the <see cref="IPlugin"/> interface,
	/// or whenever a class needs to buffer some events before writing them to a log.
	/// </summary>
	[Serializable]
	public struct EventLoggerEntry
	{
		/// <summary>
		/// The <see cref="CWLoggerEntryType"/> type of the event.
		/// </summary>
		public CWLoggerEntryType EventType;
		/// <summary>
		/// The date and time when the event occured.
		/// </summary>
		public DateTime EventDate;
		/// <summary>
		/// The message of the event.
		/// </summary>
		public string EventMessage;

		/// <summary>
		/// Constructs a new instance of the <see cref="EventLoggerEntry"/> struct with the
		/// data provided.
		/// </summary>
		/// <param name="eventType">The type of the event.</param>
		/// <param name="eventDate">The date and time the event took place.</param>
		/// <param name="eventMsg">The message related to the event.</param>
		public EventLoggerEntry(CWLoggerEntryType eventType, DateTime eventDate, string eventMsg)
		{
			EventType = eventType;
			EventDate = eventDate;
			EventMessage = eventMsg;
		}
	}

	#endregion

	#region Classes

	/// <summary>
	/// RobotsTxtEntry is a simple class that holds all the necessary information related
	/// to a robots.txt file.
	/// </summary>
	[Serializable]
	public class RobotsTxtEntry
	{
		/// <summary>
		/// Lets us know when the robots.txt file must be recollected
		/// </summary>
		public DateTime ExpirationDate;
		/// <summary>
		/// An array of paths on the host that the crawler must not follow
		/// </summary>
		public string [] DisallowedPaths;

		/// <summary>
		/// Constructs a new instance of the <see cref="RobotsTxtEntry"/> class.
		/// </summary>
		/// <param name="expiration">A <see cref="DateTime"/> object indicating when the
		/// robots.txt file must be recollected.</param>
		/// <param name="disallowedPaths">An array of strings, one for each host path that
		/// the crawler is not allowed to visit.</param>
		public RobotsTxtEntry(DateTime expiration, string [] disallowedPaths)
		{
			ExpirationDate = expiration;
			DisallowedPaths = disallowedPaths;
		}

		/// <summary>
		/// Constructs a new instance of the <see cref="RobotsTxtEntry"/> class.
		/// </summary>
		public RobotsTxtEntry()
		{
			ExpirationDate = DateTime.Today.AddMonths(1);
			DisallowedPaths = null;
		}
	}

	/// <summary>
	/// HostRequestFilterEntry is a simple class that holds information for a host for which
	/// there are pending web requests. It is used to avoid slammering a host with many web
	/// requests.
	/// </summary>
	[Serializable]
	public class HostRequestFilterEntry
	{
		/// <summary>
		/// Lets us know when the request can be regarded as expired and be deleted
		/// </summary>
		public DateTime ExpirationDate;
		/// <summary>
		/// Counts the number of pending requests for this host
		/// </summary>
		public int Count;

		/// <summary>
		/// Constructs a new instance of the <see cref="HostRequestFilterEntry"/> class and
		/// sets the expiration time to a provided time.
		/// </summary>
		/// <param name="expiration">The expiration <see cref="DateTime"/> of the entry.</param>
		public HostRequestFilterEntry(DateTime expiration)
		{
			ExpirationDate = expiration;
			Count = 1;
		}

		/// <summary>
		/// Constructs a new instance of the <see cref="HostRequestFilterEntry"/> class and
		/// sets the timeout to a default value of 30 seconds.
		/// </summary>
		public HostRequestFilterEntry()
		{
			//by default we must not visit a host every 30 seconds
			ExpirationDate = DateTime.Now.AddMilliseconds(Backoff.DefaultBackoff);
			Count = 1;
		}
	}

	/// <summary>
	/// UrlCrawlDataFile is a class that is used as the object that contains UrlCrawlData
	/// objects and is later serialized and saved in compressed form on disk. These files
	/// will later on be used to update the database with the data of the urls visited and
	/// the links they contain. All its members are public, it acts like a plain struct.
	/// </summary>
	[Serializable]
	public class UrlCrawlDataFile
	{
		/// <summary>
		/// The <see cref="ClientInfo"/> of the client who returned the data.
		/// </summary>
		public ClientInfo Info;
		/// <summary>
		/// An array of <see cref="UrlCrawlData"/> objects.
		/// </summary>
		public UrlCrawlData [] Data;

		/// <summary>
		/// Constructs a new instance of the <see cref="UrlCrawlDataFile"/> class.
		/// </summary>
		public UrlCrawlDataFile()
		{
			Info = new ClientInfo();
			Data = null;
		}

		/// <summary>
		/// Constructs a new instance of the <see cref="UrlCrawlDataFile"/> class with the
		/// provided values.
		/// </summary>
		/// <param name="info">The <see cref="ClientInfo"/> of the client who returned the data.</param>
		/// <param name="data">An array of <see cref="UrlCrawlData"/> objects.</param>
		public UrlCrawlDataFile(ClientInfo info, UrlCrawlData [] data)
		{
			Info = info;
			Data = data;
		}
	}

	/// <summary>
	/// ServerStatistics is a class that is used as a means to pass statistical information
	/// between the server and client. it contains just about everything someone needs to
	/// know in order to evaluate the crawling process status. It is also used as a means
	/// to construct graphs with various information, such as crawling speed etc.
	/// </summary>
	[Serializable]
	public class ServerStatistics
	{
		/// <summary>
		/// The <see cref="DateTime"/> when the statistics were collected.
		/// </summary>
		public DateTime StatisticsDate;
		/// <summary>
		/// The number of users registered in the system.
		/// </summary>
		public int NumUsers;
		/// <summary>
		/// The number of clients registered in the system.
		/// </summary>
		public int NumClients;
		/// <summary>
		/// The number of clients active during the last day.
		/// </summary>
		public int NumClientsActive;
		/// <summary>
		/// The number of hosts seen by the crawler.
		/// </summary>
		public int NumHosts;
		/// <summary>
		/// The number of banned hosts.
		/// </summary>
		public int NumBannedHosts;
		/// <summary>
		/// The number of robots.txt files collected from the hosts.
		/// </summary>
		public int NumRobots;
		/// <summary>
		/// The number of Urls seen by the crawler.
		/// </summary>
		public int NumUrls;
		/// <summary>
		/// The number of Urls in the ready-to-be-crawled Urls queue.
		/// </summary>
		public int NumUrlsToCrawl;
		/// <summary>
		/// The total number of visits made to Urls.
		/// </summary>
		public int NumUrlsCrawled;
		/// <summary>
		/// The number of edges contained in the Url Link Graph.
		/// </summary>
		public long LinkGraphSize;

		/// <summary>
		/// Constructs a new instance of the <see cref="ServerStatistics"/> class.
		/// </summary>
		public ServerStatistics()
		{
            StatisticsDate = DateTime.Now;
			NumUsers = 0;
			NumClients = 0;
			NumClientsActive = 0;
			NumHosts = 0;
			NumBannedHosts = 0;
			NumRobots = 0;
			NumUrls = 0;
			NumUrlsToCrawl = 0;
			NumUrlsCrawled = 0;
			LinkGraphSize = 0;
		}
	}

	/// <summary>
	/// UserStatistics is a class containing statistical information about a User participating
	/// in the procect. Ir contains information like the total number of Urls assigned and
	/// returned by all the clients this user is running.
	/// </summary>
	[Serializable]
	public class UserStatistics
	{
		/// <summary>
		/// The <see cref="DateTime"/> when the user registered in the system.
		/// </summary>
		public DateTime RegistrationDate;
		/// <summary>
		/// The number of clients this user is running.
		/// </summary>
		public int NumClients;
		/// <summary>
		/// The total number of Urls assigned to this user's clients.
		/// </summary>
		public long UrlsAssigned;
		/// <summary>
		/// The total number of urls crawled and returned by this user's clients.
		/// </summary>
		public long UrlsReturned;
		/// <summary>
		/// The <see cref="DateTime"/> of the user's last activity.
		/// </summary>
		public DateTime LastActive;

		/// <summary>
		/// Constructs a new instance of the <see cref="UserStatistics"/> class.
		/// </summary>
		public UserStatistics()
		{
			RegistrationDate = DateTime.Now;
			NumClients = 0;
			UrlsAssigned = 0;
			UrlsReturned = 0;
			LastActive = DateTime.Now;
		}
	}

	#endregion

	#region Interfaces

	#endregion

	#region Custom Attributes

	/// <summary>
	/// CrawlWaveUpdaterAttribute is an attribute that must be used on all the client update
	/// packages that will be used to update the various components of the CrawlWave Client.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class CrawlWaveUpdaterAttribute : System.Attribute
	{
		/// <summary>
		/// Constructs a new instance of the CrawlWaveUpdaterAttribute class.
		/// </summary>
		public CrawlWaveUpdaterAttribute() : base()
		{}
	}

	#endregion
}