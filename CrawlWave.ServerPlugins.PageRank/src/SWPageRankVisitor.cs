using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using EXCSDBUtil;
using EXCSCommon;

namespace SpiderWaveJobs.Classes
{
	/// <summary>
	/// 
	/// </summary>
	public class SWPageRankVisitor : SpiderWaveJobs.Classes.SWUrlBaseVisitor
	{
		protected SWRankHandler	RankHandler = null;
		protected SWUrlInfoCache		UrlInfo = null;
		protected float			PageRankDParameter=0.15f;		

		public SWPageRankVisitor(SqlConnection dblgLogin, int intInStepUrls, 
									int inIntHowManyLoops,
			SWRankHandler inRankHandler, float InPagerankDparameter):base( dblgLogin, intInStepUrls, inIntHowManyLoops)
		{
			RankHandler=inRankHandler;
			UrlInfo=new SWUrlInfoCache(dblgLogin);
			PageRankDParameter=InPagerankDparameter; //0.85 usually
			EXException.CheckEXError((PageRankDParameter>0.0f) && (PageRankDParameter<1.0f),"Problem with D Parameter in PageRank.");
		}


		public virtual ArrayList GetOutLinks(int intURLID)
		{
			return UrlInfo.GetOutLinks(intURLID);
		}

		public virtual ArrayList GetInLinks(int intURLID)
		{
			return UrlInfo.GetInLinks(intURLID);
		}

		public virtual int GetNumberOfInLinks(int intURLID)
		{
			return UrlInfo.GetNumberOfInLinks(intURLID);
		}

		public virtual int GetNumberOfOutLinks(int intURLID)
		{
			return UrlInfo.GetNumberOfOutLinks(intURLID);
		}
	
		public override void Visit(int intURLID)
		{
			base.Visit(intURLID);
			ArrayList arlsInLinks = GetInLinks(intURLID);
			float dcmPageRank=0.0f;
			if (arlsInLinks.Count>0)
			{
				float dcmPageRankOfInLink = 0.0f;
				int		intNumOfOutLinks = 0;
				foreach (int intURLInLink in arlsInLinks)
				{
					
					dcmPageRankOfInLink = RankHandler.GetRank(intURLInLink,1.0f);
					
					intNumOfOutLinks=GetNumberOfOutLinks(intURLInLink);
					dcmPageRank+=(intNumOfOutLinks==0?0.0f:dcmPageRankOfInLink/intNumOfOutLinks);
				}
				dcmPageRank=(1.0f-PageRankDParameter)+PageRankDParameter*(dcmPageRank);
				// PR(A) = (1-d) + d(PR(t1)/C(t1) + ... + PR(tn)/C(tn)) 
				// http://www.webworkshop.net/pagerank.html
			}
			else
			{
				dcmPageRank=1.0f;
			}
			RankHandler.UpdateRank(intURLID,dcmPageRank);
			if (TimeSpanPermitsToReport())
			{
				ReportIfTimeSpanPermits(string.Format("Calculated Pagerank for URL with ID:[{0}]. Pagerank is [{1:0.000000000}] with total Inlinks [{2}]. Url Iter is in count [{3}] of total [{4}]",intURLID,dcmPageRank,arlsInLinks.Count,intCurrentURL, intRowsCount));			
			}
		}

		protected override bool VisitAllURLsJustOneTime(SWUrlBaseVisitor visitor)
		{		
			if (base.VisitAllURLsJustOneTime(visitor))
			{
				return true;
			}		
			
			
			if (RankHandler.intNumUrlsThatFoundDifferenceInPrecisionDigits==0)
			{				
				Report(string.Format("----- No Pagerank found with difference more than [{0}] decimal digits. -----",RankHandler.intMinAllowedDivergedPrecisionDigits));
				//RankHandler.blnFoundURlWithoutRank = false;					
				Report("----- Trying to Flush on DB -----");
				int intCountFlush =RankHandler.DBFlush();
				Report(string.Format("----- End of Flushing. Flushed: #{0} records -----",intCountFlush));

				return true;
			}
			else
			{
				Report(string.Format("----- {0} URLs found with their Pageranks different more than [{1}] decimal digits than their previous values. -----",RankHandler.intNumUrlsThatFoundDifferenceInPrecisionDigits, RankHandler.intMinAllowedDivergedPrecisionDigits));
			}
			
			RankHandler.intNumUrlsThatFoundDifferenceInPrecisionDigits=0;
			//RankHandler.blnFoundURlWithoutRank = false;
			return false;	
		}

			
	}
}
