using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CrawlWave.ServerInstaller.Forms
{
	/// <summary>
	/// Summary description for frmStep.
	/// </summary>
	public class frmStep : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlSteps;
		private System.Windows.Forms.ImageList imgIcons;
		protected System.Windows.Forms.Label lblStep1;
		protected System.Windows.Forms.Label lblStep2;
		protected System.Windows.Forms.Label lblStep3;
		protected System.Windows.Forms.Label lblStep4;
		protected System.Windows.Forms.Label lblStep5;
		protected System.Windows.Forms.Label lblStep6;
		private System.Windows.Forms.PictureBox picSetup;
		protected System.Windows.Forms.Button cmdBack;
		protected System.Windows.Forms.Button cmdNext;
		protected System.Windows.Forms.Button cmdCancel;
		private System.ComponentModel.IContainer components;

		public frmStep()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmStep));
			this.pnlSteps = new System.Windows.Forms.Panel();
			this.lblStep6 = new System.Windows.Forms.Label();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.lblStep5 = new System.Windows.Forms.Label();
			this.lblStep4 = new System.Windows.Forms.Label();
			this.lblStep3 = new System.Windows.Forms.Label();
			this.lblStep2 = new System.Windows.Forms.Label();
			this.lblStep1 = new System.Windows.Forms.Label();
			this.picSetup = new System.Windows.Forms.PictureBox();
			this.cmdBack = new System.Windows.Forms.Button();
			this.cmdNext = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.pnlSteps.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlSteps
			// 
			this.pnlSteps.BackColor = System.Drawing.Color.Black;
			this.pnlSteps.Controls.Add(this.lblStep6);
			this.pnlSteps.Controls.Add(this.lblStep5);
			this.pnlSteps.Controls.Add(this.lblStep4);
			this.pnlSteps.Controls.Add(this.lblStep3);
			this.pnlSteps.Controls.Add(this.lblStep2);
			this.pnlSteps.Controls.Add(this.lblStep1);
			this.pnlSteps.Controls.Add(this.picSetup);
			this.pnlSteps.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlSteps.Location = new System.Drawing.Point(0, 0);
			this.pnlSteps.Name = "pnlSteps";
			this.pnlSteps.Size = new System.Drawing.Size(136, 294);
			this.pnlSteps.TabIndex = 0;
			// 
			// lblStep6
			// 
			this.lblStep6.ForeColor = System.Drawing.Color.White;
			this.lblStep6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblStep6.ImageIndex = 15;
			this.lblStep6.ImageList = this.imgIcons;
			this.lblStep6.Location = new System.Drawing.Point(0, 128);
			this.lblStep6.Name = "lblStep6";
			this.lblStep6.Size = new System.Drawing.Size(136, 24);
			this.lblStep6.TabIndex = 5;
			this.lblStep6.Text = "        Done!";
			this.lblStep6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// imgIcons
			// 
			this.imgIcons.ImageSize = new System.Drawing.Size(24, 24);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// lblStep5
			// 
			this.lblStep5.ForeColor = System.Drawing.Color.White;
			this.lblStep5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblStep5.ImageIndex = 12;
			this.lblStep5.ImageList = this.imgIcons;
			this.lblStep5.Location = new System.Drawing.Point(0, 104);
			this.lblStep5.Name = "lblStep5";
			this.lblStep5.Size = new System.Drawing.Size(136, 24);
			this.lblStep5.TabIndex = 4;
			this.lblStep5.Text = "        Startup Options";
			this.lblStep5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblStep4
			// 
			this.lblStep4.ForeColor = System.Drawing.Color.White;
			this.lblStep4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblStep4.ImageIndex = 9;
			this.lblStep4.ImageList = this.imgIcons;
			this.lblStep4.Location = new System.Drawing.Point(0, 80);
			this.lblStep4.Name = "lblStep4";
			this.lblStep4.Size = new System.Drawing.Size(136, 24);
			this.lblStep4.TabIndex = 3;
			this.lblStep4.Text = "        Data Location && IIS";
			this.lblStep4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblStep3
			// 
			this.lblStep3.ForeColor = System.Drawing.Color.White;
			this.lblStep3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblStep3.ImageIndex = 6;
			this.lblStep3.ImageList = this.imgIcons;
			this.lblStep3.Location = new System.Drawing.Point(0, 56);
			this.lblStep3.Name = "lblStep3";
			this.lblStep3.Size = new System.Drawing.Size(136, 24);
			this.lblStep3.TabIndex = 2;
			this.lblStep3.Text = "        Building Database";
			this.lblStep3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblStep2
			// 
			this.lblStep2.ForeColor = System.Drawing.Color.White;
			this.lblStep2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblStep2.ImageIndex = 3;
			this.lblStep2.ImageList = this.imgIcons;
			this.lblStep2.Location = new System.Drawing.Point(0, 32);
			this.lblStep2.Name = "lblStep2";
			this.lblStep2.Size = new System.Drawing.Size(136, 24);
			this.lblStep2.TabIndex = 1;
			this.lblStep2.Text = "        Database Setup";
			this.lblStep2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblStep1
			// 
			this.lblStep1.ForeColor = System.Drawing.Color.White;
			this.lblStep1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblStep1.ImageIndex = 0;
			this.lblStep1.ImageList = this.imgIcons;
			this.lblStep1.Location = new System.Drawing.Point(0, 8);
			this.lblStep1.Name = "lblStep1";
			this.lblStep1.Size = new System.Drawing.Size(136, 24);
			this.lblStep1.TabIndex = 0;
			this.lblStep1.Text = "        Welcome";
			this.lblStep1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// picSetup
			// 
			this.picSetup.Image = ((System.Drawing.Image)(resources.GetObject("picSetup.Image")));
			this.picSetup.Location = new System.Drawing.Point(48, 232);
			this.picSetup.Name = "picSetup";
			this.picSetup.Size = new System.Drawing.Size(32, 32);
			this.picSetup.TabIndex = 1;
			this.picSetup.TabStop = false;
			// 
			// cmdBack
			// 
			this.cmdBack.Location = new System.Drawing.Point(240, 240);
			this.cmdBack.Name = "cmdBack";
			this.cmdBack.Size = new System.Drawing.Size(80, 24);
			this.cmdBack.TabIndex = 1;
			this.cmdBack.Text = "<< Back";
			// 
			// cmdNext
			// 
			this.cmdNext.Location = new System.Drawing.Point(320, 240);
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.Size = new System.Drawing.Size(80, 24);
			this.cmdNext.TabIndex = 2;
			this.cmdNext.Text = "Next >>";
			// 
			// cmdCancel
			// 
			this.cmdCancel.Location = new System.Drawing.Point(408, 240);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(80, 24);
			this.cmdCancel.TabIndex = 3;
			this.cmdCancel.Text = "Cancel";
			// 
			// frmStep
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(502, 294);
			this.ControlBox = false;
			this.Controls.Add(this.cmdBack);
			this.Controls.Add(this.cmdNext);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.pnlSteps);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximumSize = new System.Drawing.Size(504, 296);
			this.MinimumSize = new System.Drawing.Size(504, 296);
			this.Name = "frmStep";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "frmStep";
			this.pnlSteps.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
