using System;
using System.Text;

namespace CrawlWave.ServerPlugins.WordExtraction
{
	/// <summary>
	/// IStemming defines an interface for word stemmers for different languages.
	/// </summary>
	public interface IStemming
	{
		/// <summary>
		/// Returns the stemmed form of a word
		/// </summary>
		/// <param name="word">The word to stem. It must be capitalized</param>
		/// <returns>The stemmed word</returns>
		string StemWord(string word);
	}

	/// <summary>
	/// Performs stemming for English words using Porter's Algorithm.
	/// </summary>
	internal class EnglishStemming: IStemming
	{
		private int i, i_end, j, k;
		private char []b;

		/// <summary>
		/// Construncts a new instance of the <see cref="EnglishStemming"/> class.
		/// </summary>
		public EnglishStemming()
		{
			b=new char[0];
			i=0;
			i_end=0;
		}

		/// <summary>
		/// Returns the stem of an english word
		/// </summary>
		/// <param name="word">The word to stem. It must be capitalized.</param>
		/// <returns>The stem of the word</returns>
		public string StemWord(string word)
		{
			i=word.Length;
			b=word.ToCharArray();
			i_end=0;
			k = i - 1;
			if (k > 1) 
			{
				Step1();
				Step2();
				Step3();
				Step4();
				Step5();
				Step6();
			}
			i_end = k+1;
			i = 0;
			return new string(b, 0, i_end);
		}

		private bool IsConsonant(int i)
		{
			switch(b[i])
			{
				case 'A': case 'E': case 'I': case 'O': case 'U':
					return false;
				case 'Y':
					return (i==0)?true:IsConsonant(i-1);
				default:
					return true;
			}
		}

		private int M()
		{
			int n = 0, i = 0;
			while(true) 
			{
				if (i > j) return n;
				if (! IsConsonant(i)) break; i++;
			}
			i++;
			while(true) 
			{
				while(true) 
				{
					if (i > j) return n;
					if (IsConsonant(i)) break;
					i++;
				}
				i++;
				n++;
				while(true) 
				{
					if (i > j) return n;
					if (! IsConsonant(i)) break;
					i++;
				}
				i++;
			}
		}

		private bool ContainsVowel()
		{
			for(int pos=0; pos<=j; pos++)
			{
				if(!IsConsonant(pos))
				{
					return true;
				}
			}
			return false;
		}

		private bool DoubleConsonant(int pos)
		{
			if(pos<1)
			{
				return false;
			}
			if(b[pos]!=b[pos-1])
			{
				return false;
			}
			return IsConsonant(pos);
		}

		private bool CVC(int pos)
		{
			if (pos < 2 || !IsConsonant(pos) || IsConsonant(pos-1) || !IsConsonant(pos-2))
			{
				return false;
			}
			int ch = b[i];
			if (ch == 'W' || ch == 'X' || ch == 'Y')
			{
				return false;
			}
			return true;
		}

		private bool EndsWith(string s)
		{
			int l = s.Length;
			int o = k-l+1;
			if (o < 0)
				return false;
			for (int i = 0; i < l; i++)
				if (b[o+i] != s[i])
					return false;
			j = k-l;
			return true;
		}

		private void SetTo(string s)
		{
			int l = s.Length;
			int o = j+1;
			for (int i = 0; i < l; i++)
				b[o+i] = s[i];
			k = j+l;
		}

		private void R(string s)
		{
			if(M()>0)
			{
				SetTo(s);
			}
		}

		private void Step1()
		{
			if (b[k] == 'S') 
			{
				if (EndsWith("SSES"))
					k -= 2;
				else if (EndsWith("IES"))
					SetTo("I");
				else if (b[k-1] != 'S')
					k--;
			}
			if (EndsWith("EED")) 
			{
				if (M() > 0)
					k--;
			} 
			else if ((EndsWith("ED") || EndsWith("ING")) && ContainsVowel()) 
			{
				k = j;
				if (EndsWith("AT"))
					SetTo("ATE");
				else if (EndsWith("BL"))
					SetTo("BLE");
				else if (EndsWith("IZ"))
					SetTo("IZE");
				else if (DoubleConsonant(k)) 
				{
					k--;
					int ch = b[k];
					if (ch == 'L' || ch == 'S' || ch == 'Z')
						k++;
				}
				else if (M() == 1 && CVC(k)) SetTo("E");
			}
		}

