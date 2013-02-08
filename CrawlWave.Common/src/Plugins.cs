using System;
using System.Collections;
using System.Collections.Generic;

namespace CrawlWave.Common
{
	/// <summary>
	/// Defines the interface of all the plugins that will be hosted in an <see cref="IPluginHost"/>
	/// and will be used for processing data in CrawlWave.ServerWorker.
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// Pauses the plugin's process temporarily.
		/// </summary>
		void Pause();

		/// <summary>
		/// Resumes the plugin's process if it has been paused. 
		/// </summary>
		void Resume();

		/// <summary>
		/// Starts the plugin's process.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the plugin's process.
		/// </summary>
		void Stop();

		/// <summary>
		/// Displays a form that allows the configuration of the various plugin parameters.
		/// </summary>
		void ShowSettings();

		/// <summary>
		/// Raised when the <see cref="PluginState"/> of the plugin changes.
		/// </summary>
		event EventHandler StateChanged;
	}

	/// <summary>
	/// PluginBase provides the base class that all the plugins must inherit from.
	/// </summary>
	/// <remarks>
	/// A plugin that is not statically referenced by another application and is meant to be
	/// loaded dynamically must be attributed with the <see cref="CrawlWavePluginAttribute"/>
	/// attribute, otherwise a <see cref="CWUnsupportedPluginException"/> will be thrown.
	/// </remarks>
	public class PluginBase
	{
		#region Protected variables

		/// <summary>
		/// The plugin's name.
		/// </summary>
		protected string name = String.Empty;
		/// <summary>
		/// The plugin's description.
		/// </summary>
		protected string description = String.Empty;
		/// <summary>
		/// The plugin's version.
		/// </summary>
		protected string version = String.Empty;
		/// <summary>
		/// The plugin's <see cref="PluginState"/>
		/// </summary>
		protected PluginState state = PluginState.Stopped;
		/// <summary>
		/// Indicated whether the plugin is enabled.
		/// </summary>
		protected bool enabled = true;
		/// <summary>
		/// Indicates whether the plugin requires access to the system's database.
		/// </summary>
		protected bool dataDependent = false;
		/// <summary>
		/// Contains the database connection string for data-dependent plugins.
		/// </summary>
		protected string connectionString = String.Empty;
		/// <summary>
		/// Contains the path of the directory where the plugin can store its settings.
		/// </summary>
		protected string settingsPath = String.Empty;
		/// <summary>
		/// The percentage of work already completed by the plugin.
		/// </summary>
		protected int percent = 0;
		/// <summary>
		/// An <see cref="ArrayList"/> containing <see cref="ILogger"/> objects, to which
		/// the plugin will report its status and error messages.
		/// </summary>
		protected ArrayList loggers = new ArrayList(4);
		/// <summary>
		/// A queue holding <see cref="EventLoggerEntry"/> objects, one for each event that
		/// must be reported when the plugin has a permission to report its status.
		/// </summary>
		protected Queue events = new Queue();
		/// <summary>
		/// A reference to the <see cref="IPluginHost"/> to which the plugin is attached.
		/// </summary>
		protected IPluginHost host = null;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a string containing the plugin's name.
		/// </summary>
		public string Name
		{
			get{ return name; }
		}

		/// <summary>
		/// Gets a string containing a description for the plugin.
		/// </summary>
		public string Description
		{
			get { return description; }
		}

		/// <summary>
		/// Gets a string containing the plugin's version.
		/// </summary>
		public string Version
		{
			get { return version; }
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating if the plugin is enabled.
		/// </summary>
		public bool Enabled
		{
			get { return enabled; } 
			set { enabled = value;}
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> value indicating if the plugin needs access to the
		/// system's database.
		/// </summary>
		public bool DataDependent
		{
			get { return dataDependent; }
		}

		/// <summary>
		/// Sets the Database Connection string that is used to allow the plugin to connect
		/// to the system's database if the plugin is <see cref="DataDependent"/>. 
		/// </summary>
		public string DBConnectionString
		{
			set { connectionString = value; }
		}

		/// <summary>
		/// Sets the path of the directory where the plugin can store its settings.
		/// </summary>
		public string SettingsPath
		{
			set
			{
				settingsPath = value;
				LoadSettings();
			}
		}

		/// <summary>
		/// Gets an integer value indicating the percentage of process that has been completed.
		/// </summary>
		public int Percent
		{
			get { return percent; }
		}

		/// <summary>
		/// Gets a <see cref="PluginState"/> value indicating the plugin's current state.
		/// </summary>
		public PluginState State
		{
			get { return state; }
		}

		/// <summary>
		/// Sets the <see cref="IPluginHost"/> that is hosting the plugin. Can only be called
		/// by an <see cref="IPluginHost"/> object.
		/// </summary>
		public IPluginHost Host
		{
			set { host = value; }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Causes the plugin to report its status or errors to all <see cref="ILogger"/>,
		/// after requesting permission from the <see cref="IPluginHost"/> hosting it. If
		/// no host is hosting the plugin the report is performed anyway.
		/// </summary>
		protected void Report()
		{
			try
			{
				if(host!=null)
				{
					if(!host.PermitReport(this))
					{
						return;
					}
				}
				lock(events.SyncRoot)
				{
					while(events.Count > 0)
					{
						EventLoggerEntry entry = (EventLoggerEntry)events.Dequeue();
						foreach(ILogger log in loggers)
						{
							log.LogEventEntry(entry);
						}
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Causes the plugin to report its status or errors to a <see cref="ILogger"/>,
		/// without requesting permission from an <see cref="IPluginHost"/>.
		/// </summary>
		protected void ReportImmediately(CWLoggerEntryType eventType, string msg)
		{
			try
			{
				lock(loggers)
				{
					foreach(ILogger logger in loggers)
					{
						switch(eventType)
						{
							case CWLoggerEntryType.Info:
								logger.LogInfo(msg);
								break;

							case CWLoggerEntryType.Warning:
								logger.LogWarning(msg);
								break;

							case CWLoggerEntryType.Error:
								logger.LogError(msg);
								break;

							default:
								logger.LogWarning(msg);
								break;
						}
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Enqueues an event message to the internal event queue.
		/// </summary>
		/// <param name="eventType">The <see cref="CWLoggerEntryType"/> of the event.</param>
		/// <param name="eventDate">The <see cref="DateTime"/> when the event took place.</param>
		/// <param name="msg">The message related to the event.</param>
		protected void AddToReportQueue(CWLoggerEntryType eventType, DateTime eventDate, string msg)
		{
			try
			{
				if(events!=null)
				{
					lock(events.SyncRoot)
					{
						events.Enqueue(new EventLoggerEntry(eventType, eventDate, name + ": " + msg));
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Enqueues an event message to the internal event queue.
		/// </summary>
		/// <param name="eventType">The <see cref="CWLoggerEntryType"/> of the event.</param>
		/// <param name="msg">The message related to the event.</param>
		protected void AddToReportQueue(CWLoggerEntryType eventType, string msg)
		{
			try
			{
				if(events!=null)
				{
					lock(events.SyncRoot)
					{
						events.Enqueue(new EventLoggerEntry(eventType, DateTime.Now, name + ": " + msg));
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Enqueues an <see cref="EventLoggerEntry"/> to the internal event queue.
		/// </summary>
		/// <param name="entry">The entry to enqueue.</param>
		protected void AddToReportQueue(EventLoggerEntry entry)
		{
			try
			{
				if(events!=null)
				{
					lock(events.SyncRoot)
					{
						events.Enqueue(entry);
					}
				}
			}
			catch
			{}
		}

		/// <summary>
		/// Loads the plugin's settings. Must be overriden in the derived classes if their
		/// settings are stored on a peristent storage.
		/// </summary>
		protected virtual void LoadSettings()
		{}

		/// <summary>
		/// Saves the plugin's settings. Must be overriden in the derived classes if their
		/// settings are stored on a peristent storage.
		/// </summary>
		protected virtual void SaveSettings()
		{}

		#endregion

		#region Public Methods

		/// <summary>
		/// Attaches an <see cref="ILogger"/> to the plugin so that the plugin can use it
		/// for reporting purposes.
		/// </summary>
		/// <param name="log">The <see cref="ILogger"/> to be used by the plugin.</param>
		public virtual void AttachLog(ILogger log)
		{
			try
			{
				if(!loggers.Contains(log))
				{
					loggers.Add(log);
				}
			}
			catch
			{}
		}

		#endregion

	}

	/// <summary>
	/// CrawlWavePluginAttribute is an attribute that must be used on all the plugins that
	/// are not statically referenced by ServerWorker and will be dynamically loaded.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class CrawlWavePluginAttribute : System.Attribute
	{

		/// <summary>
		/// Constructs a new instance of the CrawlWavePluginAttribute class.
		/// </summary>
		public CrawlWavePluginAttribute() : base()
		{}
	}

	/// <summary>
	/// Defines the interface for a Plugin Host, a class that will control the operation
	/// of one or more plugins.
	/// </summary>
	public interface IPluginHost
	{
		/// <summary>
		/// Gets an integer indicating the number of plugins that are currently active
		/// </summary>
		int RunningPlugins
		{
			get;
		}

		/// <summary>
		/// Gets an <see cref="List{PluginBase}"/> containing all the plugins hosted by the host
		/// </summary>
		List<PluginBase> Plugins
		{
			get;
		}

		/// <summary>
		/// Allows an Plugin to register itself in the plugin host.
		/// </summary>
		/// <param name="plugin">The Plugin to be registered.</param>
		void Register(PluginBase plugin);

		/// <summary>
		/// Pauses the operation of all the plugins.
		/// </summary>
		void PauseAllPlugins();

		/// <summary>
		/// Signals all the registered plugins to start operating.
		/// </summary>
		void StartAllPlugins();

		/// <summary>
		/// Signals all the registered plugins to stop operating.
		/// </summary>
		void StopAllPlugins();

		/// <summary>
		/// Checks if a plugin can report an event.
		/// </summary>
		/// <param name="plugin">The <see cref="IPlugin"/> that requests permission to report.</param>
		/// <returns>A <see cref="Boolean"/> value indicating whether the plugin can report.</returns>
		bool PermitReport(PluginBase plugin);

		/// <summary>
		/// Loads a plugin dynamically and registers it with the <see cref="IPluginHost"/>.
		/// </summary>
		/// <param name="fileName">The file containing the plugin to be loaded.</param>
		void LoadPlugin(string fileName);
	}
}
