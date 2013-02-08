using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using CrawlWave.Common;

namespace CrawlWave.ServerCommon
{
	/// <summary>
	/// RobotsCache is a Singleton class that holds a cache of the robots.txt files for all
	/// hosts. This way it allows easy access to the list of robots files to all the plugins
	/// that need it. It provides methods to add and remove entries to the cache, as well as
	/// events for easy client notification.
	/// </summary>
	public class RobotsCache
	{
		#region Private variables

		private static RobotsCache instance;
		private DBConnectionStringProvider dbProvider;
		private string connectionString;
		private SqlConnection dbcon;
		private Dictionary<byte[], RobotsTxtEntry> hosts;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private RobotsCache()
		{
			dbProvider = DBConnectionStringProvider.Instance();
			connectionString = dbProvider.ProvideDBConnectionString("CrawlWave.ServerCommon.RobotsCache");
			dbcon = new SqlConnection(connectionString);
			hosts = new Dictionary<byte[], RobotsTxtEntry>();
			LoadCache();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="RobotsCache"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="RobotsCache"/>.</returns>
		public static RobotsCache Instance()
		{
			if (instance == null)
			{
				//Make sure the call is thread-safe.
				Mutex mutex = new Mutex();
				mutex.WaitOne();
				if (instance == null)
				{
					instance = new RobotsCache();
				}
				mutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs when new entries about robots.txt files are added to the cache.
		/// </summary>
		public event EventHandler CacheUpdated;

		#endregion

		#region Public Properties

		/// <summary>
		/// Sets the Database Connection string to be used in case the default Connection
		/// String provided by DBConnectionStringProvider can or must not be used.
		/// </summary>
		public string DBConnectionString
		{
			set
			{
				connectionString = value;
				dbcon = new SqlConnection(connectionString);
				hosts.Clear();
				LoadCache();
			}
		}

		#endregion

		#region Public Interface methods

		/// <summary>
		/// Adds a new robots entry to the cache
		/// </summary>
		/// <param name="hostName">A string containing the hostname to add to the cache.</param>
		/// <param name="entry">The <see cref="RobotsTxtEntry"/> related to the host.</param>
		public void AddEntry(string hostName, RobotsTxtEntry entry)
		{
			byte[] key = MD5Hash.md5(hostName);
			lock (hosts)
			{
				if (!hosts.ContainsKey(key))
				{
					hosts.Add(key, entry);
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Adds a new robots entry to the cache
		/// </summary>
		/// <param name="hostNameMD5">The md5 of the hostname to add to the cache.</param>
		/// <param name="entry">The <see cref="RobotsTxtEntry"/> related to the host.</param>
		public void AddEntry(byte[] hostNameMD5, RobotsTxtEntry entry)
		{
			if (hostNameMD5.Length != 16)
			{
				throw new ArgumentException("Only 16 byte keys are accepted.");
			}
			lock (hosts)
			{
				if (!hosts.ContainsKey(hostNameMD5))
				{
					hosts.Add(hostNameMD5, entry);
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Removes a robots entry from the cache.
		/// </summary>
		/// <param name="hostName">A string containing the hostname to remove from the cache.</param>
		public void RemoveEntry(string hostName)
		{
			byte[] key = MD5Hash.md5(hostName);
			lock (hosts)
			{
				if (hosts.Remove(key))
				{
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Removes a robots entry from the cache.
		/// </summary>
		/// <param name="hostNameMD5">The md5 of the hostname to remove from the cache.</param>
		public void RemoveEntry(byte[] hostNameMD5)
		{
			if (hostNameMD5.Length != 16)
			{
				throw new ArgumentException("Only 16 byte keys are accepted.");
			}
			lock (hosts)
			{
				if (hosts.Remove(hostNameMD5))
				{
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Indexer property
		/// </summary>
		public RobotsTxtEntry this[string hostName]
		{
			get
			{
				byte[] key = MD5Hash.md5(hostName);
				RobotsTxtEntry entry = null;
				hosts.TryGetValue(key, out entry);
				return entry;
			}
		}

		/// <summary>
		/// Indexer property
		/// </summary>
		public RobotsTxtEntry this[byte[] hostNameMD5]
		{
			get
			{
				if (hostNameMD5.Length != 16)
				{
					throw new ArgumentException("Only 16 byte keys are accepted.");
				}
				RobotsTxtEntry entry = null;
				hosts.TryGetValue(hostNameMD5, out entry);
				return entry;
			}
		}

		/// <summary>
		/// Works the same way as the indexer but if the requested entry is not found in the
		/// cache then it is requested from the database again before returning null.
		/// </summary>
		/// <param name="hostID">The ID of the host (md5 hash of the hostname).</param>
		public RobotsTxtEntry GetEntry(byte[] hostID)
		{
			if (hostID.Length != 16)
			{
				throw new ArgumentException("Only 16 byte keys accepted.");
			}
			RobotsTxtEntry entry = null;
			if (hosts.TryGetValue(hostID, out entry))
			{
				return entry;
			}
			try
			{
				try
				{
					dbcon.Open();
				}
				catch
				{ }
				if (dbcon.State == ConnectionState.Closed)
				{
					//log a message
					return null;
				}
				SqlCommand cmd = new SqlCommand("cw_select_robot", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
				cmd.Parameters[0].Value = new Guid(hostID);
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				if (ds.Tables[0].Rows.Count == 0)
				{
					return null;
				}
				entry = new RobotsTxtEntry();
				entry.DisallowedPaths = SplitPaths((string)ds.Tables[0].Rows[0][1]);
				entry.ExpirationDate = (DateTime)ds.Tables[0].Rows[0][2];
				hosts.Add(hostID, entry);
				ds.Dispose();
				OnCacheUpdated(EventArgs.Empty);
				return entry;
			}
			catch
			{
				if (dbcon.State != ConnectionState.Closed)
				{
					try
					{
						dbcon.Close();
					}
					catch
					{ }
				}
				GC.Collect();
				return null;
			}
		}

		/// <summary>
		/// Works the same way as the indexer but if the requested entry is not found in the
		/// cache then it is requested from the database again before returning null.
		/// </summary>
		/// <param name="hostName">The  host name.</param>
		public RobotsTxtEntry GetEntry(string hostName)
		{
			byte[] key = MD5Hash.md5(hostName);
			RobotsTxtEntry entry = null;
			if (hosts.TryGetValue(key, out entry))
			{
				return entry;
			}
			try
			{
				try
				{
					dbcon.Open();
				}
				catch
				{ }
				if (dbcon.State == ConnectionState.Closed)
				{
					//log a message
					return null;
				}
				SqlCommand cmd = new SqlCommand("cw_select_robot", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
				cmd.Parameters[0].Value = new Guid(key);
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				if (ds.Tables[0].Rows.Count == 0)
				{
					return null;
				}
				entry = new RobotsTxtEntry();
				entry.DisallowedPaths = SplitPaths((string)ds.Tables[0].Rows[0][1]);
				entry.ExpirationDate = (DateTime)ds.Tables[0].Rows[0][2];
				hosts.Add(key, entry);
				ds.Dispose();
				OnCacheUpdated(EventArgs.Empty);
				return entry;
			}
			catch
			{
				if (dbcon.State != ConnectionState.Closed)
				{
					try
					{
						dbcon.Close();
					}
					catch
					{ }
				}
				GC.Collect();
				return null;
			}
		}

		/// <summary>
		/// Loads the cache with the banned host entries stored in the database.
		/// </summary>
		public void LoadCache()
		{
			try
			{
				try
				{
					dbcon.Open();
				}
				catch
				{ }
				if (dbcon.State == ConnectionState.Closed)
				{
					//log a message
					return;
				}
				SqlCommand cmd = new SqlCommand("cw_select_robots", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				hosts.Clear();
				byte[] key;
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					key = ((Guid)dr[0]).ToByteArray();
					RobotsTxtEntry entry = new RobotsTxtEntry((DateTime)dr[2], SplitPaths((string)dr[1]));
					hosts.Add(key, entry); //null? WTF!!! plus no existence check is required
				}
				ds.Dispose();
				OnCacheUpdated(EventArgs.Empty);
			}
			catch
			{
				if (dbcon.State != ConnectionState.Closed)
				{
					try
					{
						dbcon.Close();
					}
					catch
					{ }
				}
				GC.Collect();
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Concatenates an array of strings each containing a disallowed path into a string
		/// separated by spaces.
		/// </summary>
		/// <param name="paths">An array of strings each containing a disallowed path.</param>
		/// <returns>A string containing all the paths concatenated and separated by spaces.</returns>
		private string ConcatenatePaths(string[] paths)
		{
			if ((paths == null) || (paths.Length == 0))
			{
				return String.Empty;
			}
			else
			{
				StringBuilder sb = new StringBuilder(paths[0]);
				for (int i = 1; i < paths.Length; i++)
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
		private string[] SplitPaths(string paths)
		{
			string[] retVal = new string[0];
			if (paths != String.Empty)
			{
				retVal = paths.Split(' ');
			}
			return retVal;
		}

		/// <summary>
		/// Raises the <see cref="CacheUpdated"/> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnCacheUpdated(EventArgs e)
		{
			EventHandler handler = null;
			lock (this)
			{
				handler = CacheUpdated;
			}
			if (handler != null)
			{
				handler(this, e);
			}
		}

		#endregion
	}
}
