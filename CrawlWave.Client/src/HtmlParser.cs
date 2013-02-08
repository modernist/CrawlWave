using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CrawlWave.Common;

namespace CrawlWave.Client
{
	/// <summary>
	/// HtmlParser is a Singleton class that performs the link extraction and parsing of
	/// the contents of HTML pages and generally documents with content type "text/html".
	/// </summary>
	/// <remarks>
	/// The HtmlParser uses <see cref="System.Text.RegularExpressions">Regular Expressions
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
	///		<term>31/08/04</term>
	///		<description>The class now implements the <see cref="IParser"/> interface and
	///		most string processing tasks are performed using <see cref="StringBuilder"/>
	///		objects in order to reduce memory consumption. The thread safety mechanisms
	///		are now employed a lot more heavily so as to improve performance.</description>
	///   </item>
	///   <item>
	///		<term>17/10/04</term>
	///		<description>The class now attempts to extract charset information from the HTML
	///		document, thus allowing to check if a url's content is of interest to us. Added
	///		the ExtractDomainFlag method.</description>
	///   </item>
	///   <item>
	///		<term>26/11/04</term>
	///		<description>The class is now able to extract urls from refresh meta tags that
	///		cause redirections and are commonly found in gateway pages.</description>
	///   </item>
	///   <item>
	///		<term>20/01/05</term>
	///		<description>The class is now able to remove session IDs inlined in urls.</description>
	///   </item>
	/// </list>
	/// </para>
	/// <para>
	/// <b>Remarks from previous version's implementation</b><br/>
	/// Author:	Mod<br/>
	/// Date:	11/05/03<br/>
	/// Update History:
	/// <list type="table">
	///   <listheader>
	///		<term>Date</term>
	///		<description>Description</description>
	///   </listheader>
	///   <item>
	///		<term>24/08/03</term>
	///		<description>Added support for parameter number and anchor limitation, better
	///		exception handling during the extraction of links</description>
	///   </item>
	///   <item>
	///		<term>29/08/03</term>
	///		<description>The parser now removes any links pointing to the same url</description>
	///   </item>
	///   <item>
	///		<term>06/09/03</term>
	///		<description>Updated the code to reflect the changes in the InternetUrl class</description>
	///   </item>
	///   <item>
	///		<term>07/09/03</term>
	///		<description>Fixed a bug that caused the HtmlParser to create wrong urls if a
	///		redirection had occured when crawling a page. Updated the HtmlParser to handle
	///		absolute urls (/...) correctly.</description>
	///   </item>
	///   <item>
	///		<term>16/09/03</term>
	///		<description>Fixed a bug that caused the &lt;base href..&gt; meta not to be parsed,
	///		leading to the extraction of wrong Urls. The class is now more compliant to
	///		RFC1808.</description>
	///   </item>
	/// </list>		
	/// </para>
	/// </remarks>
	public class HtmlParser : Parser, IParser
	{
		#region Private variables

