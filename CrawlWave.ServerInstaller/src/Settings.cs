using System;

namespace CrawlWave.ServerInstaller
{
	/// <summary>
	/// Summary description for Settings.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// 
		/// </summary>
		public string SQLServer;
		/// <summary>
		/// 
		/// </summary>
		public string DBAUser;
		/// <summary>
		/// 
		/// </summary>
		public string DBAPass;
		/// <summary>
		/// 
		/// </summary>
		public string CWUser;
		/// <summary>
		/// 
		/// </summary>
		public string CWPass;
		/// <summary>
		/// 
		/// </summary>
		public int DBSize;
		/// <summary>
		/// 
		/// </summary>
		public int DBSizeMax;
		/// <summary>
		/// 
		/// </summary>
		public string DBIndexesPath;
		/// <summary>
		/// 
		/// </summary>
		public string DBDataPath;
		/// <summary>
		/// 
		/// </summary>
		public string DBLogPath;
		/// <summary>
		/// 
		/// </summary>
		public string DataFilesPath;

		/// <summary>
		/// 
		/// </summary>
		public Settings()
		{
			SQLServer = String.Empty;
			DBAUser = String.Empty;
			DBAPass = String.Empty;
			CWUser = String.Empty;
			CWPass = String.Empty;
			DBSize = 1000;
			DBSizeMax = 0;
			DBIndexesPath = String.Empty;
			DBDataPath = String.Empty;
			DBLogPath = String.Empty;
			DataFilesPath = String.Empty;
		}
	}
}
