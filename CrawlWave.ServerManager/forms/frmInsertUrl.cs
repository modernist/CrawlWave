using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CrawlWave.Common;
using CrawlWave.ServerCommon;

namespace CrawlWave.ServerManager.Forms
{
	/// <summary>
	/// frmInsertUrl allows the insertion of new Urls to be crawled in the system's database.
	/// </summary>
	public class frmInsertUrl : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox grpUrl;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdClear;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.Label lblNotice;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		//
		private Globals globals;
		//the list of extensions that are not allowed in a url (because they are of no interest to us)
		private static string []SIDVarNames={"PHPSESSID","sid","jsessionid","sessionid","session","sess_id","MSCSID"};
		//the list of extensions that are not allowed in a url (because they are of no interest to us)
		private static string []badExtensions={".css",".zip",".exe",".ps",".doc",".ppt",".pps",".xls",".vsd",".rar",".ace",".jpg",".jpeg",".gif",".png",".mpg",".mpe",".mpeg",".mpa",".avi",".bmp",".mp3",".mp4",".aac",".rm",".rmf",".ram",".mov",".qt",".asf",".asx",".wmv",".wma",".sit",".ico",".iso",".tar",".gzip",".gz",".tgz",".bz2",".cab",".msi",".msm"};
		private System.Windows.Forms.TextBox txtUrl;
		//Regular expression for matching Session IDs (32 byte long hex strings or Guids)
		private Regex sessionIDRegex;
		//used for validating the urls provided by the user.
		private UrlValidator validator;

