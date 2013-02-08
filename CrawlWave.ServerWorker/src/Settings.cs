using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using CrawlWave.Common;

namespace CrawlWave.ServerWorker
{
	/// <summary>
	/// Settings provides a collection of all the settings required for the ServerWorker's
	/// operation. It offers method to load and store the application's settings.
	/// </summary>
	public class Settings
	{
		#region Private Variables

		private static Settings instance;
		private SWSettings settings;
		private FileEventLogger log;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the ServerWorker
		/// should attempt to load the Plugins at startup. 
		/// </summary>
		public bool LoadPluginsAtStartup
		{
			get { return settings.LoadPluginsAtStartup; }
			set { settings.LoadPluginsAtStartup = value;}
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the ServerWorker
		/// should display an icon in the System Tray and hide the main form when Minimized.
		/// </summary>
		public bool MinimizeToTray
		{
			get { return settings.MinimizeToTray; }
			set { settings.MinimizeToTray = value;}
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the ServerWorker
		/// should minimize instead of exiting when the user presses the Close box.
		/// </summary>
		public bool MinimizeOnExit
		{
			get { return settings.MinimizeOnExit; }
			set { settings.MinimizeOnExit = value;}
		}

		/// <summary>
		/// Gets or sets an <see cref="ArrayList"/> containing the paths of the Plugins the
		/// ServerWorker must load at Startup.
		/// </summary>
		public ArrayList PluginList
		{
			get { return settings.PluginList; }
			set { settings.PluginList = value;}
		}

		/// <summary>
		/// Gets a reference to an <see cref="ILogger"/> that is used to log events.
		/// </summary>
		public FileEventLogger Log
		{
			get { return log; }
		}

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private Settings()
		{
			settings = new SWSettings();
			try
			{
				log = new FileEventLogger(GetPath() + "CrawlWave.ServerWorker.log", true, "CrawlWave.ServerWorker");
			}
			catch
			{
				log = null;
			}
			LoadSettings();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="Settings"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="Settings"/>.</returns>
		public static Settings Instance()
		{
			if (instance==null)
			{
				lock(typeof(Settings))
				{
					if( instance == null )
					{
						instance = new Settings();
					}
				}
			}
			return instance;
		}

		#endregion

		#region Static methods

		/// <summary>
		/// Determines the application's path.
		/// </summary>
		/// <returns>The application's path, including the trailing slashes</returns>
		internal static string GetPath()
		{
			string retVal="";
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

		#region Internal methods

		/// <summary>
		/// Loads the settings of the plugin from the configuration file.
		/// </summary>
		internal void LoadSettings()
		{
			string configFile = GetPath()+"CrawlWave.ServerWorker.Config.xml";
			try
			{
				if(!File.Exists(configFile))
				{
					//perhaps the file does not exist - probably because it has not been
					//created yet. In this case just let the class retain default values.
					return;
				}
				Stream ReadStream=File.Open(configFile, FileMode.Open);
				SoapFormatter serializer=new SoapFormatter();
				settings=(SWSettings)serializer.Deserialize(ReadStream);
				ReadStream.Close();
			}
			catch(Exception e)
			{
				if(log != null)
				{	  
					log.LogWarning("CrawlWave.ServerWorker failed to load its settings: " + e.ToString());
				}
			}
		}

		/// <summary>
		/// Saves the plugin's settings in a new xml file on disk
		/// </summary>
		internal void SaveSettings()
		{
			string configFile = GetPath()+"CrawlWave.ServerWorker.Config.xml";
			try
			{
				Stream WriteStream=File.Open(configFile, FileMode.Create);
				SoapFormatter serializer=new SoapFormatter();
				serializer.Serialize(WriteStream, settings);
				WriteStream.Close();
			}
			catch(Exception e)
			{
				if(log != null)
				{	  
					log.LogWarning("CrawlWave.ServerWorker failed to save its settings: " + e.ToString());
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// SWSettings provides a collection of all the settings required for the ServerWorker's
	/// operation. They allow the settings defined by the user or system to be stored in
	/// persistent form (in an XML file on disk) using serialization.
	/// </summary>
	[Serializable]
	public class SWSettings
	{
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the ServerWorker should attempt
		/// to load the Plugins at startup. 
		/// </summary>
		public bool LoadPluginsAtStartup;
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should display an
		/// icon in the System Tray and hide the main interface when Minimized.
		/// </summary>
		public bool MinimizeToTray;
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should minimize
		/// instead of exiting when the user presses the Close button on the Command Toolbox.
		/// </summary>
		public bool MinimizeOnExit;
		/// <summary>
		/// An <see cref="ArrayList"/> containing the paths of the Plugins the ServerWorker
		/// must load at Startup.
		/// </summary>
		public ArrayList PluginList;

		/// <summary>
		/// Constructs a new instance of the <see cref="SWSettings"/> class.
		/// </summary>
		public SWSettings()
		{
			LoadPluginsAtStartup = true;
			MinimizeToTray = false;
			MinimizeOnExit = false;
			PluginList = new ArrayList();
		}
	}
}
