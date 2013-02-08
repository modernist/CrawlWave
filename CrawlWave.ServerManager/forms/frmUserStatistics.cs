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
	/// frmUserStatistics displays statistics about users participating in the crawling process.
	/// </summary>
	public class frmUserStatistics : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox grpUsers;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.ListView lstUsers;
		private System.Windows.Forms.ColumnHeader colUserID;
		private System.Windows.Forms.ColumnHeader colUserName;
		private System.Windows.Forms.ColumnHeader colUserEmail;
		private System.Windows.Forms.ColumnHeader colNumClients;
		private System.Windows.Forms.ColumnHeader colTotalAssigned;
		private System.Windows.Forms.ColumnHeader colTotalReturned;
		private System.Windows.Forms.ColumnHeader colRegistrationDate;
		private System.Windows.Forms.ColumnHeader colLastActive;
		private System.Windows.Forms.ContextMenu mnuUser;
		private System.Windows.Forms.MenuItem mnuUserDetails;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Globals globals;

		/// <summary>
		/// Constructs a new instance of the <see cref="frmUserStatistics"/> form.
		/// </summary>
		public frmUserStatistics()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmUserStatistics));
			this.grpUsers = new System.Windows.Forms.GroupBox();
			this.lstUsers = new System.Windows.Forms.ListView();
			this.colUserID = new System.Windows.Forms.ColumnHeader();
			this.colUserName = new System.Windows.Forms.ColumnHeader();
			this.colUserEmail = new System.Windows.Forms.ColumnHeader();
			this.colNumClients = new System.Windows.Forms.ColumnHeader();
			this.colTotalAssigned = new System.Windows.Forms.ColumnHeader();
			this.colTotalReturned = new System.Windows.Forms.ColumnHeader();
			this.colRegistrationDate = new System.Windows.Forms.ColumnHeader();
			this.colLastActive = new System.Windows.Forms.ColumnHeader();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.mnuUser = new System.Windows.Forms.ContextMenu();
			this.mnuUserDetails = new System.Windows.Forms.MenuItem();
			this.grpUsers.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpUsers
			// 
			this.grpUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpUsers.Controls.Add(this.lstUsers);
			this.grpUsers.Location = new System.Drawing.Point(8, 8);
			this.grpUsers.Name = "grpUsers";
			this.grpUsers.Size = new System.Drawing.Size(520, 200);
			this.grpUsers.TabIndex = 0;
			this.grpUsers.TabStop = false;
			this.grpUsers.Text = "Registered Users";
			// 
			// lstUsers
			// 
			this.lstUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.colUserID,
																					   this.colUserName,
																					   this.colUserEmail,
																					   this.colNumClients,
																					   this.colTotalAssigned,
																					   this.colTotalReturned,
																					   this.colRegistrationDate,
																					   this.colLastActive});
			this.lstUsers.FullRowSelect = true;
			this.lstUsers.GridLines = true;
			this.lstUsers.Location = new System.Drawing.Point(8, 16);
			this.lstUsers.MultiSelect = false;
			this.lstUsers.Name = "lstUsers";
			this.lstUsers.Size = new System.Drawing.Size(504, 176);
			this.lstUsers.TabIndex = 0;
			this.lstUsers.View = System.Windows.Forms.View.Details;
			this.lstUsers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstUsers_MouseUp);
			// 
			// colUserID
			// 
			this.colUserID.Text = "ID";
			this.colUserID.Width = 35;
			// 
			// colUserName
			// 
			this.colUserName.Text = "Username";
			// 
			// colUserEmail
			// 
			this.colUserEmail.Text = "Email";
			this.colUserEmail.Width = 50;
			// 
			// colNumClients
			// 
			this.colNumClients.Text = "Clients";
			this.colNumClients.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colNumClients.Width = 45;
			// 
			// colTotalAssigned
			// 
			this.colTotalAssigned.Text = "Assigned Urls";
			this.colTotalAssigned.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colTotalAssigned.Width = 80;
			// 
			// colTotalReturned
			// 
			this.colTotalReturned.Text = "Returned Urls";
			this.colTotalReturned.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colTotalReturned.Width = 80;
			// 
			// colRegistrationDate
			// 
			this.colRegistrationDate.Text = "Registration";
			this.colRegistrationDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colRegistrationDate.Width = 75;
			// 
			// colLastActive
			// 
			this.colLastActive.Text = "Last Active";
			this.colLastActive.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colLastActive.Width = 75;
			// 
			// cmdOK
			// 
			this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdOK.Location = new System.Drawing.Point(8, 216);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(80, 24);
			this.cmdOK.TabIndex = 1;
			this.cmdOK.Text = "Refresh";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdClose.Location = new System.Drawing.Point(96, 216);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(80, 24);
			this.cmdClose.TabIndex = 2;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// mnuUser
			// 
			this.mnuUser.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuUserDetails});
			// 
			// mnuUserDetails
			// 
			this.mnuUserDetails.Index = 0;
			this.mnuUserDetails.Text = "Detailed statistics";
			this.mnuUserDetails.Click += new System.EventHandler(this.mnuUserDetails_Click);
			// 
			// frmUserStatistics
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdClose;
			this.ClientSize = new System.Drawing.Size(536, 245);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.grpUsers);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmUserStatistics";
			this.Text = "User Statistics";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmUserStatistics_Closing);
			this.Load += new System.EventHandler(this.frmUserStatistics_Load);
			this.grpUsers.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			LoadValues();
		}

		private void cmdClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void lstUsers_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button==MouseButtons.Right)
			{
				if((lstUsers.Items.Count>0)&&(lstUsers.SelectedItems.Count>0))
				{
					mnuUser.Show(lstUsers, new Point(e.X, e.Y));
				}
			}
		}

		private void mnuUserDetails_Click(object sender, System.EventArgs e)
		{
			frmUserStatisticsDetails frm = new frmUserStatisticsDetails();
			frm.UserID = int.Parse(lstUsers.SelectedItems[0].SubItems[0].Text);
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		private void frmUserStatistics_Load(object sender, System.EventArgs e)
		{
			//we won't load the values automatically since that may take a long time
		}

		private void frmUserStatistics_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			globals.LoadedForms[this.Name] = null;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Loads the statistics from the database.
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
				SqlCommand cmd = new SqlCommand("cw_select_user_statistics_list", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				//Add the entries in the list
				lstUsers.Items.Clear();
				int itemCount = 0;
				foreach(DataRow dr in ds.Tables[0].Rows)
				{
					lstUsers.Items.Add(((int)dr[0]).ToString());
					lstUsers.Items[itemCount].SubItems.Add(((string)dr[1]).Trim());
					lstUsers.Items[itemCount].SubItems.Add(((string)dr[2]).Trim());
					lstUsers.Items[itemCount].SubItems.Add(((int)dr[3]).ToString());
					lstUsers.Items[itemCount].SubItems.Add(((long)dr[4]).ToString());
					lstUsers.Items[itemCount].SubItems.Add(((long)dr[5]).ToString());
					lstUsers.Items[itemCount].SubItems.Add(((DateTime)dr[6]).ToString());
					lstUsers.Items[itemCount].SubItems.Add(((DateTime)dr[7]).ToString());
					itemCount++;
				}
				ds.Dispose();
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to load the list of Users : " + e.ToString());
				MessageBox.Show(this.Text + " failed to load the list of Users:\n" + e.Message);
				GC.Collect();
			}
		}

		#endregion
	}
}
