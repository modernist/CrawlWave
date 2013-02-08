using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using CrawlWave.Common;

namespace CrawlWave.Common.UI
{
	/// <summary>
	/// RichTextboxLogger implements an <see cref="ILogger"/> that logs messages in a Rich
	/// Textbox Control.
	/// </summary>
	[ToolboxBitmap(typeof(RichTextboxLogger),"RichTextboxLogger")]
	public class RichTextboxLogger : System.Windows.Forms.RichTextBox, ILogger
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Private variables

		private static string eventSourceName = "CrawlWave";
		private bool rememberLastEntry;
		private string lastMessage;
		private bool useColors;
		private Mutex mutex;

		#endregion

		#region Public properties

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> value indicating whether the text is colorized.
		/// </summary>
		[Category("Appearance"), Description("Gets or sets a boolean value indicating whether the text is colorized.")]
		public bool UseColors
		{
			get { return useColors; }
			set { useColors = value;}
		}

		/// <summary>
		/// Gets or sets the Event Source Name of the Logger.
		/// </summary>
		[Category("Behavior"), Description("Gets or sets the Event Source Name.")]
		public string EventSourceName
		{
			get { return eventSourceName; }
			set { eventSourceName = value;}
		}

		#endregion

		#region Constructor and Dispose methods

		/// <summary>
		/// Constructs a new instance of the <see cref="RichTextboxLogger"/> control.
		/// </summary>
		public RichTextboxLogger()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.ReadOnly = true;
			rememberLastEntry = false;
			lastMessage = String.Empty;
			useColors = true;
			mutex = new Mutex();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region Overriden methods

		/// <summary>
		/// Paints the control.
		/// </summary>
		/// <param name="pe">The <see cref="PaintEventArgs"/> related to the event.</param>
		protected override void OnPaint(PaintEventArgs pe)
		{
			// TODO: Add custom paint code here

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}

		#endregion

		#region ILogger Members

		/// <summary>
		/// Creates a Log Entry of type Error in the log in a thread-safe manner.
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogError(string msg)
		{
			try
			{
				mutex.WaitOne();
				if(eventSourceName == String.Empty)
				{
					lastMessage = "[" + DateTime.Now.ToString() + "][ERROR]   " + msg;
				}
				else
				{
					lastMessage = "[" + DateTime.Now.ToString() + "][ERROR]   " + eventSourceName + ": " + msg;
				}
				if(this.InvokeRequired)
				{
					this.BeginInvoke(new LogDelegate(this.Log), new object[]{lastMessage, CWLoggerEntryType.Error});
				}
				else
				{
					Log(lastMessage, CWLoggerEntryType.Error);
				}
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a Log Entry of type Warning in the log in a thread-safe manner.
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogWarning(string msg)
		{
			try
			{
				mutex.WaitOne();
				if(eventSourceName == String.Empty)
				{
					lastMessage = "[" + DateTime.Now.ToString() + "][WARNING] " + msg;
				}
				else
				{
					lastMessage = "[" + DateTime.Now.ToString() + "][WARNING] " + eventSourceName + ": " + msg;
				}
				if(this.InvokeRequired)
				{
					this.BeginInvoke(new LogDelegate(this.Log), new object[]{lastMessage, CWLoggerEntryType.Warning});
				}
				else
				{
					Log(lastMessage, CWLoggerEntryType.Warning);
				}
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a Log Entry of type Info in the log in a thread-safe manner.
		/// </summary>
		/// <param name="msg">
		/// The log Message
		/// </param>
		public void LogInfo(string msg)
		{
			try
			{
				mutex.WaitOne();
				if(eventSourceName == String.Empty)
				{
					lastMessage = "[" + DateTime.Now.ToString() + "][INFO]    " + msg;
				}
				else
				{
					lastMessage = "[" + DateTime.Now.ToString() + "][INFO]    " + eventSourceName + ": " + msg;
				}
				if(this.InvokeRequired)
				{
					this.BeginInvoke(new LogDelegate(this.Log), new object[]{lastMessage, CWLoggerEntryType.Info});
				}
				else
				{
					Log(lastMessage, CWLoggerEntryType.Info);
				}
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a Log Entry in the log according to the type of the entry's event.
		/// </summary>
		/// <param name="entry">The <see cref="EventLoggerEntry"/> to log.</param>
		public void LogEventEntry(EventLoggerEntry entry)
		{
			try
			{
				mutex.WaitOne();
				if(eventSourceName == String.Empty)
				{
					lastMessage = "[" + entry.EventDate.ToString() + "][" + entry.EventType.ToString().ToUpper() +"] " + entry.EventMessage;
				}
				else
				{
					lastMessage = "[" + entry.EventDate.ToString() + "][" + entry.EventType.ToString().ToUpper() +"] " + eventSourceName + ": " + entry.EventMessage;
				}
				if(this.InvokeRequired)
				{
					this.BeginInvoke(new LogDelegate(this.Log), new object[]{lastMessage, entry.EventType});
				}
				else
				{
					Log(lastMessage, entry.EventType);
				}
			}
			catch
			{}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Gets the message of the last Event Log entry.
		/// </summary>
		public string LastEntry
		{
			get
			{
				try
				{
					//Assure thread safety
					mutex.WaitOne();
					return lastMessage;
				}
				catch
				{
					return String.Empty;
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// Gets or sets a Boolean value indicating whether the logger must remember the last
		/// entry.
		/// </summary>
		public bool RememberLastEntry
		{
			get { return rememberLastEntry;	}
			set	{ rememberLastEntry = value;}
		}

		#endregion

		#region Private Methods

		private void Log(string msg, CWLoggerEntryType type)
		{
			if((this.Text.Length + msg.Length) >= this.MaxLength)
			{
				try
				{
					this.Select(0, this.TextLength /2);
					this.ReadOnly = false;
					this.Cut();
					this.ClearUndo();
					Clipboard.SetDataObject(new DataObject());
				}
				catch
				{}
				finally
				{
					this.ReadOnly = true;
					GC.Collect();
				}
			}
			if(useColors)
			{
				int pos = this.TextLength-1;
				this.SelectionStart = (pos!=-1)?pos:0;
				this.AppendText(msg + "\n");
				this.SelectionStart = pos+1;
				this.SelectionLength = msg.Length;
				switch(type)
				{
					case CWLoggerEntryType.Error:
						this.SelectionColor = Color.Red;
						break;

					case CWLoggerEntryType.Warning:
						this.SelectionColor = Color.Blue;
						break;

					default:
						this.SelectionColor = Color.Black;
						break;
				}
				this.SelectionLength = 0;
			}
			else
			{
                this.SelectionStart = this.TextLength-1;
				this.AppendText(msg);
				this.SelectionLength = 0;
			}
		}

		#endregion

		#region Private Delegates

		/// <summary>
		/// 
		/// </summary>
		private delegate void LogDelegate(string msg, CWLoggerEntryType type);

		#endregion
	}
}
