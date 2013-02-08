using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace CrawlWave.ServerInstaller.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class frmStep5 : CrawlWave.ServerInstaller.Forms.frmStep
	{
		private System.Windows.Forms.GroupBox grpAutoStart;
		private System.Windows.Forms.CheckBox chkAutoStartServerWorker;
		private System.Windows.Forms.CheckBox chkAutoStartServerManager;
		private System.ComponentModel.IContainer components = null;

		private Globals globals;

		/// <summary>
		/// 
		/// </summary>
		public frmStep5()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			globals = Globals.Instance();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grpAutoStart = new System.Windows.Forms.GroupBox();
			this.chkAutoStartServerWorker = new System.Windows.Forms.CheckBox();
			this.chkAutoStartServerManager = new System.Windows.Forms.CheckBox();
			this.grpAutoStart.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblStep1
			// 
			this.lblStep1.ImageIndex = 2;
			this.lblStep1.Name = "lblStep1";
			// 
			// lblStep2
			// 
			this.lblStep2.ImageIndex = 5;
			this.lblStep2.Name = "lblStep2";
			// 
			// lblStep3
			// 
			this.lblStep3.ImageIndex = 8;
			this.lblStep3.Name = "lblStep3";
			// 
			// lblStep4
			// 
			this.lblStep4.ImageIndex = 11;
			this.lblStep4.Name = "lblStep4";
			// 
			// lblStep5
			// 
			this.lblStep5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblStep5.ImageIndex = 13;
			this.lblStep5.Name = "lblStep5";
			// 
			// lblStep6
			// 
			this.lblStep6.Name = "lblStep6";
			// 
			// cmdBack
			// 
			this.cmdBack.Name = "cmdBack";
			this.cmdBack.TabIndex = 2;
			this.cmdBack.Click += new System.EventHandler(this.cmdBack_Click);
			// 
			// cmdNext
			// 
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.TabIndex = 3;
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.TabIndex = 4;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// grpAutoStart
			// 
			this.grpAutoStart.Controls.Add(this.chkAutoStartServerManager);
			this.grpAutoStart.Controls.Add(this.chkAutoStartServerWorker);
			this.grpAutoStart.Location = new System.Drawing.Point(144, 8);
			this.grpAutoStart.Name = "grpAutoStart";
			this.grpAutoStart.Size = new System.Drawing.Size(344, 224);
			this.grpAutoStart.TabIndex = 1;
			this.grpAutoStart.TabStop = false;
			this.grpAutoStart.Text = "Auto-start Server Applications";
			// 
			// chkAutoStartServerWorker
			// 
			this.chkAutoStartServerWorker.Location = new System.Drawing.Point(8, 24);
			this.chkAutoStartServerWorker.Name = "chkAutoStartServerWorker";
			this.chkAutoStartServerWorker.Size = new System.Drawing.Size(328, 24);
			this.chkAutoStartServerWorker.TabIndex = 0;
			this.chkAutoStartServerWorker.Text = "Start ServerWorker automatically with Windows";
			// 
			// chkAutoStartServerManager
			// 
			this.chkAutoStartServerManager.Location = new System.Drawing.Point(8, 48);
			this.chkAutoStartServerManager.Name = "chkAutoStartServerManager";
			this.chkAutoStartServerManager.Size = new System.Drawing.Size(328, 24);
			this.chkAutoStartServerManager.TabIndex = 1;
			this.chkAutoStartServerManager.Text = "Start ServerManager automatically with Windows";
			// 
			// frmStep5
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 271);
			this.Controls.Add(this.grpAutoStart);
			this.Name = "frmStep5";
			this.Text = "Startup Options (Step 5 of 6)";
			this.Load += new System.EventHandler(this.frmStep5_Load);
			this.Controls.SetChildIndex(this.grpAutoStart, 0);
			this.Controls.SetChildIndex(this.cmdCancel, 0);
			this.Controls.SetChildIndex(this.cmdNext, 0);
			this.Controls.SetChildIndex(this.cmdBack, 0);
			this.grpAutoStart.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmStep5_Load(object sender, System.EventArgs e)
		{
			globals.LoadedForms.Add(this.Name, this);
		}

		private void cmdBack_Click(object sender, System.EventArgs e)
		{
			frmStep4 frm = new frmStep4();
			frm.Show();
			globals.LoadedForms[this.Name] = null;
			globals.LoadedForms.Remove(this.Name);
			this.Close();
		}

		private void cmdNext_Click(object sender, System.EventArgs e)
		{
			if(!SetOptions())
			{
				return;
			}
			frmStep6 frm = new frmStep6();
			frm.Location = this.Location;
			globals.LoadedForms[this.Name] = null;
			globals.LoadedForms.Remove(this.Name);
			frm.Show();
			this.Close();
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("Are you sure you wish to cancel the configuration?", "CrawlWave Server Installer", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
			{
				Application.Exit();
			}
		}

		#endregion

		#region Private Methods

		private bool SetOptions()
		{
			//set the registry keys
			try
			{
				string path = Globals.GetAppPath();
				SetRegistryStartupKey("CrawlWaveServerWorker", path + "CrawlWave.ServerWorker.exe", chkAutoStartServerWorker.Checked);
				SetRegistryStartupKey("CrawlWaveServerManager", path + "CrawlWave.ServerManager.exe", chkAutoStartServerManager.Checked);
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Creates or deletes a value in the Run registry key so that
		/// the application can start automatically when Windows boot.
		/// </summary>
		/// <param name="appName">The name of the application.</param>
		/// <param name="appPath">The path of the application.</param>
		/// <param name="loadAtStartup">
		/// True makes the application load automatically at startup,
		/// False removes it from the Run registry key.
		/// </param>
		private void SetRegistryStartupKey(string appName, string appPath, bool loadAtStartup)
		{
			try
			{
				RegistryKey regKey=Registry.LocalMachine;
				RegistryKey appKey=regKey.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",true);
				if (appKey==null)
				{
					//the Run key does not exist or we can't open it - we're in deep shit.
					return;
				}
				//The value must be created if LoadAtStartup = true
				if(loadAtStartup)
				{
					appKey.SetValue(appName,appPath);
					appKey.Close();
					return;
				}
				else
				{
					//If LoadAtStartup = false the value must be deleted.
					appKey.DeleteValue(appName,false);
					appKey.Close();
					return;
				}
			}
			catch
			{
				//just choke the exception.
			}
		}

		#endregion
	}
}

