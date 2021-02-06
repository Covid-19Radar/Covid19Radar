using System;
using Xamarin.Forms;

namespace Covid19Radar.Common
{
	public abstract class Timer
	{
		protected       bool            TimerRunning  { get; set; }
		protected       int             TimeDiffernce { get; }
		public    event EventHandler?   TimeOut;

		public Timer(int timeDiffernce)
		{
			this.TimeDiffernce = timeDiffernce;
			this.TimerRunning  = false;
		}

		public void Start()
		{
			if (!this.TimerRunning) {
				this.RegisterTimer(this.HandleFunc);
			}
		}

		public void Stop()
		{
			this.TimerRunning = false;
		}

		private bool HandleFunc()
		{
			if (this.TimerRunning) {
				Device.BeginInvokeOnMainThread(() => this.OnTimeOut(EventArgs.Empty));
				this.RegisterTimer(this.HandleFunc);
			}
			return false;
		}

		protected abstract void RegisterTimer(Func<bool> callback);

		protected virtual void OnTimeOut(EventArgs e)
		{
			this.TimeOut?.Invoke(this, e);
		}
	}
}
