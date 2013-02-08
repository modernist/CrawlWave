using System;
using System.IO;
using System.Net;

namespace CrawlWave.Common
{
	/// <summary>
	/// InternetUtils is a class providing generic static Internet-related functions.
	/// </summary>
	public class InternetUtils
	{
		#region Private variables

		private static string[] UrlsToCheck = { "http://www.aueb.gr", "http://www.in.gr" };
		//these are the urls used to check whether we're connected to the internet
		private static string[] IPAddressUrls =
		{
			"http://ftp.apnic.net/stats/apnic/delegated-apnic-latest",
			"http://ftp.apnic.net/stats/arin/delegated-arin-latest",
			"http://ftp.apnic.net/stats/lacnic/delegated-lacnic-latest",
			"http://ftp.apnic.net/stats/ripe-ncc/delegated-ripencc-latest"
		};

		private static string[] AlternativeIPAddressUrls =
		{
			"ftp://ftp.arin.net/pub/stats/apnic/delegated-apnic-",
			"ftp://ftp.arin.net/pub/stats/arin/delegated-arin-",
			"ftp://ftp.arin.net/pub/stats/lacnic/delegated-lacnic-",
			"ftp://ftp.arin.net/pub/stats/ripencc/delegated-ripencc-"
		};

		#endregion

		/// <summary>
		/// Nothing to initialize - at least for now, so we'll make it a private
		/// method to make sure that no instances of this class can be created.
		/// </summary>
		private InternetUtils()
		{ }

		/// <summary>
		/// This function is used to check if we're connected to the internet. It tries
		/// to visit AUEB's home page, and if unsuccessful it tries accessing in.gr. If
		/// both the attempts fail, it's assumed that we're not connected.
		/// </summary>
		/// <returns>
		/// True if we're connected, false otherwise
		/// </returns>
		public static bool ConnectedToInternet()
		{
			bool retVal = false;
			foreach (string url in UrlsToCheck)
			{
				HttpWebRequest netRequest = (HttpWebRequest)HttpWebRequest.Create(url);
				netRequest.Timeout = 15000;
				netRequest.Method = "HEAD";
				HttpWebResponse netResponse = null;
				try
				{
					netResponse = (HttpWebResponse)netRequest.GetResponse();
					if (netResponse.StatusCode == System.Net.HttpStatusCode.OK)
					{
						//exit the loop as soon as we find out there is a connection
						retVal = true;
						break;
					}
				}
				catch (Exception)
				{
					//an error has occured, no connection available possibly
					//so we'll try getting the next url in the list
					retVal = false;
				}
				finally
				{
					//free memory
					if (netResponse != null)
					{
						netResponse.Close();
						netResponse = null;
					}
					netRequest = null;
				}
			}
			return retVal;
		}

		/// <summary>
		/// DetectConnectionSpeed attempts to detect the computer's internet connection speed
		/// by measuring how much time it takes to download the contents of a web site.
		/// </summary>
		/// <returns>A <see cref="CWConnectionSpeed"/> value containing the estimated internet
		/// connection speed.</returns>
		public static CWConnectionSpeed DetectConnectionSpeed()
		{
			CWConnectionSpeed retVal = CWConnectionSpeed.Unknown;
			try
			{
				WebClient client = new WebClient();
				HiResTimer timer = new HiResTimer();
				byte[] data;
				timer.Start();
				try
				{
					data = client.DownloadData("http://www.in.gr/");
				}
				catch
				{
					data = new byte[0];
				}
				finally
				{
					timer.Stop();
					client.Dispose();
				}
				if (data.Length > 0)
				{
					double Kbps = ((data.Length * 8) / timer.Duration);
					//determine which enumeration value fits best
					if ((Kbps > 0) && (Kbps <= 56))
						retVal = CWConnectionSpeed.Modem56K;
					else if (Kbps <= 64)
						retVal = CWConnectionSpeed.ISDN64K;
					else if (Kbps <= 128)
						retVal = CWConnectionSpeed.ISDN128K;
					else if (Kbps <= 256)
						retVal = CWConnectionSpeed.DSL256K;
					else if (Kbps <= 512)
						retVal = CWConnectionSpeed.DSL512K;
					else if (Kbps <= 1024)
						retVal = CWConnectionSpeed.DSL1M;
					else if ((Kbps > 1024) && (Kbps <= 1536))
						retVal = CWConnectionSpeed.T1;
					else if ((Kbps > 1536) && (Kbps <= 46080))
						retVal = CWConnectionSpeed.T3;
					else if ((Kbps > 46080) && (Kbps < 158720))
						retVal = CWConnectionSpeed.Fiber;
					else if (Kbps >= 158720)
						retVal = CWConnectionSpeed.ATM;
					else
						retVal = CWConnectionSpeed.Unknown;
				}
			}
			catch
			{ }
			return retVal;
		}

