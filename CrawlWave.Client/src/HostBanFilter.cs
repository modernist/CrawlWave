using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Web.Services;
//using Microsoft.Web.Services2;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.Client
{
	/// <summary>
	/// HostbanFilter is a Singleton class that performs filtering of Urls according to a 
	/// list of banned hosts that is maintained in the system's database. If a host is in
	/// the ban list then no requests at all must be made to him, not even for robots.txt
	/// files, so the <see cref="Parser"/>s consult the <see cref="HostBanFilter"/> first.
	/// </summary>
	public class HostBanFilter
	{
		#region Private variables

		private static HostBanFilter instance;	//The single class instance
		private Hashtable hostTable; //The Hashtable that will hold the entries
		//private WebServiceProxy proxy;// Provides access to the CrawlWave Server Web Sevice
		private ICrawlWaveServer proxy;
		private Globals globals; //Provides access to the global variables and application settings

		#endregion

		#region Constructor and Singleton Instance Members

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private HostBanFilter()
		{
			//Initialize the storage for the banned host entries
			hostTable = new Hashtable();
			//Get a reference to the global variables and application settings
			globals = Globals.Instance();
			//Initialize the list of banned hosts
			//proxy = WebServiceProxy.Instance();
			proxy = CrawlWaveServerProxy.Instance(globals);
			InitializeBannedHosts();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="HostBanFilter"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="HostBanFilter"/>.</returns>
		public static HostBanFilter Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new HostBanFilter();
				}
				imutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Interface members

		/// <summary>
		/// Checks if a host belongs to the list of banned hosts.
		/// </summary>
		/// <param name="hostName">The address of the host to check.</param>
		/// <returns>True if the host is banned, false otherwise.</returns>
		public bool FilterHost(ref string hostName)
		{
			bool retVal = false;
			if(hostTable.ContainsKey(hostName))
			{
				retVal = true;
			}
			return retVal;
		}

		/// <summary>
		/// Checks if a url is served by a host that belongs to the list of banned hosts.
		/// </summary>
		/// <param name="targetUrl">A <see cref="InternetUrlToIndex"/> to check.</param>
		/// <returns>True if the host is banned, false otherwise.</returns>
		public bool FilterUrl(ref InternetUrlToIndex targetUrl)
		{
			string hostName = InternetUtils.HostName(targetUrl);
			return FilterHost(ref hostName);
		}

		/// <summary>
		/// Checks if a url is served by a host that belongs to the list of banned hosts.
		/// </summary>
		/// <param name="targetUrl">A <see cref="InternetUrlToCrawl"/> to check.</param>
		/// <returns>True if the host is banned, false otherwise.</returns>
		public bool FilterUrl(ref InternetUrlToCrawl targetUrl)
		{
			string hostName = InternetUtils.HostName(targetUrl);
			return FilterHost(ref hostName);
		}

		/// <summary>
		/// Checks if a url is served by a host that belongs to the list of banned hosts.
		/// </summary>
		/// <param name="targetUrl">A <see cref="InternetUrl"/> to check.</param>
		/// <returns>True if the host is banned, false otherwise.</returns>
		public bool FilterUrl(ref InternetUrl targetUrl)
		{
			string hostName = InternetUtils.HostName(targetUrl);
			return FilterHost(ref hostName);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Clears the banned hosts list and initializes it with the latest version.
		/// </summary>
		private void InitializeBannedHosts()
		{
			try
			{
				ICrawlWaveServer server = proxy;
				try
				{
					server.IsAlive();
				}
				catch
				{
					if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
					{
						globals.FileLog.LogWarning("HostBanFilter failed to connect to the server.");
					}
					return;
				}
				DataSet ds = new DataSet();
				SerializedException sx = server.SendBannedHosts(globals.Client_Info, out ds);
				if(sx!=null)
				{
					if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
					{
						globals.FileLog.LogWarning("HostBanFilter failed to download a list of banned hosts.");
					}
					return;
				}
				else
				{
					if(ds.Tables[0].Rows.Count>0)
					{
						lock(hostTable.SyncRoot)
						{
							hostTable.Clear();
							foreach(DataRow dr in ds.Tables[0].Rows)
							{
                                Guid g = (Guid)(dr[0]);
								try
								{
									hostTable.Add(g.ToByteArray(), null);
								}
								catch
								{
									continue;
								}
							}
						}
					}
				}
				ds.Dispose();
			}
			catch
			{}
		}

		#endregion

	}
}
