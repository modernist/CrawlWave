using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using CrawlWave.Common;

namespace CrawlWave.ServerPlugins.WordExtraction
{
	/// <summary>
	/// 
	/// </summary>
	public class PluginSettings
	{
		#region Private Variables

		private static PluginSettings instance;
		private WEPluginSettings settings;
		private string connectionString;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the plugin should
		/// use the system's database in order to perform its task.
		/// </summary>
		public bool UseDatabase
		{
			get { return settings.UseDatabase; }
			set { settings.UseDatabase = value;}
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
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the plugin should
		/// attempt to extract words from the title tag of an html document.
		/// </summary>
		public bool ExtractTitleTag
		{
			get { return settings.ExtractTitleTag; }
			set { settings.ExtractTitleTag = value;}
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the plugin should
		/// attempt to extract words from the keywords and description meta tags of an html
		/// document.
		/// </summary>
		public bool ExtractMetaTags
		{
			get { return settings.ExtractMetaTags; }
			set { settings.ExtractMetaTags = value;}
		}

		/// <summary>
		/// Gets ot sets a <see cref="Boolean"/> value indicating whether the plugin should
		/// attempt to perfom spell checking on the extracted words.
		/// </summary>
		public bool PerformSpellChecking
		{
			get { return settings.PerformSpellChecking; }
			set { settings.PerformSpellChecking = value;}
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

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private PluginSettings()
		{
			settings = new WEPluginSettings();
			settings.UseDatabase = false;
			settings.ExtractTitleTag = true;
			settings.ExtractMetaTags = false;
			settings.PerformSpellChecking = false;
			settings.PauseBetweenOperations = false;
			settings.PauseDelay = 0;
			settings.DBActionTimeout = 30;
			//settings.DBConnectionString = String.Empty;
			LoadSettings();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="WEPluginSettings"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="WEPluginSettings"/>.</returns>
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

		#region Static methods

		/// <summary>
		/// Determines the plugin's path.
		/// </summary>
		/// <returns>The plugin's path, including the trailing slashes</returns>
		internal static string GetPluginPath()
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
			string configFile = GetPluginPath()+"CrawlWave.ServerPlugins.WordExtraction.Config.xml";
			try
			{
				if(!File.Exists(configFile))
				{
					//perhaps the file does not exist - probably because it has not been
					//created yet. In this case just let the class retain default values.
					return;
				}
				Stream ReadStream=File.Open(configFile, FileMode.Open);
				XmlSerializer serializer=new XmlSerializer(typeof(WEPluginSettings));
				settings=(WEPluginSettings)serializer.Deserialize(ReadStream);
				ReadStream.Close();
			}
			catch(Exception e)
			{
				e.ToString();
			}
		}

		/// <summary>
		/// Saves the plugin's settings in a new xml file on disk
		/// </summary>
		internal void SaveSettings()
		{
			string configFile = GetPluginPath()+"CrawlWave.ServerPlugins.WordExtraction.Config.xml";
			try
			{
				Stream WriteStream=File.Open(configFile, FileMode.Create);
				XmlSerializer serializer=new XmlSerializer(typeof(WEPluginSettings));
				serializer.Serialize(WriteStream, (WEPluginSettings)settings);
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
	/// WEPluginSettings provides a collection of all the settings required for the plugin's
	/// operation. They allow the settings defined by the user or system to be stored in
	/// persistent form (in an XML file on disk) using serialization.
	/// </summary>
	[Serializable]
	public class WEPluginSettings
	{
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should use the 
		/// system's database in order to perform its task.
		/// </summary>
		public bool UseDatabase;
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should attempt to
		/// extract words from the title tag of an html document.
		/// </summary>
		public bool ExtractTitleTag;
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should attempt to
		///  extract words from the keywords and description meta tags of an html document.
		/// </summary>
		public bool ExtractMetaTags;
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
		/// An integer value indicating the amount of time that can be consumed during each
		/// Database Action, before a Timeout error is thrown.
		/// </summary>
		public int DBActionTimeout;
		/// <summary>
		/// A <see cref="Boolean"/> value indicating whether the plugin should attempt to
		/// perfom spell checking on the extracted words.
		/// </summary>
		public bool PerformSpellChecking;

		/// <summary>
		/// Constructs a new instance of the <see cref="WEPluginSettings"/> class.
		/// </summary>
		public WEPluginSettings()
		{
			UseDatabase = false;
			ExtractTitleTag = true;
			ExtractMetaTags = false;
			PerformSpellChecking = false;
			PauseBetweenOperations = false;
			PauseDelay = 0;
			DBActionTimeout = 30;
		}
	}
}
