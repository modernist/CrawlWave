using System;
using System.Collections;
using System.Configuration;
using System.Threading;
using CrawlWave.Common;
using CrawlWave.ServerCommon;
using CrawlWave.Service.Properties;

namespace CrawlWave.Service
{
	/// <summary>
	/// ServerSettings is a Singleton class that holds the settings related to the server's
	/// operation. It provides easy access to all the classes that need it.
	/// </summary>
	public class ServerSettings
	{
		#region Private variables

		private static ServerSettings instance;
		private string appName;
		private SystemEventLogger log;
		private CWLogLevel logLevel;
		private CWClientActions dbLogOptions;
		private string dataFilesPath;
		private string connectionString;
		private int remotingPort;
		private RobotsCache robots;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private ServerSettings()
		{
			appName = "CrawlWave.Server";
			log = new SystemEventLogger(appName);
			logLevel = CWLogLevel.LogError; // by default assume only errors must be logged
			dbLogOptions = CWClientActions.LogNothing; //by default assume it's a production rel.
			dataFilesPath = String.Empty;
			connectionString = String.Empty;
			LoadSettings();
			robots = RobotsCache.Instance();
			robots.DBConnectionString = ProvideConnectionString();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="ServerSettings"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="ServerSettings"/>.</returns>
		public static ServerSettings Instance()
		{
			if (instance == null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex = new Mutex();
				imutex.WaitOne();
				if (instance == null)
				{
					instance = new ServerSettings();
				}
				imutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets a reference to the <see cref="SystemEventLogger"/> that will be used by all
		/// the classes for error logging.
		/// </summary>
		public SystemEventLogger Log
		{
			get { return log; }
		}

		/// <summary>
		/// Gets a <see cref="CWLogLevel"/> value indicating the logging level of the server.
		/// </summary>
		public CWLogLevel LogLevel
		{
			get { return logLevel; }
		}

		/// <summary>
		/// Gets a <see cref="CWClientActions"/> value indicating the level of logging that
		/// will be performed on the system's database.
		/// </summary>
		public CWClientActions LogOptions
		{
			get { return dbLogOptions; }
		}

		/// <summary>
		/// Gets a string indicating the path where the server will store the crawled pages.
		/// </summary>
		public string DataFilesPath
		{
			get { return dataFilesPath; }
		}

		/// <summary>
		/// Gets the port on which the server process is listening for connections
		/// </summary>
		public int RemotingPort
		{
			get { return remotingPort; }
		}

		/// <summary>
		/// Gets a reference to a <see cref="RobotsCache"/> holding a cache of the robots.txt
		/// files that are stored in the database.
		/// </summary>
		public RobotsCache Robots
		{
			get { return robots; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Provides a Database Connection String to any class that needs it, setting the
		/// application name of the connection to a combination of the application's name
		/// and a random number.
		/// </summary>
		/// <returns>A database connection string.</returns>
		public string ProvideConnectionString()
		{
			return connectionString + "Application Name =" + appName + Thread.CurrentThread.ManagedThreadId.ToString() + ";Pooling = false";
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Loads the server's settings from the Web.config configuration file
		/// </summary>
		private void LoadSettings()
		{
			try
			{
			

				logLevel = (CWLogLevel)(Properties.Settings.Default.LogLevel);
				dbLogOptions = (CWClientActions)(Properties.Settings.Default.DBLogOptions);
				remotingPort = Properties.Settings.Default.RemotingPort;
				dataFilesPath = Properties.Settings.Default.DataFilesPath;
				string sqlServer = Properties.Settings.Default.SQLServer;
				string sqlLogin = Properties.Settings.Default.SQLLogin;
				string sqlPass = Properties.Settings.Default.SQLPass;
				connectionString = "Password=" + sqlPass + ";Persist Security Info=True;User ID=" + sqlLogin + ";Initial Catalog=CrawlWave;Data Source=" + sqlServer + ";";
			}
			catch
			{ }
		}
	
		#endregion
	}
}
