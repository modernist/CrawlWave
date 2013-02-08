using System;
using System.Data;
using System.Data.SqlClient;
using EXCSCommon;
using EXCSCrypt;
using EXCSDecl;

namespace EXCSDBUtil
{
	/// <summary>
	/// TEXCSDBUtil is a utility class that handles all the DataBase utility functions.
	/// </summary>
	/// 	


	sealed public class TEXCSDBUtil
	{
		public enum enmdb_EXDBApplUser 
		{	
			db_EXSQL06_EX_Itr_Authenticator_itr_user=0,
			db_EXSQL06_TEMP3_EX_Itr_Authenticator_itr_user=1,
			db_EXFS06_EX_sa=2,
			db_EXSQL07_DBEXTrades_sa=3,
			db_EXFS06_EX_sa_InfiniteTimeOut=4//opote allaksei edw na allaksei kai to sqcnArr
		};
		
		public struct DBLGDatabaseLogin
		{
			public string	UserName;
			public string	Password;
			public string	Server;
			public string	InitialCatalog;
			public int		ConnectionTimeout;
		};
		
		public const string C_DTSTFIELD_ZOOM_ID = "ID";
		public const string C_DTSTFIELD_ZOOM_DESCRIPTION = "DESCRIPTION";		
		
		const string C_KEY_TEXCSDBUTIL = "EX-{D28A5351-9BEA-4f26-87A8-73CDFEC6B7AD}";

		public static readonly TEXCSDBUtil Instance = new TEXCSDBUtil();
		
		private SqlConnection[] sqcnArr =new SqlConnection[] {null,null,null,null,null}; // osa einai sto enum

		public static string DBGetConnectionStr(enmdb_EXDBApplUser theDB)
		{
			//dummy
			return string.Empty;
		}
		
		public SqlConnection GetOrCreateConnection(enmdb_EXDBApplUser theDB)
		{
			try
			{
				int intDBIndex = (int)theDB;
				if (sqcnArr[intDBIndex]==null)
				{
					sqcnArr[intDBIndex]=new SqlConnection(DBGetConnectionStr(theDB));
					sqcnArr[intDBIndex].Open();
				}
				return sqcnArr[intDBIndex];
			}
			catch (Exception exc)
			{
				EXException.ThrowEXException("Πρόβλημα στην δημιουργία σύνδεσης με την Βάση Δεδομένων.\n[EX-{FD48C6FC-5193-4392-9560-08D9FB7760F6}]",exc);
				return null; //never gets here
			}
		}

		public static SqlCommand DBStoredProcExecNonQuery(	enmdb_EXDBApplUser theDB, string strStoredProcName, 
			string[] strArrParametersNames, 
			object[] objArrParametersValues)
		{
			return DBStoredProcExecNonQuery(Instance.GetOrCreateConnection(theDB),strStoredProcName,strArrParametersNames, objArrParametersValues);
		}		

		
		public static SqlCommand DBStoredProcPrepare(enmdb_EXDBApplUser theDB, string strStoredProcName, 
			string[] strArrParametersNames, 
			object[] objArrParametersValues)
		{
			return DBStoredProcPrepare(Instance.GetOrCreateConnection(theDB),strStoredProcName,strArrParametersNames, objArrParametersValues);
		}
		
		public static SqlCommand DBStoredProcPrepare(SqlConnection sqlConOfSP, string strStoredProcName, 
			string[] strArrParametersNames, 
			object[] objArrParametersValues)
		{
			try
			{
				SqlCommand sqcmResult = DBCreateSimpleStoredProc(sqlConOfSP, strStoredProcName);
				sqcmResult.CommandTimeout=sqlConOfSP.ConnectionTimeout;
				if ((strArrParametersNames!=null) && (objArrParametersValues!=null))
				{
					int intArrParNamesCount = strArrParametersNames.GetLength(0)-1;
					int intParNamesCount = objArrParametersValues.GetLength(0)-1;
					EXException.CheckEXError(intArrParNamesCount==intParNamesCount,"Ο αριθμός των παραμέτρων με τον αριθμό των τιμών δεν είναι ίδιος.\n[EX-{74052B8F-FBBD-4e43-B50A-9F9E5DBE9970}]");
					for (int intCounter1=0;intCounter1<=intArrParNamesCount;intCounter1++)
					{
						sqcmResult.Parameters[strArrParametersNames[intCounter1]].Value = objArrParametersValues[intCounter1];
					}				
				}
				sqcmResult.CommandTimeout=sqlConOfSP.ConnectionTimeout;
				return sqcmResult;
			}
			catch (Exception exc)
			{
				EXException.ThrowEXException(string.Format("Πρόβλημα στην προετοιμασία της Stored Procedure [{0}].", strStoredProcName),exc);
				return null; //never gets here				
			}

		}
		
