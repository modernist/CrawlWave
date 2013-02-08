using System;
using System.Text;
using EXMath;
using EXCSCommon;
using System.Text.RegularExpressions;
using  System.Collections;


namespace EXCSStrUtl
{
	/// <summary>
	/// Summary description for EXCSStrUtl.
	/// </summary>
	
	public class TEXStrUtl
	{
		//		static public string CleverToString(bool blnCondition, decimal dcmToBeStringed, string strDefaultValue,  string strFormat)
		//		{
		//			if (blnCondition)
		//			{
		//				return dcmToBeStringed.ToString(strFormat);
		//			}
		//			else
		//			{
		//				return strDefaultValue;
		//			}
		//		}
		//
		//		static public string CleverToString(bool blnCondition, int intToBeStringed, string strDefaultValue,  string strFormat)
		//		{
		//			if (blnCondition)
		//			{
		//				return dcmToBeStringed.ToString(strFormat);
		//			}
		//			else
		//			{
		//				return strDefaultValue;
		//			}
		//		}
		static System.Text.RegularExpressions.Regex  C_REGEX_ForStripBlanks = new Regex("<[^>]*>", RegexOptions.IgnoreCase|RegexOptions.Compiled);
		static System.Text.RegularExpressions.Regex  C_REGEX_ForStripBlanksPrep1 = new Regex("<(select|option|script|style|title)(.*?)>((.|\n)*?)</(select|option|script|style|title)>", RegexOptions.IgnoreCase|RegexOptions.Compiled);
		static System.Text.RegularExpressions.Regex  C_REGEX_ForStripBlanksPrep2 = new Regex("&(nbsp|quot|copy);", RegexOptions.IgnoreCase|RegexOptions.Compiled);
		

		
		static public char[] C_CHARS_GREEK_CAPITAL = new char[] {'�','�','�','�','�','�','�','�',
																	'�','�','�','�','�','�','�','�',
																	'�','�','�','�','�','�','�','�',
																	'�','�','�','�','�','�','�','�','�'
																};
		static public char[] C_CHARS_NUMBER = new char[] {'0','1','2','3','4','5','6','7','8','9'};

		static public char[] C_CHARS_PUNCTUATION = new char[] {'~','`','!','@','#','$','%','^','&','(',')','-','_','+','=','|','\\','}',']','{','[','\'','\"','?','/','>','.','<',','};

		static public bool ExistsInCharArray(char[] charrArrToCheck, char chToFind)
		{
			System.Collections.IEnumerator enumerator=charrArrToCheck.GetEnumerator();
			while (enumerator.MoveNext()) 
			{				
				if ((char)(enumerator.Current)==chToFind)
				{
					return true;
				}
			}
			return false;
		}

		//static public CheckAllCharsInArray(string strToBeChecked, char[] charrArrToCheck)
		static public int FindOneOfCharsInArray(string strToBeChecked, char[] charrArrToCheck) //-1 on failure
		{			
			int intLen=strToBeChecked.Length-1;
			for (int intCounter1=0;intCounter1<=intLen; intCounter1++)
			{	
				if (Array.LastIndexOf (charrArrToCheck,strToBeChecked[intCounter1])>-1)
					//if (ExistsInCharArray(charrArrToCheck,strToBeChecked[intCounter1]))
				{
					return intCounter1;
				}
			}
			return -1;
		}

		static public bool CheckExistsAllChars(string strToBeChecked, char[][] charrArrToCheck) //-1 on failure
		{
			char[] charrSimpleCheck = TEXCSGenUtl.ArrayOfArrayCharToArrayChar(charrArrToCheck);
			return CheckExistsAllChars(strToBeChecked, charrSimpleCheck);			
		}

		static public bool CheckExistsAllChars(string strToBeChecked, char[] charrArrToCheck) //-1 on failure
		{			
			int intLen=strToBeChecked.Length-1;			

			for (int intCounter1=0;intCounter1<=intLen; intCounter1++)
			{				
				if (!ExistsInCharArray(charrArrToCheck,strToBeChecked[intCounter1]))
				{
					return false;
				}
			}
			return true;
		}

