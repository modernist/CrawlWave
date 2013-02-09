using System;
using System.Timers;

namespace CrawlWave.Common
{
	/// <summary>
	/// AlarmTriggerEventArgs is a class derived from EventArgs and is used for
	/// passing aguments to the OnAlarmTrigger events of the AlarmTimer class.
	/// </summary>
	public class AlarmTriggerEventArgs : EventArgs
	{
		/// <summary>
		/// The <see cref="DateTime">DateTime</see> when the Alarm is set.
		/// </summary>
		public readonly DateTime dtAlarm;

		/// <summary>
		/// The constructor for the AlarmTriggerEventArgs
		/// </summary>
		/// <param name="dt">The <see cref="DateTime">DateTime</see> when the AlarmTimer has been set</param>
		public AlarmTriggerEventArgs(DateTime dt)
		{
			dtAlarm=dt;
		}
	}

	/// <summary>
	/// AlarmTimer is a timer that raises an event at a specified time. The alarm
	/// stops after raising the first OnAlarmTrigger/OnAlarmBell Event. It is used
	/// as a simple Time Scheduler in applications that need to be 'waken up' at a
	/// given time. OnAlarmTrigger uses  the AlarmTriggerEventArgs delegate,
	/// so that the clients can find out the exact time the AlarmTimer was set to.
	/// It has a (configurable) accuracy of 1 minute which is more than enough for
	/// the scope of this project.
	/// Written:10/05/2003
	/// Updated:24/08/2004 -> Added the OnAlarmBell event.
	/// </summary>
	public class AlarmTimer
	{
		private DateTime alarmTime;
		private bool enabled;
		private Timer alarmTimer;
		
		/// <summary>
		/// The delegate for the observer of the AlarmTimer's Events
		/// </summary>
		public delegate void AlarmTrigger(object Alarm, AlarmTriggerEventArgs e);

		/// <summary>
		/// The OnAlarmBell event the clients will subscribe to
		/// </summary>
		public event EventHandler OnAlarmBell;

		/// <summary>
		/// The OnAlarmTrigger event the clients will subscribe to
		/// </summary>
		public event AlarmTrigger OnAlarmTrigger;

		/// <summary>
		/// The default constructor
		/// </summary>
		public AlarmTimer()
		{
			alarmTime=DateTime.Now;
			enabled=false;
			alarmTimer=new Timer(60000); //we need accuracy of 1 minute
			alarmTimer.Enabled=true;
			alarmTimer.Elapsed+=new ElapsedEventHandler(tmrAlarm_Elapsed);
		}

		#region Public Properties
		/// <summary>
		/// The DateTime when the events will be fired
		/// </summary>
		public DateTime AlarmTime
		{
			get { return alarmTime; }
			set { alarmTime= value; }
		}

		/// <summary>
		/// Gets/sets a value indicating if the Alarm is enabled
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				enabled=value;
				if(value==true)
				{
					//start ticking...
					this.Start();
				}
			}
		}
		#endregion

		/// <summary>
		/// Sets the AlarmTimer in motion
		/// </summary>
		public void Start()
		{
			if (!alarmTimer.Enabled)
			{
				alarmTimer.Enabled=true;
				alarmTimer.Start();
			}
		}

		/// <summary>
		/// If the AlarmTimer is enabled this method is called every minute. If the current
		/// time is equal to the Alarm Time (with an accuracy of 1 minute) then the events
		/// are fired and the Alarm is disabled.
		/// </summary>
		/// <param name="alarm">The AlarmTimer used</param>
		/// <param name="e">The internal timer's EventArgs</param>
		private void tmrAlarm_Elapsed(object alarm, ElapsedEventArgs e)
		{
			if (enabled)
			{
				DateTime now=DateTime.Now;
				if ((alarmTime.Hour==now.Hour)&&(alarmTime.Minute==now.Minute))
				{
					AlarmTriggerEventArgs alarmeventargs=new AlarmTriggerEventArgs(alarmTime);
					//Console.WriteLine("in last tick, onalarmtriggernull={0}",OnAlarmTrigger.ToString());
					if (OnAlarmTrigger!=null)//if there are observers attached
					{
						OnAlarmTrigger(this,alarmeventargs);
					}
					if(OnAlarmBell!=null)
					{
						OnAlarmBell(this,EventArgs.Empty);
					}
					//no more events must be raised, so stop the Timer
					alarmTimer.Stop();
				}
				/*else
				{
					Console.WriteLine("wait another 60secs");
				}*/
			}
		}

	}
}
