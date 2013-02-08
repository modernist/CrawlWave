using System;

namespace CrawlWave.Common
{
	/// <summary>
	/// This class represents an Internet URL. It does not contain the URL's data and HTTP
	/// Status Code. It contains a unique URL Identifier, which will save us from a lot of
	/// stupid queries. It serves as the base class for the <see cref="InternetUrlToCrawl"/>
	/// and <see cref="InternetUrlToIndex"/> classes. Its instances are used to pass Url 
	/// information between the Client and the Server and it was one of the most important
	/// reasons that led to the creation of this common Dynamic Link Library (dll).
	/// Author:	Mod
	/// Written:10/05/03
	/// Updated:29/08/03 -&gt; Added the Implementation of IComparable because without it
	///						the HtmlParser failed to identify whether a Url already
	///						exists in the list of the Out Links of its containing Url.
	///			24/08/04 -&gt; Changed m_FlagDomain from bool to byte. This way it allows
	///						using more codes for Urls that we don't yet know if they
	///						are part of the domain we wish to visit, e.g. ip addresses.
	///						Added m_Priority to indicate the priority of this Url.
	///			13/09/04 -&gt; Split the class into two subclasses so as to avoid passing
	///						unnecessary information over the web service and consume more
	///						memory than actually needed.
	///			25/10/04 -&gt; Added the FlagCheckDomain property in InternetUrlToCrawl.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.SoapInclude(typeof(InternetUrl))]
	public class InternetUrl : IComparable
	{
		#region Private variables

