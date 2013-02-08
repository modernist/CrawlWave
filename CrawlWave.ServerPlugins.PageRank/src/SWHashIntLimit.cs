using System;
using System.Collections;

namespace SpiderWaveJobs.Classes
{
	/// <summary>
	/// Summary description for SWHashIntLimit.
	/// </summary>
	public class SWHashIntLimit
	{
		SortedList listInt = null;
		int MaxCountOfItems = 0;
		
		public SWHashIntLimit(int intMaxCountOfItems)
		{
			MaxCountOfItems=intMaxCountOfItems;
			listInt=new SortedList(MaxCountOfItems);
		}

		public void Add(int intKey, int intValue)
		{
			RemoveMinimumValues();
			listInt.Add(intKey,intValue);
		}

		private void RemoveMinimumValues()
		{
			if (listInt.Count>=MaxCountOfItems)
			{
				int intPercentToRemove=MaxCountOfItems / 10; //sbhse to ena dekato.

				for (int intToRemove=1;intToRemove<=intPercentToRemove; intToRemove++)
				{
					listInt.RemoveAt(listInt.Count-1);
				}				
			}
		}

		public bool Find(int intKey, out int intValue)
		{
			if (listInt.Contains(intKey))
			{
				intValue=(int)listInt[intKey];
				return true;
			}
			else
			{
				intValue=0;
				return false;
			}
			
		}

	}
}
