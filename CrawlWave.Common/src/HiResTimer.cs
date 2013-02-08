using System;

namespace CrawlWave.Common
{
	/// <summary>
	/// HiResTimer is a class that allows for time intervals measurement. 
	/// The provided method returns results with accuracy of milliseconds.
	/// It is currently being used by the Crawler class of the Client to
	/// measure the time that it took for the data of a url to be downloaded.
	/// Author:	Kostas Stroggylos [mod], kostas@circular.gr
	/// Written:05/07/2003
	/// Updated:24/08/2004 -&gt; General revision, better comments and naming conventions.
	/// </summary>
	public class HiResTimer
	{
		#region Private variables

		private DateTime start;
		private TimeSpan duration;

		#endregion

		/// <summary>
		/// The default constructor
		/// </summary>
		public HiResTimer()
		{
			//nothing to initialize...
		}

		/// <summary>
		/// Signals the timer to start ticking.
		/// </summary>
		public void Start()
		{
			start=DateTime.Now;
		}

		/// <summary>
		/// Signals the timer to stop ticking.
		/// </summary>

		public void Stop()
		{
			duration=DateTime.Now.Subtract(start);
		}
		
		/// <summary>
		/// Gets the amount of time that has passed in msec.
		/// </summary>
		/// <remarks>
		/// It is <b>important</b> to notice that the Duration property is <b>not</b> updated
		/// automatically every millisecond. Its value will be the number of msec
		/// that have passed from the previous call to <see cref="Start">Start</see>
		/// to the previous call to <see cref="Stop">Stop</see>. In other words,
		/// the value of this property is only updated when <see cref="Stop">Stop</see>
		/// is called.
		/// </remarks>
		public long Duration
		{
			get { return (long)duration.TotalMilliseconds; }
		}
	}
}
