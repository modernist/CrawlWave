using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using CrawlWave.Common;

namespace CrawlWave.Client
{
	/// <summary>
	/// NullParser is the default parser that is used when the crawler visits a document of
	/// an unsupported content type.
	/// </summary>
	public class NullParser : Parser, IParser
	{
		#region Private variables

		private static NullParser instance; //The single class instance
		private const string supportedContentType=""; //The Content Type supported by the parser

		#endregion

		#region Constructor and Singleton Instance members

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private NullParser()
		{
			//Nothing to be done here
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="NullParser"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="NullParser"/></returns>
		public static NullParser Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new NullParser();
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
		/// Extracts links from the contents of a document.
		/// </summary>
		/// <param name="content">The contents of the document.</param>
		/// <param name="contentUrl">The url of the document.</param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects, one for
		/// each link found in the content.
		/// </returns>
		/// <remarks>
		/// This method <b>ALWAYS</b> returns an empty ArrayList.
		/// </remarks>
		public override ArrayList ExtractLinks(ref string content, ref InternetUrlToCrawl contentUrl)
		{
			ArrayList links=new ArrayList();
			ParserEventArgs e = new ParserEventArgs(contentUrl.Url);
			OnExtractLinksComplete(e);
			return links;
		}

		/// <summary>
		/// Extracts links from the contents of a document.
		/// </summary>
		/// <param name="content">The contents of the document.</param>
		/// <param name="contentUrl">The url of the document.</param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects, one for
		/// each link found in the content.
		/// </returns>
		/// <remarks>This method <b>ALWAYS</b> returns an empty ArrayList.</remarks>
		public override ArrayList ExtractLinks(byte[] content, ref InternetUrlToCrawl contentUrl)
		{
			ArrayList links=new ArrayList();
			ParserEventArgs e = new ParserEventArgs(contentUrl.Url);
			OnExtractLinksComplete(e);
			return links;
		}

		/// <summary>
		/// Extracts text from the contents of a document.
		/// </summary>
		/// <param name="content">The contents of the document.</param>
		/// <returns>The text extracted from the document.</returns>
		/// <remarks>This method <b>ALWAYS</b> returns an empty string.</remarks>
		public override string ExtractText(ref string content)
		{
			string retVal = String.Empty;
			ParserEventArgs e = new ParserEventArgs(String.Empty);
			OnExtractTextComplete(e);
			return retVal;
		}

		/// <summary>
		/// Extracts text from the contents of a document.
		/// </summary>
		/// <param name="content">The contents of the document.</param>
		/// <returns>The text extracted from the document.</returns>
		/// <remarks>This method <b>ALWAYS</b> returns an empty string.</remarks>
		public override string ExtractText(byte[] content)
		{
			string retVal = String.Empty;
			ParserEventArgs e = new ParserEventArgs(String.Empty);
			OnExtractTextComplete(e);
			return retVal;
		}

		/// <summary>
		/// Extracts the desired contents of a document.
		/// </summary>
		/// <param name="content">The contents of the document.</param>
		/// <param name="Flag">The parameter is not used in this method.</param>
		/// <returns>The contents extracted from the document.</returns>
		/// <remarks>This method <b>ALWAYS</b> returns an empty string.</remarks>
		public override string ExtractContent(ref string content, bool Flag)
		{
			string retVal = String.Empty;
			ParserEventArgs e = new ParserEventArgs(String.Empty);
			OnExtractContentComplete(e);
			return retVal;
		}

		/// <summary>
		/// Performs the extraction of content from a document.
		/// </summary>
		/// <param name="content">
		/// The contents of the document from which the content must be extracted.
		/// </param>
		/// <param name="Flag">The parameter is not used in this method.</param>
		/// <returns>A string containing the desired extracted content.</returns>
		/// <remarks>This method <b>ALWAYS</b> returns an empty string.</remarks>
		public override string ExtractContent(byte[] content, bool Flag)
		{
			string retVal = String.Empty;
			ParserEventArgs e = new ParserEventArgs(String.Empty);
			OnExtractContentComplete(e);
			return retVal;
		}

		/// <summary>
		/// Occurs when the extraction of links from a document is complete
		/// </summary>
		public event ParserEventHandler ExtractLinksComplete;

		/// <summary>
		/// Occurs when the extraction of text from a document is complete
		/// </summary>
		public event ParserEventHandler ExtractTextComplete;

		/// <summary>
		/// Occurs when the extraction of content from a document is complete
		/// </summary>
		public event ParserEventHandler ExtractContentComplete;

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
