using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using EXCSDBUtil;

namespace SpiderWaveJobs.Classes
{	
	
	
	/// <summary>
	/// Summary description for SWUrlBaseVisitor.
	/// </summary>
	public class SWUrlBaseVisitor
	{
		SqlConnection sqcnMain = null;
		int intLastUrlAsked = 0;
		int intStepUrls = 0;
		int intStepOfVisit = 1;
		int intHowManyLoops = 1;		
		protected int intRowsCount = 0;
		protected int intCurrentURL =0;
		DateTime C_MINDATE = new DateTime(1980,1,1);
		DateTime dtLastTimeReported; //= C_MINDATE;

		public TimeSpan tsReportSpan = TimeSpan.FromMilliseconds(30000);
		public int TotalUrlsProcessed =0;
		
		public SWUrlBaseVisitor(SqlConnection dblgLogin, int intInStepUrls, int intInHowManyLoops)
		{
			sqcnMain=dblgLogin;//TEXCSDBUtil.CreateSqlConnection(dblgLogin);
			intStepUrls=intInStepUrls;	
			intHowManyLoops=intInHowManyLoops;
			dtLastTimeReported=C_MINDATE;
		}


		/// <summary>
		/// ReportTriggerEventArgs is a class derived from EventArgs and is
		/// used for passing aguments to the events 
		/// </summary>
		public class ReportTriggerEventArgs : EventArgs
		{
			public readonly string strReport;

			public ReportTriggerEventArgs(string strInReport)
			{
				strReport=strInReport;
			}
		}	
		
		
		public delegate void ReportTrigger(object UrlBaseVisitor, ReportTriggerEventArgs e);
		//the event the clients will subscribe to
		public event ReportTrigger OnReportTrigger;

		//public delegate void Report(string strReport);

		DataSet GetNextURLs()
		{
			Report(string.Format("----- Trying to get the next {0} Urls -----",intStepUrls));
			DataSet dsetURLS = TEXCSDBUtil.DBStoredProcExecDataSet(sqcnMain, "dbo.sp_SW_SelectGreekURLIDs",
				new string[]{"@intFromURLID", "@intToURLID"}, 				
				new object[]{intLastUrlAsked+1,intLastUrlAsked+intStepUrls } );
			intLastUrlAsked+=intStepUrls;
			Report(string.Format("----- Succesfully got {0} Urls. Tried to get {1} Urls -----",dsetURLS.Tables[0].Rows.Count,intStepUrls));
			return dsetURLS;
		}

		protected virtual bool VisitAllURLsJustOneTime(SWUrlBaseVisitor visitor)
		{
			DataSet dsetURLS = GetNextURLs();
			while ((dsetURLS!=null) && (dsetURLS.Tables!=null) && (dsetURLS.Tables[0].Rows.Count>0))
			{
				intRowsCount=dsetURLS.Tables[0].Rows.Count;				
				intCurrentURL=0;
				Report(string.Format("----- Trying to visit {0} URLs. Searched from URLS with ID {1} to {2} -----",intRowsCount,intLastUrlAsked-intStepUrls,intLastUrlAsked ));
				foreach (DataRow dtrwIter in dsetURLS.Tables[0].Rows)
				{
					visitor.Visit((int)dtrwIter["cwur_url_id"]);
				}
				
				TotalUrlsProcessed+=intRowsCount;
				try
				{
					dsetURLS.Dispose();
				}
				catch {}

				dsetURLS = GetNextURLs();
			}
			return false; //continue if false/ if true then it must stop.
		}

		public bool VisitAllURLs(SWUrlBaseVisitor visitor)
		{
			try
			{
				Report(string.Format("----- Trying to perform {0} visits -----",intHowManyLoops));
				TotalUrlsProcessed=0;
				for (intStepOfVisit=1; intStepOfVisit<=intHowManyLoops;intStepOfVisit++)
				{
					intLastUrlAsked=0;
					Report(string.Format("----- Start of visit #{0} -----",intStepOfVisit));
					if (VisitAllURLsJustOneTime(visitor))
					{
						Report(string.Format("----- Success & Break at #{0} visit -----",intStepOfVisit));
						break;
					}					
					
					Report(string.Format("----- End of visit #{0} -----",intStepOfVisit));

				}	
				Report(string.Format("----- Finished of trying to perform {0} visits. Performed {1} of them. Total URLs visited : {2} -----",intHowManyLoops,intStepOfVisit,TotalUrlsProcessed));
				return true;
			}
			catch (Exception exc)
			{
				try
				{
					Report(string.Format("----- ERROR !!!! {0} -----",exc.Message));
				}
				catch(Exception excNew)
				{
					Report(string.Format("----- INTERNAL ERROR !!!! {0} -----",excNew.Message));
				}
				return false;
			}
		}

		public virtual void Visit(int intURLID)
		{		
			intCurrentURL++;			
		}

		protected bool TimeSpanPermitsToReport()
		{
			return ((DateTime.Now - dtLastTimeReported) > tsReportSpan);
		}

		protected void ReportIfTimeSpanPermits(string strReport)
		{
			if (OnReportTrigger!=null)
			{
				if (TimeSpanPermitsToReport())
				{
					Report(strReport);
				}
			}
		}

		protected void Report(string strReport)
		{
			if (OnReportTrigger!=null)
			{
				ReportTriggerEventArgs rteaReport = new ReportTriggerEventArgs(strReport);
				OnReportTrigger(this,rteaReport);
				dtLastTimeReported=DateTime.Now;
			}
		}
	}
}
