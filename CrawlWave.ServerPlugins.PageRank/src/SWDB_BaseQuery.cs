using System;
using System.Data.SqlClient;
using System.Data;
using EXCSDecl;
using EXCSDBUtil;

namespace SW_Main
{
	/// <summary>
	/// Summary description for SWDB_BaseQuery.
	/// </summary>
	public class TSWDB_BaseQryResElement
	{
		public int intShowOrder = 0;
		public int intQueryDetailAA = 0; 
		public int intQueryID = 0;
		public int intURLId = 0;
		public string strURL = "";
		public string strPageView = "";		
		public decimal dcmPercentSuccess = 0m;		

		public TSWDB_BaseQryResElement(	int _intShowOrder, int _intQueryDetailAA, int _intQueryID, 
			int _intURLId, string _strURL, string _strPageView, decimal _dcmPercentSuccess)
		{
			intShowOrder = _intShowOrder;
			intQueryDetailAA = _intQueryDetailAA ; 
			intQueryID = _intQueryID ;
			intURLId = _intURLId;
			strURL = _strURL;
			strPageView = _strPageView;		
			dcmPercentSuccess = _dcmPercentSuccess;			
		}

		public TSWDB_BaseQryResElement()
		{
		}

	}
	
	public class TSWDB_BaseQuery
	{
		//int intQueryID = 0;
		protected SqlCommand sqcm_NewQuery= null;
		
		public DataSet dsQueryResult = null;

		public const string CQryResDataSet = "QRDs";
		public const string CQryResTable = "QRTb";
		public const string CQryResField_Prefix = "f_";
		
		public const string CQryResField_ShowOrder = CQryResField_Prefix+"ShowOrder";
		public const string CQryResField_QueryDetailAA = CQryResField_Prefix+"QueryDetailAA";
		public const string CQryResField_QueryID = CQryResField_Prefix+"QueryID";
		public const string CQryResField_URLId = CQryResField_Prefix+"URLId";
		public const string CQryResField_URL = CQryResField_Prefix+"URL";
		public const string CQryResField_PageView = CQryResField_Prefix+"PageView";		
		public const string CQryResField_PercentSuccess = CQryResField_Prefix+"PercentSuccess";			

		public int intLastShowOrderForTempInsert = 0;

		public void InsertQryResElement(TSWDB_BaseQryResElement qryresElem, bool useShowOrderInElem)
		{
			DataRow drRow = dsQueryResult.Tables[0].NewRow();			
			
			intLastShowOrderForTempInsert++;
			drRow[CQryResField_ShowOrder]=(useShowOrderInElem==true?qryresElem.intShowOrder:intLastShowOrderForTempInsert);
			drRow[CQryResField_QueryDetailAA]=qryresElem.intQueryDetailAA;
			drRow[CQryResField_QueryID]=qryresElem.intQueryID;
			drRow[CQryResField_URLId]=qryresElem.intURLId;
			drRow[CQryResField_URL]=qryresElem.strURL;
			drRow[CQryResField_PageView]=qryresElem.strPageView;
			drRow[CQryResField_PercentSuccess]=qryresElem.dcmPercentSuccess;
			
			dsQueryResult.Tables[0].Rows.Add(drRow);
		}

		private void InitLocals()
		{
			sqcm_NewQuery = TEXCSDBUtil.DBCreateSimpleStoredProc(TSWDB_Decl.GetSQLConSW(), "dbo.sp_SW_NewQuery"); //xxxx
			/*sp_SW_NewQuery ( 	@intSearchMethodType as tinyint, @strOriginalQyery as nvarchar(500) ,
						@strProcessedQuery as nvarchar(500) , @intUserAA as int, @intQueryAA as integer output) AS*/
			dsQueryResult=CreateQueryResultsSchema();
		}
		
		public TSWDB_BaseQuery(/*int _intQueryID*/)
		{
			//intQueryID = _intQueryID;		
			InitLocals();
		}

		static DataSet CreateQueryResultsSchema()
		{		
			DataSet dsResult=TEXCSDBUtil.CreateSimpleTable(	CQryResDataSet,CQryResTable,
															new TStringTypeElement(CQryResField_ShowOrder, typeof(int)),
															new TStringTypeElement[] {	new TStringTypeElement(CQryResField_QueryDetailAA, typeof(int)),
																						new TStringTypeElement(CQryResField_QueryID, typeof(int)),
																						new TStringTypeElement(CQryResField_URLId, typeof(int)),
																						new TStringTypeElement(CQryResField_URL, typeof(string)),
																						new TStringTypeElement(CQryResField_PageView, typeof(string)),
																						new TStringTypeElement(CQryResField_PercentSuccess, typeof(decimal))																					 
																					 });
			dsResult.Tables[0].DefaultView.ApplyDefaultSort=true;
			return dsResult;
		}


	}
}
