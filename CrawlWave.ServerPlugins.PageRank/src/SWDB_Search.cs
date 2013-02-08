using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
//using System.Data.Common;
using EXCSCommon; 
using EXCSStrUtl;
using EXCSStrUtl.TextProcess;
using EXCSDBUtil;
using EXCSDecl;

namespace SW_Main
{
	/// <summary>
	/// Summary description for SWDB_Search.
	/// </summary>
	public class TSWDB_Search
	{
		//protected	System.Data.SqlClient.SqlConnection SQLConSW=null;
		protected SqlCommand sqcm_FindWordAA= null;

		public const string CSWDB_Search_DataSet = "TSWDB_Search_DataSet";
		public const string CSWDB_Search_Table = "TSWDB_Search_Table";
		public const string CSWDB_Search_Field_Prefix = "F_";
		public const string CSWDB_Search_Word = CSWDB_Search_Field_Prefix+"Word";		
		public const string CSWDB_Search_WordAA = CSWDB_Search_Field_Prefix+"WordAA";

		public TSWDB_Search()
		{			
			sqcm_FindWordAA = TSWDB_Decl.GetSQLConSW().CreateCommand();
			sqcm_FindWordAA.CommandType = CommandType.StoredProcedure;
			sqcm_FindWordAA.CommandText = "dbo.sp_SW_FindWordAA";					
			System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (sqcm_FindWordAA);
		}

		static DataSet CreateSWDB_SearchTableWordsSchema()
		{		
			DataSet dsResult=TEXCSDBUtil.CreateSimpleTable(CSWDB_Search_DataSet,CSWDB_Search_Table,new TStringTypeElement(CSWDB_Search_Word, typeof(string)), new TStringTypeElement[] {  new TStringTypeElement(CSWDB_Search_WordAA, typeof(int))});
			dsResult.Tables[0].DefaultView.ApplyDefaultSort=true;
			return dsResult;
			/*DataTable dttbResult = new DataTable(CTPSplitTable);
			DataColumn pkCol = dttbResult.Columns.Add(CTPSplitField_Word, typeof(string));
			dttbResult.PrimaryKey = new DataColumn[] {pkCol};
			dttbResult.Columns.Add(CTPSplitField_CountWord, typeof(int));
			DataSet dsResult = new DataSet(CTPSplitDataSet);
			dsResult.Tables.Add(dttbResult);			
			return dsResult;*/
		}


		public int FindWordAA(string strWord) //returns -1 if not found
		{			
			const int _C_RESULT_ERROR = -1;
			try
			{
				if (strWord==string.Empty)
				{
					return _C_RESULT_ERROR;
				}

				sqcm_FindWordAA.Parameters["@strWord"].Value = strWord;
				sqcm_FindWordAA.Parameters["@intWordAA"].Value = 0;
				sqcm_FindWordAA.ExecuteNonQuery();	
				return Convert.ToInt32(sqcm_FindWordAA.Parameters["@intWordAA"].Value);
			}
			catch (Exception exc)
			{
				EXException.ThrowEXException("Πρόβλημα στην ανεύρεση της λέξης ["+strWord+"]. EX-{920BD494-F896-485d-8C31-5A3D3DA3BB83}",exc);
				return _C_RESULT_ERROR;
			}
		}

		public string SQLStm_SimpleSearchByAppearCount(string strSQLQuery, DataSet dsFindWordsAA)
		{
			/*SELECT     SWUW_CDUR_URLID, 
			SWUW_AppearCount_t1 + SWUW_AppearCount_t2 + SWUW_AppearCount_t3 + SWUW_AppearCount_t4 + SWUW_AppearCount_t5 + SWUW_AppearCount_t6
			AS SWUW_AppearCount_tAll
			FROM         (MPLAMPLA ) tAll*/
			StringBuilder sbResult = new StringBuilder(string.Empty);
			DataRow[] dtarRows = dsFindWordsAA.Tables[0].Select(CSWDB_Search_WordAA+" > 0");	
								
			if (dtarRows.Length<=0)
			{
				return string.Empty;
			}
			
			//int intCurrentWordAA = 0;			
			int int_dtarRows_Length = dtarRows.Length;			
			for (int intCurrentWordAA=1; intCurrentWordAA<=int_dtarRows_Length;intCurrentWordAA++)
			{
				if (intCurrentWordAA==1)
				{
					sbResult.Append("SELECT SWUW_CDUR_URLID, ");
				}
				
				sbResult.Append(string.Format("SWUW_AppearCount_t{0}+",intCurrentWordAA));
				
			}
			sbResult.Remove(sbResult.Length-1,1);
			sbResult.Append(" AS SWUW_AppearCount_tAll FROM ( "+strSQLQuery+") tAll");
			return sbResult.ToString().Trim();

		}

