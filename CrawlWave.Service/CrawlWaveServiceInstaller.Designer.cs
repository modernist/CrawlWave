namespace CrawlWave.Service
{
	partial class CrawlWaveServiceInstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
			this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			// 
			// serviceInstaller
			// 
			this.serviceInstaller.Description = "CrawlWave Server distributes urls to clients and collects the crawl results";
			this.serviceInstaller.DisplayName = "CrawlWave Server";
			this.serviceInstaller.ServiceName = "CrawlWaveServer";
			this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// serviceProcessInstaller
			// 
			this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.serviceProcessInstaller.Password = null;
			this.serviceProcessInstaller.Username = null;
			// 
			// CrawlWaveServiceInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceInstaller,
            this.serviceProcessInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceInstaller serviceInstaller;
		private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
	}
}