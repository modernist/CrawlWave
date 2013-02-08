using System;
using System.Collections;
using System.Reflection;
using CrawlWave.Common;
using System.Collections.Generic;

namespace CrawlWave.ServerWorker
{
	/// <summary>
	/// PluginController is a class that implements <see cref="IPluginHost"/>. It acts as a
	/// host for all the plugins that are loaded by the Server Worker. The PluginController
	/// allows all the plugins to log their events without restrictions. It also features
	/// an event that allows the UI to be updated when the state of a plugin changes.
	/// </summary>
	public class PluginController : IPluginHost
	{
		#region Private variables

		private List<PluginBase> plugins;
		private int runningPlugins;
		private ArrayList loggers;

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// Constructs a new instance of the <see cref="PluginController"/> class.
		/// </summary>
		public PluginController()
		{
			plugins = new List<PluginBase>();
			runningPlugins = 0;
			loggers = new ArrayList(4);
			FileEventLogger log = Settings.Instance().Log;
			if(log!=null)
			{
				loggers.Add(log);
			}
		}

		#endregion

		#region IPluginHost Members

		/// <summary>
		/// Gets the number of Running Plugins.
		/// </summary>
		public int RunningPlugins
		{
			get
			{
				return runningPlugins;
			}
		}

		/// <summary>
		/// Gets a reference to the list of <see cref="IPlugin"/> objects currently loaded
		/// and managed by this instance of <see cref="PluginController"/>.
		/// </summary>
		public List<PluginBase> Plugins
		{
			get { return plugins; }
		}
        
		/// <summary>
		/// Stops all the loaded Plugins.
		/// </summary>
		public void StopAllPlugins()
		{
			foreach(PluginBase plugin in plugins)
			{
				try
				{
					((IPlugin)plugin).Stop();
					runningPlugins --;
				}
				catch(Exception e)
				{
					foreach(ILogger logger in loggers)
					{
						if(logger!=null)
						{
							logger.LogWarning("StopAllPlugins failed to stop " + plugin.Name + ": " + e.ToString());
						}
					}
					continue;
				}
			}
		}

		/// <summary>
		/// Registers a <see cref="PluginBase"/> with this instance of <see cref="PluginController"/>.
		/// </summary>
		/// <param name="plugin">The Plugin to register.</param>
		public void Register(PluginBase plugin)
		{
			if(!IsPluginRegistered(plugin))
			{
				plugins.Add(plugin);
				plugin.Host = this;
				foreach(ILogger logger in loggers)
				{
					if(logger!=null)
					{
						plugin.AttachLog(logger);
					}
				}
				((IPlugin)plugin).StateChanged+=new EventHandler(PluginController_StateChanged);
			}
			else
			{
				throw new ArgumentException("This Plugin is already loaded and registered.");
			}
		}

		/// <summary>
		/// Starts all the loaded Plugins.
		/// </summary>
		public void StartAllPlugins()
		{
			foreach(PluginBase plugin in plugins)
			{
				try
				{
					((IPlugin)plugin).Start();
					runningPlugins++;
				}
				catch(Exception e)
				{
					foreach(ILogger logger in loggers)
					{
						if(logger!=null)
						{
							logger.LogWarning("StartAllPlugins failed to start " + plugin.Name + ": " + e.ToString());
						}
					}
					continue;
				}
			}
		}

		/// <summary>
		/// Manages reporting permissions for plugins.
		/// </summary>
		/// <param name="plugin">The plugin that wishes to report an event.</param>
		/// <returns>True if the Plugin is permitted to report, otherwise false.</returns>
		public bool PermitReport(PluginBase plugin)
		{
			// TODO:  Add PluginController.PermitReport implementation
			return true;
		}

		/// <summary>
		/// Pauses all the running plugins.
		/// </summary>
		public void PauseAllPlugins()
		{
			foreach(PluginBase plugin in plugins)
			{
				try
				{
					if(plugin.State == PluginState.Running)
					{
						((IPlugin)plugin).Pause();
						runningPlugins --;
					}
				}
				catch(Exception e)
				{
					foreach(ILogger logger in loggers)
					{
						if(logger!=null)
						{
							logger.LogWarning("PauseAllPlugins failed to pause " + plugin.Name + ": " + e.ToString());
						}
					}
					continue;
				}
			}
		}

		/// <summary>
		/// Loads a plugin from disk and registers it with this instance of <see cref="PluginController"/>.
		/// </summary>
		/// <param name="fileName">The path of the file containing the Plugin to load.</param>
		/// <exception cref="CWUnsupportedPluginException">
		/// Thrown if the specified file does not contain a type that derives from 
		/// <see cref="PluginBase"/> and implements <see cref="IPlugin"/>.
		/// </exception>
		public void LoadPlugin(string fileName)
		{
			bool pluginFound = false;
			Assembly a = Assembly.LoadFile(fileName);
			System.Type []types = a.GetTypes();
			foreach(Type t in types)
			{
				if(t.GetInterface("IPlugin")!=null)
				{
					if(t.IsSubclassOf(typeof(CrawlWave.Common.PluginBase)))
					{
						if(t.GetCustomAttributes(typeof(CrawlWavePluginAttribute),false).Length>0)
						{
							PluginBase plugin = (PluginBase)Activator.CreateInstance(t);
							Register(plugin);
							pluginFound = true;
						}
					}
				}
			}
			if(!pluginFound)
			{
				throw new CWUnsupportedPluginException(fileName + " does not contain a CrawlWave plugin.");
			}
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs whenever the <see cref="PluginBase.State"/> of a loaded Plugin changes.
		/// </summary>
		public event EventHandler PluginStateChanged;

		#endregion

		#region Private methods

		/// <summary>
		/// Checks whether a <see cref="PluginBase">Plugin</see> is already registered with
		/// the controller.
		/// </summary>
		/// <param name="plugin">The plugin to check</param>
		/// <returns>True if the Plugin is already registered with the controller, false otherwise.</returns>
		private bool IsPluginRegistered(PluginBase plugin)
		{
			System.Type pt = plugin.GetType();
			foreach(PluginBase loadedPlugin in plugins)
			{
				if(pt == loadedPlugin.GetType())
				{
					return true;
				}
			}
			return false;
		}

		private void PluginController_StateChanged(object sender, EventArgs e)
		{
			OnPluginStateChanged(sender, e);
		}

		private void OnPluginStateChanged(object sender, EventArgs e)
		{
			if(PluginStateChanged != null)
			{
				PluginStateChanged(sender, e);
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Attaches a new logger to which the host must log all the events.
		/// </summary>
		/// <param name="logger"></param>
		public void AttachLogger(ILogger logger)
		{
			if(logger==null)
			{
				throw new ArgumentNullException("logger");
			}
			try
			{
				loggers.Add(logger);
			}
			catch
			{}
		}

		#endregion
	}
}
