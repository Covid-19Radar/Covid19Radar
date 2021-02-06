using System;
using Xamarin.Forms;

namespace Covid19Radar.Common
{
	public class SecondsTimer : Timer
	{
		public SecondsTimer(int timeDiffernce) : base(timeDiffernce) { }

		protected sealed override void RegisterTimer(Func<bool> callback)
		{
			this.TimerRunning = true;
			Device.StartTimer(TimeSpan.FromSeconds(this.TimeDiffernce), callback);
		}
	}
}
