using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using CrawlWave.Common;
using XpdfTextNet;

namespace CrawlWave.Client
{
	/// <summary>
	/// PdfParser is a Singleton class that performs the link extraction and parsing of
	/// the contents of Adobe Portable Document Format documents (PDF) and generally of
	/// documents with content type "application/pdf" or "application/x-pdf".
	/// </summary>
	/// <remarks>
	/// The PdfParser class uses a COM library (XPdfText) by Glyph&amp;Cog, which is based
	/// on the open source XPDF Project (http://foolabs.com/xpdf), to perform the whole
	/// processing of the PDF documents. It first converts the document into a temporary
	/// text document and then uses an instance of <see cref="TextParser"/> in order to
	/// extract the links or text it contains. It needs to use temporary storage for the
	/// intermediate text documents, which are stored in the client's work directory.
	/// </remarks>
	public class PdfParser : Parser, IParser
	{
		#region Private variables

		private static PdfParser instance; //The single class instance
		private Mutex mutex;		//Mutex supporting safe access from multiple threads
		private Encoding encoding;  //ISO-8859-7 encoding is needed to read the text files
		private XpdfTextClass converter; 
		//The object that will be used to perform the PDF to text file conversion
		private TextParser parser;
		//The object that will be used to perform the link, text and content extraction
		private Globals globals; //Provides access to the global variables and application settings
		private const string supportedContentType="application/pdf";
		//The Content Type supported by the parser
		private const string alternativeContentType = "application/x-pdf";
		//The alternative content type description (PDF has 2 IANA reserved content-types)

		#endregion

		#region Constructor and Singleton Instance members

		/// <summary>
		/// The constructor is private so that only the class itself can create an instance.
		/// </summary>
		private PdfParser()
		{
			//Initialize the synchronization mechanism
			mutex=new Mutex();
			//Initialize the encoding
			encoding=Encoding.GetEncoding("ISO-8859-7");
			//Initialize the converters and parsers
			converter = new XpdfTextClass();
			parser = TextParser.Instance();
			//Get a reference to the global variables and application settings
			globals = Globals.Instance();
		}

