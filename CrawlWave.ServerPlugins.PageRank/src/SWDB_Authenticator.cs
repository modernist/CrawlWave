using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using EXCSDBUtil; 
using EXCSCommon;


namespace SW_Main
{
	/// <summary>
	/// Summary description for SWDB_Authenticator.
	/// </summary>
	public class TSWDB_Authenticator
	{
		/*	declare @strUser as varchar(33)
			declare @strPassword as varchar(33)
			declare @intUserID as int   

			set @strUser ='akr'
			set @strPassword ='xaxa'


			exec sp_SW_NewUser  @strUser, @strPassword, @intUserID output

			select @intUserID

			SELECT    *
			FROM      SW_UR_User

			exec sp_SW_CheckUser 'xxx' , '' ,@intUserID output
			select @intUserID*/

		//protected	SqlConnection SQLConSW = null;
		protected	SqlCommand sqcm_CheckUser = null;
		protected	SqlCommand sqcm_NewUser = null;
		
		public TSWDB_Authenticator()
		{
			sqcm_CheckUser = TEXCSDBUtil.DBCreateSimpleStoredProc(TSWDB_Decl.GetSQLConSW(), "dbo.sp_SW_CheckUser");
			sqcm_NewUser = TEXCSDBUtil.DBCreateSimpleStoredProc(TSWDB_Decl.GetSQLConSW(), "dbo.sp_SW_NewUser");

		}

		public int CheckUser(string strUser, string strPassword)
		{
			sqcm_CheckUser.Parameters["@strUser"].Value = strUser.Trim(); 
			sqcm_CheckUser.Parameters["@strPassword"].Value = strPassword.Trim(); 
			sqcm_CheckUser.Parameters["@intUserID"].Value=0;
			sqcm_CheckUser.ExecuteNonQuery();
			int intResult = Convert.ToInt32(sqcm_CheckUser.Parameters["@intUserID"].Value);			
			EXException.CheckEXError(intResult>0,(intResult==-1?"Δεν βρέθηκε ο χρήστης.":(intResult==-2?"Δεν βρέθηκε το password.":"Άγνωστο πρόβλημα κατα τον έλεγχο του χρήστη.")));
			return intResult;
		}

		public int NewUser(string strUser, string strPassword)
		{
			sqcm_NewUser.Parameters["@strUser"].Value = strUser.Trim(); 
			sqcm_NewUser.Parameters["@strPassword"].Value = strPassword.Trim(); 
			sqcm_NewUser.Parameters["@intUserID"].Value=0;
			sqcm_NewUser.ExecuteNonQuery();
			int intResult = Convert.ToInt32(sqcm_NewUser.Parameters["@intUserID"].Value);			
			EXException.CheckEXError(intResult>0,(intResult==-1?"Ο χρήστης υπάρχει ήδη.":"Άγνωστο πρόβλημα κατα την εισαγωγή του χρήστη."));
			return intResult;
		}



	}




}