		public static SqlCommand DBCreateSimpleStoredProc(SqlConnection sqlConOfSP, string strStoredProcName)
		{
			try
			{
				SqlCommand sqcmResult = sqlConOfSP.CreateCommand();
				sqcmResult.CommandType = CommandType.StoredProcedure;
				sqcmResult.CommandText = strStoredProcName; //"dbo.sp_SW_CheckUser";					
				System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (sqcmResult);
				sqcmResult.CommandTimeout=sqlConOfSP.ConnectionTimeout;
				return sqcmResult;
			}
			catch (Exception exc)
			{
				EXException.ThrowEXException(string.Format("Πρόβλημα στην δημιουργία της Stored Procedure [{0}].", strStoredProcName),exc);
				return null; //never gets here
			}
		}

		
		public static SqlCommand DBStoredProcExecNonQuery(SqlConnection sqlConOfSP, string strStoredProcName, 
			string[] strArrParametersNames, 
			object[] objArrParametersValues)
		{
			try
			{
				SqlCommand sqcmResult =DBStoredProcPrepare(sqlConOfSP, strStoredProcName, strArrParametersNames,objArrParametersValues);
				sqcmResult.CommandTimeout=sqlConOfSP.ConnectionTimeout;
				sqcmResult.ExecuteNonQuery();
				return sqcmResult;
			}
			catch (Exception exc)
			{
				EXException.ThrowEXException(string.Format("Πρόβλημα στην κλήση της Stored Procedure [{0}].", strStoredProcName),exc);
				return null; //never gets here				
			}
		}
		

		public static void DBExecuteQuery(SqlCommand sqcm,DataSet dsResults)
		{//OURS!!!
			using (SqlDataAdapter adapter = new SqlDataAdapter())
			{
				adapter.SelectCommand = sqcm;
				adapter.SelectCommand.CommandTimeout=0;
				adapter.Fill(dsResults,dsResults.Tables[0].TableName);
			}
		}

		public static DataSet DBExecuteQuery(SqlConnection sqlcConnection,string strQuery) 
		{
			DataSet dsResult = new DataSet();
			using (SqlDataAdapter adapter = new SqlDataAdapter())
			{
				adapter.SelectCommand.CommandTimeout=sqlcConnection.ConnectionTimeout;
				adapter.Fill(dsResult);
			}
			return dsResult;
		}

		public static DataSet DBExecuteQuery(string connection,string query) 
		{
			DataSet dataset = new DataSet();
			using (	SqlConnection conn = new SqlConnection(connection))
			{
				return DBExecuteQuery(conn, query);
			}
		}

		public static DataSet DBExecuteQuery(enmdb_EXDBApplUser theDB,string strQuery) 
		{
			return DBExecuteQuery(Instance.GetOrCreateConnection(theDB), strQuery);
		}


		public static DataTable CreateZoomDataTable(string strTableName)
		{
			DataTable dttbResult = new DataTable(strTableName);

			DataColumn pkCol = dttbResult.Columns.Add(C_DTSTFIELD_ZOOM_ID, typeof(Int32));
			dttbResult.PrimaryKey = new DataColumn[] {pkCol};
			dttbResult.Columns.Add(C_DTSTFIELD_ZOOM_DESCRIPTION, typeof(string));
			return dttbResult;
		}

