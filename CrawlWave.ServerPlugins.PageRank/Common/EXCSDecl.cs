using System;
using System.Globalization;

namespace EXCSDecl //test Sourcesafe de
{
	/// <summary>
	/// Summary description for EXCSDecl.
	/// </summary>
	/// 
	
	sealed public class TEXCSDecl //Singleton
	{
		public const string CRSFieldPrefix = "EXRSField_";
		public const string CRSTablePrefix = "EXRSTable_";

		public const string CGenTablePrefix = "T_";
		public const string CGenFieldPrefix = "F_";

		public const string C_MSG_GR_Problem = "–—œ¬À«Ã¡";


		public const string C_FMTDefSmallGreekDateTime = "dd/MM/yyyy";
		public const string C_FMTYearMonthDateNoSlash = "yyyyMMdd";

		public const string C_SQL_AND = "AND";

		public static readonly TEXCSDecl Instance = new TEXCSDecl();

		public readonly NumberFormatInfo nfiXrhma;

		private TEXCSDecl() 
		{
			nfiXrhma = (NumberFormatInfo.CurrentInfo.Clone() as NumberFormatInfo);			
			nfiXrhma.NumberGroupSeparator = ",";
			nfiXrhma.NumberDecimalSeparator = ".";				
		}
	}

	public class TStringTypeElement
	{
		public string	elmString;
		public Type		elmType;

		public TStringTypeElement(string _elmString, Type _elmType)
		{
			elmString	= _elmString;
			elmType		= _elmType;
		}
	}
}
