using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CrawlWave.ServerInstaller.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class frmStep1 : CrawlWave.ServerInstaller.Forms.frmStep
	{
		private System.Windows.Forms.Label lblDescription;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.GroupBox grpInstallationType;
		private System.Windows.Forms.RadioButton optPrimaryServer;
		private System.Windows.Forms.RadioButton optSecondaryServer;

		private Globals globals;

		/// <summary>
		/// 
		/// </summary>
		public frmStep1()
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
			this.lblDescription = new System.Windows.Forms.Label();
			this.grpInstallationType = new System.Windows.Forms.GroupBox();
			this.optPrimaryServer = new System.Windows.Forms.RadioButton();
			this.optSecondaryServer = new System.Windows.Forms.RadioButton();
			this.grpInstallationType.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblStep1
			// 
			this.lblStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblStep1.ImageIndex = 1;
			this.lblStep1.Name = "lblStep1";
			// 
			// lblStep2
			// 
			this.lblStep2.Name = "lblStep2";
			// 
			// lblStep3
			// 
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
			// 
			// cmdNext
			// 
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// lblDescription
			// 
			this.lblDescription.BackColor = System.Drawing.SystemColors.Window;
			this.lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblDescription.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblDescription.Location = new System.Drawing.Point(144, 8);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(344, 144);
			this.lblDescription.TabIndex = 4;
			this.lblDescription.Text = @"Welcome to the CrawlWave Server Applications installer. This utility will guide you in configuring the details for your system's setup and will assist you with the configuration of the CrawlWave Server Applications. Please select the installation type to suit your setup and click on the Next button to start when you are ready.";
			this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// grpInstallationType
			// 
			this.grpInstallationType.Controls.Add(this.optSecondaryServer);
			this.grpInstallationType.Controls.Add(this.optPrimaryServer);
			this.grpInstallationType.Location = new System.Drawing.Point(144, 160);
			this.grpInstallationType.Name = "grpInstallationType";
			this.grpInstallationType.Size = new System.Drawing.Size(344, 72);
			this.grpInstallationType.TabIndex = 5;
			this.grpInstallationType.TabStop = false;
			this.grpInstallationType.Text = "Select Installation Type";
			// 
			// optPrimaryServer
			// 
			this.optPrimaryServer.Checked = true;
			this.optPrimaryServer.Location = new System.Drawing.Point(8, 16);
			this.optPrimaryServer.Name = "optPrimaryServer";
			this.optPrimaryServer.Size = new System.Drawing.Size(328, 24);
			this.optPrimaryServer.TabIndex = 0;
			this.optPrimaryServer.TabStop = true;
			this.optPrimaryServer.Text = "Primary Server (no other Servers exist, the DB must be built)";
			this.optPrimaryServer.CheckedChanged += new System.EventHandler(this.optPrimaryServer_CheckedChanged);
			// 
			// optSecondaryServer
			// 
			this.optSecondaryServer.Location = new System.Drawing.Point(8, 40);
			this.optSecondaryServer.Name = "optSecondaryServer";
			this.optSecondaryServer.Size = new System.Drawing.Size(328, 24);
			this.optSecondaryServer.TabIndex = 1;
			this.optSecondaryServer.Text = "Secondary Server (use an existing DB and Primary Server)";
			this.optSecondaryServer.CheckedChanged += new System.EventHandler(this.optSecondaryServer_CheckedChanged);
			// 
			// frmStep1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 271);
			this.Controls.Add(this.grpInstallationType);
			this.Controls.Add(this.lblDescription);
			this.Name = "frmStep1";
			this.Text = "Welcome! (Step 1 of 6)";
			this.Load += new System.EventHandler(this.frmStep1_Load);
			this.Controls.SetChildIndex(this.lblDescription, 0);
			this.Controls.SetChildIndex(this.grpInstallationType, 0);
			this.Controls.SetChildIndex(this.cmdCancel, 0);
			this.Controls.SetChildIndex(this.cmdNext, 0);
			this.Controls.SetChildIndex(this.cmdBack, 0);
			this.grpInstallationType.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void optPrimaryServer_CheckedChanged(object sender, System.EventArgs e)
		{
			optSecondaryServer.Checked = !optPrimaryServer.Checked;
		}

		private void optSecondaryServer_CheckedChanged(object sender, System.EventArgs e)
		{
			optPrimaryServer.Checked = !optSecondaryServer.Checked;
		}

		private void cmdNext_Click(object sender, System.EventArgs e)
		{
			if(optPrimaryServer.Checked)
			{
				frmStep2 frm = new frmStep2();
				frm.Location = this.Location;
				this.ShowInTaskbar = false;
				frm.Show();
			}
			else
			{
				frmStep2b frm = new frmStep2b();
				frm.Location = this.Location;
				this.ShowInTaskbar = false;
				frm.Show();
			}
			this.Hide();
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("Are you sure you wish to cancel the configuration?", "CrawlWave Server Installer", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
			{
				Application.Exit();
			}
		}

		private void frmStep1_Load(object sender, System.EventArgs e)
		{
			globals.LoadedForms.Add(this.Name, this);
		}

		#endregion

		#region Main Function

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmStep1());
		}

		#endregion

	}
}

