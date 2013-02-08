using System;
using System.Globalization;
using EXCSDecl;


namespace EXCSConvert
{
	/// <summary>
	/// Summary description for EXCSConvert.
	/// </summary>
	public class TEXCSConvert
	{
//		public TEXCSConvert()
//		{
//			//
//			// TODO: Add constructor logic here
//			//
//		}

		public static decimal XRToDecimal(string strDecimal) //XXX EPIKINDINH H XRToDecimal. DES POU KALEITAI
		{
			try
			{
				return Convert.ToDecimal(strDecimal,  TEXCSDecl.Instance.nfiXrhma);
			}
			catch
			{
				return  Decimal.Parse( strDecimal, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint,TEXCSDecl.Instance.nfiXrhma  );
			}
		}
		
		public static decimal XRToDecimal(string strDecimal,decimal dcmDefault)
		{
			try
			{
				return XRToDecimal(strDecimal);
			}
			catch
			{
				return dcmDefault;
			}
		}

		public static string XRToString(decimal dcmInput)
		{
			return dcmInput.ToString(TEXCSDecl.Instance.nfiXrhma);
		}

		public static bool IsInt32(string strValue)
		{
			try
			{
				Convert.ToInt32(strValue);
				return true;
			} 
			catch 
			{
				return false;
			}
		} //IsInteger

	}
}
