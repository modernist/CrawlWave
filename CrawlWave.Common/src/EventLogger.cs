using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CrawlWave.Common
{
	/// <summary>
	/// ILogger defines the interface for all classes that can be used to log events to
	/// different repositories.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Defines the prototype for a method that logs an event of type Information.
		/// </summary>
		/// <param name="msg">The message to log.</param>
		void LogInfo(string msg);
		/// <summary>
		/// Defines the prototype for a method that logs an event of type Warning.
		/// </summary>
		/// <param name="msg">The message to log.</param>
		void LogWarning(string msg);
		/// <summary>
		/// Defines the prototype for a method that logs an event of type Error.
		/// </summary>
		/// <param name="msg">The message to log.</param>
		void LogError(string msg);
		/// <summary>
		/// Defines the prototype for a method that logs an event contained in an <see cref="EventLoggerEntry"/> object.
		/// </summary>
		/// <param name="entry">The <see cref="EventLoggerEntry"/> to log.</param>
		void LogEventEntry(EventLoggerEntry entry);
		/// <summary>
		/// Gets the string contained in the last Log Entry.
		/// </summary>
		string LastEntry {get;}
		/// <summary>
		/// Defines whether the Logger must remember the last entry logged.
		/// </summary>
		bool RememberLastEntry { get; set;}
	}

	/// <summary>
	/// SystemEventLogger allows creating System Event Logs (available on
	/// Windows NT/2K/XP). It could be extended in order to create a custom
	/// XML log file, but for the time being the System Log will have to do.
	/// Author: Kostas Stroggylos [mod], kostas@circular.gr
	/// Written:09/05/03
	/// Updated:24/08/2004 -> Added the ILogger implementation and thread safety measures.
	/// </summary>
	public class SystemEventLogger : ILogger
	{
		#region Private variables

		private static string EventLogSource="CrawlWave";
		private string lastMessage;
		private bool rememberLastEntry;
		private EventLog eventLog;
		private Mutex mutex;

		#endregion

		#region Constructor

		/// <summary>
		/// The default constructor for the SystemEventLogger class
		/// </summary>
		public SystemEventLogger()
		{
			lastMessage=String.Empty;
			rememberLastEntry = false;
			mutex=new Mutex();
			CheckEventSource();
			eventLog=new EventLog();
			eventLog.Source=EventLogSource;
		}

		/// <summary>
		/// Constructs an instance of the <see cref="SystemEventLogger"/> class and sets the
		/// appropriate Event Source Name.
		/// </summary>
		/// <param name="EventSourceName">A string containing the Event Source name.</param>
		public SystemEventLogger(string EventSourceName)
		{
			lastMessage=String.Empty;
			rememberLastEntry = false;
			EventLogSource = EventSourceName;
			mutex=new Mutex();
			CheckEventSource();
			eventLog=new EventLog();
			eventLog.Source=EventLogSource;
		}

		#endregion

		#region Private members

		/// <summary>
		/// Checks if there is an event source named "CrawlWave"
		/// in the system and if not it creates it.
		/// </summary>
		/// <remarks>
		/// It chokes all Exceptions that might occur.
		/// </remarks>
		private void CheckEventSource()
		{
			try
			{
				//Assure thread safety
				mutex.WaitOne();
				//if this type of event log source does not exist
				if (!EventLog.SourceExists(EventLogSource))
				{
					//create it
					EventLog.CreateEventSource(EventLogSource, "Application");
				}
			}
			catch
			{
				//will occur in Windows 95/98/ME where System Event Log is not available.
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		#endregion

		#region ILogger Members

		/// <summary>
		/// Creates a System Log Entry of type Information
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogInfo(string msg)
		{
			try
			{
				//Assure thread safety
				mutex.WaitOne();
				//Assign the new value to the LastEntry property
				if(rememberLastEntry)
				{
					lastMessage=msg;
				}
				if(eventLog!=null)
				{
					//Write an Information entry with this source
					eventLog.WriteEntry(msg,EventLogEntryType.Information);
				}
			}
			catch
			{
				//An exception might occur because either the EventLog is not
				//available (Win95/98/Me) or the Event Log Service is disabled
				//or stopped or the Application Event Log could be full. In this
				//case the application could append messages to a custom XML File.
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a System Log Entry of type Error
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogError(string msg)
		{
			try
			{
				//Assure thread safety
				mutex.WaitOne();
				//Assign the new value to the LastEntry property
				if(rememberLastEntry)
				{
					lastMessage=msg;
				}
				if(eventLog!=null)
				{
					//Write an Error entry with this source
					eventLog.WriteEntry(msg,EventLogEntryType.Error);
				}
			}
			catch
			{
				//An exception might occur because either the EventLog is not
				//available (Win95/98/Me) or the Event Log Service is disabled
				//or stopped or the Application Event Log could be full. In this
				//case the application could append messages to a custom XML File.
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a System Log Entry of type Warning
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogWarning(string msg)
		{
			try
			{
				//Assure thread safety
				mutex.WaitOne();
				//Assign the new value to the LastEntry property
				if(rememberLastEntry)
				{
					lastMessage=msg;
				}
				if(eventLog!=null)
				{
					//Write a Warning entry with this source
					eventLog.WriteEntry(msg,EventLogEntryType.Warning);
				}
			}
			catch
			{
				//An exception might occur because either the EventLog is not
				//available (Win95/98/Me) or the Event Log Service is disabled
				//or stopped or the Application Event Log could be full. In this
				//case the application could append messages to a custom XML File.
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a system log entry according to the type of the entry's event
		/// </summary>
		/// <param name="entry">The <see cref="EventLoggerEntry"/> to log.</param>
		public void LogEventEntry(EventLoggerEntry entry)
		{
			try
			{
				//Assure thread safety
				mutex.WaitOne();
				//Assign the new value to the LastEntry property
				if(rememberLastEntry)
				{
					lastMessage=entry.EventMessage;
				}
				if(eventLog!=null)
				{
					//Write an entry with this source
					if(entry.EventType == CWLoggerEntryType.Info)
					{
						eventLog.WriteEntry(entry.EventDate.ToString("r") + " " + entry.EventMessage,EventLogEntryType.Information);
					}
					if(entry.EventType == CWLoggerEntryType.Warning)
					{
						eventLog.WriteEntry(entry.EventDate.ToString("r") + " " + entry.EventMessage,EventLogEntryType.Warning);
					}
					if(entry.EventType == CWLoggerEntryType.Error)
					{
						eventLog.WriteEntry(entry.EventDate.ToString("r") + " " + entry.EventMessage,EventLogEntryType.Error);
					}
				}
			}
			catch
			{
				//An exception might occur because either the EventLog is not
				//available (Win95/98/Me) or the Event Log Service is disabled
				//or stopped or the Application Event Log could be full. In this
				//case the application could append messages to a custom XML File.
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Gets the message of the last Event Log entry.
		/// </summary>
		public string LastEntry
		{
			get
			{
				try
				{
					//Assure thread safety
					mutex.WaitOne();
					return lastMessage;
				}
				catch
				{
					return String.Empty;
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// Gets or sets a Boolean value indicating whether the logger must remember the last
		/// entry.
		/// </summary>
		public bool RememberLastEntry
		{
			get { return rememberLastEntry; }
			set { rememberLastEntry = value;}
		}

		#endregion
	}


	/// <summary>
	/// FileEventLogger is a simple thread-safe Event Logger that uses a flat text file as
	/// the log entries' repository. It implements the <see cref="ILogger"/> interface and
	/// provides some extra functionality, such as Backup and automatic log truncation.
	/// Author:	Kostas Stroggylos [mod],kostas@circular.gr
	/// </summary>
	public class FileEventLogger : ILogger, IDisposable
	{
		#region Private variables

		private string fileName;
		private string lastMessage;
		private string eventSourceName;
		private bool rememberLastEntry;
		private bool autoTruncate;
		private bool shared;
		private static int maxSize=5242880;
		private static int minEntries=1000;
		private string dateFormat;
		private Mutex mutex;
		private StreamWriter stream;

		#endregion

		#region Constructors

		/// <summary>
		/// The FileEventLogger constructor. Initializes the File Streams and the thread
		/// safety / synchronization mechanism. If the appropriate flag has been set and
		/// the size of the log file has exceeded a certain limit it performs truncation.
		/// </summary>
		/// <param name="FileName">The full path of the file that will be used as a log.</param>
		/// <param name="AutoTruncate">Indicates whether the log file will be truncated automatically.</param>
		/// <param name="Shared">Indicates whether the log file will be opened in shared mode, thus allowing two or more processes to use it.</param>
		/// <param name="EventSourceName">The description of the event source application.</param>
		/// <exception cref="ArgumentException">It is thrown if the given FileName is null or empty.</exception>
		/// <exception cref="CWException">It is thrown if the Logger fails to open the given file in append mode.</exception>
		/// <remarks>
		/// If the <b>Shared</b> parameter is set to True then there may occur corruption of
		/// th log file, since each instance of the class will hool it's own copy of the file
		/// stream and each instance will overwrite the other's log. It must be used with great
		/// care, unless only the first instance is used for logging and the others are only
		/// reading the log file.
		/// </remarks>
		public FileEventLogger(string FileName, bool AutoTruncate, bool Shared, string EventSourceName)
		{
			if((FileName==null)||(FileName.Length==0))
			{
				throw new ArgumentException("The FileName cannot be empty.");
			}
			mutex=new Mutex();
			autoTruncate=AutoTruncate;
			shared = Shared;
			lastMessage=String.Empty;
			rememberLastEntry = false;
			dateFormat = "r";
			eventSourceName = EventSourceName;
			fileName=FileName;
			if(autoTruncate)
			{
				Truncate();
			}
			try
			{
				if(shared)
				{
					stream = new StreamWriter(File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
				}
				else
				{
					stream = File.AppendText(fileName);
				}
			}
			catch(Exception e)
			{
				throw new CWException("CrawlWave.Common.FileEventLogger could not open file '" + fileName + "' for Appending: " + e.Message);
			}
		}

		/// <summary>
		/// The FileEventLogger constructor. Initializes the File Streams and the thread
		/// safety / synchronization mechanism. If the appropriate flag has been set and
		/// the size of the log file has exceeded a certain limit it performs truncation.
		/// </summary>
		/// <param name="FileName">The full path of the file that will be used as a log.</param>
		/// <param name="AutoTruncate">Indicates whether the log file will be truncated automatically.</param>
		/// <param name="EventSourceName">The description of the event source application.</param>
		/// <exception cref="ArgumentException">It is thrown if the given FileName is null or empty.</exception>
		/// <exception cref="CWException">It is thrown if the Logger fails to open the given file in append mode.</exception>
		public FileEventLogger(string FileName, bool AutoTruncate, string EventSourceName)
		{
			if((FileName==null)||(FileName.Length==0))
			{
				throw new ArgumentException("The FileName cannot be empty.");
			}
			mutex=new Mutex();
			autoTruncate=AutoTruncate;
			lastMessage=String.Empty;
			rememberLastEntry = false;
			shared = false;
			dateFormat = "r";
			eventSourceName = EventSourceName;
			fileName=FileName;
			if(autoTruncate)
			{
				Truncate();
			}
			try
			{
				if(shared)
				{
					stream = new StreamWriter(File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
				}
				else
				{
					stream = File.AppendText(fileName);
				}
			}
			catch(Exception e)
			{
				throw new CWException("CrawlWave.Common.FileEventLogger could not open file '" + fileName + "' for Appending: " + e.Message);
			}
		}

		/// <summary>
		/// The FileEventLogger constructor. Initializes the File Streams and the thread
		/// safety / synchronization mechanism. This version assumes AutoTruncate is off.
		/// </summary>
		/// <param name="FileName">The full path of the file that will be used as a log.</param>
		/// <param name="EventSourceName">The description of the event source application.</param>
		/// <exception cref="ArgumentException">It is thrown if the given FileName is null or empty.</exception>
		/// <exception cref="CWException">It is thrown if the Logger fails to open the given file in append mode.</exception>
		public FileEventLogger(string FileName, string EventSourceName)
		{
			if((FileName==null)||(FileName.Length==0))
			{
				throw new ArgumentException("The FileName cannot be empty.");
			}
			mutex=new Mutex();
			autoTruncate=false;
			shared = false;
			rememberLastEntry = false;
			lastMessage=String.Empty;
			dateFormat = "r";
			eventSourceName = EventSourceName;
			fileName=FileName;
			try
			{
				if(shared)
				{
					stream = new StreamWriter(File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
				}
				else
				{
					stream = File.AppendText(fileName);
				}
			}
			catch(Exception e)
			{
				throw new CWException("CrawlWave.Common.FileEventLogger could not open file '" + fileName + "' for Appending: " + e.Message);
			}
		}

		/// <summary>
		/// The FileEventLogger constructor. Initializes the File Streams and the thread
		/// safety / synchronization mechanism. This version assumes AutoTruncate is off
		/// and the EventSourceName is an empty string.
		/// </summary>
		/// <param name="FileName">The full path of the file that will be used as a log.</param>
		/// <exception cref="ArgumentException">It is thrown if the given FileName is null or empty.</exception>
		/// <exception cref="CWException">It is thrown if the Logger fails to open the given file in append mode.</exception>
		public FileEventLogger(string FileName)
		{
			if((FileName==null)||(FileName.Length==0))
			{
				throw new ArgumentException("The FileName cannot be empty.");
			}
			mutex=new Mutex();
			autoTruncate=false;
			shared = false;
			rememberLastEntry = false;
			lastMessage=String.Empty;
			dateFormat = "r";
			eventSourceName = String.Empty;
			fileName=FileName;
			try
			{
				if(shared)
				{
					stream = new StreamWriter(File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
				}
				else
				{
					stream = File.AppendText(fileName);
				}
			}
			catch(Exception e)
			{
				throw new CWException("CrawlWave.Common.FileEventLogger could not open file '" + fileName + "' for Appending: " + e.Message);
			}
		}

		#endregion

		#region ILogger Members

		/// <summary>
		/// Creates a Log Entry of type Information in the log file in a thread-safe manner.
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogInfo(string msg)
		{
			try
			{
				mutex.WaitOne();
				if(eventSourceName==String.Empty)
				{
					lastMessage="[" + DateTime.Now.ToString(dateFormat) + "][INFO]    " + msg;
				}
				else
				{
					lastMessage="[" + DateTime.Now.ToString(dateFormat) + "][INFO]    " + eventSourceName + ": " + msg;
				}
				stream.WriteLine(lastMessage);
				stream.Flush();
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a Log Entry of type Warning in the log file in a thread-safe manner.
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogWarning(string msg)
		{
			try
			{
				mutex.WaitOne();
				if(eventSourceName==String.Empty)
				{
					lastMessage="[" + DateTime.Now.ToString(dateFormat) + "][WARNING] " + msg;
				}
				else
				{
					lastMessage="[" + DateTime.Now.ToString(dateFormat) + "][WARNING] " + eventSourceName + ": " + msg;
				}
				stream.WriteLine(lastMessage);
				stream.Flush();
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a Log Entry of type Error in the log file in a thread-safe manner.
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogError(string msg)
		{
			try
			{
				mutex.WaitOne();
				if(eventSourceName==String.Empty)
				{
					lastMessage="[" + DateTime.Now.ToString(dateFormat) + "][ERROR]   " + msg;
				}
				else
				{
					lastMessage="[" + DateTime.Now.ToString(dateFormat) + "][ERROR]   " + eventSourceName + ": " + msg;
				}
				stream.WriteLine(lastMessage);
				stream.Flush();
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a Log Entry in the log file according to the type of the entry's event.
		/// </summary>
		/// <param name="entry">The <see cref="EventLoggerEntry"/> to log.</param>
		public void LogEventEntry(EventLoggerEntry entry)
		{
			try
			{
				mutex.WaitOne();
				if(eventSourceName==String.Empty)
				{
					lastMessage="[" + entry.EventDate.ToString(dateFormat) + "]["+ entry.EventType.ToString().ToUpper() +"]   " + entry.EventMessage;
				}
				else
				{
					lastMessage="[" + entry.EventDate.ToString(dateFormat) + "]["+ entry.EventType.ToString().ToUpper() +"]   " + eventSourceName + ": " + entry.EventMessage;
				}
				stream.WriteLine(lastMessage);
				stream.Flush();
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Gets the message of the last Event Log entry.
		/// </summary>
		public string LastEntry
		{
			get
			{
				try
				{
					//Assure thread safety
					mutex.WaitOne();
					return lastMessage;
				}
				catch
				{
					return String.Empty;
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// Gets or sets a Boolean value indicating whether the logger must remember the last
		/// entry.
		/// </summary>
		public bool RememberLastEntry
		{
			get { return rememberLastEntry; }
			set { rememberLastEntry = value;}
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Cleans up resources used by the File Event Logger, closes any open File Streams
		/// and releases all synchronization mechanisms.
		/// </summary>
		public void Dispose()
		{
			try
			{
				stream.Close();
				mutex.Close();
			}
			catch
			{}
		}

		#endregion

		#region Extended functionality methods

		/// <summary>
		/// Backs up the current version of the log file to another file with the same name
		/// and a .bak extension. Not thread-safe.
		/// </summary>
		public void Backup()
		{
			try
			{
				File.Copy(fileName,fileName + ".bak", true);
			}
			catch
			{}
		}

		/// <summary>
		/// Restores the log file from another file with the same name and a .bak extension.
		/// Not thread-safe.
		/// </summary>
		public void Restore()
		{
			try
			{
				File.Copy(fileName + ".bak", fileName, true);
			}
			catch
			{}
		}

		/// <summary>
		/// Performs the truncation of the Log File, according to the values defined for
		/// the maximum file size and the number of entries to save.
		/// </summary>
		private void Truncate()
		{
			if(File.Exists(fileName))
			{
				FileInfo f=new FileInfo(fileName);
				if(f.Length>maxSize)
				{
					try
					{
						Backup();
						StreamReader input= new StreamReader(fileName);
						string text=input.ReadToEnd();
						input.Close();
						input = null;
						int entries=0,offset=text.Length;
						while((offset=text.LastIndexOf("\n[",offset-1,offset))!=-1)
						{
							entries++;
							if(entries==minEntries)
							{
								break;
							}
						}
						StreamWriter output=new StreamWriter(fileName,false);
						output.Write(text.Substring(offset+1));
						output.Close();
						output = null;
					}
					catch
					{
						Restore();
					}
					finally
					{
						GC.Collect();
					}
				}
			}
		}

		/// <summary>
		/// Gets/sets the Automatic Truncate property, which determines whether the log
		/// file will be truncated down to a certain number of entries when it grows up
		/// to a certain size.
		/// </summary>
		public bool AutoTruncate
		{
			get { return autoTruncate; }
			set { autoTruncate = value;}
		}

		#endregion
	}

	/// <summary>
	/// QueueEventLogger is a simple thread-safe Event Logger that buffers events into a
	/// Queue from which the clients can later on dequeue the <see cref="EventLoggerEntry"/>
	/// objects. It implements <see cref="ILogger"/> and offers some extra functionality
	/// such as setting an upper limit in the number of events that the queue will hold and
	/// clearing the queue of events.
	/// </summary>
	public class QueueEventLogger : ILogger
	{
		#region Private variables

		private string eventSourceName;
		private string lastMessage;
		private bool rememberLastEntry;
		private int maxSize = 100;
		private Queue<EventLoggerEntry> events;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of <see cref="QueueEventLogger"/> with default values.
		/// </summary>
		public QueueEventLogger()
		{
			eventSourceName = String.Empty;
			lastMessage = String.Empty;
			rememberLastEntry = false;
			events = new Queue<EventLoggerEntry>(maxSize);
		}

		/// <summary>
		/// Creates a new instance of <see cref="QueueEventLogger"/> with the given Event
		/// Source name.
		/// </summary>
		/// <param name="EventSourceName">The Event Source name.</param>
		public QueueEventLogger(string EventSourceName)
		{
			eventSourceName = EventSourceName;
			lastMessage = String.Empty;
			rememberLastEntry = false;
			events = new Queue<EventLoggerEntry>(maxSize);
		}

		/// <summary>
		/// Creates a new instance of <see cref="QueueEventLogger"/> with the given capacity.
		/// </summary>
		/// <param name="MaxSize">The capacity of the Event Queue.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if a negative value is supplied for MaxSize.</exception>
		public QueueEventLogger(int MaxSize)
		{
			if(MaxSize<=0)
			{
				throw new ArgumentOutOfRangeException("MaxSize", "MaxSize must be a positive value.");
			}
			eventSourceName = String.Empty;
			lastMessage = String.Empty;
			rememberLastEntry = false;
			maxSize = MaxSize;
			events = new Queue<EventLoggerEntry>(maxSize);
		}

		/// <summary>
		/// Creates a new instance of <see cref="QueueEventLogger"/> with the given Event
		/// Source Name and Queue capacity.
		/// </summary>
		/// <param name="MaxSize">The capacity of the Event Queue.</param>
		/// <param name="EventSourceName">The Event Source name.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if a negative value is supplied for MaxSize.</exception>
		public QueueEventLogger(int MaxSize, string EventSourceName)
		{
			if(MaxSize<=0)
			{
				throw new ArgumentOutOfRangeException("MaxSize", "MaxSize must be a positive value.");
			}
			eventSourceName = EventSourceName;
			lastMessage = String.Empty;
			rememberLastEntry = false;
			maxSize = MaxSize;
			events = new Queue<EventLoggerEntry>(maxSize);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the maximum number of <see cref="EventLoggerEntry"/> objects that
		/// the <see cref="QueueEventLogger"/> will hold.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if a negative value is supplied for MaxSize.</exception>
		public int MaxSize
		{
			get { return maxSize; }
			set
			{
				if(value<=0)
				{
					throw new ArgumentOutOfRangeException("MaxSize", "MaxSize must be a positive value.");
				}
				else
				{
					maxSize = value;
				}
			}
		}

		/// <summary>
		/// Gets the number of events logged in the <see cref="QueueEventLogger"/>.
		/// </summary>
		public int Count
		{
			get { return events.Count; }
		}

		/// <summary>
		/// Provides access to the internal Queue used by the <see cref="QueueEventLogger"/>.
		/// </summary>
		public Queue<EventLoggerEntry> EventQueue
		{
			get { return events; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Enqueues an <see cref="EventLoggerEntry"/> object in the <see cref="QueueEventLogger"/>.
		/// </summary>
		/// <param name="entry">The <see cref="EventLoggerEntry"/> to be logged.</param>
		public void Enqueue(EventLoggerEntry entry)
		{
			if(events.Count == maxSize)
			{
				events.Dequeue();
			}
			events.Enqueue(entry);
		}

		/// <summary>
		/// Dequeues the first <see cref="EventLoggerEntry"/> logged by the <see cref="QueueEventLogger"/>.
		/// </summary>
		/// <returns>An <see cref="EventLoggerEntry"/> containing the first event logged.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the method is called when QueueEventLogger contains no entries.</exception>
		public EventLoggerEntry Dequeue()
		{
			if(events.Count == 0)
			{
				throw new InvalidOperationException("The QueueEventLogger contains no events.");
			}
			else
			{
				return events.Dequeue();
			}
		}

		#endregion

		#region ILogger Members

		/// <summary>
		/// Logs an event of type Info
		/// </summary>
		/// <param name="msg">The message related to the event to be logged.</param>
		public void LogInfo(string msg)
		{
			try
			{
                EventLoggerEntry entry;
				entry.EventDate = DateTime.Now;
				entry.EventType = CWLoggerEntryType.Info;
				if(eventSourceName != String.Empty)
				{
					entry.EventMessage = "[" + eventSourceName + "] " + msg;
				}
				else
				{
					entry.EventMessage = msg;
				}
				lock(events)
				{
					if(events.Count > maxSize)
					{
						events.Dequeue();
					}
					events.Enqueue(entry);
					if(rememberLastEntry)
					{
						lastMessage = entry.EventMessage;
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Logs an event of type Warning
		/// </summary>
		/// <param name="msg">The message related to the event to be logged.</param>
		public void LogWarning(string msg)
		{
			try
			{
				EventLoggerEntry entry;
				entry.EventDate = DateTime.Now;
				entry.EventType = CWLoggerEntryType.Warning;
				if(eventSourceName != String.Empty)
				{
					entry.EventMessage = "[" + eventSourceName + "] " + msg;
				}
				else
				{
					entry.EventMessage = msg;
				}
				lock(events)
				{
					if(events.Count > maxSize)
					{
						events.Dequeue();
					}
					events.Enqueue(entry);
					if(rememberLastEntry)
					{
						lastMessage = entry.EventMessage;
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Logs an event of type Error
		/// </summary>
		/// <param name="msg">The message related to the event to be logged.</param>
		public void LogError(string msg)
		{
			try
			{
				EventLoggerEntry entry;
				entry.EventDate = DateTime.Now;
				entry.EventType = CWLoggerEntryType.Error;
				if(eventSourceName != String.Empty)
				{
					entry.EventMessage = "[" + eventSourceName + "] " + msg;
				}
				else
				{
					entry.EventMessage = msg;
				}
				lock(events)
				{
					if(events.Count > maxSize)
					{
						events.Dequeue();
					}
					events.Enqueue(entry);
					if(rememberLastEntry)
					{
						lastMessage = entry.EventMessage;
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Creates a log according to the type of the entry's event
		/// </summary>
		/// <param name="entry">The <see cref="EventLoggerEntry"/> to log.</param>
		public void LogEventEntry(EventLoggerEntry entry)
		{
			try
			{
				lock(events)
				{
					if(events.Count > maxSize)
					{
						events.Dequeue();
					}
					events.Enqueue(entry);
					if(rememberLastEntry)
					{
						lastMessage = entry.EventMessage;
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Gets the message of the last Event Log entry.
		/// </summary>
		public string LastEntry
		{
			get
			{
				lock(events)
				{
					if(events.Count>0)
					{
						EventLoggerEntry entry = (EventLoggerEntry)events.Peek();
						return entry.EventMessage;
					}
					else
					{
						return String.Empty;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a Boolean value indicating whether the logger must remember the last
		/// entry.
		/// </summary>
		public bool RememberLastEntry
		{
			get { return rememberLastEntry; }
			set { rememberLastEntry = value;}
		}

		#endregion
	}

}
