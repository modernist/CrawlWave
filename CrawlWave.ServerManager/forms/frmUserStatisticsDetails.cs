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
	/// frmUserStatisticsDetails displays detailed statistics about users participating in
	/// the project and the clients they are running.
	/// </summary>
	public class frmUserStatisticsDetails : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox grpUserInfo;
		private System.Windows.Forms.GroupBox grpUserClients;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.Label lblEmail;
		private System.Windows.Forms.Label lblUsernameVal;
		private System.Windows.Forms.Label lblEmailVal;
		private System.Windows.Forms.Label lblRegistration;
		private System.Windows.Forms.Label lblRegistrationVal;
		private System.Windows.Forms.ListView lstUserClients;
		private System.Windows.Forms.PictureBox picUser;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Globals globals;
		private System.Windows.Forms.ColumnHeader colClientID;
		private System.Windows.Forms.ColumnHeader colTotalAssigned;
		private System.Windows.Forms.ColumnHeader colTotalReturned;
		private System.Windows.Forms.ColumnHeader colLastActive;
		private System.Windows.Forms.ColumnHeader colCPUType;
		private System.Windows.Forms.ColumnHeader colRAMSize;
		private System.Windows.Forms.ColumnHeader colNetSpeed;
		private int userID;

		/// <summary>
		/// Constructs a new instance of the <see cref="frmUserStatisticsDetails"/> form.
		/// </summary>
		public frmUserStatisticsDetails()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			globals = Globals.Instance();
			userID = 0;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmUserStatisticsDetails));
			this.grpUserInfo = new System.Windows.Forms.GroupBox();
			this.picUser = new System.Windows.Forms.PictureBox();
			this.lblRegistrationVal = new System.Windows.Forms.Label();
			this.lblRegistration = new System.Windows.Forms.Label();
			this.lblEmailVal = new System.Windows.Forms.Label();
			this.lblUsernameVal = new System.Windows.Forms.Label();
			this.lblEmail = new System.Windows.Forms.Label();
			this.lblUsername = new System.Windows.Forms.Label();
			this.grpUserClients = new System.Windows.Forms.GroupBox();
			this.lstUserClients = new System.Windows.Forms.ListView();
			this.colClientID = new System.Windows.Forms.ColumnHeader();
			this.colTotalAssigned = new System.Windows.Forms.ColumnHeader();
			this.colTotalReturned = new System.Windows.Forms.ColumnHeader();
			this.colLastActive = new System.Windows.Forms.ColumnHeader();
			this.colCPUType = new System.Windows.Forms.ColumnHeader();
			this.colRAMSize = new System.Windows.Forms.ColumnHeader();
			this.colNetSpeed = new System.Windows.Forms.ColumnHeader();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.grpUserInfo.SuspendLayout();
			this.grpUserClients.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpUserInfo
			// 
			this.grpUserInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpUserInfo.Controls.Add(this.picUser);
			this.grpUserInfo.Controls.Add(this.lblRegistrationVal);
			this.grpUserInfo.Controls.Add(this.lblRegistration);
			this.grpUserInfo.Controls.Add(this.lblEmailVal);
			this.grpUserInfo.Controls.Add(this.lblUsernameVal);
			this.grpUserInfo.Controls.Add(this.lblEmail);
			this.grpUserInfo.Controls.Add(this.lblUsername);
			this.grpUserInfo.Location = new System.Drawing.Point(8, 8);
			this.grpUserInfo.Name = "grpUserInfo";
			this.grpUserInfo.Size = new System.Drawing.Size(432, 96);
			this.grpUserInfo.TabIndex = 0;
			this.grpUserInfo.TabStop = false;
			this.grpUserInfo.Text = "User Information";
			// 
			// picUser
			// 
			this.picUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.picUser.Image = ((System.Drawing.Image)(resources.GetObject("picUser.Image")));
			this.picUser.Location = new System.Drawing.Point(392, 56);
			this.picUser.Name = "picUser";
			this.picUser.Size = new System.Drawing.Size(32, 32);
			this.picUser.TabIndex = 6;
			this.picUser.TabStop = false;
			// 
			// lblRegistrationVal
			// 
			this.lblRegistrationVal.Location = new System.Drawing.Point(112, 64);
			this.lblRegistrationVal.Name = "lblRegistrationVal";
			this.lblRegistrationVal.Size = new System.Drawing.Size(176, 23);
			this.lblRegistrationVal.TabIndex = 5;
			this.lblRegistrationVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblRegistration
			// 
			this.lblRegistration.Location = new System.Drawing.Point(8, 64);
			this.lblRegistration.Name = "lblRegistration";
			this.lblRegistration.TabIndex = 4;
			this.lblRegistration.Text = "Registration Date";
			this.lblRegistration.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblEmailVal
			// 
			this.lblEmailVal.Location = new System.Drawing.Point(112, 40);
			this.lblEmailVal.Name = "lblEmailVal";
			this.lblEmailVal.Size = new System.Drawing.Size(176, 23);
			this.lblEmailVal.TabIndex = 3;
			this.lblEmailVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUsernameVal
			// 
			this.lblUsernameVal.Location = new System.Drawing.Point(112, 16);
			this.lblUsernameVal.Name = "lblUsernameVal";
			this.lblUsernameVal.Size = new System.Drawing.Size(176, 23);
			this.lblUsernameVal.TabIndex = 1;
			this.lblUsernameVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblEmail
			// 
			this.lblEmail.Location = new System.Drawing.Point(8, 40);
			this.lblEmail.Name = "lblEmail";
			this.lblEmail.TabIndex = 2;
			this.lblEmail.Text = "Email Address";
			this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUsername
			// 
			this.lblUsername.Location = new System.Drawing.Point(8, 16);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.TabIndex = 0;
			this.lblUsername.Text = "Username";
			this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// grpUserClients
			// 
			this.grpUserClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpUserClients.Controls.Add(this.lstUserClients);
			this.grpUserClients.Location = new System.Drawing.Point(8, 112);
			this.grpUserClients.Name = "grpUserClients";
			this.grpUserClients.Size = new System.Drawing.Size(432, 104);
			this.grpUserClients.TabIndex = 1;
			this.grpUserClients.TabStop = false;
			this.grpUserClients.Text = "User\'s Clients Information";
			// 
			// lstUserClients
			// 
			this.lstUserClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstUserClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							 this.colClientID,
																							 this.colTotalAssigned,
																							 this.colTotalReturned,
																							 this.colLastActive,
																							 this.colCPUType,
																							 this.colRAMSize,
																							 this.colNetSpeed});
			this.lstUserClients.FullRowSelect = true;
			this.lstUserClients.GridLines = true;
			this.lstUserClients.Location = new System.Drawing.Point(8, 16);
			this.lstUserClients.Name = "lstUserClients";
			this.lstUserClients.Size = new System.Drawing.Size(416, 80);
			this.lstUserClients.TabIndex = 0;
			this.lstUserClients.View = System.Windows.Forms.View.Details;
			// 
			// colClientID
			// 
			this.colClientID.Text = "Client ID";
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
			// colLastActive
			// 
			this.colLastActive.Text = "Last Active";
			this.colLastActive.Width = 70;
			// 
			// colCPUType
			// 
			this.colCPUType.Text = "CPU Type";
			this.colCPUType.Width = 70;
			// 
			// colRAMSize
			// 
			this.colRAMSize.Text = "RAM Size";
			// 
			// colNetSpeed
			// 
			this.colNetSpeed.Text = "Net Speed";
			this.colNetSpeed.Width = 70;
			// 
			// cmdOK
			// 
			this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdOK.Location = new System.Drawing.Point(8, 224);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(80, 24);
			this.cmdOK.TabIndex = 2;
			this.cmdOK.Text = "Refresh";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdClose.Location = new System.Drawing.Point(96, 224);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(80, 24);
			this.cmdClose.TabIndex = 3;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// frmUserStatisticsDetails
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdClose;
			this.ClientSize = new System.Drawing.Size(448, 253);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.grpUserClients);
			this.Controls.Add(this.grpUserInfo);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "frmUserStatisticsDetails";
			this.Text = "User\'s detailed statistics";
			this.grpUserInfo.ResumeLayout(false);
			this.grpUserClients.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Public Properties

		/// <summary>
		/// Sets the User ID of the user whose details must be displayed and causes the
		/// details to be loaded.
		/// </summary>
		public int UserID
		{
			set 
			{ 
				userID = value;
				LoadValues();
			}
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

		#endregion

		#region Private Methods

		/// <summary>
		/// Loads the detailed statistics of a user.
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
				SqlCommand cmd = new SqlCommand("cw_select_user_statistic", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@user_id", SqlDbType.Int);
				cmd.Parameters[0].Value = userID;
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				//Add the entries in the list
				lstUserClients.Items.Clear();
				int itemCount = 0;
				foreach(DataRow dr in ds.Tables[0].Rows)
				{
					lstUserClients.Items.Add(((Guid)dr[3]).ToString());
					lstUserClients.Items[itemCount].SubItems.Add(((long)dr[4]).ToString());
					lstUserClients.Items[itemCount].SubItems.Add(((long)dr[5]).ToString());
					lstUserClients.Items[itemCount].SubItems.Add(((DateTime)dr[6]).ToString());
					if(dr[7]!=DBNull.Value)
					{
						lstUserClients.Items[itemCount].SubItems.Add(((string)dr[7]).Trim());
					}
					else
					{
						lstUserClients.Items[itemCount].SubItems.Add(String.Empty);
					}
					if(dr[8]!=DBNull.Value)
					{
						lstUserClients.Items[itemCount].SubItems.Add(((short)dr[8]).ToString());
					}
					else
					{
						lstUserClients.Items[itemCount].SubItems.Add(String.Empty);
					}
					if(dr[9]!=DBNull.Value)
					{
						CWConnectionSpeed speed = (CWConnectionSpeed)((byte)dr[9]);
						lstUserClients.Items[itemCount].SubItems.Add(speed.ToString());
					}
					else
					{
						lstUserClients.Items[itemCount].SubItems.Add("Unknown");
					}
					itemCount++;
				}
				if(itemCount > 0)
				{
					DataRow dr = ds.Tables[0].Rows[0];
					lblUsernameVal.Text = ((string)dr[0]).Trim();
					lblEmailVal.Text = ((string)dr[1]).Trim();
					lblRegistrationVal.Text = ((DateTime)dr[2]).ToString();
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

		#endregion
	}
}
