using System;
using System.Collections;
using CrawlWave.Common;

namespace CrawlWave.Client
{
	/// <summary>
	/// Encapsulates the data returned by a <see cref="Parser"/> object when a parsing event
	/// is raised. The only data that interests us is the Url that was processed.
	/// </summary>
	public class ParserEventArgs : EventArgs
	{
		private string url = String.Empty;

		/// <summary>
		/// Gets a string containing the Url that is related to the event.
		/// </summary>
		public string Url
		{
			get { return url; }
		}

		/// <summary>
		/// Constructs an instance of the <see cref="ParserEventArgs"/> class.
		/// </summary>
		/// <param name="Url">The Url related to the event.</param>
		public ParserEventArgs(string Url)
		{
			url = Url;
		}
	}

	/// <summary>
	/// Defines the interface of the methods that can act as listeners for events raised by
	/// <see cref="Parser"/> objects.
	/// </summary>
	public delegate void ParserEventHandler(object sender, ParserEventArgs e);
    
	/// <summary>
	/// Defines the common interface for all Parser classes that can be used to extract
	/// text and links from documents of different content type.
	/// </summary>
	public interface IParser
	{
		/// <summary>
		/// Gets a string containing the Content Type supported by a class implementing the
		/// <see cref="IParser"/> interface.
		/// </summary>
		string ContentType
		{
			get;
		}

		/// <summary>
		/// Extracts the hypertext references (links) contained in a document.
		/// </summary>
		/// <param name="content">
		/// The content of the document that will be parsed for links.
		/// </param>
		/// <param name="contentUrl">
		/// An <see cref="InternetUrlToCrawl"/> object encapsulating the Uri address of the
		/// document to be parsed for links and its associated robots.txt file.
		/// </param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects encapsulating
		/// the links contained in the parsed document.
		/// </returns>
		ArrayList ExtractLinks(ref string content, ref InternetUrlToCrawl contentUrl);

		/// <summary>
		/// Extracts the hypertext references (links) contained in a document.
		/// </summary>
		/// <param name="content">
		/// An array of bytes holding the content of the document that will be parsed for links.
		/// </param>
		/// <param name="contentUrl">
		/// An <see cref="InternetUrlToCrawl"/> object encapsulating the Uri address of the
		/// document to be parsed for links and its associated robots.txt file.
		/// </param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects encapsulating
		/// the links contained in the parsed document.
		/// </returns>
		ArrayList ExtractLinks(byte []content, ref InternetUrlToCrawl contentUrl);

		/// <summary>
		/// Extracts the contents of the document in plain text format.
		/// </summary>
		/// <param name="content">
		/// The content of the document from which the text must be extracted.
		/// </param>
		/// <returns>
		/// A string containing the document's content in plain text format.
		/// </returns>
		string ExtractText(ref string content);

		/// <summary>
		/// Extracts the contents of the document in plain text format.
		/// </summary>
		/// <param name="content">
		/// An array of bytes holding the content of the document from which the text must be extracted.
		/// </param>
		/// <returns>
		/// A string containing the document's content in plain text format.
		/// </returns>
		string ExtractText(byte []content);

		/// <summary>
		/// Performs a special processing of a document in order to extract some parts of
		/// its contents.
		/// </summary>
		/// <param name="content">
		/// The content of the document that will go through the processing.
		/// </param>
		/// <param name="Flag">
		/// A generic boolean flag that can be used to indicate what kind of processing
		/// must be performed on the document.
		/// </param>
		/// <returns>
		/// A string containing the requested contents of a document.
		/// </returns>
		string ExtractContent(ref string content, bool Flag);

		/// <summary>
		/// Performs a special processing of a document in order to extract some parts of
		/// its contents.
		/// </summary>
		/// <param name="content">
		/// A byte array holding the content of the document that will go through the processing.
		/// </param>
		/// <param name="Flag">
		/// A generic boolean flag that can be used to indicate what kind of processing
		/// must be performed on the document.
		/// </param>
		/// <returns>
		/// A string containing the requested contents of a document.
		/// </returns>
		string ExtractContent(byte []content, bool Flag);

