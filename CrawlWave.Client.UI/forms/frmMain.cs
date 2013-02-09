using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using CrawlWave.Client.Common;
using CrawlWave.Common;
using CrawlWave.Common.UI;

namespace CrawlWave.Client.UI.Forms
{
	/// <summary>
	/// frmMain is the class that provides the User Interface to CrawlWave Client.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		#region UI components

		private System.Windows.Forms.TabPage tabStatus;
		private System.Windows.Forms.TabPage tabSettings;
		private System.Windows.Forms.TabPage tabUserInfo;
		private System.Windows.Forms.GroupBox grpStatus;
		private System.Windows.Forms.GroupBox grpStatitics;
		private CrawlWave.Common.UI.HealthBarGraph hlbSpeed;
		private CrawlWave.Common.UI.HealthBarGraph hlbMemory;
		private CrawlWave.Common.UI.HealthLineGraph hlgSpeed;
		private CrawlWave.Common.UI.HealthLineGraph hlgMemory;
		private CrawlWave.Common.UI.RichTextboxLogger rtblClientLog;
		private System.Windows.Forms.Label lblMemoryHistory;
		private System.Windows.Forms.Label lblSpeedHistory;
		private System.Windows.Forms.Label lblUSRegistrationDate;
		private System.Windows.Forms.Label lblUSNumClients;
		private System.Windows.Forms.Label lblUSUrlStats;
		private System.Windows.Forms.Label lblUSLastActive;
		private System.Windows.Forms.Label lblUSRegistrationDateVal;
		private System.Windows.Forms.Label lblUSNumClientsVal;
		private System.Windows.Forms.Label lblUSUrlStatsVal;
		private System.Windows.Forms.Label lblUSLastActiveVal;
		private System.Windows.Forms.Label lblProjectUrl;
		private System.Windows.Forms.Label lblSpeed;
		private System.Windows.Forms.Label lblMemory;
		private System.Windows.Forms.Button cmdStop;
		private System.Windows.Forms.Button cmdStart;
		private System.Windows.Forms.Button cmdPause;
		private System.Windows.Forms.Button cmdResume;
		private System.Windows.Forms.Button cmdTerminate;
		private System.Windows.Forms.NotifyIcon nfiTrayIcon;
		private System.Windows.Forms.ImageList imgIcons;
		private System.Windows.Forms.Timer tmrTimer;
		private System.Windows.Forms.Timer tmrInit;
		private System.Windows.Forms.Label lblCrawledOtherErrorsVal;
		private System.Windows.Forms.Label lblCrawledOtherErrors;
		private System.Windows.Forms.Label lblCrawledServerErrorVal;
		private System.Windows.Forms.Label lblCrawledServerError;
		private System.Windows.Forms.Label lblCrawledForbiddenVal;
		private System.Windows.Forms.Label lblCrawledForbidden;
		private System.Windows.Forms.Label lblCrawledUnauthorizedVal;
		private System.Windows.Forms.Label lblCrawledUnauthorized;
		private System.Windows.Forms.Label lblCrawledNotFoundVal;
		private System.Windows.Forms.Label lblCrawledOKVal;
		private System.Windows.Forms.Label lblCrawledVal;
		private System.Windows.Forms.Label lblCrawledTimeoutVal;
		private System.Windows.Forms.Label lblCrawledTimeout;
		private System.Windows.Forms.Label lblCrawledNotFound;
		private System.Windows.Forms.Label lblCrawledOK;
		private System.Windows.Forms.Label lblCrawled;
		private System.Windows.Forms.GroupBox grpSettingsScheduler;
		private System.Windows.Forms.GroupBox grpSettingsGeneral;
		private System.Windows.Forms.GroupBox grpSettingsNetSpeed;
		private System.Windows.Forms.CheckBox chkMinimizeToSystemTray;
		private System.Windows.Forms.CheckBox chkLoadAtStartup;
		private System.Windows.Forms.Label lblStopTime;
		private System.Windows.Forms.Label lblStartTime;
		private System.Windows.Forms.DateTimePicker dtStopTime;
		private System.Windows.Forms.DateTimePicker dtStartTime;
		private System.Windows.Forms.CheckBox chkEnableScheduler;
		private System.Windows.Forms.CheckBox chkMinimizeOnExit;
		private System.Windows.Forms.Button cmdDetectNetSpeed;
		private System.Windows.Forms.ComboBox cmbNetSpeed;
		private System.Windows.Forms.Label lblSelectConnectionSpeed;
		private System.Windows.Forms.Button cmdUndoChanges;
		private System.Windows.Forms.Button cmdSaveSettings;
		private System.Windows.Forms.GroupBox grpUserStatistics;
		private System.Windows.Forms.Button cmdRefreshStatistics;
		private System.Windows.Forms.GroupBox grpUserInformation;
		private System.Windows.Forms.Button cmdClear;
		private System.Windows.Forms.Button cmdRegister;
		private System.Windows.Forms.Label lblEmail;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.TabControl tabTabPages;
		private System.Windows.Forms.TextBox txtEmail;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TabPage tabCrawlerLog;
		private System.Windows.Forms.Button cmdClearLog;
		private System.Windows.Forms.CheckBox chkEnableLog;
		private System.Windows.Forms.TabPage tabAbout;
		private System.Windows.Forms.PictureBox picAbout;
		private System.Windows.Forms.GroupBox grpCrawlerLog;
		private System.Windows.Forms.ContextMenu mnuTray;
		private System.Windows.Forms.MenuItem mnuTrayExit;
		private System.Windows.Forms.MenuItem mnuTrayShow;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblProject;
		private System.ComponentModel.IContainer components;
		
		#endregion

		#region Private variables
		
		private int speed;
		private int memory;
		private long totalbytes;
		private long [] stats;
		private ICrawlerController core;
		private TcpChannel channel;
		private ClientSettings settings;
		private CrawlerState state;

		#endregion

		#region Constructor and Dispose Methods

