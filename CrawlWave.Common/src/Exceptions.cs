using System;

namespace CrawlWave.Common
{
	/// <summary>
	/// CWException is a child of <see cref="ApplicationException"/>, which is used to 
	/// represent any CrawlWave-specific exceptions.
	/// </summary>
	[Serializable]
	public class CWException : Exception
	{
		/// <summary>
		/// Constructs an instance of <see cref="CWException"/> class.
		/// </summary>
		public CWException(): base()
		{}

		/// <summary>
		/// Constructs an instance of <see cref="CWException"/> with a given Message.
		/// </summary>
		/// <param name="msg">The Exception message string</param>
		public CWException(string msg): base(msg)
		{}
	}

	/// <summary>
	/// CWMalformedUrlException is a child of <see cref="CWException"/>, it is thrown when
	/// a malformed Url is encountered.
	/// </summary>
	public class CWMalformedUrlException : CWException
	{
		/// <summary>
		/// Constructs an instance of <see cref="CWMalformedUrlException"/> class.
		/// </summary>
		public CWMalformedUrlException(): base("CWMalformedUrlException: ")
		{}

		/// <summary>
		/// Constructs an instance of <see cref="CWMalformedUrlException"/> with a given Message.
		/// </summary>
		/// <param name="msg">The Exception message string</param>
		public CWMalformedUrlException(string msg): base("CWMalformedUrlException: " + msg)
		{}
	}

	/// <summary>
	/// CWZipException is a child of <see cref="CWException"/>, it is thrown when
	/// a problem is encountered during a compression or decompression process.
	/// </summary>
	public class CWZipException : CWException
	{
		/// <summary>
		/// Constructs an instance of <see cref="CWZipException"/> class.
		/// </summary>
		public CWZipException(): base("CWZipException: ")
		{}

		/// <summary>
		/// Constructs an instance of <see cref="CWZipException"/> with a given Message.
		/// </summary>
		/// <param name="msg">The Exception message string</param>
		public CWZipException(string msg): base("CWZipException: " + msg)
		{}
	}

	/// <summary>
	/// CWDBConnectionFailedException is a child of <see cref="CWException"/>, it is thrown
	/// when a problem is encountered connecting to the system's database.
	/// </summary>
	public class CWDBConnectionFailedException : CWException
	{
		/// <summary>
		/// Constructs an instance of <see cref="CWDBConnectionFailedException"/> class.
		/// </summary>
		public CWDBConnectionFailedException(): base("CWDBConnectionFailedException: ")
		{}

		/// <summary>
		/// Constructs an instance of <see cref="CWDBConnectionFailedException"/> with a given Message.
		/// </summary>
		/// <param name="msg">The Exception message string</param>
		public CWDBConnectionFailedException(string msg): base("CWZipException: " + msg)
		{}
	}

	/// <summary>
	/// CWUnsupportedPluginException is a child of <see cref="CWException"/>, it is thrown 
	/// when an application tries to load a plugin that does not implement the interfaces
	/// it is required to.
	/// </summary>
	public class CWUnsupportedPluginException : CWException
	{
		/// <summary>
		/// Constructs an instance of <see cref="CWUnsupportedPluginException"/> class.
		/// </summary>
		public CWUnsupportedPluginException(): base("CWUnsupportedPluginException: ")
		{}

		/// <summary>
		/// Constructs an instance of <see cref="CWUnsupportedPluginException"/> with a given Message.
		/// </summary>
		/// <param name="msg">The Exception message string</param>
		public CWUnsupportedPluginException(string msg): base("CWUnsupportedPluginException: " + msg)
		{}
	}

	/// <summary>
	/// CWUserExistsException is a child of <see cref="CWException"/>, it is thrown when
	/// a problem is encountered during the registration of a new user.
	/// </summary>
	public class CWUserExistsException : CWException
	{
		/// <summary>
		/// Constructs an instance of <see cref="CWUserExistsException"/> class.
		/// </summary>
		public CWUserExistsException(): base("CWUserExistsException: ")
		{}

		/// <summary>
		/// Constructs an instance of <see cref="CWUserExistsException"/> with a given Message.
		/// </summary>
		/// <param name="msg">The Exception message string</param>
		public CWUserExistsException(string msg): base("CWUserExistsException: " + msg)
		{}
	}

	/// <summary>
	/// SerializedException is a class that is used to pass information about an exception
	/// that has occured in an application over a Web Service. It can throw a new exception
	/// of the appropriate type if necessary or an application can use its fields to check
	/// what type of an exception has occured and all the related information.
	/// </summary>
	[Serializable]
	public class SerializedException
	{
		/// <summary>
		/// The string containing the type of the exception
		/// </summary>
		public string Type;
		/// <summary>
		/// The message string related to the exception
		/// </summary>
		public string Message;
		/// <summary>
		/// The exception's stack trace
		/// </summary>
		public string StackTrace;

		/// <summary>
		/// Constructs a new instance of the <see cref="SerializedException"/> class, and by
		/// default it supposes an <see cref="ApplicationException"/> has occured.
		/// </summary>
		public SerializedException()
		{
			Type = "ApplicationException";
			Message = String.Empty;
			StackTrace = String.Empty;
		}

		/// <summary>
		/// Constructs a new instance of the <see cref="SerializedException"/> class with the
		/// supplied arguments.
		/// </summary>
		/// <param name="type">A string containing the type of the exception.</param>
		/// <param name="msg">The message related to the exception.</param>
		/// <param name="trace">The exception's stack trace.</param>
		public SerializedException(string type, string msg, string trace)
		{
			Type = type;
			Message = msg;
			StackTrace = trace;
		}

		/// <summary>
		/// Causes the <see cref="SerializedException"/> to throw an <see cref="Exception"/>
		/// of the type of the exception it encapsulates.
		/// </summary>
		public void ThrowException()
		{
			Type t = System.Type.GetType(this.Type);
			Exception e = (Exception)Activator.CreateInstance(t, new object [] { Message });
			e.Source = StackTrace;
			throw e;
		}
	}
}
