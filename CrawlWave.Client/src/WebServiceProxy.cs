using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Web.Services;
using System.Xml;
//using Microsoft.Web.Services2;
//using Microsoft.Web.Services2.Security;
//using Microsoft.Web.Services2.Security.Tokens;
using CrawlWave.Common;
//using CrawlWave.Common.WSCFilter;

namespace CrawlWave.Client
{
	/// <summary>
	/// WebServiceProxy is a Singleton class that provides access to the CrawlWave Server
	/// Web Service while transparently taking care of issues such as WSE Username Tokens
	/// management and automatic switching to an alternate server in case the one currently
	/// used is not available.
	/// </summary>
	//public class WebServiceProxy
	//{
	//    #region Private variables

	//    private static WebServiceProxy instance;
	//    private ArrayList servers;
	//    private CrawlWaveServer server;
	//    private CrawlWaveServer serverwse;
	//    private int failures;
	//    private Globals globals;
	//    //private UsernameToken token;
	//    //private WSCFZipFilter zipFilter;
	//    //private WSCFUnzipFilter unzipFilter;

	//    #endregion

	//    #region Constructor and Singleton Instance Members

	//    /// <summary>
	//    /// The constructor is private so that only the class itself can create an instance.
	//    /// </summary>
	//    private WebServiceProxy()
	//    {
	//        //Get a reference to the global variables and application settings
	//        globals = Globals.Instance();
	//        //initialize the list of servers and connect to the default servers
	//        servers = new ArrayList();
	//        server = new CrawlWaveServer();
	//        serverwse = new CrawlWaveServer();
	//        failures = 0;
	//        //token = null;
	//        //zipFilter = new WSCFZipFilter();
	//        //unzipFilter = new WSCFUnzipFilter();
	//        //InitProxies();
	//    }

	//    /// <summary>
	//    /// Provides a global access point for the single instance of the <see cref="WebServiceProxy"/>
	//    /// class.
	//    /// </summary>
	//    /// <returns>A reference to the single instance of <see cref="WebServiceProxy"/>.</returns>
	//    public static WebServiceProxy Instance()
	//    {
	//        if (instance==null)
	//        {
	//            //Make sure the call is thread-safe.
	//            Mutex imutex=new Mutex();
	//            imutex.WaitOne();
	//            if( instance == null )
	//            {
	//                instance = new WebServiceProxy();
	//            }
	//            imutex.Close();
	//        }
	//        return instance;
	//    }

	//    #endregion

	//    #region Public properties

	//    /// <summary>
	//    /// Provides access to a CrawlWave Server proxy that doesn't use WSE 2.0 Security.
	//    /// Must only be used to check if a server is alive and to perform user registration
	//    /// </summary>
	//    public CrawlWaveServer Server
	//    {
	//        get { return server; }
	//    }

	//    /// <summary>
	//    /// Provides access to a CrawlWave Server proxy that uses WSE 2.0 Security
	//    /// </summary>
	//    public CrawlWaveServer SecureServer
	//    {
	//        get { return serverwse; }
	//    }

	//    #endregion

	//    #region Public methods

	//    /// <summary>
	//    /// Increments the proxy's failure count by 1. If the number of failures reaches a 
	//    /// certain threshold then the proxy attempts to switch to a different CrawlWave 
	//    /// Server and resets the internal failure count.
	//    /// </summary>
	//    public void IncrementFailures()
	//    {
	//        Interlocked.Increment(ref failures);
	//        if(failures > 50)
	//        {
	//            SwitchServer();
	//            failures = 0;
	//        }
	//    }

	//    /// <summary>
	//    /// Causes the <see cref="WebServiceProxy"/> to re-initialize the security tokens
	//    /// and Pipeline Filters related with the web service.
	//    /// </summary>
	//    public void ForceInitializeProxies()
	//    {
	//        //InitProxies();
	//    }

	//    #endregion

	//    #region Private methods

	//    /// <summary>
	//    /// Downloads the list of available servers.
	//    /// </summary>
	//    private void GetServers()
	//    {
	//        try
	//        {
	//            servers.Clear();
	//            DataSet ds = null;
	//            SerializedException sx = server.SendServers(globals.Client_Info, out ds);
	//            if(sx!=null)
	//            {
	//                sx.ThrowException();
	//            }
	//            if(ds!=null)
	//            {
	//                if(ds.Tables[0].Rows.Count>0)
	//                {
	//                    Version clientVersion = new Version(globals.Client_Info.Version);
	//                    foreach(DataRow dr in ds.Tables[0].Rows)
	//                    {
	//                        Version vs = new Version((string)dr[2]);
	//                        if(vs >= clientVersion)
	//                        {
	//                            servers.Add((string)dr[1]);
	//                        }
	//                    }
	//                }
	//                ds.Dispose();
	//            }
	//        }
	//        catch(Exception e)
	//        {
	//            if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
	//            {
	//                globals.FileLog.LogWarning("WebServiceProxy failed to get a list of servers: " + e.ToString());
	//            }
	//        }
	//    }

	//    /// <summary>
	//    /// Attemps to connect to a different CrawlWave Server and initialize the proxies.
	//    /// </summary>
	//    private void SwitchServer()
	//    {
	//        string oldUrl = server.Url;
	//        try
	//        {
	//            if(servers.Count >0)
	//            {
	//                Random r = new Random();
	//                bool isAlive = false;
	//                while(!isAlive)
	//                {
	//                    server.Url = (string)servers[r.Next(servers.Count)];
	//                    try
	//                    {
	//                        isAlive = server.IsAlive(); //this will throw a timeout exception if the server
	//                        //is unavailable
	//                    }
	//                    catch
	//                    {
	//                        continue;
	//                    }
	//                }
	//                serverwse.Url = server.Url;
	//                //InitProxies();
	//            }
	//        }
	//        catch(Exception e)
	//        {
	//            server.Url = oldUrl;
	//            serverwse.Url = oldUrl;
	//            if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
	//            {
	//                globals.FileLog.LogWarning("WebServiceProxy failed to switch server: " + e.ToString());
	//            }
	//        }
	//    }

	//    ///// <summary>
	//    ///// Inits the proxies by setting the appropriate SoapFilters and Security tokens
	//    ///// required for Web Services Enhancements 2.0 Security to work.
	//    ///// </summary>
	//    //private void InitProxies()
	//    //{
	//    //    try
	//    //    {
	//    //        serverwse.Pipeline.InputFilters.Add(unzipFilter);
	//    //        serverwse.Pipeline.OutputFilters.Add(zipFilter);
	//    //        /*Guid g = new Guid(globals.Settings.Password);
	//    //        token = new UsernameToken(globals.Settings.UserName, g.ToString(), PasswordOption.SendPlainText);
				
	//    //        serverwse.RequestSoapContext.Security.Tokens.Add( token );
	//    //        serverwse.RequestSoapContext.Security.Elements.Add( new MessageSignature( token ));*/
	//    //        // Set the TTL to 60 minutes to prevent replay attacks and avoid unsynchronized clock issues
	//    //        serverwse.RequestSoapContext.Security.Timestamp.TtlInSeconds = 86400;
	//    //        server.Timeout = 300000;
	//    //        serverwse.Timeout = 300000;
	//    //    }
	//    //    catch(Exception e)
	//    //    {
	//    //        if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
	//    //        {
	//    //            globals.FileLog.LogWarning("WebServiceProxy failed to init the proxies: " + e.ToString());
	//    //        }
	//    //    }
	//    //}

	//    #endregion
	//}
}
