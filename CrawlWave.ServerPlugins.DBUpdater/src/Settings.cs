using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerPlugins.DBUpdater
{
	/// <summary>
	/// PluginSettings is a Singleton class that holds all the plugin's settings and provides
	/// a global point of access to them.
	/// </summary>
	public class PluginSettings
	{
		#region Private variables

		private static PluginSettings instance;
		private DUPluginSettings settings;
		private string connectionString;
		private DBConnectionStringProvider dbProvider;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private PluginSettings()
		{
			settings = new DUPluginSettings();
			settings.DataPath = String.Empty;
			settings.PauseBetweenOperations = false;
			settings.PauseDelay = 0;
			settings.UseTransactions = false;
			settings.DBActionTimeout = 60;
			dbProvider = DBConnectionStringProvider.Instance();
			connectionString = dbProvider.ProvideDBConnectionString(Assembly.GetExecutingAssembly().GetName().Name);
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
		/// Gets or sets a string containing the path of the folder where the plugin will
		/// attempt to read the data files from.
		/// </summary>
		public string DataPath
		{
			get { return settings.DataPath; }
			set { settings.DataPath = value;}
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
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the plugin should
		/// use transactions for its interactions with the database. Enabling this makes the
		/// plugin slower and causes the transaction log size to grow but it is safer.
		/// </summary>
		public bool UseTransactions
		{
			get { return settings.UseTransactions; }
			set { settings.UseTransactions = value;}
		}

		/// <summary>
		/// Gets or sets an integer value indicating the amount of time that can be consumed
		/// during each Database Action, before a Timeout error is thrown.
		/// </summary>
		public int DBActionTimeout
		{
			get { return settings.DBActionTimeout; }
			set { settings.DBActionTimeout = value;}
		}

		/// <summary>
		/// Gets or sets the connection string that the plugin uses to create a connection
		/// to the system's database if it is data dependent.
		/// </summary>
		public string DBConnectionString
		{
			get { return connectionString; }
			set { connectionString = value;}
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
			string configFile = GetPluginPath()+"CrawlWave.ServerPlugins.DBUpdater.Config.xml";
			try
			{
				if(!File.Exists(configFile))
				{
					//perhaps the file does not exist - probably because it has not been
					//created yet. In this case just let the class retain default values.
					return;
				}
				Stream ReadStream=File.Open(configFile, FileMode.Open);
				XmlSerializer serializer=new XmlSerializer(typeof(DUPluginSettings));
				settings=(DUPluginSettings)serializer.Deserialize(ReadStream);
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
			string configFile = GetPluginPath()+"CrawlWave.ServerPlugins.DBUpdater.Config.xml";
			try
			{
				Stream WriteStream=File.Open(configFile, FileMode.Create);
				XmlSerializer serializer=new XmlSerializer(typeof(DUPluginSettings));
				serializer.Serialize(WriteStream, settings);
				WriteStream.Close();
			}
			catch(Exception e)
			{
				e.ToString();
			}
		}

		#endregion
	}

	/// <summary>
	/// DUPluginSettings provides a collection of all the settings required for the plugin's
	/// operation. They allow the settings defined by the user or system to be stored in
	/// persistent form (in an XML file on disk) using serialization.
	/// </summary>
	[Serializable]
	public class DUPluginSettings
	{
		/// <summary>
		/// A string containing the path of the folder where the plugin will attempt to read
		/// the data files from.
		/// </summary>
		public string DataPath;
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
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should use transactions
		/// for its interactions with the database. Enabling this makes the plugin slower
		/// and causes the transaction log size to grow but it is safer.
		/// </summary>
		public bool UseTransactions;
		/// <summary>
		/// An integer value indicating the amount of time that can be consumed during each
		/// Database Action, before a Timeout error is thrown.
		/// </summary>
		public int DBActionTimeout;
	}
}