using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using CrawlWave.ServerInstaller.Forms;

namespace CrawlWave.ServerInstaller
{
	/// <summary>
	/// Summary description for Globals.
	/// </summary>
	public class Globals
	{
		#region Private variables

		private static Globals instance;
		private Hashtable loadedForms;
		private Settings settings;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private Globals()
		{
			loadedForms = new Hashtable(8);
			settings = new Settings();
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

		#region Public Properties

		/// <summary>
		/// Gets a <see cref="Hashtable"/> that holds the names of the loaded forms.
		/// </summary>
		public Hashtable LoadedForms
		{
			get { return loadedForms; }
		}

		/// <summary>
		/// Gets the application's <see cref="Settings"/>.
		/// </summary>
		public Settings ConfigurationSettings
		{
			get { return settings; }
		}

		#endregion

		#region Public methods

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

		#region Internal Static Methods

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
	}
}
