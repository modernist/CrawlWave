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
	public class SWGreekPageRankVisitor : SWPageRankVisitor
	{
		public SWGreekPageRankVisitor(SqlConnection dblgLogin, int intInStepUrls, 
			int inIntHowManyLoops,
			SWRankHandler inRankHandler, float InPagerankDparameter):base( dblgLogin, intInStepUrls, inIntHowManyLoops, inRankHandler, InPagerankDparameter)
		{			
		}


		public override ArrayList GetOutLinks(int intURLID)
		{
			return UrlInfo.GetGreekOutLinks(intURLID);
		}

		public override ArrayList GetInLinks(int intURLID)
		{
			return UrlInfo.GetGreekInLinks(intURLID);
		}

		public override int GetNumberOfInLinks(int intURLID)
		{
			return UrlInfo.GetNumberOfGreekInLinks(intURLID);
		}

		public override int GetNumberOfOutLinks(int intURLID)
		{
			return UrlInfo.GetNumberOfGreekOutLinks(intURLID);
		}
	
		
			
	}
}
