using System;
using System.Management;
using CrawlWave.Common;

namespace CrawlWave.Client.Common
{
	/// <summary>
	/// Provides static methods for retrieving hardware information about the computer
	/// running the Client. It uses Windows Management Instrumentation (WMI) in order
	/// to obtain info about the system's CPU, RAM, HDD and Internet connection.
	/// </summary>
	public sealed class ComputerInfo
	{
		/// <summary>
		/// The constructor is private since no instances of this class need to be created.
		/// </summary>
		private ComputerInfo()
		{}

		/// <summary>
		/// Retrieve the CPU Type (Vendor + Model + Speed)
		/// </summary>
		/// <returns>A string containing the CPU Type</returns>
		private static string CPUType()
		{
			string retVal="";
			ManagementClass cim=new ManagementClass("Win32_Processor");
			ManagementObjectCollection moc=cim.GetInstances();
			foreach (ManagementObject mo in moc)
			{
				try
				{
					retVal=mo.Properties["Name"].Value.ToString();
					retVal+=" "+mo.Properties["CurrentClockSpeed"].Value.ToString()+"MHz";
				}
				catch(NullReferenceException)
				{
					continue;
				}
			}
			cim.Dispose();
			moc.Dispose();
			return retVal;
		}

		/// <summary>
		/// Retrieve the free disk space on disk c: (in MB)
		/// </summary>
		/// <returns>An integer indicating the free HDD space in MB</returns>
		private static int FreeDiskSpace()
		{
			int retVal=0;
			ManagementObject disk = new ManagementObject("Win32_LogicalDisk.DeviceID=\"c:\"");
			disk.Get();
			string size="0";
			try
			{
				size=disk["FreeSpace"].ToString();
			}
			catch
			{}
			disk.Dispose();
			retVal=(int)(Convert.ToInt64(size)/1048576);
			return retVal;
		}

		/// <summary>
		/// Retrieves the total RAM size in MB
		/// </summary>
		/// <returns>An integer indicating the total RAM size in MB</returns>
		private static int MemorySize()
		{
			int retVal=0;
			ManagementClass cim=new ManagementClass("Win32_PhysicalMemory");
			ManagementObjectCollection moc=cim.GetInstances();
			foreach (ManagementObject mo in moc)
			{
				try
				{
					string size=mo.Properties["Capacity"].Value.ToString();
					retVal+=(int)(Convert.ToInt32(size)/1048576);
				}
				catch(NullReferenceException)
				{
					continue;
				}
			}
			cim.Dispose();
			moc.Dispose();
			return retVal;
		}

		/// <summary>
		/// Retrieves the Internet Connection Speed as a <see cref="CWConnectionSpeed"/>
		/// enumeration member.
		/// </summary>
		/// <returns>A <see cref="CWConnectionSpeed"/> enumeration member indicating the
		/// system's internet connection speed.</returns>
		private static CWConnectionSpeed NetSpeed()
		{
			CWConnectionSpeed speed=CWConnectionSpeed.Unknown;
			int Kbps=0;
			ManagementClass cim=new ManagementClass("Win32_NetworkAdapter");
			ManagementObjectCollection moc=cim.GetInstances();
			foreach (ManagementObject mo in moc)
			{
				try
				{
					string bps=mo.Properties["MaxSpeed"].Value.ToString();
					Kbps=(int)(Convert.ToInt32(bps)/1024);
					if(Kbps>0) break;
				}
				catch(NullReferenceException)
				{
					continue;
				}
			}
			//clean-up memory
			cim.Dispose();
			moc.Dispose();
			//determine which enumeration value fits best
			if ((Kbps>0)&&(Kbps<=56))
				speed=CWConnectionSpeed.Modem56K;
			else if (Kbps==64)
				speed=CWConnectionSpeed.ISDN64K;
			else if(Kbps==128)
				speed=CWConnectionSpeed.ISDN128K;
			else if(Kbps==256)
				speed=CWConnectionSpeed.DSL256K;
			else if (Kbps==512)
				speed=CWConnectionSpeed.DSL512K;
			else if(Kbps==1024)
				speed=CWConnectionSpeed.DSL1M;
			else if((Kbps>1024)&&(Kbps<=1536))
				speed=CWConnectionSpeed.T1;
			else if ((Kbps>1536)&&(Kbps<=46080))
				speed=CWConnectionSpeed.T3;
			else if ((Kbps>46080)&&(Kbps<158720))
				speed=CWConnectionSpeed.Fiber;
			else if (Kbps==158720)
				speed=CWConnectionSpeed.ATM;
			else
				speed=CWConnectionSpeed.Unknown;
			return speed;
		}

		/// <summary>
		/// Gets hardware and internet connection information about a computer.
		/// </summary>
		/// <returns>A <see cref="CWComputerInfo"/> struct.</returns>
		public static CWComputerInfo GetComputerInfo()
		{
			CWComputerInfo info=new CWComputerInfo();
			info.CPUType=CPUType();
			info.RAMSize=MemorySize();
			info.HDDSpace=FreeDiskSpace();
			info.ConnectionSpeed=NetSpeed();
			return info;
		}

		/// <summary>
		/// Produces a hash code for a <see cref="CWComputerInfo"/> object not including
		/// the HDDSize value in order to make the detection of hardware changes easy.
		/// </summary>
		/// <param name="info">
		/// The <see cref="CWComputerInfo"/> whose hash code is to be calculated.
		/// </param>
		/// <returns>
		/// The lower part of the input's SHA1 Hash Code as a 64 bit unsigned long.
		/// </returns>
		public static ulong GetSHA1HashCode(CWComputerInfo info)
		{
			UInt64 hash=0;
			string hwinfo=info.CPUType + " " + info.RAMSize.ToString() + " " + info.ConnectionSpeed.ToString();
			byte [] hashcode=SHA1Hash.SHA1(System.Text.Encoding.GetEncoding(1253).GetBytes(hwinfo));
			for(int i=0; i<8; i++)
			{
				hash <<= 8;
				hash |= hashcode[hashcode.Length-i-1];
			}
			GC.Collect();
			return hash;
		}
	}
}