		private int urlID;
		private string url;
		private byte[] urlMD5;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets an integer acting as a unique identifier for the <see cref="InternetUrl"/>,
		/// since it is the key associated with it in the system's database. 
		/// </summary>
		public int ID
		{
			get { return urlID; }
			set { urlID = value; }
		}
		/// <summary>
		/// Gets or sets a <see cref="String"/> containing the address of the resource this
		/// <see cref="InternetUrl"/> object encapsulates.
		/// </summary>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Uri.
		/// </exception>
		public string Url
		{
			get { return url; }
			set
			{
				//when the url changes then the MD5 must change too
				try
				{
					if (value != String.Empty)
					{
						Uri tmp = new Uri(value); //this may throw an exception
					}
					url = value;
					urlMD5 = MD5Hash.md5(value);
				}
				catch
				{
					throw new CWMalformedUrlException("The address " + value + " is not a valid Uri.");
				}
			}
		}
		/// <summary>
		/// An array of bytes that holds the MD5 Hash of the <see cref="InternetUrl"/> address.
		/// </summary>
		public byte[] MD5
		{
			get { return urlMD5; }
			set { urlMD5 = value; }
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrl"/> class.
		/// </summary>
		public InternetUrl()
		{
			urlID = 0;
			url = String.Empty;
			urlMD5 = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrl"/> class that points to
		/// a given Url.
		/// </summary>
		/// <param name="url">The Url that this object encapsulates</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Uri.
		/// </exception>
		public InternetUrl(string url)
		{
			urlID = 0;
			try
			{
				Uri tmp = new Uri(url); //this may throw an exception
				this.url = url;
			}
			catch
			{
				throw new CWMalformedUrlException("The address " + url + " is not a valid Uri.");
			}
			urlMD5 = MD5Hash.md5(url);
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrl"/> class that points to
		/// a given Url and is associated with a given Identifier.
		/// </summary>
		/// <param name="ID">The unique identifier associated with this <see cref="InternetUrl"/></param>
		/// <param name="url">The Url that this object encapsulates</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Uri.
		/// </exception>
		public InternetUrl(int ID, string url)
		{
			urlID = ID;
			try
			{
				Uri tmp = new Uri(url); //this may throw an exception
				this.url = url;
			}
			catch
			{
				throw new CWMalformedUrlException("The address " + url + " is not a valid Uri.");
			}
			urlMD5 = MD5Hash.md5(url);
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrl"/> class that points to
		/// a given Url and is associated with a given Identifier.
		/// </summary>
		/// <param name="ID">The unique identifier associated with this <see cref="InternetUrl"/></param>
		/// <param name="url">The Url that this object encapsulates</param>
		/// <param name="md5">The MD5 Hash Code for this <see cref="InternetUrl"/> object</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Uri.
		/// </exception>
		public InternetUrl(int ID, string url, byte[] md5)
		{
			urlID = ID;
			try
			{
				Uri tmp = new Uri(url); //this may throw an exception
				this.url = url;
			}
			catch
			{
				throw new CWMalformedUrlException("The address " + url + " is not a valid Uri.");
			}
			urlMD5 = md5;
		}
		/// <summary>
		/// Constructs a new <see cref="InternetUrl"/> object from an existing <see cref="InternetUrl"/> object.
		/// </summary>
		/// <param name="IUrl">The existing <see cref="InternetUrl"/> object</param>
		public InternetUrl(InternetUrl IUrl)
		{
			urlID = IUrl.ID;
			url = IUrl.Url;
			urlMD5 = IUrl.MD5;
		}

		#endregion

		#region Implementation of IComparable Interface

		/// <summary>
		/// Compares the InternetUrl with another InternetUrl object. The comparison
		/// is done based on the Url property.
		/// </summary>
		/// <param name="obj">The InternetUrl object to be compared to this one</param>
		/// <returns>An integer indicating difference between the two objects</returns>
		public int CompareTo(object obj)
		{
			if (!(obj is InternetUrl))
			{
				throw new ArgumentException("InternetUrl can only be compared to InternetUrl objects.");
			}
			InternetUrl toCompare = (InternetUrl)obj;
			return String.Compare(url.ToString(), toCompare.Url);
		}

		/// <summary>
		/// Compares an existing <see cref="InternetUrl"/> instance with the Url of the object for equality.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance</param>
		/// <returns>true if obj represents the same Url as the Url contained in this <see cref="InternetUrl"/> instance; otherwise, false.</returns>
		/// <remarks>This method must not throw any exceptions. The method automatically uses
		/// the <see cref="Object.GetType"/> method to determine if the runtime types of the
		/// objects being compared are equivalent, therefore it is not necessary to check
		/// whether the obj parameter is a <see cref="InternetUrl"/> instance.
		/// </remarks>
		public override bool Equals(object obj)
		{
			//does not check the ID member
			InternetUrl ToCompare = (InternetUrl)obj;
			for (int i = 0; i < 16; i++)
			{
				if ((urlMD5[i] != ToCompare.MD5[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns the hash code for the <see cref="InternetUrl"/>
		/// </summary>
		/// <returns>The hash code generated for the <see cref="InternetUrl"/></returns>
		/// <remarks>
		/// The code is generated using the entire Url (including the query fragment). This
		/// means that the Urls http://in.gr/index.html?id=1 and http://in.gr/index.html do
		/// not produce the same hash code. The MD5 of the Url is used to produce the hash.
		/// </remarks>
		public override int GetHashCode()
		{
			return urlMD5.GetHashCode();
		}

		#endregion

	}

	/// <summary>
	/// This class represents a URL to be crawled. It is one of the subclasses of the
	/// <see cref="InternetUrl"/> class and is used to pass information about a Url
	/// from the Server to the Client. Besides the properties inherited from its base
	/// class it also contains a flag indicating if the robots.txt file related to the
	/// host serving the Url must be fetched back to the Server, as well as a list of
	/// paths containing the directories on the specified host that must not be visited
	/// (based on the robots exclusion standard). It also contains a checksum property
	/// that allows the client to check whether the url has been updated since the last
	/// visit. 
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.SoapInclude(typeof(InternetUrlToCrawl))]
	public class InternetUrlToCrawl : InternetUrl
	{
		#region Private variables

		private long crc;
		private DomainFlagValue flagDomain;
		private bool flagFetchRobots;
		private string robotsDisallowedPaths;

		#endregion

		#region Public Properties

		/// <summary>
		/// The CRC property gets or sets the Cyclic Redundancy Check code for the 
		/// <see cref="InternetUrlToCrawl"/> object. Its default value is 0, it has
		/// a different value if the Url has already been crawled.
		/// </summary>
		public long CRC
		{
			get { return crc; }
			set { crc = value; }
		}
		/// <summary>
		/// Gets or sets a flag indicating whether the client must check if the content of
		/// the Url is in the language that interests us.
		/// </summary>
		public DomainFlagValue FlagDomain
		{
			get { return flagDomain; }
			set { flagDomain = value; }
		}
		/// <summary>
		/// Gets or sets a flag indicating whether the robots.txt file associated with the
		/// host serving this Url must be fetched back to the server.
		/// </summary>
		/// <remarks>The values this property can take are:
		/// <list type="table">
		///   <listheader>
		///		<term>Value</term>
		///		<description>Description</description>
		///   </listheader>
		///   <item>
		///		<term>False</term>
		///		<description>Indicates that the crawler does not need to download a newer
		///		version of the robots.txt file from the host.</description>
		///   </item>
		///	  <item>
		///		<term>True</term>
		///		<description>Indicates that the crawler must download a new copy of the
		///		robots.txt file from the host.</description>
		///   </item>
		/// </list>
		/// </remarks>
		public bool FlagFetchRobots
		{
			get { return flagFetchRobots; }
			set { flagFetchRobots = value; }
		}
		/// <summary>
		/// Gets or sets a string containing the space-delimited paths on a host that the
		/// crawler must not visit, based on the Robots Exclusion Standard protocol.
		/// </summary>
		public string RobotsDisallowedPaths
		{
			get { return robotsDisallowedPaths; }
			set { robotsDisallowedPaths = value; }
		}
		#endregion

		#region Public Constructors

		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrlToCrawl"/> class.
		/// </summary>
		public InternetUrlToCrawl()
		{
			ID = 0;
			//Url=String.Empty;
			//MD5 = new byte[16]{0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0};
			crc = 0L;
			flagDomain = DomainFlagValue.Unknown;
			flagFetchRobots = false;
			robotsDisallowedPaths = String.Empty;
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrlToCrawl"/> class that
		/// points to a given Url.
		/// </summary>
		/// <param name="url">The Url that this object encapsulates.</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Url.
		/// </exception>
		public InternetUrlToCrawl(string url)
		{
			Url = url; //this may throw an exception but if it succeeds it will also update the MD5
			ID = 0;
			//			try
			//			{
			//				Uri tmp = new Uri(UrlVal); //this may throw an exception
			//				Url = UrlVal;
			//			}
			//			catch
			//			{
			//				throw new CWMalformedUrlException("The address " + UrlVal + " is not a valid Uri.");
			//			}
			//m_UrlMD5=MD5Hash.md5(UrlVal);
			crc = 0L;
			flagDomain = DomainFlagValue.Unknown;
			flagFetchRobots = false;
			robotsDisallowedPaths = String.Empty;
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrlToCrawl"/> class that 
		/// points to a given Url and is associated with a given Identifier.
		/// </summary>
		/// <param name="ID">The unique identifier associated with this <see cref="InternetUrlToCrawl"/>.</param>
		/// <param name="url">The Url that this object encapsulates.</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Uri.
		/// </exception>
		public InternetUrlToCrawl(int ID, string url)
		{
			Url = url; //this may throw an exception
			this.ID = ID;
			//			try
			//			{
			//				Uri tmp = new Uri(UrlVal); //this may throw an exception
			//				Url = UrlVal;
			//			}
			//			catch
			//			{
			//				throw new CWMalformedUrlException("The address " + UrlVal + " is not a valid Uri.");
			//			}
			//m_UrlMD5=MD5Hash.md5(UrlVal);
			crc = 0L;
			flagDomain = DomainFlagValue.Unknown;
			flagFetchRobots = false;
			robotsDisallowedPaths = String.Empty;
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrlToCrawl"/> class that
		/// points to a given Url and is associated with a given Identifier. All of the
		/// parameters of the newly created instance are supplied and initialized.
		/// </summary>
		/// <param name="ID">The unique identifier associated with this <see cref="InternetUrl"/>.</param>
		/// <param name="url">The Url that this object encapsulates.</param>
		/// <param name="md5">The MD5 Hash Code for this <see cref="InternetUrl"/> object.</param>
		/// <param name="crc">The Cyclic Redundancy Check value associated with the <see cref="InternetUrl"/> object.</param>
		/// <param name="flagDomain">A <see cref="DomainFlagValue"/> flag indicating whether the crawler must check the country of origin of the contents of the Url.</param>
		/// <param name="flagFetchRobots">A boolean flag indicating whether a new copy of the robots.txt file related with this Url must be downloaded.</param>
		/// <param name="robotsDisallowedPaths">A string containing the space-delimited paths on a host that the crawler must not visit.</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Uri.
		/// </exception>
		public InternetUrlToCrawl(int ID, string url, byte[] md5, long crc, DomainFlagValue flagDomain, bool flagFetchRobots, string robotsDisallowedPaths)
		{
			Url = url; //this may throw an exception
			this.ID = ID;
			//			try
			//			{
			//				Uri tmp = new Uri(UrlVal); //this may throw an exception
			//				Url = UrlVal;
			//			}
			//			catch
			//			{
			//				throw new CWMalformedUrlException("The address " + UrlVal + " is not a valid Uri.");
			//			}
			MD5 = md5;
			this.crc = crc;
			this.flagDomain = flagDomain;
			this.flagFetchRobots = flagFetchRobots;
			this.robotsDisallowedPaths = robotsDisallowedPaths;
		}
		/// <summary>
		/// Constructs a new <see cref="InternetUrlToCrawl"/> object from an existing <see cref="InternetUrlToCrawl"/> object.
		/// </summary>
		/// <param name="IUrl">The existing <see cref="InternetUrlToCrawl"/> object</param>
		public InternetUrlToCrawl(InternetUrlToCrawl IUrl)
		{
			ID = IUrl.ID;
			Url = IUrl.Url;
			//m_UrlMD5=IUrlVal.MD5;
			crc = IUrl.CRC;
			flagDomain = IUrl.FlagDomain;
			flagFetchRobots = IUrl.FlagFetchRobots;
			robotsDisallowedPaths = IUrl.RobotsDisallowedPaths;
		}
		#endregion

	}

	/// <summary>
	/// This class represents a URL that has been encountered by the Crawler and must be 
	/// stored in the system's repository. It is a subclass of the <see cref="InternetUrl"/>
	/// class and besides the properties inherited it also contains a flag indicating if
	/// the Url is allowed to be crawled (based on the robots exclusion standard) and a
	/// flag indicating if it belongs to the country domain we're interested in (in this
	/// case it's .gr). It also has another property indicating the priority that must be
	///	given to the crawling of this Url.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.SoapInclude(typeof(InternetUrlToIndex))]
	public class InternetUrlToIndex : InternetUrl
	{
		#region Private variables

		private bool flagRobots;
		private DomainFlagValue flagDomain;
		private byte priority;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a flag indicating whether the <a href="http://www.robotstxt.org/wc/exclusion.html">
		/// Robots Exclusion Standard</a> allows us to visit this Url.
		/// </summary>
		/// <remarks>The values this property can take are:
		/// <list type="table">
		///   <listheader>
		///		<term>Value</term>
		///		<description>Description</description>
		///   </listheader>
		///   <item>
		///		<term>False</term>
		///		<description>Indicates that the crawler can freely crawl/index the Url</description>
		///   </item>
		///	  <item>
		///		<term>True</term>
		///		<description>Indicates that the crawler is not allowd to visit the Url</description>
		///   </item>
		/// </list>
		/// </remarks>
		public bool FlagRobots
		{
			get { return flagRobots; }
			set { flagRobots = value; }
		}
		/// <summary>
		/// Gets or sets a <see cref="DomainFlagValue"/> indicating whether the given 
		/// <see cref="InternetUrlToIndex"/> is a part of the web we wish to crawl.
		/// </summary>
		public DomainFlagValue FlagDomain
		{
			get { return flagDomain; }
			set { flagDomain = value; }
		}
		/// <summary>
		/// Gets or sets an unsigned 8 bit integer indicating the Url's crawling priority.
		/// </summary>
		public byte Priority
		{
			get { return priority; }
			set { priority = value; }
		}
		#endregion

		#region Public Constructors

		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrl"/> class.
		/// </summary>
		public InternetUrlToIndex()
		{
			ID = 0;
			Url = String.Empty;
			MD5 = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
			flagRobots = false;
			flagDomain = DomainFlagValue.MustVisit;
			priority = 255;
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrlToIndex"/> class that
		/// points to a given Url. Assumes that it's allowed to visit this Url (the 
		/// <see cref="FlagRobots"/> property is set to false) and that it belongs to the
		/// domain we wish to visit.
		/// </summary>
		/// <param name="url">The Url that this object encapsulates</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Url.
		/// </exception>
		public InternetUrlToIndex(string url)
		{
			Url = url; //this may throw an exception
			ID = 0;
			//			try
			//			{
			//				Uri tmp = new Uri(UrlVal); //this may throw an exception
			//				Url = UrlVal;
			//			}
			//			catch
			//			{
			//				throw new CWMalformedUrlException("The address " + UrlVal + " is not a valid Uri.");
			//			}
			//m_UrlMD5=MD5Hash.md5(UrlVal);
			flagRobots = false;
			flagDomain = DomainFlagValue.MustVisit;
			priority = 255;
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrlToIndex"/> class that
		/// points to a given Url and is associated with a given Identifier. Assumes that
		/// it's allowed to visit this Url (the <see cref="FlagRobots"/> property is set
		/// to false) and that it belongs to the domain we wish to visit.
		/// </summary>
		/// <param name="ID">The unique identifier associated with this <see cref="InternetUrlToIndex"/></param>
		/// <param name="url">The Url that this object encapsulates</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Url.
		/// </exception>
		public InternetUrlToIndex(int ID, string url)
		{
			Url = url; //this may throw an exception
			this.ID = ID;
			//			try
			//			{
			//				Uri tmp = new Uri(UrlVal); //this may throw an exception
			//				Url = UrlVal;
			//			}
			//			catch
			//			{
			//				throw new CWMalformedUrlException("The address " + UrlVal + " is not a valid Uri.");
			//			}
			//m_UrlMD5=MD5Hash.md5(UrlVal);
			flagRobots = false;
			flagDomain = DomainFlagValue.MustVisit;
			priority = 255;
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="InternetUrlToIndex"/> class that 
		/// points to a given Url and is associated with a given Identifier. All the
		/// parameters of the newly created instance are supplied and initialized.
		/// </summary>
		/// <param name="ID">The unique identifier associated with this <see cref="InternetUrlToIndex"/></param>
		/// <param name="url">The Url that this object encapsulates</param>
		/// <param name="md5">The MD5 Hash Code for this <see cref="InternetUrlToIndex"/> object</param>
		/// <param name="flagRobots">A boolean flag indicating whether it's allowed to visit the Url</param>
		/// <param name="flagDomain">A <see cref="DomainFlagValue"/> flag indicating whether the Url belongs to the domain we wish to visit</param>
		/// <param name="priority">A <see cref="System.Byte"/> flag indicating the url's crawling priority</param>
		/// <exception cref="CWMalformedUrlException">
		/// Thrown if the supplied string value is not a valid Url.
		/// </exception>
		public InternetUrlToIndex(int ID, string url, byte[] md5, bool flagRobots, DomainFlagValue flagDomain, byte priority)
		{
			Url = url; //this may throw an exception
			this.ID = ID;
			//			try
			//			{
			//				Uri tmp = new Uri(UrlVal); //this may throw an exception
			//				Url = UrlVal;
			//			}
			//			catch
			//			{
			//				throw new CWMalformedUrlException("The address " + UrlVal + " is not a valid Uri.");
			//			}
			MD5 = md5;
			this.flagRobots = flagRobots;
			this.flagDomain = flagDomain;
			this.priority = priority;
		}
		/// <summary>
		/// Constructs a new <see cref="InternetUrlToIndex"/> object from an existing <see cref="InternetUrlToIndex"/> object.
		/// </summary>
		/// <param name="IUrl">The existing <see cref="InternetUrlToIndex"/> object</param>
		public InternetUrlToIndex(InternetUrlToIndex IUrl)
		{
			ID = IUrl.ID;
			Url = IUrl.Url;
			//m_UrlMD5=IUrlVal.MD5;
			flagRobots = IUrl.FlagRobots;
			flagDomain = IUrl.FlagDomain;
			priority = IUrl.Priority;
		}

		#endregion

	}
}