		private static HtmlParser instance; //The single class instance
		private Mutex mutex;		//Mutex supporting safe access from multiple threads
		private Regex ahrefRegex;	//Regular Expression for links <a href=... and image maps <area href=...
		private Regex baseRegex;	//Regular Expression for base Url <base href=...
		private Regex charsetRegex;	//Regular Expression for charset info extraction
		private Regex frameRegex;	//Regular Expression for framesets <frame src=...
		private Regex flashRegex;	//Regular expression for flash objects <embed src=... type=...
		private Regex refreshRegex;	//Regular expression for refresh (redirect) meta tag <meta http-equiv="refresh" content=...
		private Regex robotRegex;	//Regular expression for robots meta tag
		private Regex scriptRegex;	//Regular Expression for scripts
		private Regex spacesRegex;	//Regular Expression for white space characters
		private Regex stylesRegex;	//Regular Expression for stylesheets
		private Regex stripTagRegex;	//Regular Expression for stripping tags
		private Regex sessionIDRegex;	//Regular expression for matching Session IDs (32 byte long hex strings or Guids)
		private Regex inlinedSessionIDRegex; //Regular Expression for matching session ids that are inlined in the path of a pseudo-static url
		private RobotsFilter robotsFilter; //Used to check if a url is allowed to be crawled
		private DomainFilter domainFilter; //Used to check the domain of a url
		private CultureInfo culture;//Used to perform culture-aware string processing.
		private Globals globals; //Provides access to the global variables and application settings
		private static string []badUrls={"mailto:","javascript:","JavaScript:","vbscript:","file:///","://localhost", "://127.0.0.1"};
		//the list of strings that must not be allowed in a url
		private static string []badExtensions={".css",".zip",".exe",".ps",".doc",".ppt",".pps",".xls",".vsd",".rar",".ace",".jpg",".jpeg",".gif",".png",".mpg",".mpe",".mpeg",".mpa",".avi",".bmp",".mp3",".mp4",".aac",".rm",".rmf",".ram",".mov",".qt",".asf",".asx",".wmv",".wma",".sit",".ico",".iso",".tar",".gzip",".gz",".tgz",".bz2",".cab",".msi",".msm"};
		//the list of extensions that are not allowed in a url (because they are of no interest to us)
		private static string []SIDVarNames={"PHPSESSID","sid","jsessionid","sessionid","session","sess_id","MSCSID"};
		//the list of Session ID variable names that must be filtered out
		private static char []endDelimiters = {' ','"','>','\''};
		//the list of characters used as delmiters when a link is parsed
		private static char []parameterDelimiters = {'?', ';', '&'};
		//the list pf characters used as delimiters for parameters
		private static string []attributeNames = {"href", "src", "http-equiv", "refresh", "content", "url;"};
		//the lsit of url attribute names, useful for parsing urls
		private static char []greekChars = {'á','â','ã','ä','å','æ','ç','è','é','ê','ë','ì','í','ï','ð','ñ','ó','ô','õ','ö','÷','ø','ù','Ü','Ý','Þ','ß','ü','ý','þ'};
		//the list of greek characters, used for FlagDomain extraction
		private const string supportedContentType="text/html"; 
		//The Content Type supported by the parser
		private const string alternativeContentType = "application/xml";
		//The alternative content type description (for xml documents)
		private const string alternativeContentType2 = "application/xhtml+xml";
		//The second alternative content type description (for XHTML documents)

		#endregion

