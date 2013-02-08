using System;
using System.Data;
using System.Text;
using EXCSDBUtil;
using EXCSDecl;

namespace EXCSStrUtl.TextProcess
{
	/// <summary>
	/// Summary description for EXCSStrUtl_TextProcess.
	/// </summary>
	public class TTextProcess
	{
		[Flags()]
		public enum enmsf_StrFilter 
		{	//db_EXSQL06_EX_sa = 0, 			
			sf_CapitalizeSmart =	0x0001,
			sf_DenyStopWords   =	0x0002,
			All = sf_CapitalizeSmart | sf_DenyStopWords
		};

		public enum enmrwd_ResultOfInsertWord 
		{	//db_EXSQL06_EX_sa = 0, 
			rwd_NewWord,
			rwd_WordExist,
			rwd_NoValidWord
		};

		[Flags()]
		public enum enmcpw_CapitalizeWay
		{
			cpw_KillTonosLeksis			=	0x0001,
			cpw_UseWindowsUpperFunction =	0x0002,
			//			Read   = 0x0001,
			//			Write  = 0x0002,
			//			Delete = 0x0004,
			//			Query  = 0x0008,
			//			Sync   = 0x0010 
			//			All = Created | Deleted | Changed | Renamed
		}
		
		public const string CTPSplitDataSet = "TPSPDataSet_Words";
		public const string CTPSplitTable = "TPSPDataTable_Words";
		public const string CTPSplitField_Prefix = "TPSPField_";
		public const string CTPSplitField_Word = CTPSplitField_Prefix+"Word";		
		public const string CTPSplitField_CountWord = CTPSplitField_Prefix+"CountWord";
		
		

		//public const string[] CARSTR_STOPWORDS_ENGLISH = new string[]{"hello"};

		static DataSet CreateSplitTableWordsSchema()
		{		
			DataSet dsResult=TEXCSDBUtil.CreateSimpleTable(CTPSplitDataSet,CTPSplitTable,new TStringTypeElement(CTPSplitField_Word, typeof(string)), new TStringTypeElement[] { new TStringTypeElement(CTPSplitField_CountWord, typeof(int))});
			dsResult.Tables[0].DefaultView.ApplyDefaultSort=true;
			return dsResult;
			/*DataTable dttbResult = new DataTable(CTPSplitTable);
			DataColumn pkCol = dttbResult.Columns.Add(CTPSplitField_Word, typeof(string));
			dttbResult.PrimaryKey = new DataColumn[] {pkCol};
			dttbResult.Columns.Add(CTPSplitField_CountWord, typeof(int));
			DataSet dsResult = new DataSet(CTPSplitDataSet);
			dsResult.Tables.Add(dttbResult);			
			return dsResult;*/
		}

		static enmrwd_ResultOfInsertWord InsertWord(DataSet dsTPSPDataSet_Words, string strWord) //true if new word, else false
		{
			strWord=strWord.Trim();
			if (strWord==string.Empty)
			{
				return enmrwd_ResultOfInsertWord.rwd_NoValidWord;
			}
			
			DataRow[] dtarRow = dsTPSPDataSet_Words.Tables[0].Select(CTPSplitField_Word+" = '"+strWord+"'");	
									
			if (dtarRow.Length>0)
			{
				dtarRow[0][CTPSplitField_CountWord]=Convert.ToInt32(dtarRow[0][CTPSplitField_CountWord])+1;
				return enmrwd_ResultOfInsertWord.rwd_WordExist;
			}
			else
			{
				dsTPSPDataSet_Words.Tables[0].Rows.Add(new object[]{strWord,1});				
				return enmrwd_ResultOfInsertWord.rwd_NewWord;
			}
		}

