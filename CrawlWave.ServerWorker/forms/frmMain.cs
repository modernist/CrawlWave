using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerWorker.Forms
{
	/// <summary>
	/// frmMain is the main form of the CrawlWave ServerWorker.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenu mnuPlugin;
		private System.Windows.Forms.MenuItem mnuPluginStart;
		private System.Windows.Forms.MenuItem mnuPluginStop;
		private System.Windows.Forms.MenuItem mnuPluginPause;
		private System.Windows.Forms.MenuItem mnuPluginSettings;
		private System.Windows.Forms.ColumnHeader colPluginName;
		private System.Windows.Forms.ColumnHeader colPluginState;
		private System.Windows.Forms.ColumnHeader colVersion;
		private CrawlWave.Common.UI.HealthBarGraph hbgMemory;
		private CrawlWave.Common.UI.HealthLineGraph hlgMemory;
		private CrawlWave.Common.UI.RichTextboxLogger rtblLog;
		private System.Windows.Forms.StatusBar sbStatus;
		private System.Windows.Forms.StatusBarPanel sbPanelPluginName;
		private System.Windows.Forms.Label lblMemUsage;
		private System.Windows.Forms.Label lblMemHistory;
		private System.Windows.Forms.ListView lstPlugins;
		private System.Windows.Forms.OpenFileDialog dlgBrowse;
		private System.Windows.Forms.Timer tmrTimer;
		private System.Windows.Forms.ImageList imgIcons;
		private System.Windows.Forms.TabControl tabTabPages;
		private System.Windows.Forms.TabPage tabStatus;
		private System.Windows.Forms.TabPage tabSettings;
		private System.Windows.Forms.Button cmdLoadPlugin;
		private System.Windows.Forms.Button cmdRemovePlugin;
		private System.Windows.Forms.Button cmdSaveSettings;
		private System.Windows.Forms.Button cmdLoadDefaults;
		private System.Windows.Forms.MenuItem mnuPluginResume;
		private System.Windows.Forms.CheckBox chkLoadPlugins;
		private System.Windows.Forms.CheckBox chkMinimizeToTray;
		private System.Windows.Forms.NotifyIcon nfiTrayIcon;
		private System.Windows.Forms.CheckBox chkMinimizeOnExit;
		private System.Windows.Forms.Button cmdExit;
		private System.Windows.Forms.TabPage tabLog;
		private System.Windows.Forms.TabPage tabAbout;
		private System.Windows.Forms.PictureBox picAbout;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.Label lblVersion;

		private Settings settings;
		private PluginController controller;
		private ArrayList pluginPaths;
		private int memUsage;

		/// <summary>
		/// Creates a new instance of the <see cref="frmMain"/> form.
		/// </summary>
		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			settings = Settings.Instance();
			controller = new PluginController();
			pluginPaths = new ArrayList();
			controller.AttachLogger(rtblLog);
			controller.PluginStateChanged += new EventHandler(controller_PluginStateChanged);
			memUsage = 0;
			lblVersion.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMain));
			this.hbgMemory = new CrawlWave.Common.UI.HealthBarGraph();
			this.hlgMemory = new CrawlWave.Common.UI.HealthLineGraph();
			this.sbStatus = new System.Windows.Forms.StatusBar();
			this.sbPanelPluginName = new System.Windows.Forms.StatusBarPanel();
			this.mnuPlugin = new System.Windows.Forms.ContextMenu();
			this.mnuPluginStart = new System.Windows.Forms.MenuItem();
			this.mnuPluginPause = new System.Windows.Forms.MenuItem();
			this.mnuPluginResume = new System.Windows.Forms.MenuItem();
			this.mnuPluginStop = new System.Windows.Forms.MenuItem();
			this.mnuPluginSettings = new System.Windows.Forms.MenuItem();
			this.lstPlugins = new System.Windows.Forms.ListView();
			this.colPluginName = new System.Windows.Forms.ColumnHeader();
			this.colPluginState = new System.Windows.Forms.ColumnHeader();
			this.colVersion = new System.Windows.Forms.ColumnHeader();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.dlgBrowse = new System.Windows.Forms.OpenFileDialog();
			this.tmrTimer = new System.Windows.Forms.Timer(this.components);
			this.lblMemHistory = new System.Windows.Forms.Label();
			this.lblMemUsage = new System.Windows.Forms.Label();
			this.tabTabPages = new System.Windows.Forms.TabControl();
			this.tabStatus = new System.Windows.Forms.TabPage();
			this.cmdExit = new System.Windows.Forms.Button();
			this.cmdRemovePlugin = new System.Windows.Forms.Button();
			this.cmdLoadPlugin = new System.Windows.Forms.Button();
			this.tabSettings = new System.Windows.Forms.TabPage();
			this.chkMinimizeOnExit = new System.Windows.Forms.CheckBox();
			this.chkMinimizeToTray = new System.Windows.Forms.CheckBox();
			this.chkLoadPlugins = new System.Windows.Forms.CheckBox();
			this.cmdSaveSettings = new System.Windows.Forms.Button();
			this.cmdLoadDefaults = new System.Windows.Forms.Button();
			this.tabLog = new System.Windows.Forms.TabPage();
			this.rtblLog = new CrawlWave.Common.UI.RichTextboxLogger();
			this.tabAbout = new System.Windows.Forms.TabPage();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.picAbout = new System.Windows.Forms.PictureBox();
			this.nfiTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			((System.ComponentModel.ISupportInitialize)(this.sbPanelPluginName)).BeginInit();
			this.tabTabPages.SuspendLayout();
			this.tabStatus.SuspendLayout();
			this.tabSettings.SuspendLayout();
			this.tabLog.SuspendLayout();
			this.tabAbout.SuspendLayout();
			this.SuspendLayout();
			// 
			// hbgMemory
			// 
			this.hbgMemory.Label = " KB";
			this.hbgMemory.Location = new System.Drawing.Point(8, 8);
			this.hbgMemory.Maximum = 200000;
			this.hbgMemory.Minimum = 0;
			this.hbgMemory.Name = "hbgMemory";
			this.hbgMemory.Size = new System.Drawing.Size(80, 80);
			this.hbgMemory.TabIndex = 0;
			this.hbgMemory.Value = 0;
			// 
			// hlgMemory
			// 
			this.hlgMemory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.hlgMemory.BackColor = System.Drawing.Color.Black;
			this.hlgMemory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.hlgMemory.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.hlgMemory.GridColor = System.Drawing.Color.MediumBlue;
			this.hlgMemory.Location = new System.Drawing.Point(96, 8);
			this.hlgMemory.Maximum = 100;
			this.hlgMemory.Minimum = 0;
			this.hlgMemory.Name = "hlgMemory";
			this.hlgMemory.Size = new System.Drawing.Size(400, 80);
			this.hlgMemory.TabIndex = 1;
			// 
			// sbStatus
			// 
			this.sbStatus.Location = new System.Drawing.Point(0, 271);
			this.sbStatus.Name = "sbStatus";
			this.sbStatus.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						this.sbPanelPluginName});
			this.sbStatus.ShowPanels = true;
			this.sbStatus.Size = new System.Drawing.Size(528, 22);
			this.sbStatus.TabIndex = 1;
			// 
			// sbPanelPluginName
			// 
			this.sbPanelPluginName.Width = 350;
			// 
			// mnuPlugin
			// 
			this.mnuPlugin.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuPluginStart,
																					  this.mnuPluginPause,
																					  this.mnuPluginResume,
																					  this.mnuPluginStop,
																					  this.mnuPluginSettings});
			// 
			// mnuPluginStart
			// 
			this.mnuPluginStart.Index = 0;
			this.mnuPluginStart.Text = "Start Plugin";
			this.mnuPluginStart.Click += new System.EventHandler(this.mnuPluginStart_Click);
			// 
			// mnuPluginPause
			// 
			this.mnuPluginPause.Index = 1;
			this.mnuPluginPause.Text = "Pause Plugin";
			this.mnuPluginPause.Click += new System.EventHandler(this.mnuPluginPause_Click);
			// 
			// mnuPluginResume
			// 
			this.mnuPluginResume.Index = 2;
			this.mnuPluginResume.Text = "Resume Plugin";
			this.mnuPluginResume.Click += new System.EventHandler(this.mnuPluginResume_Click);
			// 
			// mnuPluginStop
			// 
			this.mnuPluginStop.Index = 3;
			this.mnuPluginStop.Text = "Stop Plugin";
			this.mnuPluginStop.Click += new System.EventHandler(this.mnuPluginStop_Click);
			// 
			// mnuPluginSettings
			// 
			this.mnuPluginSettings.Index = 4;
			this.mnuPluginSettings.Text = "Plugin Settings";
			this.mnuPluginSettings.Click += new System.EventHandler(this.mnuPluginSettings_Click);
			// 
			// lstPlugins
			// 
			this.lstPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.colPluginName,
																						 this.colPluginState,
																						 this.colVersion});
			this.lstPlugins.FullRowSelect = true;
			this.lstPlugins.GridLines = true;
			this.lstPlugins.Location = new System.Drawing.Point(8, 104);
			this.lstPlugins.MultiSelect = false;
			this.lstPlugins.Name = "lstPlugins";
			this.lstPlugins.Size = new System.Drawing.Size(488, 87);
			this.lstPlugins.SmallImageList = this.imgIcons;
			this.lstPlugins.TabIndex = 4;
			this.lstPlugins.View = System.Windows.Forms.View.Details;
			this.lstPlugins.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstPlugins_MouseUp);
			this.lstPlugins.SelectedIndexChanged += new System.EventHandler(this.lstPlugins_SelectedIndexChanged);
			// 
			// colPluginName
			// 
			this.colPluginName.Text = "Plugin Name";
			this.colPluginName.Width = 280;
			// 
			// colPluginState
			// 
			this.colPluginState.Text = "State";
			// 
			// colVersion
			// 
			this.colVersion.Text = "Version";
			this.colVersion.Width = 88;
			// 
			// imgIcons
			// 
			this.imgIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// dlgBrowse
			// 
			this.dlgBrowse.Filter = "Dynamic Link Libraries|*.dll";
			this.dlgBrowse.Title = "Select a CrawlWave Plugin...";
			// 
			// tmrTimer
			// 
			this.tmrTimer.Enabled = true;
			this.tmrTimer.Interval = 1000;
			this.tmrTimer.Tick += new System.EventHandler(this.tmrTimer_Tick);
			// 
			// lblMemHistory
			// 
			this.lblMemHistory.Location = new System.Drawing.Point(96, 88);
			this.lblMemHistory.Name = "lblMemHistory";
			this.lblMemHistory.Size = new System.Drawing.Size(128, 16);
			this.lblMemHistory.TabIndex = 3;
			this.lblMemHistory.Text = "Memory Usage History";
			// 
			// lblMemUsage
			// 
			this.lblMemUsage.Location = new System.Drawing.Point(8, 88);
			this.lblMemUsage.Name = "lblMemUsage";
			this.lblMemUsage.Size = new System.Drawing.Size(88, 16);
			this.lblMemUsage.TabIndex = 2;
			this.lblMemUsage.Text = "Memory Usage";
			// 
			// tabTabPages
			// 
			this.tabTabPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabTabPages.Controls.Add(this.tabStatus);
			this.tabTabPages.Controls.Add(this.tabSettings);
			this.tabTabPages.Controls.Add(this.tabLog);
			this.tabTabPages.Controls.Add(this.tabAbout);
			this.tabTabPages.ImageList = this.imgIcons;
			this.tabTabPages.Location = new System.Drawing.Point(8, 8);
			this.tabTabPages.Name = "tabTabPages";
			this.tabTabPages.SelectedIndex = 0;
			this.tabTabPages.Size = new System.Drawing.Size(512, 256);
			this.tabTabPages.TabIndex = 0;
			this.tabTabPages.SelectedIndexChanged += new System.EventHandler(this.tabTabPages_SelectedIndexChanged);
			// 
			// tabStatus
			// 
			this.tabStatus.Controls.Add(this.cmdExit);
			this.tabStatus.Controls.Add(this.cmdRemovePlugin);
			this.tabStatus.Controls.Add(this.cmdLoadPlugin);
			this.tabStatus.Controls.Add(this.hbgMemory);
			this.tabStatus.Controls.Add(this.hlgMemory);
			this.tabStatus.Controls.Add(this.lblMemUsage);
			this.tabStatus.Controls.Add(this.lblMemHistory);
			this.tabStatus.Controls.Add(this.lstPlugins);
			this.tabStatus.ImageIndex = 4;
			this.tabStatus.Location = new System.Drawing.Point(4, 23);
			this.tabStatus.Name = "tabStatus";
			this.tabStatus.Size = new System.Drawing.Size(504, 229);
			this.tabStatus.TabIndex = 0;
			this.tabStatus.Text = "Status";
			// 
			// cmdExit
			// 
			this.cmdExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdExit.Location = new System.Drawing.Point(216, 200);
			this.cmdExit.Name = "cmdExit";
			this.cmdExit.Size = new System.Drawing.Size(96, 24);
			this.cmdExit.TabIndex = 7;
			this.cmdExit.Text = "Stop and Exit";
			this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
			// 
			// cmdRemovePlugin
			// 
			this.cmdRemovePlugin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdRemovePlugin.Enabled = false;
			this.cmdRemovePlugin.Location = new System.Drawing.Point(112, 200);
			this.cmdRemovePlugin.Name = "cmdRemovePlugin";
			this.cmdRemovePlugin.Size = new System.Drawing.Size(96, 24);
			this.cmdRemovePlugin.TabIndex = 6;
			this.cmdRemovePlugin.Text = "Remove Plugin";
			this.cmdRemovePlugin.Click += new System.EventHandler(this.cmdRemovePlugin_Click);
			// 
			// cmdLoadPlugin
			// 
			this.cmdLoadPlugin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdLoadPlugin.Location = new System.Drawing.Point(8, 200);
			this.cmdLoadPlugin.Name = "cmdLoadPlugin";
			this.cmdLoadPlugin.Size = new System.Drawing.Size(96, 24);
			this.cmdLoadPlugin.TabIndex = 5;
			this.cmdLoadPlugin.Text = "Load Plugin...";
			this.cmdLoadPlugin.Click += new System.EventHandler(this.cmdLoadPlugin_Click);
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.chkMinimizeOnExit);
			this.tabSettings.Controls.Add(this.chkMinimizeToTray);
			this.tabSettings.Controls.Add(this.chkLoadPlugins);
			this.tabSettings.Controls.Add(this.cmdSaveSettings);
			this.tabSettings.Controls.Add(this.cmdLoadDefaults);
			this.tabSettings.ImageIndex = 5;
			this.tabSettings.Location = new System.Drawing.Point(4, 23);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.Size = new System.Drawing.Size(504, 229);
			this.tabSettings.TabIndex = 1;
			this.tabSettings.Text = "Settings";
			// 
			// chkMinimizeOnExit
			// 
			this.chkMinimizeOnExit.Location = new System.Drawing.Point(8, 32);
			this.chkMinimizeOnExit.Name = "chkMinimizeOnExit";
			this.chkMinimizeOnExit.Size = new System.Drawing.Size(256, 24);
			this.chkMinimizeOnExit.TabIndex = 2;
			this.chkMinimizeOnExit.Text = "Minimize on Exit";
			// 
			// chkMinimizeToTray
			// 
			this.chkMinimizeToTray.Location = new System.Drawing.Point(8, 56);
			this.chkMinimizeToTray.Name = "chkMinimizeToTray";
			this.chkMinimizeToTray.Size = new System.Drawing.Size(256, 24);
			this.chkMinimizeToTray.TabIndex = 3;
			this.chkMinimizeToTray.Text = "Minimize to System Tray";
			// 
			// chkLoadPlugins
			// 
			this.chkLoadPlugins.Location = new System.Drawing.Point(8, 8);
			this.chkLoadPlugins.Name = "chkLoadPlugins";
			this.chkLoadPlugins.Size = new System.Drawing.Size(256, 24);
			this.chkLoadPlugins.TabIndex = 0;
			this.chkLoadPlugins.Text = "Load Plugins at startup";
			// 
			// cmdSaveSettings
			// 
			this.cmdSaveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdSaveSettings.Location = new System.Drawing.Point(112, 200);
			this.cmdSaveSettings.Name = "cmdSaveSettings";
			this.cmdSaveSettings.Size = new System.Drawing.Size(96, 24);
			this.cmdSaveSettings.TabIndex = 5;
			this.cmdSaveSettings.Text = "Save Settings";
			this.cmdSaveSettings.Click += new System.EventHandler(this.cmdSaveSettings_Click);
			// 
			// cmdLoadDefaults
			// 
			this.cmdLoadDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdLoadDefaults.Location = new System.Drawing.Point(8, 200);
			this.cmdLoadDefaults.Name = "cmdLoadDefaults";
			this.cmdLoadDefaults.Size = new System.Drawing.Size(96, 24);
			this.cmdLoadDefaults.TabIndex = 4;
			this.cmdLoadDefaults.Text = "Load Defaults";
			this.cmdLoadDefaults.Click += new System.EventHandler(this.cmdLoadDefaults_Click);
			// 
			// tabLog
			// 
			this.tabLog.Controls.Add(this.rtblLog);
			this.tabLog.ImageIndex = 6;
			this.tabLog.Location = new System.Drawing.Point(4, 23);
			this.tabLog.Name = "tabLog";
			this.tabLog.Size = new System.Drawing.Size(504, 229);
			this.tabLog.TabIndex = 2;
			this.tabLog.Text = "Plugin Log";
			// 
			// rtblLog
			// 
			this.rtblLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rtblLog.EventSourceName = "";
			this.rtblLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.rtblLog.Location = new System.Drawing.Point(8, 8);
			this.rtblLog.MaxLength = 50000;
			this.rtblLog.Name = "rtblLog";
			this.rtblLog.ReadOnly = true;
			this.rtblLog.RememberLastEntry = false;
			this.rtblLog.Size = new System.Drawing.Size(488, 216);
			this.rtblLog.TabIndex = 0;
			this.rtblLog.Text = "";
			this.rtblLog.UseColors = true;
			// 
			// tabAbout
			// 
			this.tabAbout.Controls.Add(this.lblVersion);
			this.tabAbout.Controls.Add(this.lblCopyright);
			this.tabAbout.Controls.Add(this.picAbout);
			this.tabAbout.ImageIndex = 7;
			this.tabAbout.Location = new System.Drawing.Point(4, 23);
			this.tabAbout.Name = "tabAbout";
			this.tabAbout.Size = new System.Drawing.Size(504, 229);
			this.tabAbout.TabIndex = 3;
			this.tabAbout.Text = "About...";
			// 
			// lblVersion
			// 
			this.lblVersion.BackColor = System.Drawing.Color.White;
			this.lblVersion.Location = new System.Drawing.Point(160, 136);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(280, 23);
			this.lblVersion.TabIndex = 2;
			this.lblVersion.Text = "Version:";
			// 
			// lblCopyright
			// 
			this.lblCopyright.BackColor = System.Drawing.Color.White;
			this.lblCopyright.Location = new System.Drawing.Point(160, 104);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(288, 23);
			this.lblCopyright.TabIndex = 1;
			this.lblCopyright.Text = "Copyright © Kostas Stroggylos, A.U.E.B. MScIS 2003-4";
			this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// picAbout
			// 
			this.picAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.picAbout.BackColor = System.Drawing.Color.White;
			this.picAbout.Image = ((System.Drawing.Image)(resources.GetObject("picAbout.Image")));
			this.picAbout.Location = new System.Drawing.Point(8, 8);
			this.picAbout.Name = "picAbout";
			this.picAbout.Size = new System.Drawing.Size(488, 216);
			this.picAbout.TabIndex = 0;
			this.picAbout.TabStop = false;
			// 
			// nfiTrayIcon
			// 
			this.nfiTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("nfiTrayIcon.Icon")));
			this.nfiTrayIcon.Text = "CrawlWave ServerWorker";
			this.nfiTrayIcon.DoubleClick += new System.EventHandler(this.nfiTrayIcon_DoubleClick);
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(528, 293);
			this.Controls.Add(this.sbStatus);
			this.Controls.Add(this.tabTabPages);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(496, 320);
			this.Name = "frmMain";
			this.Text = "CrawlWave Server Worker";
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMain_Closing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.sbPanelPluginName)).EndInit();
			this.tabTabPages.ResumeLayout(false);
			this.tabStatus.ResumeLayout(false);
			this.tabSettings.ResumeLayout(false);
			this.tabLog.ResumeLayout(false);
			this.tabAbout.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The application's entry point
		/// </summary>
		/// <param name="args">The arguments passed to the application.</param>
		[STAThread]
		public static void Main(string []args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			Application.Run(new frmMain());
		}

		#region Event Handlers

		private void frmMain_Load(object sender, System.EventArgs e)
		{
			if(settings.LoadPluginsAtStartup)
			{
				LoadPlugins();
			}
			DisplaySettings();
		}

		private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(settings.MinimizeOnExit)
			{
				this.WindowState = FormWindowState.Minimized;
				e.Cancel = true;
			}
			else
			{
				StopPlugins();
				Application.Exit();
			}
		}

		private void frmMain_Resize(object sender, System.EventArgs e)
		{
			if(WindowState == FormWindowState.Minimized)
			{
				if(settings.MinimizeToTray)
				{
					this.Hide();
					tmrTimer.Enabled = false;
					nfiTrayIcon.Visible = true;
				}
			}
			else
			{
				if(settings.MinimizeToTray)
				{
					if(nfiTrayIcon.Visible)
					{
						this.Show();
						tmrTimer.Enabled = true;
						nfiTrayIcon.Visible = false;
						this.Refresh();
					}
				}
			}
		}

		private void tabTabPages_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(tabTabPages.SelectedTab == tabLog)
			{
				try
				{
					rtblLog.SelectionStart = rtblLog.TextLength -1;
					rtblLog.Focus();
				}
				catch
				{}
			}
		}

		private void nfiTrayIcon_DoubleClick(object sender, System.EventArgs e)
		{
			this.Show();
			tmrTimer.Enabled = true;
			this.WindowState = FormWindowState.Normal;
			nfiTrayIcon.Visible = false;
		}
		
		private void mnuPluginStart_Click(object sender, System.EventArgs e)
		{
			try
			{
				((IPlugin)controller.Plugins[lstPlugins.SelectedIndices[0]]).Start();
			}
			catch(Exception ex)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to start " + lstPlugins.SelectedItems[0].SubItems[0].Text + ": " + ex.ToString());
				}
				MessageBox.Show("An error occured while attempting to start the plugin: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void mnuPluginPause_Click(object sender, System.EventArgs e)
		{
			try
			{
				((IPlugin)controller.Plugins[lstPlugins.SelectedIndices[0]]).Pause();
			}	
			catch(Exception ex)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to stop " + lstPlugins.SelectedItems[0].SubItems[0].Text + ": " + ex.ToString());
				}
				MessageBox.Show("An error occured while attempting to pause the plugin: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void mnuPluginStop_Click(object sender, System.EventArgs e)
		{
			try
			{
				((IPlugin)controller.Plugins[lstPlugins.SelectedIndices[0]]).Stop();
			}
			catch(Exception ex)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to stop " + lstPlugins.SelectedItems[0].SubItems[0].Text + ": " + ex.ToString());
				}
				MessageBox.Show("An error occured while attempting to stop the plugin: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void mnuPluginResume_Click(object sender, System.EventArgs e)
		{
			try
			{
				((IPlugin)controller.Plugins[lstPlugins.SelectedIndices[0]]).Resume();
			}
			catch(Exception ex)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to resume " + lstPlugins.SelectedItems[0].SubItems[0].Text + ": " + ex.ToString());
				}
				MessageBox.Show("An error occured while attempting to resume the plugin: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void mnuPluginSettings_Click(object sender, System.EventArgs e)
		{
			try
			{
				((IPlugin)controller.Plugins[lstPlugins.SelectedIndices[0]]).ShowSettings();
			}
			catch(Exception ex)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to display the settings for the " + lstPlugins.SelectedItems[0].SubItems[0].Text + ": " + ex.ToString());
				}
				MessageBox.Show("An error occured while attempting to display the plugin's settings: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void lstPlugins_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button==MouseButtons.Right)
			{
				if((lstPlugins.Items.Count>0)&&(lstPlugins.SelectedItems.Count>0))
				{
					mnuPlugin.Show(lstPlugins, new Point(e.X, e.Y));
				}
			}
		}

		private void lstPlugins_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(lstPlugins.SelectedItems.Count>0)
			{
				sbPanelPluginName.Text = ((PluginBase)controller.Plugins[lstPlugins.SelectedIndices[0]]).Description;
				cmdRemovePlugin.Enabled = true;
			}
			else
			{
				sbPanelPluginName.Text = String.Empty;
				cmdRemovePlugin.Enabled = false;
			}
		}

		private void cmdLoadPlugin_Click(object sender, System.EventArgs e)
		{
			try
			{
				dlgBrowse.ShowDialog();
				if(dlgBrowse.FileName!="")
				{
					string pluginPath = dlgBrowse.FileName;
					controller.LoadPlugin(pluginPath);
					pluginPaths.Add(pluginPath);
					DisplayPlugins();
					dlgBrowse.FileName = String.Empty;
				}
			}
			catch(CWUnsupportedPluginException upe)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to load plugin: " + upe.ToString());
				}
				MessageBox.Show(upe.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			catch(Exception ex)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to load plugin: " + ex.ToString());
				}
				MessageBox.Show("An error occured while loading the plugin: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void cmdRemovePlugin_Click(object sender, System.EventArgs e)
		{
			int index = -1;
			if((lstPlugins.Items.Count>0)&&(lstPlugins.SelectedItems.Count>0))
			{
				index = lstPlugins.SelectedIndices[0];
			}
			else
			{
				MessageBox.Show("You have to select a Plugin to remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			try
			{
				((IPlugin)controller.Plugins[index]).Stop();
				controller.Plugins.RemoveAt(index);
				pluginPaths.RemoveAt(index);
				lstPlugins.Items.RemoveAt(index);
			}
			catch(Exception ex)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to remove the " + lstPlugins.Items[index].SubItems[0].Text + ": " + ex.ToString());
				}
				MessageBox.Show("An error occured while removing the plugin: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void cmdExit_Click(object sender, System.EventArgs e)
		{
			if(controller!=null)
			{
				controller.StopAllPlugins();
				Application.Exit();
			}
		}

		private void cmdLoadDefaults_Click(object sender, System.EventArgs e)
		{
			LoadDefaults();
		}

		private void cmdSaveSettings_Click(object sender, System.EventArgs e)
		{
			SaveSettings();
		}

		private void controller_PluginStateChanged(object sender, EventArgs e)
		{
			DisplayPlugins();
		}

		private void tmrTimer_Tick(object sender, System.EventArgs e)
		{
			memUsage = (int)Process.GetCurrentProcess().PrivateMemorySize64/1024;
			if(memUsage>hbgMemory.Maximum)
			{
				hbgMemory.Maximum = memUsage + 1000;
			}
			hbgMemory.Value = memUsage;
			hlgMemory.NextValue = memUsage;
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show("An unhandled Exception occured: " + ((Exception)e.ExceptionObject).Message, "CrawlWave Server Worker", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			MessageBox.Show("An unhandled Application Thread Exception occured: " + ((Exception)e.Exception).Message, "CrawlWave Server Worker", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		#endregion

		#region Private Methods

		private void DisplayPlugins()
		{
			lstPlugins.Items.Clear();
			int i = 0, index = 0;
			foreach(PluginBase plugin in controller.Plugins)
			{
				switch(plugin.State)
				{
					case PluginState.Running:
						index = 0;
						break;

					case PluginState.Paused:
						index = 1;
						break;

					default:
						index = 2;
						break;
				}
				lstPlugins.Items.Add(plugin.Name, index);
				lstPlugins.Items[i].SubItems.Add(plugin.State.ToString());
				lstPlugins.Items[i++].SubItems.Add(plugin.Version);
			}
		}

		private void LoadPlugins()
		{
			try
			{
				if(settings.PluginList.Count > 0)
				{
					foreach(string pluginPath in settings.PluginList)
					{
						try
						{
							controller.LoadPlugin(pluginPath);
							pluginPaths.Add(pluginPath);
						}
						catch(Exception ex)
						{
							if(settings.Log!=null)
							{
								settings.Log.LogWarning("ServerWorker failed to load a plugin from " + pluginPath + ": " + ex.ToString());
							}
							MessageBox.Show("An error occured while loading the plugin from " + pluginPath +": " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							continue;
						}
					}
					DisplayPlugins();
				}
			}
			catch(Exception e)
			{
				if(settings.Log!=null)
				{
					settings.Log.LogWarning("ServerWorker failed to load the plugins: " + e.ToString());
				}
				MessageBox.Show("An error occured while loading the plugins: " + e.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				GC.Collect();
			}
		}

		private void DisplaySettings()
		{
			chkLoadPlugins.Checked = settings.LoadPluginsAtStartup;
			chkMinimizeToTray.Checked = settings.MinimizeToTray;
			chkMinimizeOnExit.Checked = settings.MinimizeOnExit;
		}

		private void LoadDefaults()
		{
			settings.LoadPluginsAtStartup = true;
			settings.MinimizeOnExit = false;
			settings.MinimizeToTray = false;
			DisplaySettings();
		}

		private void SaveSettings()
		{
			settings.LoadPluginsAtStartup = chkLoadPlugins.Checked;
			settings.MinimizeToTray = chkMinimizeToTray.Checked;
			settings.MinimizeOnExit = chkMinimizeOnExit.Checked;
			settings.PluginList = pluginPaths;
			settings.SaveSettings();
		}

		private void StopPlugins()
		{
			controller.StopAllPlugins();
		}

		#endregion

	}
}
