using System;
using System.Security.Cryptography;
using System.Text;
using EXCSDecl;
using EXCSStrUtl;

namespace EXCSCrypt
{
// den doulevei pros to parwn. 19/8/2003

	/// <summary>
	/// Summary description for EXCSCrypt.
	/// </summary>
	sealed public class TEXCSCrypt
	{
		// Encryption made easy with RSA
		// Both in and out data is in ByteArrays
	
		// Example: How to initialize TEXCSCrypt?
		// EXCSCrypt.TEXCSCrypt myCrypto = new EXCSCrypt.TEXCSCrypt();
		//
		// Example: How to encrypt a message?
		// byte[] myEncryptedData = myCrypto.EncryptData( System.Text.ASCIIEncoding.ASCII.GetBytes( "Your Message here!" ) );
		// 
		// Example: How to decrypt the encrypted message?
		// (Remember that the encrypted that is in a byteArray, it's return as a byteArray when encrypted
		// string myDecryptedData = System.Text.ASCIIEncoding.ASCII.GetString( myCrypto.DecryptData( myEncryptedData ) );

		private TripleDESCryptoServiceProvider DES;
		private MD5CryptoServiceProvider hashMD5;

		private TEXCSCrypt() 
		{
			DES = new TripleDESCryptoServiceProvider();
			hashMD5 = new MD5CryptoServiceProvider();
		}
		
		public static readonly TEXCSCrypt Instance = new TEXCSCrypt();

		public static string HEXStrEncryptStringTripleDES(string strToEncrypt, string theKey)
		{
			return TEXStrUtl.EncodeToHexStr(EncryptStringTripleDES(strToEncrypt,theKey));
		}

		public static string HEXStrDecryptHexStringTripleDES(string strToDecrypt, string theKey)
		{
			return DecryptStringTripleDES(TEXStrUtl.DecodeHexStrToStr(strToDecrypt),theKey);
		}
 
		public static string EncryptStringTripleDES(string strToEncrypt, string theKey)
		{			
			
			Instance.DES.Key = Instance.hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(theKey));
			Instance.DES.Mode = CipherMode.ECB;
			ICryptoTransform DESEncrypt = Instance.DES.CreateEncryptor();
			byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
			return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
		}
 
		public static string DecryptStringTripleDES(string strToDecrypt, string theKey)
		{
			Instance.DES.Key = Instance.hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(theKey));
			Instance.DES.Mode = CipherMode.ECB;
			ICryptoTransform DESDecrypt = Instance.DES.CreateDecryptor();
			byte[] Buffer = Convert.FromBase64String(strToDecrypt);
			return ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
		} 

		public static string GetStringForCloseDateScrableKey(int intDaysFromToday)
		{
			const string strKeyToScrable = "4EEE3458-B428-4b71-B7ED-F5FF05B603EB";
			return EncryptStringTripleDES(DateTime.Now.AddDays((intDaysFromToday*1.0)).ToString(TEXCSDecl.C_FMTYearMonthDateNoSlash),strKeyToScrable);
		}


		public static string HEXStrNearTodayEncryptStringTripleDES(string strToEncrypt)
		{				
			if (strToEncrypt==string.Empty)
			{
				return string.Empty;
			}
			return TEXStrUtl.EncodeToHexStr(NearTodayEncryptStringTripleDES(strToEncrypt));

		}
 
		public static string HEXStrNearTodayDecryptStringTripleDES(string strToDecrypt)
		{
			if (strToDecrypt==string.Empty)
			{
				return string.Empty;
			}
			return NearTodayDecryptStringTripleDES(TEXStrUtl.DecodeHexStrToStr(strToDecrypt));
		}

		public static string NearTodayEncryptStringTripleDES(string strToEncrypt)
		{				
			return TEXCSCrypt.EncryptStringTripleDES(strToEncrypt,GetStringForCloseDateScrableKey(0));			
		}
 
		public static string NearTodayDecryptStringTripleDES(string strToDecrypt)
		{
			string strResult=string.Empty;
			try
			{
				strResult=TEXCSCrypt.DecryptStringTripleDES(strToDecrypt,GetStringForCloseDateScrableKey(0));
			}
			catch
			{
				try
				{
					strResult=TEXCSCrypt.DecryptStringTripleDES(strToDecrypt,GetStringForCloseDateScrableKey(-1));
				}
				catch
				{
					strResult=TEXCSCrypt.DecryptStringTripleDES(strToDecrypt,GetStringForCloseDateScrableKey(1));
				}
			}
			return strResult;
		} 

	}
}