		private void Step2()
		{
			if (EndsWith("Y") && ContainsVowel())
				b[k] = 'I';
		}

		private void Step3()
		{
			if (k == 0)
				return;
			
			/* For Bug 1 */
			switch (b[k-1]) 
			{
				case 'A':
					if (EndsWith("ATIONAL")) { R("ATE"); break; }
					if (EndsWith("TIONAL")) { R("TION"); break; }
					break;
				case 'C':
					if (EndsWith("ENCI")) { R("ENCE"); break; }
					if (EndsWith("ANCI")) { R("ANCE"); break; }
					break;
				case 'E':
					if (EndsWith("IZER")) { R("IZE"); break; }
					break;
				case 'L':
					if (EndsWith("BLI")) { R("BLE"); break; }
					if (EndsWith("ALLI")) { R("AL"); break; }
					if (EndsWith("ENTLI")) { R("ENT"); break; }
					if (EndsWith("ELI")) { R("E"); break; }
					if (EndsWith("OUSLI")) { R("OUS"); break; }
					break;
				case 'O':
					if (EndsWith("IZATION")) { R("IZE"); break; }
					if (EndsWith("ATION")) { R("ATE"); break; }
					if (EndsWith("ATOR")) { R("ATE"); break; }
					break;
				case 'S':
					if (EndsWith("ALISM")) { R("AL"); break; }
					if (EndsWith("IVENESS")) { R("IVE"); break; }
					if (EndsWith("FULNESS")) { R("FUL"); break; }
					if (EndsWith("OUSNESS")) { R("OUS"); break; }
					break;
				case 'T':
					if (EndsWith("ALITI")) { R("AL"); break; }
					if (EndsWith("IVITI")) { R("IVE"); break; }
					if (EndsWith("BILITI")) { R("BLE"); break; }
					break;
				case 'G':
					if (EndsWith("LOGI")) { R("LOG"); break; }
					break;
				default :
					break;
			}
		}

		private void Step4()
		{
			switch (b[k]) 
			{
				case 'E':
					if (EndsWith("ICATE")) { R("IC"); break; }
					if (EndsWith("ATIVE")) { R(""); break; }
					if (EndsWith("ALIZE")) { R("AL"); break; }
					break;
				case 'I':
					if (EndsWith("ICITI")) { R("IC"); break; }
					break;
				case 'L':
					if (EndsWith("ICAL")) { R("IC"); break; }
					if (EndsWith("FUL")) { R(""); break; }
					break;
				case 'S':
					if (EndsWith("NESS")) { R(""); break; }
					break;
			}
		}

		private void Step5()
		{
			if (k == 0)
				return;

			/* for Bug 1 */
			switch ( b[k-1] ) 
			{
				case 'A':
					if (EndsWith("AL")) break; return;
				case 'C':
					if (EndsWith("ANCE")) break;
					if (EndsWith("ENCE")) break; return;
				case 'E':
					if (EndsWith("ER")) break; return;
				case 'I':
					if (EndsWith("IC")) break; return;
				case 'L':
					if (EndsWith("ABLE")) break;
					if (EndsWith("IBLE")) break; return;
				case 'N':
					if (EndsWith("ANT")) break;
					if (EndsWith("EMENT")) break;
					if (EndsWith("MENT")) break;
					if (EndsWith("ENT")) break; return;
				case 'O':
					if (EndsWith("ION") && j >= 0 && (b[j] == 'S' || b[j] == 'T')) break;
					if (EndsWith("OU")) break; return;
				case 'S':
					if (EndsWith("ISM")) break; return;
				case 'T':
					if (EndsWith("ATE")) break;
					if (EndsWith("ITI")) break; return;
				case 'U':
					if (EndsWith("OUS")) break; return;
				case 'V':
					if (EndsWith("IVE")) break; return;
				case 'Z':
					if (EndsWith("IZE")) break; return;
				default:
					return;
			}
			if (M() > 1)
				k = j;
		}

		private void Step6()
		{
			j = k;
			if (b[k] == 'E') 
			{
				int a = M();
				if (a > 1 || a == 1 && !CVC(k-1))
					k--;
			}
			if (b[k] == 'L' && DoubleConsonant(k) && M() > 1)
				k--;
		}
	}