		public static int FindIDInZoomtable(DataTable dttbZoom, string strDesc) //int.MinValue if not found
		{
			DataRow[] dtarRows = dttbZoom.Select(C_DTSTFIELD_ZOOM_DESCRIPTION+" = '"+strDesc+"'");	
									
			if (dtarRows.Length<=0)
			{
				return int.MinValue;
			}
			return Convert.ToInt32(dtarRows[0][0]);
		}

		static public DataTable CreateSimpleTable(string strTableName, TStringTypeElement stePrimaryKey, TStringTypeElement[] lstFields)
		{
			DataTable dttbResult = new DataTable(strTableName);
			if (stePrimaryKey!=null)
			{
				DataColumn pkCol = dttbResult.Columns.Add(stePrimaryKey.elmString, stePrimaryKey.elmType);
				dttbResult.PrimaryKey = new DataColumn[] {pkCol};
			}
			if (lstFields!=null)
			{
				if (lstFields.Length>0)
				{
					foreach(TStringTypeElement steField in lstFields)
					{
						if (steField!=null)
						{
							dttbResult.Columns.Add(steField.elmString, steField.elmType);
						}
					}
				}
			}
			return dttbResult;			
		}

		static public DataSet CreateSimpleTable(string strDataSetName, string strTableName, TStringTypeElement stePrimaryKey, TStringTypeElement[] lstFields)
		{
			DataTable dttbSimpleTable = CreateSimpleTable(strTableName,stePrimaryKey,lstFields);
			DataSet dsResult = new DataSet(strDataSetName);
			dsResult.Tables.Add(dttbSimpleTable);			
			return dsResult;
		}

		static public string CreateLoginString(DBLGDatabaseLogin dblgLogin)
		{			
			//cwrKey.SetValue(C_SW_RegConStr,"Data Source=.;Initial Catalog=CrawlWave;User ID=sa;Password=ln^!@xs!4;Persist Security Info=True;pooling=false");

			//string strResult=string.Format("database={0};server={1};user id={2};password={3};Network Library=DBMSSOCN;pooling=false",
			//cwrKey.SetValue(C_SW_RegConStr,"Data Source={0};Initial Catalog={1};User ID={2};Password={3};Persist Security Info=True;pooling=false");
			string strResult=string.Format("initial catalog={0};user id={2};data source=\"{1}\";persist security info=True;connect timeout={4};pooling=false;password=\"{3}\"",
				dblgLogin.InitialCatalog, dblgLogin.Server, dblgLogin.UserName, dblgLogin.Password,dblgLogin.ConnectionTimeout);

			//initial catalog=CrawlWave;user id=sa;data source=".";persist security info=True;connect timeout=0;pooling=false;password="ln^!@xs!4"
			return strResult;
		}

		static public SqlConnection CreateSqlConnection(DBLGDatabaseLogin dblgLogin)
		{
			SqlConnection sqcnResult = new SqlConnection(CreateLoginString(dblgLogin));
			sqcnResult.Open();
			return sqcnResult;
		}

		public static DataSet DBStoredProcExecDataSet(	enmdb_EXDBApplUser theDB, string strStoredProcName, 
			string[] strArrParametersNames, 
			object[] objArrParametersValues)
		{
			return DBStoredProcExecDataSet(Instance.GetOrCreateConnection(theDB),strStoredProcName,strArrParametersNames,objArrParametersValues);
		}

		public static DataSet DBStoredProcExecDataSet(	SqlConnection sqlConOfSP, string strStoredProcName, 
			string[] strArrParametersNames, 
			object[] objArrParametersValues)
		{
			DataSet dsResult=null;
			using (SqlCommand sqcmCommand=DBStoredProcPrepare(sqlConOfSP,strStoredProcName,strArrParametersNames, objArrParametersValues))
			{			
				using (SqlDataAdapter sqdaAdapter=new SqlDataAdapter(sqcmCommand))
				{
					dsResult=new DataSet();
					sqdaAdapter.Fill(dsResult);					
				}			
			}
			return dsResult;
		}
		

	}


}
