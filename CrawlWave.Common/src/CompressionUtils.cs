using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace CrawlWave.Common
{
	/// <summary>
	/// CompressionUtils uses the Adapter Design Pattern to provide some functionality
	/// using the SharpZipLib library. It provides static methods for easy compression
	/// and decompression of data. It is a Wrapper for classes and methods available in
	/// SharpZipLib. The compression is used for data passed between the different apps
	/// of the system. This way the whole crawling process can require less bandwidth.
	/// </summary>
	public class CompressionUtils
	{
		#region Private variables

		/// <summary>
		/// A Compression Level of 7 offers very good compression/speed ratio
		/// </summary>
		private const int compressionLevel = 7;

		#endregion

		#region Public Interface members

		/// <summary>
		/// Compresses a string and stores the result in an array of bytes.
		/// </summary>
		/// <param name="uncompressed">The uncompressed string.</param>
		/// <param name="compressed">An array of bytes where the compressed string will be stored.</param>
		/// <remarks>
		/// The compressed string is passed back to the calling method as an <b>out</b>
		/// parameter. That means that the calling method doesn't need to initialize the
		/// compressed string buffer.  
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the uncompressed input string is empty or null.
		/// </exception>
		/// <exception cref="CWZipException">
		/// Thrown if a problem is encountered during the compression process.
		/// </exception>
		public static void CompressString(ref string uncompressed, out byte[] compressed)
		{
			if ((uncompressed == null) || (uncompressed.Length == 0))
			{
				throw new ArgumentNullException("uncompressed", "The uncompressed string cannot be null or empty.");
			}
			MemoryStream ms = null;
			compressed = null;
			try
			{
				byte[] ubytes = Encoding.UTF8.GetBytes(uncompressed);
				ms = new MemoryStream();
				ZipOutputStream zip = new ZipOutputStream(ms);
				zip.SetLevel(compressionLevel);
				ZipEntry entry = new ZipEntry("1");
				zip.PutNextEntry(entry);
				zip.Write(ubytes, 0, ubytes.Length);
				zip.Finish();
				ms.Position = 0;
				compressed = ms.ToArray();
				ms.Close();
			}
			catch (Exception e)
			{
				if (ms != null)
				{
					ms.Close();
				}
				throw new CWZipException(e.Message);
			}
			finally
			{
				ms = null;
				GC.Collect();
			}
		}

		/// <summary>
		/// Compresses an array of bytes and stores the result in a new array of bytes.
		/// </summary>
		/// <param name="uncompressed">The uncompressed buffer.</param>
		/// <param name="compressed">An array of bytes where the compressed input will be stored.</param>
		/// <remarks>
		/// The compressed input is passed back to the calling method as an <b>out</b>
		/// parameter. That means that the calling method doesn't need to initialize the
		/// compressed buffer.  
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the uncompressed input buffer is empty or null.
		/// </exception>
		/// <exception cref="CWZipException">
		/// Thrown if a problem is encountered during the compression process.
		/// </exception>
		public static void CompressBuffer(byte[] uncompressed, out byte[] compressed)
		{
			if ((uncompressed == null) || (uncompressed.Length == 0))
			{
				throw new ArgumentNullException("uncompressed", "The uncompressed input buffer cannot be null or empty.");
			}
			MemoryStream ms = null;
			compressed = null;
			try
			{
				ms = new MemoryStream();
				ZipOutputStream zip = new ZipOutputStream(ms);
				zip.SetLevel(compressionLevel);
				ZipEntry entry = new ZipEntry("1");
				zip.PutNextEntry(entry);
				zip.Write(uncompressed, 0, uncompressed.Length);
				zip.Finish();
				ms.Position = 0;
				compressed = ms.ToArray();
				ms.Close();
			}
			catch (Exception e)
			{
				if (ms != null)
				{
					ms.Close();
				}
				throw new CWZipException(e.Message);
			}
			finally
			{
				ms = null;
				GC.Collect();
			}
		}

		/// <summary>
		/// Decompresses an array of bytes and stores the result into a string.
		/// </summary>
		/// <param name="compressed">An array of bytes containing the compressed string.</param>
		/// <param name="decompressed">The string where the decompressed string will be returned.</param>
		/// <remarks>
		/// The decompressed string is passed back to the calling method as an <b>out</b>
		/// parameter. That means that the calling method doesn't need to initialize the
		/// decompressed string.  
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the compressed input buffer is empty or null.
		/// </exception>
		/// <exception cref="CWZipException">
		/// Thrown if a problem is encountered during the decompression process.
		/// </exception>
		public static void DecompressToString(byte[] compressed, out string decompressed)
		{
			if ((compressed == null) || (compressed.Length == 0))
			{
				throw new ArgumentNullException("compressed", "The compressed input buffer cannot be null or empty.");
			}
			MemoryStream ms = null;
			StringBuilder sb = null;
			try
			{
				ms = new MemoryStream(compressed);
				ZipInputStream s = new ZipInputStream(ms);
				ZipEntry entry;
				entry = s.GetNextEntry();
				if (entry != null)
				{
					int size;
					sb = new StringBuilder();
					byte[] data = new byte[4096];
					while (true)
					{
						size = s.Read(data, 0, data.Length);
						if (size > 0)
						{
							sb.Append(Encoding.UTF8.GetString(data, 0, size));
						}
						else
						{
							break;
						}
					}
					s.Close();
				}
				else
				{
					s.Close();
					throw new CWZipException("The Zip archive contains no entries.");
				}
				ms.Close();
				decompressed = sb.ToString();
				sb = null;
			}
			catch (Exception e)
			{
				if (ms != null)
				{
					ms.Close();
				}
				throw new CWZipException(e.Message);
			}
			finally
			{
				ms = null;
				GC.Collect();
			}
		}

		/// <summary>
		/// Decompresses an array of bytes and stores the result into a new array of bytes.
		/// </summary>
		/// <param name="compressed">An array of bytes containing the compressed input.</param>
		/// <param name="decompressed">The buffer where the decompressed input will be returned.</param>
		/// <remarks>
		/// The decompressed buffer is passed back to the calling method as an <b>out</b>
		/// parameter. That means that the calling method doesn't need to initialize the
		/// decompressed buffer.  
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the compressed input buffer is empty or null.
		/// </exception>
		/// <exception cref="CWZipException">
		/// Thrown if a problem is encountered during the decompression process.
		/// </exception>
		public static void DecompressBuffer(byte[] compressed, out byte[] decompressed)
		{
			if ((compressed == null) || (compressed.Length == 0))
			{
				throw new ArgumentNullException("compressed", "The compressed input buffer cannot be null or empty.");
			}
			MemoryStream ms = null, output = null; ;
			try
			{
				ms = new MemoryStream(compressed);
				ZipInputStream s = new ZipInputStream(ms);
				ZipEntry entry;
				entry = s.GetNextEntry();
				if (entry != null)
				{
					int size;
					output = new MemoryStream();
					byte[] data = new byte[4096];
					while (true)
					{
						size = s.Read(data, 0, data.Length);
						if (size > 0)
						{
							output.Write(data, 0, size);
						}
						else
						{
							break;
						}
					}
					s.Close();
				}
				else
				{
					s.Close();
					throw new CWZipException("The Zip archive contains no entries.");
				}
				ms.Close();
				decompressed = output.ToArray();
				output.Close();
			}
			catch (Exception e)
			{
				if (ms != null)
				{
					ms.Close();
				}
				if (output != null)
				{
					output.Close();
				}
				throw new CWZipException(e.Message);
			}
			finally
			{
				ms = null;
				output = null;
				GC.Collect();
			}
		}

		/// <summary>
		/// Calculates the CRC (Cyclic Redundancy Check) Checksum of a string.
		/// </summary>
		/// <param name="input">The string whose the CRC is needed</param>
		/// <returns>The CRC value of the string</returns>
		public static long StringCRC(string input)
		{
			Crc32 stringCRC = new Crc32();
			stringCRC.Update(Encoding.UTF8.GetBytes(input));
			return stringCRC.Value;
		}

		/// <summary>
		/// Returns the CRC (Cyclic Redundancy Check) Checksum of an array of bytes.
		/// </summary>
		/// <param name="input">The array of bytes whose CRC is needed</param>
		/// <returns>The CRC value of the array of bytes</returns>
		public static long BufferCRC(byte[] input)
		{
			Crc32 bufferCRC = new Crc32();
			bufferCRC.Update(input);
			return bufferCRC.Value;
		}

		#endregion
	}
}