	/// <summary>
	/// Implements a stemming algorithm for greek words
	/// </summary>
	internal class GreekStemming: IStemming
	{
		/// <summary>
		/// Performs stemming on a greek word
		/// </summary>
		/// <param name="word">The word to stem</param>
		/// <returns>The stemmed word</returns>
		public string StemWord(string word)
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
				if (((l1=='Ó') && (l2=='Ï')) ||
					((l1=='Ó') && (l2=='Ç')) ||
					((l1=='Ó') && (l2=='Å')) ||
					((l1=='Í') && (l2=='Ù')) ||
					((l1=='Õ') && (l2=='Ï')) ||
					((l1=='É') && (l2=='Ï')) ||
					((l1=='Ó') && (l2=='Á')) ||
					((l1=='Ó') && (l2=='Ù')) ||
					((l1=='É') && (l2=='Á')) ||
					((l1=='É') && (l2=='Å'))) 
					sbResult[l-2]='\0';
				else if ((l1=='Á') || (l1=='Ç') || (l1=='Ï') ||
					(l1=='Å') || (l1=='Ù') || (l1=='É')) 
					sbResult[l-1]='\0';
				else if (((l1=='Ó') && (l2=='Õ') && (l3=='Ï')) ||
					((l1=='Ó') && (l2=='É') && (l3=='Å')) ||
					((l1=='Í') && (l2=='Õ') && (l3=='Ï')))
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
				if ((l5=='Ï') && (l4=='Õ') && (l3=='Ì') && (l2=='Å') && (l1=='Í')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='Å') && (l4=='Ó') && (l3=='Ô') && (l2=='Å') && (l1=='Ñ')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='É') && (l4=='Ó') && (l3=='Ì') && (l2=='Å') && (l1=='Í')) 
				{
					sbResult[l-4]='Æ';
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l5=='Á') && (l4=='Ó') && (l3=='Ì') && (l2=='Å') && (l1=='Í')) 
				{
					sbResult[l-4]='Æ';
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l5=='Ç') && (l4=='Ó') && (l3=='Ì') && (l2=='Å') && (l1=='Í')) 
				{
					sbResult[l-5]='\0';
					l-=5;
				}
				if ((l5=='É') && (l4=='Ó') && (l3=='Ô') && (l2=='É') && (l1=='Ê')) 
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
				if (((l1=='Ô') && (l2=='Ç') && (l3=='Ô') && (l4=='Ï')) ||
					((l1=='Ä') && (l2=='É') && (l3=='Ô') && (l4=='É')) ||
					((l1=='Ê') && (l2=='É') && (l3=='Ô') && (l4=='Á')) ||
					((l1=='Ñ') && (l2=='Å') && (l3=='Ô') && (l4=='Ï')) ||
					((l1=='Ñ') && (l2=='Å') && (l3=='Ô') && (l4=='Õ')) ||
					((l1=='Ô') && (l2=='Í') && (l3=='Õ') && (l4=='Ï')) ||
					((l1=='Ô') && (l2=='Å') && (l3=='Í') && (l4=='É')) ||
					((l1=='Ô') && (l2=='Å') && (l3=='Ó') && (l4=='Å')) ||
					((l1=='Ô') && (l2=='Å') && (l3=='Ó') && (l4=='Ç'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if (((l4=='Ï') && (l3=='Ô') && (l2=='É') && (l1=='Ê')) ||
					((l4=='Ç') && (l3=='Ô') && (l2=='É') && (l1=='Ê')) ||
					((l4=='Á') && (l3=='Ô') && (l2=='É') && (l1=='Í'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if (((l4=='É') && (l3=='Æ') && (l2=='Å') && (l1=='Ô')) ||
					((l4=='Ç') && (l3=='Ì') && (l2=='Á') && (l1=='Ô'))) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if (((l4=='Á') && (l3=='Æ') && (l2=='Å') && (l1=='Ô')) ||
					((l4=='Á') && (l3=='Ó') && (l2=='Å') && (l1=='Ô'))) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='É') && (l3=='Á') && (l2=='Ó') && (l1=='Ì')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l1=='Ô') && (l2=='Å') && (l3=='Ó') && (l4=='É')) 
				{
					sbResult[l-4]='É';
					sbResult[l-3]='Æ';
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l4=='Ù') && (l3=='Ì') && (l2=='Å') && (l1=='Í'))
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
				if ((l4=='Ç') && (l3=='Ì') && (l2=='Å') && (l1=='Í')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='Ï') && (l3=='Ì') && (l2=='Å') && (l1=='Í')) 
				{
					sbResult[l-4]='\0';
					l-=4;
				}
				if ((l4=='Á') && (l3=='Ì') && (l2=='Å') && (l1=='Í') && (l-4 > 3)) 
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
				if (((l1=='Ê') && (l2=='Á') && (l3=='É')) ||
					((l1=='Ä') && (l2=='Ù') && (l3=='É')) ||
					((l1=='Ô') && (l2=='Í') && (l3=='Ï'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='Ï') && (l2=='Õ') && (l1=='Ì') && (l-3 > 3)) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='Å') && (l2=='É') && (l1=='Ô')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='É') && (l2=='Ó') && (l1=='Ì')) 
				{
					sbResult[l-2]='Æ';
					sbResult[l-1]='\0';
					l-=1;
				}
				if ((l3=='Á') && (l2=='Ó') && (l1=='Ì')) 
				{
					sbResult[l-2]='Æ';
					sbResult[l-1]='\0';
					l-=1;
				}
				if ((l3=='Ù') && (l2=='Í') && (l1=='Ô')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='É') && (l2=='Á') && (l1=='Ó')) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if (((l3=='Á') && (l2=='Ó') && (l1=='Ô')) ||
					((l3=='Á') && (l2=='Ó') && (l1=='È'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if (((l1=='È') && (l2=='Ó') && (l3=='É')) ||
					((l1=='Ô') && (l2=='Ó') && (l3=='É'))) 
				{
					sbResult[l-3]='É';
					sbResult[l-2]='Æ';
					sbResult[l-1]='\0';
					l-=1;
				}
				if (((l3=='Å') && (l2=='Ó')) && ((l1=='Ô') || (l1=='È'))) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((((l3=='Ù') && (l2=='Ó')) && ((l1=='Ô') || (l1=='È'))) && (l-3>3)) 
				{
					sbResult[l-3]='\0';
					l-=3;
				}
				if ((l3=='É') && (l2=='Ä') && (l1=='É')) 
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
				if ((l2=='Á') && (l1=='Ô') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='É') && (l1=='Ä')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Ç') && (l1=='Ó')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Ç') && (l1=='È')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}

				if ((l1=='Ó') && (l2=='É') && (l-2 > 3))  sbResult[l-1]='Æ'; 

				if ((l2=='Å') && (l1=='Ô')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='É') && (l1=='Ê') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Å') && (l1=='É')) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='É') && (l1=='Í') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Ù') && ((l1=='Ó') || (l1=='È'))) sbResult[l-1]='Í';
				if ((l2=='Á') && (l1=='È') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
				if ((l2=='Á') && (l1=='Ó') && (l-2 > 3)) 
				{
					sbResult[l-2]='\0';
					l-=2;
				}
			}

			if (l > 3) 
			{
				l1=sbResult[l-1];
				if ((l1=='É') || (l1=='Å')) 
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
	}

	/// <summary>
	/// Stemming is a Singleton class that performs stemming on English and Greek words. The
	/// English Stemmer has been adapted from the PHP Implementation by Jon Abernathy,
	/// jon@chuggnutt.com (http://www.chuggnutt.com). 
	/// Author: Kostas Stroggylos, kostas@circular.gr.
	/// </summary>
	public class Stemming : IStemming
	{
		private static Stemming instance;
		private EnglishStemming en;
		private GreekStemming gr;

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private Stemming()
		{
			en=new EnglishStemming();
			gr=new GreekStemming();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="Stemming"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="Stemming"/>.</returns>
		public static Stemming Instance()
		{
			if(instance==null)
			{
				instance=new Stemming();
			}
			return instance;
		}

		/// <summary>
		/// Performs stemming on a word in all supported languages.
		/// </summary>
		/// <param name="word">The word to be stemmed.</param>
		/// <returns>The stem of the word.</returns>
		public string StemWord(string word)
		{
			return gr.StemWord(en.StemWord(word));
		}
	}

}
