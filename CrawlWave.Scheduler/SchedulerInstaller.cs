using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using Microsoft.Win32;

namespace CrawlWave.Scheduler
{
	/// <summary>
	/// Summary description for ProjectInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class ProjectInstaller : System.Configuration.Install.Installer
	{
		private System.ServiceProcess.ServiceProcessInstaller SchedulerServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller SchedulerServiceInstaller;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProjectInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stateServer"></param>
		public override void Install(IDictionary stateServer)
		{
			try
			{
				//Let the project installer do its job
				base.Install(stateServer);
				RegistryKey system, currentControlSet, services, service;
				//Open the HKEY_LOCAL_MACHINE\SYSTEM key
				system = Registry.LocalMachine.OpenSubKey("System");
				//Open CurrentControlSet
				currentControlSet = system.OpenSubKey("CurrentControlSet");
				//Go to the services key
				services = currentControlSet.OpenSubKey("Services");
				//Open the key for your service, and allow writing
				service = services.OpenSubKey(this.SchedulerServiceInstaller.ServiceName, true);
				//Add your service's description as a REG_SZ value named "Description"
				service.SetValue("Description", "Performs the automatic update and launching of CrawlWave Client.");
			}
			catch
			{
				//Console.WriteLine("An exception was thrown during service installation:\n" + e.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stateServer"></param>
		public override void Uninstall(IDictionary stateServer)
		{
			try
			{
				RegistryKey system, currentControlSet, services, service;
				//Drill down to the service key and open it with write permission
				system = Registry.LocalMachine.OpenSubKey("System");
				currentControlSet = system.OpenSubKey("CurrentControlSet");
				services = currentControlSet.OpenSubKey("Services");
				service = services.OpenSubKey(this.SchedulerServiceInstaller.ServiceName, true);
			}
			catch
			{
				//Console.WriteLine("Exception encountered while uninstalling service:\n" + e.ToString());
			}
			finally
			{
				//Let the project installer do its job
				base.Uninstall(stateServer);
			}
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SchedulerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.SchedulerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// SchedulerServiceProcessInstaller
			// 
			this.SchedulerServiceProcessInstaller.Password = null;
			this.SchedulerServiceProcessInstaller.Username = null;
			this.SchedulerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			// 
			// SchedulerServiceInstaller
			// 
			this.SchedulerServiceInstaller.ServiceName = "CWClientScheduler";
			this.SchedulerServiceInstaller.DisplayName = "CrawlWave Client Scheduler";
			this.SchedulerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																					  this.SchedulerServiceProcessInstaller,
																					  this.SchedulerServiceInstaller});

		}
		#endregion
	}
}
