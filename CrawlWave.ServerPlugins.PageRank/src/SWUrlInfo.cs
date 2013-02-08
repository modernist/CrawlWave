using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using EXCSDBUtil;


namespace SpiderWaveJobs.Classes
{
	/// <summary>
	/// Summary description for SWUrlInfo.
	/// </summary>
	public class SWUrlInfo
	{
		SqlConnection sqcnMain = null;

		public SWUrlInfo(SqlConnection dblgLogin)
		{
			sqcnMain=dblgLogin;//TEXCSDBUtil.CreateSqlConnection(dblgLogin);			
		}

		public ArrayList GetOutLinks(int intURLID)
		{
			ArrayList arlsResult = new ArrayList();
			
			using (DataSet dsetURLS = TEXCSDBUtil.DBStoredProcExecDataSet(sqcnMain, "dbo.sp_SW_SelectOutLinks",
					   new string[]{"@intURLID"}, 				
					   new object[]{intURLID } ))
			{
				if ((dsetURLS!=null) && (dsetURLS.Tables[0]!=null) && (dsetURLS.Tables[0].Rows.Count>0)) 
				{
					foreach (DataRow dtrwIter in dsetURLS.Tables[0].Rows)
					{
						arlsResult.Add((int)dtrwIter["cwlg_to_url_id"]);
					}
				}
			}

			return arlsResult;
		}

		public ArrayList GetInLinks(int intURLID)
		{
			ArrayList arlsResult = new ArrayList();
			
			using (DataSet dsetURLS = TEXCSDBUtil.DBStoredProcExecDataSet(sqcnMain, "dbo.sp_SW_SelectInLinks",
					   new string[]{"@intURLID"}, 				
					   new object[]{intURLID } ))
			{
				if ((dsetURLS!=null) && (dsetURLS.Tables[0]!=null) && (dsetURLS.Tables[0].Rows.Count>0)) 
				{
					foreach (DataRow dtrwIter in dsetURLS.Tables[0].Rows)
					{
						arlsResult.Add((int)dtrwIter["cwlg_from_url_id"]);
					}
				}
			}

			return arlsResult;
		}

		public virtual int GetNumberOfInLinks(int intURLID)
		{
			using (SqlCommand sqcmNumberOfInLinks = TEXCSDBUtil.DBStoredProcPrepare(sqcnMain, 
					   "dbo.sp_SW_SelectNumberOfInLinks",
					   new string[]{"@intURLID"}, 
					   new object[]{intURLID} ))
			{	
				return ((int)sqcmNumberOfInLinks.ExecuteScalar());
			}			
		}

		public virtual int GetNumberOfOutLinks(int intURLID)
		{
			using (SqlCommand sqcmNumberOfInLinks = TEXCSDBUtil.DBStoredProcPrepare(sqcnMain, 
					   "dbo.sp_SW_SelectNumberOfOutLinks",
					   new string[]{"@intURLID"}, 
					   new object[]{intURLID} ))
			{	
				return ((int)sqcmNumberOfInLinks.ExecuteScalar());
			}			
		}

//---------------------------------------------------------------------------------------

		public ArrayList GetGreekOutLinks(int intURLID)
		{
			ArrayList arlsResult = new ArrayList();
			
			using (DataSet dsetURLS = TEXCSDBUtil.DBStoredProcExecDataSet(sqcnMain, "dbo.sp_SW_SelectGreekOutLinks",
					   new string[]{"@intURLID"}, 				
					   new object[]{intURLID } ))
			{
				if ((dsetURLS!=null) && (dsetURLS.Tables[0]!=null) && (dsetURLS.Tables[0].Rows.Count>0)) 
				{
					foreach (DataRow dtrwIter in dsetURLS.Tables[0].Rows)
					{
						arlsResult.Add((int)dtrwIter["cwlg_to_url_id"]);
					}
				}
			}

			return arlsResult;
		}

		public ArrayList GetGreekInLinks(int intURLID)
		{
			ArrayList arlsResult = new ArrayList();
			
			using (DataSet dsetURLS = TEXCSDBUtil.DBStoredProcExecDataSet(sqcnMain, "dbo.sp_SW_SelectGreekInLinks",
					   new string[]{"@intURLID"}, 				
					   new object[]{intURLID } ))
			{
				if ((dsetURLS!=null) && (dsetURLS.Tables[0]!=null) && (dsetURLS.Tables[0].Rows.Count>0)) 
				{
					foreach (DataRow dtrwIter in dsetURLS.Tables[0].Rows)
					{
						arlsResult.Add((int)dtrwIter["cwlg_from_url_id"]);
					}
				}
			}

			return arlsResult;
		}

		public virtual int GetNumberOfGreekInLinks(int intURLID)
		{
			using (SqlCommand sqcmNumberOfInLinks = TEXCSDBUtil.DBStoredProcPrepare(sqcnMain, 
					   "dbo.sp_SW_SelectNumberOfGreekInLinks",
					   new string[]{"@intURLID"}, 
					   new object[]{intURLID} ))
			{	
				return ((int)sqcmNumberOfInLinks.ExecuteScalar());
			}			
		}

		public virtual int GetNumberOfGreekOutLinks(int intURLID)
		{
			using (SqlCommand sqcmNumberOfInLinks = TEXCSDBUtil.DBStoredProcPrepare(sqcnMain, 
					   "dbo.sp_SW_SelectNumberOfGreekOutLinks",
					   new string[]{"@intURLID"}, 
					   new object[]{intURLID} ))
			{	
				return ((int)sqcmNumberOfInLinks.ExecuteScalar());
			}			
		}


	}
}
