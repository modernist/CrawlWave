using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CrawlWave.ServerInstaller.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class frmStep3b : CrawlWave.ServerInstaller.Forms.frmStep
	{
		private System.Windows.Forms.Label lblPleaseWait;
		private System.Windows.Forms.GroupBox grpStatus;
		private System.Windows.Forms.TextBox txtStatus;
		private System.Windows.Forms.ProgressBar prgProgress;
		private System.Windows.Forms.Timer tmrStart;
		private System.ComponentModel.IContainer components = null;

		private Globals globals;
		private TextBoxWriter log;
		private SqlConnection dbcon;
		private bool success;

		/// <summary>
		/// 
		/// </summary>
		public frmStep3b()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			globals = Globals.Instance();
			success = true;
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
			this.components = new System.ComponentModel.Container();
			this.lblPleaseWait = new System.Windows.Forms.Label();
			this.grpStatus = new System.Windows.Forms.GroupBox();
			this.txtStatus = new System.Windows.Forms.TextBox();
			this.prgProgress = new System.Windows.Forms.ProgressBar();
			this.tmrStart = new System.Windows.Forms.Timer(this.components);
			this.grpStatus.SuspendLayout();
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
			this.lblStep3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblStep3.ImageIndex = 7;
			this.lblStep3.Name = "lblStep3";
			// 
			// lblStep4
			// 
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
			this.cmdBack.Click += new System.EventHandler(this.cmdBack_Click);
			// 
			// cmdNext
			// 
			this.cmdNext.Enabled = false;
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Enabled = false;
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// lblPleaseWait
			// 
			this.lblPleaseWait.Location = new System.Drawing.Point(144, 8);
			this.lblPleaseWait.Name = "lblPleaseWait";
			this.lblPleaseWait.Size = new System.Drawing.Size(344, 23);
			this.lblPleaseWait.TabIndex = 4;
			this.lblPleaseWait.Text = "Performing requested actions, please wait...";
			this.lblPleaseWait.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// grpStatus
			// 
			this.grpStatus.Controls.Add(this.txtStatus);
			this.grpStatus.Controls.Add(this.prgProgress);
			this.grpStatus.Location = new System.Drawing.Point(144, 32);
			this.grpStatus.Name = "grpStatus";
			this.grpStatus.Size = new System.Drawing.Size(344, 200);
			this.grpStatus.TabIndex = 5;
			this.grpStatus.TabStop = false;
			this.grpStatus.Text = "Status";
			// 
			// txtStatus
			// 
			this.txtStatus.Location = new System.Drawing.Point(8, 16);
			this.txtStatus.Multiline = true;
			this.txtStatus.Name = "txtStatus";
			this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtStatus.Size = new System.Drawing.Size(328, 152);
			this.txtStatus.TabIndex = 0;
			this.txtStatus.Text = "";
			// 
			// prgProgress
			// 
			this.prgProgress.Location = new System.Drawing.Point(8, 176);
			this.prgProgress.Name = "prgProgress";
			this.prgProgress.Size = new System.Drawing.Size(328, 16);
			this.prgProgress.TabIndex = 1;
			// 
			// tmrStart
			// 
			this.tmrStart.Enabled = true;
			this.tmrStart.Tick += new System.EventHandler(this.tmrStart_Tick);
			// 
			// frmStep3b
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 271);
			this.Controls.Add(this.lblPleaseWait);
			this.Controls.Add(this.grpStatus);
			this.Name = "frmStep3b";
			this.Text = "Building Database (Step 3 of 6)";
			this.Load += new System.EventHandler(this.frmStep3b_Load);
			this.Controls.SetChildIndex(this.cmdCancel, 0);
			this.Controls.SetChildIndex(this.cmdNext, 0);
			this.Controls.SetChildIndex(this.cmdBack, 0);
			this.Controls.SetChildIndex(this.grpStatus, 0);
			this.Controls.SetChildIndex(this.lblPleaseWait, 0);
			this.grpStatus.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmStep3b_Load(object sender, System.EventArgs e)
		{
			globals.LoadedForms.Add(this.Name, this);
		}

		private void tmrStart_Tick(object sender, System.EventArgs e)
		{
			tmrStart.Enabled = false;
			log = new TextBoxWriter(txtStatus);
			CheckDatabase();
		}

		private void cmdBack_Click(object sender, System.EventArgs e)
		{
			frmStep2b frm = new frmStep2b();
			frm.Show();
			globals.LoadedForms[this.Name] = null;
			globals.LoadedForms.Remove(this.Name);
			this.Close();
		}

		private void cmdNext_Click(object sender, System.EventArgs e)
		{
			frmStep4 frm = new frmStep4();
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

		#region Private methods

		private void CheckDatabase()
		{
			try
			{
				log.WriteLine("Performing required actions");
				
				ConnectToDatabase();
				if(!success)
				{
					return;
				}
				//Perform additional tasks
				PerformExtraTasks();
				//close connection if it is open
				DisconnectFromDatabase();

				log.WriteLine("All actions completed successfully.");
			}
			catch
			{
				//close connection if it is open
				DisconnectFromDatabase();
			}
			finally
			{
				//enable controls
				cmdNext.Enabled = success;
				cmdBack.Enabled = !success;
				cmdCancel.Enabled = true;
			}
		}

		private void ConnectToDatabase()
		{
			try
			{
				log.WriteLine("Checking connection to the database...");

				StringBuilder sb = new StringBuilder("Password=");
				sb.Append(globals.ConfigurationSettings.CWPass);
				sb.Append(";Persist Security Info=True;User ID=");
				sb.Append(globals.ConfigurationSettings.CWUser);
				sb.Append(";Initial Catalog=CrawlWave;Data Source=");
				sb.Append(globals.ConfigurationSettings.SQLServer);
				sb.Append(";Application Name =CrawlWave.ServerInstaller;");
				dbcon = new SqlConnection(sb.ToString());
				sb = null;

				log.WriteLine("Opening connection to database...");
				dbcon.Open();
				
				prgProgress.Value = 50;
				log.WriteLine("Connection to database succeeded.");
			}
			catch(Exception e)
			{
				log.WriteLine("Connection to database failed: " + e.Message);
				prgProgress.Value = 10;
				success = false;
			}
		}

		private void PerformExtraTasks()
		{
			log.WriteLine("Performing additional actions... done.");
			prgProgress.Value = 100;
		}

		private void DisconnectFromDatabase()
		{
			log.WriteLine("Closing connection to the database...");
			try
			{
				if(dbcon!=null)
				{
					dbcon.Close();
					dbcon.Dispose();
				}
			}
			catch
			{}
		}

		#endregion
	}
}

