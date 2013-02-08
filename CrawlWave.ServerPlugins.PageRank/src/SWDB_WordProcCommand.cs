using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using EXCSCommon; 
using EXCSStrUtl;
using EXCSStrUtl.TextProcess;
using EXCSStrUtl.TextProcess.StopWords;
using System.Threading;
using EXCSDBUtil;

namespace SW_Main
{
	/// <summary>
	/// Summary description for SWDB_WordProcCommand.
	/// </summary>	
	public class TSWDB_WordProcCommand
	{
		
		public	int	intURLsAtEveryStep = 0;
		public	int	intMiliSecsToDelayAfterEveryStep =0;
		public	IProcessReport processReport=null;
		
		private uint uintStepOfWordProc=0;

		protected	System.Data.SqlClient.SqlConnection SQLConSW;
		//private		System.Data.SqlClient.SqlCommand SQLCmd_SelectURLsToWordProc;
		//private		System.Data.SqlClient.SqlCommand SQLCmd_GetURLData;

		
		private void WordProcSingleURL(int intURLID)
		{		

			using (SqlCommand sqcm_UrlHasProcessedWords = SQLConSW.CreateCommand())
			{
				//sqcm_UrlHasProcessedWords.Transaction = myWordInsTransaction;
				sqcm_UrlHasProcessedWords.CommandText = "exec dbo.sp_SW_UrlHasProcessedWords "+intURLID.ToString();
				sqcm_UrlHasProcessedWords.ExecuteNonQuery();
			}

			string strURLData=string.Empty;						
			//const string CLocalTransName = "Trans_WordProcSingleURL";
			
			SqlDataAdapter daGETUrlData = new SqlDataAdapter("EXEC dbo.sp_SW_GetURLData "+intURLID.ToString(), SQLConSW.ConnectionString);				
			DataSet dsGetURLData = new DataSet();
			daGETUrlData.Fill(dsGetURLData);

			strURLData=dsGetURLData.Tables[0].Rows[0][0].ToString().Trim();
			strURLData=TEXStrUtl.StripTagsFromHTML(strURLData).Trim();
			if (strURLData==string.Empty)
			{					
				return;
			}

			if (TimeAllowsToReport())
			{
				Report(string.Format("Δεδομένα του URL [{0}:\r\n [{1}]",intURLID,strURLData));
			}

			SqlCommand sqcm_ConnectWord= SQLConSW.CreateCommand();
			sqcm_ConnectWord.CommandType = CommandType.StoredProcedure;
			sqcm_ConnectWord.CommandText = "dbo.sp_SW_ConnectWord";					
			System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (sqcm_ConnectWord);

			//using (SqlTransaction myWordInsTransaction = SQLConSW.BeginTransaction(IsolationLevel.ReadCommitted,CLocalTransName ))
			//{
				try
				{	
					DataSet dsSplitString = TTextProcess.SplitString(strURLData, TTextProcess.enmsf_StrFilter.sf_CapitalizeSmart | TTextProcess.enmsf_StrFilter.sf_DenyStopWords, TSWDB_Decl.CMAX_WORD_LENGTH);								
					EXException.CheckEXError(dsSplitString!=null,"EX-{A3270330-505B-4eb5-98E2-313AA1879AE6}");
					EXException.CheckEXError(dsSplitString.Tables[0].Rows.Count>0,"Δεν βρέθηκαν λέξεις. EX-{E06D3CB0-B22F-49e2-87E9-D373704AB80A}"	);

					using (SqlCommand sqcm_UnconnectURLFromUrlLinkWord = SQLConSW.CreateCommand())
					{
						//sqcm_UnconnectURLFromUrlLinkWord.Transaction = myWordInsTransaction;
						sqcm_UnconnectURLFromUrlLinkWord.CommandText = "exec dbo.sp_SW_UnconnectURLFromUrlLinkWord "+intURLID.ToString();
						sqcm_UnconnectURLFromUrlLinkWord.ExecuteNonQuery();
					}			        								
														
					//sqcm_ConnectWord.Transaction = myWordInsTransaction;
					System.Collections.IEnumerator enumerator=dsSplitString.Tables[0].Rows.GetEnumerator();										
					
					while (enumerator.MoveNext()) 
					{												
						if (CancelProcess)
						{
							throw new EXUserBreak();
						}
						
						sqcm_ConnectWord.Parameters["@intURLID"].Value = intURLID;
						sqcm_ConnectWord.Parameters["@strWord"].Value = ((enumerator.Current) as DataRow)[TTextProcess.CTPSplitField_Word].ToString();
						sqcm_ConnectWord.Parameters["@intAppearCount"].Value = Convert.ToInt32(((enumerator.Current) as DataRow)[TTextProcess.CTPSplitField_CountWord]);
						if (TimeAllowsToReport())
						{
							Report(string.Format("Σύνδεση του URL:[{0}] με την λέξη:[{1}] και με συχνότητα εμφάνισης:[{2}].",intURLID,
												((enumerator.Current) as DataRow)[TTextProcess.CTPSplitField_Word].ToString(),
												Convert.ToInt32(((enumerator.Current) as DataRow)[TTextProcess.CTPSplitField_CountWord])
								));
						}
						sqcm_ConnectWord.ExecuteNonQuery();						
					}									

					//myWordInsTransaction.Commit();
				}
				catch (Exception exc)
				{
					//myWordInsTransaction.Rollback(CLocalTransName);
					EXException.ThrowEXException("Πρόβλημα κατά την αποκωδικοπόιηση του URL: "+intURLID.ToString(),exc);
				}
				
			//}
		}

