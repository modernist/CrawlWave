using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace CrawlWave.Common
{
	/// <summary>
	/// MD5Hash provides a static method returning the MD5 Hash of a string, either as a 16 
	/// byte array or formatted as a hexadecimal string. Its constructor is private, thus 
	/// not allowing the creation of instances of the class.
	/// </summary>
	public sealed class MD5Hash
	{
		/// <summary>
		/// Default constructor. Not necessaty, since all members are static.
		/// </summary>
		private MD5Hash()
		{}
		
		/// <summary>
		/// Returns a Hash of a given string using RSA's MD5 Message Digest Algorithm. 
		/// </summary>
		/// <param name="text">The string to digest</param>
		/// <returns>An array of 16 bytes containing the MD5 hash of the input</returns>
		public static byte[] md5(string text)
		{
			byte []retVal=new byte[16];
			try
			{
				//First we need to convert the string into bytes using a text encoder.
				Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

				// Create a buffer large enough to hold the string
				byte[] unicodeText = new byte[text.Length * 2];
				enc.GetBytes(text.ToCharArray(), 0, text.Length, unicodeText, 0, true);
				
				// Now that we have a byte array we can ask the CSP to hash it
				MD5 md5 = new MD5CryptoServiceProvider();
				retVal = md5.ComputeHash(unicodeText); 
			}
			catch
			{
				retVal=new byte[] {0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0};
			}
			return retVal;
		}
		
		/// <summary>
		/// Returns a Hash of a given string using RSA's MD5 Messasge Digest Algorithm
		/// after converting it in a string in hexadecimal format.
		/// </summary>
		/// <param name="text">The string to digest</param>
		/// <returns>A string containing the input's hash value in hexadecimal form</returns>
		public static string md5string(string text)
		{
			string retVal="";
			try
			{
				//first use the md5() function to get the hash as an array of bytes
				byte []result=md5(text);

				//now convert it to a string using a StringBuilder
				StringBuilder sb = new StringBuilder();
				for (int i=0;i<result.Length;i++)
				{
					sb.Append(result[i].ToString("X2"));
				}

				// And return it
				retVal= sb.ToString();
			}
			catch
			{
				retVal="";
			}
			return retVal;
		}
	}

	/// <summary>
	/// SHA-1 Hash Algorithm implementation
	/// </summary>
	public sealed class SHA1Hash
	{
		#region Implementation of the SHA1 Algorithm

		//SHA-1 constants
		private const uint _H00 = 0x67452301;
		private const uint _H10 = 0xefcdab89;
		private const uint _H20 = 0x98badcfe;
		private const uint _H30 = 0x10325476;
		private const uint _H40 = 0xc3d2e1f0;
	
		/// <summary>
		/// This method take a byte array, pads it according to the 
		/// specifications in FIPS 180-2 and passes to the SHA-1
		/// hash function. It returns a byte array containing 
		/// the hash of the given message.
		/// </summary>
		/// <param name="message">Message to be hashed as byte array</param>
		/// <returns>Message hash value as byte array</returns>
		public static byte[] SHA1( byte[] message )
		{
			int n, pBase;
			long l;
			byte[] m;
			byte[] pad;
			byte[] swap;
			byte[] temp;

			// Determine length of message in 512-bit blocks
			n = (int)(message.Length >> 6);

			// Determine space required for padding
			if( (message.Length & 0x3f) < 56 )
			{			
				n++;
				pBase = 56;
			}
			else
			{
				n+=2;
				pBase = 120;
			}

			// Determine total message length
			l = message.Length << 3;

			// Create message padding 
			pad = new byte[pBase-(message.Length & 0x3f)];
			pad[0] = 0x80;
			for( int i = 1; i < pad.Length; i++ )
				pad[i] = 0;
			m = new byte[n*64];

			// Assemble padded message
			Array.Copy( message, 0, m, 0, message.Length );
			Array.Copy( pad, 0, m, message.Length, pad.Length );
			temp = BitConverter.GetBytes( (long)l );
			swap = new byte[8];
			for( int i = 0; i < 8; i++ )
				swap[i] = temp[7-i];
			Array.Copy( swap, 0, m, message.Length+pad.Length, 8 );

			// Call Hash1 to hash message
			return HashCore( n, m );
		}

		/// <summary>
		/// SHA-1 Hash function - This method should only be used
		/// with properly padded messages.  To hash an unpadded 
		/// message use one of the other methods.  This method is
		/// called by other methods once the message is padded
		/// and loaded as a byte array.  For internal use only!
		/// </summary>
		/// <param name="n">Number of 512-bit message segments</param>
		/// <param name="m">Message to be hashed as byte array</param>
		/// <returns>Hash value as byte array</returns>
		private static byte[] HashCore( int n, byte[] m )
		{
			// K Constants
			uint[] K = { 0x5a827999, 0x6ed9eba1, 0x8f1bbcdc, 0xca62c1d6 };
			// Calculation variables
			uint a, b, c, d, e, temp;
			// Intermediate hash values
			uint[] h = new uint[5];
			// Scheduled W values
			uint[] w = new uint[80];
			// Final hash byte array
			byte[] hash = new byte[20];
			// Used to correct for endian
			byte[] swap = new byte[4];
			byte[] swap2 = new byte[4];

			// Initial hash values
			h[0] = _H00;
			h[1] = _H10;
			h[2] = _H20;
			h[3] = _H30;
			h[4] = _H40;

			// Perform hash operation
			for( int i = 0; i < n; i++ )
			{
				// Prepare the message schedule
				for( int t = 0; t < 80; t++ )
				{
					if( t < 16 )
					{	
						for( int j = 0; j < 4; j++ )
							swap[j] = m[i*64+t*4+3-j];
						w[t] = BitConverter.ToUInt32( swap, 0 );	
					}
					else
					{
						temp = (uint)( w[t-3] ^ w[t-8] ^ w[t-14] ^ w[t-16] );
						w[t] = (uint)( (temp>>31) | (temp<<1) );
					}
				}

				//Initialize the five working variables
				a = h[0];
				b = h[1];
				c = h[2];
				d = h[3];
				e = h[4];
				
				//Perform main hash loop
				for( int t = 0; t < 80; t++ )
				{
					int kt;
					if( t < 20 )
						kt = 0;
					else if( t < 40 )
						kt = 1;
					else if( t < 60 )
						kt = 2;
					else
						kt = 3;
					temp = (uint)( (uint)( (a<<5) | (a>>27) ) + Func(t, b, c, d) 
						+ e + K[kt] + w[t] );						
					e = d;
					d = c;
					c = (uint)( (b<<30) | (b>>2) );
					b = a;
					a = temp;
				}

				//Compute the intermediate hash values
				h[0] = (uint)( (a + h[0]) );
				h[1] = (uint)( (b + h[1]) );
				h[2] = (uint)( (c + h[2]) );
				h[3] = (uint)( (d + h[3]) );
				h[4] = (uint)( (e + h[4]) );
			}
	
			// Copy Intermediate results to final hash array
			for( int i = 0; i < 5; i++)
			{
				swap2 = BitConverter.GetBytes( (uint) h[i] );
				for( int j = 0; j < 4; j++ )
				{
					swap[j] = swap2[3-j];
				}
				Array.Copy( swap, 0, hash, i*4, 4 );
			}

			return hash;
		}

		/// <summary>
		/// Performs SHA-1 logical functions.  See FIPS 180-2 for
		/// complete description.  For internal use only!
		/// </summary>
		/// <param name="t">function number</param>
		/// <param name="x">first arguement</param>
		/// <param name="y">second arguement</param>
		/// <param name="z">third arguement</param>
		/// <returns>Function results</returns>
		private static uint Func( int t, uint x, uint y, uint z )
		{
			// Ch function
			if( t < 20 )
				return (uint)( (x & y) ^ ((~x) & z) );
				// Maj function
			else if( (t < 60) && (t >= 40) )
				return (uint)( (x & y) ^ (x & z) ^ (y & z) );
				// Parity function
			else			
				return (uint)( x ^ y ^ z );
		}

		#endregion
	}
}