		static public bool IsGreekCapital(string strCheck, bool blnAllowExtraChars)
		{			
			//return (CheckExistsAllChars(strCheck,new char[][]{C_CHARS_GREEK_CAPITAL,C_CHARS_NUMBERS}));
			if (blnAllowExtraChars)
			{
				return (CheckExistsAllChars(strCheck,new char[][]{C_CHARS_GREEK_CAPITAL,C_CHARS_NUMBER,C_CHARS_PUNCTUATION}));
			}
			else
			{
				return (CheckExistsAllChars(strCheck,C_CHARS_GREEK_CAPITAL));
			}
		}
		
		static public string ToEquc(string strToBeEgualized)
		{
			StringBuilder sbResult = new StringBuilder(strToBeEgualized);
			int intLen=sbResult.Length-1;
			for (int intCounter1=0;intCounter1<=intLen; intCounter1++)
			{
				sbResult[intCounter1]=ToEquc(sbResult[intCounter1]);	
			}
			return sbResult.ToString();
		}

		static public char ToEquc(char chToBeEgualized)
		{
			char chResult = chToBeEgualized;
			switch (chToBeEgualized)
			{
				case 'A' : 
					chResult= '�';
					break;
				case 'B' : 
					chResult= '�';
					break;
				case 'E' : 
					chResult= '�';
					break;
				case 'H' : 
					chResult= '�';
					break;
				case 'I' : 
					chResult= '�';
					break;
				case 'K' : 
					chResult= '�';
					break;
				case 'M' : 
					chResult= '�';
					break;
				case 'N' : 
					chResult= '�';
					break;
				case 'O' : 
					chResult= '�';
					break;
				case 'P' : 
					chResult= '�';
					break;
				case 'T' : 
					chResult= '�';
					break;
				case 'Y' : 
					chResult= '�';
					break;
				case 'Z' : 
					chResult= '�';
					break;
				case 'X' : 
					chResult= '�';
					break;
			}
			return chResult;
		}

		static public int ConvertToInt32(string strInput, int intDefault)
		{
			try
			{
				if (strInput==string.Empty)
				{
					return intDefault;
				}
				return Convert.ToInt32(strInput);
			}
			catch
			{
				return intDefault;
			}
		}

		static public string DecodeHexStrToStr(string strHex)
		{
			if (strHex==string.Empty)
			{
				return string.Empty;
			}
			try
			{
				int intNum1=strHex.Length;
				EXException.CheckEXError((intNum1%4)==0,"EX {30CF8C85-C636-4f3e-B97E-3D3E02B433FE}");
				StringBuilder sbResult = new StringBuilder(new string(' ',intNum1 / 4));
				//sbResult.Length=intNum1*4;
				//sbResult.EnsureCapacity(intNum1*4);
				
				intNum1--;

				for (int intCounter1=0;intCounter1<=intNum1;intCounter1=intCounter1+4)
				{
					string chStringChar1=new string(strHex[intCounter1],1);
					string chStringChar2=new string(strHex[intCounter1+1],1);
					string chStringChar3=new string(strHex[intCounter1+2],1);
					string chStringChar4=new string(strHex[intCounter1+3],1);

					//byte[] btarrArray = new byte[];
									
					byte bt1 = Convert.ToByte(chStringChar1,16);
					byte bt2 = Convert.ToByte(chStringChar2,16);
					byte bt3 = Convert.ToByte(chStringChar3,16);
					byte bt4 = Convert.ToByte(chStringChar4,16);
					
					UInt32 uintChar = 0;
					TEXCSMath.FourBytesToUInt32(bt1, bt2, bt3, bt4, out uintChar);
					
					sbResult[intCounter1/4]=(char)uintChar;				
					//chStringChar.
				}

				return sbResult.ToString();
			}
			catch(Exception exc)
			{
				EXException.ThrowEXException("�������� ���� ��������������� ��� HEX ��������������.",exc);
				return string.Empty;
			}
		}
		
