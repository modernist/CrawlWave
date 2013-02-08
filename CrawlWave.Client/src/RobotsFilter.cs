using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using CrawlWave.Common;
using CrawlWave.Client;

namespace CrawlWave.Client
{
	/// <summary>
	/// RobotsFilter is a Singleton class that performs filtering of Urls according to the
	/// <a href="http://www.robotstxt.org/wc/exclusion.html">Robots Exclusion Standard</a>.
	/// It holds a <see cref="Dictionary{key, value}"/> of <see cref="RobotsTxtEntry"/> objects, one for
	/// each host in order to allow for fast lookup. It grows up to a specific size and can
	/// be saved to permament storage (an XML file) and restored from an XML file.
	/// </summary>
	public class RobotsFilter
	{
		#region Private variables

		private static RobotsFilter instance;	//The single class instance
		private const int maxItems = 100000;// The max number of entries that will be stored
		private Dictionary<string, RobotsTxtEntry> robotsTable;	//The Hashtable that will hold the entries
		private Mutex mutex;			//Mutex supporting safe access from multiple threads
		private Globals globals; //Provides access to the global variables and application settings
		private static string [] userAgent; //The various user-agent constant strings we will be searching for
		private static string disallow; //The Disallow entries identifier we'll be searching for
		private static string FileName; //The name of the file where the serialized entries are stored.
		private Encoding encoding = System.Text.Encoding.UTF8; //Needed to parse the robots.txt files

		#endregion

		#region Constructor and Singleton Instance Members

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private RobotsFilter()
		{
			//Initialize the synchronization mechanism
			mutex=new Mutex();
			//Initialize the storage for the RobotsTxtEntry objects
			robotsTable = new Dictionary<string, RobotsTxtEntry>(1024);
			//Get a reference to the global variables and application settings
			globals = Globals.Instance();
			//Initialize the various strings. Interning them saves us a little memory.
			userAgent = new string [] { String.Intern("User-agent: "), String.Intern("User-agent: *"), String.Intern("User-agent: CrawlWave")};
			disallow = String.Intern("Disallow: ");
			FileName = String.Intern(globals.AppDataPath + "RobotsCache.xml");
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="RobotsFilter"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="RobotsFilter"/>.</returns>
		public static RobotsFilter Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new RobotsFilter();
				}
				imutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Interface methods

		/// <summary>
		/// Checks if the Robots Exclusion Standard allows the crawler to visit a url.
		/// </summary>
		/// <param name="targetUrl">The url that is to be validated.</param>
		/// <param name="sourceUrl">The <see cref="InternetUrlToCrawl"/> containing the targetUrl.</param>
		/// <param name="robotsMeta">A <see cref="RobotsMetaTagValue"/> flag indicating the 
		/// restrictions posed by the robots meta tag contained in the sourceUrl.</param>
		/// <returns> A <see cref="Boolean"/> value indicating whether the crawler is 
		/// allowed (false) or disallowed (true) to visit the target Url.</returns>
		/// <remarks>This method is safe for multi-threaded operations. However only one
		/// thread will be able to perform a check at any given time.
		/// </remarks>
		public bool FilterUrl(string targetUrl, InternetUrlToCrawl sourceUrl, RobotsMetaTagValue robotsMeta)
		{
			bool retVal = false; //assume that it's allowed to crawl the targetUrl
			try
			{
				mutex.WaitOne();
				//perhaphs we should use the hash code of the hostnames as keys.
				string targetHost = InternetUtils.HostName(targetUrl);
				string sourceHost = InternetUtils.HostName(sourceUrl);
				RobotsTxtEntry robots = null;
				//Do we need to fetch the robots.txt for the source Url?
				if(sourceUrl.FlagFetchRobots)
				{
					//we must fetch the robots.txt from the source url host and update sourceUrl.
					robots = FetchRobots(sourceHost);
					sourceUrl.RobotsDisallowedPaths = ConcatenatePaths(robots.DisallowedPaths);
					sourceUrl.FlagFetchRobots = false; //fetch it only once
					//check if it exists in the Hashtable, if so update it, otherwise add it
					if(robotsTable.ContainsKey(sourceHost))
					{
						robotsTable[sourceHost] = robots;
					}
					else
					{
						robotsTable.Add(sourceHost, robots);
					}
				}
				else
				{
					//check if it exists in the Hashtable. If so check if it has expired, else just get it from InternetUrlToCrawl
					if(!robotsTable.TryGetValue(sourceHost, out robots))
					{
						robots = new RobotsTxtEntry();
						robots.DisallowedPaths = SplitPaths(sourceUrl.RobotsDisallowedPaths);
						robotsTable.Add(sourceHost, robots);
					}
					else
					{
						if(robots.ExpirationDate<DateTime.Today)
						{
							robots = FetchRobots(sourceHost);
							robotsTable[sourceHost] = robots;
						}
					}
				}
				if(targetHost != sourceHost)
				{
					//the target url is on a different host, we must get its robots.txt
					if(!robotsTable.TryGetValue(targetHost, out robots))
					{
						robots = FetchRobots(targetHost);
						robotsTable.Add(targetHost, robots);
					}
					else
					{
						if(robots.ExpirationDate<DateTime.Today)
						{
							robots = FetchRobots(targetHost);
							robotsTable[targetHost] = robots;
						}
					}
				}
				if((robotsMeta & RobotsMetaTagValue.NoFollow)>0)
				{
					//if the meta tag has the NoFollow option set then we cannot crawl targetUrl
					retVal = true;
				}
				else
				{
					robots = robotsTable[targetHost];
					//if the DisallowedPaths is null then we can crawl targetUrl, otherwise we must check the disallowed paths
					if(robots.DisallowedPaths!=null)
					{
						for(int i = 0; i < robots.DisallowedPaths.Length; i++)
						{
							if(targetUrl.IndexOf(robots.DisallowedPaths[i])!=-1)
							{
								//we found a match. It is therefore not allowed to crawl targetUrl
								retVal = true;
								break; //stop searching as soon as we have a match
							}
						}
					}
				}
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("RobotsFilter failed to filter " + targetUrl + ": " + e.ToString());
				}
			}
			finally
			{
				mutex.ReleaseMutex();
			}
			return retVal;
		}

