using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerCommon
{
	public class CrawlWaveServerProxy
	{
		private static ICrawlWaveServer proxy;
		private static CrawlWaveServerProxy instance;

		private CrawlWaveServerProxy(ICrawlWaveServerSettingsProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException();

			IChannel channel;

			switch (provider.ChannelType)
			{
				case "http":
					channel = (IChannel)(new HttpChannel());//(provider.Port));
					break;

				case "ipc":
					channel = (IChannel)(new IpcChannel(string.Format("{0}:{1}", provider.Hostname, provider.Port)));
					break;

				case "tcp":
					channel = (IChannel)(new TcpChannel());//(provider.Port));
					break;

				default:
					throw new ArgumentException();
					break;
			}

			ChannelServices.RegisterChannel(channel, true);

			proxy = (ICrawlWaveServer)Activator.GetObject(
				typeof(ICrawlWaveServer),
				string.Format("{0}://{1}:{2}/CrawlWaveServer.rem", provider.ChannelType, provider.Hostname, provider.Port));
		}

		public static ICrawlWaveServer Instance(ICrawlWaveServerSettingsProvider provider)
		{
			if (instance == null)
			{
				instance = new CrawlWaveServerProxy(provider);
			}
			return proxy;
		}

	}
}
