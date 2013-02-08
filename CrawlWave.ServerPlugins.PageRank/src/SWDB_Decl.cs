using System;
using  System.Data.SqlClient ;

namespace SW_Main
{
	/// <summary>
	/// Summary description for SWDB_Decl.
	/// </summary>
	
	sealed public class TSWDB_Decl //Singleton
	{
		public const int CMAX_WORD_LENGTH = 33;
		public const string CDBFIELD_SWUW_SWWDAA = "SWUW_SWWDAA";

		public static readonly TSWDB_Decl Instance = new TSWDB_Decl();
		public readonly SqlConnection SQLConSW;

		private TSWDB_Decl() 
		{
			SQLConSW = new System.Data.SqlClient.SqlConnection("packet size=4096;user id=sa;data source=\"APOSTOLOS\\APOSTOLOS_SQL\";persist security info=True;initial catalog=CrawlWave;Connect Timeout=100000");						
			SQLConSW.Open();		
		}

		static public SqlConnection GetSQLConSW()
		{
			return Instance.SQLConSW;
		}
	}

}
