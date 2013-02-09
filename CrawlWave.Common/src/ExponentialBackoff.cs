using System;

namespace CrawlWave.Common
{
	/// <summary>
	/// BackOffSpeed is an enumeration that allows the user to select a function
	/// that produces integers in logarithmic, linear or exponential intervals.
	/// </summary>
	public enum BackoffSpeed
	{
		/// <summary>
		/// Does not rise at all, causes <see cref="ExponentialBackoff.Next"/> to return
		/// a constant delay of 30 seconds, not dependent on the number of calls.
		/// </summary>
		Constant,
		/// <summary>
		/// Rises slowly, according to the function e^(log10(x*sqrt(x))) - pretty nice!
		/// </summary>
		Slow,
		/// <summary>
		/// Rises linearly, according to the function x
		/// </summary>
		Linear,
		/// <summary>
		/// Rises fast, according to the function 2^sqrt(3^ln(x))
		/// </summary>
		Fast,
		/// <summary>
		/// Falling smoothly, according to the function 30/(1+sqrt(x-1))
		/// </summary>
		Declining
	}

	/// <summary>
	/// ExponentialBackoff provides easy access to Exponential Backoff functions. It is
	/// used whenever there is need for a method that provides delay intervals (in msec)
	/// based on the number of calls (that correspond to the number of failed attempts).
	/// Written:10/07/2003
	/// Updated:24/08/2004 -&gt; Changed variable names, added comments, general revision.
	///			15/11/2004 -&gt; Added support for declining function.
	///			17/01/2005 -&gt; Added MaximumDelay Property and constructor parameter checking.
	/// </summary>
	public class ExponentialBackoff
	{
		#region Private variables

		private int attempts; //the number of times the BackOff function has been used
		private BackoffSpeed speed; //the function to be used for the delay calculation
		private int maxDelay; //Maximum delay - by default set to 10 minutes

		#endregion

		#region Public Constructors

		/// <summary>
		/// Default contructor for an ExponentialBackoff object. It sets the <see cref="BackoffSpeed"/>
		/// to Linear and the Maximum Delay to 10 minutes.
		/// </summary>
		public ExponentialBackoff()
		{
			attempts=0;
			maxDelay=600000;
			speed=BackoffSpeed.Linear;
		}

		/// <summary>
		/// Contructor for an ExponentialBackoff object, sets the Maximum Delay to 10 min.
		/// </summary>
		/// <param name="Speed">The <see cref="BackoffSpeed"/> that will be used for this instance</param>
		public ExponentialBackoff(BackoffSpeed Speed)
		{
			attempts=0;
			maxDelay=600000;
			speed=Speed;
		}

		/// <summary>
		/// Contructor for an ExponentialBackoff object, sets the <see cref="BackoffSpeed"/> to Linear.
		/// </summary>
		/// <param name="MaxDelay">The maximum delay that must be returned by the <see cref="Next"/> method, in milliseconds.</param>
		public ExponentialBackoff(int MaxDelay)
		{
			if(MaxDelay <= 0)
			{
				throw new ArgumentOutOfRangeException("MaxDelay", "must be greater than zero");
			}
			attempts=0;
			maxDelay=MaxDelay;
			speed=BackoffSpeed.Linear;
		}

		/// <summary>
		/// Contructor for an ExponentialBackoff object.
		/// </summary>
		/// <param name="Speed">The <see cref="BackoffSpeed"/> that will be used for this instance</param>
		/// <param name="MaxDelay">The maximum delay in milliseconds</param>
		public ExponentialBackoff(BackoffSpeed Speed, int MaxDelay)
		{
			if(MaxDelay <= 0)
			{
				throw new ArgumentOutOfRangeException("MaxDelay", "must be greater than zero");
			}
			attempts=0;
			maxDelay=MaxDelay;
			speed=Speed;
		}

		#endregion

		#region Public Interface Methods

		/// <summary>
		/// Calculates the interval in msec that must precede the next call of a method
		/// because of a previous failure.
		/// </summary>
		/// <returns>The backoff interval in milliseconds</returns>
		public int Next()
		{
			attempts++;
			int retVal=maxDelay;
			try
			{
				switch(speed)
				{
					case BackoffSpeed.Constant:
						retVal = 30000;
						break;
					case BackoffSpeed.Slow:
						retVal=1000*((int)Math.Exp(Math.Log10(attempts*Math.Sqrt(attempts))));
						break;

					case BackoffSpeed.Linear:
						retVal=1000*attempts;
						break;

					case BackoffSpeed.Fast:
						retVal=1000*((int)Math.Pow(Math.E,Math.Sqrt(Math.Pow(Math.E, Math.Log(attempts)))));
						break;

					case BackoffSpeed.Declining:
						retVal = 1000*((int)(30/(1+Math.Sqrt(attempts - 1))));
						break;
				}
				if (retVal>maxDelay) //if the value returned isn't less than the maximum delay
				{
					//Return the maximum delay and reinitialize the Attempts counter to
					//a state that will cause small values to be produced the next time
					retVal=maxDelay;
					attempts/=3;
				}
			}
			catch
			{
				//an exception has occured durnig the calculation of the interval.
				//return the maximum delay time.
				retVal=maxDelay;
			}
			return retVal;
		}

		/// <summary>
		/// Resets the delay interval
		/// </summary>
		/// <remarks>
		/// Reset() can be used whenever there is need to re-initialize the ExponentialBackOff
		/// object, that will start producing intervals as if it were just created.
		/// </remarks>
		public void Reset()
		{
			attempts=0;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the <see cref="ExponentialBackoff"/> object's <see cref="BackoffSpeed"/>.
		/// </summary>
		public BackoffSpeed Speed
		{
			get { return speed; }
			set { speed = value;}
		}

		/// <summary>
		/// Gets or sets the <see cref="ExponentialBackoff"/> object's Maximum delay in millisecods.
		/// </summary>
		public int MaximumDelay
		{
			get { return maxDelay; }
			set
			{
				if(value <= 0)
				{
					throw new ArgumentOutOfRangeException("MaximumDelay", "must be greater than zero");
				}
				maxDelay = value;
			}
		}

		/// <summary>
		/// Gets a default value for the BackOff that corresponds to 30 seconds.
		/// </summary>
		public static int DefaultBackoff
		{
			get { return 30000; }
		}

		#endregion
	}
}
