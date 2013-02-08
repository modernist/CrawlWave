using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace CrawlWave.Common.UI
{
	/// <summary>
	/// HealthBarGraph is a User Control that displays a Task-Manager like health bar. It
	/// can be used to indicate percentage of usage of a certain resource, like CPU.
	/// </summary>
	[ToolboxBitmap(typeof(HealthBarGraph),"HealthBarGraph")]
	public class HealthBarGraph : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Panel pnlBackground;
		private System.Windows.Forms.Label lblText;
		private System.Windows.Forms.PictureBox picBarEmpty;
		private System.Windows.Forms.PictureBox picBarFull;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Private variables

		private string text;
		private int curValue;
		private int minValue;
		private int maxValue;
		private int count;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the text displayed at the bottom of the control, next to the value.
		/// </summary>
		[Category("HealthBarGraph"), Description("The text displayed next to the current value")]
		public string Label
		{
			get { return text; }
			set { text = value;}
		}
		
		/// <summary>
		/// Gets or sets the Minimum value the control can display
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the value provided is negative.</exception>
		[Category("HealthBarGraph"), Description("The minimum value the Health Bar Graph can display")]
		public int Minimum
		{
			get{return minValue;}
			set
			{
				if(value<0)
				{
					throw new ArgumentOutOfRangeException("Minimum value cannot be negative.");
				}
				else
				{
					minValue=value;
					OnMiminumChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the Maximum value the control can display
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the value provided is
		/// smaller than the value of the <see cref="Minimum"/> property.</exception>
		[Category("HealthBarGraph"), Description("The maximum value the Health Bar Graph can display")]
		public int Maximum
		{
			get { return maxValue; }
			set 
			{
				if(value<minValue)
				{
					throw new ArgumentOutOfRangeException("The maximum value cannot be less than the minimum.");
				}
				else
				{
					maxValue = value;
					OnMaximumChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the current value displayed in the control
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the value provoded is
		/// less than 0 or greater then the value of the <see cref="Maximum"/> property.</exception>
		[Category("HealthBarGraph"), Description("The current value of the Health Bar Graph")]
		public int Value
		{
			get { return curValue; }
			set 
			{
				if((value<0)||(value>maxValue))
				{
					throw new ArgumentOutOfRangeException("The specified value is out of range");
				}
				else
				{
					if(value != curValue)
					{
						SetValue(value);
						OnValueChanged(EventArgs.Empty);
					}
				}
			}
		}

		#endregion

		#region Public Events
		
		/// <summary>
		/// Occurs when the <see cref="HealthBarGraph.Minimum"/> property is set to a new value
		/// </summary>
		public event EventHandler MinimumChanged;
		/// <summary>
		/// Occurs when the <see cref="HealthBarGraph.Maximum"/> property is set to a new value
		/// </summary>
		public event EventHandler MaximumChanged;
		/// <summary>
		/// Occurs when the <see cref="HealthBarGraph.Value"/> property is set to a new value
		/// </summary>
		public event EventHandler ValueChanged;

		#endregion

		/// <summary>
		/// Constructs a new instance of the <see cref="HealthBarGraph"/> class and sets the
		/// <see cref="Minimum"/>, <see cref="Maximum"/> and <see cref="Value"/> properties
		/// to their default values.
		/// </summary>
		public HealthBarGraph()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			//Initialize the control's properties
			text=String.Empty;
			minValue=0;
			maxValue=100;
			curValue=0;
			count = 0;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HealthBarGraph));
			this.pnlBackground = new System.Windows.Forms.Panel();
			this.lblText = new System.Windows.Forms.Label();
			this.picBarEmpty = new System.Windows.Forms.PictureBox();
			this.picBarFull = new System.Windows.Forms.PictureBox();
			this.pnlBackground.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlBackground
			// 
			this.pnlBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlBackground.BackColor = System.Drawing.Color.Black;
			this.pnlBackground.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlBackground.Controls.Add(this.lblText);
			this.pnlBackground.Controls.Add(this.picBarEmpty);
			this.pnlBackground.Controls.Add(this.picBarFull);
			this.pnlBackground.Location = new System.Drawing.Point(0, 0);
			this.pnlBackground.Name = "pnlBackground";
			this.pnlBackground.Size = new System.Drawing.Size(80, 80);
			this.pnlBackground.TabIndex = 0;
			// 
			// lblText
			// 
			this.lblText.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.lblText.Location = new System.Drawing.Point(0, 56);
			this.lblText.Name = "lblText";
			this.lblText.Size = new System.Drawing.Size(80, 16);
			this.lblText.TabIndex = 0;
			this.lblText.Text = "Text";
			this.lblText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// picBarEmpty
			// 
			this.picBarEmpty.Image = ((System.Drawing.Image)(resources.GetObject("picBarEmpty.Image")));
			this.picBarEmpty.Location = new System.Drawing.Point(14, 8);
			this.picBarEmpty.Name = "picBarEmpty";
			this.picBarEmpty.Size = new System.Drawing.Size(48, 48);
			this.picBarEmpty.TabIndex = 1;
			this.picBarEmpty.TabStop = false;
			// 
			// picBarFull
			// 
			this.picBarFull.Image = ((System.Drawing.Image)(resources.GetObject("picBarFull.Image")));
			this.picBarFull.Location = new System.Drawing.Point(14, 8);
			this.picBarFull.Name = "picBarFull";
			this.picBarFull.Size = new System.Drawing.Size(48, 48);
			this.picBarFull.TabIndex = 2;
			this.picBarFull.TabStop = false;
			// 
			// HealthBarGraph
			// 
			this.Controls.Add(this.pnlBackground);
			this.Name = "HealthBarGraph";
			this.Size = new System.Drawing.Size(80, 80);
			this.pnlBackground.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Private methods

		/// <summary>
		/// Resizes the picture box appropriately for the value to be displayed and updates
		/// the control's caption.
		/// </summary>
		/// <param name="val">The new value that must be displayed</param>
		private void SetValue(int val)
		{
			curValue=val;
			if(maxValue>0)
			{
				picBarEmpty.Height=36-((curValue-minValue)*27)/maxValue;
				lblText.Text=curValue.ToString() + text;
			}
			if(count++ == 100)
			{
				count=0;
				GC.Collect();
			}
		}

		#endregion

		#region Event Invokers

		/// <summary>
		/// Raises the <see cref="MinimumChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected void OnMiminumChanged(EventArgs e)
		{
			if(MinimumChanged != null)
			{
				MinimumChanged(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="MaximumChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected void OnMaximumChanged(EventArgs e)
		{
			if(MaximumChanged != null)
			{
				MaximumChanged(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected void OnValueChanged(EventArgs e)
		{
			if(ValueChanged != null)
			{
				ValueChanged(this, e);
			}
		}

		#endregion
	}
}
