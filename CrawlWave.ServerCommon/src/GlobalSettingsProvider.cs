using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace CrawlWave.ServerCommon
{
	/// <summary>
	/// GlobalSettingsProvider is a Singleton class that acts as a repository of Global settings.
	/// All the server-side applications or components can use it in order to obtain the
	/// system-wide settings, defined during system installation or by using ServerManager.
	/// </summary>
	public class GlobalSettingsProvider
	{
		#region Private variables

		private static GlobalSettingsProvider instance;
		private Settings settings;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private GlobalSettingsProvider()
		{
			settings = Settings.Instance();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="GlobalSettingsProvider"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="GlobalSettingsProvider"/>.</returns>
		public static GlobalSettingsProvider Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe.
				Mutex mutex=new Mutex();
				mutex.WaitOne();
				if( instance == null )
				{
					instance = new GlobalSettingsProvider();
				}
				mutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the maximum size of the database as defined during installation.
		/// </summary>
		public int MaxDBSize
		{
			get { return settings.MaxDBSize; }
		}

		/// <summary>
		/// Gets the path of the folder where the url crawl data files are stored.
		/// </summary>
		public string DataFilesPath
		{
			get { return settings.DataFilesPath; }
		}

		#endregion
	}
}
