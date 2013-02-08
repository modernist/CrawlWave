using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CrawlWave.Common.UI
{
	#region MultiLineGraph

	/// <summary>
	/// MultiLineGraph is a subclass of <see cref="System.Windows.Forms.Panel"/> that can
	/// display a multiple line graph.
	/// </summary>
	[ToolboxBitmap(typeof(MultiLineGraph),"MultiLineGraph")]
	public class MultiLineGraph : System.Windows.Forms.Panel
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Private variables
		/// <summary>
		/// Private variables needed for the control
		/// </summary>
		private ItemCollection itemList;
		private double minValue, maxValue, currentValue, currentMaxValue;
		private int currentPos; //in pixels, the y-axis value of the last value
		private Color gridColor;
		private bool showLegend;
		private string xText, yText; //x-axis and y-axis text
		private Pen linePen; //used to draw the grid
		private Pen axesPen; //used to draw the axes
		private SolidBrush triangeBrush; //used to draw the arrows at the end of the axes
		#endregion

		#region Public Properties 

		/// <summary>
		/// Gets the value currently displayed by the control
		/// </summary>
		[Category("MultiLineGraph"), Description("Gets the current value.")]
		public double Value
		{
			get { return currentValue; }
		}

		/// <summary>
		/// Gets or sets the minimum value supported by the control
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the value provided is negative.</exception>
		[Category("MultiLineGraph"), Description("Gets/sets the minimum value.")]
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
		[Category("MultiLineGraph"), Description("Gets/sets the maximum value.")]
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

		/// <summary>
		/// Gets or sets the <see cref="ItemCollection"/> containing the data for the lines
		/// to be drawn on the graph.
		/// </summary>
		[Category("MultiLineGraph"), Description("Gets or sets the collection of MultiLineGraphItems.")]
		public ItemCollection Items
		{
			get { return itemList; }
			set 
			{ 
				if(value!=null)
				{
					itemList = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a Boolean value indicating whether a color legend must be drawn on
		/// the graph for each <see cref="MultiLineGraphItem"/> contained in the <see cref="MultiLineGraph.Items"/>
		/// collection.
		/// </summary>
		[Category("MultiLineGraph"), Description("Gets or sets a boolean value indicating whether the legends must be displayed.")]
		public bool ShowLegend
		{
			get { return showLegend; }
			set
			{
				showLegend = value;
				OnShowLegendChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the x-Axis Legend Text
		/// </summary>
		[Category("MultiLineGraph"), Description("Gets or sets the x-Axis Legend Text.")]
		public string XAxisText
		{
			get { return xText; }
			set
			{
				xText = value;
				OnXAxisTextChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the y-Axis Legend Text
		/// </summary>
		[Category("MultiLineGraph"), Description("Gets or sets the y-Axis Legend Text.")]
		public string YAxisText
		{
			get { return yText; }
			set
			{
				yText = value;
				OnYAxisTextChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs when the <see cref="MultiLineGraph.Minimum"/> property is set to a new value.
		/// </summary>
		public event EventHandler MinimumChanged;
		/// <summary>
		/// Occurs when the <see cref="MultiLineGraph.Maximum"/> property is set to a new value.
		/// </summary>
		public event EventHandler MaximumChanged;
		/// <summary>
		/// Occurs when the <see cref="MultiLineGraph.GridColor"/> property is set to a new value.
		/// </summary>
		public event EventHandler GridColorChanged;
		/// <summary>
		/// Occurs when the <see cref="MultiLineGraph.ShowLegend"/> property is set to a new value.
		/// </summary>
		public event EventHandler ShowLegendChanged;
		/// <summary>
		/// Occurs when the <see cref="MultiLineGraph.XAxisText"/> property is set to a new value.
		/// </summary>
		public event EventHandler XAxisTextChanged;
		/// <summary>
		/// Occurs when the <see cref="MultiLineGraph.YAxisText"/> property is set to a new value.
		/// </summary>
		public event EventHandler YAxisTextChanged;

		#endregion

		#region Constructor and Dispose methods

		/// <summary>
		/// Constructs an instance of the <see cref="MultiLineGraph"/> class and sets all
		/// the properties to their default values.
		/// </summary>
		public MultiLineGraph()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|ControlStyles.DoubleBuffer|ControlStyles.ResizeRedraw,true);
			currentPos=-1;
			currentValue=0;
			currentMaxValue=0;
			minValue=0;
			maxValue=100;
			gridColor=Color.MediumBlue;
			BackColor=Color.Black;
			BorderStyle=BorderStyle.Fixed3D;
			itemList = new ItemCollection();
			showLegend = false;
			xText = String.Empty;
			yText = String.Empty;
			linePen = new Pen(gridColor, 1f);
			axesPen = new Pen(ForeColor, 1.5f);
			triangeBrush = new SolidBrush(ForeColor);
			ForeColor=Color.CornflowerBlue;
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
			itemList.Clear();
			linePen.Dispose();
			linePen = null;
			axesPen.Dispose();
			axesPen = null;
			triangeBrush.Dispose();
			triangeBrush = null;
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
			// 
			// MultiLineGraph
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ForeColorChanged += new System.EventHandler(this.MultiLineGraph_ForeColorChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MultiLineGraph_Paint);

		}
		#endregion

		#region Drawing Related Methods

		/// <summary>
		/// It is called whenever the control receives a Paint event, causing it to redraw.
		/// </summary>
		private void MultiLineGraph_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			ClearGraph(e.Graphics);
			DrawAxes(e.Graphics);
			DrawGraph(e.Graphics);
		}

		/// <summary>
		/// It is called when the ForeColor property cahnge in order to set the color of
		/// the axes Pen and the triangle brush to the new value.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MultiLineGraph_ForeColorChanged(object sender, System.EventArgs e)
		{
			axesPen.Color = ForeColor;
			triangeBrush.Color = ForeColor;
		}

		/// <summary>
		/// Clears the graph and draws the grid.
		/// </summary>
		/// <param name="g">The graphics object to be used for drawing the grid.</param>
		private void ClearGraph(Graphics g)
		{
			int pos=0;
			for(pos=this.Height - 18; pos>0; pos=pos-12)
			{
				g.DrawLine(linePen,0,pos,this.Width,pos);
			}
			for(pos=12; pos<this.Width; pos=pos+12)
			{
				g.DrawLine(linePen,pos,0,pos,this.Height);
			}
		}

		/// <summary>
		/// Draws the vertical and horizontal axes of the graph.
		/// </summary>
		/// <param name="g">The graphics object to be used for drawing the axes.</param>
		private void DrawAxes(Graphics g)
		{
			//draw the x-axis
			g.DrawLine(axesPen,12f,this.Height-17.75f,this.Width-12, this.Height-17.75f);
			//draw the y-axis
			g.DrawLine(axesPen,11.75f,6f,11.75f,this.Height-18f);
			//draw the axes legends
			if((xText!=String.Empty)||(yText!=String.Empty))
			{
				Font f = new Font(FontFamily.GenericSansSerif, 8f);
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				SizeF size;
				if(xText!=String.Empty)
				{
					size = g.MeasureString(xText, f, this.Width-24, sf);
					g.DrawString(xText, f, axesPen.Brush, (this.Width-24-size.Width)/2,this.Height-17);
				}
				if(yText!=String.Empty)
				{
					sf.FormatFlags = StringFormatFlags.DirectionVertical;
					size = g.MeasureString(yText, f, this.Height-24, sf);
					g.DrawString(yText, f, axesPen.Brush, -3, ((this.Height-18-size.Width)/2)+12, sf);
				}
				sf.Dispose();
				f.Dispose();
			}
			//draw the triangles
			g.FillPolygon(triangeBrush, new Point[]{new Point(this.Width-10,this.Height-18), new Point(this.Width-16,this.Height-14), new Point(this.Width-16, this.Height-22)});
			g.FillPolygon(triangeBrush, new Point[]{new Point(12,6), new Point(8,12), new Point(16,12)});
		}

		/// <summary>
		/// Iterates through all the Items of the graph and their values and calculates
		/// the maximum value
		/// </summary>
		private void CalculateMaximum()
		{
			currentMaxValue = 0;
			foreach(MultiLineGraphItem item in itemList)
			{
				if(item.Values!=null)
				{
					foreach(double val in item.Values)
					{
						if(val>currentMaxValue)
						{
							currentMaxValue = val;
						}
					}
				}
			}
			maxValue = currentMaxValue;
		}

		/// <summary>
		/// Draws the graph based on the history of the previous values.
		/// </summary>
		/// <param name="g">The graphics object to be used for drawing the lines.</param>
		private void DrawGraph(Graphics g)
		{
			Pen graphPen = new Pen(Color.Transparent, 1);
			int x, y, itemCount=0;
			CalculateMaximum();
			Font font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular);
			foreach(MultiLineGraphItem item in itemList)
			{
				if(item.Values!=null)
				{
					x=0; y=0;
					graphPen.Color = item.LineColor;
					if(item.Values.Count>1)
					{
						currentPos = (int)((this.Height-30) * (1-(double)(((double)item.Values[0]-minValue)/maxValue)))+12;
						for(int i=1; i<item.Values.Count; i++)
						{
							y=(int)((this.Height-30) * (1-(double)(((double)item.Values[i]-minValue)/maxValue)))+12;
							x=i*12;
							if(x>this.Width)
							{
								break;
							}
							else
							{
								g.DrawLine(graphPen,x,currentPos,x+12,y);
								currentPos=y;
							}
						}
						if(showLegend) //Display the legend
						{
							g.DrawString(item.Text,font, graphPen.Brush, this.Width-80, 12*(itemCount+1));
						}
					}
					itemCount++;
				}
			}
			font.Dispose();
			font = null;
			graphPen.Dispose();
			graphPen = null;
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

		/// <summary>
		/// Raises the <see cref="ShowLegendChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected void OnShowLegendChanged(EventArgs e)
		{
			if(ShowLegendChanged != null)
			{
				ShowLegendChanged(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="XAxisTextChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected void OnXAxisTextChanged(EventArgs e)
		{
			if(XAxisTextChanged!=null)
			{
				XAxisTextChanged(this,e);
			}
		}

		/// <summary>
		/// Raises the <see cref="YAxisTextChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected void OnYAxisTextChanged(EventArgs e)
		{
			if(YAxisTextChanged!=null)
			{
				YAxisTextChanged(this,e);
			}
		}

		#endregion
	}

	#endregion

	#region MultiLineGraphItem

	/// <summary>
	/// The information related to each line drawn in a MultiLineGraph.
	/// </summary>
	[Serializable]
	[DesignerAttribute(typeof(MultiLineGraphItemDesigner))]
	public class MultiLineGraphItem
	{
		#region Private variables
		
		private Color color;
		private ArrayList values;
		private string text;

		#endregion

		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public MultiLineGraphItem()
		{
			color = Color.Transparent;
			values = null;
			text = String.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		public MultiLineGraphItem(Color color)
		{
			this.color = color;
			values = null;
			text = String.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="values"></param>
		public MultiLineGraphItem(Color color, ArrayList values)
		{
			this.color = color;
			this.values = values;
			text = String.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="text"></param>
		public MultiLineGraphItem(Color color, string text)
		{
			this.color = color;
			values = null;
			this.text = text;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="values"></param>
		/// <param name="text"></param>
		public MultiLineGraphItem(Color color, ArrayList values, string text)
		{
			this.color = color;
			this.values = values;
			this.text = text;
		}

		#endregion

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public Color LineColor
		{
			get { return color; }
			set { color = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Text
		{
			get { return text; }
			set { text = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		public ArrayList Values
		{
			get { return values; }
			set { values = value;}
		}
		#endregion
	}

	#endregion

	#region MultiLineGraphItemDesigner

	/// <summary>
	/// A custom designer used by Items to remove unwanted 
	/// properties from the Property window in the designer
	/// </summary>
	internal class MultiLineGraphItemDesigner : ControlDesigner
	{
		/// <summary>
		/// Initializes a new instance of the TaskItemDesigner class
		/// </summary>
		public MultiLineGraphItemDesigner()
		{
			
		}

		/// <summary>
		/// Adjusts the set of properties the component exposes through 
		/// a TypeDescriptor
		/// </summary>
		/// <param name="properties">An IDictionary containing the properties 
		/// for the class of the component</param>
		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);

			properties.Remove("BackgroundImage");
			properties.Remove("Cursor");
			properties.Remove("ForeColor");
			properties.Remove("FlatStyle");
		}
	}

	#endregion

	#region ItemCollection

	/// <summary>
	/// ItemCollection is a custom <see cref="CollectionBase"/> derived class that holds a
	/// collection of <see cref="MultiLineGraphItem"/> objects. It is used by <see cref="MultiLineGraph"/>
	/// in order to hold a collection of information on the various lines that are displayed
	/// in the graph.
	/// </summary>
	public class ItemCollection : CollectionBase
	{
		#region Constructor

		/// <summary>
		/// Constructs a new instance of the <see cref="ItemCollection"/> class.
		/// </summary>
		public ItemCollection() : base()
		{}

		/// <summary>
		/// Constructs a new instance of the <see cref="ItemCollection"/> class and initializes
		/// it with the contents of another <see cref="ItemCollection"/>.
		/// </summary>
		/// <param name="items"></param>
		public ItemCollection(ItemCollection items) : base()
		{
			this.Add(items);
		}

		#endregion

		#region Properties

		/// <summary>
		/// The MultiLineGraphItem located at the specified index location within the collection
		/// </summary>
		/// <param name="index">The index of the item to retrieve from the collection</param>
		public virtual MultiLineGraphItem this[int index]
		{
			get
			{
				return this.List[index] as MultiLineGraphItem;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a <see cref="MultiLineGraphItem"/> to the collection.
		/// </summary>
		/// <param name="item">The <see cref="MultiLineGraphItem"/> to add to the collection.</param>
		public void Add(MultiLineGraphItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.List.Add(item);

		}

		/// <summary>
		/// Adds the <see cref="MultiLineGraphItem"/> objects contained into an <see cref="ItemCollection"/>
		/// into this collection.
		/// </summary>
		/// <param name="items">The <see cref="ItemCollection"/> containing the <see cref="MultiLineGraphItem"/>
		/// objects to be added in this collection.</param>
		public void Add(ItemCollection items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			for(int i=0; i<items.Count; i++)
			{
				this.Add(items[i]);
			}
		}

		/// <summary>
		/// Removes all controls from the collection
		/// </summary>
		public new void Clear()
		{
			while (this.Count > 0)
			{
				this.RemoveAt(0);
			}
		}

		/// <summary>
		/// Determines whether the specified item is a member of the collection
		/// </summary>
		/// <param name="item">The item to locate in the collection</param>
		/// <returns>true if the item is a member of the collection, otherwise false</returns>
		public bool Contains(MultiLineGraphItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return (this.IndexOf(item) != -1);
		}

		/// <summary>
		/// Retrieves the index of the specified item in the collection
		/// </summary>
		/// <param name="item">The item to locate in the collection.</param>
		/// <returns>A zero-based index value that represents the position 
		/// of the specified item in the ItemCollection.</returns>
		public int IndexOf(MultiLineGraphItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			for (int i=0; i<this.Count; i++)
			{
				if (this[i] == item)
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Removes the specified <see cref="MultiLineGraphItem"/> from the collection.
		/// </summary>
		/// <param name="item">The item to remove from the ItemCollection.</param>
		public void Remove(MultiLineGraphItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.List.Remove(item);
		}

		/// <summary>
		/// Removes an item from the collection at the specified indexed location
		/// </summary>
		/// <param name="index">The index value of the item to remove</param>
		public new void RemoveAt(int index)
		{
			this.Remove(this[index]);
		}

		#endregion
	}

	#endregion
}
