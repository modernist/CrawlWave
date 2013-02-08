using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CrawlWave.ServerInstaller.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class frmStep4 : CrawlWave.ServerInstaller.Forms.frmStep
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.PictureBox picWarning;
		private System.Windows.Forms.FolderBrowserDialog dlgBrowse;
		private System.Windows.Forms.GroupBox grpDataFilesPath;
		private System.Windows.Forms.Label lblSelectPath;
		private System.Windows.Forms.Button cmdBrowse;
		private System.Windows.Forms.TextBox txtDataFilesPath;
		private System.Windows.Forms.Label lblWarning;

		private Globals globals;

		/// <summary>
		/// 
		/// </summary>
		public frmStep4()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmStep4));
			this.lblSelectPath = new System.Windows.Forms.Label();
			this.cmdBrowse = new System.Windows.Forms.Button();
			this.txtDataFilesPath = new System.Windows.Forms.TextBox();
			this.grpDataFilesPath = new System.Windows.Forms.GroupBox();
			this.picWarning = new System.Windows.Forms.PictureBox();
			this.lblWarning = new System.Windows.Forms.Label();
			this.dlgBrowse = new System.Windows.Forms.FolderBrowserDialog();
			this.grpDataFilesPath.SuspendLayout();
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
			this.lblStep4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblStep4.ImageIndex = 10;
			this.lblStep4.Name = "lblStep4";
			// 
			// lblStep5
			// 
			this.lblStep5.Name = "lblStep5";
			// 
			// lblStep6
			// 
			this.lblStep6.Name = "lblStep6";
			// 
			// cmdBack
			// 
			this.cmdBack.Enabled = false;
			this.cmdBack.Name = "cmdBack";
			this.cmdBack.TabIndex = 2;
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
			// lblSelectPath
			// 
			this.lblSelectPath.Location = new System.Drawing.Point(8, 24);
			this.lblSelectPath.Name = "lblSelectPath";
			this.lblSelectPath.Size = new System.Drawing.Size(328, 32);
			this.lblSelectPath.TabIndex = 0;
			this.lblSelectPath.Text = "Please select the path the server will use to temporarily store the Url Crawl dat" +
				"a files before they are used to update the database.";
			// 
			// cmdBrowse
			// 
			this.cmdBrowse.Location = new System.Drawing.Point(264, 56);
			this.cmdBrowse.Name = "cmdBrowse";
			this.cmdBrowse.Size = new System.Drawing.Size(72, 24);
			this.cmdBrowse.TabIndex = 2;
			this.cmdBrowse.Text = "Browse...";
			this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
			// 
			// txtDataFilesPath
			// 
			this.txtDataFilesPath.Location = new System.Drawing.Point(8, 56);
			this.txtDataFilesPath.Name = "txtDataFilesPath";
			this.txtDataFilesPath.ReadOnly = true;
			this.txtDataFilesPath.Size = new System.Drawing.Size(248, 20);
			this.txtDataFilesPath.TabIndex = 1;
			this.txtDataFilesPath.Text = "";
			// 
			// grpDataFilesPath
			// 
			this.grpDataFilesPath.Controls.Add(this.picWarning);
			this.grpDataFilesPath.Controls.Add(this.lblWarning);
			this.grpDataFilesPath.Controls.Add(this.txtDataFilesPath);
			this.grpDataFilesPath.Controls.Add(this.lblSelectPath);
			this.grpDataFilesPath.Controls.Add(this.cmdBrowse);
			this.grpDataFilesPath.Location = new System.Drawing.Point(144, 8);
			this.grpDataFilesPath.Name = "grpDataFilesPath";
			this.grpDataFilesPath.Size = new System.Drawing.Size(344, 152);
			this.grpDataFilesPath.TabIndex = 1;
			this.grpDataFilesPath.TabStop = false;
			this.grpDataFilesPath.Text = "Temporary Data Files Path";
			// 
			// picWarning
			// 
			this.picWarning.Image = ((System.Drawing.Image)(resources.GetObject("picWarning.Image")));
			this.picWarning.Location = new System.Drawing.Point(16, 88);
			this.picWarning.Name = "picWarning";
			this.picWarning.Size = new System.Drawing.Size(48, 48);
			this.picWarning.TabIndex = 8;
			this.picWarning.TabStop = false;
			// 
			// lblWarning
			// 
			this.lblWarning.Location = new System.Drawing.Point(64, 88);
			this.lblWarning.Name = "lblWarning";
			this.lblWarning.Size = new System.Drawing.Size(272, 56);
			this.lblWarning.TabIndex = 3;
			this.lblWarning.Text = "Warning: You must make sure that the folder you select can be written to by the A" +
				"SPNET machine account, otherwise the server will fail to store crawl data and th" +
				"us perform the crawling process.";
			// 
			// dlgBrowse
			// 
			this.dlgBrowse.Description = "Select the location for CrawlWave\'s temporary files";
			// 
			// frmStep4
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 271);
			this.Controls.Add(this.grpDataFilesPath);
			this.Name = "frmStep4";
			this.Text = "Data Location and IIS Settings Selection (Step 4 of 6)";
			this.Load += new System.EventHandler(this.frmStep4_Load);
			this.Controls.SetChildIndex(this.grpDataFilesPath, 0);
			this.Controls.SetChildIndex(this.cmdCancel, 0);
			this.Controls.SetChildIndex(this.cmdNext, 0);
			this.Controls.SetChildIndex(this.cmdBack, 0);
			this.grpDataFilesPath.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmStep4_Load(object sender, System.EventArgs e)
		{
			globals.LoadedForms.Add(this.Name, this);
			if(globals.ConfigurationSettings.DataFilesPath!= String.Empty)
			{
				txtDataFilesPath.Text = globals.ConfigurationSettings.DataFilesPath;
			}
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("Are you sure you wish to cancel the configuration?", "CrawlWave Server Installer", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
			{
				Application.Exit();
			}
		}

		private void cmdNext_Click(object sender, System.EventArgs e)
		{
			if(!ValidateForm())
			{
				return;
			}
			if(!SetOptions())
			{
				return;
			}
			if(!SetWebServiceOptions())
			{
				return;
			}
			if(!CreateVirtualDirectory())
			{
				return;
			}
			frmStep5 frm = new frmStep5();
			frm.Location = this.Location;
			globals.LoadedForms[this.Name] = null;
			globals.LoadedForms.Remove(this.Name);
			frm.Show();
			this.Close();
		}

		private void cmdBrowse_Click(object sender, System.EventArgs e)
		{
			dlgBrowse.ShowDialog();
			if(dlgBrowse.SelectedPath!=String.Empty)
			{
				txtDataFilesPath.Text = dlgBrowse.SelectedPath;
				if(!dlgBrowse.SelectedPath.EndsWith("\\"))
				{
					txtDataFilesPath.AppendText("\\");
				}
				dlgBrowse.SelectedPath = String.Empty;
			}
		}

		#endregion

		#region Private Methods

		private bool ValidateForm()
		{
			if(txtDataFilesPath.Text == String.Empty)
			{
				MessageBox.Show("You must select the folder where the temporary data files will be stored!", "CrawlWave Server Installer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				cmdBrowse.Focus();
				return false;
			}
			return true;
		}

		private bool SetOptions()
		{
			try
			{
				globals.ConfigurationSettings.DataFilesPath = txtDataFilesPath.Text;
				//try to set the settings in the ServerCommon's configuration file
				//it must be stored in the same directory
				string path = Globals.GetAppPath() + "CrawlWave.ServerCommon.Config.xml";
				
				StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CrawlWave.ServerInstaller.config.CrawlWave.ServerCommon.Config.xml"));
				string configuration = sr.ReadToEnd();
				sr.Close();

				//replace the @SQLServer@, @SQLLogin@, @SQLPass@, @MaxDBSize@ values
				configuration = Regex.Replace(configuration, "@SQLServer@", globals.ConfigurationSettings.SQLServer);
				if(globals.ConfigurationSettings.CWUser == String.Empty)
				{
					configuration = Regex.Replace(configuration, "@SQLLogin@", globals.ConfigurationSettings.DBAUser);
					configuration = Regex.Replace(configuration, "@SQLPass@", globals.ConfigurationSettings.DBAPass);
				}
				else
				{
					configuration = Regex.Replace(configuration, "@SQLLogin@", globals.ConfigurationSettings.CWUser);
					configuration = Regex.Replace(configuration, "@SQLPass@", globals.ConfigurationSettings.CWPass);
				}
				configuration = Regex.Replace(configuration, "@MaxDBSize@", globals.ConfigurationSettings.DBSizeMax.ToString());
				configuration = Regex.Replace(configuration, "@DataFilesPath@", globals.ConfigurationSettings.DataFilesPath);
			
				StreamWriter sw = new StreamWriter(path);
				sw.Write(configuration);
				sw.Close();
				
			}
			catch(Exception e)
			{
				MessageBox.Show("An error occured while configuring the ServerCommon component: " + e.Message, "CrawlWave Server Installer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			return true;
		}

		private bool SetWebServiceOptions()
		{
			try
			{
				globals.ConfigurationSettings.DataFilesPath = txtDataFilesPath.Text;
				//try to set the settings in the Web Service's configuration file
				//it must be stored in the same directory
				string path = Globals.GetAppPath() + "Web.config";
				
				if(File.Exists(path))
				{
					StreamReader sr = new StreamReader(path);
					string configuration = sr.ReadToEnd();
					sr.Close();

					//replace the @DataFilesPath@, @SQLServer@, @SQLLogin@, @SQLPass@ values
					configuration = Regex.Replace(configuration, "@DataFilesPath@", globals.ConfigurationSettings.DataFilesPath.Replace("\\","\\\\"));
					configuration = Regex.Replace(configuration, "@SQLServer@", globals.ConfigurationSettings.SQLServer);
					if(globals.ConfigurationSettings.CWUser == String.Empty)
					{
						configuration = Regex.Replace(configuration, "@SQLLogin@", globals.ConfigurationSettings.DBAUser);
						configuration = Regex.Replace(configuration, "@SQLPass@", globals.ConfigurationSettings.DBAPass);
					}
					else
					{
						configuration = Regex.Replace(configuration, "@SQLLogin@", globals.ConfigurationSettings.CWUser);
						configuration = Regex.Replace(configuration, "@SQLPass@", globals.ConfigurationSettings.CWPass);
					}

					//store the updated file to disk
					StreamWriter sw = new StreamWriter(path);
					sw.Write(configuration);
					sw.Close();
				}
			}
			catch(Exception e)
			{
				MessageBox.Show("An error occured while configuring the Web Service options: " + e.Message, "CrawlWave Server Installer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			return true;
		}

		private bool CreateVirtualDirectory()
		{
			try
			{
				VirtualDirectoryHelper helper = new VirtualDirectoryHelper();
				helper.Connect();
				helper.CreateVirtualDirectory("CrawlWave.Server", "CrawlWave.Server", Globals.GetAppPath());
				//helper.DeleteVirtualDirectory("CrawlWaveTest", Globals.GetAppPath());
			}
			catch(Exception e)
			{
				MessageBox.Show("An error occured while creating the Server's Virtual Directory: " + e.Message, "CrawlWave Server Installer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			return true;
		}

		#endregion
	}
}

