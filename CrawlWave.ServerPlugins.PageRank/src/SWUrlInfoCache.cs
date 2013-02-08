using System;
using System.Data.SqlClient;
using EXCSDBUtil;

namespace SpiderWaveJobs.Classes
{
	/// <summary>
	/// Summary description for SWUrlInfoCache.
	/// </summary>
	public class SWUrlInfoCache: SWUrlInfo
	{
		SWHashIntLimit listGetNumberOfOutLinks = null;
		SWHashIntLimit listGetNumberOfInLinks = null;
		SWHashIntLimit listGetNumberOfGreekInLinks = null;
		SWHashIntLimit listGetNumberOfGreekOutLinks = null;


		public SWUrlInfoCache(SqlConnection dblgLogin):base(dblgLogin)
		{	
			listGetNumberOfOutLinks=new SWHashIntLimit(1000000);
			listGetNumberOfInLinks=new SWHashIntLimit(1000000);
			listGetNumberOfGreekOutLinks=new SWHashIntLimit(1000000);
			listGetNumberOfGreekInLinks=new SWHashIntLimit(1000000);
		}
	
		public override int GetNumberOfOutLinks(int intURLID)
		{
			int intResult=0;
			if (!(listGetNumberOfOutLinks.Find(intURLID,out intResult)))
			{			
				intResult=base.GetNumberOfOutLinks (intURLID);
				listGetNumberOfOutLinks.Add(intURLID,intResult);
			}		
			return intResult;
		}
	
		public override int GetNumberOfInLinks(int intURLID)
		{			
			int intResult=0;
			if (!(listGetNumberOfInLinks.Find(intURLID,out intResult)))
			{			
				intResult=base.GetNumberOfInLinks (intURLID);
				listGetNumberOfInLinks.Add(intURLID,intResult);
			}		
			return intResult;
		}
	
		public override int GetNumberOfGreekInLinks(int intURLID)
		{
			int intResult=0;
			if (!(listGetNumberOfGreekInLinks.Find(intURLID,out intResult)))
			{			
				intResult=base.GetNumberOfGreekInLinks (intURLID);
				listGetNumberOfGreekInLinks.Add(intURLID,intResult);
			}		
			return intResult;
		}
	
		public override int GetNumberOfGreekOutLinks(int intURLID)
		{
			int intResult=0;
			if (!(listGetNumberOfGreekOutLinks.Find(intURLID,out intResult)))
			{			
				intResult=base.GetNumberOfGreekOutLinks (intURLID);
				listGetNumberOfGreekOutLinks.Add(intURLID,intResult);
			}		
			return intResult;
		}
	}
}
