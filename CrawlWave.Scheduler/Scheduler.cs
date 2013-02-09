using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using CrawlWave.Common;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using CrawlWave.ServerCommon;

namespace CrawlWave.Scheduler
{
	/// <summary>
	/// CWClientScheduler is a Windows Service that is responsible for the automatic update
	/// of CrawlWave Client and the time scheduling operations.
	/// </summary>
	public class CWClientScheduler : System.ServiceProcess.ServiceBase
	{
		#region Private variables

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Globals globals;
		//private WebServiceProxy proxy;
		private ICrawlWaveServer proxy;
		private System.Timers.Timer tmrStart;
		private AlarmTimer startTimer;
		private Backoff updateBackoff;
		private bool updating;


		#endregion

		#region Constructor and related methods

		/// <summary>
		/// Constructs a new instance of the <see cref="CWClientScheduler"/> class.
		/// </summary>
		public CWClientScheduler()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

			globals = Globals.Instance();
			tmrStart = new System.Timers.Timer();
			startTimer = new AlarmTimer();
			if(globals.Settings.EnableScheduler)
			{
				startTimer.AlarmTime = globals.Settings.StartTime.AddMinutes(-5);
			}
			else
			{
				startTimer.AlarmTime = DateTime.Now.AddMinutes(1);
			}
			startTimer.Enabled = true;
			startTimer.OnAlarmBell += new EventHandler(startTimer_OnAlarmBell);
			startTimer.Start();
			updateBackoff = new Backoff(BackoffSpeed.Linear);
			updating = false;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// CWClientScheduler
			// 
			this.ServiceName = "CWClientScheduler";

		}
		#endregion

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

		//the service entry point
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun = { new CWClientScheduler()};
			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		#endregion

		#region Windows Service related methods

