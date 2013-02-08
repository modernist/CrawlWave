using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrawlWave.ServerPlugins.WordExtraction
{
	/// <summary>
	/// Summary description for frmPluginSettings.
	/// </summary>
	public class frmPluginSettings : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tabSettings;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.TabPage tabDBSettings;
		private System.Windows.Forms.TabPage tabWESettings;
		private System.Windows.Forms.CheckBox chkUseDatabase;
		private System.Windows.Forms.ComboBox cmbPause;
		private System.Windows.Forms.Label lblPause;
		private System.Windows.Forms.PictureBox picExtract;
		private System.Windows.Forms.PictureBox picDatabase;
		private System.Windows.Forms.CheckBox chkPause;
		private System.Windows.Forms.CheckBox chkExtractMeta;
		private System.Windows.Forms.CheckBox chkExtractTitle;
		private System.Windows.Forms.CheckBox chkSpellCheck;
		private System.Windows.Forms.Label lblDBActionTimeout;
		private System.Windows.Forms.NumericUpDown nudDBActionTimeout;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private PluginSettings settings;

		/// <summary>
		/// frmPluginSettings is the form that displays the plugin's settings and allows
		/// the user to change and store them.
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmPluginSettings));
			this.tabSettings = new System.Windows.Forms.TabControl();
			this.tabDBSettings = new System.Windows.Forms.TabPage();
			this.picDatabase = new System.Windows.Forms.PictureBox();
			this.lblPause = new System.Windows.Forms.Label();
			this.cmbPause = new System.Windows.Forms.ComboBox();
			this.chkPause = new System.Windows.Forms.CheckBox();
			this.chkUseDatabase = new System.Windows.Forms.CheckBox();
			this.tabWESettings = new System.Windows.Forms.TabPage();
			this.chkSpellCheck = new System.Windows.Forms.CheckBox();
			this.picExtract = new System.Windows.Forms.PictureBox();
			this.chkExtractMeta = new System.Windows.Forms.CheckBox();
			this.chkExtractTitle = new System.Windows.Forms.CheckBox();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.lblDBActionTimeout = new System.Windows.Forms.Label();
			this.nudDBActionTimeout = new System.Windows.Forms.NumericUpDown();
			this.tabSettings.SuspendLayout();
			this.tabDBSettings.SuspendLayout();
			this.tabWESettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDBActionTimeout)).BeginInit();
			this.SuspendLayout();
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.tabDBSettings);
			this.tabSettings.Controls.Add(this.tabWESettings);
			this.tabSettings.Location = new System.Drawing.Point(8, 8);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.SelectedIndex = 0;
			this.tabSettings.Size = new System.Drawing.Size(264, 136);
			this.tabSettings.TabIndex = 0;
			// 
			// tabDBSettings
			// 
			this.tabDBSettings.Controls.Add(this.nudDBActionTimeout);
			this.tabDBSettings.Controls.Add(this.lblDBActionTimeout);
			this.tabDBSettings.Controls.Add(this.picDatabase);
			this.tabDBSettings.Controls.Add(this.lblPause);
			this.tabDBSettings.Controls.Add(this.cmbPause);
			this.tabDBSettings.Controls.Add(this.chkPause);
			this.tabDBSettings.Controls.Add(this.chkUseDatabase);
			this.tabDBSettings.Location = new System.Drawing.Point(4, 22);
			this.tabDBSettings.Name = "tabDBSettings";
			this.tabDBSettings.Size = new System.Drawing.Size(256, 110);
			this.tabDBSettings.TabIndex = 0;
			this.tabDBSettings.Text = "Database";
			// 
			// picDatabase
			// 
			this.picDatabase.Image = ((System.Drawing.Image)(resources.GetObject("picDatabase.Image")));
			this.picDatabase.Location = new System.Drawing.Point(216, 8);
			this.picDatabase.Name = "picDatabase";
			this.picDatabase.Size = new System.Drawing.Size(32, 32);
			this.picDatabase.TabIndex = 4;
			this.picDatabase.TabStop = false;
			// 
			// lblPause
			// 
			this.lblPause.Location = new System.Drawing.Point(8, 80);
			this.lblPause.Name = "lblPause";
			this.lblPause.Size = new System.Drawing.Size(56, 23);
			this.lblPause.TabIndex = 4;
			this.lblPause.Text = "Pause for:";
			this.lblPause.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmbPause
			// 
			this.cmbPause.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPause.Items.AddRange(new object[] {
														  "15 seconds",
														  "30 seconds",
														  "1 minute",
														  "5 minutes"});
			this.cmbPause.Location = new System.Drawing.Point(64, 80);
			this.cmbPause.Name = "cmbPause";
			this.cmbPause.Size = new System.Drawing.Size(128, 21);
			this.cmbPause.TabIndex = 5;
			this.cmbPause.SelectedIndexChanged += new System.EventHandler(this.cmbPause_SelectedIndexChanged);
			// 
			// chkPause
			// 
			this.chkPause.Location = new System.Drawing.Point(8, 56);
			this.chkPause.Name = "chkPause";
			this.chkPause.Size = new System.Drawing.Size(240, 24);
			this.chkPause.TabIndex = 3;
			this.chkPause.Text = "Pause between consecutive operations";
			this.chkPause.CheckedChanged += new System.EventHandler(this.chkPause_CheckedChanged);
			// 
			// chkUseDatabase
			// 
			this.chkUseDatabase.Location = new System.Drawing.Point(8, 8);
			this.chkUseDatabase.Name = "chkUseDatabase";
			this.chkUseDatabase.Size = new System.Drawing.Size(240, 24);
			this.chkUseDatabase.TabIndex = 0;
			this.chkUseDatabase.Text = "Perform Word Extraction in database";
			this.chkUseDatabase.CheckedChanged += new System.EventHandler(this.chkUseDatabase_CheckedChanged);
			// 
			// tabWESettings
			// 
			this.tabWESettings.Controls.Add(this.chkSpellCheck);
			this.tabWESettings.Controls.Add(this.picExtract);
			this.tabWESettings.Controls.Add(this.chkExtractMeta);
			this.tabWESettings.Controls.Add(this.chkExtractTitle);
			this.tabWESettings.Location = new System.Drawing.Point(4, 22);
			this.tabWESettings.Name = "tabWESettings";
			this.tabWESettings.Size = new System.Drawing.Size(256, 110);
			this.tabWESettings.TabIndex = 1;
			this.tabWESettings.Text = "Word Extraction";
			// 
			// chkSpellCheck
			// 
			this.chkSpellCheck.Location = new System.Drawing.Point(8, 72);
			this.chkSpellCheck.Name = "chkSpellCheck";
			this.chkSpellCheck.Size = new System.Drawing.Size(240, 32);
			this.chkSpellCheck.TabIndex = 2;
			this.chkSpellCheck.Text = "Perfom Spell Checking on extracted words";
			this.chkSpellCheck.CheckedChanged += new System.EventHandler(this.chkSpellCheck_CheckedChanged);
			// 
			// picExtract
			// 
			this.picExtract.Image = ((System.Drawing.Image)(resources.GetObject("picExtract.Image")));
			this.picExtract.Location = new System.Drawing.Point(216, 8);
			this.picExtract.Name = "picExtract";
			this.picExtract.Size = new System.Drawing.Size(32, 32);
			this.picExtract.TabIndex = 2;
			this.picExtract.TabStop = false;
			// 
			// chkExtractMeta
			// 
			this.chkExtractMeta.Location = new System.Drawing.Point(8, 40);
			this.chkExtractMeta.Name = "chkExtractMeta";
			this.chkExtractMeta.Size = new System.Drawing.Size(240, 32);
			this.chkExtractMeta.TabIndex = 1;
			this.chkExtractMeta.Text = "Extract keywords from the keywords and description meta tags of html documents.";
			this.chkExtractMeta.CheckedChanged += new System.EventHandler(this.chkExtractMeta_CheckedChanged);
			// 
			// chkExtractTitle
			// 
			this.chkExtractTitle.Location = new System.Drawing.Point(8, 8);
			this.chkExtractTitle.Name = "chkExtractTitle";
			this.chkExtractTitle.Size = new System.Drawing.Size(240, 32);
			this.chkExtractTitle.TabIndex = 0;
			this.chkExtractTitle.Text = "Extract keywords from the <title> tag of html documents.";
			this.chkExtractTitle.CheckedChanged += new System.EventHandler(this.chkExtractTitle_CheckedChanged);
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(8, 152);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.TabIndex = 1;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(88, 152);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.TabIndex = 2;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// lblDBActionTimeout
			// 
			this.lblDBActionTimeout.Location = new System.Drawing.Point(8, 32);
			this.lblDBActionTimeout.Name = "lblDBActionTimeout";
			this.lblDBActionTimeout.Size = new System.Drawing.Size(128, 23);
			this.lblDBActionTimeout.TabIndex = 1;
			this.lblDBActionTimeout.Text = "Database Timeout (sec)";
			this.lblDBActionTimeout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// nudDBActionTimeout
			// 
			this.nudDBActionTimeout.Increment = new System.Decimal(new int[] {
																				 15,
																				 0,
																				 0,
																				 0});
			this.nudDBActionTimeout.Location = new System.Drawing.Point(136, 32);
			this.nudDBActionTimeout.Maximum = new System.Decimal(new int[] {
																			   600,
																			   0,
																			   0,
																			   0});
			this.nudDBActionTimeout.Minimum = new System.Decimal(new int[] {
																			   30,
																			   0,
																			   0,
																			   0});
			this.nudDBActionTimeout.Name = "nudDBActionTimeout";
			this.nudDBActionTimeout.Size = new System.Drawing.Size(56, 20);
			this.nudDBActionTimeout.TabIndex = 2;
			this.nudDBActionTimeout.Value = new System.Decimal(new int[] {
																			 30,
																			 0,
																			 0,
																			 0});
			// 
			// frmPluginSettings
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(282, 183);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.tabSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmPluginSettings";
			this.Text = "Word Extraction Plugin Settings";
			this.Load += new System.EventHandler(this.frmPluginSettings_Load);
			this.tabSettings.ResumeLayout(false);
			this.tabDBSettings.ResumeLayout(false);
			this.tabWESettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudDBActionTimeout)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void chkUseDatabase_CheckedChanged(object sender, System.EventArgs e)
		{
			chkPause.Enabled = chkUseDatabase.Checked;
		}

		private void chkPause_CheckedChanged(object sender, System.EventArgs e)
		{
			cmbPause.Enabled = chkPause.Checked;
		}

		private void cmbPause_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//nothing to be done here
		}

		private void chkExtractTitle_CheckedChanged(object sender, System.EventArgs e)
		{
			//nothing to be done here
		}

		private void chkExtractMeta_CheckedChanged(object sender, System.EventArgs e)
		{
			//nothing to be done here
		}

		private void chkSpellCheck_CheckedChanged(object sender, System.EventArgs e)
		{
			//nothing to be done here
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			//update the settings and save them
			settings.UseDatabase = chkUseDatabase.Checked;
			settings.PauseBetweenOperations = chkPause.Checked;
			if(chkPause.Checked)
			{
				settings.PauseDelay = cmbPause.SelectedIndex;
			}
			settings.ExtractMetaTags = chkExtractMeta.Checked;
			settings.ExtractTitleTag = chkExtractTitle.Checked;
			settings.PerformSpellChecking = chkSpellCheck.Checked;
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
			//Load the settings from the configuration file and update the form fields
			chkUseDatabase.Checked = settings.UseDatabase;
			chkPause.Enabled = settings.UseDatabase;
			chkPause.Checked = settings.PauseBetweenOperations;
			if((settings.PauseDelay >= 0)&&(settings.PauseDelay < cmbPause.Items.Count))
			{
				cmbPause.SelectedIndex = settings.PauseDelay;
			}
			cmbPause.Enabled = settings.PauseBetweenOperations;
			if((settings.DBActionTimeout >= nudDBActionTimeout.Minimum) && (settings.DBActionTimeout <= nudDBActionTimeout.Maximum))
			{
				nudDBActionTimeout.Value = settings.DBActionTimeout;
			}
			chkExtractMeta.Checked = settings.ExtractMetaTags;
			chkExtractTitle.Checked = settings.ExtractTitleTag;
			chkSpellCheck.Checked = settings.PerformSpellChecking;
		}

		#endregion


	}
}
