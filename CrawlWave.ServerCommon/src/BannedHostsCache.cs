using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using CrawlWave.Common;

namespace CrawlWave.ServerCommon
{
	/// <summary>
	/// BannedHostsCache is a Singleton class that holds a cache of the banned hosts. This
	/// way it allows easy access to the list of banned hosts to all the plugins that need
	/// it. It provides methods to add and remove entries to the cache, as well as events
	/// for easy client notification.
	/// </summary>
	public class BannedHostsCache
	{
		#region Private variables

		private static BannedHostsCache instance;
		private DBConnectionStringProvider dbProvider;
		private string connectionString;
		private SqlConnection dbcon;
		private Dictionary<byte[], string> hosts;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private BannedHostsCache()
		{
			dbProvider = DBConnectionStringProvider.Instance();
			connectionString = dbProvider.ProvideDBConnectionString("CrawlWave.ServerCommon.BannedHostsCache");
			dbcon = new SqlConnection(connectionString);
			hosts = new Dictionary<byte[], string>();
			LoadCache();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="BannedHostsCache"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="BannedHostsCache"/>.</returns>
		public static BannedHostsCache Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe.
				Mutex mutex=new Mutex();
				mutex.WaitOne();
				if( instance == null )
				{
					instance = new BannedHostsCache();
				}
				mutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs when the <see cref="BannedHostsCache"/> is updated (entries are added or removed).
		/// </summary>
		public event EventHandler CacheUpdated;

		#endregion

		#region Public Interface methods

		/// <summary>
		/// Adds a new banned host to the cache
		/// </summary>
		/// <param name="hostName">A string containing the hostname to add to the cache.</param>
		public void AddEntry(string hostName)
		{
			byte [] key = MD5Hash.md5(hostName);
			lock(hosts)
			{
				if(!hosts.ContainsKey(key))
				{
					hosts.Add(key, null);
                    OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Adds a new banned host to the cache
		/// </summary>
		/// <param name="hostNameMD5">The md5 of the hostname to add to the cache.</param>
		public void AddEntry(byte []hostNameMD5)
		{
			if(hostNameMD5.Length!=16)
			{
				throw new ArgumentException("Only 16 byte keys are accepted.");
			}
			lock(hosts)
			{
				if(!hosts.ContainsKey(hostNameMD5))
				{
					hosts.Add(hostNameMD5, null);
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Removes a banned host from the cache.
		/// </summary>
		/// <param name="hostName">A string containing the hostname to remove from the cache.</param>
		public void RemoveEntry(string hostName)
		{
			byte [] key = MD5Hash.md5(hostName);
			lock(hosts)
			{
				if(hosts.ContainsKey(key))
				{
					hosts.Remove(key);
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Removes a banned host from the cache.
		/// </summary>
		/// <param name="hostNameMD5">The md5 of the hostname to remove from the cache.</param>
		public void RemoveEntry(byte []hostNameMD5)
		{
			if(hostNameMD5.Length!=16)
			{
				throw new ArgumentException("Only 16 byte keys are accepted.");
			}
			lock(hosts)
			{
				if(hosts.ContainsKey(hostNameMD5))
				{
					hosts.Remove(hostNameMD5);
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Checks if a host is banned.
		/// </summary>
		/// <param name="hostName">The hostname to check.</param>
		/// <returns>True if the host is banned, false otherwise.</returns>
		public bool IsBanned(string hostName)
		{
			bool retVal = false;
			try
			{
				//no need to do any locking, the Hashtable supports multiple readers
				byte [] key = MD5Hash.md5(hostName);
				retVal = hosts.ContainsKey(key);
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Checks if a host is banned.
		/// </summary>
		/// <param name="hostNameMD5">The md5 of the name of the host to check.</param>
		/// <returns>True if the host is banned, false otherwise.</returns>
		public bool IsBanned(byte []hostNameMD5)
		{
			if(hostNameMD5.Length!=16)
			{
				throw new ArgumentException("Only 16 byte keys are accepted.");
			}
			bool retVal = false;
			try
			{
				//no need to do any locking, the Hashtable supports multiple readers
				retVal = hosts.ContainsKey(hostNameMD5);
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Attempts to refresh the banned hosts cache.
		/// </summary>
		public void RefreshCache()
		{
			try
			{
				lock(hosts)
				{
					//this will also clear the cache before reloading it.
					LoadCache();
				}
			}
			catch
			{
				GC.Collect();
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Raises the <see cref="CacheUpdated"/> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnCacheUpdated(EventArgs e)
		{
			EventHandler handler = null;
			lock(this)
			{
				handler = CacheUpdated;
			}
			if(handler!=null)
			{
				handler(this,e);
			}
		}

		/// <summary>
		/// Loads the cache with the banned host entries stored in the database.
		/// </summary>
		private void LoadCache()
		{
			try
			{
				try
				{
					dbcon.Open();
				}
				catch
				{}
				if(dbcon.State == ConnectionState.Closed)
				{
					//log a message
					return;
				}
				SqlCommand cmd = new SqlCommand("cw_select_banned_hosts", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				hosts.Clear();
				byte [] key;
				foreach(DataRow dr in ds.Tables[0].Rows)
				{
					key = ((Guid)dr[0]).ToByteArray();
					if(!hosts.ContainsKey(key))
					{
						hosts.Add(key,null);
					}
				}
				ds.Dispose();
				OnCacheUpdated(EventArgs.Empty);
			}
			catch
			{
				if(dbcon.State != ConnectionState.Closed)
				{
					try
					{
						dbcon.Close();
					}
					catch
					{}
				}
				GC.Collect();
			}
		}

		#endregion
	}
}
