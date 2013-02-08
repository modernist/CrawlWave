using System;

namespace SW_Main
{
	/// <summary>
	/// Summary description for IProcessReport.
	/// </summary>
	public interface IProcessReport
	{
		void Report(string strReport);		

		void ReportImmediately(string strReport);		
		
		bool TimeAllowsToReport();		

		void TryToCloseReporter();		

		void DoCloseReporterImmediately();					

		bool CancelProcess
		{
			get;			
		}
		
		int MinSecondsFromLastReport
		{
			get;
			set;			
		}
	}
}
