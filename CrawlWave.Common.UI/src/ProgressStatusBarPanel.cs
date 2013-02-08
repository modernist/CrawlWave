using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace CrawlWave.Common.UI
{
	/// <summary>
	/// ProgressStatusBarPanel is a subclass of <see cref="StatusBarPanel"/>. It is a 
	/// StatusBarPanel that contains a smooth progress bar with a specified color.
	/// </summary>
	[ToolboxBitmap(typeof(ProgressStatusBarPanel), "ProgressStatusBarPanel")]
	public class ProgressStatusBarPanel : StatusBarPanel
	{
		private int minimum;
		private int maximum;
		private SolidBrush brush;

		/// <summary>
		/// the current value of the progress bar
		/// </summary>
		private int value;
		/// <summary>
		/// the Progress Bar's Color
		/// </summary>
		private Color foreColor;

		/// <summary>
		/// Gets or sets the minimum value of the progress bar
		/// </summary>
		public int Minimum
		{
			get { return minimum; }
			set
			{
				if (value > maximum)
				{
					throw new ArgumentException("The minimum value can not be bigger than the maximum.");
				}
				minimum = value;
			}
		}
		/// <summary>
		/// Gets or sets the maximum value of the progress bar
		/// </summary>
		public int Maximum
		{
			get { return maximum; }
			set
			{
				if (value < minimum)
				{
					throw new ArgumentException("The maximum value can not be smaller than the minimum.");
				}
				maximum = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="ProgressBar.Value"/> property of the progress bar
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the supplied value is
		/// less than the value of the <see cref="Minimum"/> property or greater than the
		/// value of the <see cref="Maximum"/> property.</exception>
		public int Value
		{
			get { return value; }
			set
			{
				if ((value < minimum) || (value > maximum))
				{
					throw new ArgumentOutOfRangeException("Value", "Value is not between Minimum and Maximum allowed values.");
				}
				this.value = value;
				//cause the status bar to be redrawn
				if (this.Parent != null)
				{
					this.Parent.Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="Color"/> of the progress bar
		/// </summary>
		[Category("Appearance"), Description("Gets/sets the color of the progress bar.")]
		public Color ForeColor
		{
			get { return foreColor; }
			set
			{
				foreColor = value;
				brush.Color = foreColor;
				//Cause the Status Bar to be redrawn
				if (this.Parent != null)
				{
					this.Parent.Refresh();
				}
			}
		}

		/// <summary>
		/// Constructs a new instance of the <see cref="ProgressStatusBarPanel"/> class
		/// and initializes it with the default values.
		/// </summary>
		/// <param name="sb">The status bar that contains the progress bar</param>
		public ProgressStatusBarPanel(StatusBar sb)
		{
			//Allow custom drawing
			this.Style = StatusBarPanelStyle.OwnerDraw;
			//Set the color
			this.ForeColor = Color.RoyalBlue;//SystemColors.Highlight;
			this.minimum = 0;
			this.maximum = 100;
			this.value = 0;
			this.brush = new SolidBrush(foreColor);//System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(sbdevent.Bounds.X+1, sbdevent.Bounds.Y+1, barWidth, sbdevent.Bounds.Height-2),ForeColor,Color.Red,System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
			//Add a listener for the Draw event
			sb.DrawItem += new StatusBarDrawItemEventHandler(sb_DrawItem);
		}

		/// <summary>
		/// Draws the status bar
		/// </summary>
		/// <param name="sender">The control that caused it to draw again</param>
		/// <param name="sbdevent">The draw event</param>
		private void sb_DrawItem(object sender, StatusBarDrawItemEventArgs sbdevent)
		{
			//Find the appropriate panel
			if (sbdevent.Panel == this)
			{
				//Now use GDI to draw the progress bar
				int barWidth =
					(int)(value *
					(double)(sbdevent.Bounds.Width - 2) /
					(double)(maximum - minimum));
				sbdevent.Graphics.FillRectangle(brush,
					sbdevent.Bounds.X + 1, sbdevent.Bounds.Y + 1,
					barWidth, sbdevent.Bounds.Height - 2);
			}
		}

		/// <summary>
		/// Releases the managed resources used by the Control
		/// </summary>
		/// <param name="disposing">Disposing</param>
		protected override void Dispose(bool disposing)
		{
			brush.Dispose();
			base.Dispose(disposing);
		}
	}
}
