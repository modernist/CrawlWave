using System;
using System.Collections;
using System.Data ;
using EXCSDecl;

namespace EXCSCommon //test sc web 2
{
	/// <summary>
	/// Summary description for EXCSCommon.
	/// </summary>

	public class EXException:Exception
	{
		public const string C_DTSTDATASET_NAME = "DATASET_EXEXCEPTION";
		public const string C_DTSTTABLE_MAIN_NAME = "TABLE_EXEXCEPTION1";
		public const string C_DTSTFIELD_AA_NAME = "FLDAA";	
		public const string C_DTSTFIELD_MESSAGE_NAME = "FLDMESSAGE";
		public const string C_DTSTFIELD_DATETIME_NAME = "FLDDATETIME";
		public const string C_DTSTFIELD_STACKTRACE_NAME = "FLDSTACKTRACE";
		public const string C_DTSTFIELD_CLASSEXCEPTION_NAME = "FLDCLASSEXCEPTION";
		
		public DataSet dtstException;
		
		private static DataSet InitializeExceptionDataSet()
		{
			DataSet dtstResult;
			try
			{				
				dtstResult = new DataSet(C_DTSTDATASET_NAME);
				dtstResult.Tables.Add(new DataTable(C_DTSTTABLE_MAIN_NAME));
				dtstResult.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_AA_NAME, typeof(uint)));
				dtstResult.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_MESSAGE_NAME, typeof(string)));
				dtstResult.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_DATETIME_NAME, typeof(DateTime)));
				dtstResult.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_STACKTRACE_NAME, typeof(string)));
				dtstResult.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_CLASSEXCEPTION_NAME, typeof(string)));
				
			}
			catch
			{
				dtstResult=null;
			}
			return dtstResult;
		}


		private void InitializeData()
		{
			dtstException = InitializeExceptionDataSet();
			/*try
			{
				dtstException = new DataSet(C_DTSTDATASET_NAME);
				dtstException.Tables.Add(new DataTable(C_DTSTTABLE_MAIN_NAME));
				dtstException.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_AA_NAME, typeof(uint)));
				dtstException.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_MESSAGE_NAME, typeof(string)));
				dtstException.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_DATETIME_NAME, typeof(DateTime)));
				dtstException.Tables[0].Columns.Add(new DataColumn(C_DTSTFIELD_STACKTRACE_NAME, typeof(string)));
			}
			catch
			{
				dtstException=null;
			}*/
		}

		
		public virtual string EXMessage 
		{
			get
			{
				try
				{
					if (dtstException==null) 
					{
						return Message;
					}
					
					string strResult=Message;//+'\n';

					foreach (DataRow dtrwRow in (dtstException.Tables[0].Rows))
					{
						//(inner as EXException).dtstException.Tables[0].Rows.Remove(dtrwRow);
						//DataRow dtrwToAdd = dtrwRow.ItemArray
						strResult=string.Format("{0}\n{1:-3}) {2}", strResult,dtrwRow[C_DTSTFIELD_AA_NAME],dtrwRow[C_DTSTFIELD_MESSAGE_NAME]);
					} 					
					return strResult;
				}
				catch
				{
					return Message;
				}

			}
		}

		public DataSet CreateExcDataSet()
		{
			DataSet dtstResult;
			try
			{
				dtstResult=InitializeExceptionDataSet();

				dtstResult.Tables[0].Rows.Add(new object[]{1, Message,DateTime.Now,StackTrace,GetExcTypeName()});
				if (dtstException!=null)
				{
					foreach (DataRow dtrwRow in dtstException.Tables[0].Rows)
					{
						dtstResult.Tables[0].Rows.Add(new object[]{	dtstResult.Tables[0].Rows.Count+1,dtrwRow.ItemArray[1],dtrwRow.ItemArray[2],dtrwRow.ItemArray[3],dtrwRow.ItemArray[4]});
					} 					
				}				
			}
			catch
			{
				dtstResult = null;
			}
			return dtstResult;
		}

		protected virtual string GetExcTypeName()
		{
			//string strResult=this.GetType().Name; 
			return this.GetType().Name; ;
		}

		protected void AddExtraRows(ref Exception inner)
		{
			//dtstException.Tables[0].Rows.Add(new object[]{inner.Message,DateTime.Now,inner.StackTrace});
			if (inner is EXException)
			{					
				foreach (DataRow dtrwRow in (inner as EXException).dtstException.Tables[0].Rows)
				{
					dtstException.Tables[0].Rows.Add(new object[]{dtstException.Tables[0].Rows.Count+1,
																	 dtrwRow.ItemArray[1],
																	 dtrwRow.ItemArray[2],
																	 dtrwRow.ItemArray[3],
																	 dtrwRow.ItemArray[4]});
				} 					
			}				

		}

		virtual protected void AddDataRow(ref Exception inner)
		{
			try
			{
				if ((dtstException==null) || (inner==null))
				{
					return;
				}
				dtstException.Tables[0].Rows.Add(new object[]{dtstException.Tables[0].Rows.Count+1, inner.Message,DateTime.Now,inner.StackTrace,inner.GetType().Name});
				AddExtraRows(ref inner);
			}
			catch{}
		}

		public EXException():base()
		{
			InitializeData();	
		}
		
		public EXException(string strError):base(strError) 
		{
			InitializeData();	
		}

		public EXException(string strError,Exception inner):base(strError,inner) 		
		{
			InitializeData();
			AddDataRow(ref inner);
		}	
		
		public EXException(Exception inner):base(inner.Message,inner) 		
		{
			InitializeData();
			AddDataRow(ref inner);
		}

		public EXException(DataSet dtstExcDataSet):base( (dtstExcDataSet!=null)?dtstExcDataSet.Tables[0].Rows[0][C_DTSTFIELD_MESSAGE_NAME].ToString():"")
		{
			//InitializeData();
			//AddDataRow(ref inner);
			InitializeData();
			if (dtstExcDataSet!=null)
			{
				/*DataRow dtrwFirstRow=dtstException.Tables[0].Rows[0];
				//dtstException.Tables[0].Rows.Add(new object[]{dtstException.Tables[0].Rows.Count+1, dtrwFirstRow.ItemArray[1],dtrwFirstRow.ItemArray[2],dtrwFirstRow.ItemArray[3],dtrwFirstRow.ItemArray[4]});
				this._*/

				dtstExcDataSet.Tables[0].Rows.RemoveAt(0);
				foreach (DataRow dtrwRow in dtstExcDataSet.Tables[0].Rows)
				{
					dtstException.Tables[0].Rows.Add(new object[]{dtstException.Tables[0].Rows.Count+1,dtrwRow.ItemArray[1],dtrwRow.ItemArray[2],dtrwRow.ItemArray[3],dtrwRow.ItemArray[4]});
				} 				
			}
		}
		
		public static void ThrowEXException()
		{
			throw new EXException(TEXCSDecl.C_MSG_GR_Problem);
			
			//Object [] objArgs = {bdCopy};
			//bdClone = (CBaseData)Activator.CreateInstance(bdCopy.GetType(), objArgs);
			
			//Exception exc = Activator.CreateInstance(type) as Exception;
			//throw exc;
				//((GetType() as Exception) as Type)();
			//object

		}

		public static void ThrowEXException(string strError)
		{
			throw new EXException(strError);
		}

		public static void ThrowEXException(string strError,Exception inner)
		{			
			throw new EXException(strError,inner);
		}

		public static void CheckEXError(bool blnCondition, string strError)
		{
			if (!blnCondition)
			{
				ThrowEXException(strError);	
			}
		}

		public static void CheckEXError(bool blnCondition)
		{
			if (!blnCondition)
			{
				ThrowEXException();	
			}
		}

		public static void ThrowOnDSException(DataSet dtstExcDataSet)
		{
			if (dtstExcDataSet!=null)
			{
				throw new EXException(dtstExcDataSet);
			}
		}
	}



	public class TEXCSGenUtl
	{
		//		public static ArrayList ArrToArrayList(object[] objarrAnArray)
		//		{
		//			ArrayList arrlstResult = new ArrayList();
		//			foreach (object _obj in objarrAnArray)
		//			{
		//				arrlstResult.Add( _obj );
		//			}
		//			return arrlstResult;
		//		}		

		static public char[] ArrayOfArrayCharToArrayChar(char[][] charrArrToParse) 
		{			
			int intArrayLength = 0;
			int intDim = charrArrToParse.Length-1;
			for (int intCounter1=0;intCounter1<=intDim; intCounter1++)
			{
				intArrayLength+=charrArrToParse[intCounter1].Length;
			}
			
			char[] charrResult = new char[intArrayLength];			

			int intIndex=0;
			for (int intCounter1=0;intCounter1<=intDim; intCounter1++)
			{				
				int intLength = charrArrToParse[intCounter1].Length-1;
				for (int intCounter2=0;intCounter2<=intLength; intCounter2++)
				{	
					charrResult[intIndex]=charrArrToParse[intCounter1][intCounter2];
					intIndex++;
				}
			}
			return charrResult;
		}
	}


}
