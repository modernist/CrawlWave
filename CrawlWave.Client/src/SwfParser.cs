using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using CrawlWave.Common;
using CrawlWaveSwf2Html;

namespace CrawlWave.Client
{
	/// <summary>
	/// SwfParser is a Singleton class that performs the link extraction and parsing of
	/// the contents of shockwave flash movies and generally documents with content type
	/// "application/x-shockwave-flash".
	/// </summary>
	/// <remarks>
	/// The SwfParser class uses a COM library (CrawlWave.Swf2Html) to perform the whole
	/// processing of the shockwave flash documents. It first converts the document into
	/// a temporary html document and then uses an instance of <see cref="HtmlParser"/>
	/// to extract the links or text it contains. It needs to use temporary storage for
	/// the intermediate html documents, which are stored in the client's work directory.
	/// </remarks>
	public class SwfParser : Parser, IParser
	{
		#region Private variables

		private static SwfParser instance; //The single class instance
		private Mutex mutex; //Mutex supporting safe access from multiple threads
		private Encoding encoding;  //ISO-8859-7 encoding is needed to read the html files
		private CSwf2HtmlConverterClass converter; //The class that performs the conversion
		private HtmlParser parser; //This will be used to extract links after the conversion
		private Globals globals; //Provides access to the global variables and application settings
		private const string supportedContentType="application/x-shockwave-flash"; //The Content Type supported by the parser

		#endregion

		#region Constructor and Singleton Instance members

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private SwfParser()
		{
			//Initialize the synchronization mechanism
			mutex=new Mutex();
			//Initialize the Encoding
			encoding = Encoding.UTF8;//GetEncoding("ISO-8859-7");
			//Initialize the converters and parsers
			converter = new CSwf2HtmlConverterClass();
			parser = HtmlParser.Instance();
			//Get a reference to the global variables and application settings
			globals = Globals.Instance();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="SwfParser"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="SwfParser"/></returns>
		public static SwfParser Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new SwfParser();
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
		/// Extracts links from the contents of a SWF document.
		/// </summary>
		/// <param name="content">The contents of the SWF document.</param>
		/// <param name="contentUrl">The url of the PDF document.</param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects, one for
		/// each link found in the content.
		/// </returns>
		/// <exception cref="NotSupportedException">Whenever this method is called.</exception>
		/// <remarks>
		/// Since a SWF document can not be converted to a string this method <b>ALWAYS</b>
		/// throws a <see cref="NotSupportedException"/>.
		/// </remarks>
		public override ArrayList ExtractLinks(ref string content, ref InternetUrlToCrawl contentUrl)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Extracts links from the contents of a SWF document.
		/// </summary>
		/// <param name="content">The contents of the SWF document.</param>
		/// <param name="contentUrl">The url of the SWF document.</param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects, one for
		/// each link found in the content.
		/// </returns>
		/// <exception cref="ArgumentNullException">If the input buffer is null or empty.</exception>
		public override ArrayList ExtractLinks(byte[] content, ref InternetUrlToCrawl contentUrl)
		{
			ArrayList links=null;
			if((content==null)||(content.Length==0))
			{
				throw new ArgumentNullException("content", "The input buffer cannot be empty or null.");
			}
			try
			{
				mutex.WaitOne();
				string FileName = globals.AppWorkPath + Guid.NewGuid().ToString();
				string swfFileName = FileName + ".swf";
				string htmFileName = FileName + ".htm";
				FileStream swf = null;
				StreamReader htm = null;
				try
				{
					//store the swf file
					swf = new FileStream(swfFileName,FileMode.Create);
					swf.Write(content, 0, content.Length);
					swf.Close();
					swf = null;
					//convert it to html
					bool success = converter.ConvertSwfFile(swfFileName, htmFileName);
					if(success)
					{
						htm = new StreamReader(htmFileName, encoding);
						string html = htm.ReadToEnd();
						htm.Close();
						htm = null;
						links = parser.ExtractLinks(ref html, ref contentUrl);
					}
				}
				catch(Exception ex)
				{
					if(swf!=null)
					{
						try
						{
							swf.Close();
						}
						catch
						{}
					}
					if(htm!=null)
					{
						try
						{
							htm.Close();
						}
						catch
						{}
					}
					if(globals.Settings.LogLevel <= CWLogLevel.LogInfo)
					{
						globals.FileLog.LogInfo("SwfParser failed to extract links from " + contentUrl.Url + ": " + ex.ToString()); 
					}
				}
				finally
				{
					File.Delete(swfFileName);
					File.Delete(htmFileName);
				}
			}
			catch(Exception ex)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("SwfParser failed to extract links from " + contentUrl.Url + ": " + ex.Message); 
				}
			}
			finally
			{
				GC.Collect();
				mutex.ReleaseMutex();
			}
			ParserEventArgs e = new ParserEventArgs(contentUrl.Url);
			OnExtractLinksComplete(e);
			return links;
		}

		/// <summary>
		/// Extracts text from the contents of a SWF document.
		/// </summary>
		/// <param name="content">The contents of the SWF document.</param>
		/// <returns>The text extracted from the SWF document.</returns>
		/// <exception cref="NotSupportedException">Whenever this method is called.</exception>
		/// <remarks>
		/// Since a SWF document can not be converted to a string this method <b>ALWAYS</b>
		/// throws a <see cref="NotSupportedException"/>.
		/// </remarks>
		public override string ExtractText(ref string content)
		{
			throw new NotSupportedException("The ExtractText method of the SwfParser cannot accept a string as input.");
		}

