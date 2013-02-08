using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Win32;
using CrawlWave.Common;

namespace CrawlWave.Client.Common
{
	/// <summary>
	/// ClientSettings provides a collection of all the settings that are required for the 
	/// CrawlWave Client's operation. They allow the settings defined by the user or system 
	/// to be stored in persistent form (in an XML file on disk) using serialization.
	/// </summary>
	[Serializable]
	public class ClientSettings
	{
		#region Private variables

		private string configFile; // holds the path of the configuration file

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value that determines if the application
		/// will start automatically with Windows.
		/// </summary>
		public bool LoadAtStartup;
		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value that determines if the application
		/// will become a system tray icon when minimized.
		/// </summary>
		public bool MinimizeToTray;
		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value that determines if the application
		/// will minimize instead of exit if the user presses the x button on the ControlBox
		/// </summary>
		public bool MinimizeOnExit;
		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value that determines if the application
		/// scheduler will be enabled (Crawling will start / stop at a user-defined time).
		/// </summary>
		public bool EnableScheduler;
		/// <summary>
		/// Gets or sets a <see cref="DateTime"/> value indicating the time at which the
		/// application must start the crawling process.
		/// </summary>
		public DateTime StartTime;
		/// <summary>
		/// Gets or sets a <see cref="DateTime"/> value indicating the time at which the
		/// application must stop the crawling process.
		/// </summary>
		public DateTime StopTime;
		/// <summary>
		/// Gets or sets an integer containing the ID of the user running the client.
		/// </summary>
		public int UserID;
		/// <summary>
		/// Gets or sets a <see cref="String"/> value containing the username of the user
		/// who is running the client.
		/// </summary>
		public string UserName;
		/// <summary>
		/// Gets or sets a <see cref="String"/> value containing the email address of the 
		/// user running the client.
		/// </summary>
		public string Email;
		/// <summary>
		/// Gets or sets an array of bytes containing the hash value of the password of the
		/// user running the client.
		/// </summary>
		public byte [] Password;
		/// <summary>
		/// Gets or sets a <see cref="Guid"/> containing the unique ID of the client.
		/// </summary>
		public Guid ClientID;
		/// <summary>
		/// Gets or sets an unsigned long integer containing the lower 64 bits of the hash
		/// code that occurs from the user's computer's hardware information.
		/// </summary>
		public ulong HardwareInfo;
		/// <summary>
		/// Gets or sets a <see cref="CWLogLevel"/> value indicating how extensive logging
		/// will be used.
		/// </summary>
		public CWLogLevel LogLevel;
		/// <summary>
		/// Gets or sets a <see cref="CWConnectionSpeed"/> value indicating the speed of the
		/// user's internet connection in order to determine the crawling speed.
		/// </summary>
		public CWConnectionSpeed ConnectionSpeed;

		//entries needed for remoting configuration;

		public string RemotingHost;
		public string RemotingProtocol;
		public int RemotingPort;



		#endregion

		#region Constructors
		
		/// <summary>
		/// Constructs an instance of the <see cref="ClientSettings"/> class with the default values.
		/// </summary>
		public ClientSettings()
		{
			LoadAtStartup = false;
			MinimizeToTray = false;
			MinimizeOnExit = false;
			EnableScheduler = false;
			StartTime = DateTime.Now;
			StopTime = DateTime.Now;
			UserID = 0;
			UserName = String.Empty;
			Password = null;
			Email = String.Empty;
			ClientID = Guid.Empty;
			HardwareInfo = 0;
			LogLevel = CWLogLevel.LogInfo;
			ConnectionSpeed = CWConnectionSpeed.Unknown;

			RemotingHost = "localhost";
			RemotingProtocol = "tcp";
			RemotingPort = 15461;

			configFile = String.Empty;
		}

