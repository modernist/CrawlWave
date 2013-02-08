using System;
using System.Net;
using System.Collections;
using System.Xml.Serialization;

namespace CrawlWave.Common
{
	/// <summary>
	/// This class represents the results of the crawling process on a Url. It contains
	/// the URL's location, CRC, HTTP Status Code, the timestamp of the retrieval, the
	/// last modification time and, finally, the data as a simple string form. It is a
	/// big class (an instance could consume >100K) but we'll have to live with that.
	/// </summary>
	[Serializable]
	public class UrlCrawlData
	{
		#region Private variables

		/// <summary>
		/// the url entity, perhaps we could change it with System.Uri
		/// </summary>
		private InternetUrlToCrawl url;
		/// <summary>
		/// A boolean flag indicating whether the Url has been updated since the last visit
		/// </summary>
		private bool updated;
		/// <summary>
		/// A boolean flag indicating whether the Url has redirected the crawler to another
		/// </summary>
		private bool redirected;
		/// <summary>
		/// A boolean flag indicating whether the url where the crawler has been redirected
		/// is allowed to be crawled according to the Robots Exclusion Standard
		/// </summary>
		private bool redirectedFlagRobots;
		/// <summary>
		/// A flag indicating whether the url where the crawler has been redirected belongs
		/// to the part of the web we wish to crawl
		/// </summary>
		private DomainFlagValue redirectedFlagDomain;
		/// <summary>
		/// A flag indicating the priority of the url where the crawler has been redirected
		/// </summary>
		private byte redirectedPriority;
		/// <summary>
		/// The <see cref="HttpStatusCode"/> that the server returned during the crawling
		/// </summary>
		private System.Net.HttpStatusCode httpStatusCode;
		/// <summary>
		/// The contents of the Url in string format
		/// </summary>
		private string data;
		/// <summary>
		/// The <see cref="DateTime"/> when the Url was last visited
		/// </summary>
		private DateTime timeStamp;
		/// <summary>
		/// The time it took (in msec) to download the contents of the url.
		/// </summary>
		private int retrievalTime;
		/*/// <summary>
		/// The list of urls (out links) contained in the url, it will contain a number
		/// of <see cref="InternetUrl"/> objects
		/// </summary>
		[XmlElement(Type=typeof(InternetUrlToIndex))]
		private ArrayList m_OutLinks;*/
		/// <summary>
		/// An array of <see cref="InternetUrlToIndex"/> with the out links contained in a
		/// Url's contents.
		/// </summary>
		private InternetUrlToIndex [] outLinks;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an instance of the <see cref="UrlCrawlData"/> class and initializes
		/// it with the default values.
		/// </summary>
		public UrlCrawlData()
		{
			url=new InternetUrlToCrawl();
			updated=false;
			redirected=false;
			redirectedFlagRobots = false;
			redirectedFlagDomain = DomainFlagValue.MustVisit;
			redirectedPriority = 255;
			httpStatusCode=HttpStatusCode.OK;
			data=String.Empty;
			timeStamp=DateTime.UtcNow;
			retrievalTime=0;
			outLinks=null;//new ArrayList();
		}

		#endregion
		
		#region Public Properties

