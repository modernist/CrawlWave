using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using CrawlWave.ServerCommon;
using System.Threading;

namespace CrawlWave.Service
{
	partial class Service : ServiceBase
	{
		private ServerSettings settings;
		//private TcpChannel channel;
		private Thread serviceThread;
		private bool mustStop;

		public Service()
		{
			InitializeComponent();
			settings = ServerSettings.Instance();
		}

		protected override void OnStart(string[] args)
		{
			serviceThread = new Thread(new ThreadStart(StartService));
			serviceThread.Start();
		}

		protected override void OnStop()
		{
			settings.Log.LogInfo("CrawlWave Service is stopping");
			mustStop = true;
			UnregisterService();
			//nothing to clean up, just log it
		}

		private void StartService()
		{
			settings.Log.LogInfo("CrawlWave Service is starting");
			try
			{
				RegisterService();
				settings.Log.LogInfo("Remoting configuration completed successfully");
				while (!mustStop)
					Thread.Sleep(15000);
			}
			catch (Exception e)
			{
				settings.Log.LogError("Remoting configuration failed: " + e.ToString());
			}
		}

		private void RegisterService()
		{
			string configPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;

			RemotingConfiguration.Configure(configPath, true);

			//channel = new TcpChannel(settings.RemotingPort);
			//WellKnownServiceTypeEntry srventry = new WellKnownServiceTypeEntry(typeof(CrawlWaveServer), "CrawlWaveServer.rem", WellKnownObjectMode.Singleton);
			//RemotingConfiguration.ApplicationName = "CrawlWaveServer";
			//RemotingConfiguration.RegisterWellKnownServiceType(srventry);
		}

		private void UnregisterService()
		{
			//ChannelServices.UnregisterChannel(channel);
		}
	}
}