		public string SQLBasicStm_FindURLSWithWordsAA(string strQuery, int intMAXWordsToSearch)
		{
			DataSet dsFindWordsAA=null;
			return SQLBasicStm_FindURLSWithWordsAA(	strQuery, intMAXWordsToSearch, out dsFindWordsAA);			
		}


		
		public string SQLBasicStm_FindURLSWithWordsAA(string strQuery, int intMAXWordsToSearch, out DataSet dsFindWordsAA)
		{
			StringBuilder sbResult = new StringBuilder(string.Empty);

			dsFindWordsAA = FindWordsAA(strQuery);
			
			if (dsFindWordsAA==null)
			{
				return string.Empty;
			}
			
			DataRow[] dtarRows = dsFindWordsAA.Tables[0].Select(CSWDB_Search_WordAA+" > 0");	
								
			if (dtarRows.Length<=0)
			{
				return string.Empty;
			}
			
			//int intCurrentWordAA = 0;
			int int_dtarRows_Length = dtarRows.Length;
			EXException.CheckEXError(int_dtarRows_Length<=intMAXWordsToSearch, "Ο αριθμός των λέξεων προς αναζήτηση είναι μεγαλύτερος του επιτρεπόμενου. Παρακαλώ ξαναπροσπαθήστε με λιγότερες λέξεις.");
			for (int intCurrentWordAA=1; intCurrentWordAA<=int_dtarRows_Length;intCurrentWordAA++)
			{
				if (intCurrentWordAA==1)
				{
					sbResult.Append("SELECT t1.SWUW_CDUR_URLID AS SWUW_CDUR_URLID,");
				}
				sbResult.Append(string.Format("t{0}.SWUW_AppearCount AS SWUW_AppearCount_t{0},",intCurrentWordAA ));
			}
			sbResult.Remove(sbResult.Length-1,1);
			
			int intCurrWordAA=0;
			System.Collections.IEnumerator enumerator=dtarRows.GetEnumerator();
			while (enumerator.MoveNext()) 
			{	
				intCurrWordAA++;
				if (intCurrWordAA==1)
				{
					sbResult.Append(" FROM ");

				}
				else
				{
					sbResult.Append(" INNER JOIN ");
				}
				sbResult.Append(string.Format(" ( SELECT SWUW_CDUR_URLID, SWUW_AppearCount FROM SW_UW_UrlLinkWord WITH (nolock) WHERE (SWUW_SWWDAA = {0})) t{1} ", Convert.ToInt32(((enumerator.Current) as DataRow)[1]),  intCurrWordAA ));
				if (intCurrWordAA>1)
				{
					sbResult.Append(string.Format(" ON t{0}.SWUW_CDUR_URLID = t{1}.SWUW_CDUR_URLID  ",intCurrWordAA-1, intCurrWordAA ));
				}
				
			}
			return sbResult.ToString().Trim();
		}
		
