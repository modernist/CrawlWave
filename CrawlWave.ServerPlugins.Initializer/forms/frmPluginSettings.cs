using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrawlWave.ServerPlugins.Initializer
{
	/// <summary>
	/// Summary description for frmPluginSettings.
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
		private System.Windows.Forms.Label lblThreads;
		private System.Windows.Forms.TabPage tabInput;
		private System.Windows.Forms.ImageList imlTabIcons;
		private System.Windows.Forms.Label lblProcess;
		private System.Windows.Forms.NumericUpDown nudThreads;
		private System.Windows.Forms.CheckBox chkCleanUrls;
		private System.Windows.Forms.CheckBox chkCheckUrls;
		private System.Windows.Forms.Button cmdBrowseInput;
		private System.Windows.Forms.Button cmdBrowseOutput;
		private System.Windows.Forms.TextBox txtInputFileName;
		private System.Windows.Forms.TextBox txtOutputFileName;
		private System.Windows.Forms.GroupBox grpInputFile;
		private System.Windows.Forms.GroupBox grpOutputFile;
		private System.Windows.Forms.OpenFileDialog cdlBrowseInput;
		private System.Windows.Forms.SaveFileDialog cdlBrowseOutput;
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
			this.chkCheckUrls = new System.Windows.Forms.CheckBox();
			this.chkCleanUrls = new System.Windows.Forms.CheckBox();
			this.picPause = new System.Windows.Forms.PictureBox();
			this.lblPause = new System.Windows.Forms.Label();
			this.cmbPause = new System.Windows.Forms.ComboBox();
			this.chkPause = new System.Windows.Forms.CheckBox();
			this.lblProcess = new System.Windows.Forms.Label();
			this.lblThreads = new System.Windows.Forms.Label();
			this.nudThreads = new System.Windows.Forms.NumericUpDown();
			this.tabInput = new System.Windows.Forms.TabPage();
			this.txtInputFileName = new System.Windows.Forms.TextBox();
			this.cmdBrowseInput = new System.Windows.Forms.Button();
			this.imlTabIcons = new System.Windows.Forms.ImageList(this.components);
			this.cdlBrowseInput = new System.Windows.Forms.OpenFileDialog();
			this.txtOutputFileName = new System.Windows.Forms.TextBox();
			this.cmdBrowseOutput = new System.Windows.Forms.Button();
			this.grpInputFile = new System.Windows.Forms.GroupBox();
			this.grpOutputFile = new System.Windows.Forms.GroupBox();
			this.cdlBrowseOutput = new System.Windows.Forms.SaveFileDialog();
			this.tabSettings.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudThreads)).BeginInit();
			this.tabInput.SuspendLayout();
			this.grpInputFile.SuspendLayout();
			this.grpOutputFile.SuspendLayout();
			this.SuspendLayout();
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
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.tabGeneral);
			this.tabSettings.Controls.Add(this.tabInput);
			this.tabSettings.ImageList = this.imlTabIcons;
			this.tabSettings.Location = new System.Drawing.Point(8, 8);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.SelectedIndex = 0;
			this.tabSettings.Size = new System.Drawing.Size(264, 136);
			this.tabSettings.TabIndex = 0;
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.chkCheckUrls);
			this.tabGeneral.Controls.Add(this.chkCleanUrls);
			this.tabGeneral.Controls.Add(this.picPause);
			this.tabGeneral.Controls.Add(this.lblPause);
			this.tabGeneral.Controls.Add(this.cmbPause);
			this.tabGeneral.Controls.Add(this.chkPause);
			this.tabGeneral.Controls.Add(this.lblProcess);
			this.tabGeneral.Controls.Add(this.lblThreads);
			this.tabGeneral.Controls.Add(this.nudThreads);
			this.tabGeneral.ImageIndex = 0;
			this.tabGeneral.Location = new System.Drawing.Point(4, 23);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Size = new System.Drawing.Size(256, 109);
			this.tabGeneral.TabIndex = 0;
			this.tabGeneral.Text = "General";
			// 
			// chkCheckUrls
			// 
			this.chkCheckUrls.Location = new System.Drawing.Point(144, 32);
			this.chkCheckUrls.Name = "chkCheckUrls";
			this.chkCheckUrls.Size = new System.Drawing.Size(80, 24);
			this.chkCheckUrls.TabIndex = 4;
			this.chkCheckUrls.Text = "Check Urls";
			// 
			// chkCleanUrls
			// 
			this.chkCleanUrls.Location = new System.Drawing.Point(64, 32);
			this.chkCleanUrls.Name = "chkCleanUrls";
			this.chkCleanUrls.Size = new System.Drawing.Size(80, 24);
			this.chkCleanUrls.TabIndex = 3;
			this.chkCleanUrls.Text = "Clean Urls";
			this.chkCleanUrls.CheckedChanged += new System.EventHandler(this.chkCleanUrls_CheckedChanged);
			// 
			// picPause
			// 
			this.picPause.Image = ((System.Drawing.Image)(resources.GetObject("picPause.Image")));
			this.picPause.Location = new System.Drawing.Point(24, 80);
			this.picPause.Name = "picPause";
			this.picPause.Size = new System.Drawing.Size(24, 24);
			this.picPause.TabIndex = 18;
			this.picPause.TabStop = false;
			// 
			// lblPause
			// 
			this.lblPause.Location = new System.Drawing.Point(64, 80);
			this.lblPause.Name = "lblPause";
			this.lblPause.Size = new System.Drawing.Size(72, 23);
			this.lblPause.TabIndex = 6;
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
			this.cmbPause.TabIndex = 7;
			// 
			// chkPause
			// 
			this.chkPause.Location = new System.Drawing.Point(8, 56);
			this.chkPause.Name = "chkPause";
			this.chkPause.Size = new System.Drawing.Size(240, 24);
			this.chkPause.TabIndex = 5;
			this.chkPause.Text = "Pause between consecutive operations";
			this.chkPause.CheckedChanged += new System.EventHandler(this.chkPause_CheckedChanged);
			// 
			// lblProcess
			// 
			this.lblProcess.Location = new System.Drawing.Point(8, 32);
			this.lblProcess.Name = "lblProcess";
			this.lblProcess.Size = new System.Drawing.Size(56, 23);
			this.lblProcess.TabIndex = 2;
			this.lblProcess.Text = "Process";
			this.lblProcess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblThreads
			// 
			this.lblThreads.Location = new System.Drawing.Point(8, 8);
			this.lblThreads.Name = "lblThreads";
			this.lblThreads.Size = new System.Drawing.Size(136, 23);
			this.lblThreads.TabIndex = 0;
			this.lblThreads.Text = "Number of threads to use";
			this.lblThreads.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// nudThreads
			// 
			this.nudThreads.Location = new System.Drawing.Point(144, 8);
			this.nudThreads.Maximum = new System.Decimal(new int[] {
																	   10,
																	   0,
																	   0,
																	   0});
			this.nudThreads.Minimum = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   0});
			this.nudThreads.Name = "nudThreads";
			this.nudThreads.Size = new System.Drawing.Size(104, 20);
			this.nudThreads.TabIndex = 1;
			this.nudThreads.Value = new System.Decimal(new int[] {
																	 5,
																	 0,
																	 0,
																	 0});
			// 
			// tabInput
			// 
			this.tabInput.Controls.Add(this.grpInputFile);
			this.tabInput.Controls.Add(this.grpOutputFile);
			this.tabInput.ImageIndex = 1;
			this.tabInput.Location = new System.Drawing.Point(4, 23);
			this.tabInput.Name = "tabInput";
			this.tabInput.Size = new System.Drawing.Size(256, 109);
			this.tabInput.TabIndex = 1;
			this.tabInput.Text = "Files";
			// 
			// txtInputFileName
			// 
			this.txtInputFileName.Location = new System.Drawing.Point(8, 16);
			this.txtInputFileName.Name = "txtInputFileName";
			this.txtInputFileName.Size = new System.Drawing.Size(144, 20);
			this.txtInputFileName.TabIndex = 1;
			this.txtInputFileName.Text = "";
			// 
			// cmdBrowseInput
			// 
			this.cmdBrowseInput.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdBrowseInput.ImageIndex = 2;
			this.cmdBrowseInput.ImageList = this.imlTabIcons;
			this.cmdBrowseInput.Location = new System.Drawing.Point(160, 16);
			this.cmdBrowseInput.Name = "cmdBrowseInput";
			this.cmdBrowseInput.Size = new System.Drawing.Size(72, 23);
			this.cmdBrowseInput.TabIndex = 2;
			this.cmdBrowseInput.Text = "Select...";
			this.cmdBrowseInput.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdBrowseInput.Click += new System.EventHandler(this.cmdBrowseInput_Click);
			// 
			// imlTabIcons
			// 
			this.imlTabIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imlTabIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imlTabIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlTabIcons.ImageStream")));
			this.imlTabIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// cdlBrowseInput
			// 
			this.cdlBrowseInput.Filter = "Text Files|*.txt";
			// 
			// txtOutputFileName
			// 
			this.txtOutputFileName.Location = new System.Drawing.Point(8, 16);
			this.txtOutputFileName.Name = "txtOutputFileName";
			this.txtOutputFileName.Size = new System.Drawing.Size(144, 20);
			this.txtOutputFileName.TabIndex = 4;
			this.txtOutputFileName.Text = "";
			// 
			// cmdBrowseOutput
			// 
			this.cmdBrowseOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cmdBrowseOutput.ImageIndex = 3;
			this.cmdBrowseOutput.ImageList = this.imlTabIcons;
			this.cmdBrowseOutput.Location = new System.Drawing.Point(160, 16);
			this.cmdBrowseOutput.Name = "cmdBrowseOutput";
			this.cmdBrowseOutput.Size = new System.Drawing.Size(72, 23);
			this.cmdBrowseOutput.TabIndex = 5;
			this.cmdBrowseOutput.Text = "Select...";
			this.cmdBrowseOutput.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.cmdBrowseOutput.Click += new System.EventHandler(this.cmdBrowseOutput_Click);
			// 
			// grpInputFile
			// 
			this.grpInputFile.Controls.Add(this.txtInputFileName);
			this.grpInputFile.Controls.Add(this.cmdBrowseInput);
			this.grpInputFile.Location = new System.Drawing.Point(8, 8);
			this.grpInputFile.Name = "grpInputFile";
			this.grpInputFile.Size = new System.Drawing.Size(240, 48);
			this.grpInputFile.TabIndex = 6;
			this.grpInputFile.TabStop = false;
			this.grpInputFile.Text = "Input File";
			// 
			// grpOutputFile
			// 
			this.grpOutputFile.Controls.Add(this.cmdBrowseOutput);
			this.grpOutputFile.Controls.Add(this.txtOutputFileName);
			this.grpOutputFile.Location = new System.Drawing.Point(8, 56);
			this.grpOutputFile.Name = "grpOutputFile";
			this.grpOutputFile.Size = new System.Drawing.Size(240, 48);
			this.grpOutputFile.TabIndex = 7;
			this.grpOutputFile.TabStop = false;
			this.grpOutputFile.Text = "Output File";
			// 
			// cdlBrowseOutput
			// 
			this.cdlBrowseOutput.Filter = "Text Files|*.txt";
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
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmPluginSettings";
			this.Text = "Initializer Plugin Settings";
			this.Load += new System.EventHandler(this.frmPluginSettings_Load);
			this.tabSettings.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudThreads)).EndInit();
			this.tabInput.ResumeLayout(false);
			this.grpInputFile.ResumeLayout(false);
			this.grpOutputFile.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void chkCleanUrls_CheckedChanged(object sender, System.EventArgs e)
		{
			grpOutputFile.Enabled = chkCleanUrls.Checked;
		}

		private void chkPause_CheckedChanged(object sender, System.EventArgs e)
		{
			cmbPause.Enabled = chkPause.Checked;
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			if(ValidateForm())
			{
				settings.Threads = (int)nudThreads.Value;
				settings.CleanUrls = chkCleanUrls.Checked;
				settings.CheckUrls = chkCheckUrls.Checked;
				settings.PauseBetweenOperations = chkPause.Checked;
				if(chkPause.Checked)
				{
					settings.PauseDelay = cmbPause.SelectedIndex;
				}
				settings.InputFile = txtInputFileName.Text;
				if(chkCleanUrls.Checked)
				{
					settings.OutpufFile = txtOutputFileName.Text;
				}
				settings.SaveSettings();
				this.Close();
			}
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void cmdBrowseInput_Click(object sender, System.EventArgs e)
		{
			cdlBrowseInput.ShowDialog();
			if(cdlBrowseInput.FileName!=String.Empty)
			{
				txtInputFileName.Text = cdlBrowseInput.FileName;
			}
		}

		private void cmdBrowseOutput_Click(object sender, System.EventArgs e)
		{
			cdlBrowseOutput.ShowDialog();
			if(cdlBrowseOutput.FileName != String.Empty)
			{
				txtOutputFileName.Text = cdlBrowseOutput.FileName;
			}
		}

		private void frmPluginSettings_Load(object sender, System.EventArgs e)
		{
			if((settings.Threads >= nudThreads.Minimum)&&(settings.Threads <= nudThreads.Maximum))
			{
				nudThreads.Value = settings.Threads;
			}
			else
			{
				nudThreads.Value = nudThreads.Minimum;
			}
			chkCleanUrls.Checked = settings.CleanUrls;
			chkCheckUrls.Checked = settings.CheckUrls;
			grpOutputFile.Enabled = settings.CleanUrls;
			chkPause.Checked = settings.PauseBetweenOperations;
			cmbPause.Enabled = settings.PauseBetweenOperations;
			if((settings.PauseDelay >= 0)&&(settings.PauseDelay < cmbPause.Items.Count))
			{
				cmbPause.SelectedIndex = settings.PauseDelay;
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Validates the settings provided by the user.
		/// </summary>
		/// <returns>
		/// True if the settings are OK, false if there is an error or something is missing.
		/// </returns>
		private bool ValidateForm()
		{
			if(txtInputFileName.Text == String.Empty)
			{
				MessageBox.Show("You must select an Input File Name!",this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				tabSettings.SelectedTab = tabInput;
				txtInputFileName.Focus();
				return false;
			}
			if((chkCleanUrls.Checked)&&(txtOutputFileName.Text == String.Empty))
			{
				MessageBox.Show("You must select an Output File Name!",this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				tabSettings.SelectedTab = tabInput;
				txtOutputFileName.Focus();
				return false;
			}
			if((chkCleanUrls.Checked)&&(txtOutputFileName.Text == txtInputFileName.Text))
			{
				//perhaps this should be allowed
				MessageBox.Show("The output file cannot be the same as the input file!",this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				tabSettings.SelectedTab = tabInput;
				txtOutputFileName.Focus();
				return false;
			}
			return true;
		}

		#endregion

	}
}
