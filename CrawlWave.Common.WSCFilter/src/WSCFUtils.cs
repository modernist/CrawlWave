using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib;

namespace CrawlWave.Common.WSCFilter
{
	/// <summary>
	/// Defines an enumeration of different compression algorithms that can be used.
	/// </summary>
	public enum CompressionType
	{
		/// <summary>
		/// Enables Gzip compression (faster)
		/// </summary>
		GZip,
		/// <summary>
		/// Enables Bzip2 compression (slower)
		/// </summary>
		BZip2,
		/// <summary>
		/// Enables Zip compression
		/// </summary>
		Zip
	}

	/// <summary>
	/// Implements static methods that allow easy compression and decompression of strings,
	/// byte arrays and streams using <see cref="SharpZipLib"/> classes.
	/// </summary>
	public class WSCFCompression
	{
		/// <summary>
		/// Defines the <see cref="CompressionType"/> that will be used by the filters
		/// </summary>
		public static CompressionType CompressionProvider = CompressionType.GZip;

		/// <summary>
		/// Creates a new compression output stream that reads from an input stream based
		/// on the value of the <see cref="CompressionProvider"/> property.
		/// </summary>
		/// <param name="inputStream">The <see cref="Stream"/> that will be used for input.</param>
		/// <returns>An appropriate <see cref="SharpZipLib"/> output stream.</returns>
		private static Stream OutputStream(Stream inputStream)
		{
			switch(CompressionProvider)
			{
				case CompressionType.BZip2:
                    return new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(inputStream);
				case CompressionType.GZip:
                    return new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(inputStream);
				case CompressionType.Zip:
                    return new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(inputStream);
				default:
                    return new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(inputStream);
			}
		}

		/// <summary>
		/// Ctreates a new decompression input stream that reads from an input stream based
		/// on the value of the <see cref="CompressionProvider"/> property.
		/// </summary>
		/// <param name="inputStream">The <see cref="Stream"/> that will be used for input.</param>
		/// <returns>An appropriate <see cref="SharpZipLib"/> input stream.</returns>
		private static Stream InputStream(Stream inputStream)
		{
			switch(CompressionProvider)
			{
				case CompressionType.BZip2:
                    return new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(inputStream);
				case CompressionType.GZip:
                    return new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inputStream);
				case CompressionType.Zip:
                    return new ICSharpCode.SharpZipLib.Zip.ZipInputStream(inputStream);
				default:
                    return new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inputStream);                                                                        
			}
		}

		/// <summary>
		/// Compresses a byte array.
		/// </summary>
		/// <param name="buffer">The input buffer</param>
		/// <returns>An array of bytes containing the compressed data</returns>
		public static byte[] Compress(byte[] buffer)
		{
			return CompressToStream(buffer).ToArray();
		}

		/// <summary>
		/// Compresses a byte array into a <see cref="MemoryStream"/>.
		/// </summary>
		/// <param name="buffer">The input buffer.</param>
		/// <returns>A <see cref="MemoryStream"/> containing the compressed input.</returns>
		public static MemoryStream CompressToStream(byte[] buffer)
		{
			MemoryStream ms = new MemoryStream();
			Stream s = OutputStream(ms);
			s.Write(buffer,0, buffer.Length);
			s.Close();
			return ms;
		}
 
		/// <summary>
		/// Compresses a string
		/// </summary>
		/// <param name="inputString">The input string</param>
		/// <returns>The compressed string in Base64 format.</returns>
		public static string Compress(string inputString)
		{
			byte[] data = CompressToByte(inputString);
			string strOut = Convert.ToBase64String(data);
			return strOut;
		}

		/// <summary>
		/// Compresses a string into an array of bytes
		/// </summary>
		/// <param name="inputString">The input string.</param>
		/// <returns>An array of bytes containing the compressed input.</returns>
		public static byte[] CompressToByte(string inputString)
		{
			byte[] data = Encoding.UTF8.GetBytes(inputString);
			return Compress(data);
		}
 
		/// <summary>
		/// Decompresses a string of compressed data in Base64 format into a new string
		/// </summary>
		/// <param name="inputString">The input string in Base64 format</param>
		/// <returns>The decompressed string.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the input srting is null.</exception>
		public string DeCompress(string inputString)
		{
			string outString = string.Empty;
			if (inputString == null)
			{
				throw new ArgumentNullException("inputString","The input string is null and cannot be decompressed.");
			}
			try
			{
				byte[] data = Convert.FromBase64String(inputString.Trim());
				outString = System.Text.Encoding.UTF8.GetString(DeCompress(data));
			}
			catch (NullReferenceException  nEx)
			{
				return nEx.Message;
			}
			return outString;
		}
 
		/// <summary>
		/// Decompresses an array of bytes
		/// </summary>
		/// <param name="buffer">The input buffer</param>
		/// <returns>A byte array containing the decompressed input</returns>
		public static  byte[]  DeCompress(byte[] buffer)
		{
			MemoryStream outStream = DeCompressToStream(buffer);
			byte[] data = outStream.ToArray();
			outStream.Close();
			return data;
		}

		/// <summary>
		/// Decompresses a byte array into a new <see cref="MemoryStream"/>.
		/// </summary>
		/// <param name="buffer">The input buffer.</param>
		/// <returns>A <see cref="MemoryStream"/> containing the decompressed data.</returns>
		public static MemoryStream DeCompressToStream(byte[] buffer)
		{
			return DeCompressToStream(new MemoryStream(buffer));
		}

		/// <summary>
		/// Decompresses a <see cref="Stream"/> into a new <see cref="MemoryStream"/>.
		/// </summary>
		/// <param name="inputStream">The input <see cref="Stream"/>.</param>
		/// <returns>A new <see cref="MemoryStream"/> containing the decompressed data.</returns>
		public static MemoryStream DeCompressToStream(Stream inputStream)
		{
			Stream inStream = InputStream(inputStream);
			MemoryStream outStream = new MemoryStream();
			byte[] buffer = new byte[4096]; int size = 0;
			while(true)
			{
				size = inStream.Read(buffer,0,4096);
				if(size>0)
				{
					outStream.Write(buffer,0,size);
				}
				else
				{
					break;
				}
			}
			inStream.Close();
			return outStream;
		}
	}

	/// <summary>
	/// Defines constants used in <see cref="WSCFZipFilter"/> and <see cref="WSCFUnzipFilter"/>.
	/// </summary>
	public class Constants
	{
		/// <summary>
		/// The name of the SOAP header element that the filter uses as identifier
		/// </summary>
		public static string WSCFCompressionElement = "WSCFilter";
		/// <summary>
		/// The name of the first attribute
		/// </summary>
		public static string WSCFAttribute = "bWSCFZipped";
		/// <summary>
		/// The name of the second attribute
		/// </summary>
		public static string WSCFTypeAttribute = "CompressMethod";
		/// <summary>
		/// The constant size of 1KB (2^10)
		/// </summary>
		public static int	WSCF1KB = 1024;
	}

}