		static public char CapitalizeSmart(char chrToCapitalize, enmcpw_CapitalizeWay capWay)
		{
			//char chrResult=vhrToCapitalize;
			if ((capWay & enmcpw_CapitalizeWay.cpw_UseWindowsUpperFunction)!=0)
			{
				chrToCapitalize=chrToCapitalize.ToString().ToUpper()[0];
			}
			else
			{
				switch (chrToCapitalize)
				{
					case 'a' : 
						chrToCapitalize = 'A';
						break;
					case 'b' : 
						chrToCapitalize = 'B';
						break;
					case 'c' : 
						chrToCapitalize = 'C';
						break;
					case 'd' : 
						chrToCapitalize = 'D';
						break;
					case 'e' : 
						chrToCapitalize = 'E';
						break;
					case 'f' : 
						chrToCapitalize = 'F';
						break;
					case 'g' : 
						chrToCapitalize = 'G';
						break;
					case 'h' : 
						chrToCapitalize = 'H';
						break;
					case 'i' : 
						chrToCapitalize = 'I';
						break;
					case 'j' : 
						chrToCapitalize = 'J';
						break;
					case 'k' : 
						chrToCapitalize = 'K';
						break;
					case 'l' : 
						chrToCapitalize = 'L';
						break;
					case 'm' : 
						chrToCapitalize = 'M';
						break;
					case 'n' : 
						chrToCapitalize = 'N';
						break;
					case 'o' : 
						chrToCapitalize = 'O';
						break;
					case 'p' : 
						chrToCapitalize = 'P';
						break;
					case 'q' : 
						chrToCapitalize = 'Q';
						break;
					case 'r' : 
						chrToCapitalize = 'R';
						break;
					case 's' : 
						chrToCapitalize = 'S';
						break;
					case 't' : 
						chrToCapitalize = 'T';
						break;
					case 'u' : 
						chrToCapitalize = 'U';
						break;
					case 'v' : 
						chrToCapitalize = 'V';
						break;
					case 'w' : 
						chrToCapitalize = 'W';
						break;
					case 'x' : 
						chrToCapitalize = 'X';
						break;
					case 'Y' : 
						chrToCapitalize = 'Y';
						break;
					case 'z' : 
						chrToCapitalize = 'Z';
						break;					
//greek
					case 'á' : 
						chrToCapitalize = 'Á';
						break;
					case 'Ü' : 
						chrToCapitalize = '¢';
						break;
					case 'â' : 
						chrToCapitalize = 'Â';
						break;
					case 'ã' : 
						chrToCapitalize = 'Ã';
						break;
					case 'ä' : 
						chrToCapitalize = 'Ä';
						break;
					case 'å' : 
						chrToCapitalize = 'Å';
						break;
					case 'Ý' : 
						chrToCapitalize = '¸';
						break;				
					case 'æ' : 
						chrToCapitalize = 'Æ';
						break;
					case 'ç' : 
						chrToCapitalize = 'Ç';
						break;
					case 'Þ' : 
						chrToCapitalize = '¹';
						break;
					case 'è' : 
						chrToCapitalize = 'È';
						break;
					case 'é' : 
						chrToCapitalize = 'É';
						break;
					case 'ß' : 
						chrToCapitalize = 'º';
						break;				
					case 'ú' : 
						chrToCapitalize = 'Ú';
						break;		
					case 'À' : 
						chrToCapitalize = 'Ú';
						break;
					case 'ê' : 
						chrToCapitalize = 'Ê';
						break;
					case 'ë' : 
						chrToCapitalize = 'Ë';
						break;
					case 'ì' : 
						chrToCapitalize = 'Ì';
						break;
					case 'í' : 
						chrToCapitalize = 'Í';
						break;
					case 'î' : 
						chrToCapitalize = 'Î';
						break;
					case 'ï' : 
						chrToCapitalize = 'Ï';
						break;				
					case 'ü' : 
						chrToCapitalize = '¼';
						break;
					case 'ð' : 
						chrToCapitalize = 'Ð';
						break;
					case 'ñ' : 
						chrToCapitalize = 'Ñ';
						break;
					case 'ó' : 
						chrToCapitalize = 'Ó';
						break;
					case 'ò' : 
						chrToCapitalize = 'Ó';
						break;
					case 'ô' : 
						chrToCapitalize = 'Ô';
						break;
					case 'õ' : 
						chrToCapitalize = 'Õ';
						break;				
					case 'ý' : 
						chrToCapitalize = '¾';
						break;				
					case 'û' : 
						chrToCapitalize = 'Û';
						break;
					case 'à' : 
						chrToCapitalize = 'Û';
						break;
					case 'ö' : 
						chrToCapitalize = 'Ö';
						break;				
					case '÷' : 
						chrToCapitalize = '×';
						break;				
					case 'ø' : 
						chrToCapitalize = 'Ø';
						break;				
					case 'ù' : 
						chrToCapitalize = 'Ù';
						break;				
					case 'þ' : 
						chrToCapitalize = '¿';
						break;
				}

	
				if ((capWay & enmcpw_CapitalizeWay.cpw_KillTonosLeksis)!=0)
				{
					switch (chrToCapitalize)
					{						
						case '¢' : 
							chrToCapitalize = 'Á';
							break;
						case '¸' : 
							chrToCapitalize = 'Å';
							break;
						case '¹' : 
							chrToCapitalize = 'Ç';
							break;
						case 'º' : 
							chrToCapitalize = 'É';
							break;
						case 'Ú' : 
							chrToCapitalize = 'É';
							break;		
						case '¼' : 
							chrToCapitalize = 'Ï';
							break;				
						case '¾' : 
							chrToCapitalize = 'Õ';
							break;				
						case 'Û' : 
							chrToCapitalize = 'Õ';
							break;				
						case '¿' : 
							chrToCapitalize = 'Ù';
							break;				
					}
				}
				chrToCapitalize=chrToCapitalize.ToString().ToUpper()[0];
			}
			return chrToCapitalize;
		}