		/// <summary>
		/// Stores the entries contained in the internal Hashtable in serialized form in a
		/// file on permanent storage (disk). It is thread-safe.
		/// </summary>
		public void SaveEntries()
		{
			try
			{
				mutex.WaitOne();
				Stream WriteStream=File.Open(FileName, FileMode.Create);
				SoapFormatter serializer=new SoapFormatter();
				serializer.Serialize(WriteStream,robotsTable);
				WriteStream.Close();
				serializer = null;
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("RobotsFilter failed to store the cache: " + e.ToString());
				}	
			}
			finally
			{
				GC.Collect();
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Loads the internal Hashtable from the cache of robots.txt files on disk and
		/// removes the expired entries. It is thread-safe.
		/// </summary>
		public void LoadEntries()
		{
			try
			{
				mutex.WaitOne();
				if(!File.Exists(FileName))
				{
					//perhaps the file does not exist - probably because it has not been
					//created yet. In this case just let the class retain default values.
					return;
				}
				Stream ReadStream = File.Open(FileName, FileMode.Open);
				SoapFormatter serializer = new SoapFormatter();
				robotsTable = (Dictionary<string,RobotsTxtEntry>)serializer.Deserialize(ReadStream);
				ReadStream.Close();
				serializer = null;
				//When enumerating through the Hashtable entries it is not allowed to remove
				//items, so a new ArrayList is used to hold the keys of the expired entries
				//which will be removed from the Hashtable once the enumeration is complete.
				List<string> expiredEntries = new List<string>();
				lock(robotsTable)
				{
					//find the expired entries
					foreach(KeyValuePair<string, RobotsTxtEntry> kvp in robotsTable)
					{
						if(kvp.Value != null)
						{
							if(kvp.Value.ExpirationDate < DateTime.Today)
							{
								expiredEntries.Add(kvp.Key);
							}
						}
					}
					//remove the expired entries
					foreach(string key in expiredEntries)
					{
						robotsTable.Remove(key);
					}
				}
				expiredEntries.Clear();
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("RobotsFilter failed to load the cache: " + e.ToString());
				}
			}
			finally
			{
				GC.Collect();
				mutex.ReleaseMutex();
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Downloads the robots.txt file from a host and returns its contents as a string.
		/// </summary>
		/// <param name="hostname">The host from where the robots.txt will be downloaded.</param>
		/// <returns>A string containing the contents of the robots.txt file if it exists,
		/// an empty string if it doesn't exist (an HTTP Status Code 404 is returned) or a
		/// null reference if something else goes wrong.</returns>
		/// <remarks>
		/// DownloadRobots does not follow redirections by default, because that might lead
		/// to a deadlock. For example, if the crawler wants to visit http://a.host/a.html
		/// and the robots.txt for that host is not available the RobotsFilter will attempt
		/// to download http://a.host/robots.txt. If this url redirects the RobotsFilter to
		/// a.html then there is a deadlock, because both robots.txt and a.html will have to
		/// wait for one another.
		/// </remarks>
		private string DownloadRobots(string hostname)
		{
			string retVal = null;
			try
			{
				HttpWebRequest robotsRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + hostname + "/robots.txt");
				robotsRequest.UserAgent = globals.UserAgent;
				robotsRequest.Timeout=15000;
				robotsRequest.AllowAutoRedirect = false;
				HttpWebResponse robotsResponse = null;
				try
				{
					robotsResponse = (HttpWebResponse)robotsRequest.GetResponse();
				}
				catch(WebException we) //either WebException
				{
					HttpWebResponse response=(HttpWebResponse)we.Response;
					if (response!=null)
					{
						//although an exception occured we're able to get the Status Code
						if(response.StatusCode == HttpStatusCode.NotFound)
						{
							retVal = String.Empty;
						}
						response.Close();
					}
				}
				if (robotsResponse!=null)
				{
					Stream receiveStream = robotsResponse.GetResponseStream();
					StreamReader readStream = new StreamReader( receiveStream, encoding );
					try
					{
						retVal = readStream.ReadToEnd();
					}
					finally
					{
						robotsResponse.Close();
						receiveStream.Close();
						readStream.Close();
					}
					//we now have the contents of the robots.txt
				}
			}
			catch(Exception)
			{
				retVal = null;
			}
			finally
			{
				GC.Collect();
			}
			return retVal;
		}

		/// <summary>
		/// Downloads a robots.txt from a specified host, parses it and constructs a new
		/// <see cref="RobotsTxtEntry"/> object with the entries of the downloaded file.
		/// </summary>
		/// <param name="hostname">The host for which the robots.txt file is required.</param>
		/// <returns>A new <see cref="RobotsTxtEntry"/> object based on the newly downloaded
		/// robots.txt file.</returns>
		private RobotsTxtEntry FetchRobots(string hostname)
		{
			RobotsTxtEntry retVal = new RobotsTxtEntry();
			try
			{
				string robots = DownloadRobots(hostname);
				if(robots!=null)
				{
					if(robots == String.Empty)
					{
						retVal.DisallowedPaths = new string [0];
					}
					else
					{
						retVal.DisallowedPaths = ParseRobots(robots);
					}
				}
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Parses a string (the contents robots.txt file) and creates a list of all the
		/// disallowed paths for 2 types of crawlers: * (every crawler) and CrawlWave.
		/// </summary>
		/// <param name="contents">The contents of the robots.txt file to be parsed.</param>
		/// <returns>An array of strings, each containing a disallowed path.</returns>
		private string [] ParseRobots(string contents)
		{
			List<string> paths = new List<string>();
			if (contents.Length>0)
			{
				int startpos=0, i=0;
				string []disallowedpaths;
				string currentpath;
				//split	the string into an array of strings, line by line
				disallowedpaths=contents.Split('\n');
				for (i=0; i<disallowedpaths.Length; i++)
				{
					if (disallowedpaths[i].StartsWith(userAgent[0])) //it is a User-agent line
					{
						while (!(disallowedpaths[i].StartsWith(userAgent[1])||(disallowedpaths[i].StartsWith(userAgent[2]))))
						{
							i++;//skip to the next line
							if(i==disallowedpaths.Length) break;
						}
					}
					else
					{
						startpos = disallowedpaths[i].IndexOf(disallow);
						if (startpos!=-1) //if it is a Disallow: entry
						{
							//clean-up the path and insert it in the list
							currentpath=disallowedpaths[i].Substring(10).Trim(' ', '\t', '\r');
							startpos = currentpath.IndexOf('#');
							if(startpos!=-1)
							{
								currentpath = currentpath.Substring(0,startpos).Trim();
							}
							if(!paths.Contains(currentpath))
							{
                                paths.Add(currentpath);
							}
						}
					}
				}
			}
			return paths.ToArray();
		}

		/// <summary>
		/// Concatenates an array of strings each containing a disallowed path into a string
		/// separated by spaces.
		/// </summary>
		/// <param name="paths">An array of strings each containing a disallowed path.</param>
		/// <returns>A string containing all the paths concatenated and separated by spaces.</returns>
		private string ConcatenatePaths(string [] paths)
		{
			if((paths==null)||(paths.Length==0))
			{
				return String.Empty;
			}
			else
			{
				StringBuilder sb = new StringBuilder(paths[0]);
				for(int i=1; i<paths.Length; i++)
				{
					sb.Append(' ');
					sb.Append(paths[i]);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Does the exact opposite from what <see cref="ConcatenatePaths"/> does.
		/// </summary>
		/// <param name="paths">A string containing disallowed paths separated by spaces</param>
		/// <returns>An array of strings, each containing one disallowed path.</returns>
		private string [] SplitPaths(string paths)
		{
			string [] retVal = new string [0];
			if(paths!=String.Empty)
			{
				retVal = paths.Split(' ');
			}
			return retVal;
		}

		#endregion
	}
}