		/// <summary>
		/// Downloads the latest version of the IP Address files from APNIC,ARIN,LACNIC and
		/// RIPE and stores them in the directory provided with the appropriate names.
		/// </summary>
		/// <param name="DirectoryPath">
		/// The path of the directory where the downloaded files will be stored. The path
		/// <strong>must</strong> contain the trailing backslash.
		/// </param>
		public static void UpdateIPAddressFiles(string DirectoryPath)
		{
			if (!DirectoryPath.EndsWith("\\"))
			{
				throw new ArgumentException("The DirectoryPath must contain the trailing backslash.");
			}
			bool use_Alternative = false;
			string Url, FileName;
			string DateString = DateTime.Now.ToString("yyyyMMdd");
			WebRequest.RegisterPrefix("ftp:", new FtpRequestCreator());
			for (int i = 0; i < IPAddressUrls.Length; i++)
			{
				try
				{
					if (use_Alternative)
					{
						Url = AlternativeIPAddressUrls[i];
					}
					else
					{
						Url = IPAddressUrls[i];
					}
					if (Url.EndsWith("-"))
					{
						Url = Url + DateString;
					}
					switch (i)
					{
						case 0:
							FileName = DirectoryPath + "apnic.latest";
							break;
						case 1:
							FileName = DirectoryPath + "arin.latest";
							break;
						case 2:
							FileName = DirectoryPath + "lacnic.latest";
							break;
						case 3:
							FileName = DirectoryPath + "ripencc.latest";
							break;
						default:
							FileName = DirectoryPath + "other.latest";
							break;
					}
					if (Url.StartsWith("ftp://"))
					{
						WebRequest wr = WebRequest.Create(Url);
						wr.Method = "get";
						FtpWebRequest fr = wr as FtpWebRequest;
						fr.Passive = true;
						WebResponse resp = fr.GetResponse();
						Stream rs = resp.GetResponseStream();
						if (rs.CanRead)
						{
							FileStream fs = new FileStream(FileName, FileMode.Create);
							byte[] buffer = new byte[1024];
							int bytes_read = 0;
							while ((bytes_read = rs.Read(buffer, 0, 1024)) > 0)
							{
								fs.Write(buffer, 0, bytes_read);
							}
							fs.Close();
						}
						rs.Close();
					}
					else
					{
						WebClient client = new WebClient();
						string tmpFileName = FileName + ".tmp";
						try
						{
							client.DownloadFile(Url, tmpFileName);
							File.Copy(tmpFileName, FileName, true);
							File.Delete(tmpFileName);
						}
						catch (Exception e)
						{
							File.Delete(tmpFileName);
							throw e;
						}
					}
					use_Alternative = false;
				}
				catch
				{
					if (!use_Alternative)
					{
						use_Alternative = true;
						i--;
					}
				}
			}
		}

		/// <summary>
		/// Returns the host name of a Url
		/// </summary>
		/// <param name="url">The Url to examine</param>
		/// <returns>A string containing the Url's host name or IP Address.</returns>
		public static string HostName(string url)
		{
			try
			{
				Uri uri = new Uri(url);
				return uri.Host;
			}
			catch
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// Returns the host name of an <see cref="InternetUrlToCrawl"/>
		/// </summary>
		/// <param name="url">The <see cref="InternetUrlToCrawl"/> to examine</param>
		/// <returns>A string containing the Url's host name or IP Address.</returns>
		public static string HostName(InternetUrlToCrawl url)
		{
			try
			{
				Uri uri = new Uri(url.Url);
				return uri.Host;
			}
			catch
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// Returns the host name of an <see cref="InternetUrlToIndex"/>
		/// </summary>
		/// <param name="url">The <see cref="InternetUrlToIndex"/> to examine</param>
		/// <returns>A string containing the Url's host name or IP Address.</returns>
		public static string HostName(InternetUrlToIndex url)
		{
			try
			{
				Uri uri = new Uri(url.Url);
				return uri.Host;
			}
			catch
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// Returns the host name of an <see cref="InternetUrl"/>
		/// </summary>
		/// <param name="url">The <see cref="InternetUrl"/> to examine</param>
		/// <returns>A string containing the Url's host name or IP Address.</returns>
		public static string HostName(InternetUrl url)
		{
			try
			{
				Uri uri = new Uri(url.Url);
				return uri.Host;
			}
			catch
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// Encodes a string to Base64 Format
		/// </summary>
		/// <param name="data">The string to be converted to Base64 format.</param>
		/// <returns>A string containing the Base64 formatted input.</returns>
		public static string Base64Encode(string data)
		{
			try
			{
				byte[] encData_byte = new byte[data.Length];
				encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
				string encodedData = Convert.ToBase64String(encData_byte);
				return encodedData;
			}
			catch (Exception e)
			{
				throw new Exception("Error in Base64Encode" + e.Message);
			}
		}

		/// <summary>
		/// Converts a Base64 Encoded string to its original form.
		/// </summary>
		/// <param name="data">The data to be converted from Base64 format.</param>
		/// <returns>The decoded string.</returns>
		public static string Base64Decode(string data)
		{
			try
			{
				System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
				System.Text.Decoder utf8Decode = encoder.GetDecoder();

				byte[] todecode_byte = Convert.FromBase64String(data);
				int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
				char[] decoded_char = new char[charCount];
				utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
				string result = new String(decoded_char);
				return result;
			}
			catch (Exception e)
			{
				throw new Exception("Error in Base64Decode" + e.Message);
			}
		}
	}
}
