using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrawlWave.ServerManager.Forms
{
	/// <summary>
	/// frmSplash is displayed when the CrawlWave ServerManager starts up.
	/// </summary>
	public class frmSplash : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Timer tmrClose;
		private System.Windows.Forms.PictureBox picSplash;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Creates a new instance of the <see cref="frmSplash"/> form.
		/// </summary>
		public frmSplash()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSplash));
			this.tmrClose = new System.Windows.Forms.Timer(this.components);
			this.picSplash = new System.Windows.Forms.PictureBox();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tmrClose
			// 
			this.tmrClose.Enabled = true;
			this.tmrClose.Interval = 2500;
			this.tmrClose.Tick += new System.EventHandler(this.tmrClose_Tick);
			// 
			// picSplash
			// 
			this.picSplash.Image = ((System.Drawing.Image)(resources.GetObject("picSplash.Image")));
			this.picSplash.Location = new System.Drawing.Point(0, 0);
			this.picSplash.Name = "picSplash";
			this.picSplash.Size = new System.Drawing.Size(448, 200);
			this.picSplash.TabIndex = 0;
			this.picSplash.TabStop = false;
			// 
			// lblCopyright
			// 
			this.lblCopyright.BackColor = System.Drawing.SystemColors.Window;
			this.lblCopyright.Location = new System.Drawing.Point(232, 168);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(208, 24);
			this.lblCopyright.TabIndex = 2;
			this.lblCopyright.Text = "Kostas Stroggylos 2003-2004 MScIS";
			this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.label1.Location = new System.Drawing.Point(144, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(296, 23);
			this.label1.TabIndex = 6;
			this.label1.Text = "A.U.E.B. Distributed Web Crawling Project";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// frmSplash
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(448, 200);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblCopyright);
			this.Controls.Add(this.picSplash);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmSplash";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "frmSplash";
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void tmrClose_Tick(object sender, System.EventArgs e)
		{
			tmrClose.Enabled = false;
			this.Close();
		}

		#endregion
	}
}