		/// <summary>
		/// Creates a new instance of the <see cref="frmInsertUrl"/> form.
		/// </summary>
		public frmInsertUrl()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			globals = Globals.Instance();
			sessionIDRegex = new Regex(@"([0-9a-fA-F]{40,64})|([\{|\(]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[\)|\}]?)$", RegexOptions.CultureInvariant|RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.Compiled); //@"^([0-9a-f]{32})|(\{?[0-9a-f]{8}-([0-9a-f]{4}-){3}-[0-9a-f]{12}\}?)$"
			validator = UrlValidator.Instance();
			globals.LoadedForms[this.Name] = this;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmInsertUrl));
			this.txtUrl = new System.Windows.Forms.TextBox();
			this.grpUrl = new System.Windows.Forms.GroupBox();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdClear = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.lblNotice = new System.Windows.Forms.Label();
			this.grpUrl.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtUrl
			// 
			this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtUrl.Location = new System.Drawing.Point(8, 16);
			this.txtUrl.MaxLength = 500;
			this.txtUrl.Name = "txtUrl";
			this.txtUrl.Size = new System.Drawing.Size(312, 20);
			this.txtUrl.TabIndex = 0;
			this.txtUrl.Text = "";
			// 
			// grpUrl
			// 
			this.grpUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpUrl.Controls.Add(this.txtUrl);
			this.grpUrl.Location = new System.Drawing.Point(8, 8);
			this.grpUrl.Name = "grpUrl";
			this.grpUrl.Size = new System.Drawing.Size(328, 48);
			this.grpUrl.TabIndex = 0;
			this.grpUrl.TabStop = false;
			this.grpUrl.Text = "New Url Address";
			// 
			// cmdOK
			// 
			this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdOK.Location = new System.Drawing.Point(8, 112);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(80, 24);
			this.cmdOK.TabIndex = 2;
			this.cmdOK.Text = "Insert";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdClear
			// 
			this.cmdClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClear.Location = new System.Drawing.Point(96, 112);
			this.cmdClear.Name = "cmdClear";
			this.cmdClear.Size = new System.Drawing.Size(80, 24);
			this.cmdClear.TabIndex = 3;
			this.cmdClear.Text = "Clear";
			this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdClose.Location = new System.Drawing.Point(184, 112);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(80, 24);
			this.cmdClose.TabIndex = 4;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// lblNotice
			// 
			this.lblNotice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblNotice.Location = new System.Drawing.Point(8, 64);
			this.lblNotice.Name = "lblNotice";
			this.lblNotice.Size = new System.Drawing.Size(328, 40);
			this.lblNotice.TabIndex = 1;
			this.lblNotice.Text = "Note: This adds only one Url at a time to the system\'s database. In order to add " +
				"a large number of Urls it is preferable to use the CrawlWave Initializer Plugin." +
				"";
			// 
			// frmInsertUrl
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdClose;
			this.ClientSize = new System.Drawing.Size(344, 141);
			this.Controls.Add(this.lblNotice);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdClear);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.grpUrl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "frmInsertUrl";
			this.Text = "Add a new Url";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmInsertUrl_Closing);
			this.grpUrl.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			if(!ValidateForm())
			{
				return;
			}
			if(!InsertUrl())
			{
				return;
			}
			txtUrl.Text = String.Empty;
		}

		private void cmdClear_Click(object sender, System.EventArgs e)
		{
			txtUrl.Text = String.Empty;
		}

		private void cmdClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void frmInsertUrl_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			globals.LoadedForms[this.Name] = null;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Performs a processing of the GET parameters of dynamic urls. It removes any
		/// session IDs and limits the number of parameters to 3, so as to avoid urls that
		/// act as "black holes". It also removes named anchors from the end of the urls
		/// for the same reason and performs a calculation of the url's 'importance', taking
		/// into account the length of the absolute path and the number of its parameters.
		/// </summary>
		/// <param name="url">The url to be processed and whose priority is wanted.</param>
		/// <returns>An unsigned 8 bit integer indicating the Url's priority.</returns>
		private byte ProcessUrl(ref string url)
		{
			byte priority = 1;
			Uri uri = new Uri(url);
			StringBuilder sb= new StringBuilder(uri.AbsoluteUri);
			int pos=url.IndexOf('#'), i=0;
			if (pos!=-1)
			{
				//the url contains an anchor, so it must be cleaned
				sb.Remove(pos,url.Length-pos);
				priority++;
			}
			//calculate the absolute path's depth  and increase the priority accordingly
			if(uri.AbsolutePath.Length>1)
			{
				for(i=1; i<uri.AbsolutePath.Length; i++)
				{
					if(uri.AbsolutePath[i]=='/')
					{
						priority++;
					}
				}
			}
			//now check for url parameters
			pos=url.IndexOf('?');
			if (pos!=-1)
			{
				//it's a dynamic url, so it must be processed.
				priority+=2;
				uri=new Uri(sb.ToString());
				//calculate the absolute path's depth  and increase the priority accordingly
				if(uri.AbsolutePath.Length>1)
				{
					for(i=1; i<uri.AbsolutePath.Length; i++)
					{
						if(uri.AbsolutePath[i]=='/')
						{
							priority++;
						}
					}
				}
				sb.Remove(pos, sb.Length-pos);
				int numParams=0; bool foundSID=false;
				string [] urlParams = uri.Query.Split('?','&',';');
				foreach (string urlParam in urlParams)
				{
					if(urlParam!="")
					{
						//check if the parameter is a session id, if so then skip it
						foundSID=false;
						for(i=0; i<SIDVarNames.Length; i++)
						{
							if(urlParam.IndexOf(SIDVarNames[i])!=-1)
							{
								foundSID=true;
								priority+=2; //Each SID variable has a penalty of 2 points
								break;
							}
						}
						if(!foundSID)
						{
							if(sessionIDRegex.IsMatch(urlParam))
							{
								foundSID = true;
								priority +=2;
							}
						}
						if(!foundSID)
						{
							if(numParams==0)
							{
								sb.Append('?');
							}
							else
							{
								sb.Append('&');
							}
							sb.Append(urlParam);
							numParams++;
							priority++;
						}
					}
					if(numParams==3) //only keep as many as 3 parameters
					{
						break;
					}
				}
			}
			url=sb.ToString();
			return priority;
		}

		/// <summary>
		/// Validates the user's input
		/// </summary>
		/// <returns>True if the user input is OK, otherwise false.</returns>
		private bool ValidateForm()
		{
			txtUrl.Text = txtUrl.Text.Trim();
			if(txtUrl.Text == String.Empty)
			{
				MessageBox.Show("You must supply a new Url Address to insert!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtUrl.Focus();
				return false;
			}
			if(!validator.Validate(txtUrl.Text))
			{
				MessageBox.Show("The Url Address you provided is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtUrl.Focus();
				return false;
			}
			return true;
		}

		/// <summary>
		/// Inserts a new Url in the system's database.
		/// </summary>
		/// <returns>True if the operation succeeds, false otherwise.</returns>
		private bool InsertUrl()
		{
			bool retVal = false;
			try
			{
				//Perform some extra validation and processing of the Url
				string url = txtUrl.Text;
				byte priority = 0;
				InternetUrlToIndex toInsert = null;
				priority = ProcessUrl(ref url);
				txtUrl.Text = url;
				toInsert = new InternetUrlToIndex(url);
				toInsert.FlagDomain = DomainFlagValue.Unknown;
				toInsert.FlagRobots = false;
				toInsert.Priority = priority;
				//Connect to the database
				SqlConnection dbcon = null;
				try
				{
					dbcon = new SqlConnection(globals.ProvideConnectionString());
					dbcon.Open();
				}
				catch(Exception e)
				{
					if(dbcon != null)
					{
						dbcon.Dispose();
						dbcon = null;
					}
					throw e;
				}
				//perform the insertion
				SqlCommand hostcmd = new SqlCommand("cw_insert_host", dbcon);
				hostcmd.CommandType = CommandType.StoredProcedure;
				hostcmd.Parameters.Add("@host_id", SqlDbType.UniqueIdentifier);
				hostcmd.Parameters.Add("@host_name", SqlDbType.NVarChar, 100);

				string hostname = InternetUtils.HostName(toInsert);
				Guid host_id = new Guid(MD5Hash.md5(hostname));
				hostcmd.Parameters[0].Value = host_id;
				hostcmd.Parameters[1].Value = hostname;
				hostcmd.ExecuteNonQuery();

				SqlCommand urlcmd = new SqlCommand("cw_insert_url", dbcon);
				urlcmd.CommandType = CommandType.StoredProcedure;
                urlcmd.Parameters.Add("@url", SqlDbType.NVarChar, 500);
				urlcmd.Parameters.Add("@url_md5", SqlDbType.UniqueIdentifier);
				urlcmd.Parameters.Add("@url_host_id", SqlDbType.UniqueIdentifier);
				urlcmd.Parameters.Add("@url_priority", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@flag_domain", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@flag_robots", SqlDbType.TinyInt);
				urlcmd.Parameters.Add("@id", SqlDbType.Int);
				urlcmd.Parameters["@id"].Direction = ParameterDirection.Output;
				urlcmd.Parameters[0].Value = toInsert.Url;
				urlcmd.Parameters[1].Value = new Guid(toInsert.MD5);
				urlcmd.Parameters[2].Value = host_id;
				urlcmd.Parameters[3].Value = toInsert.Priority;
				urlcmd.Parameters[4].Value = (byte)toInsert.FlagDomain;
				urlcmd.Parameters[5].Value = (byte)((toInsert.FlagRobots)?1:0);
				urlcmd.ExecuteNonQuery();
				urlcmd.Dispose();
				dbcon.Close();
				retVal = true;
			}
			catch(Exception e)
			{
				globals.Log.LogError("CrawlWave.ServerManager failed to insert an entry in the Banned Hosts table: " + e.ToString());
				MessageBox.Show(this.Text + " failed to insert the provided host in the list of Banned Hosts:\n" + e.Message);
				GC.Collect();
			}
			return retVal;
		}

		#endregion

	}
}
