using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CrawlWave.ServerInstaller.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class frmStep3 : CrawlWave.ServerInstaller.Forms.frmStep
	{
		private System.Windows.Forms.ProgressBar prgProgress;
		private System.Windows.Forms.GroupBox grpStatus;
		private System.Windows.Forms.TextBox txtStatus;
		private System.Windows.Forms.Label lblPleaseWait;
		private System.Windows.Forms.Timer tmrStart;
		private System.ComponentModel.IContainer components = null;

		private Globals globals;
		private TextBoxWriter log;
		private SqlConnection dbcon;
		private bool success;

		/// <summary>
		/// 
		/// </summary>
		public frmStep3()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			globals = Globals.Instance();
			success = true;
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
			this.components = new System.ComponentModel.Container();
			this.prgProgress = new System.Windows.Forms.ProgressBar();
			this.grpStatus = new System.Windows.Forms.GroupBox();
			this.txtStatus = new System.Windows.Forms.TextBox();
			this.lblPleaseWait = new System.Windows.Forms.Label();
			this.tmrStart = new System.Windows.Forms.Timer(this.components);
			this.grpStatus.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblStep1
			// 
			this.lblStep1.ImageIndex = 2;
			this.lblStep1.Name = "lblStep1";
			// 
			// lblStep2
			// 
			this.lblStep2.ImageIndex = 5;
			this.lblStep2.Name = "lblStep2";
			// 
			// lblStep3
			// 
			this.lblStep3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblStep3.ImageIndex = 7;
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
			this.cmdBack.Enabled = false;
			this.cmdBack.Name = "cmdBack";
			this.cmdBack.TabIndex = 3;
			this.cmdBack.Click += new System.EventHandler(this.cmdBack_Click);
			// 
			// cmdNext
			// 
			this.cmdNext.Enabled = false;
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.TabIndex = 4;
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Enabled = false;
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.TabIndex = 5;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// prgProgress
			// 
			this.prgProgress.Location = new System.Drawing.Point(8, 176);
			this.prgProgress.Name = "prgProgress";
			this.prgProgress.Size = new System.Drawing.Size(328, 16);
			this.prgProgress.TabIndex = 1;
			// 
			// grpStatus
			// 
			this.grpStatus.Controls.Add(this.txtStatus);
			this.grpStatus.Controls.Add(this.prgProgress);
			this.grpStatus.Location = new System.Drawing.Point(144, 32);
			this.grpStatus.Name = "grpStatus";
			this.grpStatus.Size = new System.Drawing.Size(344, 200);
			this.grpStatus.TabIndex = 2;
			this.grpStatus.TabStop = false;
			this.grpStatus.Text = "Status";
			// 
			// txtStatus
			// 
			this.txtStatus.Location = new System.Drawing.Point(8, 16);
			this.txtStatus.Multiline = true;
			this.txtStatus.Name = "txtStatus";
			this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtStatus.Size = new System.Drawing.Size(328, 152);
			this.txtStatus.TabIndex = 0;
			this.txtStatus.Text = "";
			// 
			// lblPleaseWait
			// 
			this.lblPleaseWait.Location = new System.Drawing.Point(144, 8);
			this.lblPleaseWait.Name = "lblPleaseWait";
			this.lblPleaseWait.Size = new System.Drawing.Size(344, 23);
			this.lblPleaseWait.TabIndex = 1;
			this.lblPleaseWait.Text = "Performing requested actions, please wait...";
			this.lblPleaseWait.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tmrStart
			// 
			this.tmrStart.Enabled = true;
			this.tmrStart.Tick += new System.EventHandler(this.tmrStart_Tick);
			// 
			// frmStep3
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 271);
			this.Controls.Add(this.lblPleaseWait);
			this.Controls.Add(this.grpStatus);
			this.Name = "frmStep3";
			this.Text = "Building Database (Step 3 of 6)";
			this.Load += new System.EventHandler(this.frmStep3_Load);
			this.Controls.SetChildIndex(this.grpStatus, 0);
			this.Controls.SetChildIndex(this.lblPleaseWait, 0);
			this.Controls.SetChildIndex(this.cmdCancel, 0);
			this.Controls.SetChildIndex(this.cmdNext, 0);
			this.Controls.SetChildIndex(this.cmdBack, 0);
			this.grpStatus.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void frmStep3_Load(object sender, System.EventArgs e)
		{
			globals.LoadedForms.Add(this.Name, this);
		}

		private void tmrStart_Tick(object sender, System.EventArgs e)
		{
			tmrStart.Enabled = false;
			log = new TextBoxWriter(txtStatus);
			DoBuildDatabase();
		}

		private void cmdBack_Click(object sender, System.EventArgs e)
		{
			frmStep2 frm = new frmStep2();
			frm.Show();
			globals.LoadedForms[this.Name] = null;
			globals.LoadedForms.Remove(this.Name);
			this.Close();
		}

		private void cmdNext_Click(object sender, System.EventArgs e)
		{
			frmStep4 frm = new frmStep4();
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

		#region Private methods

		private void DoBuildDatabase()
		{
			try
			{
				log.WriteLine("Performing required actions");
				
				ConnectToDatabase();
				if(!success)
				{
					return;
				}
				CreateDatabase();
				if(!success)
				{
					return;
				}
				CreateDBUser();
				if(!success)
				{
					return;
				}
				CreateDBObjects();
				if(!success)
				{
					return;
				}
				CreateDBJobs();
				if(!success)
				{
					return;
				}
				PerformExtraTasks();
				//close connection if it is open
				DisconnectFromDatabase();

				log.WriteLine("All actions completed successfully.");
			}
			catch
			{
				//close connection if it is open
				DisconnectFromDatabase();
			}
			finally
			{
				//enable controls
				cmdNext.Enabled = success;
				cmdBack.Enabled = !success;
				cmdCancel.Enabled = true;
			}
		}

		private void ConnectToDatabase()
		{
			try
			{
				log.WriteLine("Opening connection to database...");

				StringBuilder sb = new StringBuilder("Password=");
				sb.Append(globals.ConfigurationSettings.DBAPass);
				sb.Append(";Persist Security Info=True;User ID=");
				sb.Append(globals.ConfigurationSettings.DBAUser);
				sb.Append(";Initial Catalog=master;Data Source=");
				sb.Append(globals.ConfigurationSettings.SQLServer);
				sb.Append(";Application Name =CrawlWave.ServerInstaller;");
				dbcon = new SqlConnection(sb.ToString());
				sb = null;
				dbcon.Open();
				
				prgProgress.Value = 1;
				log.WriteLine("Connection to database succeeded.");
			}
			catch(Exception e)
			{
				log.WriteLine("Connection to database failed: " + e.Message);
				success = false;
			}
		}

		private void CreateDatabase()
		{
			try
			{
				log.WriteLine("Creating Database...");

				StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CrawlWave.ServerInstaller.sqlscripts.db_database.sql"));
				string sqlString = sr.ReadToEnd();
				sr.Close();

				//calculate the sizes for the files
				int indexesSize = (int)(globals.ConfigurationSettings.DBSize * 0.35);
				int dataSize = (int)(globals.ConfigurationSettings.DBSize * 0.6);
				int logSize = (int)(globals.ConfigurationSettings.DBSize * 0.05);

				//set the paths and sizes for the files
				log.WriteLine("Setting the path for Index to " + globals.ConfigurationSettings.DBIndexesPath);
				sqlString = Regex.Replace(sqlString, "@IndexesPath@", globals.ConfigurationSettings.DBIndexesPath);
				log.WriteLine("Setting the size for Index to " + indexesSize.ToString() + "MB");
				sqlString = Regex.Replace(sqlString, "@IndexesSize@", indexesSize.ToString());

				log.WriteLine("Setting the path for Data to " + globals.ConfigurationSettings.DBDataPath);
				sqlString = Regex.Replace(sqlString, "@DataPath@", globals.ConfigurationSettings.DBDataPath);
				log.WriteLine("Setting the size for Data to " + dataSize.ToString() + "MB");
				sqlString = Regex.Replace(sqlString, "@DataSize@", dataSize.ToString());

				log.WriteLine("Setting the path for Log to " + globals.ConfigurationSettings.DBLogPath);
				sqlString = Regex.Replace(sqlString, "@LogPath@", globals.ConfigurationSettings.DBLogPath);
				log.WriteLine("Setting the size for Log to " + logSize.ToString() + "MB");
				sqlString = Regex.Replace(sqlString, "@LogSize@", logSize.ToString());
				
				//set the max sizes for the files if necessary
				if(globals.ConfigurationSettings.DBSizeMax>0)
				{
					int indexesSizeMax = (int)(globals.ConfigurationSettings.DBSizeMax * 0.35);
					int dataSizeMax = (int)(globals.ConfigurationSettings.DBSizeMax * 0.60);
					int logSizeMax = (int)(globals.ConfigurationSettings.DBSizeMax * 0.05);
					log.WriteLine("Setting the max size for Index to " + indexesSizeMax.ToString() + "MB");
					sqlString = Regex.Replace(sqlString, "@IndexesSizeMax@", "MAXSIZE = " + indexesSizeMax.ToString() + "MB,");
					log.WriteLine("Setting the max size for Data to " + dataSizeMax.ToString() + "MB");
					sqlString = Regex.Replace(sqlString, "@DataSizeMax@", "MAXSIZE = " + dataSizeMax.ToString() + "MB,");
					log.WriteLine("Setting the max size for Log to " + logSizeMax.ToString() + "MB");
					sqlString = Regex.Replace(sqlString, "@LogSizeMax@", "MAXSIZE = " + logSizeMax.ToString() + "MB,");
				}
				else
				{
					sqlString = Regex.Replace(sqlString, "@IndexesSizeMax@", String.Empty);
					sqlString = Regex.Replace(sqlString, "@DataSizeMax@", String.Empty);
					sqlString = Regex.Replace(sqlString, "@LogSizeMax@", String.Empty);
				}
				
				string [] commands = Regex.Split(sqlString, "\nGO");
				if(commands.Length == 0)
				{
					log.WriteLine("Failed to retrieve the database creation command.");
					success = false;
					return;
				}
				else
				{
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = dbcon;
					cmd.CommandTimeout = 1200;

					int commands_count = commands.Length;
					for(int i = 0; i < commands_count; i++)
					{
						string command = commands[i].Trim();
						if(command.Length>0)
						{
                            cmd.CommandText = command;
							cmd.ExecuteNonQuery();
						}
						prgProgress.Value = 1 + (int)((i)*10/commands_count);
					}
					prgProgress.Value = 11;
					cmd.Dispose();
				}
				
				log.WriteLine("Creation of the database Succeeded");
			}
			catch(Exception e)
			{
				log.WriteLine("Creation of the database Failed: " + e.Message);
				success = false;
			}
		}

		private void CreateDBUser()
		{
			try
			{
				log.WriteLine("Creating Database Users...");

				StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CrawlWave.ServerInstaller.sqlscripts.db_users.sql"));
				string sqlString = sr.ReadToEnd();
				sr.Close();

				//replace the @CrawlWaveDBUser@, @MachineName@ values
				if(globals.ConfigurationSettings.CWUser == String.Empty)
				{
					sqlString = Regex.Replace(sqlString, "@CrawlWaveDBUser@", globals.ConfigurationSettings.DBAUser);
					sqlString = Regex.Replace(sqlString, "@CrawlWaveDBPass@", globals.ConfigurationSettings.DBAPass);
				}
				else
				{
					sqlString = Regex.Replace(sqlString, "@CrawlWaveDBUser@", globals.ConfigurationSettings.CWUser);
					sqlString = Regex.Replace(sqlString, "@CrawlWaveDBPass@", globals.ConfigurationSettings.CWPass);
				}
				sqlString = Regex.Replace(sqlString, "@MachineName@", Environment.MachineName);

				string [] commands = Regex.Split(sqlString, "\nGO");
				if(commands.Length == 0)
				{
					log.WriteLine("Failed to retrieve the database users creation command.");
					success = false;
					return;
				}
				else
				{
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = dbcon;

					int commands_count = commands.Length;
					for(int i = 0; i < commands_count; i++)
					{
						string command = commands[i].Trim();
						if(command.Length>0)
						{
							cmd.CommandText = command;
							cmd.ExecuteNonQuery();
						}
						prgProgress.Value = 11 + (int)((i)*10/commands_count);
					}
					prgProgress.Value = 21;
					cmd.Dispose();
				}
				
				log.WriteLine("Creation of the database users Succeeded");
			}
			catch(Exception e)
			{
				log.WriteLine("Creation of the database users Failed: " + e.Message);
				success = false;
			}
		}

		private void CreateDBObjects()
		{
			try
			{
				log.WriteLine("Creating Database objects...");

				StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CrawlWave.ServerInstaller.sqlscripts.db_objects.sql"));
				string sqlString = sr.ReadToEnd();
				sr.Close();

				string [] commands = Regex.Split(sqlString, "\nGO");
				if(commands.Length == 0)
				{
					log.WriteLine("Failed to retrieve the database objects creation command.");
					success = false;
					return;
				}
				else
				{
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = dbcon;

					int commands_count = commands.Length;
					for(int i = 0; i < commands_count; i++)
					{
						string command = commands[i].Trim();
						if(command.Length>0)
						{
							cmd.CommandText = command;
							cmd.ExecuteNonQuery();
						}
						prgProgress.Value = 95;
					}
					cmd.Dispose();
				}
				
				log.WriteLine("Creation of the database objects Succeeded");
			}
			catch(Exception e)
			{
				log.WriteLine("Creation of the database objects Failed: " + e.Message);
				success = false;
			}
		}

		private void CreateDBJobs()
		{
			try
			{
				log.WriteLine("Creating Database jobs...");

				StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CrawlWave.ServerInstaller.sqlscripts.db_jobs.sql"));
				string sqlString = sr.ReadToEnd();
				sr.Close();

				string [] commands = Regex.Split(sqlString, "\nGO\n");
				if(commands.Length == 0)
				{
					log.WriteLine("Failed to retrieve the database jobs creation command.");
					success = false;
					return;
				}
				else
				{
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = dbcon;

					int commands_count = commands.Length;
					for(int i = 0; i < commands_count; i++)
					{
						string command = commands[i].Trim();
						if(command.Length>0)
						{
							cmd.CommandText = command;
							cmd.ExecuteNonQuery();
						}
						prgProgress.Value = 95 + (int)((i)*3/commands_count);
					}
					prgProgress.Value = 98;
					cmd.Dispose();
				}
				
				log.WriteLine("Creation of the database jobs Succeeded");
			}
			catch(Exception e)
			{
				log.WriteLine("Creation of the database jobs Failed: " + e.Message);
				success = false;
			}
		}

		private void PerformExtraTasks()
		{
			log.WriteLine("Performing additional actions... done.");
			prgProgress.Value = 100;
		}

		private void DisconnectFromDatabase()
		{
			log.WriteLine("Closing connection to the database...");
			try
			{
				if(dbcon!=null)
				{
					dbcon.Close();
					dbcon.Dispose();
				}
			}
			catch
			{}
		}

		#endregion
	}
}

