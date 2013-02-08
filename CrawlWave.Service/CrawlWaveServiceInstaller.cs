using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace CrawlWave.Service
{
	[RunInstaller(true)]
	public partial class CrawlWaveServiceInstaller : Installer
	{
		public CrawlWaveServiceInstaller()
		{
			InitializeComponent();
		}
	}
}