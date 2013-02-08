using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using CrawlWave.Common;
using CrawlWave.Client.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.Scheduler
{
	/// <summary>
	/// Globals is a Singleton class that holds all the variables that need to be available
	/// globally throughout the application. It is a Singleton so that only one instance of
	/// these variables will exist during the application's execution and all other classes
	/// can use this class when they need access on any of them.
	/// </summary>
	public class Globals : ICrawlWaveServerSettingsProvider
	{
		#region Private variables

		/// <summary>
		/// The single class instance
		/// </summary>
		private static Globals instance;
		/// <summary>
		/// The application's settings.
		/// </summary>
		private ClientSettings settings;
		/// <summary>
		/// The path of the directory where the application is stored.
		/// </summary>
		private string appPath;
		/// <summary>
		/// Identifier for the events stored in the system event log
		/// </summary>
		private string logEventSource;
		/// <summary>
		/// The name of the file that will store the application's event log.
		/// </summary>
		private string logFileName;
		/// <summary>
		/// The client's info.
		/// </summary>
		private ClientInfo clientInfo;
		/// <summary>
		/// The <see cref="SystemEventLogger"/> that will be used by the application
		/// </summary>
		private SystemEventLogger systemLog;
		/// <summary>
		/// The <see cref="FileEventLogger"/> that will be used by the application
		/// </summary>
		private FileEventLogger fileLog;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// Constructs a new instance of the <see cref="Globals"/> class.
		/// </summary>
		private Globals()
		{
			//Initialize the variables
			string path = GetAppPath();
			try
			{
				//If the application cannot write to its local path then it will attempt
				//to write in the personal path of the current user, under Application Data 
				FileStream fs =  File.Create(path + "test.dat");
				fs.Close();
				File.Delete(path + "test.dat");
			}
			catch
			{
				path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\CrawlWave\\";
			}
			appPath = String.Intern(path);
			logEventSource = String.Intern("CrawlWave.ClientScheduler");
			logFileName = String.Intern(appPath + "CrawlWave.ClientScheduler.log");
			settings = new ClientSettings(appPath + "data\\CrawlWave.Client.config.xml");
			settings.LoadSettings();
			clientInfo = new ClientInfo();
			clientInfo.UserID = settings.UserID;
			clientInfo.ClientID = settings.ClientID;
			clientInfo.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			systemLog = new SystemEventLogger(logEventSource);
			fileLog = new FileEventLogger(logFileName, true, logEventSource);
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="Globals"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="Globals"/>.</returns>
		public static Globals Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new Globals();
				}
				imutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a string containing the application's path, including the trailing baskslash.
		/// </summary>
		public string AppPath
		{
			get { return appPath; }
		}

		/// <summary>
		/// Gets a string containing the description of the event source used in the log.
		/// </summary>
		public string LogEventSource
		{
			get { return logEventSource; }
		}

		/// <summary>
		/// Gets the name of the file that will store the application's event log.
		/// </summary>
		public string LogFileName
		{
			get { return logFileName; }
		}

		/// <summary>
		/// Gets an <see cref="ClientSettings"/> object providing access to the application's settings
		/// </summary>
		public ClientSettings Settings
		{
			get { return settings; }
		}

		/// <summary>
		/// Gets the client's <see cref="ClientInfo"/>.
		/// </summary>
		public ClientInfo Client_Info
		{
			get { return clientInfo; }
			//set { clientInfo = value;}
		}

		/// <summary>
		/// Gets a reference to the <see cref="SystemEventLogger"/> used by the application.
		/// </summary>
		public SystemEventLogger SystemLog
		{
			get { return systemLog; }
		}

		/// <summary>
		/// Gets a reference to the <see cref="SystemEventLogger"/> used by the application.
		/// </summary>
		public FileEventLogger FileLog
		{
			get { return fileLog; }
		}

		#endregion

		#region Static methods

		/// <summary>
		/// Determines the application's path.
		/// </summary>
		/// <returns>The application's path, including the trailing slashes</returns>
		internal static string GetAppPath()
		{
			string retVal=String.Empty;
			string path=String.Empty;
			try
			{
				path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
			}
			catch
			{
				//it didn't work, so assume it's the current drectory
				path = Environment.CommandLine.Trim('"');
			}
			try
			{
				retVal=path.Substring(0,path.LastIndexOf('\\')) + "\\";
			}
			catch 
			{
				//if every attempt fails assume it's the working directory
				retVal=".\\";
			}
			return retVal;
		}


		#endregion

		#region ICrawlWaveServerSettingsProvider Members

		public string Hostname
		{
			get { return settings.RemotingHost; }
		}

		public int Port
		{
			get { return settings.RemotingPort; }
		}

		public string ChannelType
		{
			get { return settings.RemotingProtocol; }
		}

		#endregion
	}
}
