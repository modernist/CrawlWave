using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using EXCSDBUtil;


namespace SpiderWaveJobs.Classes
{
	/// <summary>
	/// Summary description for SWRankDBHandle.
	/// </summary>
	public class SWRankHandler
	{
		SqlConnection sqcnMain = null;
		
		public int intMinAllowedDivergedPrecisionDigits=0;
		public float fltPowerToMultiply = 0f;
		public int intNumUrlsThatFoundDifferenceInPrecisionDigits=0;
		//public bool blnFoundURlWithoutRank = false;
		private int intRankTypeID=-1;

		public SWRankHandler(SqlConnection dblgLogin, int intInRankTypeID,
			int intInMinAllowedDivergedPrecisionDigits)
		{
			sqcnMain=dblgLogin;//TEXCSDBUtil.CreateSqlConnection(dblgLogin);			
			intMinAllowedDivergedPrecisionDigits=intInMinAllowedDivergedPrecisionDigits;
			fltPowerToMultiply=(float)Math.Pow(10,intMinAllowedDivergedPrecisionDigits);
			intRankTypeID=intInRankTypeID;
			DeleteRank(); //TODO: MAYBE I SHOULD REMOVE IT.
		}

		public virtual int DBFlush()
		{			
			return 0; //how many were flushed to DataBase.
		}

		public int RankTypeID
		{
			get
			{
				return intRankTypeID;
			}
		}

		public virtual float GetRank(int intURLID, float dcmDefaultValue)
		{
			bool blnFoundDBValue;
			return DBGetRank(RankTypeID,intURLID,dcmDefaultValue,out blnFoundDBValue);
		}
				

		public virtual void UpdateRank( int intURLID, float dcmRank)
		{
			DBUpdateRank(RankTypeID,intURLID,dcmRank);
		}

		public void DeleteRank()
		{
			DBDeleteRank(RankTypeID);			
		}


		private void DBDeleteRank(int intRankTypeID)
		{
			using (SqlCommand sqcmCommand=TEXCSDBUtil.DBStoredProcExecNonQuery(sqcnMain, "dbo.sp_SW_DeleteRank",
					   new string[]{"@intRankTypeID"}, 				
					   new object[]{intRankTypeID } ))
			{	
			}
		}	

		protected float DBGetRank(int intRankTypeID, int intURLID, float dcmDefaultValue,out bool blnFoundDBValue)
		{
			float dcmResult=dcmDefaultValue;
			byte intFoundRankValue=0;
			blnFoundDBValue=false;
			using (SqlCommand sqcmCommand=TEXCSDBUtil.DBStoredProcExecNonQuery(sqcnMain, "dbo.sp_SW_SelectRank",
					   new string[]{"@intRankTypeID", "@intURLID", "@numRank", "@intFoundRankValue"}, 				
					   new object[]{intRankTypeID,intURLID,dcmResult, intFoundRankValue  } ))
			{	
				if ( (byte)(sqcmCommand.Parameters["@intFoundRankValue"].Value)==1) 
				{					
					dcmResult=(float)((decimal)sqcmCommand.Parameters["@numRank"].Value);					
					blnFoundDBValue=true;
				} 
			}
			return dcmResult;
		}

		protected bool HasSignificantDifference(float fltOld, float fltNew)
		{
			double dcmDiff=Math.Abs((double)(fltOld-fltNew));
			if (dcmDiff>1)
			{				
				return true;
			}
			
			return ((dcmDiff*fltPowerToMultiply)>1);			
		}

		protected void DBUpdateRank(int intRankTypeID, int intURLID, float dcmRank)
		{
			float dcmOldValue=0.0f;
			int intFoundRankOldValue=0;
			
			using (SqlCommand sqcmCommand=TEXCSDBUtil.DBStoredProcExecNonQuery(sqcnMain, "dbo.sp_SW_UpdateRank",
					   new string[]{"@intRankTypeID", "@intURLID", "@numRank", "@numRankOldValue","@intFoundRankOldValue"}, 				
					   new object[]{intRankTypeID,intURLID,(decimal)dcmRank,dcmOldValue,intFoundRankOldValue   } ))
			{					
				if ((byte)sqcmCommand.Parameters["@intFoundRankOldValue"].Value==1)
				{
					try
					{
						dcmOldValue=(float)((decimal)sqcmCommand.Parameters["@numRankOldValue"].Value);
					}
					catch
					{
						dcmOldValue=float.MinValue;
					}
				}
			}
			
			if (dcmOldValue<float.MinValue+1)
			{
//				blnFoundURlWithoutRank=true;
				intNumUrlsThatFoundDifferenceInPrecisionDigits++;
				return;
			}
			
			if (HasSignificantDifference(dcmOldValue, dcmRank))
			{
				intNumUrlsThatFoundDifferenceInPrecisionDigits++;
				return;
			}
			return;			
		}
		
	}
}
