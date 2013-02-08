using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using CrawlWave.Common;
using CrawlWave.Common.UI;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerManager.Forms
{
	/// <summary>
	/// frmServerStatistics allows monitoring of the Crawling process by presenting status graphs..
	/// </summary>
	public class frmServerStatistics : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private CrawlWave.Common.UI.MultiLineGraph mlgGraph;
		private System.Windows.Forms.GroupBox grpGraph;
		private System.Windows.Forms.GroupBox grpOptions;
		private System.Windows.Forms.Button cmdRefresh;
		private System.Windows.Forms.ComboBox cmbGraphType;
		private System.Windows.Forms.Label lblGraphType;
		private System.Windows.Forms.Button cmdClose;

		private Globals globals;
		private ArrayList urlsTotal, urlsToCrawl, urlData, urlLinkGraphEdges, usersTotal, clientsTotal, clientsActive, hostsTotal, hostsBanned, hostsRobots;

		/// <summary>
		/// Constructs a new instance of the <see cref="frmServerStatistics"/> form.
		/// </summary>
		public frmServerStatistics()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			globals = Globals.Instance();
			urlsTotal = new ArrayList();
			urlsToCrawl = new ArrayList();
			urlData = new ArrayList();
			urlLinkGraphEdges = new ArrayList();
			usersTotal = new ArrayList();
			clientsTotal = new ArrayList();
			clientsActive = new ArrayList();
			hostsTotal = new ArrayList();
			hostsBanned = new ArrayList();
			hostsRobots = new ArrayList();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmServerStatistics));
			this.mlgGraph = new CrawlWave.Common.UI.MultiLineGraph();
			this.grpGraph = new System.Windows.Forms.GroupBox();
			this.grpOptions = new System.Windows.Forms.GroupBox();
			this.cmdClose = new System.Windows.Forms.Button();
			this.lblGraphType = new System.Windows.Forms.Label();
			this.cmbGraphType = new System.Windows.Forms.ComboBox();
			this.cmdRefresh = new System.Windows.Forms.Button();
			this.grpGraph.SuspendLayout();
			this.grpOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// mlgGraph
			// 
			this.mlgGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mlgGraph.BackColor = System.Drawing.Color.Black;
			this.mlgGraph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mlgGraph.ForeColor = System.Drawing.Color.Bisque;
			this.mlgGraph.GridColor = System.Drawing.Color.MediumBlue;
			this.mlgGraph.Location = new System.Drawing.Point(8, 16);
			this.mlgGraph.Maximum = 0;
			this.mlgGraph.Minimum = 0;
			this.mlgGraph.Name = "mlgGraph";
			this.mlgGraph.ShowLegend = true;
			this.mlgGraph.Size = new System.Drawing.Size(440, 216);
			this.mlgGraph.TabIndex = 0;
			this.mlgGraph.XAxisText = "";
			this.mlgGraph.YAxisText = "";
			// 
			// grpGraph
			// 
			this.grpGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpGraph.Controls.Add(this.mlgGraph);
			this.grpGraph.Location = new System.Drawing.Point(8, 8);
			this.grpGraph.Name = "grpGraph";
			this.grpGraph.Size = new System.Drawing.Size(456, 240);
			this.grpGraph.TabIndex = 0;
			this.grpGraph.TabStop = false;
			this.grpGraph.Text = "Server Statistics Graph";
			// 
			// grpOptions
			// 
			this.grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpOptions.Controls.Add(this.cmdClose);
			this.grpOptions.Controls.Add(this.lblGraphType);
			this.grpOptions.Controls.Add(this.cmbGraphType);
			this.grpOptions.Controls.Add(this.cmdRefresh);
			this.grpOptions.Location = new System.Drawing.Point(8, 248);
			this.grpOptions.Name = "grpOptions";
			this.grpOptions.Size = new System.Drawing.Size(456, 48);
			this.grpOptions.TabIndex = 1;
			this.grpOptions.TabStop = false;
			this.grpOptions.Text = "Graph Options";
			// 
			// cmdClose
			// 
			this.cmdClose.Location = new System.Drawing.Point(368, 16);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(80, 24);
			this.cmdClose.TabIndex = 3;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// lblGraphType
			// 
			this.lblGraphType.Location = new System.Drawing.Point(8, 16);
			this.lblGraphType.Name = "lblGraphType";
			this.lblGraphType.Size = new System.Drawing.Size(72, 23);
			this.lblGraphType.TabIndex = 0;
			this.lblGraphType.Text = "Graph Type";
			this.lblGraphType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmbGraphType
			// 
			this.cmbGraphType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbGraphType.Items.AddRange(new object[] {
															  "Url Statistics Graph",
															  "Host Statistics Graph",
															  "User Statistics Graph"});
			this.cmbGraphType.Location = new System.Drawing.Point(88, 16);
			this.cmbGraphType.Name = "cmbGraphType";
			this.cmbGraphType.Size = new System.Drawing.Size(184, 21);
			this.cmbGraphType.TabIndex = 1;
			// 
			// cmdRefresh
			// 
			this.cmdRefresh.Location = new System.Drawing.Point(280, 16);
			this.cmdRefresh.Name = "cmdRefresh";
			this.cmdRefresh.Size = new System.Drawing.Size(80, 24);
			this.cmdRefresh.TabIndex = 2;
			this.cmdRefresh.Text = "Refresh";
			this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
			// 
			// frmServerStatistics
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 301);
			this.Controls.Add(this.grpOptions);
			this.Controls.Add(this.grpGraph);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(480, 328);
			this.Name = "frmServerStatistics";
			this.Text = "Server Statistics";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmServerStatistics_Closing);
			this.Load += new System.EventHandler(this.frmServerStatistics_Load);
			this.grpGraph.ResumeLayout(false);
			this.grpOptions.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmServerStatistics_Load(object sender, System.EventArgs e)
		{
			LoadValues();
		}

		private void frmServerStatistics_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            globals.LoadedForms[this.Name] = null;		
		}

		private void cmdRefresh_Click(object sender, System.EventArgs e)
		{
			BuildGraph();
		}

		private void cmdClose_Click(object sender, System.EventArgs e)
		{
            this.Close();		
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
				SqlCommand cmd = new SqlCommand("cw_select_server_statistics", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				//fill the data lists
				if(ds.Tables[0].Rows.Count>0)
				{
					usersTotal.Clear();
					clientsTotal.Clear();
					clientsActive.Clear();
					hostsTotal.Clear();
					hostsBanned.Clear();
					hostsRobots.Clear();
					urlsTotal.Clear();
					urlsToCrawl.Clear();
					urlData.Clear();
					urlLinkGraphEdges.Clear();
					foreach(DataRow dr in ds.Tables[0].Rows)
					{
						usersTotal.Add((double)(int)dr[2]);
						clientsTotal.Add((double)(int)dr[3]);
						clientsActive.Add((double)(int)dr[4]);
						hostsTotal.Add((double)(int)dr[5]);
						hostsBanned.Add((double)(int)dr[6]);
						hostsRobots.Add((double)(int)dr[7]);
						urlsTotal.Add((double)(int)dr[8]);
						urlsToCrawl.Add((double)(int)dr[9]);
						urlData.Add((double)(int)dr[10]);
						urlLinkGraphEdges.Add((double)(long)dr[11]);
					}
					usersTotal.Reverse();
					clientsTotal.Reverse();
					clientsActive.Reverse();
					hostsTotal.Reverse();
					hostsBanned.Reverse();
					hostsRobots.Reverse();
					urlsTotal.Reverse();
					urlsToCrawl.Reverse();
					urlData.Reverse();
					urlLinkGraphEdges.Reverse();
				}
				ds.Dispose();
			}
			catch(Exception ex)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to retrieve the Server statistics: " + ex.ToString());
				MessageBox.Show(this.Text + " failed to retrieve the Server statistics:\n" + ex.Message);
				GC.Collect();
			}
		}

		/// <summary>
		/// Builds the appropriate graph, according to the user's selection.
		/// </summary>
		private void BuildGraph()
		{
			switch(cmbGraphType.SelectedIndex)
			{
				case 0:
					//urls graph
					mlgGraph.Items.Clear();
					mlgGraph.Maximum = 100;
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.LimeGreen, urlsTotal, "Total Urls"));
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.CornflowerBlue, urlsToCrawl, "Ready Urls"));
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.Coral, urlData, "Crawled Urls"));
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.Yellow, urlLinkGraphEdges, "Link Graph"));
					mlgGraph.YAxisText = "Number of Urls";
					mlgGraph.Refresh();
					break;

				case 1:
					//hosts
					mlgGraph.Items.Clear();
					mlgGraph.Maximum = 100;
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.LimeGreen, hostsTotal, "Total hosts"));
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.CornflowerBlue, hostsBanned, "Banned hosts"));
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.Coral, hostsRobots, "Robots cache"));
					mlgGraph.YAxisText = "Number of Hosts";
					mlgGraph.Refresh();
					break;

				case 2:
					//users
					mlgGraph.Items.Clear();
					mlgGraph.Maximum = 100;
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.LimeGreen, usersTotal, "Total users"));
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.CornflowerBlue, clientsTotal, "Total clients"));
					mlgGraph.Items.Add(new MultiLineGraphItem(Color.Coral, clientsActive, "Active clients"));
					mlgGraph.YAxisText = "Number of Users / Clients";
					mlgGraph.Refresh();
					break;

				default:
					MessageBox.Show("You must select a Graph Type!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					cmbGraphType.Focus();
					break;
			}
		}

		#endregion
	}
}
