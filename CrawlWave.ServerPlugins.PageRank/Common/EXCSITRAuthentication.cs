using System;
using System.Diagnostics;
using EXCSCommon;
using EXCSDBUtil;
using System.Data;
using System.Text.RegularExpressions;

namespace EXCSITRAuthentication 
{
	/// Summary description for EXCSITRAuthentication.

	public enum ConnString
	{
		TEST = 1,
		EX = 2
	}


	public class TEXCSITRAuthentication 
	{
		
		protected	System.Data.SqlClient.SqlConnection SQLConn;
		private		System.Data.SqlClient.SqlCommand SQLCmd;
		protected	int MaxRetryTimes = 4;
		public		int SessionTimeOutMins;
		protected	Regex UsrPwdPattern = new Regex(@"[A-Za-z0-9]",RegexOptions.Compiled  );


		public TEXCSITRAuthentication (ConnString enumConnString, int intSessionTimeoutMins)
		{
			SessionTimeOutMins = intSessionTimeoutMins;

			this.SQLConn = new System.Data.SqlClient.SqlConnection();
			switch (enumConnString)
			{
				case ConnString.TEST :
					this.SQLConn.ConnectionString =  TEXCSDBUtil.DBGetConnectionStr(EXCSDBUtil.TEXCSDBUtil.enmdb_EXDBApplUser.db_EXSQL06_TEMP3_EX_Itr_Authenticator_itr_user); // TEXCSDBUtil.C_CONSTR_ITR_EXSQL06_TEMP3;
					break;
				case ConnString.EX :
					this.SQLConn.ConnectionString = TEXCSDBUtil.DBGetConnectionStr(EXCSDBUtil.TEXCSDBUtil.enmdb_EXDBApplUser.db_EXSQL06_EX_Itr_Authenticator_itr_user); //TEXCSDBUtil.C_CONSTR_ITR_EXSQL06_EX;
					break;
				default:			
					this.SQLConn.ConnectionString = TEXCSDBUtil.DBGetConnectionStr(EXCSDBUtil.TEXCSDBUtil.enmdb_EXDBApplUser.db_EXSQL06_TEMP3_EX_Itr_Authenticator_itr_user); // TEXCSDBUtil.C_CONSTR_ITR_EXSQL06_TEMP3;
					break;
			}


			this.SQLCmd = new System.Data.SqlClient.SqlCommand();
			this.SQLCmd.CommandType = CommandType.StoredProcedure;
			this.SQLCmd.Connection = this.SQLConn;

			this.SQLConn.Open();
		}


//--- Login --- AN EINAI ARNITIKO EXW PROBLIMA. ALLIWS EINAI TO LOGINID
// MPOREI NA SOU EPISTREPSEI LOGIN ID, ALLA PREPEI NA TSEKARW TO bMustChangePasswd
		public int Login (string strUserName, string strPassword, //DateTime dtLoginDateTime, 
					int intSessionID, ref Boolean bMustChangePasswd, ref string strFullName, ref string strError)
		{
			SQLCmd.Parameters.Clear();
			SQLCmd.CommandText = "ITR_DoLogin";

			System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (SQLCmd);
			
    		SQLCmd.Parameters["@strUserName"].Value = strUserName;
			SQLCmd.Parameters["@strPIN"].Value = strPassword;
			SQLCmd.Parameters["@LogDate"].Value = DateTime.Now; // dtLoginDateTime.ToString();
			SQLCmd.Parameters["@SessionID"].Value = intSessionID;
			SQLCmd.Parameters["@nbRetryTimes"].Value = MaxRetryTimes;
			SQLCmd.Parameters["@ExpTimeoutMins"].Value = SessionTimeOutMins;
			SQLCmd.Parameters["@bMustChangePwd"].Value = false;
			SQLCmd.Parameters["@FullName"].Value = "";
			SQLCmd.Parameters["@errMess"].Value = "";

			SQLCmd.ExecuteNonQuery();

			bMustChangePasswd = Convert.ToBoolean (SQLCmd.Parameters["@bMustChangePwd"].Value);
			strFullName = SQLCmd.Parameters["@FullName"].Value.ToString();
			strError =  SQLCmd.Parameters["@errMess"].Value.ToString();

			return Convert.ToInt32 (SQLCmd.Parameters[0].Value);
		}
//----------


//--- Logout ---
		public int Logout (int intLoginID) //, DateTime dtLogoutDateTime)
		{
			SQLCmd.Parameters.Clear();
			SQLCmd.CommandText = "ITR_DoLogout";

			System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (SQLCmd);
			
			SQLCmd.Parameters["@LoginID"].Value = intLoginID;
			SQLCmd.Parameters["@LogDate"].Value = DateTime.Now; //dtLogoutDateTime;

			SQLCmd.ExecuteNonQuery();

			return Convert.ToInt32 (SQLCmd.Parameters[0].Value);
		}
//----------


//--- IsPasswdRegex ---
		protected Boolean IsPasswdRegex (string strPasswd, ref string strError)
		{
			if (strPasswd.Length != 5)
			{
				strError = "Το Password πρέπει να αποτελείται από 5 χαρακτήρες.";
				return false;
			}

			if (UsrPwdPattern.Matches(strPasswd).Count != strPasswd.Length)
			{
				strError = "Το Password περιέχει μη επιτρεπτούς χαρακτήρες.";
				return false;
			}

			return true;
		}
//----------

//--- ChangePassword --- PERNAW TO LOGIN ID
//AN GYRISEI 0 OL OK. AN GYRISEI ARNITIKO DEIXNW TO MINIMA KAI ELEGXW TO bLoggedOut
		public int ChangePassword (int intLoginID, string strOldPwd, string strNewPwd, //DateTime dtLoginDateTime, 
					int intSessionID, ref Boolean bLoggedOut, ref string strError)
		{			
			if (! IsPasswdRegex (strNewPwd, ref strError))
			{
				bLoggedOut = false;
				return -2;
			}


			SQLCmd.Parameters.Clear();
			SQLCmd.CommandText = "ITR_DoChangePIN";

			System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (SQLCmd);
			
			SQLCmd.Parameters["@LoginID"].Value = intLoginID;
			SQLCmd.Parameters["@strOldPIN"].Value = strOldPwd;
			SQLCmd.Parameters["@strNewPIN"].Value = strNewPwd;
			SQLCmd.Parameters["@LogDate"].Value = DateTime.Now; // dtLoginDateTime.ToString();
			SQLCmd.Parameters["@SessionID"].Value = intSessionID;
			SQLCmd.Parameters["@nbRetryTimes"].Value = MaxRetryTimes;
			SQLCmd.Parameters["@ExpTimeoutMins"].Value = SessionTimeOutMins;
			SQLCmd.Parameters["@bLoggedOut"].Value = false;
			SQLCmd.Parameters["@errMess"].Value = "";

			SQLCmd.ExecuteNonQuery();
			bLoggedOut = Convert.ToBoolean (SQLCmd.Parameters["@bLoggedOut"].Value);
			strError =  SQLCmd.Parameters["@errMess"].Value.ToString();

			return Convert.ToInt32 (SQLCmd.Parameters[0].Value);
		}
//----------


//--- Authenticate ---
// EPISTREFEI TO ACCOUNTID. ARNITIKO H 0 EXEI LATHOS
//
		public int Authenticate(int intLoginID, int intSessionID, string strActionDescription, 
					ref Boolean bLoggedOut, ref Boolean bMustChangePwd, ref string strError)
		{			
			SQLCmd.Parameters.Clear();
			SQLCmd.CommandText = "ITR_DoAuthenticate";

			System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (SQLCmd);
			
			SQLCmd.Parameters["@LoginID"].Value = intLoginID;
			SQLCmd.Parameters["@LogDate"].Value = DateTime.Now; // dtLoginDateTime.ToString();
			SQLCmd.Parameters["@SessionID"].Value = intSessionID;
			SQLCmd.Parameters["@strOrderAction"].Value = strActionDescription;
			SQLCmd.Parameters["@ExpTimeoutMins"].Value = SessionTimeOutMins;
			SQLCmd.Parameters["@bLoggedOut"].Value = false;
			SQLCmd.Parameters["@bMustChangePwd"].Value = false;
			SQLCmd.Parameters["@errMess"].Value = "";

			SQLCmd.ExecuteNonQuery();
			bLoggedOut = Convert.ToBoolean (SQLCmd.Parameters["@bLoggedOut"].Value);
			bMustChangePwd = Convert.ToBoolean (SQLCmd.Parameters["@bMustChangePwd"].Value);
			strError =  SQLCmd.Parameters["@errMess"].Value.ToString();

			return Convert.ToInt32 (SQLCmd.Parameters[0].Value);
		}
//----------


//--- DeactivatePassword ---
		public int DeactivatePassword (int intLoginID, int intSessionID, ref string strError)
		{			
			SQLCmd.Parameters.Clear();
			SQLCmd.CommandText = "ITR_DoDeactivetePIN";

			System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (SQLCmd);
		
			SQLCmd.Parameters["@LoginID"].Value = intLoginID;
			SQLCmd.Parameters["@LogDate"].Value = DateTime.Now; // dtLoginDateTime.ToString();
			SQLCmd.Parameters["@SessionID"].Value = intSessionID;
			SQLCmd.Parameters["@ExpTimeoutMins"].Value = SessionTimeOutMins;
			//SQLCmd.Parameters["@bMustChangePwd"].Value = false;
			SQLCmd.Parameters["@errMess"].Value = "";

			SQLCmd.ExecuteNonQuery();
			//bMustChangePwd = Convert.ToBoolean (SQLCmd.Parameters["@bMustChangePwd"].Value);
			strError =  SQLCmd.Parameters["@errMess"].Value.ToString();

			return Convert.ToInt32 (SQLCmd.Parameters[0].Value);
			}
//----------


//--- IsUserNameRegex ---
		protected Boolean IsUserNameRegex (string strUserName, ref string strError)
		{
			if ((strUserName.Length < 5) || (strUserName.Length > 15))
			{
				strError = "Το Username πρέπει να αποτελείται από 5 έως 15 χαρακτήρες.";
				return false;
			}

			if (UsrPwdPattern.Matches(strUserName).Count != strUserName.Length)
			{
				strError = "Το Username περιέχει μη επιτρεπτούς χαρακτήρες.";			
				return false;
			}

			return true;
		}
//----------

//--- ChangeUserName ---
		public int ChangeUserName (int intLoginID, string strNewUserName, //DateTime dtLoginDateTime, 
					int intSessionID, ref Boolean bLoggedOut, ref Boolean bMustChangePwd, ref string strError)
		{
			if (! IsUserNameRegex (strNewUserName, ref strError))
			{
				bLoggedOut = false;
				bMustChangePwd = false;
				return -2;
			}


			SQLCmd.Parameters.Clear();
			SQLCmd.CommandText = "ITR_DoChangeUID";

			System.Data.SqlClient.SqlCommandBuilder.DeriveParameters (SQLCmd);
		
			SQLCmd.Parameters["@LoginID"].Value = intLoginID;
			SQLCmd.Parameters["@NewUID"].Value = strNewUserName;
			SQLCmd.Parameters["@LogDate"].Value = DateTime.Now; // dtLoginDateTime.ToString();
			SQLCmd.Parameters["@SessionID"].Value = intSessionID;
			SQLCmd.Parameters["@ExpTimeoutMins"].Value = SessionTimeOutMins;
			SQLCmd.Parameters["@bMustChangePwd"].Value = false;
			SQLCmd.Parameters["@bLoggedOut"].Value = false;
			SQLCmd.Parameters["@errMess"].Value = "";

			SQLCmd.ExecuteNonQuery();
			bLoggedOut = Convert.ToBoolean (SQLCmd.Parameters["@bLoggedOut"].Value);
			bMustChangePwd = Convert.ToBoolean (SQLCmd.Parameters["@bMustChangePwd"].Value);
			strError =  SQLCmd.Parameters["@errMess"].Value.ToString();

			return Convert.ToInt32 (SQLCmd.Parameters[0].Value);
		}
//----------

	}
}