		/// <summary>
		/// Occurs when the extraction of links from a url is complete.
		/// </summary>
		event ParserEventHandler ExtractLinksComplete;

		/// <summary>
		/// Occurs when the extraction of text from a url is complete.
		/// </summary>
		event ParserEventHandler ExtractTextComplete;

		/// <summary>
		/// Occurs when the extraction of content from a url is complete.
		/// </summary>
		event ParserEventHandler ExtractContentComplete;

	}

	/// <summary>
	/// The base class for all classes that parse documents of different content types.
	/// </summary>
	public abstract class Parser
	{
		#region IParser Members

		/// <summary>
		/// Gets a string containing the Content Type supported by a class implementing the
		/// <see cref="IParser"/> interface.
		/// </summary>
		public abstract string ContentType
		{
			get;
		}

		/// <summary>
		/// Extracts the hypertext references (links) contained in a document.
		/// </summary>
		/// <param name="content">
		/// The content of the document that will be parsed for links.
		/// </param>
		/// <param name="contentUrl">
		/// An <see cref="InternetUrlToCrawl"/> object encapsulating the Uri address of the
		/// document to be parsed for links and its associated robots.txt file.
		/// </param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects encapsulating
		/// the links contained in the parsed document.
		/// </returns>
		public abstract ArrayList ExtractLinks(ref string content, ref InternetUrlToCrawl contentUrl);
		
		/// <summary>
		/// Extracts the hypertext references (links) contained in a document.
		/// </summary>
		/// <param name="content">
		/// An array of bytes holding the content of the document that will be parsed for links.
		/// </param>
		/// <param name="contentUrl">
		/// An <see cref="InternetUrlToCrawl"/> object encapsulating the Uri address of the
		/// document to be parsed for links and its associated robots.txt file.
		/// </param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects encapsulating
		/// the links contained in the parsed document.
		/// </returns>
		public abstract ArrayList ExtractLinks(byte[] content, ref InternetUrlToCrawl contentUrl);

		/// <summary>
		/// Extracts the contents of the document in plain text format.
		/// </summary>
		/// <param name="content">
		/// The content of the document from which the text must be extracted.
		/// </param>
		/// <returns>
		/// A string containing the document's content in plain text format.
		/// </returns>
		public abstract string ExtractText(ref string content);

		/// <summary>
		/// Extracts the contents of the document in plain text format.
		/// </summary>
		/// <param name="content">
		/// An array of bytes holding the content of the document from which the text must be extracted.
		/// </param>
		/// <returns>
		/// A string containing the document's content in plain text format.
		/// </returns>
		public abstract string ExtractText(byte[] content);

		/// <summary>
		/// Performs a special processing of a document in order to extract some parts of
		/// its contents.
		/// </summary>
		/// <param name="content">
		/// The content of the document that will go through the processing.
		/// </param>
		/// <param name="Flag">
		/// A generic boolean flag that can be used to indicate what kind of processing
		/// must be performed on the document.
		/// </param>
		/// <returns>
		/// A string containing the requested contents of a document.
		/// </returns>
		public abstract string ExtractContent(ref string content, bool Flag);

		/// <summary>
		/// Performs a special processing of a document in order to extract some parts of
		/// its contents.
		/// </summary>
		/// <param name="content">
		/// A byte array holding the content of the document that will go through the processing.
		/// </param>
		/// <param name="Flag">
		/// A generic boolean flag that can be used to indicate what kind of processing
		/// must be performed on the document.
		/// </param>
		/// <returns>
		/// A string containing the requested contents of a document.
		/// </returns>
		public abstract string ExtractContent(byte[] content, bool Flag);

		#endregion
	}

}