		/*SELECT	t1.SWUW_CDUR_URLID AS SWUW_CDUR_URLID,
					t1.SWUW_AppearCount AS SWUW_AppearCount_t1,
					t2.SWUW_AppearCount AS SWUW_AppearCount_t2, 
					t3.SWUW_AppearCount AS SWUW_AppearCount_t3, 
					t4.SWUW_AppearCount AS SWUW_AppearCount_t4, 
					t5.SWUW_AppearCount AS SWUW_AppearCount_t5, 
					t6.SWUW_AppearCount AS SWUW_AppearCount_t6
					FROM         (	SELECT     SWUW_CDUR_URLID, SWUW_AppearCount
									FROM          SW_UW_UrlLinkWord WITH (nolock)
									WHERE      (SWUW_SWWDAA = 11687)) t1 
									INNER JOIN
									(	SELECT     SWUW_CDUR_URLID, SWUW_AppearCount
										FROM          SW_UW_UrlLinkWord WITH (nolock)
										WHERE      (SWUW_SWWDAA = 11688)) t2 
									ON t1.SWUW_CDUR_URLID = t2.SWUW_CDUR_URLID 
									INNER JOIN
									(	SELECT     SWUW_CDUR_URLID, SWUW_AppearCount
										FROM          SW_UW_UrlLinkWord WITH (nolock)
										WHERE      (SWUW_SWWDAA = 11689)) t3 
									ON t2.SWUW_CDUR_URLID = t3.SWUW_CDUR_URLID 
									INNER JOIN
									(	SELECT     SWUW_CDUR_URLID, SWUW_AppearCount
										FROM          SW_UW_UrlLinkWord WITH (nolock)
										WHERE      (SWUW_SWWDAA = 11690)) t4 
									ON t3.SWUW_CDUR_URLID = t4.SWUW_CDUR_URLID 
									INNER JOIN
									(	SELECT     SWUW_CDUR_URLID, SWUW_AppearCount
										FROM          SW_UW_UrlLinkWord WITH (nolock)
										WHERE      (SWUW_SWWDAA = 11691)) t5 
									ON t4.SWUW_CDUR_URLID = t5.SWUW_CDUR_URLID 
									INNER JOIN
									(	SELECT     SWUW_CDUR_URLID, SWUW_AppearCount
										FROM          SW_UW_UrlLinkWord WITH (nolock)
										WHERE      (SWUW_SWWDAA = 11692)) t6 
									ON t5.SWUW_CDUR_URLID = t6.SWUW_CDUR_URLID */

		public string SQLWhereStm_FindWordsAA(string strFieldURLAA, string strQuery)
		{			
			string strResult = string.Empty;

			using (DataSet dsFindWordsAA = FindWordsAA(strQuery))
			{
				if (dsFindWordsAA==null)
				{
					return strResult;
				}
				
				DataRow[] dtarRows = dsFindWordsAA.Tables[0].Select(CSWDB_Search_WordAA+" > 0");	
									
				if (dtarRows.Length<=0)
				{
					return strResult;
				}
				
				int intCurrentWordAA = 0;
				System.Collections.IEnumerator enumerator=dtarRows.GetEnumerator();
				while (enumerator.MoveNext()) 
				{	
					intCurrentWordAA = Convert.ToInt32(((enumerator.Current) as DataRow)[1]);
					strResult = strResult+ " "+TEXCSDecl.C_SQL_AND + " (" + strFieldURLAA + "=" + intCurrentWordAA.ToString() + ")";
				}
				
				if (strResult!=string.Empty)
				{
					strResult=strResult.Substring(TEXCSDecl.C_SQL_AND.Length+1);
				}
				
				return strResult.Trim();
			}
			
		}

		
		public DataSet FindWordsAA(string strQuery)
		{					
			if (strQuery.Trim()==string.Empty)
			{
				return null;
			}

			DataSet dsSplitString = TTextProcess.SplitString(strQuery, TTextProcess.enmsf_StrFilter.sf_CapitalizeSmart | TTextProcess.enmsf_StrFilter.sf_DenyStopWords, TSWDB_Decl.CMAX_WORD_LENGTH);								
			EXException.CheckEXError(dsSplitString!=null,"EX-{60B7D234-F4CB-4f5e-B8CF-988023368446}");
			if (dsSplitString.Tables[0].Rows.Count<=0)
			{	
				return null;
			}			

			DataSet dsResult = CreateSWDB_SearchTableWordsSchema();
			
			string strCurrentWord=string.Empty;

			System.Collections.IEnumerator enumerator=dsSplitString.Tables[0].Rows.GetEnumerator();
			while (enumerator.MoveNext()) 
			{	
				strCurrentWord=((enumerator.Current) as DataRow)[0].ToString();
				dsResult.Tables[0].Rows.Add(new object[]{strCurrentWord, FindWordAA(strCurrentWord)});
			}

			return dsResult;
		}




	}
}
