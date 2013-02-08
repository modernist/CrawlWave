/*
 * This file implements the ftp:// protocol using the Pluggable protocol feature
 * of System.Net namespace
 */

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace CrawlWave.Common
{
	/// <summary>
	/// Defines constants for the different types of FTP commands supported by the
	/// <see cref="FtpWebRequest"/> class.
	/// </summary>
	public enum FtpCommandType
	{
		/// <summary>
		/// Commands starting with 1 are Control Commands
		/// </summary>
		FtpControlCommand	   = 1,
		/// <summary>
		/// Commands starting with 2 are Data receive related commands
		/// </summary>
		FtpDataReceiveCommand  = 2,
		/// <summary>
		/// Commands starting with 3 are Data send related commands
		/// </summary>
		FtpDataSendCommand	   = 3,
		/// <summary>
		/// Commands unsupported by the server return an error code starting with 4
		/// </summary>
		FtpCommandNotSupported = 4,
	}

	/// <summary>
	/// Contains simple methods that allow easy detection of the connection status, based
	/// on the description of the server's response.
	/// </summary>
	public class ResponseDescription 
	{
		private int m_dwStatus;
		private string m_szStatusDescription;

		/// <summary>
		/// Gets or sets the response's status code
		/// </summary>
		public int Status 
		{
			get { return m_dwStatus; }
			set { m_dwStatus = value; }
		}
		/// <summary>
		/// Gets ot sets the response's status description
		/// </summary>
		public string StatusDescription 
		{
			get { return m_szStatusDescription; }
			set { m_szStatusDescription = value; }
		}
		/// <summary>
		/// Gets a <see cref="Boolean"/> value indicating whether the status code begins 
		/// with 1, which means it's a positive preliminary response.
		/// </summary>
		public bool PositivePreliminary 
		{
			get { return ( m_dwStatus / 100 == 1); }
		}
		/// <summary>
		/// Gets a <see cref="Boolean"/> value indicating whether the status code begins 
		/// with 2, which means it's a positive completion response.
		/// </summary>
		public bool PositiveCompletion 
		{
			get { return ( m_dwStatus / 100 == 2); }
		}
		/// <summary>
		/// Gets a <see cref="Boolean"/> value indicating whether the status code begins 
		/// with 3, which means it's a positive intermediate response.
		/// </summary>
		public bool PositiveIntermediate 
		{
			get { return ( m_dwStatus / 100 == 3); }
		}
		/// <summary>
		/// Gets a <see cref="Boolean"/> value indicating whether the status code begins 
		/// with 4, which means it's a transient negative response.
		/// </summary>
		public bool TransientNegativeCompletion 
		{
			get { return ( m_dwStatus / 100 == 4); }
		}
		/// <summary>
		/// Gets a <see cref="Boolean"/> value indicating whether the status code begins 
		/// with 5, which means it's a permanent negative response.
		/// </summary>
		public bool PermanentNegativeCompletion 
		{
			get { return ( m_dwStatus / 100 == 5); }
		}
	}

	/// <summary>
	/// Provides an implementation of the <see cref="IWebRequestCreate"/> interface, thus
	/// allowing the creation of <see cref="FtpWebRequest"/> class instances.
	/// </summary>
	public class FtpRequestCreator : IWebRequestCreate
	{
		/// <summary>
		/// Constructs an <see cref="FtpRequestCreator"/> instance.
		/// </summary>
		public FtpRequestCreator()
		{}
    
		/// <summary>
		/// Creates an <see cref="FtpWebRequest"/> instance from a given <see cref="Uri"/>
		/// </summary>
		/// <param name="Url">The resource that the <see cref="FtpWebRequest"/> refers to.</param>
		/// <returns>A <see cref="WebRequest"/> object for the given Url</returns>
		public WebRequest Create(Uri Url)
		{
			return new FtpWebRequest(Url);
		}
	}

	/// <summary>
	/// Provides an implementation of a <see cref="Stream"/> that is used for tranferring
	/// files over the FTP protocol.
	/// </summary>
	internal class FtpStream: Stream 
	{
		private Stream m_Stream;
		private bool m_fCanRead;
		private bool m_fCanWrite;
		private bool m_fCanSeek;
		private bool m_fClosedByUser;

		internal FtpStream() 
		{
			m_Stream = null;
		}

		internal FtpStream(Stream stream, bool canread, bool canwrite, bool canseek) 
		{
			m_Stream = stream;
			m_fCanRead = canread;
			m_fCanWrite = canwrite;
			m_fCanSeek = canseek;
			m_fClosedByUser = false;
		}

		public override bool CanRead 
		{
			get { return m_fCanRead; }
		}

		public override bool CanWrite 
		{
			get { return m_fCanWrite; }
		}

		public override bool CanSeek 
		{
			get { return m_fCanSeek; }
		}

		public override long Length 
		{
			get 
			{
				throw new NotSupportedException("This stream cannot be seeked");
			}
		}

		public override long Position 
		{
			get 
			{
				throw new NotSupportedException("This stream cannot be seeked");
			}
			set 
			{
				throw new NotSupportedException("This stream cannot be seeked");
			}
		}

		public override long Seek(long offset, SeekOrigin origin) 
		{
			throw new NotSupportedException("This stream cannot be seeked");
		}

		public override void Flush() 
		{}

		public override void SetLength(long value) 
		{
			throw new NotSupportedException("This stream cannot be seeked");
		}
        
		public override void Close() 
		{
			m_fClosedByUser = true;
		}

		public override void Write(Byte [] buffer, int offset, int length) 
		{
			if(m_fClosedByUser)
			{
				throw new IOException("Cannot operate on a closed stream");
			}
			InternalWrite(buffer,offset,length);
		}

		internal void InternalWrite(Byte [] buffer, int offset, int length) 
		{
			m_Stream.Write(buffer,offset,length);
		}

		public override int Read(Byte [] buffer, int offset, int length) 
		{
			if(m_fClosedByUser)
			{
				throw new IOException("Cannot operate on a closed stream");
			}
			return InternalRead(buffer,offset,length);
		}
	
		internal int InternalRead(Byte [] buffer, int offset, int length) 
		{
			return m_Stream.Read(buffer,offset,length);
		}
	
		internal long InternalPosition 
		{
			get { return m_Stream.Position;	}
			set { m_Stream.Position = value;}
		}

		internal long InternalLength 
		{
			get { return m_Stream.Length; }
		}

		internal void InternalClose() 
		{
			m_Stream.Close();
		}
	
		internal Stream GetStream() 
		{
			return m_Stream;
		}
	}

	/// <summary>
	/// Provides an implementation of <see cref="WebResponse"/> that is used for transferring
	/// using the File Transfer Protocol. Provides pluggable FTP protocol support for the
	/// <see cref="System.Net"/> namespace.
	/// </summary>
	public class FtpWebResponse : WebResponse
	{
		// Properties overriding WebResponse
		private int 	_StatusCode;
		private String 	_StatusDescription;
		private String 	_ContentType ;
		private String  _Log;
		private Stream 	_ResponseStream ;	

		internal FtpWebResponse()
		{
			_StatusCode = -1;
			_StatusDescription = null;
			_ContentType = null;
			_ResponseStream = null;
			_Log = null;
		}
	
		internal FtpWebResponse(int StatusCode, string StatusDescription, String Log) 
		{
			_StatusCode = StatusCode;
			_StatusDescription = StatusDescription;
			_Log = Log;

			_ResponseStream = Stream.Null;
		}
	
		/// <summary>
		/// Gets a <see cref="String"/> containing information about the content type of
		/// the file being tranferred.
		/// </summary>
		public override String ContentType 
		{
			get	{ return _ContentType; } 
			set { throw new NotSupportedException("This property cannot be set"); }
		}
	
		/// <summary>
		/// Provides access to the Stream related with the FTP file transfer.
		/// </summary>
		/// <returns>A <see cref="Stream"/> with the contents of the requested resource.</returns>
		public override Stream GetResponseStream()
		{
			if(_ResponseStream == null)
			{
				throw new ApplicationException("No response stream for this kind of method");
			}
			return _ResponseStream;	
		}	
	
		/// <summary>
		/// Gets the status code for the FTP connection.
		/// </summary>
		public int Status	
		{
			get { return _StatusCode; }
		}	
	
		/// <summary>
		/// Gets a <see cref="String"/> with the description of the connection's status.
		/// </summary>
		public String StatusDescription 
		{
			get { return _StatusDescription; }
		}	

		/// <summary>
		/// Gets a <see cref="String"/> containing the entire transaction log of an FTP session.
		/// </summary>
		public string TransactionLog 
		{
			get { return _Log; }
		}
	
		internal void SetDownloadStream(Stream datastream)
		{
			_ResponseStream = datastream;		
			_ResponseStream.Position = 0;
		}
	}

	/// <summary>
	/// This is a descendant of <see cref="WebRequest"/> and acts as the entry point for
	/// all ftp:// requests.
	/// </summary>
	public class FtpWebRequest : WebRequest
	{
		private const int SOCKET_ERROR = -1;
		private String m_szCmdParameter;	
		private Socket m_DataSocket;
		private Socket m_ControlSocket;		
		private Uri m_RequestUri;
		private Uri m_ProxyUri;
		private Stream m_RequestStream;
		private ICredentials m_Credentials;
		private IWebProxy	m_Proxy;
#if RTM
    private UriBuilder m_ServicePoint;
#else
		private ServicePoint m_ServicePoint;
#endif
		private bool _bPassiveMode;
		private long m_dwdwContentLength;
		private string m_szContentType;
		private StringBuilder m_sbControlSocketLog;
		private Exception m_Exception;
		private string m_szMethod;
		private String m_szServerMethod;		
		private FtpCommandType m_CommandType;

		// various timeouts
		private int m_CommandSendTimeout;
		private int m_DataReceiveTimeout;
		private int m_ConnectTimeout;
		private int m_DataSendTimeout;
    
		/// <summary>
		/// Gets or sets a flag indicating whether Passive mode will be used for the FTP connection.
		/// </summary>
		public bool Passive 
		{
			set { _bPassiveMode = value; }
			get { return _bPassiveMode; }
		}

		/// <summary>
		/// Gets or sets the FTP Method command that will be used for the request, like
		/// GET, PUT or LIST.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if the value provided is a null string.</exception>
		/// <exception cref="NotSupportedException">Thrown if the specified command is not supported.</exception>
		public override String Method
		{	
			get { return m_szMethod; }
			set 
			{
				if(value == null)
				{
					throw new ArgumentNullException("Method");
				}
				m_szServerMethod = GetServerCommand(value);
				m_CommandType = FindCommandType(m_szServerMethod);
				if(m_CommandType == FtpCommandType.FtpCommandNotSupported )
				{
					throw new NotSupportedException(value + " is not supported");
				}
				m_szMethod=value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="ICredentials"/> related with the FTP request.
		/// </summary>
		public override ICredentials Credentials 
		{
			get { return m_Credentials; }
			set { m_Credentials = value; }
		}

		/// <summary>
		/// ConnectionGroupName property is not supported.
		/// </summary>
		public override string ConnectionGroupName 
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets or sets the length of the resource related with the <see cref="FtpWebRequest"/>.
		/// </summary>
		public override long ContentLength 
		{
			get { return m_dwdwContentLength; }
			set { m_dwdwContentLength = value; }
		}

		/// <summary>
		/// Gets or sets a <see cref="String"/> describing the Content type of the resource
		/// related to the <see cref="FtpWebRequest"/>.
		/// </summary>
		public override string ContentType 
		{
			/* override */ get { return m_szContentType; }
			/* override */ set { m_szContentType = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IWebProxy"/> used for the <see cref="FtpWebRequest"/>
		/// connection.
		/// </summary>
		public override IWebProxy Proxy 
		{
			get { return m_Proxy; }
			set { m_Proxy = value;}
		}

		/// <summary>
		/// Gets or sets the number of milliseconds before a Data Send Timeout occurs.
		/// </summary>
		public int DataSendTimeout 
		{
			set { m_DataSendTimeout = value; }
			get { return m_DataSendTimeout; }
		}

		/// <summary>
		/// Gets or sets the number of milliseconds before a Data Receive Timeout occurs.
		/// </summary>
		public int DataReceiveTimeout
		{
			set { m_DataReceiveTimeout = value; }
			get { return m_DataReceiveTimeout; }
		}

		/// <summary>
		/// Gets or sets the number of milliseconds before a Connect Timeout occurs.
		/// </summary>
		public int ConnectTimeout 
		{
			set { m_ConnectTimeout = value; }
			get { return m_ConnectTimeout; }
		}

		/// <summary>
		/// Gets or sets the number of milliseconds before a Command Timeout occurs.
		/// </summary>
		public int CommandTimeout 
		{
			set { m_CommandSendTimeout = value; }
			get { return m_CommandSendTimeout; }
		}
    
		/// <summary>
		/// Provides access to the <see cref="Stream"/> related to the FTP request.
		/// </summary>
		/// <returns>A <see cref="Stream"/> containing the request's data.</returns>
		public override Stream GetRequestStream()
		{	
			if(m_CommandType != FtpCommandType.FtpDataSendCommand)
				throw new InvalidOperationException("cant upload data with this method type");

			if(	m_RequestStream == null)
				m_RequestStream = new FtpStream(new MemoryStream(), false, true, false);	
			else
				throw new InvalidOperationException("request stream already retrieved");
			
			return m_RequestStream;		
		}
	
		/// <summary>
		/// Constructs a new <see cref="FtpWebRequest"/> instance that points to a given Uri.
		/// </summary>
		/// <param name="Url">The <see cref="Uri"/> of the requested file.</param>
		public FtpWebRequest(Uri Url)
		{
			// do whatever initialization is required here
			if(Url.Scheme != "ftp")  // This class is only for ftp urls
				throw new NotSupportedException("This protocol is not supported");

			Trace.WriteLine("FtpWebRequest::ctor(" + Url.AbsoluteUri + ")");
		
			m_RequestUri = Url;
			m_szMethod = "dir";  // default is to retrieve a file
			_bPassiveMode = false;

			m_szCmdParameter = Url.AbsolutePath; // default is the path of url is path of file to retrieve

			m_sbControlSocketLog = new StringBuilder();
#if RTM
        m_ServicePoint = new UriBuilder(Url);
#endif        
			m_CommandSendTimeout = 60000;
			m_DataReceiveTimeout = 60000;
			m_ConnectTimeout = 60000;
			m_DataSendTimeout = 60000;
		}

		/// <summary>
		/// Sends the request to the FTP server and returns the response.
		/// </summary>
		/// <returns>A <see cref="WebResponse"/> containing the FTP server's response.</returns>
		public override WebResponse GetResponse() 
		{
			// 1) Login to the FTP server
			String user = "anonymous";
			String pass = "User@";

			// going through proxy ?
			if(m_Proxy != null) 
			{
				// get the proxy uri
				m_ProxyUri = GetProxyUri();

				if(m_ProxyUri != null) 
				{
					if(m_Proxy.Credentials != null) 
					{
						NetworkCredential cred = m_Proxy.Credentials.GetCredential(m_ProxyUri,null);

						user = cred.UserName;
						pass = cred.Password;

						if((user == null) || (user == ""))
						{
							user = "anonymous";
						}
						if((pass == null) || (pass == ""))
						{
							pass = "User@";
						}
					} 
					else
					{
						if(m_Credentials != null) 
						{
							NetworkCredential cred = m_Credentials.GetCredential(m_RequestUri,null);
    				
							if(cred != null) 
							{
								user = cred.UserName;
								pass = cred.Password;
							}

							if((user == null) || (user == ""))
							{
								user = "anonymous";
							}
							if((pass == null) || (pass == ""))
							{
								pass = "User@";
							}
						}
					}
					user = user + "@" + m_RequestUri.Host.ToString();
				} 
#if RTM
            Uri u = m_Proxy.GetProxy(m_RequestUri);
            m_ServicePoint.Host = u.Host;
            m_ServicePoint.Port = u.Port;
#else
				m_ServicePoint = ServicePointManager.FindServicePoint( m_RequestUri, m_Proxy);
#endif			
			} 
			else 
			{
				// we come here if no proxy was specified in WebRequest,
				// or proxy was not needed for this uri
#if RTM
            m_ServicePoint.Host = m_RequestUri.Host;
            m_ServicePoint.Port = m_RequestUri.Port;

#else
				m_ServicePoint = ServicePointManager.FindServicePoint( m_RequestUri,null);
#endif

				if(m_Credentials != null) 
				{
					NetworkCredential cred = m_Credentials.GetCredential(m_RequestUri,null);
				
					user = cred.UserName;
					pass = cred.Password;

					if((user == null) || (user == ""))
					{
						user = "anonymous";
					}
					if((pass == null) || (pass == ""))
					{
						pass = "User@";
					}
				}
			}

			if(!DoLogin(user,pass)) 
			{
				throw new ApplicationException("Login Failed\nServer Log:\n" + m_sbControlSocketLog.ToString());
			}

			return (WebResponse)GetFtpResponse();
		}
	
		private WebResponse GetFtpResponse()
		{
			FtpWebResponse ftpresponse = null;			
		
			if(m_CommandType == FtpCommandType.FtpDataReceiveCommand 
				|| m_CommandType == FtpCommandType.FtpDataSendCommand)
			{
				if(_bPassiveMode)
				{
					OpenPassiveDataConnection();								
				}
				else
				{
					OpenDataConnection();									
				}
			}
			// negotiate data connection
			string sztype = "I";
			if(m_szContentType == "ascii") 
			{
				sztype = "A";
			}

			SendCommand("TYPE",sztype);
			ResponseDescription resp_desc = ReceiveCommandResponse();

			if(!resp_desc.PositiveCompletion) 
			{
				throw new ApplicationException("Data negotiation failed:\n" + m_sbControlSocketLog.ToString());
			}
		
			if(m_szServerMethod == "PWD")
				m_szCmdParameter = null;

			SendCommand(m_szServerMethod, m_szCmdParameter);
			//ftpresponse = ReceiveResponse();
			resp_desc = ReceiveCommandResponse();
		
			if(m_CommandType == FtpCommandType.FtpDataSendCommand)
			{				
				//if(resp_desc.Status/100 == 1) // Positive preliminary reply
				if(resp_desc.PositivePreliminary) // Positive preliminary reply
				{
					if(m_RequestStream != null)
					{
						Socket DataConnection;
						if( _bPassiveMode)
							DataConnection = m_DataSocket;							
						else
							DataConnection = m_DataSocket.Accept();								
						if(DataConnection == null)
						{
							throw new ProtocolViolationException("Accept failed ");						
						}
					
						SendData(DataConnection);							
						DataConnection.Close();										

						//ftpresponse = ReceiveResponse();
						ResponseDescription resp = ReceiveCommandResponse();
						ftpresponse = new FtpWebResponse(resp.Status, resp.StatusDescription, m_sbControlSocketLog.ToString());
					}
					else
					{  // Data to be send is not specified
						throw new ApplicationException("Data to be uploaded not specified");
					}					
				}
				else
				{
					//Console.WriteLine(resp_desc.StatusDescription);	
					m_Exception = new ApplicationException(ComposeExceptionMessage(resp_desc, m_sbControlSocketLog.ToString()));
				}
				CloseDataConnection();
			}
			else if(m_CommandType == FtpCommandType.FtpDataReceiveCommand)
			{					
				//if(resp_desc.Status/100 == 1) // Positive preliminary reply
				if(resp_desc.PositivePreliminary) // Positive preliminary reply
				{
					Socket DataConnection;
					if( _bPassiveMode)
						DataConnection = m_DataSocket;							
					else
						DataConnection = m_DataSocket.Accept();											
					if(DataConnection == null)
					{
						throw new ProtocolViolationException("DataConnection failed ");
					}					
					Stream datastream = ReceiveData(DataConnection);								
					//ftpresponse = ReceiveResponse();
					ResponseDescription resp = ReceiveCommandResponse();
					ftpresponse = new FtpWebResponse(resp.Status, resp.StatusDescription, m_sbControlSocketLog.ToString());
					ftpresponse.SetDownloadStream(datastream);
				} 
				else 
				{
					m_Exception = new ApplicationException(ComposeExceptionMessage(resp_desc, m_sbControlSocketLog.ToString()));
				}
				CloseDataConnection();
			} 
			else 
			{
				// this is a FtpControlCommand
				ftpresponse = new FtpWebResponse(resp_desc.Status, resp_desc.StatusDescription, m_sbControlSocketLog.ToString());
			}

			if(m_Exception != null) 
			{
				Debug.Assert(ftpresponse == null);
				throw m_Exception;
			}
		
			return ftpresponse;
		}	

		private bool DoLogin(String UserID, String Password)
		{
			ResponseDescription resp;
#if RTM
		OpenControlConnection(m_ServicePoint.Uri);
#else
			OpenControlConnection(m_ServicePoint.Address);
#endif
			SendCommand("USER" , UserID);
			resp = ReceiveCommandResponse();
		
			if(resp.Status == 331) // password required
			{
				SendCommand("PASS", Password);
			} 
			else
			{
				return false;
			}
		
			resp = ReceiveCommandResponse();
			if(resp.Status == 230)
			{
				return true;
			}

			return false;	
		}

		private void OpenDataConnection()
		{
			if( m_DataSocket != null) 
			{
				// Throw some exception here because passed socket is already in use
				throw new ApplicationException("Data socket is already open.");
			}
			m_DataSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			m_DataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, m_ConnectTimeout);
			m_DataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, m_DataReceiveTimeout);

			IPHostEntry localHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        
			//IPEndPoint	epListener = new IPEndPoint(localHostEntry.AddressList[0], IPAddress.Any);		
			IPEndPoint	epListener = new IPEndPoint(localHostEntry.AddressList[0], 0);		
		        
			m_DataSocket.Bind(epListener);
			m_DataSocket.Listen(5); // Put the dataSocket * & in Listen mode

			//m_DataSocket.Connect(epListener);

			IPEndPoint localEP = (IPEndPoint) m_DataSocket.LocalEndPoint;
			UInt32 localEPAddress = (UInt32)localEP.Address.Address;
			String szLocal = FormatAddress(localEPAddress, localEP.Port);						

			SendCommand("PORT",szLocal);                
			ResponseDescription resp_desc = ReceiveCommandResponse();
			//Console.WriteLine(resp_desc.StatusDescription + szLocal);
			if( !resp_desc.PositiveCompletion )
			{
				// throw some exception here
				throw new ApplicationException("Couldnt open data connection\n" + ComposeExceptionMessage(resp_desc, m_sbControlSocketLog.ToString()));
			}
		}

		private void OpenPassiveDataConnection()
		{
			Trace.WriteLine("Opening Passive Data Connection");
	    
			if( m_DataSocket != null) 
			{ // Through some exception here because passed socket is already in use
				throw new ProtocolViolationException();
			}
			String IPAddressStr =null;
			int Port = 0 ;
		
			SendCommand("PASV","");                
			//FtpWebResponse response = ReceiveResponse();
			ResponseDescription resp_desc = ReceiveCommandResponse();
		
			if( !resp_desc.PositiveCompletion )
			{
				// throw some exception here
				throw new ApplicationException("Couldnt open passive data connection\n" + ComposeExceptionMessage(resp_desc, m_sbControlSocketLog.ToString()));
			}
		
			//Console.WriteLine(resp_desc.StatusDescription);
			//Find the IPAddress and port address from response
			IPAddressStr = getIPAddress(resp_desc.StatusDescription);
			Port = getPort(resp_desc.StatusDescription);

			Trace.WriteLine("Creating passive data socket");
			m_DataSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );       		
			if( m_DataSocket == null )
			{
				throw new ProtocolViolationException("Error in creating Data Socket");	
			} 					

			m_DataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, m_ConnectTimeout);
			m_DataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, m_DataReceiveTimeout);
        
			//IPHostEntry serverHostEntry = Dns.GetHostByName(m_RequestUri.Host);		
			//IPEndPoint	serverEndPoint = new IPEndPoint(serverHostEntry.AddressList[0], Port);        		
			//IPEndPoint	serverEndPoint = new IPEndPoint(IPAddress.Parse(IPAddressStr), Port); 		

			IPEndPoint	serverEndPoint = new IPEndPoint(IPAddress.Parse(IPAddressStr), Port);        		
			Trace.WriteLine("Passive Socket connecting to: " + IPAddressStr + " Port: " + Port);

			try 
			{
				m_DataSocket.Connect(serverEndPoint);
			} 
			catch
			{
				m_DataSocket.Close();
				m_DataSocket = null;				
				//throw new ProtocolViolationException("Passive connection failure");   
				throw;
			}		
			return;
		}

		private Uri GetProxyUri() 
		{
			// going through a proxy ?
			Uri u = null;
			if(m_Proxy != null && !m_Proxy.IsBypassed(m_RequestUri)) 
			{
				u = m_Proxy.GetProxy(m_RequestUri);
			}
			return u;
		}

		private void OpenControlConnection(Uri uriToConnect)
		{
			String Host = uriToConnect.Host;
			int Port = uriToConnect.Port;

			if(m_ControlSocket != null) // socketalready in use
			{
				throw new ProtocolViolationException("Control connection already in use");            
			}

			m_ControlSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			EndPoint clientIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
			EndPoint clientEndPoint = clientIPEndPoint;

			m_ControlSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, m_ConnectTimeout);
			m_ControlSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, m_DataReceiveTimeout);

			try 
			{
				m_ControlSocket.Bind(clientEndPoint);
			} 
			catch(Exception e) 
			{
				m_ControlSocket.Close();
				m_ControlSocket = null;			
				throw new ApplicationException(" Error in opening control connection",e);            
			}
        
			clientEndPoint = m_ControlSocket.LocalEndPoint;
			try
			{
				IPHostEntry serverHostEntry = Dns.GetHostEntry(Host);
				IPEndPoint serverEndPoint = new IPEndPoint( serverHostEntry.AddressList[0], Port );
				try 
				{
					m_ControlSocket.Connect(serverEndPoint); 
				} 
				catch 
				{  // throw some more suitable exception here 
					m_ControlSocket.Close();
					m_ControlSocket = null;
					throw; //new ApplicationException("Error in creating control connection",e);
				}  	
			}
			catch
			{
				//m_ControlSocket.Close();
				//m_ControlSocket = null;
				throw ;        
			}

			// Get the initial response after connection		
			MemoryStream responseStream = new MemoryStream();
			while(true)
			{			
				int BufferSize = 256;
				Byte[] recvbuffer = new Byte[BufferSize + 1];			
				int bytesread = m_ControlSocket.Receive(recvbuffer, BufferSize, 0 );			
				responseStream.Write(recvbuffer,0,bytesread);			
				if(IsCompleteResponse(responseStream))			
					break;			
			}
			return;		
		}

		/// <summary>
		/// Closes the Data Connection to the FTP Server and all the sockets related with it.
		/// </summary>
		public void CloseDataConnection()
		{
			if(m_DataSocket != null)
			{
				m_DataSocket.Close();
				m_DataSocket = null;		
			}
		}

		private void CloseControlConnection()
		{
			m_ControlSocket.Close();
			m_ControlSocket = null;
		}	

		private Stream ReceiveData(Socket Accept)
		{
			if( m_DataSocket == null ) // something went wrong, this should never happen
			{
				throw new ArgumentNullException();
			}
        
			MemoryStream responseStream = new MemoryStream();
			int BufferSize = 256;
			Byte [] recvbuffer = new Byte[BufferSize + 1];        
			while(true) 
			{
				int bytesread = 0;
				recvbuffer[bytesread] = (Byte)'\0';            			          
				bytesread = Accept.Receive(recvbuffer, BufferSize, 0 );			
				if( bytesread <= 0 ) 
					break;  			
				responseStream.Write(recvbuffer,0,bytesread);														
			} 				
			//Console.WriteLine("Bytes Received " + responseStream.Length);
			return responseStream;
		}

		private void SendCommand (String RequestedMethod, String Parametertopass)
		{
			String Command = RequestedMethod;
	    
			if(Parametertopass != null && !Parametertopass.Equals(""))
				Command = Command +	" " + Parametertopass ;

			Command = Command +  "\r\n";

			m_sbControlSocketLog.Append(Command);
			Trace.WriteLine(Command);
		
			//Console.WriteLine(Command);
			Byte[] sendbuffer = Encoding.ASCII.GetBytes(Command.ToCharArray());		
			if( m_ControlSocket == null ) 
			{
				throw new ProtocolViolationException ();			
			}
			int cbReturn = m_ControlSocket.Send( sendbuffer,Command.Length, 0);

			if(cbReturn < 0) 
			{
				throw new ApplicationException("Error writing to control socket");
			}
		
			return;
		}
		
		private ResponseDescription ReceiveCommandResponse()
		{ 
			ResponseDescription resp_desc = new ResponseDescription();
		
			int StatusCode = 0;
			String StatusDescription = null;
		
			bool bCompleteResponse=false;     
			if( m_ControlSocket == null ) // something went wrong protocol violation exception
			{   			
				throw new ApplicationException("Control Connection not opened");
			}
        
			MemoryStream responseStream = new MemoryStream();
		
			while(true) 
			{			
				int BufferSize = 256;
				Byte[] recvbuffer = new Byte[BufferSize + 1];
				int bytesread = 0;
				recvbuffer[0] = (Byte)'\0';				
				bytesread = m_ControlSocket.Receive(recvbuffer, BufferSize, 0 );									

				if( bytesread <= 0 ) 
					break;  // No response recieve to process, exit the loop
				
				//copy the recieved data in responsestream					
				responseStream.Write(recvbuffer,0,bytesread);

				String szResponse = Encoding.ASCII.GetString(recvbuffer,0,bytesread);
				Trace.WriteLine(szResponse);
			
				m_sbControlSocketLog.Append(szResponse);
			
				bCompleteResponse = IsCompleteResponse(responseStream);				
				if(bCompleteResponse)
				{		
					Trace.WriteLine("___ RESPONSE IS COMPLETE");
					break;			
				}
			}

			// @TODO: Remove this stream stuff, and read from stringbuffer instead.
			if(bCompleteResponse)
			{
				// now get status code	
				try
				{
					responseStream.Position=0;
					Byte [] bStatusCode = new Byte[3];
					responseStream.Read (bStatusCode, 0, 3) ;
					String statuscodestr=Encoding.ASCII.GetString(bStatusCode,0,3);
					StatusCode = Convert.ToInt16( statuscodestr);
				}
				catch
				{
					StatusCode =  -1;
				}
				// Copy the response in Status Description
				int responsesize = (int)responseStream.Length;
				responseStream.Position = 0;
				Byte [] bStatusDescription = new Byte[responsesize];
				responseStream.Read (bStatusDescription, 0, responsesize) ;			
				StatusDescription = Encoding.ASCII.GetString(bStatusDescription,4,responsesize-4);			
			}
			else
			{
				// something went wrong here			
				throw new ProtocolViolationException ();			
			}

			resp_desc.Status = StatusCode;
			resp_desc.StatusDescription = StatusDescription;
			return resp_desc;
		}
	
		private bool IsCompleteResponse(Stream responseStream)
		{		
			bool bIsComplete = false;		
			int responselength = (int) responseStream.Length;
			responseStream.Position = 0;		
			if(responselength >= 5) // statuscode(3 char)+CRLF(2 char)
			{
				int StatusCode=-1;									
				Byte[] ByteArray = new Byte[responselength];			
				String statuscodestr;							
				responseStream.Read(ByteArray,0,responselength);		
				statuscodestr=Encoding.ASCII.GetString(ByteArray,0,responselength);			
				if (responselength==5 && ByteArray[responselength-1] == '\n')	
				{
					// Last character LF seems to be very special case, need to verify					
					bIsComplete = true;
				}			
				else if ((ByteArray [responselength-1] == '\n') && (ByteArray [responselength-2] == '\r'))	
				{							
					bIsComplete = true;
				}			
				if (responselength==5 && ByteArray[responselength-1] == '\n')	
				{
					// Last character LF seems to be very special case, need to verify
					bIsComplete = true;
				}			
				if(bIsComplete) //do additional verifications
				{
					try
					{
						StatusCode = Convert.ToInt16( statuscodestr.Substring(0,3));
					}
					catch
					{
						StatusCode =  -1;					
					}		
					if (statuscodestr[3] == '-') 
					{
						// multiline response verify whether response is complete, reponse must be ended with CRLF					
						//find out the beginning of last line					
						int lastlinestart =0;
						for(lastlinestart=responselength-2;lastlinestart >0;lastlinestart--)
						{						
							if ( ByteArray [lastlinestart] == '\n' && ByteArray [lastlinestart-1] == '\r')
								break;						
						}					
						if(lastlinestart ==0) 
						{
							bIsComplete = false; // Multilines not recieved						
						}
						else if(statuscodestr[lastlinestart+4] != ' ') //still not completed
						{
							bIsComplete = false;
						}
						else
						{
							int endcode = -1;						
							try
							{
								endcode = Convert.ToInt16( statuscodestr.Substring(lastlinestart+1,3));
							}
							catch
							{
								endcode = -1;
							}		
							if (endcode != StatusCode)
								bIsComplete = false; // error invalid response					
						}
						
					}
					else if (statuscodestr[3] != ' ')
					{
						StatusCode = -1;														
					}
				}
			}
			else
			{			
				bIsComplete = false;						
			}
			return bIsComplete;                
		}
	
		/// <summary>
		/// This method returns the status code if the response is OK, otherwise it returns -1.
		/// </summary>
		/// <param name="command">The Server Command</param>
		/// <returns>The status code for the command if it is successfull, otherwise -1.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the command given is an empty string.</exception>
		/// <exception cref="NotSupportedException">Thrown if a not supported FTP command is given.</exception>
		public static String GetServerCommand(String command) 
		{
			if(command == null)
				throw new ArgumentNullException("command");

			string cmd = command.ToLower();
			string ret = null;
		
			if(cmd == "dir")
				ret = "LIST";
			else
				if(cmd == "get")
				ret = "RETR";
			else
				if(cmd == "cd")
				ret = "CWD";
			else
				if(cmd == "pwd")
				ret = "PWD";
			else
				if(cmd == "put")
				ret = "STOR";

			if(ret == null)
				throw new NotSupportedException(command);
			
			return ret;
		}
	
		/// <summary>
		/// Checks what kind of an <see cref="FtpCommandType"/> a certain command is.
		/// </summary>
		/// <param name="command">The command to check.</param>
		/// <returns>The <see cref="FtpCommandType"/> of the given command.</returns>
		public static FtpCommandType FindCommandType(String command)
		{	
			// Currently supprting only these four commands which return data on datasocket
			if(	command.Equals("USER") 
				||	command.Equals("PASS") 
				|| 	command.Equals("CWD") 
				||	command.Equals("PWD")
				||	command.Equals("CDUP") 
				||	command.Equals("QUIT"))
				return FtpCommandType.FtpControlCommand;
			else 
				if(	command.Equals("RETR")
				||	command.Equals("LIST"))
				return FtpCommandType.FtpDataReceiveCommand;
			else 
				if(	command.Equals("STOR")
				||	command.Equals("STOU"))
				return FtpCommandType.FtpDataSendCommand;
			else 
				return FtpCommandType.FtpCommandNotSupported;
		}

		private int SendData(Socket Accept)
		{
			if( Accept == null ) 
			{
				throw new ArgumentNullException();			
			}
			((FtpStream)m_RequestStream).InternalPosition=0;
		
			int Length = (int) ((FtpStream)m_RequestStream).InternalLength;
			Byte [] sendbuffer = new Byte[Length];

			((FtpStream)m_RequestStream).InternalRead(sendbuffer,0,Length);		
			int cbReturn = Accept.Send( sendbuffer, Length, 0);

			// close the request stream
			((FtpStream)m_RequestStream).InternalClose();
		
			return cbReturn;
		}
	
		private String FormatAddress(UInt32 Address, int Port )
		{
			StringBuilder sb = new StringBuilder(32);
	
			UInt32 Quotient = Address;
			UInt32 Remainder;	
			while( Quotient!=0 ) 
			{
				Remainder = Quotient % 256;
				Quotient = Quotient / 256;
				sb.Append(Remainder);
				sb.Append(',');
			}
			sb.Append( Port / 256 );
			sb.Append(',');
			sb.Append(Port % 256 );
			return sb.ToString();
		}

		private String getIPAddress(String str)
		{
			StringBuilder IPstr=new StringBuilder(32);
			String Substr = null;
			int pos1 = str.IndexOf("(")+1;
			int pos2 = str.IndexOf(",");
			for(int i =0; i<3;i++)
			{
				Substr = str.Substring(pos1,pos2-pos1)+".";
				IPstr.Append(Substr);
				pos1 = pos2+1;
				pos2 = str.IndexOf(",",pos1);
			}		
			Substr = str.Substring(pos1,pos2-pos1);
			IPstr.Append(Substr);		
			return IPstr.ToString();
		}

		private int getPort(String str)
		{
			int Port=0;
			int pos1 = str.IndexOf("(");
			int pos2 = str.IndexOf(",");
			for(int i =0; i<3;i++) //skip the ip addresss
			{			
				pos1 = pos2+1;
				pos2 = str.IndexOf(",",pos1);
			}		
			pos1 = pos2+1;
			pos2 = str.IndexOf(",",pos1);		
			String PortSubstr1=str.Substring(pos1,pos2-pos1);
		
			pos1 = pos2+1;
			pos2 = str.IndexOf(")",pos1);
			String PortSubstr2=str.Substring(pos1,pos2-pos1);
		
			//evaluate port number
			Port = Convert.ToInt32(PortSubstr1) * 256 ;
			Port = Port + Convert.ToInt32(PortSubstr2);									
			return Port;
		}

		internal string ComposeExceptionMessage(ResponseDescription resp_desc, string log) 
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("FTP Protocol Error.....\n");
			sb.Append("Status: " + resp_desc.Status + "\n");
			sb.Append("Description: " + resp_desc.StatusDescription + "\n\n");
			sb.Append("--------------------------------\n");
			sb.Append(log);
			sb.Append("\n");

			return sb.ToString();
		}
	}
}