		/// <summary>
		/// Provides a global access point for the single instance of the <see cref="PdfParser"/>
		/// class.
		/// </summary>
		/// <returns>A reference to the single instance of <see cref="PdfParser"/></returns>
		public static PdfParser Instance()
		{
			if (instance==null)
			{
				//Make sure the call is thread-safe. We cannot use the private mutex since
				//it hasn't yet been initialized - it gets initialized in the constructor.
				Mutex imutex=new Mutex();
				imutex.WaitOne();
				if( instance == null )
				{
					instance = new PdfParser();
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
		/// Gets a string indicating the Content Type of the documents supported by the parser.
		/// </summary>
		public string AlternativeContentType
		{
			get { return alternativeContentType; }
		}

		/// <summary>
		/// Extracts links from the contents of a PDF document.
		/// </summary>
		/// <param name="content">The contents of the PDF document.</param>
		/// <param name="contentUrl">The url of the PDF document.</param>
		/// <returns>
		/// An <see cref="ArrayList"/> of <see cref="InternetUrlToIndex"/> objects, one for
		/// each link found in the content.
		/// </returns>
		/// <exception cref="NotSupportedException">Whenever this method is called.</exception>
		/// <remarks>
		/// Since a PDF document can not be converted to a string this method <b>ALWAYS</b>
		/// throws a <see cref="NotSupportedException"/>.
		/// </remarks>
		public override ArrayList ExtractLinks(ref string content, ref InternetUrlToCrawl contentUrl)
		{
			throw new NotSupportedException("The ExtractLinks method of the PdfParser cannot accept a string as input.");
		}

		/// <summary>
		/// Extracts links from the contents of a PDF document.
		/// </summary>
		/// <param name="content">The contents of the PDF document.</param>
		/// <param name="contentUrl">The url of the PDF document.</param>
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
				string pdfFileName = FileName + ".pdf";
				string txtFileName = FileName + ".txt";
				FileStream pdf = null;
				StreamReader txt = null;
				try
				{
					//store the pdf file
					pdf = new FileStream(pdfFileName,FileMode.Create);
					pdf.Write(content, 0, content.Length);
					pdf.Close();
					pdf = null;
					bool success = false;
					//convert it to text
					try
					{
						converter.loadFile(pdfFileName);
						converter.convertToTextFile(1, converter.numPages, txtFileName);
						success = true;
					}
					catch
					{
						success = false;
					}
					finally
					{
						converter.closeFile();
					}
					if(success)
					{
						txt = new StreamReader(txtFileName, encoding);
						string text = txt.ReadToEnd();
						txt.Close();
						txt = null;
						links = parser.ExtractLinks(ref text, ref contentUrl);
					}
					else
					{
						txt = null;
					}
				}
				catch(Exception ex)
				{
					if(pdf!=null)
					{
						try
						{
							pdf.Close();
						}
						catch
						{}
					}
					if(txt!=null)
					{
						try
						{
							txt.Close();
						}
						catch
						{}
					}
					if(globals.Settings.LogLevel <= CWLogLevel.LogInfo)
					{
						globals.FileLog.LogWarning("PdfParser failed to extract links from " + contentUrl.Url + ": " + ex.ToString()); 
					}
				}
				finally
				{
					File.Delete(pdfFileName);
					File.Delete(txtFileName);
				}
			}
			catch
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("PdfParser failed to extract links from " + contentUrl.Url); 
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
		/// Extracts text from the contents of a PDF document.
		/// </summary>
		/// <param name="content">The contents of the PDF document.</param>
		/// <returns>The text extracted from the PDF document.</returns>
		/// <exception cref="NotSupportedException">Whenever this method is called.</exception>
		/// <remarks>
		/// Since a PDF document can not be converted to a string this method <b>ALWAYS</b>
		/// throws a <see cref="NotSupportedException"/>.
		/// </remarks>
		public override string ExtractText(ref string content)
		{
			throw new NotSupportedException("The ExtractText method of the PdfParser cannot accept a string as input.");
		}

		/// <summary>
		/// Extracts text from the contents of a PDF document.
		/// </summary>
		/// <param name="content">The contents of the PDF document.</param>
		/// <returns>The text extracted from the PDF document.</returns>
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
				string pdfFileName = FileName + ".pdf";
				string txtFileName = FileName + ".txt";
				FileStream pdf = null;
				StreamReader txt = null;
				try
				{
					//store the pdf file
					pdf = new FileStream(pdfFileName,FileMode.Create);
					pdf.Write(content, 0, content.Length);
					pdf.Close();
					pdf = null;
					bool success = false;
					//convert it to text
					try
					{
						converter.loadFile(pdfFileName);
						converter.convertToTextFile(1, converter.numPages, txtFileName);
						success = true;
					}
					catch
					{
						success = false;
					}
					finally
					{
						converter.closeFile();
					}
					if(success)
					{
						txt = new StreamReader(txtFileName, encoding);
						string text = txt.ReadToEnd();
						txt.Close();
						txt = null;
						retVal = parser.ExtractText(ref text);
					}
					else
					{
						txt = null;
					}
				}
				catch(Exception ex)
				{
					if(pdf!=null)
					{
						try
						{
							pdf.Close();
						}
						catch
						{}
					}
					if(txt!=null)
					{
						try
						{
							txt.Close();
						}
						catch
						{}
					}
					if(globals.Settings.LogLevel <= CWLogLevel.LogInfo)
					{
						globals.FileLog.LogWarning("PdfParser failed to extract text: " + ex.ToString()); 
					}
				}
				finally
				{
					File.Delete(pdfFileName);
					File.Delete(txtFileName);
				}
			}
			catch(Exception ex)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("PdfParser failed to extract text: " + ex.Message); 
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
		/// Performs the extraction of content from a PDF document. Depending on the value
		/// of the Flag provided it simply returns a string same as the text produced from
		/// the parsing of the PDF document or it removes consecutive whitespace characters
		/// in order to perform a compaction.
		/// </summary>
		/// <param name="content">
		/// The contents of the document from which the content must be extracted.
		/// </param>
		/// <param name="Flag">Determines what kind of processing will be performed on the
		/// input. If set to false it simply returns a string same to the input. If set to
		/// true it performs whitespace compaction.
		/// </param>
		/// <returns>A string containing the desired extracted content.</returns>
		/// <exception cref="NotSupportedException">Whenever this method is called.</exception>
		/// <remarks>
		/// Since a PDF document can not be converted to a string this method <b>ALWAYS</b>
		/// throws a <see cref="NotSupportedException"/>.
		/// </remarks>
		public override string ExtractContent(ref string content, bool Flag)
		{
			throw new NotSupportedException("The ExtractContent method of the PdfParser cannot accept a string as input.");
		}

		/// <summary>
		/// Performs the extraction of content from a PDF document. Depending on the value
		/// of the Flag provided it simply returns a string same as the text produced from
		/// the parsing of the PDF document or it removes consecutive whitespace characters
		/// in order to perform a compaction.
		///  </summary>
		/// <param name="content">
		/// The contents of the document from which the content must be extracted.
		/// </param>
		/// <param name="Flag">Determines what kind of processing will be performed on the
		/// input. If set to false it simply returns a string same as the text produced from
		/// the parsing of the PDF document. If set to true it removes consecutive white
		/// space characters in order to perform a compaction.
		/// </param>
		/// <returns>A string containing the desired extracted content.</returns>
		/// <exception cref="ArgumentNullException">If the input buffer is null or empty.</exception>
		public override string ExtractContent(byte[] content, bool Flag)
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
				string pdfFileName = FileName + ".pdf";
				string txtFileName = FileName + ".txt";
				FileStream pdf = null;
				StreamReader txt = null;
				try
				{
					//store the pdf file
					pdf = new FileStream(pdfFileName,FileMode.Create);
					pdf.Write(content, 0, content.Length);
					pdf.Close();
					pdf = null;
					bool success = false;
					//convert it to text
					try
					{
						converter.loadFile(pdfFileName);
						converter.convertToTextFile(1, converter.numPages, txtFileName);
						success = true;
					}
					catch
					{
						success = false;
					}
					finally
					{
						converter.closeFile();
					}
					if(success)
					{
						txt = new StreamReader(txtFileName, encoding);
						string text = txt.ReadToEnd();
						txt.Close();
						txt = null;
						retVal = parser.ExtractContent(ref text, false);
					}
					else
					{
						retVal = String.Empty;
					}
				}
				catch(Exception ex)
				{
					if(pdf!=null)
					{
						try
						{
							pdf.Close();
						}
						catch
						{}
					}
					if(txt!=null)
					{
						try
						{
							txt.Close();
						}
						catch
						{}
					}
					if(globals.Settings.LogLevel <= CWLogLevel.LogInfo)
					{
						globals.FileLog.LogWarning("PdfParser failed to extract content: " + ex.ToString()); 
					}
				}
				finally
				{
					File.Delete(pdfFileName);
					File.Delete(txtFileName);
				}
			}
			catch(Exception ex)
			{
				if(globals.Settings.LogLevel <= CWLogLevel.LogWarning)
				{
					globals.FileLog.LogWarning("PdfParser failed to extract content: " + ex.Message); 
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
		/// Occurs when the extraction of links from a PDF document is complete
		/// </summary>
		public event ParserEventHandler ExtractLinksComplete;

		/// <summary>
		/// Occurs when the extraction of text from a PDF document is complete
		/// </summary>
		public event ParserEventHandler ExtractTextComplete;

		/// <summary>
		/// Occurs when the extraction of content from a PDF document is complete
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