		static public string EncodeToHexStr(string strToHex)
		{
			//char[] charrToHex = strToHex.ToCharArray();
			
			if (strToHex==string.Empty)
			{
				return string.Empty;
			}
			try
			{
				int intNum1=strToHex.Length;
				StringBuilder sbResult = new StringBuilder(strToHex+strToHex+strToHex+strToHex);
				//sbResult.Length=intNum1*4;
				//sbResult.EnsureCapacity(intNum1*4);
				
				intNum1--;

				for (int intCounter1=0;intCounter1<=intNum1;intCounter1++)
				{
					char chStringChar=strToHex[intCounter1];
					//byte[] btarrArray = new byte[];
					UInt16  uintChar=(UInt16)chStringChar;
					
					byte bt1 = 0;
					byte bt2 = 0;
					byte bt3 = 0;
					byte bt4 = 0;
					
					TEXCSMath.UInt32ToFourBytes(uintChar, out bt1, out bt2, out bt3, out bt4);
					

					string strChar1=bt1.ToString("X");
					string strChar2=bt2.ToString("X");
					string strChar3=bt3.ToString("X");
					string strChar4=bt4.ToString("X");
					
					sbResult[intCounter1*4]=strChar1[0];
					sbResult[(intCounter1*4)+1]=strChar2[0];
					sbResult[(intCounter1*4)+2]=strChar3[0];
					sbResult[(intCounter1*4)+3]=strChar4[0];
					
					//chStringChar.
				}

				return sbResult.ToString();
			}
			catch(Exception exc)
			{
				EXException.ThrowEXException("�������� ���� ������������ �� HEX �������������.",exc);
				return string.Empty;
			}

		}
		
		static public bool ExistsInChrArr(char chrToSearch, char[] chrarrList)
		{
			foreach (char _chr in chrarrList)
			{
				if (_chr==chrToSearch)
				{
					return true;
				}
			}
			return false;
		}

		static public string ExtractWord(int intNumberOfWord, string strInput, char[] chrarrList)
		{
			try
			{
				string [] strarrSplit = null;
				strarrSplit = strInput.Split(chrarrList);
				if (intNumberOfWord-1>strarrSplit.Length)
				{
					return string.Empty;
				}
				else
				{
					return strarrSplit[intNumberOfWord-1];
				}
			}
			catch
			{
				return string.Empty;
			}
		}

		static public int WordCount(string strInput, char[] chrarrList)
		{
			string [] strarrSplit = null;
			strarrSplit = strInput.Split(chrarrList);
			return strarrSplit.Length;			
		}


		static public string _OLDExtractWord(int intNumberOfWord, string strInput, char[] chrarrList)
		{
			int intCounter1=0,intFinalLen=0;
			int intLen=strInput.Length-1;
			StringBuilder strResult = new StringBuilder(strInput); // kanonika einai Result:=StringOfChar(' ',SLen);
	
			intCounter1=WordPosition(intNumberOfWord,strInput,chrarrList);  
			if (intCounter1>=0)
			{
				//find the end of the current word}
				while	(intCounter1 <= intLen)						 
				{
					if (!ExistsInChrArr(strInput[intCounter1], chrarrList))
					{
						//add the intCounter1'th character to result
						++intFinalLen;
						
						strResult[intFinalLen-1]=strInput[intCounter1];
						++intCounter1;
					}
					else
					{
						break;
					}
				}		  

			}
			return strResult.ToString().Substring(0,intFinalLen);
		}


		static public int WordPosition(int intNumberOfWord, string strInput, char[] chrarrList)
		{
			int intCounter1=0,intNum1=0, intWordPosition=-1;
			int intLen=strInput.Length-1;	
	
			while ((intCounter1 <= intLen) & (intNum1!=intNumberOfWord)) 
			{
				//skip over delimiters
				while	(intCounter1 <= intLen)  						
				{
					if (ExistsInChrArr(strInput[intCounter1], chrarrList)) 
					{
						++intCounter1;
					}
					else
					{
						break;
					}
				}
				//if we're not beyond end of S, we're at the start of a word
				if (intCounter1 <= intLen)
				{
					++intNum1;	
				}
				//if not finished, find the end of the current word
				if (intNum1!=intNumberOfWord)
				{
					while	(intCounter1 <= intLen)						 
					{
						if (!ExistsInChrArr(strInput[intCounter1], chrarrList))
						{
							++intCounter1;
						}
						else
						{
							break;
						}
					}		  
				}
				else
				{
					intWordPosition=intCounter1;
				}
				
			}
			return intWordPosition;
		}



