using System;
using Xamarin.Forms;

namespace Covid19Radar.Common
{
	public class HoursTimer : Timer
	{
		public HoursTimer(int timeDiffernce) : base(timeDiffernce) { }

		protected sealed override void RegisterTimer(Func<bool> callback)
		{
			this.TimerRunning = true;
			Device.StartTimer(TimeSpan.FromMinutes(60 - DateTime.UtcNow.Minute + this.TimeDiffernce), callback);
		}
	}
}
