using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Web.Services2;

namespace CrawlWave.Common.WSCFilter
{
	/// <summary>
	/// WSCFZipFilter is a filter that when added to the pipeline compresses the body element
	/// and adds the result as an attachment to soap message. The body element is deleted,
	/// so only the compressed element is transported over the network. On the receiving
	/// side the <see cref="WSCFUnzipFilter"/> should be added to the pipeline
	/// </summary>
	/// <remarks>
	/// <b>Prerequisites:</b>
	/// <para>
	/// WSE 2.0 release installed.
	/// ICSharpZipLib available and referenced.
	/// </para>
	/// </remarks>
	/// <example>
	/// Adding a Filter to the pipeline:<br/>
	/// Client side (i.e. WinForms app):<br/>
	/// <para>
	///	<code>Server.Pipeline.InputFilters.Add(new WSCFUnzipFilter());</code>
	///	</para>
	/// <br/>
	/// Server side (i.e WebService / ASP.Net): add the following section to the web.config
	/// file, in the &lt; microsoft.web.services2&gt; section.<br/>
	/// <code>
	///	  &lt;filters&gt;<br/>
	///		&lt;output&gt;<br/>
	///		  &lt;add type="CrawlWave.Common.WSCFilter.WSCFZipFilter,CrawlWave.Common.WSCFilter" /&gt;<br/>
	///		&lt;/output&gt;<br/>
	///   &lt;/filters&gt;
	/// </code>
	/// </example>
	public class WSCFZipFilter : SoapOutputFilter
	{
		#region Private variables
		
		/// <summary>
		/// The minimum size of the SOAP body element for compression to be enabled
		/// </summary>
		private static int minFilterSize = 100*Constants.WSCF1KB;
		/// <summary>
		/// A flag indicating if the filter is enabled
		/// </summary>
		private static bool enabled	= true;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the minimum message size in KB for compression to be enabled.
		/// </summary>
		public static int MinFilterSizeKB
		{
			get { return minFilterSize; }
			set { minFilterSize = value * Constants.WSCF1KB; }
		}

		/// <summary>
		/// Gets or sets a flag indicating whether the filter is enabled.
		/// </summary>
		public static bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		#endregion

		#region Base Class Methods Override

		/// <summary>
		/// Processes the SOAP Messages and compresses them if necessary
		/// </summary>
		/// <param name="envelope">The <see cref="SoapEnvelope"/> to process.</param>
		public override void ProcessMessage(SoapEnvelope envelope)
		{
			if (!enabled)
			{
				return;
			}
			//add an attribute to specify that the filter has been applied on this envelope.
			XmlElement soapHeader = envelope.CreateHeader();

			if (envelope.Body.InnerText.Length < minFilterSize)
			{
				return;
			}
			else
			{
				soapHeader.AppendChild(CreateCustomHeader(soapHeader, "1" ));
			}
			//compress the body element.
			MemoryStream result = new MemoryStream(WSCFCompression.Compress(Encoding.UTF8.GetBytes(envelope.Body.InnerXml)));

			//Attach zipped result to the envelope.
			Microsoft.Web.Services2.Attachments.Attachment attch = new Microsoft.Web.Services2.Attachments.Attachment("APPLICATION/OCTET-STREAM", result);
			envelope.Context.Attachments.Add(attch);

			//remove old body.
			XmlElement newBody = envelope.CreateBody();
			newBody.RemoveAll();
			envelope.SetBodyObject(newBody);
			GC.Collect();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates a custom Xml header to be attached in a <see cref="SoapEnvelope"/>.
		/// </summary>
		/// <param name="header">The original <see cref="XmlElement"/> SOAP header.</param>
		/// <param name="AttributeValue">The value of the header attribute.</param>
		/// <returns>A new <see cref="XmlElement"/> object containing a SOAP header.</returns>
		private XmlElement CreateCustomHeader(XmlElement header, string AttributeValue)
		{
			XmlDocument document = header.OwnerDocument;
			XmlElement customHeader = document.CreateElement(Constants.WSCFCompressionElement);
			customHeader.SetAttribute(Constants.WSCFAttribute, AttributeValue);
			customHeader.SetAttribute(Constants.WSCFTypeAttribute,((int)WSCFCompression.CompressionProvider).ToString());
			return customHeader;
		}

		#endregion
	}
}