		/// <summary>
		/// Constructs a new instance of the <see cref="frmMain"/> class.
		/// </summary>
		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			speed = 0;
			memory = 0;
			totalbytes = 0;
			
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
			this.tabTabPages = new System.Windows.Forms.TabControl();
			this.tabStatus = new System.Windows.Forms.TabPage();
			this.cmdTerminate = new System.Windows.Forms.Button();
			this.cmdStop = new System.Windows.Forms.Button();
			this.grpStatus = new System.Windows.Forms.GroupBox();
			this.hlgMemory = new CrawlWave.Common.UI.HealthLineGraph();
			this.hlgSpeed = new CrawlWave.Common.UI.HealthLineGraph();
			this.hlbMemory = new CrawlWave.Common.UI.HealthBarGraph();
			this.hlbSpeed = new CrawlWave.Common.UI.HealthBarGraph();
			this.grpStatitics = new System.Windows.Forms.GroupBox();
			this.lblCrawled = new System.Windows.Forms.Label();
			this.lblCrawledOtherErrorsVal = new System.Windows.Forms.Label();
			this.lblCrawledOtherErrors = new System.Windows.Forms.Label();
			this.lblCrawledServerErrorVal = new System.Windows.Forms.Label();
			this.lblCrawledServerError = new System.Windows.Forms.Label();
			this.lblCrawledForbiddenVal = new System.Windows.Forms.Label();
			this.lblCrawledForbidden = new System.Windows.Forms.Label();
			this.lblCrawledUnauthorizedVal = new System.Windows.Forms.Label();
			this.lblCrawledUnauthorized = new System.Windows.Forms.Label();
			this.lblCrawledNotFoundVal = new System.Windows.Forms.Label();
			this.lblCrawledOKVal = new System.Windows.Forms.Label();
			this.lblCrawledVal = new System.Windows.Forms.Label();
			this.lblCrawledTimeoutVal = new System.Windows.Forms.Label();
			this.lblCrawledTimeout = new System.Windows.Forms.Label();
			this.lblCrawledNotFound = new System.Windows.Forms.Label();
			this.lblCrawledOK = new System.Windows.Forms.Label();
			this.lblMemoryHistory = new System.Windows.Forms.Label();
			this.lblSpeedHistory = new System.Windows.Forms.Label();
			this.lblSpeed = new System.Windows.Forms.Label();
			this.lblMemory = new System.Windows.Forms.Label();
			this.cmdStart = new System.Windows.Forms.Button();
			this.cmdPause = new System.Windows.Forms.Button();
			this.cmdResume = new System.Windows.Forms.Button();
			this.tabCrawlerLog = new System.Windows.Forms.TabPage();
			this.chkEnableLog = new System.Windows.Forms.CheckBox();
			this.cmdClearLog = new System.Windows.Forms.Button();
			this.grpCrawlerLog = new System.Windows.Forms.GroupBox();
			this.rtblClientLog = new CrawlWave.Common.UI.RichTextboxLogger();
			this.tabUserInfo = new System.Windows.Forms.TabPage();
			this.grpUserStatistics = new System.Windows.Forms.GroupBox();
			this.lblUSLastActiveVal = new System.Windows.Forms.Label();
			this.lblUSUrlStatsVal = new System.Windows.Forms.Label();
			this.lblUSNumClientsVal = new System.Windows.Forms.Label();
			this.lblUSRegistrationDateVal = new System.Windows.Forms.Label();
			this.lblUSLastActive = new System.Windows.Forms.Label();
			this.lblUSUrlStats = new System.Windows.Forms.Label();
			this.lblUSNumClients = new System.Windows.Forms.Label();
			this.lblUSRegistrationDate = new System.Windows.Forms.Label();
			this.cmdRefreshStatistics = new System.Windows.Forms.Button();
			this.grpUserInformation = new System.Windows.Forms.GroupBox();
			this.cmdClear = new System.Windows.Forms.Button();
			this.cmdRegister = new System.Windows.Forms.Button();
			this.lblEmail = new System.Windows.Forms.Label();
			this.txtEmail = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.lblUsername = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.tabSettings = new System.Windows.Forms.TabPage();
			this.cmdUndoChanges = new System.Windows.Forms.Button();
			this.cmdSaveSettings = new System.Windows.Forms.Button();
			this.grpSettingsScheduler = new System.Windows.Forms.GroupBox();
			this.lblStopTime = new System.Windows.Forms.Label();
			this.lblStartTime = new System.Windows.Forms.Label();
			this.dtStopTime = new System.Windows.Forms.DateTimePicker();
			this.dtStartTime = new System.Windows.Forms.DateTimePicker();
			this.chkEnableScheduler = new System.Windows.Forms.CheckBox();
			this.grpSettingsGeneral = new System.Windows.Forms.GroupBox();
			this.chkMinimizeOnExit = new System.Windows.Forms.CheckBox();
			this.chkMinimizeToSystemTray = new System.Windows.Forms.CheckBox();
			this.chkLoadAtStartup = new System.Windows.Forms.CheckBox();
			this.grpSettingsNetSpeed = new System.Windows.Forms.GroupBox();
			this.lblSelectConnectionSpeed = new System.Windows.Forms.Label();
			this.cmdDetectNetSpeed = new System.Windows.Forms.Button();
			this.cmbNetSpeed = new System.Windows.Forms.ComboBox();
			this.tabAbout = new System.Windows.Forms.TabPage();
			this.lblProjectUrl = new System.Windows.Forms.Label();
			this.lblProject = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.picAbout = new System.Windows.Forms.PictureBox();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.nfiTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.mnuTray = new System.Windows.Forms.ContextMenu();
			this.mnuTrayShow = new System.Windows.Forms.MenuItem();
			this.mnuTrayExit = new System.Windows.Forms.MenuItem();
			this.tmrTimer = new System.Windows.Forms.Timer(this.components);
			this.tmrInit = new System.Windows.Forms.Timer(this.components);
			this.tabTabPages.SuspendLayout();
			this.tabStatus.SuspendLayout();
			this.grpStatus.SuspendLayout();
			this.grpStatitics.SuspendLayout();
			this.tabCrawlerLog.SuspendLayout();
			this.grpCrawlerLog.SuspendLayout();
			this.tabUserInfo.SuspendLayout();
			this.grpUserStatistics.SuspendLayout();
			this.grpUserInformation.SuspendLayout();
			this.tabSettings.SuspendLayout();
			this.grpSettingsScheduler.SuspendLayout();
			this.grpSettingsGeneral.SuspendLayout();
			this.grpSettingsNetSpeed.SuspendLayout();
			this.tabAbout.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabTabPages
			// 
			this.tabTabPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabTabPages.Controls.Add(this.tabStatus);
			this.tabTabPages.Controls.Add(this.tabUserInfo);
			this.tabTabPages.Controls.Add(this.tabSettings);
			this.tabTabPages.Controls.Add(this.tabCrawlerLog);
			this.tabTabPages.Controls.Add(this.tabAbout);
			this.tabTabPages.ImageList = this.imgIcons;
			this.tabTabPages.Location = new System.Drawing.Point(8, 8);
			this.tabTabPages.Name = "tabTabPages";
			this.tabTabPages.SelectedIndex = 0;
			this.tabTabPages.Size = new System.Drawing.Size(512, 304);
			this.tabTabPages.TabIndex = 0;
			this.tabTabPages.SelectedIndexChanged += new System.EventHandler(this.tabTabPages_SelectedIndexChanged);
			// 
			// tabStatus
			// 
			this.tabStatus.Controls.Add(this.cmdTerminate);
			this.tabStatus.Controls.Add(this.cmdStop);
			this.tabStatus.Controls.Add(this.grpStatus);
			this.tabStatus.Controls.Add(this.cmdStart);
			this.tabStatus.Controls.Add(this.cmdPause);
			this.tabStatus.Controls.Add(this.cmdResume);
			this.tabStatus.ImageIndex = 0;
			this.tabStatus.Location = new System.Drawing.Point(4, 23);
			this.tabStatus.Name = "tabStatus";
			this.tabStatus.Size = new System.Drawing.Size(504, 277);
			this.tabStatus.TabIndex = 0;
			this.tabStatus.Text = "Current Status";
			// 
			// cmdTerminate
			// 
			this.cmdTerminate.Location = new System.Drawing.Point(360, 248);
			this.cmdTerminate.Name = "cmdTerminate";
			this.cmdTerminate.Size = new System.Drawing.Size(80, 24);
			this.cmdTerminate.TabIndex = 5;
			this.cmdTerminate.Text = "Terminate";
			this.cmdTerminate.Click += new System.EventHandler(this.cmdTerminate_Click);
			// 
			// cmdStop
			// 
			this.cmdStop.Location = new System.Drawing.Point(272, 248);
			this.cmdStop.Name = "cmdStop";
			this.cmdStop.Size = new System.Drawing.Size(80, 24);
			this.cmdStop.TabIndex = 4;
			this.cmdStop.Text = "Stop";
			this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
			// 
			// grpStatus
			// 
			this.grpStatus.Controls.Add(this.hlgMemory);
			this.grpStatus.Controls.Add(this.hlgSpeed);
			this.grpStatus.Controls.Add(this.hlbMemory);
			this.grpStatus.Controls.Add(this.hlbSpeed);
			this.grpStatus.Controls.Add(this.grpStatitics);
			this.grpStatus.Controls.Add(this.lblMemoryHistory);
			this.grpStatus.Controls.Add(this.lblSpeedHistory);
			this.grpStatus.Controls.Add(this.lblSpeed);
			this.grpStatus.Controls.Add(this.lblMemory);
			this.grpStatus.Location = new System.Drawing.Point(8, 8);
			this.grpStatus.Name = "grpStatus";
			this.grpStatus.Size = new System.Drawing.Size(488, 232);
			this.grpStatus.TabIndex = 0;
			this.grpStatus.TabStop = false;
			this.grpStatus.Text = "Crawler Status";
			// 
			// hlgMemory
			// 
			this.hlgMemory.BackColor = System.Drawing.Color.Black;
			this.hlgMemory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.hlgMemory.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.hlgMemory.GridColor = System.Drawing.Color.MediumBlue;
			this.hlgMemory.Location = new System.Drawing.Point(96, 120);
			this.hlgMemory.Maximum = 100000;
			this.hlgMemory.Minimum = 0;
			this.hlgMemory.Name = "hlgMemory";
			this.hlgMemory.Size = new System.Drawing.Size(200, 80);
			this.hlgMemory.TabIndex = 5;
			// 
			// hlgSpeed
			// 
			this.hlgSpeed.BackColor = System.Drawing.Color.Black;
			this.hlgSpeed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.hlgSpeed.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.hlgSpeed.GridColor = System.Drawing.Color.MediumBlue;
			this.hlgSpeed.Location = new System.Drawing.Point(96, 16);
			this.hlgSpeed.Maximum = 100;
			this.hlgSpeed.Minimum = 0;
			this.hlgSpeed.Name = "hlgSpeed";
			this.hlgSpeed.Size = new System.Drawing.Size(200, 80);
			this.hlgSpeed.TabIndex = 1;
			// 
			// hlbMemory
			// 
			this.hlbMemory.Label = " KB";
			this.hlbMemory.Location = new System.Drawing.Point(8, 120);
			this.hlbMemory.Maximum = 100000;
			this.hlbMemory.Minimum = 0;
			this.hlbMemory.Name = "hlbMemory";
			this.hlbMemory.Size = new System.Drawing.Size(80, 80);
			this.hlbMemory.TabIndex = 4;
			this.hlbMemory.Value = 0;
			// 
			// hlbSpeed
			// 
			this.hlbSpeed.Label = " KB/s";
			this.hlbSpeed.Location = new System.Drawing.Point(8, 16);
			this.hlbSpeed.Maximum = 100;
			this.hlbSpeed.Minimum = 0;
			this.hlbSpeed.Name = "hlbSpeed";
			this.hlbSpeed.Size = new System.Drawing.Size(80, 80);
			this.hlbSpeed.TabIndex = 0;
			this.hlbSpeed.Value = 0;
			// 
			// grpStatitics
			// 
			this.grpStatitics.Controls.Add(this.lblCrawled);
			this.grpStatitics.Controls.Add(this.lblCrawledOtherErrorsVal);
			this.grpStatitics.Controls.Add(this.lblCrawledOtherErrors);
			this.grpStatitics.Controls.Add(this.lblCrawledServerErrorVal);
			this.grpStatitics.Controls.Add(this.lblCrawledServerError);
			this.grpStatitics.Controls.Add(this.lblCrawledForbiddenVal);
			this.grpStatitics.Controls.Add(this.lblCrawledForbidden);
			this.grpStatitics.Controls.Add(this.lblCrawledUnauthorizedVal);
			this.grpStatitics.Controls.Add(this.lblCrawledUnauthorized);
			this.grpStatitics.Controls.Add(this.lblCrawledNotFoundVal);
			this.grpStatitics.Controls.Add(this.lblCrawledOKVal);
			this.grpStatitics.Controls.Add(this.lblCrawledVal);
			this.grpStatitics.Controls.Add(this.lblCrawledTimeoutVal);
			this.grpStatitics.Controls.Add(this.lblCrawledTimeout);
			this.grpStatitics.Controls.Add(this.lblCrawledNotFound);
			this.grpStatitics.Controls.Add(this.lblCrawledOK);
			this.grpStatitics.Location = new System.Drawing.Point(304, 8);
			this.grpStatitics.Name = "grpStatitics";
			this.grpStatitics.Size = new System.Drawing.Size(176, 216);
			this.grpStatitics.TabIndex = 8;
			this.grpStatitics.TabStop = false;
			this.grpStatitics.Text = "Statistics";
			// 
			// lblCrawled
			// 
			this.lblCrawled.Location = new System.Drawing.Point(8, 16);
			this.lblCrawled.Name = "lblCrawled";
			this.lblCrawled.Size = new System.Drawing.Size(88, 23);
			this.lblCrawled.TabIndex = 0;
			this.lblCrawled.Text = "Crawled Urls";
			this.lblCrawled.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCrawledOtherErrorsVal
			// 
			this.lblCrawledOtherErrorsVal.Location = new System.Drawing.Point(96, 184);
			this.lblCrawledOtherErrorsVal.Name = "lblCrawledOtherErrorsVal";
			this.lblCrawledOtherErrorsVal.Size = new System.Drawing.Size(72, 23);
			this.lblCrawledOtherErrorsVal.TabIndex = 15;
			this.lblCrawledOtherErrorsVal.Text = "0";
			this.lblCrawledOtherErrorsVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCrawledOtherErrors
			// 
			this.lblCrawledOtherErrors.Location = new System.Drawing.Point(8, 184);
			this.lblCrawledOtherErrors.Name = "lblCrawledOtherErrors";
			this.lblCrawledOtherErrors.Size = new System.Drawing.Size(88, 23);
			this.lblCrawledOtherErrors.TabIndex = 14;
			this.lblCrawledOtherErrors.Text = "Other errors";
			this.lblCrawledOtherErrors.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCrawledServerErrorVal
			// 
			this.lblCrawledServerErrorVal.Location = new System.Drawing.Point(96, 160);
			this.lblCrawledServerErrorVal.Name = "lblCrawledServerErrorVal";
			this.lblCrawledServerErrorVal.Size = new System.Drawing.Size(72, 24);
			this.lblCrawledServerErrorVal.TabIndex = 13;
			this.lblCrawledServerErrorVal.Text = "0";
			this.lblCrawledServerErrorVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCrawledServerError
			// 
			this.lblCrawledServerError.Location = new System.Drawing.Point(8, 160);
			this.lblCrawledServerError.Name = "lblCrawledServerError";
			this.lblCrawledServerError.Size = new System.Drawing.Size(88, 23);
			this.lblCrawledServerError.TabIndex = 12;
			this.lblCrawledServerError.Text = "Server Error";
			this.lblCrawledServerError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCrawledForbiddenVal
			// 
			this.lblCrawledForbiddenVal.Location = new System.Drawing.Point(96, 112);
			this.lblCrawledForbiddenVal.Name = "lblCrawledForbiddenVal";
			this.lblCrawledForbiddenVal.Size = new System.Drawing.Size(72, 23);
			this.lblCrawledForbiddenVal.TabIndex = 9;
			this.lblCrawledForbiddenVal.Text = "0";
			this.lblCrawledForbiddenVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCrawledForbidden
			// 
			this.lblCrawledForbidden.Location = new System.Drawing.Point(8, 112);
			this.lblCrawledForbidden.Name = "lblCrawledForbidden";
			this.lblCrawledForbidden.Size = new System.Drawing.Size(88, 23);
			this.lblCrawledForbidden.TabIndex = 8;
			this.lblCrawledForbidden.Text = "Forbidden";
			this.lblCrawledForbidden.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCrawledUnauthorizedVal
			// 
			this.lblCrawledUnauthorizedVal.Location = new System.Drawing.Point(96, 88);
			this.lblCrawledUnauthorizedVal.Name = "lblCrawledUnauthorizedVal";
			this.lblCrawledUnauthorizedVal.Size = new System.Drawing.Size(72, 23);
			this.lblCrawledUnauthorizedVal.TabIndex = 7;
			this.lblCrawledUnauthorizedVal.Text = "0";
			this.lblCrawledUnauthorizedVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCrawledUnauthorized
			// 
			this.lblCrawledUnauthorized.Location = new System.Drawing.Point(8, 88);
			this.lblCrawledUnauthorized.Name = "lblCrawledUnauthorized";
			this.lblCrawledUnauthorized.Size = new System.Drawing.Size(88, 23);
			this.lblCrawledUnauthorized.TabIndex = 6;
			this.lblCrawledUnauthorized.Text = "Unauthorized";
			this.lblCrawledUnauthorized.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCrawledNotFoundVal
			// 
			this.lblCrawledNotFoundVal.Location = new System.Drawing.Point(96, 64);
			this.lblCrawledNotFoundVal.Name = "lblCrawledNotFoundVal";
			this.lblCrawledNotFoundVal.Size = new System.Drawing.Size(72, 23);
			this.lblCrawledNotFoundVal.TabIndex = 5;
			this.lblCrawledNotFoundVal.Text = "0";
			this.lblCrawledNotFoundVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCrawledOKVal
			// 
			this.lblCrawledOKVal.Location = new System.Drawing.Point(96, 40);
			this.lblCrawledOKVal.Name = "lblCrawledOKVal";
			this.lblCrawledOKVal.Size = new System.Drawing.Size(72, 24);
			this.lblCrawledOKVal.TabIndex = 3;
			this.lblCrawledOKVal.Text = "0";
			this.lblCrawledOKVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCrawledVal
			// 
			this.lblCrawledVal.Location = new System.Drawing.Point(96, 16);
			this.lblCrawledVal.Name = "lblCrawledVal";
			this.lblCrawledVal.Size = new System.Drawing.Size(72, 23);
			this.lblCrawledVal.TabIndex = 1;
			this.lblCrawledVal.Text = "0";
			this.lblCrawledVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCrawledTimeoutVal
			// 
			this.lblCrawledTimeoutVal.Location = new System.Drawing.Point(96, 136);
			this.lblCrawledTimeoutVal.Name = "lblCrawledTimeoutVal";
			this.lblCrawledTimeoutVal.Size = new System.Drawing.Size(72, 23);
			this.lblCrawledTimeoutVal.TabIndex = 11;
			this.lblCrawledTimeoutVal.Text = "0";
			this.lblCrawledTimeoutVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCrawledTimeout
			// 
			this.lblCrawledTimeout.Location = new System.Drawing.Point(8, 136);
			this.lblCrawledTimeout.Name = "lblCrawledTimeout";
			this.lblCrawledTimeout.Size = new System.Drawing.Size(88, 23);
			this.lblCrawledTimeout.TabIndex = 10;
			this.lblCrawledTimeout.Text = "Timeout";
			this.lblCrawledTimeout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCrawledNotFound
			// 
			this.lblCrawledNotFound.Location = new System.Drawing.Point(8, 64);
			this.lblCrawledNotFound.Name = "lblCrawledNotFound";
			this.lblCrawledNotFound.Size = new System.Drawing.Size(88, 23);
			this.lblCrawledNotFound.TabIndex = 4;
			this.lblCrawledNotFound.Text = "404 Not Found";
			this.lblCrawledNotFound.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCrawledOK
			// 
			this.lblCrawledOK.Location = new System.Drawing.Point(8, 40);
			this.lblCrawledOK.Name = "lblCrawledOK";
			this.lblCrawledOK.Size = new System.Drawing.Size(88, 23);
			this.lblCrawledOK.TabIndex = 2;
			this.lblCrawledOK.Text = "200 OK";
			this.lblCrawledOK.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblMemoryHistory
			// 
			this.lblMemoryHistory.Location = new System.Drawing.Point(96, 200);
			this.lblMemoryHistory.Name = "lblMemoryHistory";
			this.lblMemoryHistory.Size = new System.Drawing.Size(200, 23);
			this.lblMemoryHistory.TabIndex = 7;
			this.lblMemoryHistory.Text = "Memory Usage History";
			// 
			// lblSpeedHistory
			// 
			this.lblSpeedHistory.Location = new System.Drawing.Point(96, 96);
			this.lblSpeedHistory.Name = "lblSpeedHistory";
			this.lblSpeedHistory.Size = new System.Drawing.Size(200, 23);
			this.lblSpeedHistory.TabIndex = 3;
			this.lblSpeedHistory.Text = "Speed History";
			// 
			// lblSpeed
			// 
			this.lblSpeed.Location = new System.Drawing.Point(8, 96);
			this.lblSpeed.Name = "lblSpeed";
			this.lblSpeed.Size = new System.Drawing.Size(80, 24);
			this.lblSpeed.TabIndex = 2;
			this.lblSpeed.Text = "Speed";
			this.lblSpeed.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// lblMemory
			// 
			this.lblMemory.Location = new System.Drawing.Point(8, 200);
			this.lblMemory.Name = "lblMemory";
			this.lblMemory.Size = new System.Drawing.Size(80, 24);
			this.lblMemory.TabIndex = 6;
			this.lblMemory.Text = "Mem Usage";
			this.lblMemory.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// cmdStart
			// 
			this.cmdStart.Location = new System.Drawing.Point(8, 248);
			this.cmdStart.Name = "cmdStart";
			this.cmdStart.Size = new System.Drawing.Size(80, 24);
			this.cmdStart.TabIndex = 1;
			this.cmdStart.Text = "Start";
			this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
			// 
			// cmdPause
			// 
			this.cmdPause.Location = new System.Drawing.Point(96, 248);
			this.cmdPause.Name = "cmdPause";
			this.cmdPause.Size = new System.Drawing.Size(80, 24);
			this.cmdPause.TabIndex = 2;
			this.cmdPause.Text = "Pause";
			this.cmdPause.Click += new System.EventHandler(this.cmdPause_Click);
			// 
			// cmdResume
			// 
			this.cmdResume.Location = new System.Drawing.Point(184, 248);
			this.cmdResume.Name = "cmdResume";
			this.cmdResume.Size = new System.Drawing.Size(80, 24);
			this.cmdResume.TabIndex = 3;
			this.cmdResume.Text = "Resume";
			this.cmdResume.Click += new System.EventHandler(this.cmdResume_Click);
			// 
			// tabCrawlerLog
			// 
			this.tabCrawlerLog.Controls.Add(this.chkEnableLog);
			this.tabCrawlerLog.Controls.Add(this.cmdClearLog);
			this.tabCrawlerLog.Controls.Add(this.grpCrawlerLog);
			this.tabCrawlerLog.ImageIndex = 3;
			this.tabCrawlerLog.Location = new System.Drawing.Point(4, 23);
			this.tabCrawlerLog.Name = "tabCrawlerLog";
			this.tabCrawlerLog.Size = new System.Drawing.Size(504, 277);
			this.tabCrawlerLog.TabIndex = 3;
			this.tabCrawlerLog.Text = "Crawler Log";
			// 
			// chkEnableLog
			// 
			this.chkEnableLog.Location = new System.Drawing.Point(96, 248);
			this.chkEnableLog.Name = "chkEnableLog";
			this.chkEnableLog.TabIndex = 2;
			this.chkEnableLog.Text = "Enable Log";
			// 
			// cmdClearLog
			// 
			this.cmdClearLog.Location = new System.Drawing.Point(8, 248);
			this.cmdClearLog.Name = "cmdClearLog";
			this.cmdClearLog.Size = new System.Drawing.Size(80, 24);
			this.cmdClearLog.TabIndex = 1;
			this.cmdClearLog.Text = "Clear log";
			this.cmdClearLog.Click += new System.EventHandler(this.cmdClearLog_Click);
			// 
			// grpCrawlerLog
			// 
			this.grpCrawlerLog.Controls.Add(this.rtblClientLog);
			this.grpCrawlerLog.Location = new System.Drawing.Point(8, 8);
			this.grpCrawlerLog.Name = "grpCrawlerLog";
			this.grpCrawlerLog.Size = new System.Drawing.Size(488, 232);
			this.grpCrawlerLog.TabIndex = 0;
			this.grpCrawlerLog.TabStop = false;
			this.grpCrawlerLog.Text = "Last Crawler events";
			// 
			// rtblClientLog
			// 
			this.rtblClientLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rtblClientLog.DetectUrls = false;
			this.rtblClientLog.EventSourceName = "CrawlWave.Client";
			this.rtblClientLog.Location = new System.Drawing.Point(8, 16);
			this.rtblClientLog.MaxLength = 50000;
			this.rtblClientLog.Name = "rtblClientLog";
			this.rtblClientLog.ReadOnly = true;
			this.rtblClientLog.RememberLastEntry = false;
			this.rtblClientLog.Size = new System.Drawing.Size(472, 208);
			this.rtblClientLog.TabIndex = 0;
			this.rtblClientLog.Text = "";
			this.rtblClientLog.UseColors = true;
			// 
			// tabUserInfo
			// 
			this.tabUserInfo.Controls.Add(this.grpUserStatistics);
			this.tabUserInfo.Controls.Add(this.grpUserInformation);
			this.tabUserInfo.ImageIndex = 2;
			this.tabUserInfo.Location = new System.Drawing.Point(4, 23);
			this.tabUserInfo.Name = "tabUserInfo";
			this.tabUserInfo.Size = new System.Drawing.Size(504, 277);
			this.tabUserInfo.TabIndex = 2;
			this.tabUserInfo.Text = "User Information";
			// 
			// grpUserStatistics
			// 
			this.grpUserStatistics.Controls.Add(this.lblUSLastActiveVal);
			this.grpUserStatistics.Controls.Add(this.lblUSUrlStatsVal);
			this.grpUserStatistics.Controls.Add(this.lblUSNumClientsVal);
			this.grpUserStatistics.Controls.Add(this.lblUSRegistrationDateVal);
			this.grpUserStatistics.Controls.Add(this.lblUSLastActive);
			this.grpUserStatistics.Controls.Add(this.lblUSUrlStats);
			this.grpUserStatistics.Controls.Add(this.lblUSNumClients);
			this.grpUserStatistics.Controls.Add(this.lblUSRegistrationDate);
			this.grpUserStatistics.Controls.Add(this.cmdRefreshStatistics);
			this.grpUserStatistics.Location = new System.Drawing.Point(8, 152);
			this.grpUserStatistics.Name = "grpUserStatistics";
			this.grpUserStatistics.Size = new System.Drawing.Size(488, 120);
			this.grpUserStatistics.TabIndex = 1;
			this.grpUserStatistics.TabStop = false;
			this.grpUserStatistics.Text = "User Statistics";
			// 
			// lblUSLastActiveVal
			// 
			this.lblUSLastActiveVal.Location = new System.Drawing.Point(144, 88);
			this.lblUSLastActiveVal.Name = "lblUSLastActiveVal";
			this.lblUSLastActiveVal.Size = new System.Drawing.Size(248, 23);
			this.lblUSLastActiveVal.TabIndex = 8;
			this.lblUSLastActiveVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUSUrlStatsVal
			// 
			this.lblUSUrlStatsVal.Location = new System.Drawing.Point(144, 64);
			this.lblUSUrlStatsVal.Name = "lblUSUrlStatsVal";
			this.lblUSUrlStatsVal.Size = new System.Drawing.Size(248, 23);
			this.lblUSUrlStatsVal.TabIndex = 7;
			this.lblUSUrlStatsVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUSNumClientsVal
			// 
			this.lblUSNumClientsVal.Location = new System.Drawing.Point(144, 40);
			this.lblUSNumClientsVal.Name = "lblUSNumClientsVal";
			this.lblUSNumClientsVal.Size = new System.Drawing.Size(248, 23);
			this.lblUSNumClientsVal.TabIndex = 6;
			this.lblUSNumClientsVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUSRegistrationDateVal
			// 
			this.lblUSRegistrationDateVal.Location = new System.Drawing.Point(144, 16);
			this.lblUSRegistrationDateVal.Name = "lblUSRegistrationDateVal";
			this.lblUSRegistrationDateVal.Size = new System.Drawing.Size(248, 24);
			this.lblUSRegistrationDateVal.TabIndex = 5;
			this.lblUSRegistrationDateVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUSLastActive
			// 
			this.lblUSLastActive.Location = new System.Drawing.Point(8, 88);
			this.lblUSLastActive.Name = "lblUSLastActive";
			this.lblUSLastActive.Size = new System.Drawing.Size(136, 23);
			this.lblUSLastActive.TabIndex = 4;
			this.lblUSLastActive.Text = "Last Activity Date";
			this.lblUSLastActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUSUrlStats
			// 
			this.lblUSUrlStats.Location = new System.Drawing.Point(8, 64);
			this.lblUSUrlStats.Name = "lblUSUrlStats";
			this.lblUSUrlStats.Size = new System.Drawing.Size(136, 23);
			this.lblUSUrlStats.TabIndex = 3;
			this.lblUSUrlStats.Text = "Urls Assigned / Returned";
			this.lblUSUrlStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUSNumClients
			// 
			this.lblUSNumClients.Location = new System.Drawing.Point(8, 40);
			this.lblUSNumClients.Name = "lblUSNumClients";
			this.lblUSNumClients.Size = new System.Drawing.Size(136, 23);
			this.lblUSNumClients.TabIndex = 2;
			this.lblUSNumClients.Text = "Number of Clients";
			this.lblUSNumClients.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUSRegistrationDate
			// 
			this.lblUSRegistrationDate.Location = new System.Drawing.Point(8, 16);
			this.lblUSRegistrationDate.Name = "lblUSRegistrationDate";
			this.lblUSRegistrationDate.Size = new System.Drawing.Size(136, 23);
			this.lblUSRegistrationDate.TabIndex = 1;
			this.lblUSRegistrationDate.Text = "Registration Date";
			this.lblUSRegistrationDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmdRefreshStatistics
			// 
			this.cmdRefreshStatistics.Location = new System.Drawing.Point(400, 88);
			this.cmdRefreshStatistics.Name = "cmdRefreshStatistics";
			this.cmdRefreshStatistics.Size = new System.Drawing.Size(80, 24);
			this.cmdRefreshStatistics.TabIndex = 0;
			this.cmdRefreshStatistics.Text = "Refresh";
			this.cmdRefreshStatistics.Click += new System.EventHandler(this.cmdRefreshStatistics_Click);
			// 
			// grpUserInformation
			// 
			this.grpUserInformation.Controls.Add(this.cmdClear);
			this.grpUserInformation.Controls.Add(this.cmdRegister);
			this.grpUserInformation.Controls.Add(this.lblEmail);
			this.grpUserInformation.Controls.Add(this.txtEmail);
			this.grpUserInformation.Controls.Add(this.lblPassword);
			this.grpUserInformation.Controls.Add(this.lblUsername);
			this.grpUserInformation.Controls.Add(this.txtPassword);
			this.grpUserInformation.Controls.Add(this.txtUsername);
			this.grpUserInformation.Location = new System.Drawing.Point(8, 8);
			this.grpUserInformation.Name = "grpUserInformation";
			this.grpUserInformation.Size = new System.Drawing.Size(488, 136);
			this.grpUserInformation.TabIndex = 0;
			this.grpUserInformation.TabStop = false;
			this.grpUserInformation.Text = "User Information";
			// 
			// cmdClear
			// 
			this.cmdClear.Location = new System.Drawing.Point(96, 104);
			this.cmdClear.Name = "cmdClear";
			this.cmdClear.Size = new System.Drawing.Size(80, 24);
			this.cmdClear.TabIndex = 7;
			this.cmdClear.Text = "Clear";
			this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
			// 
			// cmdRegister
			// 
			this.cmdRegister.Location = new System.Drawing.Point(8, 104);
			this.cmdRegister.Name = "cmdRegister";
			this.cmdRegister.Size = new System.Drawing.Size(80, 24);
			this.cmdRegister.TabIndex = 6;
			this.cmdRegister.Text = "Register";
			this.cmdRegister.Click += new System.EventHandler(this.cmdRegister_Click);
			// 
			// lblEmail
			// 
			this.lblEmail.Location = new System.Drawing.Point(16, 72);
			this.lblEmail.Name = "lblEmail";
			this.lblEmail.Size = new System.Drawing.Size(80, 24);
			this.lblEmail.TabIndex = 4;
			this.lblEmail.Text = "Email Address";
			this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtEmail
			// 
			this.txtEmail.Location = new System.Drawing.Point(96, 72);
			this.txtEmail.MaxLength = 50;
			this.txtEmail.Name = "txtEmail";
			this.txtEmail.Size = new System.Drawing.Size(144, 20);
			this.txtEmail.TabIndex = 5;
			this.txtEmail.Text = "";
			// 
			// lblPassword
			// 
			this.lblPassword.Location = new System.Drawing.Point(16, 48);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(80, 23);
			this.lblPassword.TabIndex = 2;
			this.lblPassword.Text = "Password";
			this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblUsername
			// 
			this.lblUsername.Location = new System.Drawing.Point(16, 24);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(80, 23);
			this.lblUsername.TabIndex = 0;
			this.lblUsername.Text = "Username";
			this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(96, 48);
			this.txtPassword.MaxLength = 50;
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(144, 20);
			this.txtPassword.TabIndex = 3;
			this.txtPassword.Text = "";
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(96, 24);
			this.txtUsername.MaxLength = 50;
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(144, 20);
			this.txtUsername.TabIndex = 1;
			this.txtUsername.Text = "";
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.cmdUndoChanges);
			this.tabSettings.Controls.Add(this.cmdSaveSettings);
			this.tabSettings.Controls.Add(this.grpSettingsScheduler);
			this.tabSettings.Controls.Add(this.grpSettingsGeneral);
			this.tabSettings.Controls.Add(this.grpSettingsNetSpeed);
			this.tabSettings.ImageIndex = 1;
			this.tabSettings.Location = new System.Drawing.Point(4, 23);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.Size = new System.Drawing.Size(504, 277);
			this.tabSettings.TabIndex = 1;
			this.tabSettings.Text = "Client Settings";
			// 
			// cmdUndoChanges
			// 
			this.cmdUndoChanges.Location = new System.Drawing.Point(112, 248);
			this.cmdUndoChanges.Name = "cmdUndoChanges";
			this.cmdUndoChanges.Size = new System.Drawing.Size(96, 24);
			this.cmdUndoChanges.TabIndex = 4;
			this.cmdUndoChanges.Text = "Undo Changes";
			this.cmdUndoChanges.Click += new System.EventHandler(this.cmdUndoChanges_Click);
			// 
			// cmdSaveSettings
			// 
			this.cmdSaveSettings.Location = new System.Drawing.Point(8, 248);
			this.cmdSaveSettings.Name = "cmdSaveSettings";
			this.cmdSaveSettings.Size = new System.Drawing.Size(96, 24);
			this.cmdSaveSettings.TabIndex = 3;
			this.cmdSaveSettings.Text = "Save Settings";
			this.cmdSaveSettings.Click += new System.EventHandler(this.cmdSaveSettings_Click);
			// 
			// grpSettingsScheduler
			// 
			this.grpSettingsScheduler.Controls.Add(this.lblStopTime);
			this.grpSettingsScheduler.Controls.Add(this.lblStartTime);
			this.grpSettingsScheduler.Controls.Add(this.dtStopTime);
			this.grpSettingsScheduler.Controls.Add(this.dtStartTime);
			this.grpSettingsScheduler.Controls.Add(this.chkEnableScheduler);
			this.grpSettingsScheduler.Location = new System.Drawing.Point(240, 144);
			this.grpSettingsScheduler.Name = "grpSettingsScheduler";
			this.grpSettingsScheduler.Size = new System.Drawing.Size(256, 96);
			this.grpSettingsScheduler.TabIndex = 2;
			this.grpSettingsScheduler.TabStop = false;
			this.grpSettingsScheduler.Text = "Schedule";
			// 
			// lblStopTime
			// 
			this.lblStopTime.Location = new System.Drawing.Point(8, 64);
			this.lblStopTime.Name = "lblStopTime";
			this.lblStopTime.Size = new System.Drawing.Size(136, 23);
			this.lblStopTime.TabIndex = 3;
			this.lblStopTime.Text = "Stop Time";
			this.lblStopTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblStartTime
			// 
			this.lblStartTime.Location = new System.Drawing.Point(8, 40);
			this.lblStartTime.Name = "lblStartTime";
			this.lblStartTime.Size = new System.Drawing.Size(136, 23);
			this.lblStartTime.TabIndex = 1;
			this.lblStartTime.Text = "Start Time";
			this.lblStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dtStopTime
			// 
			this.dtStopTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dtStopTime.Location = new System.Drawing.Point(144, 64);
			this.dtStopTime.Name = "dtStopTime";
			this.dtStopTime.Size = new System.Drawing.Size(104, 20);
			this.dtStopTime.TabIndex = 4;
			this.dtStopTime.Value = new System.DateTime(2005, 2, 8, 0, 0, 0, 0);
			// 
			// dtStartTime
			// 
			this.dtStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dtStartTime.Location = new System.Drawing.Point(144, 40);
			this.dtStartTime.Name = "dtStartTime";
			this.dtStartTime.Size = new System.Drawing.Size(104, 20);
			this.dtStartTime.TabIndex = 2;
			this.dtStartTime.Value = new System.DateTime(2005, 2, 8, 0, 0, 0, 0);
			// 
			// chkEnableScheduler
			// 
			this.chkEnableScheduler.Location = new System.Drawing.Point(8, 16);
			this.chkEnableScheduler.Name = "chkEnableScheduler";
			this.chkEnableScheduler.Size = new System.Drawing.Size(240, 24);
			this.chkEnableScheduler.TabIndex = 0;
			this.chkEnableScheduler.Text = "Enable Scheduler";
			this.chkEnableScheduler.CheckedChanged += new System.EventHandler(this.chkEnableScheduler_CheckedChanged);
			// 
			// grpSettingsGeneral
			// 
			this.grpSettingsGeneral.Controls.Add(this.chkMinimizeOnExit);
			this.grpSettingsGeneral.Controls.Add(this.chkMinimizeToSystemTray);
			this.grpSettingsGeneral.Controls.Add(this.chkLoadAtStartup);
			this.grpSettingsGeneral.Location = new System.Drawing.Point(8, 8);
			this.grpSettingsGeneral.Name = "grpSettingsGeneral";
			this.grpSettingsGeneral.Size = new System.Drawing.Size(488, 128);
			this.grpSettingsGeneral.TabIndex = 0;
			this.grpSettingsGeneral.TabStop = false;
			this.grpSettingsGeneral.Text = "Startup and appearance options";
			// 
			// chkMinimizeOnExit
			// 
			this.chkMinimizeOnExit.Location = new System.Drawing.Point(8, 72);
			this.chkMinimizeOnExit.Name = "chkMinimizeOnExit";
			this.chkMinimizeOnExit.Size = new System.Drawing.Size(312, 24);
			this.chkMinimizeOnExit.TabIndex = 2;
			this.chkMinimizeOnExit.Text = "Minimize on Exit";
			// 
			// chkMinimizeToSystemTray
			// 
			this.chkMinimizeToSystemTray.Location = new System.Drawing.Point(8, 48);
			this.chkMinimizeToSystemTray.Name = "chkMinimizeToSystemTray";
			this.chkMinimizeToSystemTray.Size = new System.Drawing.Size(312, 24);
			this.chkMinimizeToSystemTray.TabIndex = 1;
			this.chkMinimizeToSystemTray.Text = "Send to System Tray when minimized";
			// 
			// chkLoadAtStartup
			// 
			this.chkLoadAtStartup.Location = new System.Drawing.Point(8, 24);
			this.chkLoadAtStartup.Name = "chkLoadAtStartup";
			this.chkLoadAtStartup.Size = new System.Drawing.Size(312, 24);
			this.chkLoadAtStartup.TabIndex = 0;
			this.chkLoadAtStartup.Text = "Load automatically at Windows start";
			// 
			// grpSettingsNetSpeed
			// 
			this.grpSettingsNetSpeed.Controls.Add(this.lblSelectConnectionSpeed);
			this.grpSettingsNetSpeed.Controls.Add(this.cmdDetectNetSpeed);
			this.grpSettingsNetSpeed.Controls.Add(this.cmbNetSpeed);
			this.grpSettingsNetSpeed.Location = new System.Drawing.Point(8, 144);
			this.grpSettingsNetSpeed.Name = "grpSettingsNetSpeed";
			this.grpSettingsNetSpeed.Size = new System.Drawing.Size(224, 96);
			this.grpSettingsNetSpeed.TabIndex = 1;
			this.grpSettingsNetSpeed.TabStop = false;
			this.grpSettingsNetSpeed.Text = "Network Settings";
			// 
			// lblSelectConnectionSpeed
			// 
			this.lblSelectConnectionSpeed.Location = new System.Drawing.Point(8, 16);
			this.lblSelectConnectionSpeed.Name = "lblSelectConnectionSpeed";
			this.lblSelectConnectionSpeed.Size = new System.Drawing.Size(208, 24);
			this.lblSelectConnectionSpeed.TabIndex = 0;
			this.lblSelectConnectionSpeed.Text = "Select Internet Connection Speed";
			this.lblSelectConnectionSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmdDetectNetSpeed
			// 
			this.cmdDetectNetSpeed.Location = new System.Drawing.Point(8, 64);
			this.cmdDetectNetSpeed.Name = "cmdDetectNetSpeed";
			this.cmdDetectNetSpeed.Size = new System.Drawing.Size(80, 24);
			this.cmdDetectNetSpeed.TabIndex = 2;
			this.cmdDetectNetSpeed.Text = "Auto Detect";
			this.cmdDetectNetSpeed.Click += new System.EventHandler(this.cmdDetectNetSpeed_Click);
			// 
			// cmbNetSpeed
			// 
			this.cmbNetSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbNetSpeed.Items.AddRange(new object[] {
															 "Unknown",
															 "Dial-Up Modem 56Kbps",
															 "ISDN 64Kbps",
															 "ISDN 128Kbps",
															 "DSL 256Kbps",
															 "DSL 384Kbps",
															 "DSL 512Kbps",
															 "DSL 1Mbps",
															 "T1 / E1",
															 "T3 / E3",
															 "Fiber",
															 "ATM 155Mbps"});
			this.cmbNetSpeed.Location = new System.Drawing.Point(8, 40);
			this.cmbNetSpeed.Name = "cmbNetSpeed";
			this.cmbNetSpeed.Size = new System.Drawing.Size(208, 21);
			this.cmbNetSpeed.TabIndex = 1;
			// 
			// tabAbout
			// 
			this.tabAbout.Controls.Add(this.lblProjectUrl);
			this.tabAbout.Controls.Add(this.lblProject);
			this.tabAbout.Controls.Add(this.lblVersion);
			this.tabAbout.Controls.Add(this.lblInfo);
			this.tabAbout.Controls.Add(this.picAbout);
			this.tabAbout.ImageIndex = 4;
			this.tabAbout.Location = new System.Drawing.Point(4, 23);
			this.tabAbout.Name = "tabAbout";
			this.tabAbout.Size = new System.Drawing.Size(504, 277);
			this.tabAbout.TabIndex = 4;
			this.tabAbout.Text = "About...";
			// 
			// lblProjectUrl
			// 
			this.lblProjectUrl.BackColor = System.Drawing.Color.White;
			this.lblProjectUrl.Location = new System.Drawing.Point(176, 136);
			this.lblProjectUrl.Name = "lblProjectUrl";
			this.lblProjectUrl.Size = new System.Drawing.Size(288, 32);
			this.lblProjectUrl.TabIndex = 3;
			this.lblProjectUrl.Text = "Project info page: http://www.spiderwave.aueb.gr/";
			this.lblProjectUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblProject
			// 
			this.lblProject.BackColor = System.Drawing.Color.White;
			this.lblProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblProject.Location = new System.Drawing.Point(168, 112);
			this.lblProject.Name = "lblProject";
			this.lblProject.Size = new System.Drawing.Size(296, 24);
			this.lblProject.TabIndex = 0;
			this.lblProject.Text = "A.U.E.B. Distributed Web Crawling Project ";
			this.lblProject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblVersion
			// 
			this.lblVersion.BackColor = System.Drawing.Color.White;
			this.lblVersion.Location = new System.Drawing.Point(304, 232);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(160, 23);
			this.lblVersion.TabIndex = 2;
			this.lblVersion.Text = "Version:";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblInfo
			// 
			this.lblInfo.BackColor = System.Drawing.Color.White;
			this.lblInfo.Location = new System.Drawing.Point(40, 184);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(424, 40);
			this.lblInfo.TabIndex = 1;
			this.lblInfo.Text = "Uses SharpZipLib, XPDF Library (http://www.foolabs.com" +
				"/xpdf/), Macromedia\'s Flash Search SDK.";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// picAbout
			// 
			this.picAbout.Image = ((System.Drawing.Image)(resources.GetObject("picAbout.Image")));
			this.picAbout.Location = new System.Drawing.Point(24, 8);
			this.picAbout.Name = "picAbout";
			this.picAbout.Size = new System.Drawing.Size(456, 264);
			this.picAbout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.picAbout.TabIndex = 0;
			this.picAbout.TabStop = false;
			// 
			// imgIcons
			// 
			this.imgIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
			this.imgIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// nfiTrayIcon
			// 
			this.nfiTrayIcon.ContextMenu = this.mnuTray;
			this.nfiTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("nfiTrayIcon.Icon")));
			this.nfiTrayIcon.Text = "CrawlWave Client";
			this.nfiTrayIcon.DoubleClick += new System.EventHandler(this.nfiTrayIcon_DoubleClick);
			// 
			// mnuTray
			// 
			this.mnuTray.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuTrayShow,
																					this.mnuTrayExit});
			// 
			// mnuTrayShow
			// 
			this.mnuTrayShow.Index = 0;
			this.mnuTrayShow.Text = "Show";
			this.mnuTrayShow.Click += new System.EventHandler(this.mnuTrayShow_Click);
			// 
			// mnuTrayExit
			// 
			this.mnuTrayExit.Index = 1;
			this.mnuTrayExit.Text = "Exit";
			this.mnuTrayExit.Click += new System.EventHandler(this.mnuTrayExit_Click);
			// 
			// tmrTimer
			// 
			this.tmrTimer.Interval = 5000;
			this.tmrTimer.Tick += new System.EventHandler(this.tmrTimer_Tick);
			// 
			// tmrInit
			// 
			this.tmrInit.Enabled = true;
			this.tmrInit.Interval = 1000;
			this.tmrInit.Tick += new System.EventHandler(this.tmrInit_Tick);
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(528, 319);
			this.Controls.Add(this.tabTabPages);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "CrawlWave Client";
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMain_Closing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.tabTabPages.ResumeLayout(false);
			this.tabStatus.ResumeLayout(false);
			this.grpStatus.ResumeLayout(false);
			this.grpStatitics.ResumeLayout(false);
			this.tabCrawlerLog.ResumeLayout(false);
			this.grpCrawlerLog.ResumeLayout(false);
			this.tabUserInfo.ResumeLayout(false);
			this.grpUserStatistics.ResumeLayout(false);
			this.grpUserInformation.ResumeLayout(false);
			this.tabSettings.ResumeLayout(false);
			this.grpSettingsScheduler.ResumeLayout(false);
			this.grpSettingsGeneral.ResumeLayout(false);
			this.grpSettingsNetSpeed.ResumeLayout(false);
			this.tabAbout.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion

		#region Main Method

		/// <summary>
		/// The application's entry point
		/// </summary>
		/// <param name="args">The arguments passed to the application.</param>
		[STAThread]
		public static void Main(string []args)
		{
			Application.Run(new frmMain());
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Disables all the controls of the Status Tab page
		/// </summary>
		private void DisableControls()
		{
			cmdStart.Enabled = false;
			cmdPause.Enabled = false;
			cmdResume.Enabled = false;
			cmdStop.Enabled = false;
			cmdTerminate.Enabled = false;
		}

		/// <summary>
		/// Enables the appropriate buttons according to the crawler's state.
		/// </summary>
		/// <param name="state">The <see cref="CrawlerState"/> of the Client.</param>
		private void EnableControls(CrawlerState state)
		{
			switch(state)
			{
				case CrawlerState.Stopped:
					cmdStart.Enabled = true;
					cmdStop.Enabled = false;
					cmdPause.Enabled = false;
					cmdResume.Enabled = false;
					cmdTerminate.Enabled = true;
					break;

				case CrawlerState.Paused:
					cmdStart.Enabled = false;
					cmdStop.Enabled = false;
					cmdPause.Enabled = false;
					cmdResume.Enabled = false;
					cmdTerminate.Enabled = true;
					break;

				case CrawlerState.Running:
					cmdStart.Enabled = false;
					cmdStop.Enabled = true;
					cmdPause.Enabled = true;
					cmdResume.Enabled = false;
					cmdTerminate.Enabled = true;
					break;

				default:
					DisableControls();
					break;
			}
		}

		/// <summary>
		/// Updates the statistics controls (labels).
		/// </summary>
		private void UpdateStatistics()
		{
			lblCrawledVal.Text = stats[0].ToString();
			lblCrawledOKVal.Text = stats[1].ToString();
			lblCrawledNotFoundVal.Text = stats[2].ToString();
			lblCrawledUnauthorizedVal.Text = stats[4].ToString();
			lblCrawledForbiddenVal.Text = stats[5].ToString();
			lblCrawledTimeoutVal.Text = stats[6].ToString();
			lblCrawledServerErrorVal.Text = stats[7].ToString();
			lblCrawledOtherErrorsVal.Text = stats[8].ToString();
		}

		/// <summary>
		/// Loads the <see cref="ClientSettings"/> of the Client and populates the fields.
		/// </summary>
		private void LoadSettings()
		{
			settings = core.GetSettings();
			txtUsername.Text = settings.UserName;
			if(settings.UserName != String.Empty)
			{
				grpUserInformation.Enabled = false;
			}
			else
			{
				grpUserStatistics.Enabled = false;
				DisableControls();
			}
			txtEmail.Text = settings.Email;
			chkLoadAtStartup.Checked = settings.LoadAtStartup;
			chkMinimizeToSystemTray.Checked = settings.MinimizeToTray;
			chkMinimizeOnExit.Checked = settings.MinimizeOnExit;
			SelectConnectionSpeed(settings.ConnectionSpeed);
			chkEnableScheduler.Checked = settings.EnableScheduler;
			dtStartTime.Enabled = settings.EnableScheduler;
			dtStopTime.Enabled = settings.EnableScheduler;
			lblStartTime.Enabled = settings.EnableScheduler;
			lblStopTime.Enabled = settings.EnableScheduler;
			dtStartTime.Value = settings.StartTime;
			dtStopTime.Value = settings.StopTime;
			state = core.GetState();
			EnableControls(state);
		}

		/// <summary>
		/// Selects the right item in the Connection Speed ComboBox
		/// </summary>
		/// <param name="speed">The Connection Speed to be selected.</param>
		private void SelectConnectionSpeed(CWConnectionSpeed speed)
		{
			switch(speed)
			{
				case CWConnectionSpeed.Unknown:
					cmbNetSpeed.SelectedIndex = 0;
					break;
				
				case CWConnectionSpeed.Modem56K:
					cmbNetSpeed.SelectedIndex = 1;
					break;

				case CWConnectionSpeed.ISDN64K:
					cmbNetSpeed.SelectedIndex = 2;
					break;

				case CWConnectionSpeed.ISDN128K:
					cmbNetSpeed.SelectedIndex = 3;
					break;

				case CWConnectionSpeed.DSL256K:
					cmbNetSpeed.SelectedIndex = 4;
					break;

				case CWConnectionSpeed.DSL384K:
					cmbNetSpeed.SelectedIndex = 5;
					break;

				case CWConnectionSpeed.DSL512K:
					cmbNetSpeed.SelectedIndex = 6;
					break;

				case CWConnectionSpeed.DSL1M:
					cmbNetSpeed.SelectedIndex = 7;
					break;

				case CWConnectionSpeed.T1:
					cmbNetSpeed.SelectedIndex = 8;
					break;

				case CWConnectionSpeed.T3:
					cmbNetSpeed.SelectedIndex = 9;
					break;

				case CWConnectionSpeed.Fiber:
					cmbNetSpeed.SelectedIndex = 10;
					break;

				case CWConnectionSpeed.ATM:
					cmbNetSpeed.SelectedIndex = 11;
					break;
				
				default:
					cmbNetSpeed.SelectedIndex = 0;
					break;
			}
		}

		/// <summary>
		/// Attempts to detect network speed by performing a WMI lookup. If the WMI method
		/// fails it uses <see cref="InternetUtils.DetectConnectionSpeed"/>.
		/// </summary>
		/// <returns>True if it succeeds, false otherwise.</returns>
		private bool DetectNetworkSpeed()
		{
			try
			{
				CWComputerInfo info = ComputerInfo.GetComputerInfo();
				if(info.ConnectionSpeed == CWConnectionSpeed.Unknown)
				{
					info.ConnectionSpeed = InternetUtils.DetectConnectionSpeed();
				}
				SelectConnectionSpeed(info.ConnectionSpeed);
			}
			catch
			{
				return false;
			}
			return true;
		}

		#endregion
		
		#region Event Handlers

		/// <summary>
		/// Called right after the form is initialized in order to connect to the Remotable
		/// object exposed by the Client Core.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tmrInit_Tick(object sender, System.EventArgs e)
		{
			tmrInit.Enabled = false;
			try
			{
				channel = new TcpChannel();
				ChannelServices.RegisterChannel(channel, true);

				core = (ICrawlerController) Activator.GetObject(
					typeof(ICrawlerController),
					"tcp://localhost:15460/Controller.rem");
				tmrTimer.Enabled = true;
				LoadSettings();
			}
			catch
			{
				MessageBox.Show("Failed to connect to Client Core. Please make sure it is running.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}
		}

		/// <summary>
		/// Called at regular intervals in order to refresh the form status.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tmrTimer_Tick(object sender, System.EventArgs e)
		{
			try
			{
				if(chkEnableLog.Checked)
				{
					Queue<EventLoggerEntry> events = core.GetEventQueue();
					while(events.Count>0)
					{
						EventLoggerEntry entry = events.Dequeue();
						rtblClientLog.LogEventEntry(entry);
					}
				}
				state = core.GetState();
				EnableControls(state);
				memory = core.GetMemoryUsage();
				if(memory < hlbMemory.Maximum)
				{
					hlbMemory.Value = memory;
				}
				hlgMemory.NextValue = memory;
				if((core != null)&&(core.GetState() == CrawlerState.Running))
				{
					stats = core.GetStatistics();
					speed = (int)(Math.Round((double)((stats[9] - totalbytes)/5120)));
					totalbytes = stats[9];
					hlbSpeed.Value = speed;
					hlgSpeed.NextValue = speed;
					UpdateStatistics();
				}
			}
			catch
			{}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chkEnableScheduler_CheckedChanged(object sender, System.EventArgs e)
		{
			dtStartTime.Enabled = chkEnableScheduler.Checked;
			dtStopTime.Enabled = chkEnableScheduler.Checked;
			lblStartTime.Enabled = chkEnableScheduler.Checked;
			lblStopTime.Enabled = chkEnableScheduler.Checked;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdStart_Click(object sender, System.EventArgs e)
		{
			if(core!=null)
			{
				try
				{
					Cursor = Cursors.WaitCursor;
					SerializedException sx = core.Start();
					if(sx!=null)
					{
						MessageBox.Show("Failed to start the crawler: " + sx.Message);
					}
				}
				catch
				{}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdPause_Click(object sender, System.EventArgs e)
		{
			if(core!=null)
			{
				try
				{
					Cursor = Cursors.WaitCursor;
					if(state == CrawlerState.Running)
					{
						SerializedException sx = core.Pause();
						if(sx!=null)
						{
							MessageBox.Show("Failed to pause the crawler: " + sx.Message);
						}
					}
				}
				catch
				{}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdResume_Click(object sender, System.EventArgs e)
		{
			if(core!=null)
			{
				try
				{
					Cursor = Cursors.WaitCursor;
					if(state == CrawlerState.Paused)
					{
						SerializedException sx = core.Resume();
						if(sx!=null)
						{
							MessageBox.Show("Failed to resume the crawler: " + sx.Message);
						}
					}
				}
				catch
				{}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdStop_Click(object sender, System.EventArgs e)
		{
			if(core!=null)
			{
				try
				{
					Cursor = Cursors.WaitCursor;
					if(state == CrawlerState.Running)
					{
						SerializedException sx = core.Stop();
						if(sx!=null)
						{
							MessageBox.Show("Failed to stop the crawler: " + sx.Message);
						}
					}
				}
				catch
				{}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdTerminate_Click(object sender, System.EventArgs e)
		{
			if(core!=null)
			{
				try
				{
					Cursor = Cursors.WaitCursor;
					SerializedException sx = null;
					core.Terminate(ref sx);
					if(sx!=null)
					{
						MessageBox.Show("Failed to terminate the crawler: " + sx.Message);
					}
					Application.Exit();
				}
				catch
				{}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdRegister_Click(object sender, System.EventArgs e)
		{
			try
			{
				txtUsername.Text = txtUsername.Text.Trim();
				if(txtUsername.Text == String.Empty)
				{
					MessageBox.Show("You must supply a username!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					txtUsername.Focus();
					return;
				}
				txtPassword.Text = txtPassword.Text.Trim();
				if(txtPassword.Text == String.Empty)
				{
					MessageBox.Show("You must supply a password!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					txtPassword.Focus();
					return;
				}
				EmailValidator validator = EmailValidator.Instance();
				if(!validator.Validate(txtEmail.Text))
				{
					MessageBox.Show("You must supply a valid email address!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					txtEmail.Focus();
					return;
				}
			}
			catch
			{}
			try
			{
				Cursor = Cursors.WaitCursor;
				SerializedException sx = core.RegisterUser(txtUsername.Text, txtPassword.Text, txtEmail.Text);
				if(sx!=null)
				{
					MessageBox.Show("An error occured during the registration: " + sx.Message);
					return;
				}
				else
				{
					settings = core.GetSettings();
					LoadSettings();
					EnableControls(CrawlerState.Stopped);
				}
			}
			catch(Exception ex)
			{
				ex.ToString();
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdClear_Click(object sender, System.EventArgs e)
		{
			txtUsername.Text = String.Empty;
			txtPassword.Text = String.Empty;
			txtEmail.Text = String.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdRefreshStatistics_Click(object sender, System.EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				if(core!=null)
				{
					UserStatistics stats = null;
					SerializedException sx = core.GetUserStatistics(ref stats);
					if(sx!=null)
					{
						MessageBox.Show("An error occured while retrieving the statistics:" + sx.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
					else
					{
						lblUSRegistrationDateVal.Text = stats.RegistrationDate.ToString();
						lblUSNumClientsVal.Text = stats.NumClients.ToString();
						lblUSUrlStatsVal.Text = stats.UrlsAssigned.ToString() + " / " + stats.UrlsReturned.ToString();
						lblUSLastActiveVal.Text = stats.LastActive.ToString();
					}
				}
			}
			catch
			{
				//globals.FileLog.LogWarning("CrawlWave.Client: Failed to retrieve user's statistics: " + ex.ToString());
			}
			finally
			{
				Cursor = Cursors.Default;
				GC.Collect();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdDetectNetSpeed_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;

				if(!InternetUtils.ConnectedToInternet())
				{
					MessageBox.Show("You need to be connected to the Internet before attempting to detect the network speed!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
				if(!DetectNetworkSpeed())
				{
					MessageBox.Show("Failed to estimate the connection speed.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			catch
			{}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdSaveSettings_Click(object sender, System.EventArgs e)
		{
			settings.LoadAtStartup = chkLoadAtStartup.Checked;
			settings.MinimizeToTray = chkMinimizeToSystemTray.Checked;
			settings.MinimizeOnExit = chkMinimizeOnExit.Checked;
			settings.EnableScheduler = chkEnableScheduler.Checked;
			settings.StartTime = dtStartTime.Value;
			settings.StopTime = dtStopTime.Value;
			switch(cmbNetSpeed.SelectedIndex)
			{
				case 0:
					settings.ConnectionSpeed  = CWConnectionSpeed.Unknown;
					break;
				
				case 1:
					settings.ConnectionSpeed = CWConnectionSpeed.Modem56K;
					break;

				case 2:
					settings.ConnectionSpeed = CWConnectionSpeed.ISDN64K;
					break;

				case 3:
					settings.ConnectionSpeed = CWConnectionSpeed.ISDN128K;
					break;

				case 4:
					settings.ConnectionSpeed = CWConnectionSpeed.DSL256K;
					break;

				case 5:
					settings.ConnectionSpeed = CWConnectionSpeed.DSL384K;
					break;

				case 6:
					settings.ConnectionSpeed = CWConnectionSpeed.DSL512K;
					break;

				case 7:
					settings.ConnectionSpeed = CWConnectionSpeed.DSL1M;
					break;

				case 8:
					settings.ConnectionSpeed = CWConnectionSpeed.T1;
					break;

				case 9:
					settings.ConnectionSpeed = CWConnectionSpeed.T3;
					break;

				case 10:
					settings.ConnectionSpeed = CWConnectionSpeed.Fiber;
					break;

				case 11:
					settings.ConnectionSpeed = CWConnectionSpeed.ATM;
					break;
				
				default:
					settings.ConnectionSpeed = CWConnectionSpeed.Unknown;
					break;
			}
			try
			{
				core.SetSettings(settings);
			}
			catch
			{}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdUndoChanges_Click(object sender, System.EventArgs e)
		{
			LoadSettings();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdClearLog_Click(object sender, System.EventArgs e)
		{
			rtblClientLog.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabTabPages_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(tabTabPages.SelectedTab == tabCrawlerLog)
			{
				try
				{
					rtblClientLog.SelectionStart = rtblClientLog.TextLength-1;
					rtblClientLog.Focus();
				}
				catch
				{}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmMain_Load(object sender, System.EventArgs e)
		{
			lblVersion.Text = "Version:" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			try
			{
				SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
			}
			catch
			{}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmMain_Resize(object sender, System.EventArgs e)
		{
			if(WindowState == FormWindowState.Minimized)
			{
				if(settings.MinimizeToTray)
				{
					nfiTrayIcon.Visible = true;
					tmrTimer.Enabled = false;
					this.Hide();
				}
			}
			else
			{
				if(nfiTrayIcon.Visible == true)
				{
					nfiTrayIcon.Visible = false;
					tmrTimer.Enabled = true;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(settings.MinimizeOnExit)
			{
				WindowState = FormWindowState.Minimized;
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Handles the SessionEnding events, allowing the client to terminate if the system
		/// is shutting down but not be affected if the user is simply logging off.
		/// </summary>
		/// <param name="sender">The object related to the event.</param>
		/// <param name="e">The arguments related to the event.</param>
		private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
		{
			if(e.Reason == SessionEndReasons.SystemShutdown)
			{
				if(core!=null)
				{
					try
					{
						SerializedException sx = null;
						core.Terminate(ref sx);
						if(sx!=null)
						{
							MessageBox.Show("Failed to terminate the crawler: " + sx.Message);
						}
						Application.Exit();
					}
					catch
					{}
				}
				e.Cancel = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuTrayShow_Click(object sender, System.EventArgs e)
		{
			this.Show();
			WindowState = FormWindowState.Normal;
			nfiTrayIcon.Visible = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuTrayExit_Click(object sender, System.EventArgs e)
		{
			/*if(core!=null)
			{
				if(core.GetState() != CrawlerState.Stopped)
				{
					try
					{
						core.Stop();
					}
					catch
					{}
				}
			}*/
			nfiTrayIcon.Visible = false;
			Application.Exit();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nfiTrayIcon_DoubleClick(object sender, System.EventArgs e)
		{
			this.Show();
			WindowState = FormWindowState.Normal;
			nfiTrayIcon.Visible = false;
		}

		#endregion

	}
}