		public void DoWordProcAll()
		{
			try
			{
				try
				{
					uintStepOfWordProc=0;
					while (true)
					{
						if (uint.MaxValue-(++uintStepOfWordProc)<5)
						{
							uintStepOfWordProc=0;
						}
						try
						{
							ReportImmediately(string.Format("Προσπάθεια [{0}] να κληθεί η DoWordProc.",uintStepOfWordProc));
							DoWordProc();
							if (CancelProcess)
							{
								throw new EXUserBreak();
							}
						}
						catch (Exception exc)
						{
							if (exc is EXUserBreak)
							{
								throw exc;
							}
							else
							{
								ReportImmediately("ΠΡΟΒΛΗΜΑ !!!!!!!!!! " +exc.Message);
							}
						}
					}
				}
				catch (EXUserBreak excUser)
				{
				}
			}
			finally
			{
				ReportImmediately("ΤΕΛΟΣ του WORD EXTRACTION." );
				if (processReport!=null)
				{
					processReport.TryToCloseReporter();
				}
			}
		}

		public void DoWordProc()
		{
			if (CancelProcess)
			{
				throw new EXUserBreak();
			}
			
			ReportImmediately("Κλήση της sp_SW_SelectURLsToWordProc.");
			try
			{
				SqlCommand sqcm_URLsToIterate= SQLConSW.CreateCommand();
				sqcm_URLsToIterate.CommandType = CommandType.StoredProcedure;
				sqcm_URLsToIterate.CommandText = "dbo.sp_SW_SelectURLsToWordProc";					
				System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (sqcm_URLsToIterate);

				sqcm_URLsToIterate.Parameters["@intHowManyURLs"].Value = intURLsAtEveryStep;
				sqcm_URLsToIterate.CommandTimeout=SQLConSW.ConnectionTimeout;
				/*SqlDataReader sqdr_URLs = sqcm_URLsToIterate.ExecuteReader( );*/
			
				SqlDataAdapter daURLsToIterate = new SqlDataAdapter( sqcm_URLsToIterate);			
				DataSet dsURLsToIterate = new DataSet();			
				daURLsToIterate.Fill(dsURLsToIterate);
			

				if (dsURLsToIterate.Tables[0]==null)
				{
					ReportImmediately("Δεν υπάρχουν δεδομένα.");
					return; //xxx
				}	
			
				uint uintHowManyRecords=(uint)dsURLsToIterate.Tables[0].Rows.Count;
				ReportImmediately(string.Format("Θα εξεταστούν [{0}] εγγραφές.",uintHowManyRecords));
			
				System.Collections.IEnumerator enumerator=dsURLsToIterate.Tables[0].Rows.GetEnumerator();
				uint uintRowCounter=0;
				while (enumerator.MoveNext()) 
				{		
					
					if (CancelProcess)
					{
						throw new EXUserBreak();
					}

					try
					{
						int intURLID=Convert.ToInt32(((enumerator.Current) as DataRow)[0]);
						if (intURLID>0)
						{
							WordProcSingleURL(intURLID);
							++uintRowCounter;
							if (TimeAllowsToReport())
							{
								Report(string.Format("Τρέχουσα επεξεργασία: Προσπάθεια [{0}], Url [{1}] από τα [{2}]",uintStepOfWordProc,uintRowCounter,uintHowManyRecords));
							}
							if (intMiliSecsToDelayAfterEveryStep>0)
							{
								Thread.Sleep(intMiliSecsToDelayAfterEveryStep);
							}
						}
					
					}
					catch (Exception exc)
					{						
						string strErr=exc.Message;
						ReportImmediately("ΠΡΟΒΛΗΜΑ !!! "+strErr);
						//EXException.ThrowEXException("Πρόβλημα!", exc);
					}				
				}	
			}
			finally
			{
				ReportImmediately("Τέλος της DoWordProc.");
			}			
		}	
				
		private void Report(string strReport)
		{
			if (processReport!=null)
			{
				processReport.Report(strReport);
			}
		}

		private void ReportImmediately(string strReport)
		{
			if (processReport!=null)
			{
				processReport.ReportImmediately(strReport);
			}
		}

		private bool TimeAllowsToReport()
		{			
			if (processReport!=null)
			{
				return processReport.TimeAllowsToReport();
			}
			return false;
		}


		private bool CancelProcess
		{
			get
			{
				if (processReport!=null)
				{
					return processReport.CancelProcess;
				}
				return false;
			}
		}
		
		public  TSWDB_WordProcCommand(int _intURLsAtEveryStep, int _intMiliSecsToDelayAfterEveryStep) //constructor
		{
			intURLsAtEveryStep = _intURLsAtEveryStep;	
			intMiliSecsToDelayAfterEveryStep=_intMiliSecsToDelayAfterEveryStep;
			processReport=null;
			SQLConSW=TSWDB_Decl.GetSQLConSW();
		}

		public  TSWDB_WordProcCommand(TEXCSDBUtil.DBLGDatabaseLogin _dbgLogin, IProcessReport _procReport, int _intURLsAtEveryStep, int _intMiliSecsToDelayAfterEveryStep)
		{			
			intURLsAtEveryStep = _intURLsAtEveryStep;	
			intMiliSecsToDelayAfterEveryStep=_intMiliSecsToDelayAfterEveryStep;
			processReport=_procReport;
			//processReport.ShowReporter();

			SQLConSW=TEXCSDBUtil.CreateSqlConnection(_dbgLogin);			

		}
	}

	public class EXUserBreak:Exception
	{
		public EXUserBreak():base()
		{
		}
	}
}
