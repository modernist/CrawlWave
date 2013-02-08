using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerPlugins.WordExtraction
{
	/// <summary>
	/// WordsCache is a Singleton class that holds a cache of all the words (actually their
	/// stems) that have been extracted from all the pages stored in the database. This way
	/// it allows easy lookup of word ids and decreases the number of queries that have to
	/// be executed on the system's database drastically. It provides methods to add and 
	/// remove entries to the cache, as well as events for easy client notification.
	/// </summary>
	public class WordsCache
	{
		#region Private variables

		private static WordsCache instance;
		private PluginSettings settings;
		private DBConnectionStringProvider dbProvider;
		private SqlConnection dbcon;
		private Hashtable words;
		private Stemming stemming;
		private CultureInfo culture;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private WordsCache()
		{
			settings = PluginSettings.Instance();
			dbProvider = DBConnectionStringProvider.Instance();
			settings.DBConnectionString = dbProvider.ProvideDBConnectionString("CrawlWave.ServerPlugins.WordExtraction");
			dbcon = new SqlConnection(settings.DBConnectionString);
			words = new Hashtable();
			stemming = Stemming.Instance();
			culture = new CultureInfo("el-GR");
			LoadCache();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="WordsCache"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="WordsCache"/>.</returns>
		public static WordsCache Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe.
				Mutex mutex=new Mutex();
				mutex.WaitOne();
				if( instance == null )
				{
					instance = new WordsCache();
				}
				mutex.Close();
			}
			return instance;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs when an update is performed on  the cache (a word is added or removed).
		/// </summary>
		public event EventHandler CacheUpdated;

		#endregion

		#region Public Interface methods

		/// <summary>
		/// Adds a new word to the cache
		/// </summary>
		/// <param name="word">A string containing the word to add to the cache.</param>
		public void AddWord(string word)
		{
			string key = stemming.StemWord(CapitalizeString(word));
			lock(words)
			{
				if(!words.ContainsKey(key))
				{
					int word_id = InsertWord(key);
					if(word_id !=-1)
					{
						words.Add(key, word_id);
						OnCacheUpdated(EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Adds a new word that has already been capitalized and stemmed to the cache
		/// </summary>
		/// <param name="word">A string containing the word to add to the cache.</param>
		public void AddStemmedWord(string word)
		{
			lock(words)
			{
				if(!words.ContainsKey(word))
				{
					int word_id = InsertWord(word);
					if(word_id !=-1)
					{
						words.Add(word, word_id);
						OnCacheUpdated(EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Removes a word from the cache.
		/// </summary>
		/// <param name="word">A string containing the word to remove from the cache.</param>
		public void RemoveWord(string word)
		{
			string key = stemming.StemWord(CapitalizeString(word));
			lock(words)
			{
				if(words.ContainsKey(key))
				{
					words.Remove(key);
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Removes a capitalized and stemmed word from the cache.
		/// </summary>
		/// <param name="word">The word to remove from the cache.</param>
		public void RemoveStemmedWord(string word)
		{
			lock(words)
			{
				if(words.ContainsKey(word))
				{
					words.Remove(word);
					OnCacheUpdated(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Indexer property, returns the id of a capitalized and stemmed word or -1 if not found
		/// </summary>
		public int this[string word]
		{
			get
			{
				if(words.ContainsKey(word))
				{
					return (int)words[word];
				}
				return -1;
			}
		}

		#endregion

		#region Private methods

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
				SqlCommand cmd = new SqlCommand("cw_select_words",dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				string word = String.Empty;
				int word_id = -1;
				words.Clear();
				foreach(DataRow dr in ds.Tables[0].Rows)
				{
					word = (string)dr[0];
					word_id = (int)dr[1];
					if(!words.ContainsKey(word))
					{
						words.Add(word, word_id);
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

		private int InsertWord(string word)
		{
			int retVal = -1;
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
					return -1;
				}
				SqlCommand cmd = new SqlCommand("cw_insert_word",dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@cwwo_word", SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@cwwo_word_id", SqlDbType.Int);
				cmd.Parameters[1].Direction = ParameterDirection.Output;
				cmd.Parameters[0].Value = word;
				cmd.ExecuteNonQuery();
				retVal = (int)cmd.Parameters[1].Value;
				cmd.Dispose();
				dbcon.Close();
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
			return retVal;
		}

		/// <summary>
		/// Converts a string to its capitalized form.
		/// </summary>
		/// <param name="strToCapitalize">The string to capitalize</param>
		/// <returns>The capitalized string</returns>
		private string CapitalizeString(string strToCapitalize)
		{
			StringBuilder sb=new StringBuilder(strToCapitalize.ToUpper(culture));
			sb.Replace('¢','Á');
			sb.Replace('¸','Å');
			sb.Replace('¹','Ç');
			sb.Replace('º','É');
			sb.Replace('Ú','É');
			sb.Replace('¼','Ï');
			sb.Replace('¾','Õ');
			sb.Replace('Û','Õ');
			sb.Replace('¿','Ù');
			sb.Replace('¹','Ç');
			sb.Replace('À','É');
			sb.Replace('à','Õ');
			return sb.ToString();
		}

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

		#endregion

	}
}
