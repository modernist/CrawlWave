using System;
using System.Collections.Generic;
using System.Text;

namespace CrawlWave.ServerCommon
{
	public interface ICrawlWaveServerSettingsProvider
	{
		string Hostname
		{
			get;
		}

		int Port
		{
			get;
		}

		string ChannelType
		{
			get;
		}
	}
}
