using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CrawlWave.Common;

namespace CrawlWave.Client
{
	/// <summary>
	/// TextParser is a Singleton class that performs the link extraction and parsing of
	/// the contents of text files and generally documents with content type "text/plain".
	/// </summary>
	/// <remarks>
	/// The TextParser uses <see cref="System.Text.RegularExpressions">Regular Expressions
	/// </see> in order to perform the link extraction. The Regular Expression objects are
	/// used with the <see cref="RegexOptions.Compiled"/> option. This allows them to work
	/// much faster, because they are compiled and loaded statically into main memory. The
	/// disadvantage of using this option is that the memory allocated for these instances
	/// is never released, not even when they are disposed, until the application that is
	/// using them terminates. This is the main reason that lead to the implementation of
	/// this class as a Singleton. This way it is easy to make sure that only one instance
	/// of each of the required regular expression objects will ever be constructed.<br/>
	/// <para>
	/// Update History:
	/// <list type="table">
	///   <listheader>
	///		<term>Date</term>
	///		<description>Description</description>
	///   </listheader>
	///   <item>
	///		<term>12/09/04</term>
	///		<description>Initial Release. The class implements the <see cref="IParser"/>
	///		interface and most string processing tasks are performed using <see cref="StringBuilder"/>
	///		objects in order to reduce memory consumption. The thread safety mechanisms
	///		are employed heavily so as to improve performance.
	///		</description>
	///   </item>
	/// </list>
	/// </para>
	/// <para>
	/// <b>TODO:</b> Implement Domain Flagging and Robots Flagging methods
	/// </para>
	/// </remarks>
	public class TextParser : Parser, IParser
	{
		#region Private variables

		private static TextParser instance; //The single class instance
		private Mutex mutex;		//Mutex supporting safe access from multiple threads
		private Regex hrefRegex;	//Regular Expression for links of type http://...
		private Regex sessionIDRegex;	//Regular expression for matching Session IDs
		private Regex spacesRegex;	//Regular Expression for compacting white space characters
		private RobotsFilter robotsFilter; //Used to check if a url is allowed to be crawled
		private DomainFilter domainFilter; //Used to check the domain of a url
		private Globals globals; //Provides access to the global variables and application settings
		private static string []badExtensions={".css",".zip",".exe",".ps",".doc",".ppt",".pps",".xls",".vsd",".rar",".ace",".jpg",".jpeg",".gif",".png",".mpg",".mpe",".mpeg",".mpa",".avi",".bmp",".mp3",".mp4",".aac",".rm",".rmf",".ram",".mov",".qt",".asf",".asx",".wmv",".wma",".sit",".ico",".iso",".tar",".gzip",".gz",".tgz",".bz2",".cab",".msi",".msm"};
		//the list of extensions that are not allowed in a url (because they are of no interest to us)
		private static string []SIDVarNames={"PHPSESSID","sid","jsessionid","sessionid","session","sess_id","MSCSID"};
		//the list of Session ID variable names that must be filtered out
		private static char []greekChars = {'á','â','ã','ä','å','æ','ç','è','é','ê','ë','ì','í','ï','ð','ñ','ó','ô','õ','ö','÷','ø','ù','Ü','Ý','Þ','ß','ü','ý','þ'};
		//the list of greek characters, used for FlagDomain extraction
		private const string supportedContentType="text/plain"; 
		//The Content Type supported by the parser

		#endregion

		#region Constructor and Singleton Instance members

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private TextParser()
		{
			//Initialize the synchronization mechanism
			mutex=new Mutex();
			//Initialize the Regular Expressions
			hrefRegex=new Regex(@"(http|https)://[\w]+(\.[\w]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			//use "(http|ftp|https)://[\w]+(\.[\w]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?" to enable ftp urls
			sessionIDRegex = new Regex(@"([0-9a-fA-F]{40,64})|([\{|\(]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[\)|\}]?)$", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled); //@"^([0-9a-f]{32})|(\{?[0-9a-f]{8}-([0-9a-f]{4}-){3}-[0-9a-f]{12}\}?)$"
			spacesRegex = new Regex(@"\s+",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			//Initialize the filters
			robotsFilter = RobotsFilter.Instance();
			domainFilter = DomainFilter.Instance();
			//Get a reference to the global variables and application settings
			globals = Globals.Instance();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="TextParser"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="TextParser"/></returns>
		public static TextParser Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new TextParser();
				}
				imutex.Close();
			}
			return instance;
		}

