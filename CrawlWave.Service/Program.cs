using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System;

namespace CrawlWave.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			ServiceBase[] ServicesToRun;

			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new ServiceBase[] { new Service() };

			ServiceBase.Run(ServicesToRun);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ServerSettings.Instance().Log.LogError("An unhandled exception occured in CrawlWaver.Service: " + ((Exception)e.ExceptionObject).ToString());
		}
	}
}