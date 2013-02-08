using System;
using EXCSCommon;

namespace EXMath
{
	/// <summary>
	/// Summary description for EXCSMath.
	/// </summary>
	public class TEXCSMath
	{
		public TEXCSMath()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static public void UInt32ToFourBytes(UInt32 uint32Input, out byte bt1, out byte bt2, out byte bt3, out byte bt4)
		{
			bt1 = (byte)((uint32Input&0xF000)>>12);
			bt2 = (byte)((uint32Input&0x0F00)>>8);
			bt3 = (byte)((uint32Input&0x00F0)>>4);
			bt4 = (byte)(uint32Input&0x000F); 
		}

		static public void FourBytesToUInt32(byte bt1, byte bt2, byte bt3, byte bt4, out UInt32 uint32Input)
		{
			uint32Input=0x0000;
			UInt32 uint1 = (UInt32)bt1;
			UInt32 uint2 = (UInt32)bt2;
			UInt32 uint3 = (UInt32)bt3;
			UInt32 uint4 = (UInt32)bt4;

			uint1=uint1<<12;
			uint2=uint2<<8;
			uint3=uint3<<4;
			//uint4=uint1<<12;
			
			uint32Input = (uint1|uint2|uint3|uint4);
		}
			


		static public decimal XRRoundToInt(decimal dcmToRound)
		{
			decimal dcmTrunced =Math.Abs(dcmToRound-decimal.Truncate(dcmToRound));
			if (dcmTrunced>=0.5m)
			{
				return (dcmToRound<0?decimal.Floor(dcmToRound):decimal.Truncate(dcmToRound));
			}
			else
			{
				return (dcmToRound<0?decimal.Truncate(dcmToRound):decimal.Floor(dcmToRound));
			}
		}

		static public bool IsXRRounded(decimal dcmXAATickerValue)
		{
			return dcmXAATickerValue==XRRoundXAA(dcmXAATickerValue);
		}

		static public decimal XRPriceMinimumStep(decimal dcmXAATickerValue)
		{			
			if ((dcmXAATickerValue>=0m) && (dcmXAATickerValue<3m))
			{
				return 	0.01m;
			}	
			if ((dcmXAATickerValue>=3m) && (dcmXAATickerValue<60m))
			{
				return 	0.02m;
			}
			if ((dcmXAATickerValue>60m) && (dcmXAATickerValue<3m))
			{
				return 	0.05m;
			}
			EXException.ThrowEXException(string.Format("Δεν βρέθηκε σωστό βήμα για την τιμή {0}.", dcmXAATickerValue));
			return 0m;
		}


		static public decimal XRRoundXAA(decimal dcmXAATickerValue)
		{
			decimal dcmResult=decimal.Round(dcmXAATickerValue, 2);
			if ((dcmResult>=0m) && (dcmResult<3m))
			{
				return 	XRRoundBasedOnTickSize(dcmResult,0.01m,2);
			}	
			if ((dcmResult>=3m) && (dcmResult<60m))
			{
				return 	XRRoundBasedOnTickSize(dcmResult,0.02m,2);
			}
			if (dcmResult>60m)
			{
				return 	XRRoundBasedOnTickSize(dcmResult,0.05m,2);
			}
			EXException.ThrowEXException(string.Format("Δεν βρέθηκε σωστό βήμα για την τιμή {0}.", dcmXAATickerValue));
			return 0m;
		}


		static public decimal XRRoundBasedOnTickSize(decimal dcmInput,decimal dcmTickerSizer/* 0.01,0.02, 0.05*/, int intRoundToDigits)
		{
			decimal dcmResult=decimal.Round(dcmInput, intRoundToDigits);
			decimal dcmTemp = (dcmResult / dcmTickerSizer);
			if (XRRoundToInt(dcmTemp)!=dcmTemp)
			{
				dcmResult=XRRoundToInt(dcmTemp)*dcmTickerSizer;	
			}
			return dcmResult;
		}
	}
}
