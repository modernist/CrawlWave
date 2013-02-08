using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerPlugins.WordExtraction
{
	/// <summary>
	/// WordExtractorPlugin is an <see cref="IPlugin"/> that performs extraction of words
	/// from the contents of documents either already in the database or currently being
	/// inserted in the database.
	/// </summary>
	[CrawlWavePlugin]
	public class WordExtractorPlugin : PluginBase, IPlugin
	{
		#region Private variables

		private Mutex mutex;
		private PluginSettings settings;
		private Thread pluginThread;
		private WordExtractor wordExtractor;
		private WordsCache cache;
		private DBConnectionStringProvider dbProvider;
		private bool mustStop;
		private SqlConnection dbcon;
		private ExponentialBackoff backoff;

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new instance of the <see cref="WordExtractorPlugin"/> class.
		/// </summary>
		public WordExtractorPlugin()
		{
			mutex = new Mutex();
			settings = PluginSettings.Instance();
			name = "CrawlWave.ServerPlugins.WordExtraction";
			description = "CrawlWave Word Extraction Plugin";
			dataDependent = false;
			state = PluginState.Stopped;
			enabled = true;
			version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			percent = 0;
			mustStop = false;
			pluginThread = null;
			if(settings.UseDatabase)
			{
				dbProvider = DBConnectionStringProvider.Instance();
				settings.DBConnectionString = dbProvider.ProvideDBConnectionString(name);
				dbcon = new SqlConnection(settings.DBConnectionString);
			}
			wordExtractor = WordExtractor.Instance();
			cache = WordsCache.Instance();
			backoff = new ExponentialBackoff(BackoffSpeed.Slow, 30000);
		}

		#endregion

		#region IPlugin Members

		/// <summary>
		/// Starts the word extraction process from the visited pages. If the plugin is 
		/// already in the <see cref="PluginState.Running"/> state it has no effect.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the plugin is in the <see cref="PluginState.Paused"/> state.
		/// </exception>
		public void Start()
		{
			if(state == PluginState.Running)
			{
				return;
			}
			if(state == PluginState.Paused)
			{
				throw new InvalidOperationException("The plugin is in the Paused state and cannot be started.");
			}
			if(!settings.UseDatabase)
			{
				//Must not use the database, so we don't need to start any threads
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
				AddToReportQueue(CWLoggerEntryType.Info, "Starting Word Extraction Plugin.");
				Report();
				return;
			}
			try
			{
				mustStop = false;
				AddToReportQueue(CWLoggerEntryType.Info, "Starting Word Extraction Plugin.");
				//Initialize the connection to the database
				//InitializeResultsQueue();
				//create the thread that will be extracting words from stored documents.
				pluginThread = new Thread(new ThreadStart(PerformExtraction));
				pluginThread.IsBackground = true;
				pluginThread.Priority = ThreadPriority.Lowest;
				pluginThread.Name = "Word Extractor Plugin Thread";
				pluginThread.Start();
				//Notify the clients that the plugin's process has started
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Error, "The WordExtraction Plugin failed to start: " + ex.ToString());
				mustStop = true;
				state = PluginState.Stopped;
				try
				{
					StopThreads();
				}
				catch(Exception exc)
				{
					ReportImmediately(CWLoggerEntryType.Error, "The WordExtraction Plugin failed to stop all threads: " + exc.ToString());
				}
			}
			finally
			{
				Report();
			}
		}

		/// <summary>
		/// Pauses the word extraction process by calling <see cref="Thread.Suspend"/> on 
		/// the running threads. If the plugin is already in the <see cref="PluginState.Paused"/>
		/// state it has no effect.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the plugin is in the <see cref="PluginState.Stopped"/> state.
		/// </exception>
		public void Pause()
		{
			if(state == PluginState.Paused)
			{
				return;
			}
			if(state == PluginState.Stopped)
			{
				throw new InvalidOperationException("The plugin is in the Stopped state and cannot be paused.");
			}
			if(!settings.UseDatabase)
			{
				//Must not use the database, so we don't need to suspend any threads
				state = PluginState.Paused;
				OnStateChanged(EventArgs.Empty);
				return;
			}
			try
			{
				state = PluginState.Paused;
				SuspendThreads();
				OnStateChanged(EventArgs.Empty);
			}
			catch
			{
				//if something goes wrong return to the Running state
				state = PluginState.Running;
			}
			finally
			{
				//hmmm....
			}
		}

		/// <summary>
		/// Resumes the word extraction process if it has been paused.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the plugin is in the <see cref="PluginState.Stopped"/> or 
		/// <see cref="PluginState.Running"/> state.
		/// </exception>
		public void Resume()
		{
			if((state == PluginState.Stopped)||(state == PluginState.Running))
			{
				throw new InvalidOperationException("The plugin is not in the Paused state and cannot be resumed.");
			}
			if(!settings.UseDatabase)
			{
				//Must not use the database, so we don't need to resume any threads
				state = PluginState.Running;
				OnStateChanged(EventArgs.Empty);
				return;
			}
			try
			{
				state = PluginState.Running;
				ResumeThreads();
				OnStateChanged(EventArgs.Empty);
			}
			catch
			{
				//if something goes wrong return to the Paused state
				state = PluginState.Paused;
			}
			finally
			{
				//hmmm....
			}
		}

		/// <summary>
		/// Stops the word extraction process. If the plugin is already in the <see cref="PluginState.Stopped"/>
		/// state it has no effect. If the word extraction is in progress it is not stopped
		/// abruptly but the method waits until the processing of the current document is 
		/// complete.
		/// </summary>
		public void Stop()
		{
			if(state == PluginState.Stopped)
			{
				return;
			}
			if(!settings.UseDatabase)
			{
				//Must not use the database, so we don't need to stop any threads
				state = PluginState.Stopped;
				OnStateChanged(EventArgs.Empty);
				AddToReportQueue(CWLoggerEntryType.Info, "Stopping Word Extraction Plugin.");
				return;
			}
			try
			{
				mustStop = true;
				AddToReportQueue(CWLoggerEntryType.Info, "Stopping Word Extraction plugin.");
				//wait for all the threads to finish
				while(pluginThread.IsAlive)
				{
					Thread.Sleep(1000);
				}
				//Stop all threads
				StopThreads();
				state = PluginState.Stopped;
				//notify the other classes
				OnStateChanged(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				ReportImmediately(CWLoggerEntryType.Warning, "Word Extraction plugin failed to stop: " + ex.ToString());
			}
			finally
			{
				Report();
				mustStop = false;
			}
		}

		/// <summary>
		/// Displays a form with the plugin's settings.
		/// </summary>
		public void ShowSettings()
		{
			frmPluginSettings frm = new frmPluginSettings();
			frm.Show();
		}

		/// <summary>
		/// Occurs when the plugin's <see cref="PluginState"/> changes.
		/// </summary>
		public event System.EventHandler StateChanged;

		#endregion

		#region Base class override methods

		/// <summary>
		/// Loads the plugin's settings from the appropriate configuration file
		/// </summary>
		protected override void LoadSettings()
		{
			settings.LoadSettings();	
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Extracts the words found in the contents of a document. Used by DBUpdater when
		/// a document is stored in the database in order to extract the words it contains
		/// and add them to the database at the same time.
		/// </summary>
		/// <param name="data">The <see cref="UrlCrawlData"/> to be processed.</param>
		public void ExtractWords(ref UrlCrawlData data)
		{
			//First try to extract the words from the document. If something goes wrong just
			//return, otherwise add the words to the cache, remove any old words related to
			//the url with this id from the database and store the new url-words.
			try
			{
				SortedList words = wordExtractor.ExtractWords(data.Data);
				if(words.Count == 0)
				{
					return;
				}
				//add all the words to the database if they don't exist already
				string word = String.Empty;
				short word_count = 0;
				int word_id = -1;
				foreach(DictionaryEntry de in words)
				{
					word = (string)de.Key;
					cache.AddStemmedWord(word);
				}
				//remove all the old words related to this url from the database
				RemoveUrlWords(data.ID);
				//now add relationships between the url and its words
				foreach(DictionaryEntry d in words)
				{
					word = (string)d.Key;
					word_count = (short)d.Value;
					word_id = cache[word];
					AddUrlWord(data.ID, word_id, word_count);
				}
				UpdateUrlDataLastProcess(data.ID);
			}
			catch(Exception e)
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, "WordExtractionPlugin failed to extract words from Url with ID " + data.ID.ToString() + ": " + e.ToString()));
			}
		}

		/// <summary>
		/// Extracts the words found in the contents of a document. It does not add anything
		/// to the database, it just creates a <see cref="SortedList"/> with the words and
		/// the number of their appearances in the input data.
		/// </summary>
		/// <param name="data">The data to be processed.</param>
		/// <returns>
		/// A <see cref="SortedList"/> with the words and their number of appearances.
		/// </returns>
		public SortedList ExtractWords(ref string data)
		{
			try
			{
				return wordExtractor.ExtractWords(data);
			}
			catch
			{
				return new SortedList();
			}
		}

		#endregion

		#region Private methods

		private void PerformExtraction()
		{
			try
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Info, DateTime.Now, "CrawlWave Word Extraction Plugin thread has started with ID 0x" + Thread.CurrentThread.GetHashCode().ToString("x4")));
				//the user may have enabled database just before starting the plugin
				if(settings.UseDatabase)
				{
					if(dbcon == null)
					{
						dbProvider = DBConnectionStringProvider.Instance();
						settings.DBConnectionString = dbProvider.ProvideDBConnectionString(name);
						dbcon = new SqlConnection(settings.DBConnectionString);
					}
				}
				while(!mustStop)
				{
					//Select a page from the database, perform word extraction and store the
					//results back in the database
					int UrlID = 0;
					string data = String.Empty;
					int waitSeconds = 0;
					SelectUrlForWordExtraction(out UrlID, out data);
					if(UrlID!=0)
					{
						try
						{
							backoff.Reset();
							SortedList words = wordExtractor.ExtractWords(data);
							if(words.Count != 0)
							{
								//add all the words to the database if they don't exist already
								string word = String.Empty;
								short word_count = 0;
								int word_id = -1;
								foreach(DictionaryEntry de in words)
								{
									word = (string)de.Key;
									cache.AddStemmedWord(word);
								}
								//remove all the old words related to this url from the database
								RemoveUrlWords(UrlID);
								//now add relationships between the url and its words
								foreach(DictionaryEntry d in words)
								{
									word = (string)d.Key;
									word_count = (short)((int)d.Value);
									word_id = cache[word];
									AddUrlWord(UrlID, word_id, word_count);
								}
							}
							UpdateUrlDataLastProcess(UrlID);
						}
						catch(Exception e)
						{
							events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Warning, DateTime.Now, "WordExtractionPlugin failed to extract words from Url with ID " + UrlID.ToString() + ": " + e.ToString()));
							continue;
						}
					}
					else
					{
						waitSeconds = backoff.Next()/1000;
						for(int i = 0; i < waitSeconds; i++)
						{
							Thread.Sleep(1000);
							if(mustStop)
							{
								break;
							}
						}
					}
					Report();
					if(settings.PauseBetweenOperations)
					{
						waitSeconds = PauseInSeconds();
						for(int i = 0; i < waitSeconds; i++)
						{
							Thread.Sleep(1000);
							if(mustStop)
							{
								break;
							}
						}
					}
				}
			}
			catch(ThreadAbortException)
			{
				//The thread was asked to abort, which means it must return at once
				return;
			}
			catch(ThreadInterruptedException)
			{
				//The thread has been asked to Join. We have nothing to do but return.
				return;
			}
			finally
			{
				events.Enqueue(new EventLoggerEntry(CWLoggerEntryType.Info, DateTime.Now, Thread.CurrentThread.Name + " has stopped."));
			}
		}

		/// <summary>
		/// Stops the plugin's threads
		/// </summary>
		private void StopThreads()
		{
			if(pluginThread.IsAlive)
			{
				pluginThread.Join(15000);
			}
		}

		/// <summary>
		/// Suspends the plugin's threads
		/// </summary>
		private void SuspendThreads()
		{
			if(pluginThread.IsAlive)
			{
				pluginThread.Suspend();
			}
		}

		/// <summary>
		/// Resumes the plugin's suspended threads
		/// </summary>
		private void ResumeThreads()
		{
			if((pluginThread.ThreadState & (ThreadState.Suspended | ThreadState.SuspendRequested)) >0)
			{
				pluginThread.Resume();
			}
		}

		/// <summary>
		/// Removes all the associations between a url and words from the database
		/// </summary>
		/// <param name="UrlID">The ID of the Url</param>
		private void RemoveUrlWords(int UrlID)
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
				SqlCommand cmd = new SqlCommand("cw_delete_url_words",dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBActionTimeout;
				cmd.Parameters.Add("@url_id", SqlDbType.Int);
				cmd.Parameters[0].Value = UrlID;
				int rows = cmd.ExecuteNonQuery();
				AddToReportQueue(CWLoggerEntryType.Info, "WordExtractor Plugin deleted " + rows.ToString() + " associated with url " + UrlID.ToString());
				cmd.Dispose();
				dbcon.Close();
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "CrawlWave WordExtractor Plugin failed to delete the words of url with id " + UrlID.ToString() + ": " + e.Message);
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

		/// <summary>
		/// Adds a relationship between a Url and a word to the database
		/// </summary>
		/// <param name="UrlID">The ID of the Url</param>
		/// <param name="wordId">The ID of the word</param>
		/// <param name="wordCount">The number of times the word appeared in the Url.</param>
		private void AddUrlWord(int UrlID, int wordId, short wordCount)
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
				SqlCommand cmd = new SqlCommand("cw_insert_url_word",dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBActionTimeout;
				cmd.Parameters.Add("@url_id", SqlDbType.Int);
				cmd.Parameters.Add("@word_id", SqlDbType.Int);
				cmd.Parameters.Add("@count", SqlDbType.SmallInt);
				cmd.Parameters[0].Value = UrlID;
				cmd.Parameters[1].Value = wordId;
				cmd.Parameters[2].Value = wordCount;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				dbcon.Close();
			}
			catch(Exception e)
			{
				AddToReportQueue(CWLoggerEntryType.Warning, "CrawlWave Word Extractor plugin failed to add word " + wordId.ToString() + " to url " + UrlID + ": " + e.Message);
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

		/// <summary>
		/// Updates a url's last process date after the word extraction is complete
		/// </summary>
		/// <param name="UrlID">The ID of the Url to update</param>
		private void UpdateUrlDataLastProcess(int UrlID)
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
				SqlCommand cmd = new SqlCommand("cw_update_url_data_last_process",dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBActionTimeout;
				cmd.Parameters.Add("@url_id", SqlDbType.Int);
				cmd.Parameters[0].Value = UrlID;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				dbcon.Close();
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

		/// <summary>
		/// Selects a random Url to extract words from.
		/// </summary>
		/// <param name="UrlID">The ID of the selected Url.</param>
		/// <param name="data">The data of the selected Url.</param>
		private void SelectUrlForWordExtraction(out int UrlID, out string data)
		{
			UrlID = 0;
			data = String.Empty;
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
				SqlCommand cmd = new SqlCommand("cw_select_url_for_word_extraction",dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = settings.DBActionTimeout;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				if(ds.Tables.Count==0)
				{
					return;
				}
				UrlID = (int)ds.Tables[0].Rows[0][0];
				byte [] buffer = (byte [])ds.Tables[0].Rows[0][1];
				CompressionUtils.DecompressToString(buffer, out data);
				ds.Dispose();
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

		/// <summary>
		/// Returns the number of seconds the thread must wait before continuing to the next loop
		/// </summary>
		/// <returns>The delay amount in seconds</returns>
		private int PauseInSeconds()
		{
			int retVal = 30;
			switch(settings.PauseDelay)
			{
				case 0:
					retVal = 15;
				break;

				case 1:
					retVal = 30;
				break;

				case 2:
					retVal = 60;
				break;

				case 3:
					retVal = 300;
				break;

				default:
					retVal = 30;
				break;
			}
			return retVal;
		}

		#endregion

		#region Event Invokers

		/// <summary>
		/// Raises the <see cref="StateChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> related to the event.</param>
		private void OnStateChanged(EventArgs e)
		{
			if(StateChanged != null)
			{
				StateChanged(this, e);
			}
		}

		#endregion
	}
}
