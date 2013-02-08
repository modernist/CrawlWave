using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CSKSetup;

namespace CrawlWave.ServerInstaller.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class frmStep2 : CrawlWave.ServerInstaller.Forms.frmStep
	{
		private System.Windows.Forms.Label lblDBSize;
		private System.Windows.Forms.TextBox txtCWPass2;
		private System.Windows.Forms.TextBox txtCWPass;
		private System.Windows.Forms.TextBox txtCWUser;
		private System.Windows.Forms.Label lblCWPass;
		private System.Windows.Forms.Label lblCWUser;
		private System.Windows.Forms.TextBox txtDBAPass;
		private System.Windows.Forms.TextBox txtDBAUser;
		private System.Windows.Forms.Label lblServer;
		private System.Windows.Forms.ComboBox cmbServer;
		private System.Windows.Forms.GroupBox grpSQLSettings;
		private System.Windows.Forms.FolderBrowserDialog dlgBrowseDB;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.NumericUpDown nudDBSizeMax;
		private System.Windows.Forms.NumericUpDown nudDBSize;
		private System.Windows.Forms.CheckBox chkDBSizeMax;
		private System.Windows.Forms.Label lblDBAUserPass;
		private System.Windows.Forms.Label lblDBIndexesPath;
		private System.Windows.Forms.TextBox txtDBIndexesPath;
		private System.Windows.Forms.Label lblDBDataPath;
		private System.Windows.Forms.Button cmdSelectDBLogPath;
		private System.Windows.Forms.TextBox txtDBLogPath;
		private System.Windows.Forms.Label lblDBLogPath;
		private System.Windows.Forms.Button cmdSelectDBIndexesPath;
		private System.Windows.Forms.TextBox txtDBDataPath;
		private System.Windows.Forms.Button cmdSelectDBDataPath;

		private Globals globals;

		/// <summary>
		/// 
		/// </summary>
		public frmStep2()
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
			this.grpSQLSettings = new System.Windows.Forms.GroupBox();
			this.cmdSelectDBDataPath = new System.Windows.Forms.Button();
			this.txtDBDataPath = new System.Windows.Forms.TextBox();
			this.lblDBDataPath = new System.Windows.Forms.Label();
			this.cmdSelectDBIndexesPath = new System.Windows.Forms.Button();
			this.txtDBIndexesPath = new System.Windows.Forms.TextBox();
			this.lblDBIndexesPath = new System.Windows.Forms.Label();
			this.chkDBSizeMax = new System.Windows.Forms.CheckBox();
			this.nudDBSizeMax = new System.Windows.Forms.NumericUpDown();
			this.cmdSelectDBLogPath = new System.Windows.Forms.Button();
			this.txtDBLogPath = new System.Windows.Forms.TextBox();
			this.lblDBLogPath = new System.Windows.Forms.Label();
			this.nudDBSize = new System.Windows.Forms.NumericUpDown();
			this.lblDBSize = new System.Windows.Forms.Label();
			this.txtCWPass2 = new System.Windows.Forms.TextBox();
			this.txtCWPass = new System.Windows.Forms.TextBox();
			this.txtCWUser = new System.Windows.Forms.TextBox();
			this.lblCWPass = new System.Windows.Forms.Label();
			this.lblCWUser = new System.Windows.Forms.Label();
			this.lblDBAUserPass = new System.Windows.Forms.Label();
			this.txtDBAPass = new System.Windows.Forms.TextBox();
			this.txtDBAUser = new System.Windows.Forms.TextBox();
			this.lblServer = new System.Windows.Forms.Label();
			this.cmbServer = new System.Windows.Forms.ComboBox();
			this.dlgBrowseDB = new System.Windows.Forms.FolderBrowserDialog();
			this.grpSQLSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDBSizeMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDBSize)).BeginInit();
			this.SuspendLayout();
			// 
			// lblStep1
			// 
			this.lblStep1.ImageIndex = 2;
			this.lblStep1.Name = "lblStep1";
			// 
			// lblStep2
			// 
			this.lblStep2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblStep2.ImageIndex = 4;
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
			// grpSQLSettings
			// 
			this.grpSQLSettings.Controls.Add(this.cmdSelectDBDataPath);
			this.grpSQLSettings.Controls.Add(this.txtDBDataPath);
			this.grpSQLSettings.Controls.Add(this.lblDBDataPath);
			this.grpSQLSettings.Controls.Add(this.cmdSelectDBIndexesPath);
			this.grpSQLSettings.Controls.Add(this.txtDBIndexesPath);
			this.grpSQLSettings.Controls.Add(this.lblDBIndexesPath);
			this.grpSQLSettings.Controls.Add(this.chkDBSizeMax);
			this.grpSQLSettings.Controls.Add(this.nudDBSizeMax);
			this.grpSQLSettings.Controls.Add(this.cmdSelectDBLogPath);
			this.grpSQLSettings.Controls.Add(this.txtDBLogPath);
			this.grpSQLSettings.Controls.Add(this.lblDBLogPath);
			this.grpSQLSettings.Controls.Add(this.nudDBSize);
			this.grpSQLSettings.Controls.Add(this.lblDBSize);
			this.grpSQLSettings.Controls.Add(this.txtCWPass2);
			this.grpSQLSettings.Controls.Add(this.txtCWPass);
			this.grpSQLSettings.Controls.Add(this.txtCWUser);
			this.grpSQLSettings.Controls.Add(this.lblCWPass);
			this.grpSQLSettings.Controls.Add(this.lblCWUser);
			this.grpSQLSettings.Controls.Add(this.lblDBAUserPass);
			this.grpSQLSettings.Controls.Add(this.txtDBAPass);
			this.grpSQLSettings.Controls.Add(this.txtDBAUser);
			this.grpSQLSettings.Controls.Add(this.lblServer);
			this.grpSQLSettings.Controls.Add(this.cmbServer);
			this.grpSQLSettings.Location = new System.Drawing.Point(144, 8);
			this.grpSQLSettings.Name = "grpSQLSettings";
			this.grpSQLSettings.Size = new System.Drawing.Size(344, 224);
			this.grpSQLSettings.TabIndex = 1;
			this.grpSQLSettings.TabStop = false;
			this.grpSQLSettings.Text = "SQL Server Settings";
			// 
			// cmdSelectDBDataPath
			// 
			this.cmdSelectDBDataPath.Location = new System.Drawing.Point(264, 168);
			this.cmdSelectDBDataPath.Name = "cmdSelectDBDataPath";
			this.cmdSelectDBDataPath.Size = new System.Drawing.Size(72, 24);
			this.cmdSelectDBDataPath.TabIndex = 19;
			this.cmdSelectDBDataPath.Text = "Browse...";
			this.cmdSelectDBDataPath.Click += new System.EventHandler(this.cmdSelectDBDataPath_Click);
			// 
			// txtDBDataPath
			// 
			this.txtDBDataPath.Location = new System.Drawing.Point(112, 168);
			this.txtDBDataPath.Name = "txtDBDataPath";
			this.txtDBDataPath.ReadOnly = true;
			this.txtDBDataPath.Size = new System.Drawing.Size(144, 20);
			this.txtDBDataPath.TabIndex = 18;
			this.txtDBDataPath.Text = "";
			// 
			// lblDBDataPath
			// 
			this.lblDBDataPath.Location = new System.Drawing.Point(8, 168);
			this.lblDBDataPath.Name = "lblDBDataPath";
			this.lblDBDataPath.TabIndex = 17;
			this.lblDBDataPath.Text = "DB Data File Path";
			this.lblDBDataPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmdSelectDBIndexesPath
			// 
			this.cmdSelectDBIndexesPath.Location = new System.Drawing.Point(264, 144);
			this.cmdSelectDBIndexesPath.Name = "cmdSelectDBIndexesPath";
			this.cmdSelectDBIndexesPath.Size = new System.Drawing.Size(72, 24);
			this.cmdSelectDBIndexesPath.TabIndex = 16;
			this.cmdSelectDBIndexesPath.Text = "Browse...";
			this.cmdSelectDBIndexesPath.Click += new System.EventHandler(this.cmdSelectDBIndexesPath_Click);
			// 
			// txtDBIndexesPath
			// 
			this.txtDBIndexesPath.Location = new System.Drawing.Point(112, 144);
			this.txtDBIndexesPath.Name = "txtDBIndexesPath";
			this.txtDBIndexesPath.ReadOnly = true;
			this.txtDBIndexesPath.Size = new System.Drawing.Size(144, 20);
			this.txtDBIndexesPath.TabIndex = 15;
			this.txtDBIndexesPath.Text = "";
			// 
			// lblDBIndexesPath
			// 
			this.lblDBIndexesPath.Location = new System.Drawing.Point(8, 144);
			this.lblDBIndexesPath.Name = "lblDBIndexesPath";
			this.lblDBIndexesPath.TabIndex = 14;
			this.lblDBIndexesPath.Text = "DB Indexes Path";
			this.lblDBIndexesPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chkDBSizeMax
			// 
			this.chkDBSizeMax.Location = new System.Drawing.Point(216, 120);
			this.chkDBSizeMax.Name = "chkDBSizeMax";
			this.chkDBSizeMax.Size = new System.Drawing.Size(48, 24);
			this.chkDBSizeMax.TabIndex = 12;
			this.chkDBSizeMax.Text = "Max";
			this.chkDBSizeMax.CheckedChanged += new System.EventHandler(this.chkDBSizeMax_CheckedChanged);
			// 
			// nudDBSizeMax
			// 
			this.nudDBSizeMax.Enabled = false;
			this.nudDBSizeMax.Increment = new System.Decimal(new int[] {
																		   100,
																		   0,
																		   0,
																		   0});
			this.nudDBSizeMax.Location = new System.Drawing.Point(264, 120);
			this.nudDBSizeMax.Maximum = new System.Decimal(new int[] {
																		 100000,
																		 0,
																		 0,
																		 0});
			this.nudDBSizeMax.Minimum = new System.Decimal(new int[] {
																		 1000,
																		 0,
																		 0,
																		 0});
			this.nudDBSizeMax.Name = "nudDBSizeMax";
			this.nudDBSizeMax.Size = new System.Drawing.Size(72, 20);
			this.nudDBSizeMax.TabIndex = 13;
			this.nudDBSizeMax.Value = new System.Decimal(new int[] {
																	   1000,
																	   0,
																	   0,
																	   0});
			// 
			// cmdSelectDBLogPath
			// 
			this.cmdSelectDBLogPath.Location = new System.Drawing.Point(264, 192);
			this.cmdSelectDBLogPath.Name = "cmdSelectDBLogPath";
			this.cmdSelectDBLogPath.Size = new System.Drawing.Size(72, 24);
			this.cmdSelectDBLogPath.TabIndex = 22;
			this.cmdSelectDBLogPath.Text = "Browse...";
			this.cmdSelectDBLogPath.Click += new System.EventHandler(this.cmdSelectDBLogPath_Click);
			// 
			// txtDBLogPath
			// 
			this.txtDBLogPath.Location = new System.Drawing.Point(112, 192);
			this.txtDBLogPath.MaxLength = 1000;
			this.txtDBLogPath.Name = "txtDBLogPath";
			this.txtDBLogPath.ReadOnly = true;
			this.txtDBLogPath.Size = new System.Drawing.Size(144, 20);
			this.txtDBLogPath.TabIndex = 21;
			this.txtDBLogPath.Text = "";
			// 
			// lblDBLogPath
			// 
			this.lblDBLogPath.Location = new System.Drawing.Point(8, 192);
			this.lblDBLogPath.Name = "lblDBLogPath";
			this.lblDBLogPath.TabIndex = 20;
			this.lblDBLogPath.Text = "DB Log File Path";
			this.lblDBLogPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// nudDBSize
			// 
			this.nudDBSize.Increment = new System.Decimal(new int[] {
																		100,
																		0,
																		0,
																		0});
			this.nudDBSize.Location = new System.Drawing.Point(144, 120);
			this.nudDBSize.Maximum = new System.Decimal(new int[] {
																	  100000,
																	  0,
																	  0,
																	  0});
			this.nudDBSize.Minimum = new System.Decimal(new int[] {
																	  100,
																	  0,
																	  0,
																	  0});
			this.nudDBSize.Name = "nudDBSize";
			this.nudDBSize.Size = new System.Drawing.Size(64, 20);
			this.nudDBSize.TabIndex = 11;
			this.nudDBSize.Value = new System.Decimal(new int[] {
																	1000,
																	0,
																	0,
																	0});
			// 
			// lblDBSize
			// 
			this.lblDBSize.Location = new System.Drawing.Point(8, 120);
			this.lblDBSize.Name = "lblDBSize";
			this.lblDBSize.Size = new System.Drawing.Size(144, 23);
			this.lblDBSize.TabIndex = 10;
			this.lblDBSize.Text = "Database Size in MB, initial";
			this.lblDBSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtCWPass2
			// 
			this.txtCWPass2.Location = new System.Drawing.Point(224, 96);
			this.txtCWPass2.MaxLength = 30;
			this.txtCWPass2.Name = "txtCWPass2";
			this.txtCWPass2.PasswordChar = '*';
			this.txtCWPass2.Size = new System.Drawing.Size(112, 20);
			this.txtCWPass2.TabIndex = 9;
			this.txtCWPass2.Text = "";
			// 
			// txtCWPass
			// 
			this.txtCWPass.Location = new System.Drawing.Point(112, 96);
			this.txtCWPass.MaxLength = 30;
			this.txtCWPass.Name = "txtCWPass";
			this.txtCWPass.PasswordChar = '*';
			this.txtCWPass.Size = new System.Drawing.Size(112, 20);
			this.txtCWPass.TabIndex = 8;
			this.txtCWPass.Text = "";
			// 
			// txtCWUser
			// 
			this.txtCWUser.Location = new System.Drawing.Point(112, 72);
			this.txtCWUser.MaxLength = 30;
			this.txtCWUser.Name = "txtCWUser";
			this.txtCWUser.Size = new System.Drawing.Size(224, 20);
			this.txtCWUser.TabIndex = 6;
			this.txtCWUser.Text = "";
			// 
			// lblCWPass
			// 
			this.lblCWPass.Location = new System.Drawing.Point(8, 96);
			this.lblCWPass.Name = "lblCWPass";
			this.lblCWPass.TabIndex = 7;
			this.lblCWPass.Text = "CrawlWave Pass";
			this.lblCWPass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCWUser
			// 
			this.lblCWUser.Location = new System.Drawing.Point(8, 72);
			this.lblCWUser.Name = "lblCWUser";
			this.lblCWUser.TabIndex = 5;
			this.lblCWUser.Text = "CrawlWave User";
			this.lblCWUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDBAUserPass
			// 
			this.lblDBAUserPass.Location = new System.Drawing.Point(8, 48);
			this.lblDBAUserPass.Name = "lblDBAUserPass";
			this.lblDBAUserPass.TabIndex = 2;
			this.lblDBAUserPass.Text = "DBA User / Pass";
			this.lblDBAUserPass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtDBAPass
			// 
			this.txtDBAPass.Location = new System.Drawing.Point(224, 48);
			this.txtDBAPass.MaxLength = 30;
			this.txtDBAPass.Name = "txtDBAPass";
			this.txtDBAPass.PasswordChar = '*';
			this.txtDBAPass.Size = new System.Drawing.Size(112, 20);
			this.txtDBAPass.TabIndex = 4;
			this.txtDBAPass.Text = "";
			// 
			// txtDBAUser
			// 
			this.txtDBAUser.Location = new System.Drawing.Point(112, 48);
			this.txtDBAUser.MaxLength = 30;
			this.txtDBAUser.Name = "txtDBAUser";
			this.txtDBAUser.Size = new System.Drawing.Size(112, 20);
			this.txtDBAUser.TabIndex = 3;
			this.txtDBAUser.Text = "";
			// 
			// lblServer
			// 
			this.lblServer.Location = new System.Drawing.Point(8, 24);
			this.lblServer.Name = "lblServer";
			this.lblServer.Size = new System.Drawing.Size(104, 23);
			this.lblServer.TabIndex = 0;
			this.lblServer.Text = "Server";
			this.lblServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmbServer
			// 
			this.cmbServer.Location = new System.Drawing.Point(112, 24);
			this.cmbServer.Name = "cmbServer";
			this.cmbServer.Size = new System.Drawing.Size(224, 21);
			this.cmbServer.TabIndex = 1;
			// 
			// dlgBrowseDB
			// 
			this.dlgBrowseDB.Description = "Select a location for the new database";
			// 
			// frmStep2
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 271);
			this.Controls.Add(this.grpSQLSettings);
			this.Name = "frmStep2";
			this.Text = "Database Setup (Step 2 of 6)";
			this.Load += new System.EventHandler(this.frmStep2_Load);
			this.Controls.SetChildIndex(this.cmdCancel, 0);
			this.Controls.SetChildIndex(this.cmdNext, 0);
			this.Controls.SetChildIndex(this.cmdBack, 0);
			this.Controls.SetChildIndex(this.grpSQLSettings, 0);
			this.grpSQLSettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudDBSizeMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDBSize)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmStep2_Load(object sender, System.EventArgs e)
		{
			globals.LoadedForms.Add(this.Name, this);
			try
			{
				ArrayList servers = DataBaseServers.GetDBServers();
				foreach(string server in servers)
				{
					cmbServer.Items.Add(server);
				}
				//set the fields in case we came back from step3
				if(globals.ConfigurationSettings.SQLServer!=String.Empty)
				{
					if(globals.ConfigurationSettings.SQLServer == ".")
					{
						cmbServer.Text = "(local)";
					}
					else
					{
						cmbServer.Text = globals.ConfigurationSettings.SQLServer;
					}
					txtDBAUser.Text = globals.ConfigurationSettings.DBAUser;
					txtDBAPass.Text = globals.ConfigurationSettings.DBAPass;
					txtCWUser.Text = globals.ConfigurationSettings.CWUser;
					txtCWPass.Text = globals.ConfigurationSettings.CWPass;
					txtCWPass2.Text = globals.ConfigurationSettings.CWPass;
					nudDBSize.Value = globals.ConfigurationSettings.DBSize;
					if(globals.ConfigurationSettings.DBSizeMax != 0)
					{
						chkDBSizeMax.Checked = true;
						nudDBSizeMax.Value = globals.ConfigurationSettings.DBSizeMax;
						nudDBSizeMax.Enabled = true;
					}
					txtDBIndexesPath.Text = globals.ConfigurationSettings.DBIndexesPath;
					txtDBDataPath.Text = globals.ConfigurationSettings.DBDataPath;
					txtDBLogPath.Text = globals.ConfigurationSettings.DBLogPath;
				}
			}
			catch
			{}
			finally
			{
				GC.Collect();
			}
		}

		private void cmdBack_Click(object sender, System.EventArgs e)
		{
			if(globals.IsFormLoaded("frmStep1"))
			{
				frmStep1 frm = (frmStep1)globals.LoadedForms["frmStep1"];
				frm.ShowInTaskbar = false;
				frm.Visible = true;
				globals.LoadedForms[this.Name] = null;
				globals.LoadedForms.Remove(this.Name);
				this.Close();
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
			frmStep3 frm = new frmStep3();
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

		private void chkDBSizeMax_CheckedChanged(object sender, System.EventArgs e)
		{
			nudDBSizeMax.Enabled = chkDBSizeMax.Checked;
		}

		private void cmdSelectDBIndexesPath_Click(object sender, System.EventArgs e)
		{
			try
			{
				dlgBrowseDB.ShowDialog();
				if(dlgBrowseDB.SelectedPath != String.Empty)
				{
					txtDBIndexesPath.Text = dlgBrowseDB.SelectedPath;
					if(!dlgBrowseDB.SelectedPath.EndsWith("\\"))
					{
						txtDBIndexesPath.AppendText("\\");
					}
					dlgBrowseDB.SelectedPath = String.Empty;
				}
			}
			catch
			{}
		}

		private void cmdSelectDBDataPath_Click(object sender, System.EventArgs e)
		{
			try
			{
				dlgBrowseDB.ShowDialog();
				if(dlgBrowseDB.SelectedPath != String.Empty)
				{
					txtDBDataPath.Text = dlgBrowseDB.SelectedPath;
					if(!dlgBrowseDB.SelectedPath.EndsWith("\\"))
					{
						txtDBDataPath.AppendText("\\");
					}
					dlgBrowseDB.SelectedPath = String.Empty;
				}
			}
			catch
			{}		
		}

		private void cmdSelectDBLogPath_Click(object sender, System.EventArgs e)
		{
			try
			{
				dlgBrowseDB.ShowDialog();
				if(dlgBrowseDB.SelectedPath != String.Empty)
				{
					txtDBLogPath.Text = dlgBrowseDB.SelectedPath;
					if(!dlgBrowseDB.SelectedPath.EndsWith("\\"))
					{
						txtDBLogPath.AppendText("\\");
					}
					dlgBrowseDB.SelectedPath = String.Empty;
				}
			}
			catch
			{}
		}

		#endregion

		#region Private Methods

		private bool ValidateForm()
		{
			if(cmbServer.Text==String.Empty)
			{
				MessageBox.Show("You must select an SQL Server!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				cmbServer.Focus();
				return false;
			}
			if(txtDBAUser.Text == String.Empty)
			{
				MessageBox.Show("You must provide the username of an SQL Server administrator!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtDBAUser.Focus();
				return false;
			}
			if(txtCWUser.Text == String.Empty)
			{
				if(MessageBox.Show("You have not provided a CrawlWave username. Do you wish to use the SQL Server administrator's username?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.No)
				{
					txtCWUser.Focus();
					return false;
				}
			}
			if((txtCWPass.Text!=String.Empty)||(txtCWPass2.Text!=String.Empty))
			{
				if(txtCWPass.Text!=txtCWPass2.Text)
				{
					MessageBox.Show("The passwords you provided do not match!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					txtCWPass.Focus();
					return false;
				}
			}
			if(chkDBSizeMax.Checked)
			{
				if(nudDBSize.Value > nudDBSizeMax.Value)
				{
					MessageBox.Show("The maximum DB Size cannot be smaller than the initial size!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					nudDBSizeMax.Focus();
					return false;
				}
			}
			if(txtDBIndexesPath.Text == String.Empty)
			{
				MessageBox.Show("You must select the path for the Database Indexes!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtDBIndexesPath.Focus();
				return false;
			}
			if(txtDBDataPath.Text == String.Empty)
			{
				MessageBox.Show("You must select the path for the Database Data Files!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtDBDataPath.Focus();
				return false;
			}
			if(txtDBLogPath.Text == String.Empty)
			{
				MessageBox.Show("You must select the path for the Database Log Files!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtDBLogPath.Focus();
				return false;
			}
			if((txtDBIndexesPath.Text == txtDBDataPath.Text)||(txtDBDataPath.Text == txtDBLogPath.Text)||(txtDBIndexesPath.Text == txtDBLogPath.Text))
			{
				if(MessageBox.Show("It is not advisable to put Database Indexes on the same path as Data Files or Log Files. Do you wish to continue with this configuration?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.No)
				{
					return false;
				}
			}
			return true;
		}

		private bool SetOptions()
		{
			try
			{
				if(cmbServer.Text == "(local)")
				{
					globals.ConfigurationSettings.SQLServer = ".";
				}
				else
				{
					globals.ConfigurationSettings.SQLServer = cmbServer.Text;
				}
				globals.ConfigurationSettings.DBAUser = txtDBAUser.Text;
				globals.ConfigurationSettings.DBAPass = txtDBAPass.Text;
				globals.ConfigurationSettings.CWUser = txtCWUser.Text;
				globals.ConfigurationSettings.CWPass = txtCWPass.Text;
				globals.ConfigurationSettings.DBSize = (int)nudDBSize.Value;
				if(chkDBSizeMax.Checked)
				{
					globals.ConfigurationSettings.DBSizeMax = (int)nudDBSizeMax.Value;
				}
				else
				{
					globals.ConfigurationSettings.DBSizeMax = 0;
				}
				globals.ConfigurationSettings.DBIndexesPath = txtDBIndexesPath.Text;
				globals.ConfigurationSettings.DBDataPath = txtDBDataPath.Text;
				globals.ConfigurationSettings.DBLogPath = txtDBLogPath.Text;
			}
			catch
			{
				return false;
			}
			return true;
		}

		#endregion
	}
}

