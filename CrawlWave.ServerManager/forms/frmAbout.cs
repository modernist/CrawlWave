using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerManager.Forms
{
	/// <summary>
	/// frmAbout displays various information about CrawlWave Server Manager.
	/// </summary>
	public class frmAbout : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox picAbout;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblProject;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Globals globals;

		/// <summary>
		/// Constructs a new instance of the <see cref="frmAbout"/> form.
		/// </summary>
		public frmAbout()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			globals = Globals.Instance();
			globals.LoadedForms[this.Name] = this;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmAbout));
			this.picAbout = new System.Windows.Forms.PictureBox();
			this.cmdClose = new System.Windows.Forms.Button();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblProject = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// picAbout
			// 
			this.picAbout.Image = ((System.Drawing.Image)(resources.GetObject("picAbout.Image")));
			this.picAbout.Location = new System.Drawing.Point(8, 8);
			this.picAbout.Name = "picAbout";
			this.picAbout.Size = new System.Drawing.Size(448, 200);
			this.picAbout.TabIndex = 0;
			this.picAbout.TabStop = false;
			// 
			// cmdClose
			// 
			this.cmdClose.BackColor = System.Drawing.SystemColors.Window;
			this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdClose.Location = new System.Drawing.Point(368, 176);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(80, 24);
			this.cmdClose.TabIndex = 1;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// lblCopyright
			// 
			this.lblCopyright.BackColor = System.Drawing.SystemColors.Window;
			this.lblCopyright.Location = new System.Drawing.Point(240, 152);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(208, 24);
			this.lblCopyright.TabIndex = 3;
			this.lblCopyright.Text = "© Kostas Stroggylos 2003-2004";
			this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblVersion
			// 
			this.lblVersion.BackColor = System.Drawing.SystemColors.Window;
			this.lblVersion.Location = new System.Drawing.Point(272, 128);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(176, 23);
			this.lblVersion.TabIndex = 4;
			this.lblVersion.Text = "Version:";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblProject
			// 
			this.lblProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblProject.Location = new System.Drawing.Point(152, 104);
			this.lblProject.Name = "lblProject";
			this.lblProject.Size = new System.Drawing.Size(296, 23);
			this.lblProject.TabIndex = 5;
			this.lblProject.Text = "A.U.E.B. Distributed Web Crawling Project";
			this.lblProject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// frmAbout
			// 
			this.AcceptButton = this.cmdClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.cmdClose;
			this.ClientSize = new System.Drawing.Size(466, 215);
			this.Controls.Add(this.lblProject);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lblCopyright);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.picAbout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAbout";
			this.Text = "About CrawlWave Server Manager";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmAbout_Closing);
			this.Load += new System.EventHandler(this.frmAbout_Load);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void cmdClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void frmAbout_Load(object sender, System.EventArgs e)
		{
			lblVersion.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		private void frmAbout_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            globals.LoadedForms[this.Name] = null;
		}

		#endregion
	}
}
