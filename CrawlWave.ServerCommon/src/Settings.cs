using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace CrawlWave.ServerCommon
{
	/// <summary>
	/// Settings is a Singleton class that holds all the settings and provides a global 
	/// point of access to them.
	/// </summary>
	public class Settings
	{
		#region Private variables

		private static Settings instance;
		private SCSettings settings;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private Settings()
		{
			settings = new SCSettings();
			settings.SQLServer = ".";
			settings.SQLLogin = "sa";
			settings.SQLPass = String.Empty;
			settings.DataFilesPath = String.Empty;
			settings.MaxDBSize = 0;
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

		#region Public Properties

		/// <summary>
		/// Gets the SQL server used for the system's data repository.
		/// </summary>
		public string SQLServer
		{
			get { return settings.SQLServer; }
		}

		/// <summary>
		/// Gets the login of the system's database
		/// </summary>
		public string SQLLogin
		{
			get { return settings.SQLLogin;}
		}

		/// <summary>
		/// Gets the password of the system's database
		/// </summary>
		public string SQLPass
		{
			get { return settings.SQLPass; }
		}

		/// <summary>
		/// Gets the path where the temporary data files are stored.
		/// </summary>
		public string DataFilesPath
		{
			get { return settings.DataFilesPath; }
		}

		/// <summary>
		/// Gets the maximum allowed database size.
		/// </summary>
		public int MaxDBSize
		{
			get { return settings.MaxDBSize; }
		}

		#endregion

		#region Static methods

		/// <summary>
		/// Determines the library's path.
		/// </summary>
		/// <returns>The library's path, including the trailing slashes</returns>
		internal static string GetPath()
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
			string configFile = GetPath()+"CrawlWave.ServerCommon.Config.xml";
			try
			{
				if(!File.Exists(configFile))
				{
					//perhaps the file does not exist - probably because it has not been
					//created yet. In this case just let the class retain default values.
					return;
				}
				Stream ReadStream=File.Open(configFile, FileMode.Open);
				XmlSerializer serializer=new XmlSerializer(typeof(SCSettings));
				settings=(SCSettings)serializer.Deserialize(ReadStream);
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
			string configFile = GetPath()+"CrawlWave.ServerCommon.Config.xml";
			try
			{
				Stream WriteStream=File.Open(configFile, FileMode.Create);
				XmlSerializer serializer=new XmlSerializer(typeof(SCSettings));
				serializer.Serialize(WriteStream, settings);
				WriteStream.Close();
			}
			catch
			{}
		}

		#endregion
	}

	/// <summary>
	/// SCSettings provides a collection of all the settings required for the library's
	/// operation. They allow the settings defined by the user or system to be stored in
	/// persistent form (in an XML file on disk) using serialization.
	/// </summary>
	[Serializable]
	public class SCSettings
	{
		/// <summary>
		/// The SQL Server used for the system's data repository.
		/// </summary>
		public string SQLServer;
		/// <summary>
		/// The login of the system's database.
		/// </summary>
		public string SQLLogin;
		/// <summary>
		/// The password of the system's database
		/// </summary>
		public string SQLPass;
		/// <summary>
		/// The path where the temporary data files will be stored.
		/// </summary>
		public string DataFilesPath;
		/// <summary>
		/// An integer value indicating the maximum allowed size of the database.
		/// </summary>
		public int MaxDBSize;
	}
}
