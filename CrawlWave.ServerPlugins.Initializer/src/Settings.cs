using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerPlugins.Initializer
{
	/// <summary>
	/// PluginSettings is a Singleton class that holds all the plugin's settings and provides
	/// a global point of access to them.
	/// </summary>
	public class PluginSettings
	{
		#region Private variables

		private static PluginSettings instance;
		private INPluginSettings settings;
		private Random rnd;
		private DBConnectionStringProvider dbProvider;
		private string inputFile;
		private string outputFile;
		private string appName;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private PluginSettings()
		{
			settings = new INPluginSettings();
			settings.Threads = 5;
			settings.CleanUrls = true;
			settings.CheckUrls = true;
			settings.PauseBetweenOperations = false;
			settings.PauseDelay = 0;
			dbProvider = DBConnectionStringProvider.Instance();
			rnd = new Random();
			appName = Assembly.GetExecutingAssembly().GetName().Name;
			inputFile = String.Empty;
			outputFile = String.Empty;
			LoadSettings();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="PluginSettings"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="PluginSettings"/>.</returns>
		public static PluginSettings Instance()
		{
			if (instance==null)
			{
				lock(typeof(PluginSettings))
				{
					if( instance == null )
					{
						instance = new PluginSettings();
					}
				}
			}
			return instance;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets an integer value indicating the number of threads that the plugin
		/// must use to visit the hosts contained in the input file.
		/// </summary>
		public int Threads
		{
			get { return settings.Threads; }
			set { settings.Threads = value;}
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the plugin should attempt to
		/// trim out malformed or otherwise inappropriate urls given as input and save the
		/// results to a new text file.
		/// </summary>
		public bool CleanUrls
		{
			get { return settings.CleanUrls; }
			set { settings.CleanUrls = value;}
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the plugin should attempt to
		/// visit the urls contained in the Input file in order to initialize the system's
		/// database.
		/// </summary>
		public bool CheckUrls
		{
			get { return settings.CheckUrls; }
			set { settings.CheckUrls = value;}
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the plugin should
		/// pause its operation between consecutive loops or subtasks. This option allows
		/// the adjustment of the load that the plugin puts on the system.
		/// </summary>
		public bool PauseBetweenOperations
		{
			get { return settings.PauseBetweenOperations; }
			set { settings.PauseBetweenOperations = value;}
		}

		/// <summary>
		/// Gets or sets an integer value indicating the amount of time that the plugin must
		/// pause for between each loop or subtask.
		/// </summary>
		public int PauseDelay
		{
			get { return settings.PauseDelay; }
			set { settings.PauseDelay = value;}
		}

		/// <summary>
		/// Gets or sets the path of the file containing the list of urls to check or visit.
		/// </summary>
		public string InputFile
		{
			get { return inputFile; }
			set { inputFile = value;}
		}

		/// <summary>
		/// Gets or sets the path of the file where the cleaned list of urls will be stored.
		/// </summary>
		public string OutpufFile
		{
			get { return outputFile; }
			set { outputFile = value;}
		}

		#endregion

		#region Static methods

		/// <summary>
		/// Determines the plugin's path.
		/// </summary>
		/// <returns>The plugin's path, including the trailing slashes</returns>
		internal static string GetPluginPath()
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

		#region Internal methods

		/// <summary>
		/// Loads the settings of the plugin from the configuration file.
		/// </summary>
		internal void LoadSettings()
		{
			string configFile = GetPluginPath()+"CrawlWave.ServerPlugins.Initializer.Config.xml";
			try
			{
				if(!File.Exists(configFile))
				{
					//perhaps the file does not exist - probably because it has not been
					//created yet. In this case just let the class retain default values.
					return;
				}
				Stream ReadStream=File.Open(configFile, FileMode.Open);
				XmlSerializer serializer=new XmlSerializer(typeof(INPluginSettings));
				settings=(INPluginSettings)serializer.Deserialize(ReadStream);
				ReadStream.Close();
			}
			catch
			{}
		}

		/// <summary>
		/// Saves the plugin's settings in a new xml file on disk
		/// </summary>
		internal void SaveSettings()
		{
			string configFile = GetPluginPath()+"CrawlWave.ServerPlugins.Initializer.Config.xml";
			try
			{
				Stream WriteStream=File.Open(configFile, FileMode.Create);
				XmlSerializer serializer=new XmlSerializer(typeof(INPluginSettings));
				serializer.Serialize(WriteStream, settings);
				WriteStream.Close();
			}
			catch
			{}
		}

		/// <summary>
		/// Provides a connection string to any method that needs to access the database
		/// </summary>
		/// <returns>A connection string.</returns>
		internal string ProvideDBConnectionString()
		{	
			return dbProvider.ProvideDBConnectionString(appName + rnd.Next().ToString());
		}

		#endregion
	}

	/// <summary>
	/// INPluginSettings provides a collection of all the settings required for the plugin's
	/// operation. They allow the settings defined by the user or system to be stored in
	/// persistent form (in an XML file on disk) using serialization.
	/// </summary>
	[Serializable]
	public class INPluginSettings
	{
		/// <summary>
		/// The number of to use for visiting the hosts.
		/// </summary>
		public int Threads;
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should attempt to
		/// trim out malformed or otherwise inappropriate urls given as input and save the
		/// results to a new text file.
		/// </summary>
		public bool CleanUrls;
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should visit the
		/// urls contained in the Input file in order to initialize the system's database.
		/// </summary>
		public bool CheckUrls;
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should pause its 
		/// operation between consecutive loops or subtasks.
		/// </summary>
		public bool PauseBetweenOperations;
		/// <summary>
		/// An integer value indicating the amount of time that the plugin must pause for
		/// between each loop or subtask.
		/// </summary>
		public int PauseDelay;
	}
}