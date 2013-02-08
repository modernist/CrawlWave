using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CrawlWave.ServerInstaller.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class frmStep6 : CrawlWave.ServerInstaller.Forms.frmStep
	{
		private System.Windows.Forms.Label lblDone;
		private System.ComponentModel.IContainer components = null;

		private Globals globals;

		/// <summary>
		/// 
		/// </summary>
		public frmStep6()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			globals = Globals.Instance();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmStep6));
			this.lblDone = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblStep1
			// 
			this.lblStep1.ImageIndex = 2;
			this.lblStep1.Name = "lblStep1";
			// 
			// lblStep2
			// 
			this.lblStep2.ImageIndex = 5;
			this.lblStep2.Name = "lblStep2";
			// 
			// lblStep3
			// 
			this.lblStep3.ImageIndex = 8;
			this.lblStep3.Name = "lblStep3";
			// 
			// lblStep4
			// 
			this.lblStep4.ImageIndex = 11;
			this.lblStep4.Name = "lblStep4";
			// 
			// lblStep5
			// 
			this.lblStep5.ImageIndex = 14;
			this.lblStep5.Name = "lblStep5";
			// 
			// lblStep6
			// 
			this.lblStep6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(161)));
			this.lblStep6.ImageIndex = 16;
			this.lblStep6.Name = "lblStep6";
			// 
			// cmdBack
			// 
			this.cmdBack.Enabled = false;
			this.cmdBack.Name = "cmdBack";
			this.cmdBack.Click += new System.EventHandler(this.cmdBack_Click);
			// 
			// cmdNext
			// 
			this.cmdNext.Name = "cmdNext";
			this.cmdNext.Text = "Finish >>";
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			// 
			// cmdCancel
			// 
			this.cmdCancel.Enabled = false;
			this.cmdCancel.Name = "cmdCancel";
			// 
			// lblDone
			// 
			this.lblDone.BackColor = System.Drawing.SystemColors.Window;
			this.lblDone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblDone.Image = ((System.Drawing.Image)(resources.GetObject("lblDone.Image")));
			this.lblDone.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.lblDone.Location = new System.Drawing.Point(144, 8);
			this.lblDone.Name = "lblDone";
			this.lblDone.Size = new System.Drawing.Size(344, 224);
			this.lblDone.TabIndex = 4;
			this.lblDone.Text = "The CrawlWave Server Applications Installation and Configuration is complete. The" +
				" crawling process may now begin. Please do not forget to use the CrawlWave Serve" +
				"rManager for additional configuration tasks. Press the Finish to complete the in" +
				"stallation.";
			this.lblDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// frmStep6
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(498, 271);
			this.Controls.Add(this.lblDone);
			this.Name = "frmStep6";
			this.Text = "Done!";
			this.Load += new System.EventHandler(this.frmStep6_Load);
			this.Controls.SetChildIndex(this.lblDone, 0);
			this.Controls.SetChildIndex(this.cmdCancel, 0);
			this.Controls.SetChildIndex(this.cmdNext, 0);
			this.Controls.SetChildIndex(this.cmdBack, 0);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void cmdNext_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void cmdBack_Click(object sender, System.EventArgs e)
		{
			frmStep5 frm = new frmStep5();
			frm.Show();
			globals.LoadedForms[this.Name] = null;
			globals.LoadedForms.Remove(this.Name);
			this.Close();
		}

		private void frmStep6_Load(object sender, System.EventArgs e)
		{
			globals.LoadedForms.Add(this.Name, this);
		}

		#endregion
	}
}