		/// <summary>
		/// Extracts text from the contents of a SWF document.
		/// </summary>
		/// <param name="content">The contents of the SWF document.</param>
		/// <returns>The text extracted from the SWF document.</returns>
		/// <exception cref="ArgumentNullException">If the input buffer is null or empty.</exception>
		/// <remarks>
		/// </remarks>
		public override string ExtractText(byte[] content)
		{
			string retVal = String.Empty;
			if((content==null)||(content.Length==0))
			{
				throw new ArgumentNullException("content", "The input buffer cannot be empty or null.");
			}
			try
			{
				mutex.WaitOne();
				string FileName = globals.AppWorkPath + Guid.NewGuid().ToString();
				string swfFileName = FileName + ".swf";
				string htmFileName = FileName + ".htm";
				FileStream swf = null;
				StreamReader htm = null;
				try
				{
					//store the swf file
					swf = new FileStream(swfFileName,FileMode.Create);
					swf.Write(content, 0, content.Length);
					swf.Close();
					swf = null;
					//convert it to html
					bool success = converter.ConvertSwfFile(swfFileName, htmFileName);
					if(success)
					{
						htm = new StreamReader(htmFileName, encoding);
						string html = htm.ReadToEnd();
						htm.Close();
						htm = null;
						retVal = parser.ExtractText(ref html);
					}
				}
				catch(Exception ex)
				{
					if(swf!=null)
					{
						try
						{
							swf.Close();
						}
						catch
						{}
					}
					if(htm!=null)
					{
						try
						{
							htm.Close();
						}
						catch
						{}
					}
					if(globals.Settings.LogLevel <= CWLogLevel.LogInfo)
					{
						globals.FileLog.LogWarning("SwfParser failed to extract text: " + ex.ToString()); 
					}
				}
				finally
				{
					File.Delete(swfFileName);
					File.Delete(htmFileName);
				}
			}
			catch(Exception ex)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("SwfParser failed to extract text: " + ex.Message); 
				}
			}
			finally
			{
				GC.Collect();
				mutex.ReleaseMutex();
			}
			ParserEventArgs e = new ParserEventArgs(String.Empty);
			OnExtractTextComplete(e);
			return retVal;
		}

		/// <summary>
		/// Extracts the desired contents of a SWF document.
		/// </summary>
		/// <param name="content">The contents of the SWF document.</param>
		/// <param name="Flag">
		/// If set to false it simply returns a string containing the HTML format of the 
		/// input. If set to true it returns the text format of the input after performing
		/// a white space compaction.
		/// </param>
		/// <returns>The contents extracted from the SWF document.</returns>
		/// <exception cref="NotSupportedException">Whenever this method is called.</exception>
		public override string ExtractContent(ref string content, bool Flag)
		{
			throw new NotSupportedException("The ExtractContent method of the SwfParser cannot accept a string as input.");
		}

		/// <summary>
		/// Performs the extraction of content from a SWF document. Depending on the value
		/// of the Flag provided it simply returns a string containing the HTML format of 
		/// the input or it returns the text format of the input after performing a white
		/// space compaction.
		/// </summary>
		/// <param name="content">
		/// The contents of the document from which the content must be extracted.
		/// </param>
		/// <param name="Flag">Determines what kind of processing will be performed on the
		/// input. If set to false it simply returns a string containing the HTML format of 
		/// the input. If set to true it returns the text format of the input after performing
		/// a white space compaction.
		/// </param>
		/// <returns>A string containing the desired extracted content.</returns>
		/// <exception cref="ArgumentNullException">If the input buffer is null or empty.</exception>
		public override string ExtractContent(byte[] content, bool Flag)
		{
			string retVal = null;
			if((content==null)||(content.Length==0))
			{
				throw new ArgumentNullException("content", "The input buffer cannot be empty or null.");
			}
			try
			{
				mutex.WaitOne();
				string FileName = globals.AppWorkPath + Guid.NewGuid().ToString();
				string swfFileName = FileName + ".swf";
				string htmFileName = FileName + ".htm";
				FileStream swf = null;
				StreamReader htm = null;
				try
				{
					//store the swf file
					swf = new FileStream(swfFileName,FileMode.Create);
					swf.Write(content, 0, content.Length);
					swf.Close();
					swf = null;
					//convert it to html
					bool success = converter.ConvertSwfFile(swfFileName, htmFileName);
					if(success)
					{
						htm = new StreamReader(htmFileName, encoding);
						string html = htm.ReadToEnd();
						htm.Close();
						htm = null;
						if(!Flag)
						{
							retVal = html;
						}
						else
						{
							retVal = parser.ExtractText(ref html);
						}
					}
				}
				catch(Exception ex)
				{
					if(swf!=null)
					{
						try
						{
							swf.Close();
						}
						catch
						{}
					}
					if(htm!=null)
					{
						try
						{
							htm.Close();
						}
						catch
						{}
					}
					if(globals.Settings.LogLevel <= CWLogLevel.LogInfo)
					{
						globals.FileLog.LogWarning("SwfParser failed to extract text: " + ex.ToString()); 
					}
				}
				finally
				{
					File.Delete(swfFileName);
					File.Delete(htmFileName);
				}
			}
			catch(Exception ex)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("SwfParser failed to extract content: " + ex.Message); 
				}
			}
			finally
			{
				GC.Collect();
				mutex.ReleaseMutex();
			}
			ParserEventArgs e = new ParserEventArgs(String.Empty);
			OnExtractContentComplete(e);
			return retVal;
		}

		/// <summary>
		/// Occurs when the extraction of links from a SWF document is complete
		/// </summary>
		public event ParserEventHandler ExtractLinksComplete;

		/// <summary>
		/// Occurs when the extraction of text from a SWF document is complete
		/// </summary>
		public event ParserEventHandler ExtractTextComplete;

		/// <summary>
		/// Occurs when the extraction of content from a SWF document is complete
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