		#endregion

		#region IParser Members

		/// <summary>
		/// Gets a string indicating the Content Type of the documents supported by the parser.
		/// </summary>
		public override string ContentType
		{
			get { return supportedContentType; }
		}

		/// <summary>
		/// Performs the extraction of links from a text document. It can extract simple
		/// links that are separated from the rest of the text using spaces or line brakes
		/// or any other delimiters. The results are returned as an <see cref="ArrayList"/>
		/// of <see cref="InternetUrlToIndex"/> objects.
		/// </summary>
		/// <remarks>
		/// Besides the parsing and extraction of Urls, ExtractLinks also performs other 
		/// tasks as well, such as:<br/>
		/// <list type="bullet">
		///   <item>
		///     <description>Filtering of urls to resources of unsupported content-type, e.g. css, images, etc.</description>
		///   </item>
		///   <item>
		///     <description>Filtering of multimple links to the same url and to the document itself.</description>
		///   </item>
		///   <item>
		///     <description>Filtering of session id variables in dynamic Urls and limiting
		///     of the number of GET variables in dynamic Urls.</description>
		///   </item>
		///   <item>
		///     <description>Flagging of Urls according to their country domain.</description>
		///   </item>
		/// </list>
		/// <b>Update History</b>
		/// <list type="table">
		///   <listheader>
		///		<term>Date</term>
		///		<description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>15/09/04</term>
		///     <description>First release. A lot more needs to be done.</description>
		///   </item>
		/// </list>
		/// </remarks>
		/// <param name="content">The text that must be parsed for links. It is passed by
		/// reference in order to reduce memory consumption.</param>
		/// <param name="contentUrl">The Url from which the content comes.</param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects, one for
		/// each link found in the content.
		/// </returns>
		public override ArrayList ExtractLinks(ref string content, ref InternetUrlToCrawl contentUrl)
		{
			ArrayList links = new ArrayList();
			// It is important to notice that if the FlagFetchRobots of the contentUrl is
			// true then the TextParser must remember this value because during the Robots
			// Filtering it will become false so as not to download the robots.txt file
			// every time a Url must be filtered.
			//bool FlagFetchRobots = contentUrl.FlagFetchRobots;
			try
			{
				//make sure only one thread will parse contents at a time.
				//mutex.WaitOne();
				if(contentUrl.FlagDomain!=DomainFlagValue.MustVisit)
				{
					contentUrl.FlagDomain = ExtractDomainFlag(ref content);

					if (contentUrl.FlagDomain != DomainFlagValue.MustVisit)
						if (InternetUtils.HostName(contentUrl).Contains("ebay.com"))
							contentUrl.FlagDomain = DomainFlagValue.MustVisit;
				}
				//perform the hyperlink matching
				MatchCollection matches = hrefRegex.Matches(content);

				if(matches.Count>0)
				{
					string documentUrl = contentUrl.Url;
					string baseUrl = BaseUrl(ref documentUrl);
					byte priority = 0;

					foreach(Match m in matches)
					{
						try
						{
							string url = m.Value.Trim();
							url = NormalizeUrl(ref url, ref baseUrl);
							priority = CleanUrlParams(ref url);
							if(FilterUrl(ref url, ref documentUrl))
							{
								InternetUrlToIndex iurl = new InternetUrlToIndex(url);
								iurl.Priority = priority;
								iurl.FlagDomain = domainFilter.FilterUrl(ref url);
								//[mod 24/2/05] No robots.txt checking is performed for non-greek urls
								if(iurl.FlagDomain == DomainFlagValue.MustVisit)
								{
									iurl.FlagRobots = robotsFilter.FilterUrl(url, contentUrl, RobotsMetaTagValue.NoMeta);
								}
								else
								{
									iurl.FlagRobots = false;
								}
								if(!links.Contains(iurl)) 
								{
									links.Add(iurl);
								}
							}
						}
						catch
						{
							if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
							{
								globals.FileLog.LogInfo("TextParser failed to parse " + m.Value);
							}
							continue;
						}
					}
				}
			}
			catch(Exception ex)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning(ex.Message);
				}
			}
			finally
			{
				//mutex.ReleaseMutex();
			}
			//contentUrl.FlagFetchRobots = FlagFetchRobots;
			ParserEventArgs e = new ParserEventArgs(contentUrl.Url);
			OnExtractLinksComplete(e);
			links.TrimToSize();
			return links;
		}

		/// <summary>
		/// Performs the extraction of links from a text document. It can extract simple
		/// links that are separated from the rest of the text using spaces or line brakes
		/// or any other delimiters. The results are returned as an <see cref="ArrayList"/>
		/// of <see cref="InternetUrlToIndex"/> objects.
		/// </summary>
		/// <remarks>
		/// Besides the parsing and extraction of Urls, ExtractLinks also performs other 
		/// tasks as well, such as:<br/>
		/// <list type="bullet">
		///   <item>
		///     <description>Filtering of urls to resources of unsupported content-type, e.g. css, images, etc.</description>
		///   </item>
		///   <item>
		///     <description>Filtering of multimple links to the same url and to the document itself.</description>
		///   </item>
		///   <item>
		///     <description>Filtering of session id variables in dynamic Urls and limiting
		///     of the number of GET variables in dynamic Urls.</description>
		///   </item>
		///   <item>
		///     <description>Flagging of Urls according to their country domain.</description>
		///   </item>
		/// </list>
		/// <b>Update History</b>
		/// <list type="table">
		///   <listheader>
		///		<term>Date</term>
		///		<description>Description</description>
		///   </listheader>
		///   <item>
		///     <term>15/09/04</term>
		///     <description>First release. A lot more needs to be done.</description>
		///   </item>
		/// </list>
		/// </remarks>
		/// <param name="content">The text that must be parsed for links. IIt is passed as
		/// an array of bytes containing the text contents in UTF8 binary format, in order
		/// to reduce memory consumption.</param>
		/// <param name="contentUrl">The Url from which the content comes.</param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects, one for
		/// each link found in the content.
		/// </returns>
		public override ArrayList ExtractLinks(byte[] content, ref InternetUrlToCrawl contentUrl)
		{
			ArrayList retVal = null;
			try
			{
				mutex.WaitOne();
				string html = Encoding.UTF8.GetString(content);
				retVal = ExtractLinks(ref html, ref contentUrl);
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
			return retVal;
		}

		/// <summary>
		/// Performs the extraction of text from a text document. The text is extracted by
		/// compacting consecutive white space characters.
		/// </summary>
		/// <param name="content">
		/// The contents of the document from which the text must be extracted. Passes by
		/// reference in order to reduce memory consumption.
		/// </param>
		/// <returns>A string containing the 'clean' text extracted from the document.</returns>
		public override string ExtractText(ref string content)
		{
			string retVal = String.Empty;
			StringBuilder sb=new StringBuilder(content);
			sb.Replace("\r\n", " ");
			sb.Replace('\n', ' ');
			sb.Replace('\t', ' ');
			retVal = spacesRegex.Replace(sb.ToString()," ");
			ParserEventArgs e = new ParserEventArgs(String.Empty);
			OnExtractTextComplete(e);
			return retVal;
		}

		/// <summary>
		/// Performs the extraction of text from a text document. The text is extracted by
		/// compacting consecutive white space characters.
		/// </summary>
		/// <param name="content">
		/// The contents of the document from which the text must be extracted.
		/// </param>
		/// <returns>A string containing the 'clean' text extracted from the document.</returns>
		public override string ExtractText(byte[] content)
		{
			string retVal = String.Empty;
			try
			{
				string text = Encoding.UTF8.GetString(content);
				retVal = ExtractText(ref text);
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Performs the extraction of content from a text document. Depending on the value
		/// of the Flag provided it simply returns a string same as the input or it removes
		/// consecutive spaces in order to perform a white space compaction.
		/// </summary>
		/// <param name="content">
		/// The contents of the document from which the content must be extracted.
		/// </param>
		/// <param name="Flag">Determines what kind of processing will be performed on the
		/// input. It has no effect, the method performs white space character compaction
		/// on the input string.
		/// </param>
		/// <returns>A string containing the desired extracted content.</returns>
		public override string ExtractContent(ref string content, bool Flag)
		{
			string retVal = String.Empty;
			retVal = ExtractText(ref content);
			ParserEventArgs e = new ParserEventArgs(String.Empty);
			OnExtractContentComplete(e);
			return retVal;
		}

		/// <summary>
		/// Performs the extraction of content from a text document. Depending on the value
		/// of the Flag provided it simply returns a string same as the input or it removes
		/// consecutive white space characters in order to perform a compaction.
		/// </summary>
		/// <param name="content">
		/// The contents of the document from which the content must be extracted.
		/// </param>
		/// <param name="Flag">Determines what kind of processing will be performed on the
		/// input. If set to false it simply returns a string same to the input. If set to
		/// true it performs whitespace compaction.
		/// </param>
		/// <returns>A string containing the desired extracted content.</returns>
		public override string ExtractContent(byte[] content, bool Flag)
		{
			string retVal = String.Empty;
			try
			{
				string text = Encoding.UTF8.GetString(content);
				retVal = ExtractContent(ref text, Flag);
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Occurs when the extraction of links from a text document is complete
		/// </summary>
		public event ParserEventHandler ExtractLinksComplete;

		/// <summary>
		/// Occurs when the extraction of text from a text document is complete
		/// </summary>
		public event ParserEventHandler ExtractTextComplete;

		/// <summary>
		/// Occurs when the extraction of content from a text document is complete
		/// </summary>
		public event ParserEventHandler ExtractContentComplete;

		#endregion

		#region Private members

		/// <summary>
		/// Finds the base Url of a given Url, including the trailing slash. Works for both
		/// absolute and relative Urls.
		/// </summary>
		/// <example>
		/// <code>
		/// string contentUrl = "http://www.in.gr";
		/// string content = "&lt;html&gt;&lt;/html&gt;";
		/// string result = null;
		/// result = BaseUrl(ref contentUrl, ref content); // returns http://www.in.gr/
		/// contentUrl = "http://www.in.gr/photos/a.html";
		/// result = BaseUrl(ref contentUrl, ref content); // returns http://www.in.gr/photos/
		/// contentUrl = "photos/a.html";
		/// result = BaseUrl(ref contentUrl, ref content); // returns photos/
		/// </code>
		/// </example>
		/// <remarks>
		/// This method does not use <see cref="StringBuilder"/> objects to perform the
		/// processing, since that would only cause more code complexity without any gain.<br/>
		/// <b>Update History</b>
		/// Update History:
		/// <list type="table">
		///   <listheader>
		///		<term>Date</term>
		///		<description>Description</description>
		///   </listheader>
		///   <item>
		///		<term>15/09/04</term>
		///		<description>Initial release, based on <see cref="HtmlParser.BaseUrl"/>.</description>
		///   </item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if the input strings are null.</exception>
		/// <param name="contentUrl">
		/// The Url from which the base Url must be extracted.
		/// </param>
		/// <returns>A string containing the base Url of the given Url.</returns>
		protected internal string BaseUrl(ref string contentUrl)
		{
			if(contentUrl == null || contentUrl == String.Empty)
			{
				throw new ArgumentNullException();
			}
			string retVal = String.Empty;
			try
			{
				
				if(contentUrl.EndsWith("/"))
				{
					//The url contains a trailing slash, so no processing is required
					retVal = contentUrl;
				}
				else
				{
					//first try to see if there is a double slash (ie protocol)
					int doubleSlashPos=contentUrl.IndexOf("//");
					//start from the end trying to find the position of the last slash
					int lastSlashPos=0;
					for (lastSlashPos=contentUrl.Length-1; lastSlashPos>=0; lastSlashPos--)
					{
						if (contentUrl[lastSlashPos]=='/')
							break;
					}
					if (((doubleSlashPos>0)&&(lastSlashPos==doubleSlashPos+1))||(lastSlashPos==-1))
					{
						retVal=contentUrl+'/';
					}
					else
					{
						retVal=contentUrl.Substring(0,lastSlashPos)+'/';
					}
				}
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo("TextParser.BaseUrl failed: " + e.ToString());
				}
			}
			return retVal;
		}

		/// <summary>
		/// Returns the canonical form of a url whose base url is baseUrl, even if they are
		/// relative urls, e.g.
		/// ../photos/a.html + http://www.in.gr/ram/ returns http://www.in.gr/photos/a.html
		/// /pages/index.php + http://www.in.gr/ram/ returns http://www.in.gr/pages/index.php
		/// http://www.in.gr + null returns http://www.in.gr/
		/// </summary>
		/// <param name="url">The url to be normalized.</param>
		/// <param name="baseUrl">The base url of the url to be normalized.</param>
		/// <returns>The normalized form of the url.</returns>
		protected internal string NormalizeUrl(ref string url, ref string baseUrl)
		{
			string retVal="";
			//check if the url contains the protocol segment.
			if ((url.StartsWith("http"))||(url.StartsWith("ftp")))
			{
				//the Url is already in almost canonical form. We need to convert &amp; to &
				Uri uri=new Uri(url.Replace("&amp;","&"));
				retVal=uri.AbsoluteUri;//CombinePaths(baseUrl, url);
			}
			else
			{
				//simply combining the two paths will do.
				retVal=CombineUrls(baseUrl, url.Replace("&amp;","&"));
			}
			//check for and replace occurences of double slashes after the protocol segment
			int startpos=retVal.IndexOf("//");
			while(startpos!=-1)
			{
				if(retVal[startpos-1]!=':')
				{
					retVal=retVal.Remove(startpos,1);
				}
				startpos=retVal.IndexOf("//",startpos+2);
			}
			//the url cannot be smaller than 10 chars
			if(retVal.Length<=10)
			{
				retVal="";
			}
			//the Url is now 'clean'
			return retVal;
		}

		/// <summary>
		/// Combines two urls, either relative or not, to provide the resulting url
		/// </summary>
		/// <param name="baseUrl">The base url.</param>
		/// <param name="newUrl">The url to be combined with baseUrl.</param>
		/// <returns>The url that occurs from the combination of baseUrl and newUrl.</returns>
		protected internal string CombineUrls(string baseUrl, string newUrl)
		{
			string retVal="";
			try
			{
				Uri baseUri= new Uri(baseUrl);
				Uri newUri = new Uri(baseUri,newUrl);
				retVal = newUri.AbsoluteUri;
			}
			catch(UriFormatException)
			{
				retVal=""; //this will lead to losing a url
			}
			return retVal;
		}

		/// <summary>
		/// Performs a processing of the GET parameters of dynamic urls. It removes any
		/// session IDs and limits the number of parameters to 3, so as to avoid urls that
		/// act as "black holes". It also removes named anchors from the end of the urls
		/// for the same reason and performs a calculation of the url's 'importance', taking
		/// into account the length of the absolute path and the number of its parameters.
		/// </summary>
		/// <remarks>
		/// This method uses <see cref="StringBuilder"/> objects to perform the processing,
		/// and receives its input by reference, so as to minimize the memory consumed.<br/>
		/// The priority of the resulting url is calculated as follows:<br/>
		/// The priority for a top level url is equal to 1.<br/>
		/// If the Absolute Path of the url is not empty then for each level (folder depth)
		/// the priority is increased by 1.<br/>
		/// If the url is dynamic (it has parameters) the priority is increased by 2 and for
		/// each parameter contained in the Query segment if it is a session id variable the
		/// priority is increased by 2, otherwise it is increased by 1.<br/>
		/// Finally, if the url contains named anchors the priority is increased by 1.<br/>
		/// <b>Update History</b>
		/// <list type="table">
		///   <listheader>
		///		<term>Date</term>
		///		<description>Description</description>
		///   </listheader>
		///   <item>
		///		<term>15/09/04</term>
		///		<description>Initial release, works exactly like <see cref="HtmlParser.CleanUrlParams"/>. 
		///		</description>
		///   </item>
		/// </list>
		/// </remarks>
		/// <example>
		/// http://www.aueb.gr/ -&gt; http://www.aueb.gr/, priority:1<br/>
		/// http://www.aueb.gr/index.php?id=1#top -&gt; http://www.aueb.gr/index.php?id=1, priority:5<br/>
		/// http://www.aueb.gr/a/b/index.php?id=5&amp;session_id=xxx&amp;a=1&amp;b=2#top -&gt; http://www.aueb.gr/a/b/index.php?id=5&amp;a=1&amp;b=2, priority: 11
		/// </example>
		/// <param name="url">
		/// The Url from which the Session ID params must be removed. Passed by reference so that it
		/// can be altered and avoid using more memory since most of the urls aren't dynamic.
		/// </param>
		/// <returns>An unsigned 8 bit integer indicating the Url's priority.</returns>
		protected internal byte CleanUrlParams(ref string url)
		{
			StringBuilder sb= new StringBuilder(url);
			byte priority = 1;
			int pos=url.IndexOf('#'), i=0;
			if (pos!=-1)
			{
				//the url contains an anchor, so it must be cleaned
				sb.Remove(pos,url.Length-pos);
				priority++;
			}
			//now check for url parameters
			pos=url.IndexOf('?');
			if (pos!=-1)
			{
				//it's a dynamic url, so it must be processed.
				priority+=2;
				Uri uri=new Uri(sb.ToString());
				//calculate the absolute path's depth  and increase the priority accordingly
				if(uri.AbsolutePath.Length>1)
				{
					for(i=1; i<uri.AbsolutePath.Length; i++)
					{
						if(uri.AbsolutePath[i]=='/')
						{
							priority++;
						}
					}
				}
				sb.Remove(pos, sb.Length-pos);
				int numParams=0; bool foundSID=false;
				string [] urlParams = uri.Query.Split('?','&');
				foreach (string urlParam in urlParams)
				{
					if(urlParam!="")
					{
						//check if the parameter is a session id, if so then skip it
						foundSID=false;
						for(i=0; i<SIDVarNames.Length; i++)
						{
							if(urlParam.IndexOf(SIDVarNames[i])!=-1)
							{
								foundSID=true;
								priority+=2;
								break;
							}
						}
						if(!foundSID)
						{
							if(sessionIDRegex.IsMatch(urlParam))
							{
								foundSID = true;
								priority +=2;
							}
						}
						if(!foundSID)
						{
							if(numParams==0)
							{
								sb.Append('?');
							}
							else
							{
								sb.Append('&');
							}
							sb.Append(urlParam);
							numParams++;
							priority++;
						}
					}
					if(numParams==3) //only keep as many as 3 parameters
					{
						break;
					}
				}
			}
			url=sb.ToString();
			return priority;
		}

		/// <summary>
		/// FilterUrl filters out urls that must not be visited, such as urls to photos.
		/// Returns true if the Url is OK and can be crawled and false if it must be 
		/// rejected. This could be adapted to filter out urls that are not part of a
		/// specific domain, eg .gr or it could be adapted to use some kind of rules like
		/// regular expressions that define which urls should be filtered out.
		/// <br/>Written: 19/5/03<br/>
		/// Updated:30/8/03 - Added support for removing circular links
		/// </summary>
		/// <param name="url">The url to examine</param>
		/// <param name="contentUrl">The url in whose the contents the examined Url is found</param>
		/// <returns>True if it is OK to crawl the url, false otherwise</returns>
		protected internal bool FilterUrl(ref string url, ref string contentUrl)
		{
			if((url==String.Empty)||(url == null))
			{
				return false;
			}
			//check if it is a circular link
			if (url==contentUrl)
			{
				return false;
			}
			//check against the list of bad urls
			for (int i=0; i<badExtensions.Length; i++)
			{
				if (url.IndexOf(badExtensions[i])!=-1)
				{
					return false; //exit the loop as soon as we discover it's a bad url
				}
			}
			return true;
		}

		/// <summary>
		/// Attempts to extract the appropriate FlagDomain value from the contents of the document.
		/// </summary>
		/// <param name="content">The HTML content that must be parsed for Domain Flag value.</param>
		/// <returns>A <see cref="DomainFlagValue"/> indicating whether the content of the 
		/// text document is in the language that interests us.
		/// </returns>
		private DomainFlagValue ExtractDomainFlag(ref string content)
		{
			DomainFlagValue retVal = DomainFlagValue.Unknown;
			try
			{
				//attempt to find any greek character in the contents of the document.
				if(content.IndexOfAny(greekChars)!=-1)
				{
					//a greek character was found, assume the content is in greek
					retVal= DomainFlagValue.MustVisit;
				}
				else
				{
					retVal = DomainFlagValue.MustNotVisit;
				}
			}
			catch
			{}
			return retVal;
		}

		#endregion

		#region Event Invokers

		/// <summary>
		/// Raises an ExtractLinksComplete event when the extraction of links is complete
		/// </summary>
		/// <param name="e">The <see cref="ParserEventArgs"/> related to the event.</param>
		private void OnExtractLinksComplete(ParserEventArgs e)
		{
			if(ExtractLinksComplete!=null)
			{
				ExtractLinksComplete(this, e);
			}
		}

		/// <summary>
		/// Raises an ExtractTextComplete event when the extraction of text is complete
		/// </summary>
		/// <param name="e">The <see cref="ParserEventArgs"/> related to the event.</param>
		private void OnExtractTextComplete(ParserEventArgs e)
		{
			if(ExtractTextComplete!=null)
			{
				ExtractTextComplete(this, e);
			}
		}

		/// <summary>
		/// Raises an ExtractContentComplete event when the extraction of content is complete
		/// </summary>
		/// <param name="e">The <see cref="ParserEventArgs"/> related to the event.</param>
		private void OnExtractContentComplete(ParserEventArgs e)
		{
			if(ExtractContentComplete!=null)
			{
				ExtractContentComplete(this, e);
			}
		}

		#endregion

	}
}