		static public int _OLDWordCount(string strInput, char[] chrarrList)
		{
			int intCounter1=0,intNum1=0;
			int intLen=strInput.Length-1;	
	
			while (intCounter1 <= intLen)  
			{
				//skip over delimiters
				while	(intCounter1 <= intLen)  						
				{
					if (ExistsInChrArr(strInput[intCounter1], chrarrList)) 
					{
						++intCounter1;
					}
					else
					{
						break;
					}
				}
				//if we're not beyond end of S, we're at the start of a word
				if (intCounter1 <= intLen)
				{
					++intNum1;	
				}
				//find the end of the current word
				while	(intCounter1 <= intLen)						 
				{
					if (!ExistsInChrArr(strInput[intCounter1], chrarrList))
					{
						++intCounter1;
					}
					else
					{
						break;
					}
				}		  

				
			}
			return intNum1;
		}


		public static string NulltoString(string strInput)
		{
			try
			{
				return (strInput==null?string.Empty:strInput);
			}
			catch
			{
				return string.Empty;
			}
		}
		
		public static string StripTagsFromHTML(string strHTML)
		{															// Removes tags from passed HTML
			//---
			strHTML=C_REGEX_ForStripBlanksPrep1.Replace(strHTML,string.Empty);										
			strHTML=C_REGEX_ForStripBlanksPrep2.Replace(strHTML,string.Empty);					
			//---
			return C_REGEX_ForStripBlanks.Replace(  strHTML,  string.Empty);										
		}

		

		public static bool IsEmail(string strInputEmail) // http://www.codeproject.com/useritems/Valid_Email_Addresses.asp
		{
			strInputEmail  = NulltoString(strInputEmail);
			string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
			Regex re = new Regex(strRegex);
			if (re.IsMatch(strInputEmail))
			{
				return (true);
			}
			else
			{
				return (false);
			}
		}
	
