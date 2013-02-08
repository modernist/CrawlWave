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
		

		
		static public char[] C_CHARS_GREEK_CAPITAL = new char[] {'Α','Β','Γ','Δ','Ε','Ζ','Η','Θ',
																	'Ι','Κ','Λ','Μ','Ν','Ξ','Ο','Π',
																	'Ρ','Σ','Τ','Υ','Φ','Χ','Ψ','Ω',
																	'Ά','Έ','Ή','Ί','Ϊ','Ό','Ύ','Ϋ','Ώ'
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
					chResult= 'Α';
					break;
				case 'B' : 
					chResult= 'Β';
					break;
				case 'E' : 
					chResult= 'Ε';
					break;
				case 'H' : 
					chResult= 'Η';
					break;
				case 'I' : 
					chResult= 'Ι';
					break;
				case 'K' : 
					chResult= 'Κ';
					break;
				case 'M' : 
					chResult= 'Μ';
					break;
				case 'N' : 
					chResult= 'Ν';
					break;
				case 'O' : 
					chResult= 'Ο';
					break;
				case 'P' : 
					chResult= 'Ρ';
					break;
				case 'T' : 
					chResult= 'Τ';
					break;
				case 'Y' : 
					chResult= 'Υ';
					break;
				case 'Z' : 
					chResult= 'Ζ';
					break;
				case 'X' : 
					chResult= 'Χ';
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
				EXException.ThrowEXException("Πρόβλημα στην αποκωδικοποίηση του HEX αλφαριθμητικού.",exc);
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
				EXException.ThrowEXException("Πρόβλημα στην κωδικοποίηση σε HEX αλφαριθμητικό.",exc);
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
				if (((l1=='Σ') && (l2=='Ο')) ||
					((l1=='Σ') && (l2=='Η')) ||
					((l1=='Σ') && (l2=='Ε')) ||
					((l1=='Ν') && (l2=='Ω')) ||
					((l1=='Υ') && (l2=='Ο')) ||
					((l1=='Ι') && (l2=='Ο')) ||
					((l1=='Σ') && (l2=='Α')) ||
					((l1=='Σ') && (l2=='Ω')) ||
					((l1=='Ι') && (l2=='Α')) ||
					((l1=='Ι') && (l2=='Ε'))) 
					sbResult[l-2]='\0';
				else if ((l1=='Α') || (l1=='Η') || (l1=='Ο') ||
					(l1=='Ε') || (l1=='Ω') || (l1=='Ι')) 
					sbResult[l-1]='\0';
				else if (((l1=='Σ') && (l2=='Υ') && (l3=='Ο')) ||
					((l1=='Σ') && (l2=='Ι') && (l3=='Ε')) ||
					((l1=='Ν') && (l2=='Υ') && (l3=='Ο')))
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
				if ((l5=='Ο') && (l4=='Υ') && (l3=='Μ') && (l2=='Ε') && (l1=='Ν')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='Ε') && (l4=='Σ') && (l3=='Τ') && (l2=='Ε') && (l1=='Ρ')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='Ι') && (l4=='Σ') && (l3=='Μ') && (l2=='Ε') && (l1=='Ν')) 
				{
					sbResult[l-4]='Ζ';
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l5=='Α') && (l4=='Σ') && (l3=='Μ') && (l2=='Ε') && (l1=='Ν')) 
				{
					sbResult[l-4]='Ζ';
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l5=='Η') && (l4=='Σ') && (l3=='Μ') && (l2=='Ε') && (l1=='Ν')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='Ι') && (l4=='Σ') && (l3=='Τ') && (l2=='Ι') && (l1=='Κ')) 
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
				if (((l1=='Τ') && (l2=='Η') && (l3=='Τ') && (l4=='Ο')) ||
					((l1=='Δ') && (l2=='Ι') && (l3=='Τ') && (l4=='Ι')) ||
					((l1=='Κ') && (l2=='Ι') && (l3=='Τ') && (l4=='Α')) ||
					((l1=='Ρ') && (l2=='Ε') && (l3=='Τ') && (l4=='Ο')) ||
					((l1=='Ρ') && (l2=='Ε') && (l3=='Τ') && (l4=='Υ')) ||
					((l1=='Τ') && (l2=='Ν') && (l3=='Υ') && (l4=='Ο')) ||
					((l1=='Τ') && (l2=='Ε') && (l3=='Ν') && (l4=='Ι')) ||
					((l1=='Τ') && (l2=='Ε') && (l3=='Σ') && (l4=='Ε')) ||
					((l1=='Τ') && (l2=='Ε') && (l3=='Σ') && (l4=='Η'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if (((l4=='Ο') && (l3=='Τ') && (l2=='Ι') && (l1=='Κ')) ||
					((l4=='Η') && (l3=='Τ') && (l2=='Ι') && (l1=='Κ')) ||
					((l4=='Α') && (l3=='Τ') && (l2=='Ι') && (l1=='Ν'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if (((l4=='Ι') && (l3=='Ζ') && (l2=='Ε') && (l1=='Τ')) ||
					((l4=='Η') && (l3=='Μ') && (l2=='Α') && (l1=='Τ'))) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if (((l4=='Α') && (l3=='Ζ') && (l2=='Ε') && (l1=='Τ')) ||
					((l4=='Α') && (l3=='Σ') && (l2=='Ε') && (l1=='Τ'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='Ι') && (l3=='Α') && (l2=='Σ') && (l1=='Μ')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l1=='Τ') && (l2=='Ε') && (l3=='Σ') && (l4=='Ι')) 
				{
					sbResult[l-4]='Ι';
					sbResult[l-3]='Ζ';
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l4=='Ω') && (l3=='Μ') && (l2=='Ε') && (l1=='Ν'))
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
				if ((l4=='Η') && (l3=='Μ') && (l2=='Ε') && (l1=='Ν')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='Ο') && (l3=='Μ') && (l2=='Ε') && (l1=='Ν')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='Α') && (l3=='Μ') && (l2=='Ε') && (l1=='Ν') && (l-4 > 3)) 
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
				if (((l1=='Κ') && (l2=='Α') && (l3=='Ι')) ||
					((l1=='Δ') && (l2=='Ω') && (l3=='Ι')) ||
					((l1=='Τ') && (l2=='Ν') && (l3=='Ο'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='Ο') && (l2=='Υ') && (l1=='Μ') && (l-3 > 3)) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='Ε') && (l2=='Ι') && (l1=='Τ')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='Ι') && (l2=='Σ') && (l1=='Μ')) 
				{
					sbResult[l-2]='Ζ';
					sbResult[l-1]='\0';
					l-=1;
				}
				if ((l3=='Α') && (l2=='Σ') && (l1=='Μ')) 
				{
					sbResult[l-2]='Ζ';
					sbResult[l-1]='\0';
					l-=1;
				}
				if ((l3=='Ω') && (l2=='Ν') && (l1=='Τ')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='Ι') && (l2=='Α') && (l1=='Σ')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if (((l3=='Α') && (l2=='Σ') && (l1=='Τ')) ||
					((l3=='Α') && (l2=='Σ') && (l1=='Θ'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if (((l1=='Θ') && (l2=='Σ') && (l3=='Ι')) ||
					((l1=='Τ') && (l2=='Σ') && (l3=='Ι'))) 
				{
					sbResult[l-3]='Ι';
					sbResult[l-2]='Ζ';
					sbResult[l-1]='\0';
					l-=1;
				}
				if (((l3=='Ε') && (l2=='Σ')) && ((l1=='Τ') || (l1=='Θ'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((((l3=='Ω') && (l2=='Σ')) && ((l1=='Τ') || (l1=='Θ'))) && (l-3>3)) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='Ι') && (l2=='Δ') && (l1=='Ι')) 
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
				if ((l2=='Α') && (l1=='Τ') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Ι') && (l1=='Δ')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Η') && (l1=='Σ')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Η') && (l1=='Θ')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}



				if ((l1=='Σ') && (l2=='Ι') && (l-2 > 3))  sbResult[l-1]='Ζ'; 

				if ((l2=='Ε') && (l1=='Τ')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Ι') && (l1=='Κ') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Ε') && (l1=='Ι')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Ι') && (l1=='Ν') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Ω') && ((l1=='Σ') || (l1=='Θ'))) sbResult[l-1]='Ν';
				if ((l2=='Α') && (l1=='Θ') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Α') && (l1=='Σ') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
			}


			if (l > 3) 
			{
				l1=sbResult[l-1];
				if ((l1=='Ι') || (l1=='Ε')) 
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
						EXException.ThrowEXException("Διπλό λεκτικό σε πίνακα. EX-{F79B34BB-DC2A-4844-B6ED-8FDBEBE72F6D}", exc);
					}
				}
			}
			return slResult;
		}

	}

}
