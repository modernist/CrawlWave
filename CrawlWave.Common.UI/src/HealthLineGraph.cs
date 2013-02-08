using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace CrawlWave.Common.UI
{
	/// <summary>
	/// HealthLineGraph is a subclass of <see cref="System.Windows.Forms.Panel"/> that can
	/// display a usage history graph like the one in Windows Task Manager. It offers user-
	/// selectable forecolor, grid color and background color and it can hold up to 800
	/// values. Since each time it is redrawn it is shifted by 2 pixels this means that it
	/// can hold enough values to fill a 1600x1200 pixel resolution screen.
	/// </summary>
	[ToolboxBitmap(typeof(HealthLineGraph),"HealthLineGraph")]
	public class HealthLineGraph : System.Windows.Forms.Panel
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Private variables
		/// <summary>
		/// Private variables needed for the control
		/// </summary>		
		private bool mustWrapValues;
		private List<double> Values;
		private double minValue, maxValue, currentValue, currentMaxValue;
		private int currentPos; //in pixels, the y-axis value of the last value
		private int drawCounter;
		private Color gridColor;
		private Pen linePen; //used to draw the grid
		private Pen graphPen;//used to draw the graph

		#endregion

		#region Public Properties 

		/// <summary>
		/// Gets the value currently displayed by the control
		/// </summary>
		[Category("HealthLineGraph"), Description("Gets the current value.")]
		public double Value
		{
			get { return currentValue; }
		}

		/// <summary>
		/// Gets or sets the minimum value supported by the control
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the value provided is negative.</exception>
		[Category("HealthLineGraph"), Description("Gets/sets the minimum value.")]
		public double Minimum
		{
			get { return minValue; }
			set
			{
				if(value<0)
				{
					throw new ArgumentException("The minimum value cannot be negative.");
				}
				else
				{
					minValue=value;
					OnMiminumChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the control's maximum supported value
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the value provided is 
		/// negative or smaller than the value of the <see cref="Minimum"/> property.</exception>
		[Category("HealthLineGraph"), Description("Gets/sets the maximum value.")]
		public double Maximum
		{
			get { return maxValue;}
			set
			{
				if(value<0)
				{
					throw new ArgumentOutOfRangeException("The maximum value cannot be negative.");
				}
				if(value<minValue)
				{
					throw new ArgumentOutOfRangeException("The maximum value cannot be less than the minimum value.");
				}
				maxValue=value;
				OnMaximumChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Sets the next value to be displayed and causes the control to redraw
		/// </summary>
		/// <remarks>
		/// If the value supplied is greater than the value of the <see cref="Maximum"/>
		/// property then the value of the Maximum property is set to the current value.
		/// This causes the graph to redraw and dynamically adapt to the new range.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the value provided is
		/// smaller than the value of the <see cref="Minimum"/> property.</exception> 
		[Category("HealthLineGraph"), Description("Sets the next value and causes the control to redraw.")]
		public double NextValue
		{
			set 
			{
				if(value<minValue)
				{
					throw new ArgumentOutOfRangeException("The new value cannot be less than the minimum value.");
				}
				if(value>maxValue)
				{
					maxValue=value;
					OnMaximumChanged(EventArgs.Empty);
					currentMaxValue=value;
				}
				SetNextValue(value);
				Invalidate();
			}
		}

		/// <summary>
		/// Gets/sets the Color to be used for the displayed grid
		/// </summary>
		[Category("Appearance"), Description("Sets the next value and causes the control to redraw.")]
		public Color GridColor
		{
			get { return gridColor; }
			set
			{
				gridColor = value;
				linePen.Color = gridColor;
				OnGridColorChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs when the <see cref="HealthLineGraph.Minimum"/> property is set to a new value
		/// </summary>
		public event EventHandler MinimumChanged;
		/// <summary>
		/// Occurs when the <see cref="HealthLineGraph.Maximum"/> property is set to a new value
		/// </summary>
		public event EventHandler MaximumChanged;
		/// <summary>
		/// Occurs when the <see cref="HealthLineGraph.GridColor"/> property is set to a new value
		/// </summary>
		public event EventHandler GridColorChanged;

		#endregion

		/// <summary>
		/// Constructs an instance of the <see cref="HealthLineGraph"/> class and sets all
		/// the properties to their default values.
		/// </summary>
		public HealthLineGraph()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|ControlStyles.DoubleBuffer|ControlStyles.ResizeRedraw,true);
			mustWrapValues=false;
			currentPos=-1;
			currentValue=0;
			currentMaxValue=0;
			drawCounter=0;
			minValue=0;
			maxValue=100;
            Values = new List<double>(800); //up to 1600 pixels width
			gridColor=Color.MediumBlue;
			BackColor=Color.Black;
			linePen = new Pen(gridColor, 1f);
			graphPen = new Pen(ForeColor, 1f);
			ForeColor=Color.CornflowerBlue;
			BorderStyle=BorderStyle.Fixed3D;
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
			Values.Clear();
			Values = null;
			linePen.Dispose();
			linePen = null;
			graphPen.Dispose();
			graphPen = null;
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// HealthLineGraph
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ForeColorChanged += new System.EventHandler(this.HealthLineGraph_ForeColorChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.HealthLineGraph_Paint);

		}
		#endregion

		#region Drawing Related Methods

		/// <summary>
		/// It is called whenever the control receives a Paint event, causing it to redraw.
		/// </summary>
		private void HealthLineGraph_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			ClearGraph(e.Graphics);
			DrawGraph(e.Graphics);
		}

		/// <summary>
		/// It is called when the ForeColor of the control changes in order to set the color of
		/// the graph Pen.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HealthLineGraph_ForeColorChanged(object sender, System.EventArgs e)
		{
			graphPen.Color = ForeColor;
		}

		/// <summary>
		/// Clears the graph and draws the grid.
		/// </summary>
		/// <param name="g">The graphics object to be used for drawing the grid.</param>
		private void ClearGraph(Graphics g)
		{
			int pos=0;
			for(pos=11; pos<this.Height; pos=pos+12)
			{
				g.DrawLine(linePen,0,pos,this.Width,pos);
			}
			for(pos=this.Width-drawCounter*2-1; pos>=0; pos=pos-12)
			{
				g.DrawLine(linePen,pos,0,pos,this.Height);
			}
		}

		/// <summary>
		/// Draws the graph based on the history of the previous values.
		/// </summary>
		/// <param name="g">The graphics object to be used for drawing the line.</param>
		private void DrawGraph(Graphics g)
		{
			if(Values.Count>0)
			{
				int y=0, x=0;
				for(int i=0; i<Values.Count; i++)
				{
					y=(int)((this.Height-3) * (1-((double)Values[Values.Count-i-1]-minValue)/maxValue));// - 1;
					x=this.Width-i*2;
					if(x<0)
					{
						break;
					}
					else
					{
						g.DrawLine(graphPen,x-2,currentPos,x-4,y);
						currentPos=y;
					}
				}
			}
		}

		/// <summary>
		/// Sets the new value to be displayed in the control, makes sure that the grid
		/// is moved one step forward and calculates the current maximum value, so that
		/// the height of the line can be adjusted dynamically.
		/// </summary>
		/// <param name="nextValue">The new value to be displayed in the control.</param>
		private void SetNextValue(double nextValue)
		{
			if(mustWrapValues)
			{
				double oldValue=Values[0];
				Values.RemoveAt(0);
				if(oldValue==currentMaxValue)
				{
					currentMaxValue=0;
					foreach(double d in Values)
					{
						if(d>currentMaxValue)
						{
							currentMaxValue=d;
						}
					}
				}
			}
			Values.Add(nextValue);
			if(Values.Count==800)
			{
				mustWrapValues=true;
			}
			drawCounter++;
			if(drawCounter==6)
			{
				drawCounter=0;
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
		/// Raises the <see cref="GridColorChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected void OnGridColorChanged(EventArgs e)
		{
			if(GridColorChanged != null)
			{
				GridColorChanged(this, e);
			}
		}

		#endregion
	}
}
