using System;
using System.Collections;
using System.IO;
using System.Reflection;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerManager
{
	/// <summary>
	/// Globals is a Singleton class holding all the common data for the ServerManager.
	/// </summary>
	public class Globals
	{
		#region Private variables

		private static Globals instance;
		private DBConnectionStringProvider dbProvider;
		private string appName;
		private string appPath;
		private Hashtable loadedForms;
		private FileEventLogger log;
		private static string [] formNames = {"frmBannedHosts","frmInsertUrl","frmUserStatistics","frmServerStatistics","frmClientUpdate","frmServerList","frmAbout"};

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private Globals()
		{
			dbProvider = DBConnectionStringProvider.Instance();
			appName = "CrawlWave.ServerManager";
			appPath = GetAppPath();
			loadedForms = new Hashtable(8);
			foreach(string formName in formNames)
			{
				loadedForms.Add(formName, null);
			}
			log = new FileEventLogger(appPath + appName + ".log", true, appName);
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
				instance = new Globals();
			}
			return instance;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets a reference to an <see cref="ILogger"/> object that can be used for logging
		/// the application's events.
		/// </summary>
		public FileEventLogger Log
		{
			get { return log; }
		}

		/// <summary>
		/// Gets a reference to a <see cref="Hashtable"/> containing references to all the
		/// application's forms.
		/// </summary>
		public Hashtable LoadedForms
		{
			get { return loadedForms; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Provides a Database Connection String that allows the Server Manager to use the
		/// system's database.
		/// </summary>
		/// <returns>A Database Connection String</returns>
		public string ProvideConnectionString()
		{
			return dbProvider.ProvideDBConnectionString(appName);
		}

		/// <summary>
		/// Checks if a form is loaded and visible.
		/// </summary>
		/// <param name="formName">The name of the form to check for.</param>
		/// <returns>True if the form is loaded and visible, false otherwise.</returns>
		public bool IsFormLoaded(string formName)
		{
			if(!loadedForms.ContainsKey(formName))
			{
				throw new ArgumentException(formName + " :Invalid Form Name");
			}
			if(loadedForms[formName]==null)
			{
				return false;
			}
			else
			{
				return true;
			}
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
