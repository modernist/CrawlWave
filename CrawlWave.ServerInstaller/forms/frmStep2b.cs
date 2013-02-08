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
	public class frmStep2b : CrawlWave.ServerInstaller.Forms.frmStep
	{
		private System.Windows.Forms.GroupBox grpSQLSettings;
		private System.Windows.Forms.TextBox txtCWPass;
		private System.Windows.Forms.TextBox txtCWUser;
		private System.Windows.Forms.Label lblCWPass;
		private System.Windows.Forms.Label lblCWUser;
		private System.Windows.Forms.Label lblServer;
		private System.Windows.Forms.TextBox txtCWPass2;
		private System.Windows.Forms.Label lblPass2;
		private System.Windows.Forms.ComboBox cmbServer;
		private System.ComponentModel.IContainer components = null;

		private Globals globals;

		/// <summary>
		/// 
		/// </summary>
		public frmStep2b()
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
			this.cmbServer = new System.Windows.Forms.ComboBox();
			this.lblPass2 = new System.Windows.Forms.Label();
			this.txtCWPass2 = new System.Windows.Forms.TextBox();
			this.txtCWPass = new System.Windows.Forms.TextBox();
			this.txtCWUser = new System.Windows.Forms.TextBox();
			this.lblCWPass = new System.Windows.Forms.Label();
			this.lblCWUser = new System.Windows.Forms.Label();
			this.lblServer = new System.Windows.Forms.Label();
			this.grpSQLSettings.SuspendLayout();
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
			this.cmdBack.Click += new System.EventHandler(this.cmdBack_Click);
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
			// grpSQLSettings
			// 
			this.grpSQLSettings.Controls.Add(this.cmbServer);
			this.grpSQLSettings.Controls.Add(this.lblPass2);
			this.grpSQLSettings.Controls.Add(this.txtCWPass2);
			this.grpSQLSettings.Controls.Add(this.txtCWPass);
			this.grpSQLSettings.Controls.Add(this.txtCWUser);
			this.grpSQLSettings.Controls.Add(this.lblCWPass);
			this.grpSQLSettings.Controls.Add(this.lblCWUser);
			this.grpSQLSettings.Controls.Add(this.lblServer);
			this.grpSQLSettings.Location = new System.Drawing.Point(144, 8);
			this.grpSQLSettings.Name = "grpSQLSettings";
			this.grpSQLSettings.Size = new System.Drawing.Size(344, 224);
			this.grpSQLSettings.TabIndex = 4;
			this.grpSQLSettings.TabStop = false;
			this.grpSQLSettings.Text = "SQL Server Settings";
			// 
			// cmbServer
			// 
			this.cmbServer.Location = new System.Drawing.Point(112, 24);
			this.cmbServer.Name = "cmbServer";
			this.cmbServer.Size = new System.Drawing.Size(224, 21);
			this.cmbServer.TabIndex = 16;
			// 
			// lblPass2
			// 
			this.lblPass2.Location = new System.Drawing.Point(8, 96);
			this.lblPass2.Name = "lblPass2";
			this.lblPass2.TabIndex = 15;
			this.lblPass2.Text = "Password (again)";
			this.lblPass2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtCWPass2
			// 
			this.txtCWPass2.Location = new System.Drawing.Point(112, 96);
			this.txtCWPass2.MaxLength = 30;
			this.txtCWPass2.Name = "txtCWPass2";
			this.txtCWPass2.PasswordChar = '*';
			this.txtCWPass2.Size = new System.Drawing.Size(224, 20);
			this.txtCWPass2.TabIndex = 14;
			this.txtCWPass2.Text = "";
			// 
			// txtCWPass
			// 
			this.txtCWPass.Location = new System.Drawing.Point(112, 72);
			this.txtCWPass.MaxLength = 30;
			this.txtCWPass.Name = "txtCWPass";
			this.txtCWPass.PasswordChar = '*';
			this.txtCWPass.Size = new System.Drawing.Size(224, 20);
			this.txtCWPass.TabIndex = 13;
			this.txtCWPass.Text = "";
			// 
			// txtCWUser
			// 
			this.txtCWUser.Location = new System.Drawing.Point(112, 48);
			this.txtCWUser.MaxLength = 30;
			this.txtCWUser.Name = "txtCWUser";
			this.txtCWUser.Size = new System.Drawing.Size(224, 20);
			this.txtCWUser.TabIndex = 11;
			this.txtCWUser.Text = "";
			// 
			// lblCWPass
			// 
			this.lblCWPass.Location = new System.Drawing.Point(8, 72);
			this.lblCWPass.Name = "lblCWPass";
			this.lblCWPass.TabIndex = 12;
			this.lblCWPass.Text = "CrawlWave Pass";
			this.lblCWPass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCWUser
			// 
			this.lblCWUser.Location = new System.Drawing.Point(8, 48);
			this.lblCWUser.Name = "lblCWUser";
			this.lblCWUser.TabIndex = 10;
			this.lblCWUser.Text = "CrawlWave User";
			this.lblCWUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblServer
			// 
			this.lblServer.Location = new System.Drawing.Point(8, 24);
			this.lblServer.Name = "lblServer";
			this.lblServer.Size = new System.Drawing.Size(104, 23);
			this.lblServer.TabIndex = 9;
			this.lblServer.Text = "Server";
			this.lblServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// frmStep2b
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 271);
			this.Controls.Add(this.grpSQLSettings);
			this.Name = "frmStep2b";
			this.Text = "Database Setup (Step 2 of 6)";
			this.Load += new System.EventHandler(this.frmStep2b_Load);
			this.Controls.SetChildIndex(this.grpSQLSettings, 0);
			this.Controls.SetChildIndex(this.cmdCancel, 0);
			this.Controls.SetChildIndex(this.cmdNext, 0);
			this.Controls.SetChildIndex(this.cmdBack, 0);
			this.grpSQLSettings.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmStep2b_Load(object sender, System.EventArgs e)
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
					txtCWUser.Text = globals.ConfigurationSettings.CWUser;
					txtCWPass.Text = globals.ConfigurationSettings.CWPass;
					txtCWPass2.Text = globals.ConfigurationSettings.CWPass;
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
			frmStep3b frm = new frmStep3b();
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

		private bool ValidateForm()
		{
			if(cmbServer.Text==String.Empty)
			{
				MessageBox.Show("You must select an SQL Server!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				cmbServer.Focus();
				return false;
			}
			if(txtCWUser.Text == String.Empty)
			{
				MessageBox.Show("You must provide a CrawlWave username!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtCWUser.Focus();
				return false;
			}
			if(txtCWPass.Text!=txtCWPass2.Text)
			{
				MessageBox.Show("The passwords you provided do not match!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtCWPass.Focus();
				return false;
			}
			if((txtCWPass.Text==String.Empty)&&(txtCWPass2.Text==String.Empty))
			{
				if(MessageBox.Show("You have provided an empty password. Are you sure the user has been configured with an empty password?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.No)
				{
					txtCWPass.Focus();
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
				globals.ConfigurationSettings.CWUser = txtCWUser.Text;
				globals.ConfigurationSettings.CWPass = txtCWPass.Text;
				globals.ConfigurationSettings.DBSize = 1;
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