		#region Constructor and Singleton Instance methods

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private HtmlParser()
		{
			//Initialize the synchronization mechanism
			mutex=new Mutex();
			//Initialize the Regular Expressions
			ahrefRegex = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			baseRegex = new Regex("base\\s*href=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			charsetRegex = new Regex("<meta\\s*http-equiv=([^>])*charset\\s*=\\s*([^>])*(iso-8859-7|windows-1253)([^>])*>",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			frameRegex = new Regex("frame\\s*.*src\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			flashRegex = new Regex("<embed\\s*([^>])*src\\s*=([^>])*type\\s*=([^>])*application/x-shockwave-flash([^>])*>",  RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			refreshRegex = new Regex("<meta\\s*http-equiv=([^>])*refresh([^>])*content\\s*=\\s*\"[^>]*\">", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			robotRegex = new Regex("<meta\\s*name\\s*=\\s*\"robots\"\\s*content\\s*=\\s*\"[^>]*\">", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			scriptRegex = new Regex(@"(?i)<script([^>])*>(\w|\W)*</script([^>])*>",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled); //@"(?i)<script([^>])*>(\w|\W)*</script([^>])*>" or @"<script[^>]*>(\w|\W)*?</script[^>]*>"
			spacesRegex = new Regex(@"\s+",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			stylesRegex = new Regex(@"<style([^>])*>(\w|\W)*</style([^>])*>",RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			stripTagRegex = new Regex("<[^>]*>", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);//<[^>]+> or   >(?:(?<t>[^<]*))
			sessionIDRegex = new Regex(@"([0-9a-fA-F]{40,64})|([\{|\(]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[\)|\}]?)$", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled); //@"^([0-9a-f]{32})|(\{?[0-9a-f]{8}-([0-9a-f]{4}-){3}-[0-9a-f]{12}\}?)$"
			inlinedSessionIDRegex = new Regex(@"/(%28|\{)?(([0-9a-fA-F]{8}[-]?(([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}))|([0-9a-fA-F]{12,64}))(%29|\})?/", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			//Initialize the filters
			robotsFilter = RobotsFilter.Instance();
			domainFilter = DomainFilter.Instance();
			//Initialize the culture info to Greek (ISO)
			culture = CultureInfo.CreateSpecificCulture("el-GR");
			//Get a reference to the global variables and application settings
			globals = Globals.Instance();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="HtmlParser"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="HtmlParser"/>.</returns>
		public static HtmlParser Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new HtmlParser();
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
		/// Gets a string indicating the alternative Content Type of the documents supported by the parser.
		/// </summary>
		public string AlternativeContentType
		{
			get { return alternativeContentType; }
		}

		/// <summary>
		/// Gets a string indicating the second alternative Content Type of the documents supported by the parser.
		/// </summary>
		public string AlternativeContentType2
		{
			get { return alternativeContentType2; }
		}

		/// <summary>
		/// Performs the extraction of links from an html document. It can extract simple
		/// links, image map links and frame links. The results are returned as an <see cref="ArrayList"/>
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
		///     <description>Filtering of script urls (javascript and vbscript) and mailto urls.</description>
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
		///     <term>26/11/04</term>
		///     <description>Added support for extracting links from refresh meta tags.</description>
		///   </item>
		///   <item>
		///     <term>12/09/04</term>
		///     <description>General revision, rewritten many functions from scratch.</description>
		///   </item>
		///   <item>
		///     <term>30/08/03</term>
		///     <description>Added support for filtering circular links.</description>
		///   </item>
		///   <item>
		///     <term>28/08/03</term>
		///     <description>Added support for limiting the number of parameters of dynamic urls.</description>
		///   </item>
		///   <item>
		///     <term>23/07/03</term>
		///     <description>Added support for removing session ids from dynamic urls.</description>
		///   </item>
		/// </list>
		/// </remarks>
		/// <param name="content">The HTML content of a Url that must be parsed for links.
		/// It is passed by reference in order to reduce memory consumption.</param>
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
			bool FlagFetchRobots = contentUrl.FlagFetchRobots;
			try
			{
				//make sure only one thread will parse HTML contents at a time.
				//mutex.WaitOne();
				MatchCollection []matches = { null, null, null, null };
				//first perform the href tag matching
				matches[0]=ahrefRegex.Matches(content);
				//and then the frame tag matching
				matches[1]=frameRegex.Matches(content);
				//and then the flash tag matching
				matches[2]=flashRegex.Matches(content);
				//and finally the refresh meta tag matching
				matches[3]=refreshRegex.Matches(content);

				string documentUrl = contentUrl.Url;
				string baseUrl = BaseUrl(ref documentUrl, ref content);
				RobotsMetaTagValue robotsMeta = ExtractRobotsMetaTag(ref content);
				if(contentUrl.FlagDomain!=DomainFlagValue.MustVisit)
				{
					contentUrl.FlagDomain = ExtractDomainFlag(ref content);

					if (contentUrl.FlagDomain != DomainFlagValue.MustVisit)
						if (InternetUtils.HostName(contentUrl).Contains("ebay.com"))
							contentUrl.FlagDomain = DomainFlagValue.MustVisit;
				}
				byte priority = 0;
				int i=0;
				for(i=0; i<matches.Length; i++)
				{
					if(matches[i].Count>0)
					{
						foreach(Match m in matches[i])
						{
							try
							{
								string url = TrimUrl(m.Value);
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
										iurl.FlagRobots = robotsFilter.FilterUrl(url, contentUrl, robotsMeta);
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
									globals.FileLog.LogInfo("HtmlParser failed to parse " + m.Value);
								}
								continue;
							}
						}
					}
				}
				for(i=0; i<matches.Length; i++)
				{
					matches[i] = null;
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
			contentUrl.FlagFetchRobots = FlagFetchRobots;
			ParserEventArgs e = new ParserEventArgs(contentUrl.Url);
			OnExtractLinksComplete(e);
			links.TrimToSize();
			return links;
		}

		/// <summary>
		/// Performs the extraction of links from an html document. It can extract simple
		/// links, image map links and frame links. The results are returned as an <see cref="ArrayList"/>
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
		///     <description>Filtering of script urls (javascript and vbscript) and mailto urls, as well as stylesheets.</description>
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
		///     <term>12/09/04</term>
		///     <description>General revision, rewritten many functions from scratch.</description>
		///   </item>
		///   <item>
		///     <term>30/08/03</term>
		///     <description>Added support for filtering circular links.</description>
		///   </item>
		///   <item>
		///     <term>28/08/03</term>
		///     <description>Added support for limiting the number of parameters of dynamic urls.</description>
		///   </item>
		///   <item>
		///     <term>23/07/03</term>
		///     <description>Added support for removing session ids from dynamic urls.</description>
		///   </item>
		/// </list>
		/// </remarks>
		/// <param name="content">The HTML content of a Url that must be parsed for links.
		/// It is passed as an array of bytes containing the HTML contents in UTF8 binary
		/// format, in order to reduce memory consumption.</param>
		/// <param name="contentUrl">The Url from which the content comes.</param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects, one for each
		/// link found in the content.
		/// </returns>
		public override ArrayList ExtractLinks(byte []content, ref InternetUrlToCrawl contentUrl)
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
		/// Performs the extraction of text from the HTML contents of an html document. The
		/// text is extracted by removing all the tags in the document and compacting white
		/// space characters (consecutive) and other html entities, like &amp;nbsp;.
		/// </summary>
		/// <param name="content">The html contents of the document from which the text must
		/// be extracted.</param>
		/// <returns>A string containing the 'clean' text extracted from the document.</returns>
		public override string ExtractText(ref string content)
		{
			string retVal = String.Empty;
			StringBuilder sb=new StringBuilder(stripTagRegex.Replace(content," "));
			sb.Replace("\r\n", " ");
			sb.Replace('\n', ' ');
			sb.Replace('\t', ' ');
			sb.Replace("&nbsp;", " ");
			retVal = spacesRegex.Replace(sb.ToString()," ");
			return retVal;
		}

		/// <summary>
		/// Performs the extraction of text from the contents of an HTML document by removing
		/// all the html tags. Receives an array of bytes containing the UTF8 encoded format
		/// of a string.
		/// </summary>
		/// <param name="content">An array of bytes containing the UTF8 encoded format of
		/// an HTML document.</param>
		/// <returns>A string containing the 'clean' text extracted from the document.</returns>
		public override string ExtractText(byte[] content)
		{
			string retVal = String.Empty;
			try
			{
				string html = Encoding.UTF8.GetString(content);
				retVal = ExtractText(ref html);
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Performs the extraction of content from an HTML document. Depending on the value
		/// of the Flag provided it simply removes all kinds of scripts from the document or
		/// it removes the scripts and includes the content of the description and keywords
		/// meta tags. [Note: this is not implemented since this could allow techniques such
		/// as keyword stuffing to influence the page's ranking.]
		/// </summary>
		/// <param name="content">The html contents of the document from which the content
		/// must be extracted.</param>
		/// <param name="Flag">Determines what kind of processing will be performed on the
		/// input. If set to false only the various kinds of scripts are removed from the
		/// input. If set to false it could also extract information from meta tags, such as
		/// description and keyword meta tags. However this is not a good idea, so instead a
		/// <see cref="NotSupportedException"/> is thrown.</param>
		/// <returns>A string containing the desired extracted content.</returns>
		/// <exception cref="NotSupportedException">Thrown if the value of the flag is true.</exception>
		public override string ExtractContent(ref string content, bool Flag)
		{
			if(Flag)
			{
				throw new NotSupportedException();
			}
			string retVal = content;
			//string retVal = String.Empty;
			//retVal = scriptRegex.Replace(content, " ");
			//retVal = stylesRegex.Replace(retVal, " ");
			retVal = spacesRegex.Replace(retVal, " ");
			return retVal;
		}

		/// <summary>
		/// Performs the extraction of content from an HTML document. Depending on the value
		/// of the Flag provided it simply removes all kinds of scripts from the document or
		/// it removes the scripts and includes the content of the description and keywords
		/// meta tags. [Note: this is not implemented since this could allow techniques such
		/// as keyword stuffing to influence the page's ranking.]
		/// </summary>
		/// <param name="content">The html contents of the document from which the content
		/// must be extracted. Passed as an array of bytes containing the UTF8 format of the
		/// HTML content.</param>
		/// <param name="Flag">Determines what kind of processing will be performed on the
		/// input. If set to false only the various kinds of scripts are removed from the
		/// input. If set to true it could also extract information from meta tags, such as
		/// description and keyword meta tags. However this is not a good idea, so instead a
		/// <see cref="NotSupportedException"/> is thrown.</param>
		/// <returns>A string containing the desired extracted content.</returns>
		/// <exception cref="NotSupportedException">Thrown if the value of the flag is true.</exception>
		public override string ExtractContent(byte[] content, bool Flag)
		{
			if(Flag)
			{
				throw new NotSupportedException();
			}
			string retVal = String.Empty;
			try
			{
				string html = Encoding.UTF8.GetString(content);
				retVal = ExtractContent(ref html, Flag);
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Occurs when the extraction of links from an html document is complete
		/// </summary>
		public event ParserEventHandler ExtractLinksComplete;

		/// <summary>
		/// Occurs when the extraction of text from an html document is complete
		/// </summary>
		public event ParserEventHandler ExtractTextComplete;

		/// <summary>
		/// Occurs when the extraction of content from an html document is complete
		/// </summary>
		public event ParserEventHandler ExtractContentComplete;

		#endregion

		#region Private and protected members

		/// <summary>
		/// Clears the contents of a <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> object to be cleared.</param>
		private void ClearString(ref StringBuilder sb)
		{
			sb.Remove(0,sb.Length);
		}

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
		///		<term>11/05/03</term>
		///		<description>Initial release.</description>
		///   </item>
		///   <item>
		///		<term>16/09/03</term>
		///		<description>Better compliance with RFC1808, now able to parse the
		///		&lt;base href="..."&gt; meta tag.</description>
		///   </item>
		///   <item>
		///		<term>01/09/04</term>
		///		<description>Updated the code in order to improve performance and decrease
		///		memory consumption.</description>
		///   </item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if the input strings are null.</exception>
		/// <param name="contentUrl">
		/// The Url from which the base Url must be extracted.
		/// </param>
		/// <param name="content">
		/// The contents of the Url that must be parsed for the base meta tag.
		/// </param>
		/// <returns>A string containing the base Url of the given Url.</returns>
		protected internal string BaseUrl(ref string contentUrl, ref string content)
		{
			if(contentUrl == null || content == null)
			{
				throw new ArgumentNullException();
			}
			string retVal = null;
			try
			{
				//first the content must be searched for the <base href ...> meta tag
				Match baseMatch = baseRegex.Match(content);
				//If a match has been found then we must parse the tag to get the base url
				if(baseMatch.Success)
				{
					retVal = TrimUrl(baseMatch.Value);
				}
				else
				{
					//no base meta tag has been found, we need to parse the url
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
				baseMatch = null;
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo("HtmlParser.BaseUrl failed: " + e.ToString());
				}
			}
			return retVal;
		}

		/// <summary>
		/// Takes as input a string containing a Url probably surrounded by other tags and
		/// enclosed in quotes or double quotes and it trims out everything but the Url.
		/// </summary>
		/// <example>
		/// <code>
		/// string input = "a href=index.html target=_blank";
		/// string result = null;
		/// result = TrimUrl(input); // returns index.html
		/// input = "a class=\"text\" href=\"http://www.in.gr/\" target=\"_blank\"";
		/// result = TrimUrl(input); // returns http://www.in.gr/
		/// input = "frame width=500 src=\"index.html\" noresize&gt;";
		/// result = TrimUrl(input); // returns index.html
		/// input = "base src=\"http://www.aueb.gr/\"&gt;;
		/// result = TrimUrl(input); // returns http://www.aueb.gr/
		/// </code>
		/// </example>
		/// <remarks>
		/// This method uses <see cref="StringBuilder"/> objects to perform the processing,
		/// in order to increase performance and consume the least possible memory.
		/// </remarks>
		/// <param name="input">The input string containing the Url to be trimmed out.</param>
		/// <returns>A string containing the trimmed-out Url.</returns>
		protected internal string TrimUrl(string input)
		{
			string retVal=null;
			try
			{
				int startpos=0, endpos=0, offset=0;
				StringBuilder sb=new StringBuilder(input);
				input = input.ToLower(culture);
				// try to find the position of the "href" or "src" strings. Set the start
				// position to this position and the offset to the next position. Get the
				// substring from this position to the end of the input string.
				startpos = input.IndexOf(attributeNames[0]); //href
				if(startpos != -1)
				{
					offset = startpos+4;
					sb.Remove(0,offset);
				}
				else
				{
					startpos = input.IndexOf(attributeNames[1]); //src
					if(startpos!=-1)
					{
						offset = startpos+3;
						sb.Remove(0,offset);
					}
					else
					{
						//attempt to find refresh meta tag. It one exists no src attribute will exist
						startpos = input.IndexOf(attributeNames[2], offset); //http-equiv
						if(startpos!=-1)
						{
							startpos = input.IndexOf(attributeNames[3], startpos); //refresh
							if(startpos!=-1)
							{
								startpos = input.IndexOf(attributeNames[4], startpos); //content
								if(startpos!=-1)
								{
									startpos=input.IndexOf(attributeNames[5], startpos); //;url
									if(startpos!=-1)
									{
										sb.Remove(0,startpos+4-offset);
										offset = startpos+4-offset;
									}
								}
							}
						}
					}
				}
				//try to find the = character, delete it and all the characters before it
				startpos=input.IndexOf('=',offset);
				sb.Remove(0, startpos+1-offset);
				offset += startpos+1-offset;
				// Left-trim the input from spaces, quotes and double quotes, so as to move
				// to the position where the actual url starts
				while((sb.Length>0)&&((sb[0]==endDelimiters[0])||(sb[0]==endDelimiters[1])||(sb[0]==endDelimiters[3]))) //' ' or " or '
				{
					sb.Remove(0,1);
					offset++;
				}
				// Find the position of the next delimiter, which marks the end of the url.
				// Delete all the characters after this position.
				endpos=input.IndexOfAny(endDelimiters, offset);
				if(endpos!=-1)
				{
					while(sb.Length>endpos-offset)
					{
						sb.Remove(sb.Length-1,1);
					}
				}
				//Trim the final string from any delimiters and return it
				retVal=sb.ToString().Trim(endDelimiters);
			}
			catch(Exception e)
			{
				if(globals.Settings.LogLevel == CWLogLevel.LogInfo)
				{
					globals.FileLog.LogInfo("HtmlParser.TrimUrl failed: " + e.ToString() + "\nInput:" + input);
				}
			}
			return retVal;
		}

		/// <summary>
		/// Goes one directory up, e.g. http://www.in.gr/photos/menu/ -> http://www.in.gr/photos/
		/// </summary>
		/// <param name="url">The initial Url.</param>
		/// <returns>It's parent directory Url.</returns>
		protected internal string OneUp(ref string url)
		{
			string retVal = String.Empty;
			StringBuilder sb = new StringBuilder(url);
			//if there is a / at the end of the Url get rid of it
			if(url.EndsWith("/"))
			{
				sb.Remove(sb.Length-1,1);
			}
			//now try to see if there is a double slash (ie protocol)
			int doubleslashpos = url.IndexOf("//");
			//now start from the end trying to find the position of the last /
			int lastslashpos = 0;
			for (lastslashpos = sb.Length-1; lastslashpos >= 0; lastslashpos--)
			{
				if (sb[lastslashpos]=='/')
					break;
			}
			if (((doubleslashpos>0)&&(lastslashpos==doubleslashpos+1))||(lastslashpos==-1))
			{
				sb.Append('/');
				retVal = sb.ToString(); //it is a top-level directory, cannot be shortened
			}
			else
			{
				retVal = sb.ToString(0,lastslashpos)+'/';
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
			string retVal=String.Empty;
			//check if the url contains the protocol segment.
			if ((url.StartsWith("http"))||(url.StartsWith("ftp")))
			{
				//the Url is already in almost canonical form. We need to convert &amp; to &
				url = url.Replace("&amp;", "&");
				url = url.Replace("%26", "&");
				Uri uri=new Uri(url.Replace("&#38;","&"));
				retVal=uri.AbsoluteUri;//CombinePaths(baseUrl, url);
			}
			else
			{
				//simply combining the two paths will do.
				url = url.Replace("&amp;", "&");
				url = url.Replace("%26", "&");
				retVal=CombineUrls(baseUrl, url.Replace("&#38;","&"));
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
				retVal=String.Empty;
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
			string retVal=String.Empty;
			try
			{
				Uri baseUri= new Uri(baseUrl);
				Uri newUri = new Uri(baseUri,newUrl);
				retVal = newUri.AbsoluteUri;
				baseUri = null;
			}
			catch(UriFormatException)
			{
				retVal=String.Empty; //this will lead to losing a url
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
		///		<term>4/11/04</term>
		///		<description>Added support for checking for session IDs in hexadecimal or
		///		Guid format in all parameters, not only the ones whose name is in the list
		///		of common Session ID Variable names. 
		///		</description>
		///   </item>
		///   <item>
		///		<term>12/09/04</term>
		///		<description>Rewritten from scratch. All the processing is performed using
		///		<see cref="StringBuilder"/> objects to hold the strings. The priority of
		///		the url is calculated. The method now removes session id variables that are
		///		within the Query segment without removing all the parameters after such a
		///		variable is encountered. Improved efficiency and less memory consumption. 
		///		</description>
		///   </item>
		///   <item>
		///		<term>23/08/03</term>
		///		<description>
		///		Limit the number of parameters of dynamic urls to 3, remove named anchors.
		///		</description>
		///   </item>
		///   <item>
		///		<term>28/07/03</term>
		///		<description>Initial release, after noticing that there can be countless
		///		session ids for the same url, causing some urls to act as "black holes".
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
			StringBuilder sb = new StringBuilder(url);
			byte priority = 1;
			int pos=url.IndexOf('#'), i=0;
			if (pos!=-1)
			{
				//the url contains an anchor, so it must be cleaned
				sb.Remove(pos,url.Length-pos);
				priority++;
			}
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
			//now check for url parameters
			pos=url.IndexOfAny(parameterDelimiters);
			if (pos!=-1)
			{
				//it's a dynamic url, so it must be processed.
				priority+=2;
				sb.Remove(pos, sb.Length-pos);
				int numParams=0; bool foundSID=false;
				string [] urlParams = uri.Query.Split(parameterDelimiters, 20);
				foreach (string urlParam in urlParams)
				{
					if(urlParam!=String.Empty)
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
			//now check for inlined session id parameters
			if(inlinedSessionIDRegex.IsMatch(url))
			{
				url = inlinedSessionIDRegex.Replace(url,"/");
				priority +=2;
			}
			sb = null;
			return priority;
		}

		/// <summary>
		/// FilterUrl filters out urls that must not be visited, such as javascript: or
		/// mailto: urls. Returns true if the Url is OK and can be crawled and false if
		/// it must be rejected. This could be adapted to filter out urls that are not 
		/// part of a specific domain, eg .gr or it could be adapted to use some kind of
		/// rules like regular expressions that define which urls should be filtered out.
		/// <br/>Written: 19/5/03<br/>
		/// Updated:30/8/03 - Added support for removing circular links
		/// </summary>
		/// <param name="url">The url to examine</param>
		/// <param name="contentUrl">The url in whose the contents the examined Url is found</param>
		/// <returns>True if it is OK to crawl the url, false otherwise</returns>
		protected internal bool FilterUrl(ref string url, ref string contentUrl)
		{
			if ((url==String.Empty)||(url == null))
			{
				return false;
			}
			//check if it is a circular link
			if (url==contentUrl)
			{
				return false;
			}
			//check against the list of bad urls
			int i=0;
			for (i=0; i<badUrls.Length; i++)
			{
				if (url.IndexOf(badUrls[i])!=-1)
				{
					return false; //exit the loop as soon as we discover it's a bad url
				}
			}
			for (i=0; i<badExtensions.Length; i++)
			{
				if (url.IndexOf(badExtensions[i],6)!=-1)
				{
					return false; //exit the loop as soon as we discover it's a bad url
				}
			}
			return true;
		}

		/// <summary>
		/// Parses the content of an HTML page for the robots meta tag and if one exists it
		/// creates a <see cref="RobotsMetaTagValue"/> corresponding to the options it sets.
		/// </summary>
		/// <param name="content">The HTML content that must be parsed for the robots meta tag.</param>
		/// <returns>A <see cref="RobotsMetaTagValue"/> corresponding to the robots meta tag options of the document.</returns>
		private RobotsMetaTagValue ExtractRobotsMetaTag(ref string content)
		{
			RobotsMetaTagValue retVal = RobotsMetaTagValue.NoMeta;
			try
			{
				//first of all check if a robots meta tag exists
				Match match = robotRegex.Match(content);
				if(match.Success)
				{
					string tag = match.Value.ToLower(culture);
					if(tag.IndexOf("noindex")!=-1)
					{
						//The NoIndex option is set
						retVal ^= RobotsMetaTagValue.Index;
						retVal |= RobotsMetaTagValue.NoIndex;
					}
					if(tag.IndexOf("nofollow")!=-1)
					{
						//The NoFollow option is set
						retVal ^= RobotsMetaTagValue.Follow;
						retVal |= RobotsMetaTagValue.NoFollow;
					}
					if(tag.IndexOf("all")!=-1)
					{
						retVal = RobotsMetaTagValue.IndexFollow;
					}
					if(tag.IndexOf("none")!=-1)
					{
						retVal = (RobotsMetaTagValue.NoIndex | RobotsMetaTagValue.NoFollow);
					}
				}
				match = null;
			}
			catch
			{}
			return retVal;
		}

		/// <summary>
		/// Attempts to extract the appropriate FlagDomain value from the contents of the page.
		/// </summary>
		/// <param name="content">The HTML content that must be parsed for Domain Flag value.</param>
		/// <returns>A <see cref="DomainFlagValue"/> indicating whether the content of the 
		/// HTML document is in the language that interests us.
		/// </returns>
		/// <remarks>
		/// The method first attempts to perform a regular expression math for a charset tag,
		/// if this doesn't succeed it attempts to find greek letters in the content.
		/// </remarks>
		private DomainFlagValue ExtractDomainFlag(ref string content)
		{
			DomainFlagValue retVal = DomainFlagValue.Unknown;
			try
			{
				//attempt to perform a meta http-equiv tag matching
				Match match = charsetRegex.Match(content);
				if(match.Success)
				{
					//a match was found, so the Url must be flagged with the MustVisit value
					retVal = DomainFlagValue.MustVisit;
				}
				else
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
				match = null;
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
