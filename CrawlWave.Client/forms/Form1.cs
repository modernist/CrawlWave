using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CrawlWave.Common;
using CrawlWave.Common.UI;
using CrawlWave.Client;

namespace CrawlWave.Client.Forms
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button button3;
		private HealthBarGraph healthBarGraph1;
		private HealthLineGraph healthLineGraph1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button button4;
		private System.ComponentModel.IContainer components;

		/*private SwfParser swf;
		private HtmlParser html;
		private TextParser text;
		private PdfParser pdf;
		Crawler crawler;
		private Globals globals;*/

		/// <summary>
		/// Main Form
		/// </summary>
		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			//CWComputerInfo info=ComputerInfo.GetComputerInfo();
			//label1.Text=info.CPUType + " " + info.RAMSize + "MB " + info.HDDSpace + "MB " + info.ConnectionSpeed.ToString() + " " + ComputerInfo.GetSHA1HashCode(info).ToString();

			/*string FileName="c:\\test.swf";
			string Url="http://localhost/";
			FileStream InputFile=new FileStream(FileName,FileMode.Open);
			byte [] buffer=new byte[InputFile.Length];
			InputFile.Read(buffer,0,buffer.Length);
			InputFile.Close();
			SwfParser parser = SwfParser.Instance();
			parser.ExtractLinks(buffer,ref Url);
			swf = SwfParser.Instance();
			html = HtmlParser.Instance();
			text = TextParser.Instance();
			pdf = PdfParser.Instance();
			crawler = new Crawler();
			crawler.StateChanged +=new EventHandler(StateChanged);
			crawler.StatisticsChanged += new EventHandler(crawler_StatisticsChanged);
			globals = Globals.Instance();*/
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.button3 = new System.Windows.Forms.Button();
			this.healthBarGraph1 = new HealthBarGraph();
			this.healthLineGraph1 = new HealthLineGraph();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.button4 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(296, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(216, 184);
			this.label1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(8, 96);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(280, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 288);
			this.button1.Name = "button1";
			this.button1.TabIndex = 2;
			this.button1.Text = "button1";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(8, 120);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox2.Size = new System.Drawing.Size(280, 72);
			this.textBox2.TabIndex = 3;
			this.textBox2.Text = "";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(88, 288);
			this.button2.Name = "button2";
			this.button2.TabIndex = 4;
			this.button2.Text = "button2";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// listBox1
			// 
			this.listBox1.HorizontalScrollbar = true;
			this.listBox1.Location = new System.Drawing.Point(8, 200);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(280, 82);
			this.listBox1.TabIndex = 5;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(168, 288);
			this.button3.Name = "button3";
			this.button3.TabIndex = 6;
			this.button3.Text = "button3";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// healthBarGraph1
			// 
			this.healthBarGraph1.Label = " KB";
			this.healthBarGraph1.Location = new System.Drawing.Point(8, 8);
			this.healthBarGraph1.Maximum = 100000;
			this.healthBarGraph1.Minimum = 0;
			this.healthBarGraph1.Name = "healthBarGraph1";
			this.healthBarGraph1.Size = new System.Drawing.Size(80, 80);
			this.healthBarGraph1.TabIndex = 7;
			this.healthBarGraph1.Value = 0;
			// 
			// healthLineGraph1
			// 
			this.healthLineGraph1.BackColor = System.Drawing.Color.Black;
			this.healthLineGraph1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.healthLineGraph1.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.healthLineGraph1.GridColor = System.Drawing.Color.MediumBlue;
			this.healthLineGraph1.Location = new System.Drawing.Point(96, 8);
			this.healthLineGraph1.Maximum = 100;
			this.healthLineGraph1.Minimum = 0;
			this.healthLineGraph1.Name = "healthLineGraph1";
			this.healthLineGraph1.Size = new System.Drawing.Size(416, 80);
			this.healthLineGraph1.TabIndex = 8;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(248, 288);
			this.button4.Name = "button4";
			this.button4.TabIndex = 9;
			this.button4.Text = "button4";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 317);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.healthLineGraph1);
			this.Controls.Add(this.healthBarGraph1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		//[STAThread]
		//static void Main() 
		//{
		//	Application.Run(new Form1());
		//}


		private void test()
		{	
			/*CrawlWavePdf2Text.CPdf2TextConverterClass conv = new CrawlWavePdf2Text.CPdf2TextConverterClass();
			for(int i=0; i<10; i++)
			{
				long retVal = conv.ConvertPdf2Text("c:\\test.pdf","c:\\test"+i.ToString()+".txt");
				Console.WriteLine(retVal.ToString());
				System.Threading.Thread.Sleep(500);
			}*/
		}

		private void StateChanged(object sender, EventArgs e)
		{
			//listBox1.Items.Add(((Crawler)sender).State.ToString());
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			/*//HtmlParser parser = HtmlParser.Instance();
			//string url=parser.TrimUrl(textBox1.Text);
			//byte b = parser.CleanUrlParams(ref url);
			//label1.Text = url + " " + b.ToString();
			//Uri uri = new Uri(url); string burl = "";
			//label1.Text = uri.AbsoluteUri + " " + uri.PathAndQuery + parser.NormalizeUrl(ref url, ref burl);
			if(crawler.State == CrawlerState.Stopped)
			{
				crawler.Start();
			}
			else
			{
				//crawler.Pause();
				//crawler.Resume();
				crawler.Stop();
			}
			InternetUrlToCrawl url = new InternetUrlToCrawl("http://127.0.0.1/test/test.pdf");
			crawler.CrawlUrl(ref url);*/
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
		/*	HtmlParser parser = HtmlParser.Instance();
			InternetUrlToCrawl url = new InternetUrlToCrawl(textBox1.Text);
			string content=textBox2.Text;
			ArrayList links=parser.ExtractLinks(ref content, ref url);
			listBox1.Items.Clear();
			foreach(InternetUrlToIndex i in links)
			{
				listBox1.Items.Add(i.Url);
			}*/
			//Regex sessionIDRegex = new Regex(@"[{|\(]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[\)|}]?$", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled);
			//MessageBox.Show(sessionIDRegex.IsMatch(textBox1.Text).ToString());
			//^[{|\(]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[\)|}]?$


			//string link = "href = \"http://www.in.gr/a.php?id=x1&myvar=1234567890ABCDEF1234567890ABCDEF&sid=mpamies&othervar\"";
			//link = parser.TrimUrl(link);
			//byte b = parser.CleanUrlParams(ref link);
			//MessageBox.Show(b.ToString() + link);
            //test();			
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			/*ArrayList links = new ArrayList();
			string FileName="c:\\test.swf";
			InternetUrlToCrawl Url= new InternetUrlToCrawl("http://localhost/");
			FileStream InputFile=new FileStream(FileName,FileMode.Open);
			byte [] buffer=new byte[InputFile.Length];
			InputFile.Read(buffer,0,buffer.Length);
			InputFile.Close();
			
			links = swf.ExtractLinks(buffer,ref Url);
			textBox2.Text = swf.ExtractText(buffer) + "\n-----------------------\n" + swf.ExtractContent(buffer, true);
			foreach(InternetUrlToIndex i in links)
			{
				listBox1.Items.Add(i.Url);
			}
			listBox1.Items.Add("----------------------");

			FileName = "c:\\test.pdf";
			InputFile=new FileStream(FileName,FileMode.Open);
			buffer=new byte[InputFile.Length];
			InputFile.Read(buffer,0,buffer.Length);
			InputFile.Close();

			Url.Url = "http://www.amd.com/";
			links = pdf.ExtractLinks(buffer, ref Url);
			textBox2.Text = textBox2.Text + "\n------------------------\n" + pdf.ExtractText(buffer) + "\n------------------------\n" + pdf.ExtractContent(buffer, true);
			foreach(InternetUrlToIndex i in links)
			{
				listBox1.Items.Add(i.Url);
			}*/
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			/*int KB = Process.GetCurrentProcess().PrivateMemorySize/1024;
			//(int)(GC.GetTotalMemory(false)/1024);
			healthBarGraph1.Value = KB;
			healthLineGraph1.NextValue = KB;*/
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			/*WebServiceProxy proxy = WebServiceProxy.Instance();
			SerializedException sx = null;
			int ID = 0;
			byte [] password = MD5Hash.md5("mpamies");
			sx = proxy.Server.RegisterUser(ref ID, "circular", password, "kostas@circular.gr");
			globals.Settings.UserID = ID;
			globals.Settings.UserName = "circular";
			globals.Settings.Email = "kostas@circular.gr";
			CWComputerInfo info = ComputerInfo.GetComputerInfo();
			globals.Settings.HardwareInfo = ComputerInfo.GetSHA1HashCode(info);
			globals.Settings.SaveSettings();
			ClientInfo ci = new ClientInfo();
			ci.UserID = globals.Settings.UserID;
			sx = proxy.SecureServer.RegisterClient(ref ci, info);
			//globals.Client_Info.ClientID = ci.ClientID;
			globals.Settings.ClientID = ci.ClientID;
			globals.Settings.SaveSettings();*/
		}

		private void crawler_StatisticsChanged(object sender, EventArgs e)
		{
			/*StringBuilder sb = new StringBuilder();
			for(int i = 0; i< crawler.Statistics.Length; i++)
			{
				sb.Append(crawler.Statistics[i].ToString() + " ");
			}
			label1.Text = sb.ToString();*/
		}
	}
}