		/// <summary>
		/// Gets or sets the Url of the <see cref="InternetUrlToCrawl"/> encapsulated by a 
		/// <see cref="UrlCrawlData"/> object.
		/// </summary>
		public string Url
		{
			get { return url.Url; }
			set { url.Url = value;}
		}
		/// <summary>
		/// Gets or sets the <see cref="InternetUrlToCrawl"/> encapsulated by this object.
		/// </summary>
		public InternetUrlToCrawl UrlToCrawl
		{
			get { return url; }
			set { url = value;}
		}
		/// <summary>
		/// Gets or sets a unique number that corresponds to the ID associated with an
		/// instance of the <see cref="UrlCrawlData"/> class.
		/// </summary>
		public int ID
		{
			get { return url.ID; }
			set { url.ID = value;}
		}
		/// <summary>
		/// Gets or sets the MD5 Hash Code for the Url that the object points to.
		/// </summary>
		public byte[] MD5
		{
			get { return url.MD5; }
			set { url.MD5 = value;}
		}
		/// <summary>
		/// Gets or sets a boolean value indicating whether the Url has been updated
		/// since the last visit.
		/// </summary>
		public bool Updated
		{
			get { return updated; }
			set { updated = value;}
		}
		/// <summary>
		/// Gets or sets a boolean flag indicating whether the Url has redirected the
		/// crawler to another Url.
		/// </summary>
		public bool Redirected
		{
			get { return redirected; }
			set { redirected = value;}
		}
		/// <summary>
		/// Gets or sets a boolean flag indicating whether the url where the crawler has 
		/// been redirected is allowed to be crawled by the Robots Exclusion Standard
		/// </summary>
		public bool RedirectedFlagRobots
		{
			get { return redirectedFlagRobots; }
			set { redirectedFlagRobots = value;}
		}
		/// <summary>
		/// Gets or sets a flag indicating whether the url where the crawler has been 
		/// redirected belongs to the part of the web we wish to crawl
		/// </summary>
		public DomainFlagValue RedirectedFlagDomain
		{
			get { return redirectedFlagDomain; }
			set { redirectedFlagDomain = value;}
		}
		/// <summary>
		/// Gets or sets a flag indicating the priority of the url where the crawler has
		/// been redirected
		/// </summary>
		public byte RedirectedPriority
		{
			get { return redirectedPriority; }
			set { redirectedPriority = value;}
		}
		/// <summary>
		/// Gets or sets the Cyclic Reduncancy Check value that is calculated from the
		/// contents of this Url.
		/// </summary>
		public long CRC
		{
			get { return url.CRC; }
			set { url.CRC = value;}
		}
		/// <summary>
		/// Gets or sets a flag indicating whether the server has asked the client to fetch
		/// the robots.txt file from the host.
		/// </summary>
		/// <remarks>
		/// For more information see also <see cref="InternetUrlToCrawl.FlagFetchRobots"/>.
		/// <seealso cref="InternetUrlToCrawl.FlagFetchRobots">InternetUrl.FlagFetchRobots</seealso>
		/// </remarks>
		public bool FlagFetchRobots
		{
			get { return url.FlagFetchRobots; }
			set { url.FlagFetchRobots = value;}
		}
		/// <summary>
		/// Gets or sets a string containing the paths disallowed by the robots.txt file on
		/// the host, separated by a space.
		/// </summary>
		public string RobotsDisallowedPaths
		{
			get { return url.RobotsDisallowedPaths; }
			set { url.RobotsDisallowedPaths = value;}
		}
		/// <summary>
		/// Gets or sets the <see cref="HttpStatusCode"/> that the server returned during
		/// the last visit to the Url encapsulated by this object.
		/// </summary>
		public System.Net.HttpStatusCode HttpStatusCode
		{
			get { return httpStatusCode; }
			set { httpStatusCode = value;}
		}
        /// <summary>
        /// Gets or sets a string that contains the Url's data.
        /// </summary>
		public string Data
		{
			get { return data; }
			set { data = value;}
		}
		/// <summary>
		/// Gets or sets the <see cref="DateTime"/> when the Url was last visited.
		/// </summary>
		public DateTime TimeStamp
		{
			get { return timeStamp; }
			set { timeStamp = value;}
		}
		/// <summary>
		/// Gets or sets the time it took (in msec) to download the contents of the Url.
		/// </summary>
		public int RetrievalTime 
		{
			get { return retrievalTime; }
			set { retrievalTime = value;}
		}
		/*/// <summary>
		/// Gets or sets an <see cref="ArrayList"/> with the Urls (out links) contained
		/// in the contents of this Url, it contains a number of <see cref="InternetUrlToIndex"/> objects.
		/// </summary>
		public ArrayList OutLinks
		{
			get{ return m_OutLinks; }
			set{ m_OutLinks = value;}
		}*/
		/// <summary>
		/// Gets or sets an array of <see cref="InternetUrlToIndex"/> with the out links
		/// contained in a Url's contents.
		/// </summary>
		public InternetUrlToIndex [] OutLinks
		{
			get{ return outLinks; }
			set{ outLinks = value;}			
		}

		#endregion

/*		#region Public Interface Functions
		
		/// <summary>
		/// Adds a Url to the collection of out links associated with this Url.
		/// </summary>
		/// <param name="UrlToAdd">The address of the Url to add to the collection.</param>
		/// <exception cref="CWException">
		/// Thrown if the Url cannot be added to the collection of out links
		/// </exception>
		public void AddOutLink(string UrlToAdd)
		{
			try
			{
				m_OutLinks.Add(new InternetUrlToIndex(UrlToAdd));
			}
			catch (Exception e)
			{
				throw new CWException(e.Message);
			}
		}

		/// <summary>
		/// Adds a Url to the collection of out links associated with this Url.
		/// </summary>
		/// <param name="UrlToAdd">
		/// An <see cref="InternetUrlToIndex"/> containing the address of the Url to be added
		/// to the collection.
		/// </param>
		/// <exception cref="CWException">
		/// Thrown if the Url cannot be added to the collection of out links
		/// </exception>
		public void AddOutLink(InternetUrlToIndex UrlToAdd)
		{
			try
			{
				m_OutLinks.Add(UrlToAdd);
			}
			catch (Exception e)
			{
				throw new CWException(e.Message);
			}
		}

		#endregion*/
	}
}