		static public string GreekStem(string word)
		{		
			int l;
			l=word.Length;
			if (l<=3)
			{
				return word;
			}
			
			char         l1,l2,l3,l4,l5;

			StringBuilder sbResult = new StringBuilder(word);
			l1=sbResult[l-1]; 
			l2=sbResult[l-2];  
			l3=sbResult[l-3];

			/* ----------------------- level 1  ------------------------- */
			if (l>3) 
			{
				if (((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�')) ||
					((l1=='�') && (l2=='�'))) 
					sbResult[l-2]='\0';
				else if ((l1=='�') || (l1=='�') || (l1=='�') ||
					(l1=='�') || (l1=='�') || (l1=='�')) 
					sbResult[l-1]='\0';
				else if (((l1=='�') && (l2=='�') && (l3=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�')))
					sbResult[l-3]='\0';
			}
			/* --------------------- level 2 ------------------------- */

			l=sbResult.Length;

			if (l > 7) 
			{
				l1=sbResult[l-1];
				l2=sbResult[l-2];  
				l3=sbResult[l-3];  
				l4=sbResult[l-4];
				l5=sbResult[l-5];
				if ((l5=='�') && (l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='�') && (l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='�') && (l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-4]='�';
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l5=='�') && (l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-4]='�';
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l5=='�') && (l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='�') && (l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
			}

			if (l > 6) 
			{
				l1=sbResult[l-1];
				l2=sbResult[l-2];
				l3=sbResult[l-3];
				l4=sbResult[l-4];
				if (((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if (((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) ||
					((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) ||
					((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if (((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) ||
					((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�'))) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if (((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) ||
					((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l1=='�') && (l2=='�') && (l3=='�') && (l4=='�')) 
				{
					sbResult[l-4]='�';
					sbResult[l-3]='�';
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�'))
					if ((l-4)>3) 
					{
						sbResult[l-4]='\0';
						l-=4;
					}
					else 
					{
						sbResult[l-3]='\0';
						l-=3;
					}
				if ((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='�') && (l3=='�') && (l2=='�') && (l1=='�') && (l-4 > 3)) 
				{ 
					sbResult[l-4]='\0';
					l-=4;
				}
			}

			if (l > 5) 
			{
				l1=sbResult[l-1];
				l2=sbResult[l-2];
				l3=sbResult[l-3];
				if (((l1=='�') && (l2=='�') && (l3=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='�') && (l2=='�') && (l1=='�') && (l-3 > 3)) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-2]='�';
					sbResult[l-1]='\0';
					l-=1;
				}
				if ((l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-2]='�';
					sbResult[l-1]='\0';
					l-=1;
				}
				if ((l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='�') && (l2=='�') && (l1=='�')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if (((l3=='�') && (l2=='�') && (l1=='�')) ||
					((l3=='�') && (l2=='�') && (l1=='�'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if (((l1=='�') && (l2=='�') && (l3=='�')) ||
					((l1=='�') && (l2=='�') && (l3=='�'))) 
				{
					sbResult[l-3]='�';
					sbResult[l-2]='�';
					sbResult[l-1]='\0';
					l-=1;
				}
				if (((l3=='�') && (l2=='�')) && ((l1=='�') || (l1=='�'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((((l3=='�') && (l2=='�')) && ((l1=='�') || (l1=='�'))) && (l-3>3)) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='�') && (l2=='�') && (l1=='�')) 
				{
					l-=3;
					if (l > 3) sbResult[l-3]='\0';
					else l+=3;
				}
			}

			if (l > 4) 
			{
				l1=sbResult[l-1]; 
				l2=sbResult[l-2];
				if ((l2=='�') && (l1=='�') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='�') && (l1=='�')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='�') && (l1=='�')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='�') && (l1=='�')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}



				if ((l1=='�') && (l2=='�') && (l-2 > 3))  sbResult[l-1]='�'; 

				if ((l2=='�') && (l1=='�')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='�') && (l1=='�') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='�') && (l1=='�')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='�') && (l1=='�') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='�') && ((l1=='�') || (l1=='�'))) sbResult[l-1]='�';
				if ((l2=='�') && (l1=='�') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='�') && (l1=='�') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
			}


			if (l > 3) 
			{
				l1=sbResult[l-1];
				if ((l1=='�') || (l1=='�')) 
				{
					sbResult[l-1]='\0';
					l-=1;
				}
			}
			
			int intIndexZero=0;
			string strResult = sbResult.ToString();

			intIndexZero = strResult.IndexOf("\0");
			if (intIndexZero>0)
			{
				return strResult.Substring(0,intIndexZero);
			}
			else
			{
				return strResult;
			}
		}

		static public int CountSubStringInString(string strSubString, string strTargetToSearch)
		{			
			int intIndex = strTargetToSearch.IndexOf(strSubString);
			if (intIndex<0)
			{
				return 0;
			}		
			
			int intSubStringLength = strSubString.Length;
			int intStart = intIndex+intSubStringLength;
			int intResult = 1;

			while ((intIndex=strTargetToSearch.IndexOf(strSubString,intStart))>=0)
			{
				intResult++;
				intStart = intIndex+intSubStringLength;
			}
			return intResult;
		}

		static public SortedList InsertFromStrArray(string[] strarrInputArray, bool blnIgnoreOnDuplicate)		
		{
			SortedList slResult = new SortedList();
			int intStrArrLength = strarrInputArray.Length-1;
			for (int intCounter1=0;intCounter1<= intStrArrLength; intCounter1++)
			{	
				try
				{
					slResult.Add(strarrInputArray[intCounter1],null);
				}
				catch (Exception exc)
				{
					if (!blnIgnoreOnDuplicate)
					{
						EXException.ThrowEXException("����� ������� �� ������. EX-{F79B34BB-DC2A-4844-B6ED-8FDBEBE72F6D}", exc);
					}
				}
			}
			return slResult;
		}

	}

}