		/// <summary>
		/// Constructs a new instance of the <see cref="ClientSettings"/> class and attempts
		/// to load the settings from the provided file.
		/// </summary>
		/// <param name="fileName">The path of the configuration file.</param>
		public ClientSettings(string fileName)
		{
			if((fileName == null)||(fileName == String.Empty))
			{
				throw new ArgumentException("The filename cannot be empty");
			}
			LoadAtStartup = false;
			MinimizeToTray = false;
			MinimizeOnExit = false;
			EnableScheduler = false;
			StartTime = DateTime.Now;
			StopTime = DateTime.Now;
			UserID = 0;
			UserName = String.Empty;
			Password = null;
			Email = String.Empty;
			ClientID = Guid.Empty;
			HardwareInfo = 0;
			LogLevel = CWLogLevel.LogInfo;
			ConnectionSpeed = CWConnectionSpeed.Unknown;

			RemotingHost = "localhost";
			RemotingProtocol = "tcp";
			RemotingPort = 15461;

			configFile = fileName;
			LoadSettings();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Loads the settings of the application from the configuration file.
		/// </summary>
		/// <param name="fileName">The path of the configuration file.</param>
		public void LoadSettings(string fileName)
		{
			if((fileName == null)||(fileName == String.Empty))
			{
				throw new ArgumentException("The filename cannot be empty");
			}
			try
			{
				if(!File.Exists(fileName))
				{
					//perhaps the file does not exist - probably because it has not been
					//created yet. In this case just let the class retain default values.
					return;
				}
				Stream ReadStream=File.Open(fileName, FileMode.Open);
				XmlSerializer serializer=new XmlSerializer(typeof(ClientSettings));
				ClientSettings settings=(ClientSettings)serializer.Deserialize(ReadStream);
				ReadStream.Close();
				this.ClientID=settings.ClientID;
				this.ConnectionSpeed=settings.ConnectionSpeed;
				this.Email=settings.Email;
				this.EnableScheduler=settings.EnableScheduler;
				this.HardwareInfo=settings.HardwareInfo;
				this.LoadAtStartup=settings.LoadAtStartup;
				this.MinimizeOnExit=settings.MinimizeOnExit;
				this.MinimizeToTray=settings.MinimizeToTray;
				this.Password=settings.Password;
				this.StartTime = settings.StartTime;
				this.StopTime = settings.StopTime;
				this.UserID = settings.UserID;
				this.UserName = settings.UserName;

				this.RemotingHost = settings.RemotingHost;
				this.RemotingProtocol = settings.RemotingProtocol;
				this.RemotingPort = settings.RemotingPort;

			}
			catch
			{}
		}

		/// <summary>
		/// Loads the settings of the application from the configuration file.
		/// </summary>
		public void LoadSettings()
		{
			if(configFile == String.Empty)
			{
				throw new InvalidOperationException("The class cannot load its settings because the Config File path has not been initialized. Use ClientSettings(string fileName) instead.");
			}
			try
			{
				if(!File.Exists(configFile))
				{
					//perhaps the file does not exist - probably because it has not been
					//created yet. In this case just let the class retain default values.
					return;
				}
				Stream ReadStream=File.Open(configFile, FileMode.Open);
				XmlSerializer serializer=new XmlSerializer(typeof(ClientSettings));
				ClientSettings settings=(ClientSettings)serializer.Deserialize(ReadStream);
				ReadStream.Close();
				this.ClientID=settings.ClientID;
				this.ConnectionSpeed=settings.ConnectionSpeed;
				this.Email=settings.Email;
				this.EnableScheduler=settings.EnableScheduler;
				this.HardwareInfo=settings.HardwareInfo;
				this.LoadAtStartup=settings.LoadAtStartup;
				this.MinimizeOnExit=settings.MinimizeOnExit;
				this.MinimizeToTray=settings.MinimizeToTray;
				this.Password=settings.Password;
				this.StartTime = settings.StartTime;
				this.StopTime = settings.StopTime;
				this.UserID = settings.UserID;
				this.UserName = settings.UserName;

				this.RemotingHost = settings.RemotingHost;
				this.RemotingProtocol = settings.RemotingProtocol;
				this.RemotingPort = settings.RemotingPort;

			}
			catch
			{}
		}

		/// <summary>
		/// Saves the application's settings in a new xml file on disk
		/// </summary>
		/// <remarks>
		/// The class attempts to detect the assembly's codebase path using Reflection.
		/// </remarks>
		public void SaveSettings()
		{
			//first of all save the configuration file to disk
			if(configFile == String.Empty)
			{
				throw new InvalidOperationException("The SaveSettings method cannot be used because the class has not been initialized yet.");
			}
			try
			{
				Stream WriteStream=File.Open(configFile, FileMode.Create);
				XmlSerializer serializer=new XmlSerializer(typeof(ClientSettings));
				serializer.Serialize(WriteStream,this);
				WriteStream.Close();
			}
			catch
			{}
			//now update the registry if necessary
			try
			{
				RegistryKey regKey=Registry.LocalMachine;
				RegistryKey cwrKey=regKey.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",true);
				if (cwrKey!=null)
				{
					string ValueName = "CrawlWave.Client";
					if(LoadAtStartup)
					{
						if(cwrKey.GetValue(ValueName)==null)
						{
							cwrKey.SetValue(ValueName, (string)(GetAppPath() + "CrawlWave.Client.UI.exe"));
						}
					}
					else
					{
						if(cwrKey.GetValue(ValueName)!=null)
						{
							cwrKey.DeleteValue(ValueName);
						}
					}
				}
			}
			catch
			{}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Determines the application's path.
		/// </summary>
		/// <returns>The application's path, including the trailing slashes</returns>
		private static string GetAppPath()
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
	}
}
