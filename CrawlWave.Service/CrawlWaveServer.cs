using System;
using System.Collections.Generic;
using System.Text;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.Service
{
	public class CrawlWaveServer : MarshalByRefObject, ICrawlWaveServer
	{
		private ServerEngine engine;

		public CrawlWaveServer()
		{
			engine = new ServerEngine();
		}

		#region Remoting related methods

		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		#region IServer Members

		public bool IsAlive()
		{
			return true;
		}

		public SerializedException GetClientComputerInfo(ClientInfo ci, CWComputerInfo info)
		{
			return engine.StoreNewClientComputerInfo(ci, info);
		}

		public SerializedException GetCrawlResults(ClientInfo ci, UrlCrawlData[] data)
		{
			return engine.StoreCrawlResults(ci, data);
		}

		public SerializedException RegisterClient(ref ClientInfo ci, CWComputerInfo info)
		{
			return engine.StoreClientRegistrationInfo(ref ci, info);
		}

		public SerializedException RegisterUser(ref int ID, string username, byte[] password, string email)
		{
			return engine.StoreUserRegistrationInfo(ref ID, username, password, email);
		}

		public SerializedException SendBannedHosts(ClientInfo ci, out System.Data.DataSet data)
		{
			data = null;
			return engine.SelectBannedHosts(ci, ref data);
		}

		public SerializedException SendLatestVersion(ClientInfo ci, out string version)
		{
			version = string.Empty;
			return engine.SelectLatestVersion(ci, ref version);
		}

		public SerializedException SendServers(ClientInfo ci, out System.Data.DataSet data)
		{
			data = null;
			return engine.SelectServers(ci, ref data);
		}

		public SerializedException SendUpdatedVersion(ClientInfo ci, string version, out byte[] data)
		{
			data = null;
			return engine.SelectUpdatedVersion(ci, version, data);
		}

		public SerializedException SendUrlsToCrawl(ClientInfo ci, out InternetUrlToCrawl[] data)
		{
			data = null;
			engine.LogClientAction(ci, CWClientActions.LogSendUrlsToCrawl);
			return engine.SelectUrlsToCrawl(ci, ref data);
		}

		public SerializedException SendUserStatistics(ClientInfo ci, out UserStatistics stats)
		{
			stats = new UserStatistics();
			engine.LogClientAction(ci, CWClientActions.LogSendUserStatistics);
			return engine.SelectUserStatistics(ci, ref stats);
		}

		#endregion
	}
}