		/// <summary>
		/// Sets things in motion so that the service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			tmrStart.Interval = 15000;
			tmrStart.Enabled = true;
			tmrStart.Elapsed += new ElapsedEventHandler(tmrStart_Elapsed);
		}
 
		/// <summary>
		/// Stops this service.
		/// </summary>
		protected override void OnStop()
		{
			GC.Collect();
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Handles the automatic launch of the Client.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void startTimer_OnAlarmBell(object sender, EventArgs e)
		{
			if(!updating)
			{
				LaunchClient();
			}
		}

		/// <summary>
		/// Handles the automatic update of the Client's version.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tmrStart_Elapsed(object sender, ElapsedEventArgs e)
		{
			tmrStart.Enabled = false;
			tmrStart.Dispose();
			try
			{
				UpdateClient();
				//LaunchClient();
			}
			catch(Exception ex)
			{
				globals.SystemLog.LogWarning("CWClientScheduler failed to update the Client: " + ex.ToString());
			}
			finally
			{
				GC.Collect();
			}
		}

		#endregion

		#region Update related methods

		/// <summary>
		/// Tries to terminate all running client processes because otherwise the
		/// client executable cannot be replaced by a newer version.
		/// </summary>
		private void StopClientProcesses()
		{
			try
			{
				Process []ClientProcesses=Process.GetProcessesByName("CrawlWave.Client");
				if (ClientProcesses.Length==0)
				{
					//there are no client processes running
					return;
				}
				else //try to terminate each process
				{
					foreach(Process ClientProcess in ClientProcesses)
					{
						try
						{
							//First try to terminate it by closing its main window.
							ClientProcess.CloseMainWindow();
							if (ClientProcess.HasExited)
							{
								//attempt to terminate the gui was successful
								ClientProcess.Close();
							}
							else
							{
								//The Client is running in quiet mode, so we must kill it
								//TODO: Use the remoting interface of the Client in order
								//to stop it and then terminate its process.
								ClientProcess.Kill();
							}
						}
						catch(Exception e)
						{
							if(globals.Settings.LogLevel<=CWLogLevel.LogWarning)
							{
								globals.SystemLog.LogWarning("CrawlWave.Client Scheduler failed to terminate all the client processes: "+e.ToString());
							}
							continue; //try to terminate the rest of the processes.
						}
					}
				}
			}
			catch(Exception e)
			{
				if (globals.Settings.LogLevel<=CWLogLevel.LogWarning)
				{
					globals.SystemLog.LogWarning("CrawlWave.Client Scheduler encountered an error while stoping the client processes:" +e.ToString());
				}
			}
		}

		/// <summary>
		/// Launches the CrawlWave Client.
		/// </summary>
		private void LaunchClient()
		{
			while(updating)
			{
				Thread.Sleep(Backoff.DefaultBackoff); //wait for the update to finish
			}
			try
			{
				Process.Start(globals.AppPath + "CrawlWave.Client.exe");
				if (globals.Settings.LogLevel<=CWLogLevel.LogInfo)
				{
					globals.SystemLog.LogInfo("CrawlWave Client Scheduler started the Client :)");
				}
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel<=CWLogLevel.LogError)
				{
					globals.SystemLog.LogError("CrawlWave Client Scheduler failed to start the Client: "+e.ToString());
				}
			}
			finally
			{
				GC.Collect();
			}
		}

		/// <summary>
		/// Attempts to find the CrawlWave Client's version
		/// </summary>
		/// <returns>The client's <see cref="Version"/>.</returns>
		private Version GetClientVersion()
		{
			Version retVal = new Version(1,0,2004,1220);
			try
			{
				Assembly a = System.Reflection.Assembly.LoadFile(globals.AppPath + "CrawlWave.Client.exe");
				retVal = a.GetName().Version;
			}
			catch
			{
				//warn...
			}
			return retVal;
		}

		/// <summary>
		/// Performs the update of the Client's version. It queries an available server for
		/// the latest version and if a new version exists it goes on with the update.
		/// </summary>
		private void UpdateClient()
		{
			try
			{
				while(!InternetUtils.ConnectedToInternet())
				{
					Thread.Sleep(updateBackoff.Next());
				}
				//proxy = WebServiceProxy.Instance();
				proxy = CrawlWaveServerProxy.Instance(globals);


				string latest = String.Empty;
				SerializedException sx = proxy.SendLatestVersion(globals.Client_Info, out latest);
				if(sx!=null)
				{
					globals.SystemLog.LogError("CrawlWave Client Scheduler failed to retrieve the latest version of the Client:" + sx.Message);
					return;
				}

				Version latestVersion = new Version(latest);
				if(GetClientVersion()<latestVersion)
				{
					//we must update the client. First of all download the update.
					updating = true;
					byte [] buffer = new byte[0];
					sx = proxy.SendUpdatedVersion(globals.Client_Info, latest, out buffer);
					if(sx!=null || buffer.Length==0)
					{
						globals.SystemLog.LogError("CrawlWave Client Scheduler failed to retrieve the latest version of the Client: " + sx.Message);
						updating = false;
						return;
					}
					//save the compressed file to disk. If necessary launch the installer.
					string updateFileName = globals.AppPath + latest + ".zip";
					FileStream outputStream = new FileStream(updateFileName, FileMode.Create);
					outputStream.Write(buffer, 0, buffer.Length);
					outputStream.Close();

					string mustLaunchInstaller = ExtractUpdatedFiles(updateFileName);
					if(mustLaunchInstaller!=String.Empty)
					{
						//Launch Installer and exit
						Process.Start(mustLaunchInstaller);
					}
				}
			}
			catch
			{}
			finally
			{
				updating = false;
			}
		}

		/// <summary>
		/// Extracts the file. If a file that cannot be updated has been extracted it is
		/// saved as a filename with a .upd extension. The installer then replaces the
		/// original files.
		/// </summary>
		/// <param name="fileName">The name of the zip archive containing the updates.</param>
		/// <returns>The name of an installer executable, empty if none is found.</returns>
		private string ExtractUpdatedFiles(string fileName)
		{
			string retVal = String.Empty;
			try
			{
				ZipInputStream zs = new ZipInputStream(File.OpenRead(fileName));
				ZipEntry entry = null;
				while((entry = zs.GetNextEntry())!=null)
				{
					string updatedFileName = entry.Name;
					switch(updatedFileName)
					{
						case "CrawlWave.Common.dll":
						case "CrawlWave.Client.Common.dll":
						case "CrawlWave.Common.WSCFilter.dll":
						case "CrawlWave.Scheduler.exe":
							updatedFileName = globals.AppPath + updatedFileName + ".upd";
							break;

						default:
							updatedFileName = globals.AppPath + updatedFileName;
							break;
					}

					FileStream outStream = new FileStream(updatedFileName, FileMode.Create);
					int size = 4096;
					byte [] data = new byte[4096];
					while(true)
					{
						size = zs.Read(data, 0, data.Length);
						if (size > 0) 
						{
							outStream.Write(data, 0, size);
						}
						else 
						{
							break;
						}
					}
					outStream.Close();
					if(entry.Name.EndsWith(".exe"))
					{
						if((entry.Name!="CrawlWave.Client.exe")&&(entry.Name!="CrawlWave.Scheduler.exe"))
						{
							Assembly a = Assembly.LoadFile(updatedFileName);
							System.Type []types = a.GetTypes();
							foreach(Type t in types)
							{
								object [] attributes = t.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute),false);
								if((attributes != null) && (attributes.Length>0))
								{
									foreach (System.ComponentModel.DescriptionAttribute attribute in attributes)
									{
										if(attribute.Description == "CrawlWave Client Updater")
										{
											retVal = updatedFileName;
										}
									}
									break;
								}
							}
						}
					}
				}
				zs.Close();
			}
			catch(Exception e)
			{
				globals.SystemLog.LogWarning("ClientScheduler: Updated files extraction failed: " + e.ToString());
			}
			return retVal;
		}

		#endregion
	}
}
