using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace CrawlWave.ServerManager.Forms
{
	/// <summary>
	/// frmMain is the main form of the CrawlWave Server Manager.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private XPExplorerBar.TaskPane tskTasks;
		private XPExplorerBar.Expando expHostTasks;
		private XPExplorerBar.Expando expStatisticsTasks;
		private XPExplorerBar.Expando expVersioningTasks;
		private System.Windows.Forms.ImageList imgIcons;
		private XPExplorerBar.TaskItem tsiManageBannedHosts;
		private XPExplorerBar.TaskItem tsiAddUrlToCrawl;
		private XPExplorerBar.TaskItem tsiUserStatistics;
		private XPExplorerBar.TaskItem tsiServerStatistics;
		private XPExplorerBar.TaskItem tsiAddClientUpdate;
		private XPExplorerBar.TaskItem tsiManageServerList;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MainMenu mnuMain;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.MenuItem mnuWindows;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuHelpAbout;

		private Globals globals;

		/// <summary>
		/// Constructs a new instance of the <see cref="frmMain"/> form.
		/// </summary>
		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			tskTasks.UseCustomTheme("Shellstyle.dll");

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tskTasks = new XPExplorerBar.TaskPane();
            this.expHostTasks = new XPExplorerBar.Expando();
            this.tsiManageBannedHosts = new XPExplorerBar.TaskItem();
            this.imgIcons = new System.Windows.Forms.ImageList(this.components);
            this.tsiAddUrlToCrawl = new XPExplorerBar.TaskItem();
            this.expStatisticsTasks = new XPExplorerBar.Expando();
            this.tsiUserStatistics = new XPExplorerBar.TaskItem();
            this.tsiServerStatistics = new XPExplorerBar.TaskItem();
            this.expVersioningTasks = new XPExplorerBar.Expando();
            this.tsiAddClientUpdate = new XPExplorerBar.TaskItem();
            this.tsiManageServerList = new XPExplorerBar.TaskItem();
            this.mnuMain = new System.Windows.Forms.MainMenu(this.components);
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.mnuWindows = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.tskTasks)).BeginInit();
            this.tskTasks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expHostTasks)).BeginInit();
            this.expHostTasks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expStatisticsTasks)).BeginInit();
            this.expStatisticsTasks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expVersioningTasks)).BeginInit();
            this.expVersioningTasks.SuspendLayout();
            this.SuspendLayout();
            // 
            // tskTasks
            // 
            this.tskTasks.AutoScroll = true;
            this.tskTasks.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.tskTasks.CustomSettings.GradientEndColor = System.Drawing.Color.SlateGray;
            this.tskTasks.CustomSettings.Watermark = ((System.Drawing.Image)(resources.GetObject("resource.Watermark")));
            this.tskTasks.Dock = System.Windows.Forms.DockStyle.Left;
            this.tskTasks.Expandos.AddRange(new XPExplorerBar.Expando[] {
            this.expHostTasks,
            this.expStatisticsTasks,
            this.expVersioningTasks});
            this.tskTasks.Location = new System.Drawing.Point(0, 0);
            this.tskTasks.Name = "tskTasks";
            this.tskTasks.Size = new System.Drawing.Size(200, 405);
            this.tskTasks.TabIndex = 1;
            this.tskTasks.Text = "Task Pane";
            // 
            // expHostTasks
            // 
            this.expHostTasks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.expHostTasks.Animate = true;
            this.expHostTasks.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expHostTasks.Items.AddRange(new System.Windows.Forms.Control[] {
            this.tsiManageBannedHosts,
            this.tsiAddUrlToCrawl});
            this.expHostTasks.Location = new System.Drawing.Point(12, 12);
            this.expHostTasks.Name = "expHostTasks";
            this.expHostTasks.Size = new System.Drawing.Size(176, 100);
            this.expHostTasks.TabIndex = 0;
            this.expHostTasks.Text = "Hosts Tasks";
            this.expHostTasks.TitleImage = ((System.Drawing.Image)(resources.GetObject("expHostTasks.TitleImage")));
            // 
            // tsiManageBannedHosts
            // 
            this.tsiManageBannedHosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tsiManageBannedHosts.BackColor = System.Drawing.Color.Transparent;
            this.tsiManageBannedHosts.Image = ((System.Drawing.Image)(resources.GetObject("tsiManageBannedHosts.Image")));
            this.tsiManageBannedHosts.ImageIndex = 0;
            this.tsiManageBannedHosts.ImageList = this.imgIcons;
            this.tsiManageBannedHosts.Location = new System.Drawing.Point(8, 48);
            this.tsiManageBannedHosts.Name = "tsiManageBannedHosts";
            this.tsiManageBannedHosts.Size = new System.Drawing.Size(150, 16);
            this.tsiManageBannedHosts.TabIndex = 0;
            this.tsiManageBannedHosts.Text = "Manage Banned Hosts";
            this.tsiManageBannedHosts.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tsiManageBannedHosts.UseVisualStyleBackColor = false;
            this.tsiManageBannedHosts.Click += new System.EventHandler(this.tsiManageBannedHosts_Click);
            // 
            // imgIcons
            // 
            this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
            this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imgIcons.Images.SetKeyName(0, "");
            this.imgIcons.Images.SetKeyName(1, "");
            this.imgIcons.Images.SetKeyName(2, "");
            this.imgIcons.Images.SetKeyName(3, "");
            this.imgIcons.Images.SetKeyName(4, "");
            this.imgIcons.Images.SetKeyName(5, "");
            this.imgIcons.Images.SetKeyName(6, "");
            this.imgIcons.Images.SetKeyName(7, "");
            // 
            // tsiAddUrlToCrawl
            // 
            this.tsiAddUrlToCrawl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tsiAddUrlToCrawl.BackColor = System.Drawing.Color.Transparent;
            this.tsiAddUrlToCrawl.Image = ((System.Drawing.Image)(resources.GetObject("tsiAddUrlToCrawl.Image")));
            this.tsiAddUrlToCrawl.ImageIndex = 1;
            this.tsiAddUrlToCrawl.ImageList = this.imgIcons;
            this.tsiAddUrlToCrawl.Location = new System.Drawing.Point(8, 72);
            this.tsiAddUrlToCrawl.Name = "tsiAddUrlToCrawl";
            this.tsiAddUrlToCrawl.Size = new System.Drawing.Size(150, 16);
            this.tsiAddUrlToCrawl.TabIndex = 1;
            this.tsiAddUrlToCrawl.Text = "Add a new Url to crawl";
            this.tsiAddUrlToCrawl.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tsiAddUrlToCrawl.UseVisualStyleBackColor = false;
            this.tsiAddUrlToCrawl.Click += new System.EventHandler(this.tsiAddUrlToCrawl_Click);
            // 
            // expStatisticsTasks
            // 
            this.expStatisticsTasks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.expStatisticsTasks.Animate = true;
            this.expStatisticsTasks.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expStatisticsTasks.Items.AddRange(new System.Windows.Forms.Control[] {
            this.tsiUserStatistics,
            this.tsiServerStatistics});
            this.expStatisticsTasks.Location = new System.Drawing.Point(12, 124);
            this.expStatisticsTasks.Name = "expStatisticsTasks";
            this.expStatisticsTasks.Size = new System.Drawing.Size(176, 100);
            this.expStatisticsTasks.TabIndex = 1;
            this.expStatisticsTasks.Text = "Statistics Tasks";
            this.expStatisticsTasks.TitleImage = ((System.Drawing.Image)(resources.GetObject("expStatisticsTasks.TitleImage")));
            // 
            // tsiUserStatistics
            // 
            this.tsiUserStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tsiUserStatistics.BackColor = System.Drawing.Color.Transparent;
            this.tsiUserStatistics.Image = ((System.Drawing.Image)(resources.GetObject("tsiUserStatistics.Image")));
            this.tsiUserStatistics.ImageIndex = 2;
            this.tsiUserStatistics.ImageList = this.imgIcons;
            this.tsiUserStatistics.Location = new System.Drawing.Point(8, 48);
            this.tsiUserStatistics.Name = "tsiUserStatistics";
            this.tsiUserStatistics.Size = new System.Drawing.Size(150, 16);
            this.tsiUserStatistics.TabIndex = 0;
            this.tsiUserStatistics.Text = "View User statistics";
            this.tsiUserStatistics.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tsiUserStatistics.UseVisualStyleBackColor = false;
            this.tsiUserStatistics.Click += new System.EventHandler(this.tsiUserStatistics_Click);
            // 
            // tsiServerStatistics
            // 
            this.tsiServerStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tsiServerStatistics.BackColor = System.Drawing.Color.Transparent;
            this.tsiServerStatistics.Image = ((System.Drawing.Image)(resources.GetObject("tsiServerStatistics.Image")));
            this.tsiServerStatistics.ImageIndex = 3;
            this.tsiServerStatistics.ImageList = this.imgIcons;
            this.tsiServerStatistics.Location = new System.Drawing.Point(8, 72);
            this.tsiServerStatistics.Name = "tsiServerStatistics";
            this.tsiServerStatistics.Size = new System.Drawing.Size(150, 16);
            this.tsiServerStatistics.TabIndex = 1;
            this.tsiServerStatistics.Text = "View Server statistics";
            this.tsiServerStatistics.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tsiServerStatistics.UseVisualStyleBackColor = false;
            this.tsiServerStatistics.Click += new System.EventHandler(this.tsiServerStatistics_Click);
            // 
            // expVersioningTasks
            // 
            this.expVersioningTasks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.expVersioningTasks.Animate = true;
            this.expVersioningTasks.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.expVersioningTasks.Items.AddRange(new System.Windows.Forms.Control[] {
            this.tsiAddClientUpdate,
            this.tsiManageServerList});
            this.expVersioningTasks.Location = new System.Drawing.Point(12, 236);
            this.expVersioningTasks.Name = "expVersioningTasks";
            this.expVersioningTasks.Size = new System.Drawing.Size(176, 100);
            this.expVersioningTasks.TabIndex = 2;
            this.expVersioningTasks.Text = "Versioning Tasks";
            this.expVersioningTasks.TitleImage = ((System.Drawing.Image)(resources.GetObject("expVersioningTasks.TitleImage")));
            // 
            // tsiAddClientUpdate
            // 
            this.tsiAddClientUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tsiAddClientUpdate.BackColor = System.Drawing.Color.Transparent;
            this.tsiAddClientUpdate.Image = ((System.Drawing.Image)(resources.GetObject("tsiAddClientUpdate.Image")));
            this.tsiAddClientUpdate.ImageIndex = 5;
            this.tsiAddClientUpdate.ImageList = this.imgIcons;
            this.tsiAddClientUpdate.Location = new System.Drawing.Point(8, 48);
            this.tsiAddClientUpdate.Name = "tsiAddClientUpdate";
            this.tsiAddClientUpdate.Size = new System.Drawing.Size(150, 16);
            this.tsiAddClientUpdate.TabIndex = 1;
            this.tsiAddClientUpdate.Text = "Add a Client Update";
            this.tsiAddClientUpdate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tsiAddClientUpdate.UseVisualStyleBackColor = false;
            this.tsiAddClientUpdate.Click += new System.EventHandler(this.tsiAddClientUpdate_Click);
            // 
            // tsiManageServerList
            // 
            this.tsiManageServerList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tsiManageServerList.BackColor = System.Drawing.Color.Transparent;
            this.tsiManageServerList.Image = ((System.Drawing.Image)(resources.GetObject("tsiManageServerList.Image")));
            this.tsiManageServerList.ImageIndex = 7;
            this.tsiManageServerList.ImageList = this.imgIcons;
            this.tsiManageServerList.Location = new System.Drawing.Point(8, 72);
            this.tsiManageServerList.Name = "tsiManageServerList";
            this.tsiManageServerList.Size = new System.Drawing.Size(150, 16);
            this.tsiManageServerList.TabIndex = 2;
            this.tsiManageServerList.Text = "Manage Server List";
            this.tsiManageServerList.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.tsiManageServerList.UseVisualStyleBackColor = false;
            this.tsiManageServerList.Click += new System.EventHandler(this.tsiManageServerList_Click);
            // 
            // mnuMain
            // 
            this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuExit,
            this.mnuWindows,
            this.mnuHelp});
            // 
            // mnuExit
            // 
            this.mnuExit.Index = 0;
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuWindows
            // 
            this.mnuWindows.Index = 1;
            this.mnuWindows.MdiList = true;
            this.mnuWindows.Text = "Windows";
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 2;
            this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Text = "Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Index = 0;
            this.mnuHelpAbout.Text = "About";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // frmMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(736, 405);
            this.Controls.Add(this.tskTasks);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Menu = this.mnuMain;
            this.Name = "frmMain";
            this.Text = "CrawlWave Server Manager";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tskTasks)).EndInit();
            this.tskTasks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expHostTasks)).EndInit();
            this.expHostTasks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expStatisticsTasks)).EndInit();
            this.expStatisticsTasks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.expVersioningTasks)).EndInit();
            this.expVersioningTasks.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			Application.Run(new frmMain());
		}

		#region Event Handlers

		private void tsiManageBannedHosts_Click(object sender, System.EventArgs e)
		{
			if(!globals.IsFormLoaded("frmBannedHosts"))
			{
				frmBannedHosts frmBanned = new frmBannedHosts();
				frmBanned.MdiParent = this;
				frmBanned.Show();
			}
			else
			{
				frmBannedHosts frm = (frmBannedHosts)globals.LoadedForms["frmBannedHosts"];
				frm.Focus();
			}
		}

		private void tsiAddUrlToCrawl_Click(object sender, System.EventArgs e)
		{
			if(!globals.IsFormLoaded("frmInsertUrl"))
			{
				frmInsertUrl frm = new frmInsertUrl();
				frm.MdiParent = this;
				frm.Show();
			}
			else
			{
                frmInsertUrl frm = (frmInsertUrl)globals.LoadedForms["frmInsertUrl"];
				frm.Focus();
			}
		}

		private void tsiUserStatistics_Click(object sender, System.EventArgs e)
		{
			if(!globals.IsFormLoaded("frmUserStatistics"))
			{
				frmUserStatistics frm = new frmUserStatistics();
				frm.MdiParent = this;
				frm.Show();
			}
			else
			{
				frmUserStatistics frm = (frmUserStatistics)globals.LoadedForms["frmUserStatistics"];
				frm.Focus();
			}
		}

		private void tsiServerStatistics_Click(object sender, System.EventArgs e)
		{
			if(!globals.IsFormLoaded("frmServerStatistics"))
			{
				frmServerStatistics frm = new frmServerStatistics();
				frm.MdiParent = this;
				frm.Show();
			}
			else
			{
				frmServerStatistics frm = (frmServerStatistics)globals.LoadedForms["frmServerStatistics"];
				frm.Focus();
			}
		}

		private void tsiAddClientUpdate_Click(object sender, System.EventArgs e)
		{
			if(!globals.IsFormLoaded("frmClientUpdate"))
			{
				frmClientUpdate frm = new frmClientUpdate();
				frm.MdiParent = this;
				frm.Show();
			}
			else
			{
				frmClientUpdate frm = (frmClientUpdate)globals.LoadedForms["frmClientUpdate"];
				frm.Focus();
			}
		}

		private void tsiManageServerList_Click(object sender, System.EventArgs e)
		{
			if(!globals.IsFormLoaded("frmServerList"))
			{
				frmServerList frm = new frmServerList();
				frm.MdiParent = this;
				frm.Show();
			}
			else
			{
				frmServerList frm = (frmServerList)globals.LoadedForms["frmServerList"];
				frm.Focus();
			}
		}
		private void frmMain_Load(object sender, System.EventArgs e)
		{
			frmSplash frm = new frmSplash();
			frm.ShowDialog();
			frm = null;
		}

		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void mnuHelpAbout_Click(object sender, System.EventArgs e)
		{
			if(!globals.IsFormLoaded("frmAbout"))
			{
				frmAbout frm = new frmAbout();
				frm.MdiParent = this;
				frm.Show();
			}
			else
			{
				frmAbout frm = (frmAbout)globals.LoadedForms["frmAbout"];
				frm.Focus();
			}			
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show("An unhandled Exception occured: " + ((Exception)e.ExceptionObject).Message, "CrawlWave Server Manager", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			MessageBox.Show("An unhandled Application Thread Exception occured: " + ((Exception)e.Exception).Message, "CrawlWave Server Manager", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		#endregion

	}
}
