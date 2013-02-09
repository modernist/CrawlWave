using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CrawlWave.Common;

namespace CrawlWave.Client
{
	/// <summary>
	/// HostRequestFilter is a Singleton class that performs synchronization of the crawler
	/// threads by monitoring the requests that are targeted at the hosts and not allowing
	/// the crawler to slammer a host.
	/// </summary>
	/// <remarks>
	/// HostRequestFilter maintains a <see cref="Hashtable"/> of <see cref="HostRequestFilterEntry"/>
	/// objects and each time a crawler thread attempts to visit a page it requests permission
	/// from this class. Its mission is to avoid slammering a single host with multiple web
	/// requests or very frequent requests. A single host must be visited at most once every
	/// 30 seconds (robots.txt requests are not taken into account, since they will be very
	/// infrequent). This class monitors the requests to hosts and either asks the crawlers
	/// to wait until 30 seconds have passed since the last visit to the host or gives them
	/// permission to go ahead, and cleans up the internal Hashtable from expired entries.
	/// </remarks>
	public class HostRequestFilter
	{
		#region Private variables

		private static HostRequestFilter instance;	//The single class instance
		private Dictionary<string, HostRequestFilterEntry> hostTable;//The Hashtable that will hold the entries
		private Mutex mutex;		//Mutex supporting safe access from multiple threads
		//private Thread cleanupThread; //The thread that will be performing cleanup in the background
		private Globals globals; //Provides access to the global variables and application settings

		#endregion

		#region Constructor and Singleton Instance Members

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private HostRequestFilter()
		{
			//Initialize the synchronization mechanism
			mutex=new Mutex();
			//Initialize the storage for the HostRequestFilterEntry objects
			hostTable = new Dictionary<string, HostRequestFilterEntry>(128);
			//Get a reference to the global variables and application settings
			globals = Globals.Instance();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="HostRequestFilter"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="HostRequestFilter"/>.</returns>
		public static HostRequestFilter Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new HostRequestFilter();
				}
				imutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Interface methods

		/// <summary>
		/// Checks if at least 30 seconds have passed since the last request to a given host
		/// was made, in order not to slammer it with simultaneous or frequent requests.
		/// </summary>
		/// <param name="hostName">The host name from which a request will be made.</param>
		/// <returns>
		/// An integer containing the number of milliseconds a crawler thread must wait 
		/// before visiting this host.
		/// </returns>
		public int FilterHost(ref string hostName)
		{
			int retVal = 0;
			try
			{
				mutex.WaitOne();
				HostRequestFilterEntry hostEntry;
				if(hostTable.TryGetValue(hostName, out hostEntry))
				{
					if (hostName.Contains("ebay.com"))
						return 5000;

					if(hostEntry.ExpirationDate < DateTime.Now)
					{
						//the entry has expired, so we can visit the host and we must
						//update the entry
						hostTable[hostName].ExpirationDate = DateTime.Now.AddMilliseconds(Backoff.DefaultBackoff);
						retVal = 0;
					}
					else
					{
						//we must update the entry and calculate the appropriate delay
						retVal = ((TimeSpan)hostEntry.ExpirationDate.Subtract(DateTime.Now)).Milliseconds;
						hostTable[hostName].ExpirationDate.AddMilliseconds(Backoff.DefaultBackoff);
						hostTable[hostName].Count++;
					}
				}
				else
				{
					//create a new entry with the default timeout
					hostEntry = new HostRequestFilterEntry();
					hostTable.Add(hostName, hostEntry);
					retVal = 0;
				}
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo("HostRequestFilter: " + e.ToString());
				}
			}
			finally
			{
				mutex.ReleaseMutex();
			}
			return retVal;
		}

		/// <summary>
		/// Checks if at least 30 seconds have passed since the last request to a given host
		/// was made, in order not to slammer it with simultaneous or frequent requests.
		/// </summary>
		/// <param name="targetUrl">
		/// A <see cref="InternetUrl"/> that is served by a host we wish to check.
		/// </param>
		/// <returns>
		/// An integer containing the number of milliseconds a crawler thread must wait
		/// before visiting this host.
		/// </returns>
		public int FilterUrl(ref InternetUrl targetUrl)
		{
			string hostName = InternetUtils.HostName(targetUrl);
			return FilterHost(ref hostName);
		}

		/// <summary>
		/// Checks if at least 30 seconds have passed since the last request to a given host
		/// was made, in order not to slammer it with simultaneous or frequent requests.
		/// </summary>
		/// <param name="targetUrl">
		/// A <see cref="InternetUrlToCrawl"/> that is served by a host we wish to check.
		/// </param>
		/// <returns>
		/// An integer containing the number of milliseconds a crawler thread must wait
		/// before visiting this host.
		/// </returns>
		public int FilterUrl(ref InternetUrlToCrawl targetUrl)
		{
			string hostName = InternetUtils.HostName(targetUrl);
			return FilterHost(ref hostName);
		}

		/// <summary>
		/// Checks if at least 30 seconds have passed since the last request to a given host
		/// was made, in order not to slammer it with simultaneous or frequent requests.
		/// </summary>
		/// <param name="targetUrl">
		/// A <see cref="InternetUrlToIndex"/> that is served by a host we wish to check.
		/// </param>
		/// <returns>
		/// An integer containing the number of milliseconds a crawler thread must wait
		/// before visiting this host.
		/// </returns>
		public int FilterUrl(ref InternetUrlToIndex targetUrl)
		{
			string hostName = InternetUtils.HostName(targetUrl);
			return FilterHost(ref hostName);
		}

		/// <summary>
		/// Checks if at least 30 seconds have passed since the last request to a given host
		/// was made, in order not to slammer it with simultaneous or frequent requests.
		/// </summary>
		/// <param name="targetUrl">A url that is served by a host we wish to check</param>
		/// <returns>An integer containing the number of milliseconds a crawler thread must wait
		/// before visiting this host.</returns>
		public int FilterUrl(ref string targetUrl)
		{
			string hostName = InternetUtils.HostName(targetUrl);
			return FilterHost(ref hostName);
		}

		/// <summary>
		/// Clears the <see cref="HostRequestFilter"/> from expired <see cref="HostRequestFilterEntry"/>
		/// objects. It must not be used when new entries might be added to the filter.
		/// </summary>
		public void Clear()
		{
			try
			{
				lock(hostTable)
				{
					//When enumerating through the Hashtable entries it is not allowed to remove
					//items, so a new ArrayList is used to hold the keys of the expired entries
					//which will be removed from the Hashtable once the enumeration is complete.
					List<string> expiredEntries = new List<string>();
					foreach(KeyValuePair<string, HostRequestFilterEntry> kvp in hostTable)
					{
						try
						{
							if(kvp.Value.ExpirationDate < DateTime.Now)
							{
								expiredEntries.Add(kvp.Key);
							}
						}
						catch
						{
							continue;
						}
					}
					foreach(string key in expiredEntries)
					{
						try
						{
							hostTable.Remove(key);
						}
						catch
						{
							continue;
						}
					}
					expiredEntries.Clear();
				}
			}
			catch
			{}
			finally
			{
				GC.Collect(); //this will clean up the memory used by the deleted entries
			}
		}

		#endregion
	}
}
