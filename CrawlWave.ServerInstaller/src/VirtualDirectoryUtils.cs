using System;
using System.Collections;
using System.DirectoryServices;
using System.Globalization;
using System.IO;

namespace CrawlWave.ServerInstaller
{
	/// <summary>
	/// VirtualDirectoryHelper creates or modifies a virtual directory of a web site hosted
	/// on Internet Information Server.
	/// </summary>
	public class VirtualDirectoryHelper
	{
		#region Private variables

		private string serverName;
		private DirectoryEntry iisServer;
		private IISVersion iisVersion;
		private static string VirDirSchemaName = "IIsWebVirtualDir";

		#endregion

		#region Public properties

		/// <summary>
		/// Gets or sets the Server Name
		/// </summary>
		public string ServerName
		{
			get { return serverName; }
			set { serverName = value;}
		}

		#endregion

		#region Protected members

		/// <summary>
		/// Defines the IIS versions supported by the class.
		/// </summary>
		protected enum IISVersion 
		{
			/// <summary>
			/// Unknown version or no IIS installed
			/// </summary>
			None,
			/// <summary>
			/// NT 4.0
			/// </summary>
			Four,
			/// <summary>
			/// Win 2K/XP
			/// </summary>
			Five,
			/// <summary>
			/// Win 2003
			/// </summary>
			Six
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new instance of the <see cref="VirtualDirectoryHelper"/> class.
		/// </summary>
		public VirtualDirectoryHelper()
		{
			serverName = "localhost";
			iisVersion = IISVersion.None;
			FindIISVersion();
		}

		/// <summary>
		/// Constructs a new instance of the <see cref="VirtualDirectoryHelper"/> class.
		/// </summary>
		/// <param name="serverName">The server to connect to.</param>
		public VirtualDirectoryHelper(string serverName)
		{
			if(serverName == String.Empty)
			{
				throw new ArgumentNullException("serverName");
			}
			serverName = serverName;
			iisVersion = IISVersion.None;
			FindIISVersion();
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Gets the version of IIS corresponding with the current OS.
		/// </summary>
		private void FindIISVersion()
		{
			if (iisVersion == IISVersion.None) 
			{
				Version osVersion = Environment.OSVersion.Version;
				if (osVersion.Major < 5) 
				{
					// Win NT 4 kernel -> IIS4
					iisVersion = IISVersion.Four;
				} 
				else 
				{
					switch (osVersion.Minor) 
					{
						case 0:
							// Win 2000 kernel -> IIS5
							iisVersion = IISVersion.Five;
							break;
						case 1:
							// Win XP kernel -> IIS5
							iisVersion = IISVersion.Five;
							break;
						case 2:
							// Win 2003 kernel -> IIS6
							iisVersion = IISVersion.Six;
							break;
					}
				}
			}
		}

		private void CheckIISSettings() 
		{
			if (!DirectoryEntryExists(string.Format(CultureInfo.InvariantCulture, "IIS://{0}/W3SVC", serverName))) 
			{
				throw new ApplicationException("The webservice at " + serverName + " does not exist or is not reachable.");
			}
		}

		private bool DirectoryEntryExists(string path) 
		{
			DirectoryEntry entry = new DirectoryEntry(path);
			try 
			{
				//trigger the *private* entry.Bind() method
				object adsobject = entry.NativeObject;
				return true;
			} 
			catch 
			{
				return false;
			} 
			finally 
			{
				entry.Dispose();
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		///	Connects to IISServer
		/// </summary>
		public void Connect()
		{
			try
			{
				iisServer = new DirectoryEntry("IIS://" + serverName + "/W3SVC/1");
			}
			catch (Exception e)
			{
				throw new Exception("Could not connect to: " + serverName, e);
			}
		}

		/// <summary>
		/// Creates a new virtual directory on IIS Server.
		/// </summary>
		/// <param name="dirName">The name of the directory to create.</param>
		/// <param name="appName">The name of the application to create.</param>
		/// <param name="physicalPath">The physical path of the virtual directory.</param>
		public void CreateVirtualDirectory(string dirName, string appName, string physicalPath)
		{
			//first attempt to build it using EnterpriseServices
			try
			{
				System.EnterpriseServices.Internal.IISVirtualRoot vr = new System.EnterpriseServices.Internal.IISVirtualRoot();
				string sError;
				vr.Create("IIS://" + serverName + "/W3SVC/1/Root", physicalPath, dirName, out sError);
				if(sError==String.Empty)
				{
					//everything went ok, so we can return
					return;
				}
			}
			catch
			{}
			//it hasn't succeeded so we must try to create it using DirectoryServices
			DirectoryEntry folderRoot = iisServer.Children.Find("Root",VirDirSchemaName);
			try
			{
				DirectoryEntry newVirDir = folderRoot.Children.Add(dirName,VirDirSchemaName);
				// Set Properties
				newVirDir.Properties["AccessRead"].Add(true);
				newVirDir.Properties["Path"].Add(physicalPath);
				// Create an Application
				//newVirDir.Invoke("AppCreate",true);
				newVirDir.Properties["AppFriendlyName"][0] = appName;
				newVirDir.Properties["FrontPageWeb"][0] = 1;
				if(iisVersion == IISVersion.Four)
				{
					newVirDir.Invoke("AppCreate", true);
				}
				else
				{
					newVirDir.Invoke("AppCreate2", true);
				}
				// Save Changes
				newVirDir.CommitChanges();
				folderRoot.CommitChanges();
				iisServer.CommitChanges();
			}
			catch (Exception e)
			{
				throw new Exception("Virtual Directory " + dirName + " Already Exists",e);
			}
		}

		/// <summary>
		/// Deletes a virtual directory from IIS Server.
		/// </summary>
		/// <param name="dirName">The name of the directory to delete.</param>
		/// <param name="physicalPath">The physical path of the virtual directory.</param>
		public void DeleteVirtualDirectory(string dirName, string physicalPath)
		{
			//first attempt to delete it using EnterpriseServices
			try
			{
				System.EnterpriseServices.Internal.IISVirtualRoot vr = new System.EnterpriseServices.Internal.IISVirtualRoot();
				string sError;
				vr.Delete("IIS://" + serverName + "/W3SVC/1/Root", physicalPath, dirName, out sError);
				if(sError==String.Empty)
				{
					//everything went ok, so we can return
					return;
				}
			}
			catch
			{}
			//it hasn't succeeded so we must try to delete it using DirectoryServices
			DirectoryEntry folderRoot = iisServer.Children.Find("Root",VirDirSchemaName);
			try
			{
				DirectoryEntry virDir = folderRoot.Children.Find(dirName,VirDirSchemaName);
				if(virDir != null)
				{
					folderRoot.Children.Remove(virDir);
				}
				folderRoot.CommitChanges();
				iisServer.CommitChanges();
			}
			catch (Exception e)
			{
				throw new Exception("Virtual Directory " + dirName + " does not exist or could not be deleted",e);
			}
		}

		#endregion
	}
}
