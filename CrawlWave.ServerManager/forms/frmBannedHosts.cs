using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerManager.Forms
{
	/// <summary>
	/// frmBannedHosts allows the Management of Banned Hosts.
	/// </summary>
	public class frmBannedHosts : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView lstBanned;
		private System.Windows.Forms.Label lblAddNew;
		private System.Windows.Forms.TextBox txtHostAddress;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.ImageList imgIcons;
		private System.Windows.Forms.ColumnHeader colAddress;
		private System.Windows.Forms.ColumnHeader colID;
		private System.Windows.Forms.Button cmdRefresh;
		private System.Windows.Forms.GroupBox grpBanned;
		private System.Windows.Forms.GroupBox grpAddNew;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenu mnuHosts;
		private System.Windows.Forms.MenuItem mnuHostsDelete;

		private Globals globals;
		private UrlValidator validator;

		/// <summary>
		/// Constructs a new instance of the <see cref="frmBannedHosts"/> form.
		/// </summary>
		public frmBannedHosts()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			globals = Globals.Instance();
			validator = UrlValidator.Instance();
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmBannedHosts));
			this.grpBanned = new System.Windows.Forms.GroupBox();
			this.lstBanned = new System.Windows.Forms.ListView();
			this.colID = new System.Windows.Forms.ColumnHeader();
			this.colAddress = new System.Windows.Forms.ColumnHeader();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.lblAddNew = new System.Windows.Forms.Label();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdRefresh = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.txtHostAddress = new System.Windows.Forms.TextBox();
			this.grpAddNew = new System.Windows.Forms.GroupBox();
			this.mnuHosts = new System.Windows.Forms.ContextMenu();
			this.mnuHostsDelete = new System.Windows.Forms.MenuItem();
			this.grpBanned.SuspendLayout();
			this.grpAddNew.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpBanned
			// 
			this.grpBanned.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpBanned.Controls.Add(this.lstBanned);
			this.grpBanned.Location = new System.Drawing.Point(8, 8);
			this.grpBanned.Name = "grpBanned";
			this.grpBanned.Size = new System.Drawing.Size(416, 168);
			this.grpBanned.TabIndex = 0;
			this.grpBanned.TabStop = false;
			this.grpBanned.Text = "Banned Hosts";
			// 
			// lstBanned
			// 
			this.lstBanned.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstBanned.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.colID,
																						this.colAddress});
			this.lstBanned.FullRowSelect = true;
			this.lstBanned.GridLines = true;
			this.lstBanned.Location = new System.Drawing.Point(8, 16);
			this.lstBanned.MultiSelect = false;
			this.lstBanned.Name = "lstBanned";
			this.lstBanned.Size = new System.Drawing.Size(400, 144);
			this.lstBanned.SmallImageList = this.imgIcons;
			this.lstBanned.TabIndex = 0;
			this.lstBanned.View = System.Windows.Forms.View.Details;
			this.lstBanned.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstBanned_MouseUp);
			// 
			// colID
			// 
			this.colID.Text = "Host ID";
			this.colID.Width = 80;
			// 
			// colAddress
			// 
			this.colAddress.Text = "Host Address";
			this.colAddress.Width = 312;
			// 
			// imgIcons
			// 
			this.imgIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// lblAddNew
			// 
			this.lblAddNew.Location = new System.Drawing.Point(8, 16);
			this.lblAddNew.Name = "lblAddNew";
			this.lblAddNew.Size = new System.Drawing.Size(80, 23);
			this.lblAddNew.TabIndex = 0;
			this.lblAddNew.Text = "Host Address";
			this.lblAddNew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmdOK
			// 
			this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdOK.Location = new System.Drawing.Point(8, 240);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(80, 24);
			this.cmdOK.TabIndex = 2;
			this.cmdOK.Text = "Insert";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdRefresh
			// 
			this.cmdRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdRefresh.Location = new System.Drawing.Point(96, 240);
			this.cmdRefresh.Name = "cmdRefresh";
			this.cmdRefresh.Size = new System.Drawing.Size(80, 24);
			this.cmdRefresh.TabIndex = 3;
			this.cmdRefresh.Text = "Refresh";
			this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdClose.Location = new System.Drawing.Point(184, 240);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(80, 24);
			this.cmdClose.TabIndex = 4;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// txtHostAddress
			// 
			this.txtHostAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtHostAddress.Location = new System.Drawing.Point(88, 16);
			this.txtHostAddress.MaxLength = 100;
			this.txtHostAddress.Name = "txtHostAddress";
			this.txtHostAddress.Size = new System.Drawing.Size(320, 20);
			this.txtHostAddress.TabIndex = 1;
			this.txtHostAddress.Text = "";
			// 
			// grpAddNew
			// 
			this.grpAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpAddNew.Controls.Add(this.lblAddNew);
			this.grpAddNew.Controls.Add(this.txtHostAddress);
			this.grpAddNew.Location = new System.Drawing.Point(8, 184);
			this.grpAddNew.Name = "grpAddNew";
			this.grpAddNew.Size = new System.Drawing.Size(416, 48);
			this.grpAddNew.TabIndex = 1;
			this.grpAddNew.TabStop = false;
			this.grpAddNew.Text = "Add a new banned host";
			// 
			// mnuHosts
			// 
			this.mnuHosts.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuHostsDelete});
			// 
			// mnuHostsDelete
			// 
			this.mnuHostsDelete.Index = 0;
			this.mnuHostsDelete.Text = "Remove from list";
			this.mnuHostsDelete.Click += new System.EventHandler(this.mnuHostsDelete_Click);
			// 
			// frmBannedHosts
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdClose;
			this.ClientSize = new System.Drawing.Size(432, 269);
			this.Controls.Add(this.grpAddNew);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdRefresh);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.grpBanned);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmBannedHosts";
			this.Text = "Banned Hosts Management";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmBannedHosts_Closing);
			this.Load += new System.EventHandler(this.frmBannedHosts_Load);
			this.grpBanned.ResumeLayout(false);
			this.grpAddNew.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmBannedHosts_Load(object sender, System.EventArgs e)
		{
			LoadValues();
		}

		private void frmBannedHosts_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			globals.LoadedForms[this.Name] = null;
		}

		private void lstBanned_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button==MouseButtons.Right)
			{
				if((lstBanned.Items.Count>0)&&(lstBanned.SelectedItems.Count>0))
				{
					mnuHosts.Show(lstBanned, new Point(e.X, e.Y));
				}
			}
		}

		private void mnuHostsDelete_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("Are you sure you wish to remove this host from the list of banned hosts?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
			{
				DeleteHost(lstBanned.SelectedItems[0].SubItems[0].Text);
			}
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			if(!ValidateForm())
			{
				return;
			}
			if(!InsertHost())
			{
				return;
			}
			txtHostAddress.Text = String.Empty;
		}

		private void cmdRefresh_Click(object sender, System.EventArgs e)
		{
			LoadValues();
		}

		private void cmdClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Loads and displays the list of Banned Hosts from the database.
		/// </summary>
		private void LoadValues()
		{
			try
			{
				//Connect to the database
				SqlConnection dbcon = null;
				try
				{
					dbcon = new SqlConnection(globals.ProvideConnectionString());
					dbcon.Open();
				}
				catch(Exception e)
				{
					if(dbcon != null)
					{
						dbcon.Dispose();
						dbcon = null;
					}
					throw e;
				}
				//Load the values from the database
				SqlCommand cmd = new SqlCommand("cw_select_banned_host_names", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				//Add the entries in the list
				lstBanned.Items.Clear();
				int itemCount = 0;
				Guid g = Guid.Empty;
				foreach(DataRow dr in ds.Tables[0].Rows)
				{
                    g = (Guid)(dr[0]);
					lstBanned.Items.Add(g.ToString(), 0);
					lstBanned.Items[itemCount].SubItems.Add((string)dr[1]);
					itemCount++;
				}
				ds.Dispose();
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to load the list of Banned Hosts : " + e.ToString());
				MessageBox.Show(this.Text + " failed to load the list of Banned Hosts:\n" + e.Message);
				GC.Collect();
			}
		}

		/// <summary>
		/// Performs validation of the user's input
		/// </summary>
		/// <returns>True if the input values are OK, otherwise false.</returns>
		private bool ValidateForm()
		{
			if(txtHostAddress.Text == String.Empty)
			{
				MessageBox.Show("You must supply the address of the host you wish to add to the banned list!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtHostAddress.Focus();
				return false;
			}
			try
			{
				if(validator.Validate(txtHostAddress.Text))
				{
					//it is a fully qualified Url, let's get its hostname
					Uri uri = new Uri(txtHostAddress.Text);
					txtHostAddress.Text = uri.Host;
				}
				else
				{
                    //it's not a fully qualified Url. let's check if it's a valid hostname.
					if(Uri.CheckHostName(txtHostAddress.Text)== UriHostNameType.Unknown)
					{
						MessageBox.Show("The host name you supplied is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						txtHostAddress.Focus();
						return false;
					}
				}
			}
			catch
			{
				MessageBox.Show("The host name you supplied is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtHostAddress.Focus();
				return false;
			}
			return true;
		}

		/// <summary>
		/// Inserts a new entry in the list of banned hosts.
		/// </summary>
		/// <returns>True if the operation succeeds, false if a failure occurs.</returns>
		/// <remarks>
		/// If the provided host already exists in the database then a record is added in
		/// the list of banned hosts, otherwise a host record is first inserted and then
		/// a record is added in the list of banned hosts.
		/// </remarks>
		private bool InsertHost()
		{
			bool retVal = false;
			try
			{
				//Connect to the database
				SqlConnection dbcon = null;
				try
				{
					dbcon = new SqlConnection(globals.ProvideConnectionString());
					dbcon.Open();
				}
				catch(Exception e)
				{
					if(dbcon != null)
					{
						dbcon.Dispose();
						dbcon = null;
					}
					throw e;
				}
				//perform the insertion
				SqlCommand cmd = new SqlCommand("cw_insert_banned_host", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@host_name", SqlDbType.NVarChar, 100);
				Guid g = new Guid(MD5Hash.md5(txtHostAddress.Text));
				cmd.Parameters[0].Value = g;
				cmd.Parameters[1].Value = txtHostAddress.Text;
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				dbcon.Close();
				lstBanned.Items.Add(g.ToString(), 0);
				lstBanned.Items[lstBanned.Items.Count-1].SubItems.Add(txtHostAddress.Text);
				retVal = true;
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to insert an entry in the Banned Hosts table: " + e.ToString());
				MessageBox.Show(this.Text + " failed to insert the provided host in the list of Banned Hosts:\n" + e.Message);
				GC.Collect();
			}
			return retVal;
		}

		/// <summary>
		/// Removes a host from the 'black list' of banned hosts by deleting a row from the
		/// Banned Hosts table.
		/// </summary>
		/// <param name="hostID">The ID of the host to be removed from the 'black list'.</param>
		private void DeleteHost(string hostID)
		{
			try
			{
				//Connect to the database
				SqlConnection dbcon = null;
				try
				{
					dbcon = new SqlConnection(globals.ProvideConnectionString());
					dbcon.Open();
				}
				catch(Exception e)
				{
					if(dbcon != null)
					{
						dbcon.Dispose();
						dbcon = null;
					}
					throw e;
				}
				SqlCommand cmd = new SqlCommand("cw_delete_banned_host", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
				cmd.Parameters[0].Value = new Guid(hostID);
				cmd.ExecuteNonQuery();
				cmd.Dispose();
				dbcon.Close();
				lstBanned.SelectedItems[0].ImageIndex = 1;
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to remove an entry from the Banned Hosts table: " + e.ToString());
				MessageBox.Show(this.Text + " failed to remove the provided host from the list of Banned Hosts:\n" + e.Message);
				GC.Collect();
			}
		}

		#endregion
	}
}
