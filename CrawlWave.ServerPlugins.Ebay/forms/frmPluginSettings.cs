using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrawlWave.ServerPlugins.Ebay
{
	/// <summary>
	/// frmPluginSettings is the form that displays the Plugin's settings and allows the 
	/// user to change and save them.
	/// </summary>
	public class frmPluginSettings : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.TabControl tabSettings;
		private System.Windows.Forms.ComboBox cmbPause;
		private System.Windows.Forms.CheckBox chkPause;
		private System.Windows.Forms.Label lblPause;
		private System.Windows.Forms.PictureBox picPause;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imlTabIcons;
		private System.Windows.Forms.TabPage tabDataBase;
		private System.Windows.Forms.CheckBox chkUseTransactions;
		private System.Windows.Forms.NumericUpDown nudDBActionTimeout;
		private System.Windows.Forms.Label lblDBActionTimeout;
		private PluginSettings settings;

		/// <summary>
		/// Constructs a new instance of the Plugin's settings form.
		/// </summary>
		public frmPluginSettings()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			settings = PluginSettings.Instance();
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmPluginSettings));
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.tabSettings = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.imlTabIcons = new System.Windows.Forms.ImageList(this.components);
			this.picPause = new System.Windows.Forms.PictureBox();
			this.lblPause = new System.Windows.Forms.Label();
			this.cmbPause = new System.Windows.Forms.ComboBox();
			this.chkPause = new System.Windows.Forms.CheckBox();
			this.tabDataBase = new System.Windows.Forms.TabPage();
			this.nudDBActionTimeout = new System.Windows.Forms.NumericUpDown();
			this.lblDBActionTimeout = new System.Windows.Forms.Label();
			this.chkUseTransactions = new System.Windows.Forms.CheckBox();
			this.tabSettings.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.tabDataBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDBActionTimeout)).BeginInit();
			this.SuspendLayout();
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(8, 152);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.TabIndex = 9;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(88, 152);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.TabIndex = 10;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.tabGeneral);
			this.tabSettings.Controls.Add(this.tabDataBase);
			this.tabSettings.ImageList = this.imlTabIcons;
			this.tabSettings.Location = new System.Drawing.Point(8, 8);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.SelectedIndex = 0;
			this.tabSettings.Size = new System.Drawing.Size(264, 136);
			this.tabSettings.TabIndex = 11;
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.picPause);
			this.tabGeneral.Controls.Add(this.lblPause);
			this.tabGeneral.Controls.Add(this.cmbPause);
			this.tabGeneral.Controls.Add(this.chkPause);
			this.tabGeneral.ImageIndex = 0;
			this.tabGeneral.Location = new System.Drawing.Point(4, 23);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Size = new System.Drawing.Size(256, 109);
			this.tabGeneral.TabIndex = 0;
			this.tabGeneral.Text = "General";
			// 
			// imlTabIcons
			// 
			this.imlTabIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imlTabIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlTabIcons.ImageStream")));
			this.imlTabIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// picPause
			// 
			this.picPause.Image = ((System.Drawing.Image)(resources.GetObject("picPause.Image")));
			this.picPause.Location = new System.Drawing.Point(8, 80);
			this.picPause.Name = "picPause";
			this.picPause.Size = new System.Drawing.Size(24, 24);
			this.picPause.TabIndex = 18;
			this.picPause.TabStop = false;
			// 
			// lblPause
			// 
			this.lblPause.Location = new System.Drawing.Point(40, 80);
			this.lblPause.Name = "lblPause";
			this.lblPause.Size = new System.Drawing.Size(104, 23);
			this.lblPause.TabIndex = 16;
			this.lblPause.Text = "Pause delay";
			this.lblPause.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmbPause
			// 
			this.cmbPause.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPause.Items.AddRange(new object[] {
														  "15 seconds",
														  "30 seconds",
														  "45 seconds",
														  "1 minute"});
			this.cmbPause.Location = new System.Drawing.Point(144, 80);
			this.cmbPause.Name = "cmbPause";
			this.cmbPause.Size = new System.Drawing.Size(104, 21);
			this.cmbPause.TabIndex = 15;
			// 
			// chkPause
			// 
			this.chkPause.Location = new System.Drawing.Point(8, 56);
			this.chkPause.Name = "chkPause";
			this.chkPause.Size = new System.Drawing.Size(240, 24);
			this.chkPause.TabIndex = 14;
			this.chkPause.Text = "Pause between consecutive operations";
			this.chkPause.CheckedChanged += new System.EventHandler(this.chkPause_CheckedChanged);
			// 
			// tabDataBase
			// 
			this.tabDataBase.Controls.Add(this.nudDBActionTimeout);
			this.tabDataBase.Controls.Add(this.lblDBActionTimeout);
			this.tabDataBase.Controls.Add(this.chkUseTransactions);
			this.tabDataBase.ImageIndex = 2;
			this.tabDataBase.Location = new System.Drawing.Point(4, 23);
			this.tabDataBase.Name = "tabDataBase";
			this.tabDataBase.Size = new System.Drawing.Size(256, 109);
			this.tabDataBase.TabIndex = 1;
			this.tabDataBase.Text = "Database";
			// 
			// nudDBActionTimeout
			// 
			this.nudDBActionTimeout.Increment = new System.Decimal(new int[] {
																				 30,
																				 0,
																				 0,
																				 0});
			this.nudDBActionTimeout.Location = new System.Drawing.Point(176, 32);
			this.nudDBActionTimeout.Maximum = new System.Decimal(new int[] {
																			   6000,
																			   0,
																			   0,
																			   0});
			this.nudDBActionTimeout.Minimum = new System.Decimal(new int[] {
																			   60,
																			   0,
																			   0,
																			   0});
			this.nudDBActionTimeout.Name = "nudDBActionTimeout";
			this.nudDBActionTimeout.Size = new System.Drawing.Size(72, 20);
			this.nudDBActionTimeout.TabIndex = 2;
			this.nudDBActionTimeout.Value = new System.Decimal(new int[] {
																			 60,
																			 0,
																			 0,
																			 0});
			// 
			// lblDBActionTimeout
			// 
			this.lblDBActionTimeout.Location = new System.Drawing.Point(8, 32);
			this.lblDBActionTimeout.Name = "lblDBActionTimeout";
			this.lblDBActionTimeout.Size = new System.Drawing.Size(168, 23);
			this.lblDBActionTimeout.TabIndex = 1;
			this.lblDBActionTimeout.Text = "Database Action Timeout (sec)";
			this.lblDBActionTimeout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chkUseTransactions
			// 
			this.chkUseTransactions.Location = new System.Drawing.Point(8, 8);
			this.chkUseTransactions.Name = "chkUseTransactions";
			this.chkUseTransactions.Size = new System.Drawing.Size(240, 24);
			this.chkUseTransactions.TabIndex = 0;
			this.chkUseTransactions.Text = "Use Transactions (slower but more safe)";
			// 
			// frmPluginSettings
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(280, 181);
			this.Controls.Add(this.tabSettings);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmPluginSettings";
			this.Text = "DBUpdater Plugin Settings";
			this.Load += new System.EventHandler(this.frmPluginSettings_Load);
			this.tabSettings.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.tabDataBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudDBActionTimeout)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void chkPause_CheckedChanged(object sender, System.EventArgs e)
		{
			cmbPause.Enabled = chkPause.Checked;
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			settings.PauseBetweenOperations = chkPause.Checked;
			if(chkPause.Checked)
			{
				settings.PauseDelay = cmbPause.SelectedIndex;
			}
			settings.UseTransactions = chkUseTransactions.Checked;
			settings.DBActionTimeout = (int)nudDBActionTimeout.Value;
			settings.SaveSettings();
			this.Close();
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void frmPluginSettings_Load(object sender, System.EventArgs e)
		{
			chkPause.Checked = settings.PauseBetweenOperations;
			cmbPause.Enabled = settings.PauseBetweenOperations;
			if((settings.PauseDelay >= 0)&&(settings.PauseDelay < cmbPause.Items.Count))
			{
				cmbPause.SelectedIndex = settings.PauseDelay;
			}
            chkUseTransactions.Checked = settings.UseTransactions;
			if((settings.DBActionTimeout >= nudDBActionTimeout.Minimum) && (settings.DBActionTimeout <= nudDBActionTimeout.Maximum))
			{
				nudDBActionTimeout.Value = settings.DBActionTimeout;
			}
		}

		#endregion

	}
}
