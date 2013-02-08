using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using EXCSDBUtil;

namespace SpiderWaveJobs.Classes
{
	/// <summary>
	/// Summary description for SWRankHandlerHash.
	/// </summary>
	public class SWRankHandlerHash: SWRankHandler
	{
		Hashtable hashRank=null;

		internal struct RankInternal
		{
			internal float fltRank;
			internal bool  blnUpdateToDB;

			internal RankInternal(float inRank, bool inUpdateToDB)
			{
				fltRank=inRank;
				blnUpdateToDB=inUpdateToDB;
			}
		}
		
		
		public SWRankHandlerHash(SqlConnection dblgLogin, int intInRankTypeID,
				int intInMinAllowedDivergedPrecisionDigits):base(dblgLogin,intInRankTypeID,intInMinAllowedDivergedPrecisionDigits)
		{			
			hashRank=new Hashtable();
		}

	
		public override int DBFlush()
		{
			//return base.DBFlush ();
			int intCountUpdated = 0;
			foreach (DictionaryEntry hashEnum in hashRank)
			{
				if (((RankInternal)hashEnum.Value).blnUpdateToDB)
				{
					DBUpdateRank(RankTypeID,(int)hashEnum.Key,((RankInternal)hashEnum.Value).fltRank);
					intCountUpdated++;
				}
			}
			return intCountUpdated;
		}
	
		public override float GetRank(int intURLID, float dcmDefaultValue)
		{
			// TODO:  Add SWRankHandlerHash.GetRank implementation
			//return base.GetRank (intURLID, dcmDefaultValue);
			if (hashRank.ContainsKey(intURLID))
			{
				return ((RankInternal)hashRank[intURLID]).fltRank;					
			}
			else
			{
				bool blnFoundDBValue;
				float fltResult=DBGetRank(RankTypeID,intURLID,dcmDefaultValue,out blnFoundDBValue);
				hashRank.Add(intURLID,new RankInternal(fltResult,false));
				return fltResult;
			}
		}
	
		public override void UpdateRank(int intURLID, float dcmRank)
		{
			// TODO:  Add SWRankHandlerHash.UpdateRank implementation
			//base.UpdateRank (intURLID, dcmRank);
			if (hashRank.ContainsKey(intURLID))
			{
				RankInternal rnkCurrent=((RankInternal)hashRank[intURLID]);
				if (HasSignificantDifference(rnkCurrent.fltRank, dcmRank))
				{
					intNumUrlsThatFoundDifferenceInPrecisionDigits++;
				}
				rnkCurrent.fltRank=dcmRank;
				rnkCurrent.blnUpdateToDB=true;
				hashRank[intURLID]=rnkCurrent;
			}
			else
			{
				hashRank.Add(intURLID,new RankInternal(dcmRank,true));
			}
		}
	}
}
