using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CrawlWave.Common;
using CrawlWave.ServerCommon;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace CrawlWave.ServerManager.Forms
{
	/// <summary>
	/// frmClientUpdate allows the creation of CrawlWave Client updates.
	/// </summary>
	public class frmClientUpdate : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.GroupBox grpAddNewVersion;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdBrowse;
		private System.Windows.Forms.Button cmdClear;
		private System.Windows.Forms.TextBox txtNewVersion;
		private System.Windows.Forms.Label lblNewVersion;
		private System.Windows.Forms.Label lblLatestVersion;
		private System.Windows.Forms.Label lblLatestVersionVal;
		private System.Windows.Forms.Label lblFilesToInclude;
		private System.Windows.Forms.ProgressBar prgProgress;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.ListView lstFiles;
		private System.Windows.Forms.ColumnHeader colFileName;
		private System.Windows.Forms.ColumnHeader colVersion;
		private System.Windows.Forms.ColumnHeader colSize;
		private System.Windows.Forms.OpenFileDialog dlgBrowse;
		private System.Windows.Forms.ImageList imgIcons;
		private System.Windows.Forms.ContextMenu mnuFiles;
		private System.Windows.Forms.MenuItem mnuFileRemove;
		private System.Windows.Forms.Button cmdBrowseSetup;
		private System.Windows.Forms.OpenFileDialog dlgBrowseSetup;

		private Globals globals;

		/// <summary>
		/// Creates a new instance of the <see cref="frmClientUpdate"/> form. 
		/// </summary>
		public frmClientUpdate()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmClientUpdate));
			this.grpAddNewVersion = new System.Windows.Forms.GroupBox();
			this.cmdBrowseSetup = new System.Windows.Forms.Button();
			this.lstFiles = new System.Windows.Forms.ListView();
			this.colFileName = new System.Windows.Forms.ColumnHeader();
			this.colVersion = new System.Windows.Forms.ColumnHeader();
			this.colSize = new System.Windows.Forms.ColumnHeader();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.prgProgress = new System.Windows.Forms.ProgressBar();
			this.lblFilesToInclude = new System.Windows.Forms.Label();
			this.cmdBrowse = new System.Windows.Forms.Button();
			this.lblNewVersion = new System.Windows.Forms.Label();
			this.txtNewVersion = new System.Windows.Forms.TextBox();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdClear = new System.Windows.Forms.Button();
			this.lblLatestVersion = new System.Windows.Forms.Label();
			this.lblLatestVersionVal = new System.Windows.Forms.Label();
			this.cmdClose = new System.Windows.Forms.Button();
			this.dlgBrowse = new System.Windows.Forms.OpenFileDialog();
			this.mnuFiles = new System.Windows.Forms.ContextMenu();
			this.mnuFileRemove = new System.Windows.Forms.MenuItem();
			this.dlgBrowseSetup = new System.Windows.Forms.OpenFileDialog();
			this.grpAddNewVersion.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpAddNewVersion
			// 
			this.grpAddNewVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpAddNewVersion.Controls.Add(this.cmdBrowseSetup);
			this.grpAddNewVersion.Controls.Add(this.lstFiles);
			this.grpAddNewVersion.Controls.Add(this.prgProgress);
			this.grpAddNewVersion.Controls.Add(this.lblFilesToInclude);
			this.grpAddNewVersion.Controls.Add(this.cmdBrowse);
			this.grpAddNewVersion.Controls.Add(this.lblNewVersion);
			this.grpAddNewVersion.Controls.Add(this.txtNewVersion);
			this.grpAddNewVersion.Location = new System.Drawing.Point(8, 32);
			this.grpAddNewVersion.Name = "grpAddNewVersion";
			this.grpAddNewVersion.Size = new System.Drawing.Size(336, 192);
			this.grpAddNewVersion.TabIndex = 2;
			this.grpAddNewVersion.TabStop = false;
			this.grpAddNewVersion.Text = "Add a new version update";
			// 
			// cmdBrowseSetup
			// 
			this.cmdBrowseSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdBrowseSetup.Location = new System.Drawing.Point(104, 160);
			this.cmdBrowseSetup.Name = "cmdBrowseSetup";
			this.cmdBrowseSetup.Size = new System.Drawing.Size(88, 24);
			this.cmdBrowseSetup.TabIndex = 6;
			this.cmdBrowseSetup.Text = "Add Installer";
			this.cmdBrowseSetup.Click += new System.EventHandler(this.cmdBrowseSetup_Click);
			// 
			// lstFiles
			// 
			this.lstFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.colFileName,
																					   this.colVersion,
																					   this.colSize});
			this.lstFiles.FullRowSelect = true;
			this.lstFiles.GridLines = true;
			this.lstFiles.Location = new System.Drawing.Point(8, 32);
			this.lstFiles.MultiSelect = false;
			this.lstFiles.Name = "lstFiles";
			this.lstFiles.Size = new System.Drawing.Size(320, 97);
			this.lstFiles.SmallImageList = this.imgIcons;
			this.lstFiles.TabIndex = 1;
			this.lstFiles.View = System.Windows.Forms.View.Details;
			this.lstFiles.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstFiles_MouseUp);
			// 
			// colFileName
			// 
			this.colFileName.Text = "File Name";
			this.colFileName.Width = 180;
			// 
			// colVersion
			// 
			this.colVersion.Text = "Version";
			// 
			// colSize
			// 
			this.colSize.Text = "Size";
			this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// imgIcons
			// 
			this.imgIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// prgProgress
			// 
			this.prgProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.prgProgress.Location = new System.Drawing.Point(200, 164);
			this.prgProgress.Name = "prgProgress";
			this.prgProgress.Size = new System.Drawing.Size(128, 16);
			this.prgProgress.TabIndex = 5;
			// 
			// lblFilesToInclude
			// 
			this.lblFilesToInclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblFilesToInclude.Location = new System.Drawing.Point(8, 16);
			this.lblFilesToInclude.Name = "lblFilesToInclude";
			this.lblFilesToInclude.Size = new System.Drawing.Size(320, 16);
			this.lblFilesToInclude.TabIndex = 0;
			this.lblFilesToInclude.Text = "Files to include in the update:";
			this.lblFilesToInclude.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// cmdBrowse
			// 
			this.cmdBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdBrowse.Location = new System.Drawing.Point(8, 160);
			this.cmdBrowse.Name = "cmdBrowse";
			this.cmdBrowse.Size = new System.Drawing.Size(88, 24);
			this.cmdBrowse.TabIndex = 4;
			this.cmdBrowse.Text = "Add Files...";
			this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
			// 
			// lblNewVersion
			// 
			this.lblNewVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblNewVersion.Location = new System.Drawing.Point(8, 136);
			this.lblNewVersion.Name = "lblNewVersion";
			this.lblNewVersion.Size = new System.Drawing.Size(88, 23);
			this.lblNewVersion.TabIndex = 2;
			this.lblNewVersion.Text = "Update Version:";
			this.lblNewVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtNewVersion
			// 
			this.txtNewVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtNewVersion.Location = new System.Drawing.Point(104, 136);
			this.txtNewVersion.Name = "txtNewVersion";
			this.txtNewVersion.Size = new System.Drawing.Size(224, 20);
			this.txtNewVersion.TabIndex = 3;
			this.txtNewVersion.Text = "";
			// 
			// cmdOK
			// 
			this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdOK.Location = new System.Drawing.Point(16, 232);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(88, 24);
			this.cmdOK.TabIndex = 3;
			this.cmdOK.Text = "Create Update";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdClear
			// 
			this.cmdClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClear.Location = new System.Drawing.Point(112, 232);
			this.cmdClear.Name = "cmdClear";
			this.cmdClear.Size = new System.Drawing.Size(88, 24);
			this.cmdClear.TabIndex = 4;
			this.cmdClear.Text = "Clear";
			this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
			// 
			// lblLatestVersion
			// 
			this.lblLatestVersion.Location = new System.Drawing.Point(8, 8);
			this.lblLatestVersion.Name = "lblLatestVersion";
			this.lblLatestVersion.Size = new System.Drawing.Size(200, 23);
			this.lblLatestVersion.TabIndex = 0;
			this.lblLatestVersion.Text = "Latest available Client Update Version:";
			this.lblLatestVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblLatestVersionVal
			// 
			this.lblLatestVersionVal.Location = new System.Drawing.Point(208, 8);
			this.lblLatestVersionVal.Name = "lblLatestVersionVal";
			this.lblLatestVersionVal.Size = new System.Drawing.Size(136, 23);
			this.lblLatestVersionVal.TabIndex = 1;
			this.lblLatestVersionVal.Text = "N/A";
			this.lblLatestVersionVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmdClose
			// 
			this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClose.Location = new System.Drawing.Point(208, 232);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(88, 24);
			this.cmdClose.TabIndex = 5;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// dlgBrowse
			// 
			this.dlgBrowse.Filter = "All Files|*.*";
			this.dlgBrowse.Title = "Selecta file to be added in the update...";
			// 
			// mnuFiles
			// 
			this.mnuFiles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuFileRemove});
			// 
			// mnuFileRemove
			// 
			this.mnuFileRemove.Index = 0;
			this.mnuFileRemove.Text = "Remove file...";
			this.mnuFileRemove.Click += new System.EventHandler(this.mnuFileRemove_Click);
			// 
			// dlgBrowseSetup
			// 
			this.dlgBrowseSetup.Filter = "Programs|*.exe";
			this.dlgBrowseSetup.Title = "Add CrawlWave Client Update Installer...";
			// 
			// frmClientUpdate
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(352, 263);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.lblLatestVersionVal);
			this.Controls.Add(this.lblLatestVersion);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.grpAddNewVersion);
			this.Controls.Add(this.cmdClear);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmClientUpdate";
			this.Text = "Add a new Client Update";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmClientUpdate_Closing);
			this.Load += new System.EventHandler(this.frmClientUpdate_Load);
			this.grpAddNewVersion.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmClientUpdate_Load(object sender, System.EventArgs e)
		{
			LoadValues();
		}

		private void frmClientUpdate_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			globals.LoadedForms[this.Name] = null;
		}

		private void cmdBrowse_Click(object sender, System.EventArgs e)
		{
			dlgBrowse.ShowDialog();
			if(dlgBrowse.FileName!= String.Empty)
			{
				AddFile(dlgBrowse.FileName);
				dlgBrowse.FileName = String.Empty;
			}
		}

		private void cmdBrowseSetup_Click(object sender, System.EventArgs e)
		{
			dlgBrowseSetup.ShowDialog();
			if(dlgBrowseSetup.FileName!=String.Empty)
			{
				AddInstaller(dlgBrowseSetup.FileName);
				dlgBrowseSetup.FileName = String.Empty;
			}
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			if(!ValidateForm())
			{
				return;
			}
			if(!AddUpdate())
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

		private void lstFiles_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button==MouseButtons.Right)
			{
				if((lstFiles.Items.Count>0)&&(lstFiles.SelectedItems.Count>0))
				{
					mnuFiles.Show(lstFiles, new Point(e.X, e.Y));
				}
			}		
		}

		private void mnuFileRemove_Click(object sender, System.EventArgs e)
		{
			if(MessageBox.Show("Are you sure you wish to remove this file from the list?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
			{
				lstFiles.Items.RemoveAt(lstFiles.SelectedIndices[0]);
			}		
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Retrieves the version of the latest available update.
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
				SqlCommand cmd = new SqlCommand("cw_select_client_versions", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				DataSet ds = new DataSet();
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(ds);
				da.Dispose();
				cmd.Dispose();
				dbcon.Close();
				Version latestVersion = new Version(0,0,0,0);
				Version currentVersion;
				foreach(DataRow dr in ds.Tables[0].Rows)
				{
					try
					{
						currentVersion = new Version(((string)dr[0]).Trim());
						if(currentVersion > latestVersion)
						{
							latestVersion = currentVersion;
						}
					}
					catch
					{
						continue;
					}
				}
				ds.Dispose();
				lblLatestVersionVal.Text = latestVersion.ToString();
			}
			catch(Exception ex)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to retrieve the latest version: " + ex.ToString());
				MessageBox.Show(this.Text + " failed to retrieve the latest version:\n" + ex.Message);
				GC.Collect();
			}
		}

		/// <summary>
		/// Clears the list of files and the other fields.
		/// </summary>
		private void ClearForm()
		{
			lstFiles.Items.Clear();
			txtNewVersion.Text = String.Empty;
			prgProgress.Value = 0;
			LoadValues();
		}

		/// <summary>
		/// Validates the user's input before creating an updated version.
		/// </summary>
		/// <returns></returns>
		private bool ValidateForm()
		{
			if(lstFiles.Items.Count == 0)
			{
				MessageBox.Show("You must select at least one file in order to create an update!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				cmdBrowse.Focus();
				return false;
			}
			else
			{
				string file = String.Empty;
				ArrayList al = new ArrayList();
				for(int i = 0; i< lstFiles.Items.Count; i++)
				{
					file = Path.GetFileName(lstFiles.Items[i].SubItems[0].Text);
					if(al.Contains(file))
					{
						MessageBox.Show("A duplicate file name has been detected:\n" + file, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return false;
					}
					else
					{
						al.Add(file);
					}
				}
				al.Clear();
			}
			if(txtNewVersion.Text == String.Empty)
			{
				MessageBox.Show("You must provide the update version!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtNewVersion.Focus();
				return false;
			}
			else
			{
				try
				{
					Version v = new Version(txtNewVersion.Text);
					if(v <= new Version(lblLatestVersionVal.Text))
					{
						MessageBox.Show("The update version you provided must be greater than " + lblLatestVersionVal.Text , this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						txtNewVersion.Focus();
						return false;
					}
				}
				catch
				{
					MessageBox.Show("The version you provided is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					txtNewVersion.Focus();
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Adds a file to the list of files that will be deployed with the update.
		/// </summary>
		/// <param name="file"></param>
		private void AddFile(string file)
		{
			try
			{
				FileVersionInfo fvi = null;
				FileInfo fi = null;
				switch(Path.GetExtension(file))
				{
					case ".exe":
					case ".EXE":
						if(Path.GetFileName(file)=="CrawlWave.Client.exe")
						{
							Assembly a = Assembly.LoadFile(file);
							txtNewVersion.Text = a.GetName().Version.ToString();
						}
						lstFiles.Items.Add(file, 0);
						fvi = FileVersionInfo.GetVersionInfo(file);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fvi.FileVersion);
						fi = new FileInfo(file);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fi.Length.ToString());
					break;

					case ".dll":
					case ".DLL":
						lstFiles.Items.Add(file, 1);
						fvi = FileVersionInfo.GetVersionInfo(file);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fvi.FileVersion);
						fi = new FileInfo(file);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fi.Length.ToString());
						break;

					case ".txt":
					case ".TXT":
					case ".xml":
					case ".XML":
					case ".rtf":
					case ".RTF":
						lstFiles.Items.Add(file, 2);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add("N/A");
						fi = new FileInfo(file);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fi.Length.ToString());
						break;

					case ".hlp":
					case ".HLP":
						lstFiles.Items.Add(file, 3);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add("N/A");
						fi = new FileInfo(file);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fi.Length.ToString());
						break;

					case ".chm":
					case ".CHM":
						lstFiles.Items.Add(file, 4);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add("N/A");
						fi = new FileInfo(file);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fi.Length.ToString());
						break;

					default:
						lstFiles.Items.Add(file, 5);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add("N/A");
						fi = new FileInfo(file);
						lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fi.Length.ToString());
						break;
				}
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to add a file in the update list: " + e.ToString());
				MessageBox.Show(this.Text + " failed to add a file in the update list:\n" + e.Message);
				GC.Collect();
			}
		}

		/// <summary>
		/// Checks if the supplied file contains a valid CrawlWave Client Updater and if so
		/// it adds it to the list of files to be deployed with the update.
		/// </summary>
		/// <param name="file"></param>
		private void AddInstaller(string file)
		{
			try
			{
				Assembly a = Assembly.LoadFile(file);
				System.Type []types = a.GetTypes();
				bool setupFound = false;
				foreach(Type t in types)
				{
					object [] attributes = t.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute),false);
					if((attributes != null) && (attributes.Length>0))
					{
						foreach (System.ComponentModel.DescriptionAttribute attribute in attributes)
						{
							if(attribute.Description == "CrawlWave Client Updater")
							{
								setupFound = true;
							}
						}
						break;
					}
				}
				if(!setupFound)
				{
					MessageBox.Show("This is not a valid CrawlWave Client Updater.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					lstFiles.Items.Add(file, 6);
					FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(file);
					lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fvi.FileVersion);
					FileInfo fi = new FileInfo(file);
					lstFiles.Items[lstFiles.Items.Count-1].SubItems.Add(fi.Length.ToString());
				}
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to add an update installer: " + e.ToString());
				MessageBox.Show(this.Text + " failed to add an update installer:\n" + e.Message);
				GC.Collect();
			}
		}

		/// <summary>
		/// Builds the update and stores it in the system's database.
		/// </summary>
		/// <returns>True if the operation succeeds, false otherwise.</returns>
		private bool AddUpdate()
		{
			bool retVal = false;
			try
			{
				byte [] buffer = new byte[0];
				FileStream ifs = null;
				FileInfo fi = null;
				MemoryStream oms = new MemoryStream();
				Crc32 crc = new Crc32();
				ZipOutputStream zs = new ZipOutputStream(oms);
				int numFiles = lstFiles.Items.Count;
				for(int i = 0; i<numFiles ; i++)
				{
					string entryName = Path.GetFileName(lstFiles.Items[i].SubItems[0].Text);
					fi = new FileInfo(lstFiles.Items[i].SubItems[0].Text);
					ifs = File.OpenRead(lstFiles.Items[i].SubItems[0].Text);
					buffer = new byte[fi.Length];
					ifs.Read(buffer, 0, buffer.Length);
					ZipEntry entry = new ZipEntry(entryName);
					entry.DateTime = fi.LastWriteTime;
					entry.Size = fi.Length;
					crc.Reset();
					crc.Update(buffer);
					entry.Crc = crc.Value;
					zs.PutNextEntry(entry);
					zs.Write(buffer, 0, buffer.Length);
					prgProgress.Value = (int)(((i+1)*80)/numFiles);
				}
				zs.Finish();
				zs.Close();

				buffer = oms.ToArray();
				oms.Close();

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
				prgProgress.Value = 85;
				SqlCommand cmd = new SqlCommand("cw_insert_client_update", dbcon);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@client_update_version", SqlDbType.Char, 15);
				cmd.Parameters.Add("@client_update_image", SqlDbType.Image);
				Version latestVersion = new Version(txtNewVersion.Text);
				cmd.Parameters[0].Value = latestVersion.ToString();
				cmd.Parameters[1].Value = buffer;
				prgProgress.Value = 90;
				cmd.ExecuteNonQuery();
				dbcon.Close();
				prgProgress.Value = 100;
				lblLatestVersionVal.Text = latestVersion.ToString();
				retVal = true;
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to create an update: " + e.ToString());
				MessageBox.Show(this.Text + " failed to create and update:\n" + e.Message);
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
