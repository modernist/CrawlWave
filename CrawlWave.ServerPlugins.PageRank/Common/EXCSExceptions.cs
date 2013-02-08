using System;
using EXCSCommon;

namespace EXCSCommon.Exceptions
{
	/// <summary>
	/// Summary description for EXCSExceptions.
	/// </summary>
	public class EXExcLoginFailure:EXException
	{
		new public static void CheckEXError(bool blnCondition, string strError)
		{
			if (!blnCondition)
			{
				ThrowEXException(strError);	
			}
		}

		new public static void ThrowEXException(string strError)
		{
			throw new EXExcLoginFailure(strError);
		}

		new public static void ThrowEXException(string strError,Exception inner)
		{
			throw new EXExcLoginFailure(strError, inner);
		}


		public EXExcLoginFailure(string strError):base(strError) {}

		public EXExcLoginFailure(string strError,Exception inner):base(strError,inner){}

	}

	public class EXExcLoginChangePassword:EXException
	{
		new public static void CheckEXError(bool blnCondition, string strError)
		{
			if (!blnCondition)
			{
				ThrowEXException(strError);	
			}
		}

		new public static void ThrowEXException(string strError)
		{
			throw new EXExcLoginChangePassword(strError);
		}

		new public static void ThrowEXException(string strError,Exception inner)
		{
			throw new EXExcLoginChangePassword(strError, inner);
		}


		public EXExcLoginChangePassword(string strError):base(strError) {}

		public EXExcLoginChangePassword(string strError,Exception inner):base(strError,inner){}

		/*override protected void AddDataRow(ref Exception inner)
		{
			try
			{
				if ((dtstException==null) || (inner==null))
				{
					return;
				}
				dtstException.Tables[0].Rows.Add(new object[]{dtstException.Tables[0].Rows.Count+1, inner.Message,DateTime.Now,inner.StackTrace,GetExcTypeName()});
				AddExtraRows(ref inner);
			}
			catch{}
		}*/

		/*new protected string GetExcTypeName()
		{
			return this.GetType().Name; 
		}*/
	}

	public class EXRequestInfo:EXException
	{
		new public static void CheckEXError(bool blnCondition, string strError)
		{
			if (!blnCondition)
			{
				ThrowEXException(strError);	
			}
		}

		new public static void ThrowEXException(string strError)
		{
			throw new EXRequestInfo(strError);
		}

		new public static void ThrowEXException(string strError,Exception inner)
		{
			throw new EXRequestInfo(strError, inner);
		}


		public EXRequestInfo(string strError):base(strError) {}

		public EXRequestInfo(string strError,Exception inner):base(strError,inner){}

	}

}
