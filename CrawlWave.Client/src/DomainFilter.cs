using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using CrawlWave.Common;

namespace CrawlWave.Client
{
	/// <summary>
	/// DomainFilter is a Singleton class that performs filtering of Urls according to their
	/// country domain.
	/// </summary>
	/// <remarks>
	/// It is very difficult to determine the domain of origin of a Url when it doesn't have
	/// a host name but an IP Address instead. This is why an <see cref="IPCountryTable"/> 
	/// is used. This causes an extra 3.5MB of memory consumption but allows for extremely 
	/// fast (&gt;100000/sec) ISO country code lookups. It only works for IPv4 addresses.
	/// </remarks>
	public class DomainFilter
	{
		#region Private variables

		private static DomainFilter instance;	//The single class instance
		private Mutex mutex;			//Mutex supporting safe access from multiple threads
		private IPCountryTable ipTable; //The IPCountryTable holding the IP addresses info.
		private Regex ipAddressRegex;	//A Regular Expression to match IP v4 Addresses
		private static string [] FileNames; //The name of the files containing the IP entries
		private Globals globals; //Provides access to the global variables and application settings

		#endregion

		#region Constructor and Singleton Instance Members

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private DomainFilter()
		{
			//Initialize the synchronization mechanism
			mutex=new Mutex();
			//Initialize the storage for the IP Addresses
			ipTable = new IPCountryTable(16); //keyLength of 16 will create 65536 root nodes
			//Initialize the various strings. Interning them saves us a little memory.
			FileNames = new string [] {String.Intern("apnic.latest"), String.Intern("arin.latest"), String.Intern("lacnic.latest"), String.Intern("ripencc.latest"),};
			//initialize the regular expression
			ipAddressRegex = new Regex(@"^(?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}$",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			//For IPv6 addresses the following pattern can be used:
			// ^(([\dA-Fa-f]{1,4}:){7}[\dA-Fa-f]{1,4})(:([\d]{1,3}.){3}[\d]{1,3})?$
			// and the input length must be between 16 and 39 characters
			//Get a reference to the global variables and application settings
			globals = Globals.Instance();
			//Load the IP Address tables into the storage
			LoadIPAddresses();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="DomainFilter"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="DomainFilter"/>.</returns>
		public static DomainFilter Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new DomainFilter();
				}
				imutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public interface methods

		/// <summary>
		/// Checks if a Url belongs to the part of the web we wish to crawl.
		/// </summary>
		/// <param name="targetUrl">The url to examine</param>
		/// <returns>
		/// A <see cref="DomainFlagValue"/> indicating whether the input Url belongs to the
		/// part of the web we wish to crawl.
		/// </returns>
		/// <remarks>
		/// Since it is possible for a url that belongs to a non-greek domain (e.g. .com) to
		/// contain greek content, all Urls that do not belong to the .gr domain will get the
		/// value of <see cref="DomainFlagValue.Unknown"/>. This allows the system to assign
		/// them at a later time to a client who will visit them and check their content-type
		/// encoding, in order to determine whether they are of interest to the system.
		/// </remarks>
		public DomainFlagValue FilterUrl(ref string targetUrl)
		{
			DomainFlagValue retVal = DomainFlagValue.Unknown;
			try
			{
				mutex.WaitOne();
				string targetHost = InternetUtils.HostName(targetUrl);
				if(targetHost.EndsWith(".gr") || targetHost.Contains("ebay.com"))
				{
					retVal = DomainFlagValue.MustVisit;
				}
				else
				{
					if(IsIPAddress(ref targetHost))
					{
						if(ipTable.GetCountry(targetHost)=="GR")
						{
							retVal = DomainFlagValue.MustVisit;
						}
					}
				}
			}
			catch(Exception e)
			{
				globals.FileLog.LogWarning(e.ToString());
			}
			finally
			{
				mutex.ReleaseMutex();
			}
			return retVal;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Loads the IP addresses from the APNIC, ARIN, LACNIC and RIPENCC registry files.
		/// If the files are more than 1 month old then a fresh copy is downloaded.
		/// </summary>
		private void LoadIPAddresses()
		{
			//Check if the files are too old.
			try
			{
				if(File.Exists(globals.AppDataPath + FileNames[0]))
				{
					if(File.GetLastWriteTime(globals.AppDataPath + FileNames[0]).AddMonths(1)<DateTime.Now)
					{
						//They are more than a month old, we need to download a fresh copy.
						InternetUtils.UpdateIPAddressFiles(globals.AppDataPath);
					}
				}
			}
			catch(Exception e)
			{
				globals.FileLog.LogWarning(e.ToString());
			}
			//Load the files into the cache. [just from ripe]
			for(int i = 3; i< FileNames.Length; i++)
			{
				try
				{
					ipTable.LoadStatisticsFile(globals.AppDataPath + FileNames[i],true);
				}
				catch
				{
					globals.FileLog.LogWarning("DomainFilter failed to load IP addresses from " + FileNames[i]);
					continue;
				}
			}
			GC.Collect();
		}

		/// <summary>
		/// Checks if a string contains a valid IPv4 or IPv6 Address
		/// </summary>
		/// <param name="address">The string to check</param>
		/// <returns>True if it is a valid IP address, false otherwise.</returns>
		private bool IsIPAddress(ref string address)
		{
			try
			{
				if(ipAddressRegex.IsMatch(address))
				{
					IPAddress test = IPAddress.Parse(address);
					//if the previous line does't throw an exception then it's an IP Address
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false; //it's not an IP Address
			}
		}

		#endregion
	}
}
