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
	/// frmServerList allows the management of the list of available CrawlWave Servers.
	/// </summary>
	public class frmServerList : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox grpExisting;
		private System.Windows.Forms.GroupBox grpAddServer;
		private System.Windows.Forms.ListView lstServers;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdAdd;
		private System.Windows.Forms.Button cmdClear;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.Label lblAddress;
		private System.Windows.Forms.TextBox txtAddress;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.TextBox txtVersion;
		private System.Windows.Forms.ColumnHeader colID;
		private System.Windows.Forms.ColumnHeader colAddress;
		private System.Windows.Forms.ColumnHeader colVersion;
		private System.Windows.Forms.ImageList imgIcons;
		private System.Windows.Forms.ContextMenu mnuServer;
		private System.Windows.Forms.MenuItem mnuServerRemove;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem mnuServerCopyAddress;

		private Globals globals;

		/// <summary>
		/// Constructs a new instance of the <see cref="frmServerList"/> form.
		/// </summary>
		public frmServerList()
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmServerList));
			this.grpExisting = new System.Windows.Forms.GroupBox();
			this.lstServers = new System.Windows.Forms.ListView();
			this.colID = new System.Windows.Forms.ColumnHeader();
			this.colAddress = new System.Windows.Forms.ColumnHeader();
			this.colVersion = new System.Windows.Forms.ColumnHeader();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.grpAddServer = new System.Windows.Forms.GroupBox();
			this.txtVersion = new System.Windows.Forms.TextBox();
			this.lblVersion = new System.Windows.Forms.Label();
			this.txtAddress = new System.Windows.Forms.TextBox();
			this.lblAddress = new System.Windows.Forms.Label();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdAdd = new System.Windows.Forms.Button();
			this.cmdClear = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.mnuServer = new System.Windows.Forms.ContextMenu();
			this.mnuServerRemove = new System.Windows.Forms.MenuItem();
			this.mnuServerCopyAddress = new System.Windows.Forms.MenuItem();
			this.grpExisting.SuspendLayout();
			this.grpAddServer.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpExisting
			// 
			this.grpExisting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpExisting.Controls.Add(this.lstServers);
			this.grpExisting.Location = new System.Drawing.Point(8, 8);
			this.grpExisting.Name = "grpExisting";
			this.grpExisting.Size = new System.Drawing.Size(456, 144);
			this.grpExisting.TabIndex = 0;
			this.grpExisting.TabStop = false;
			this.grpExisting.Text = "Existing Servers List";
			// 
			// lstServers
			// 
			this.lstServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.colID,
																						 this.colAddress,
																						 this.colVersion});
			this.lstServers.FullRowSelect = true;
			this.lstServers.GridLines = true;
			this.lstServers.Location = new System.Drawing.Point(8, 16);
			this.lstServers.MultiSelect = false;
			this.lstServers.Name = "lstServers";
			this.lstServers.Size = new System.Drawing.Size(440, 120);
			this.lstServers.SmallImageList = this.imgIcons;
			this.lstServers.TabIndex = 0;
			this.lstServers.View = System.Windows.Forms.View.Details;
			this.lstServers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstServers_MouseUp);
			// 
			// colID
			// 
			this.colID.Text = "ID";
			this.colID.Width = 50;
			// 
			// colAddress
			// 
			this.colAddress.Text = "Address";
			this.colAddress.Width = 320;
			// 
			// colVersion
			// 
			this.colVersion.Text = "Version";
			this.colVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// imgIcons
			// 
			this.imgIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// grpAddServer
			// 
			this.grpAddServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpAddServer.Controls.Add(this.txtVersion);
			this.grpAddServer.Controls.Add(this.lblVersion);
			this.grpAddServer.Controls.Add(this.txtAddress);
			this.grpAddServer.Controls.Add(this.lblAddress);
			this.grpAddServer.Location = new System.Drawing.Point(8, 160);
			this.grpAddServer.Name = "grpAddServer";
			this.grpAddServer.Size = new System.Drawing.Size(456, 48);
			this.grpAddServer.TabIndex = 1;
			this.grpAddServer.TabStop = false;
			this.grpAddServer.Text = "Add a new Server";
			// 
			// txtVersion
			// 
			this.txtVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtVersion.Location = new System.Drawing.Point(376, 16);
			this.txtVersion.Name = "txtVersion";
			this.txtVersion.Size = new System.Drawing.Size(72, 20);
			this.txtVersion.TabIndex = 3;
			this.txtVersion.Text = "";
			// 
			// lblVersion
			// 
			this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblVersion.Location = new System.Drawing.Point(328, 16);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(48, 23);
			this.lblVersion.TabIndex = 2;
			this.lblVersion.Text = "Version";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtAddress
			// 
			this.txtAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAddress.Location = new System.Drawing.Point(56, 16);
			this.txtAddress.Name = "txtAddress";
			this.txtAddress.Size = new System.Drawing.Size(264, 20);
			this.txtAddress.TabIndex = 1;
			this.txtAddress.Text = "";
			// 
			// lblAddress
			// 
			this.lblAddress.Location = new System.Drawing.Point(8, 16);
			this.lblAddress.Name = "lblAddress";
			this.lblAddress.Size = new System.Drawing.Size(48, 23);
			this.lblAddress.TabIndex = 0;
			this.lblAddress.Text = "Address";
			this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmdOK
			// 
			this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdOK.Location = new System.Drawing.Point(8, 216);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(80, 24);
			this.cmdOK.TabIndex = 2;
			this.cmdOK.Text = "Refresh";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdAdd
			// 
			this.cmdAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdAdd.Location = new System.Drawing.Point(96, 216);
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.Size = new System.Drawing.Size(80, 24);
			this.cmdAdd.TabIndex = 3;
			this.cmdAdd.Text = "Add";
			this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			// 
			// cmdClear
			// 
			this.cmdClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClear.Location = new System.Drawing.Point(184, 216);
			this.cmdClear.Name = "cmdClear";
			this.cmdClear.Size = new System.Drawing.Size(80, 24);
			this.cmdClear.TabIndex = 4;
			this.cmdClear.Text = "Clear";
			this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClose.Location = new System.Drawing.Point(272, 216);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(80, 24);
			this.cmdClose.TabIndex = 5;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// mnuServer
			// 
			this.mnuServer.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuServerRemove,
																					  this.mnuServerCopyAddress});
			// 
			// mnuServerRemove
			// 
			this.mnuServerRemove.Index = 0;
			this.mnuServerRemove.Text = "Remove Server";
			this.mnuServerRemove.Click += new System.EventHandler(this.mnuServerRemove_Click);
			// 
			// mnuServerCopyAddress
			// 
			this.mnuServerCopyAddress.Index = 1;
			this.mnuServerCopyAddress.Text = "Copy address";
			this.mnuServerCopyAddress.Click += new System.EventHandler(this.mnuServerCopyAddress_Click);
			// 
			// frmServerList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 245);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdClear);
			this.Controls.Add(this.cmdAdd);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.grpAddServer);
			this.Controls.Add(this.grpExisting);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmServerList";
			this.Text = "Server List Management";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmServerList_Closing);
			this.Load += new System.EventHandler(this.frmServerList_Load);
			this.grpExisting.ResumeLayout(false);
			this.grpAddServer.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmServerList_Load(object sender, System.EventArgs e)
		{
			LoadValues();
		}

		private void frmServerList_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			globals.LoadedForms[this.Name] = null;
		}

		private void lstServers_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button==MouseButtons.Right)
			{
				if((lstServers.Items.Count>0)&&(lstServers.SelectedItems.Count>0))
				{
					mnuServer.Show(lstServers, new Point(e.X, e.Y));
				}
			}	
		}

		private void mnuServerRemove_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("Are you sure you wish to remove this server from the list?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
			{
				if(RemoveServer())
				{
					lstServers.Items.RemoveAt(lstServers.SelectedIndices[0]);
				}
			}	
		}

		private void mnuServerCopyAddress_Click(object sender, System.EventArgs e)
		{
			try
			{
				Clipboard.SetDataObject(lstServers.SelectedItems[0].SubItems[1].Text, true);
			}
			catch
			{}
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			LoadValues();
		}

		private void cmdAdd_Click(object sender, System.EventArgs e)
		{
			if(!ValidateForm())
			{
				return;
			}
			if(!AddServer())
			{
				return;
			}
			ClearForm();
		}

		private void cmdClear_Click(object sender, System.EventArgs e)
		{
			ClearForm();
		}

		private void cmdClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Loads the list of available servers.
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
				catch(Exception ex)
				{
					if(dbcon != null)
					{
						dbcon.Dispose();
						dbcon = null;
					}
					throw ex;
				}
				//Load the values from the database
				SqlCommand cmd = new SqlCommand("cw_select_servers", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				lstServers.Items.Clear();
				int itemCount = 0;
				foreach(DataRow dr in ds.Tables[0].Rows)
				{
					lstServers.Items.Add(((int)dr[0]).ToString(), 0);
					lstServers.Items[itemCount].SubItems.Add(((string)dr[1]).Trim());
					lstServers.Items[itemCount].SubItems.Add(((string)dr[2]).Trim());
					itemCount++;
				}
				ds.Dispose();
			}
			catch(Exception ex)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to retrieve the list of available Servers: " + ex.ToString());
				MessageBox.Show(this.Text + " failed to retrieve the list of available Servers:\n" + ex.Message);
				GC.Collect();
			}
		}

		/// <summary>
		/// Clears the form's input fields.
		/// </summary>
		private void ClearForm()
		{
			txtAddress.Text = String.Empty;
			txtVersion.Text = String.Empty;
		}

		/// <summary>
		/// Performs validation of the user's input before inserting a new server to the list.
		/// </summary>
		/// <returns>True if the input is OK, false otherwise.</returns>
		private bool ValidateForm()
		{
			if(txtAddress.Text == String.Empty)
			{
				MessageBox.Show("You must provide the address of the new Server!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtAddress.Focus();
				return false;
			}
			else
			{
				try
				{
					Uri uri = new Uri(txtAddress.Text);
					if(!uri.AbsolutePath.EndsWith("CrawlWaveServer.asmx"))
					{
						if(MessageBox.Show("This address does not seem to be a valid CrawlWave Server address. Are you sure you wish to continue?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.No)
						{
							txtAddress.Focus();
							return false;
						}
					}
				}
				catch
				{
					MessageBox.Show("The address you provided does not seem to be valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					txtAddress.Focus();
					return false;
				}
			}
			if(txtVersion.Text == String.Empty)
			{
				MessageBox.Show("You must provide the new Server's version!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtVersion.Focus();
				return false;
			}
			else
			{
				try
				{
					Version v = new Version(txtVersion.Text);
				}
				catch
				{
					MessageBox.Show("The version you provided is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					txtVersion.Focus();
					return false;
				}
			}
			for (int i=0; i<lstServers.Items.Count; i++)
			{
				if(txtAddress.Text == lstServers.Items[i].SubItems[1].Text)
				{
					MessageBox.Show("This Server is already in the list!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					txtAddress.Focus();
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Inserts a new server in the database list of servers.
		/// </summary>
		/// <returns>True if the operation succeeds, false otherwise.</returns>
		private bool AddServer()
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
				catch(Exception ex)
				{
					if(dbcon != null)
					{
						dbcon.Dispose();
						dbcon = null;
					}
					throw ex;
				}
				SqlCommand cmd = new SqlCommand("cw_insert_server", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@url", SqlDbType.NVarChar, 255);
				cmd.Parameters.Add("@version", SqlDbType.NChar, 15);
				cmd.Parameters[0].Value = txtAddress.Text;
				cmd.Parameters[1].Value = txtVersion.Text;
				cmd.ExecuteNonQuery();
				dbcon.Close();
				LoadValues();
				retVal = true;
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to add the Server: " + e.ToString());
				MessageBox.Show(this.Text + " failed to add the Server:\n" + e.Message);
				retVal = false;
			}
			finally
			{
				GC.Collect();
			}
			return retVal;
		}

		/// <summary>
		/// Removes a list from the list.
		/// </summary>
		/// <returns>True if the operation succeeds, false otherwise.</returns>
		private bool RemoveServer()
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
				catch(Exception ex)
				{
					if(dbcon != null)
					{
						dbcon.Dispose();
						dbcon = null;
					}
					throw ex;
				}
				SqlCommand cmd = new SqlCommand("cw_delete_server", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@server_id", SqlDbType.Int);
				cmd.Parameters[0].Value = int.Parse(lstServers.SelectedItems[0].SubItems[0].Text);
				cmd.ExecuteNonQuery();
				dbcon.Close();
				retVal = true;
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to remove the Server: " + e.ToString());
				MessageBox.Show(this.Text + " failed to remove the Server:\n" + e.Message);
				retVal = false;
			}
			finally
			{
				GC.Collect();
			}
			return retVal;
		}

		#endregion
	}
}
