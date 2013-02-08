using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Web.Services2;

namespace CrawlWave.Common.WSCFilter
{
	/// <summary>
	/// WSCFUnzipFilter is a filter that when added to the pipeline decompresses the body
	/// element attachment and sets the result as the soap message body. The filter will
	/// only do so if it finds evidence that the <see cref="WSCFZipFilter"/> has been used.
	/// </summary>
	/// <remarks>
	/// <b>Prerequisites:</b>
	/// <para>
	/// WSE 2.0 release installed.<br/>
	/// ICSharpZipLib available and referenced.
	/// </para>
	/// </remarks>
	/// <example>
	/// Adding a Filter to the pipeline:<br/>
	/// Client side (i.e. WinForms app):<br/>
	/// <para>
	/// <code>Server.Pipeline.InputFilters.Add(new WSCFUnzipFilter());</code>
	/// </para>
	/// <br/>
	/// Server side (i.e Web Service / ASP.Net): add the following section to the web.config
	/// file, in the &lt;microsoft.web.services2&gt; section.
	/// <code>
	///   &lt;filters&gt;<br/>
	///		&lt;input&gt;<br/>
	///		  &lt;add type="CrawlWave.Common.WSCFilter.WSCFUnzipFilter,CrawlWave.Common.WSCFilter" /&gt;<br/>
	///		&lt;/input&gt;<br/>
	///   &lt;/filters&gt;
	/// </code>
	/// </example>
	public class WSCFUnzipFilter : SoapInputFilter
	{
		#region Public base class method overrides

		/// <summary>
		/// Processes the SOAP Messages and decompresses them if necessary
		/// </summary>
		/// <param name="envelope">The <see cref="SoapEnvelope"/> to process.</param>
		public override void ProcessMessage(SoapEnvelope envelope)
		{
			if(envelope.Header == null)
			{
				return;
			}
			//check if the ZipFilter has been applied.
			XmlNodeList elemList = envelope.Header.GetElementsByTagName(Constants.WSCFCompressionElement);
			if (elemList.Count == 0)
			{
				return;
			}
			//The header contains the element, let's check if the body is compressed
			if (elemList[0].Attributes[Constants.WSCFAttribute].Value.Equals("0"))
			{
				return;
			}
			//make sure we decompress using the same method we used for compression.
			WSCFCompression.CompressionProvider = (CompressionType)Convert.ToInt32((string)elemList[0].Attributes[Constants.WSCFTypeAttribute].Value);
			//remove the element from the envelope, it's no longer necessary.
			envelope.Header.RemoveChild(elemList[0]);
			//decompress the envelope attachments
			MemoryStream outStream = WSCFCompression.DeCompressToStream(envelope.Context.Attachments[0].Stream);
			//replace the body element.
			XmlElement body = envelope.CreateBody();
			body.InnerXml = System.Text.Encoding.UTF8.GetString(outStream.ToArray());
			GC.Collect();
		}

		#endregion
	}
}
