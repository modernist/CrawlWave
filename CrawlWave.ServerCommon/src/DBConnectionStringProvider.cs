using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CrawlWave.ServerCommon
{
	/// <summary>
	/// DBConnectionStringProvider is a Singleton class that implements a Connection String
	/// Provider. Any Server-side component or application can use this class to obtain a
	/// Connection String that will allow it to access the system's database.
	/// </summary>
	public class DBConnectionStringProvider
	{
		#region Private variables

		private static DBConnectionStringProvider instance;
		private string connectionString;
		private Settings settings;

		#endregion

		#region Constructor and Signleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private DBConnectionStringProvider()
		{
			connectionString = String.Empty;
			settings = Settings.Instance();
			LoadSettings();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="DBConnectionStringProvider"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="DBConnectionStringProvider"/>.</returns>
		public static DBConnectionStringProvider Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe.
				Mutex mutex=new Mutex();
				mutex.WaitOne();
				if( instance == null )
				{
					instance = new DBConnectionStringProvider();
				}
				mutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Provides a Connection String to all the server applications that need access to
		/// the system's database.
		/// </summary>
		/// <param name="appName">
		/// The name of the application that needs access to the system's database.
		/// </param>
		/// <returns>The Connection String that the calling application can use.</returns>
		public string ProvideDBConnectionString(string appName)
		{
			if(connectionString !=String.Empty)
			{
				return connectionString + "Application Name = " + appName + ";Pooling = false";
			}
			else
			{
				return "Password=;Persist Security Info=True;User ID=sa;Initial Catalog=CrawlWave;Data Source=.;Application Name = " + appName + ";Pooling = false";
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Creates the connection string.
		/// </summary>
		private void LoadSettings()
		{
			try
			{
				connectionString = "Password=" + settings.SQLPass + ";Persist Security Info=True;User ID=" + settings.SQLLogin + ";Initial Catalog=CrawlWave;Data Source=" + settings.SQLServer + ";";
			}
			catch
			{}
		}

		#endregion
	}
}