		static public string CapitalizeSmart(string strToCapitalize, enmcpw_CapitalizeWay capWay)
		{
			int intLength=strToCapitalize.Length;
			if (intLength>0)
			{				
				StringBuilder sbToCap = new StringBuilder(strToCapitalize);
				--intLength;
				for (int intCounter1=0;intCounter1<=intLength;intCounter1++)
				{
					sbToCap[intCounter1]=CapitalizeSmart(sbToCap[intCounter1], capWay);
				}
				return sbToCap.ToString();
			}
			else
			{
				return string.Empty;
			}
		}

		static public DataSet SplitString(string strInputText, enmsf_StrFilter sfTextFilter, int intMaxLengthOfWord)
		{
			DataSet dsResult = CreateSplitTableWordsSchema();			

			string strDelim = " ,=+.:-_/\\{}\"\'!@#$%^&*()[]|~`;<>?«»\t\n\r";
			char[] charrDelimiter = strDelim.ToCharArray();
			string[] strarrSplit = strInputText.Split(charrDelimiter);
			
			string strToInsert=string.Empty;
			foreach (string strWord in strarrSplit) 
			{
				strToInsert=strWord.Trim();
				if (strToInsert==string.Empty)
				{
					continue;
				}

				if ((enmsf_StrFilter.sf_CapitalizeSmart & sfTextFilter)!=0)
				{
					strToInsert=CapitalizeSmart(strToInsert,enmcpw_CapitalizeWay.cpw_KillTonosLeksis);
				}

				if ((enmsf_StrFilter.sf_DenyStopWords & sfTextFilter)!=0)
				{
					if (EXCSStrUtl.TextProcess.StopWords.TStopWords.ExistsInAllStopWords(strToInsert))
					{
						strToInsert=string.Empty;
					}
				}

				if ( (strToInsert!=string.Empty) && (strToInsert.Length<=intMaxLengthOfWord) )
				{
					InsertWord(dsResult,strToInsert);
				}
			}
			return dsResult;
		}
	}
}
