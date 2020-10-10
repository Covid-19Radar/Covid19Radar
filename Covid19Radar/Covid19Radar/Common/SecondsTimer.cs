﻿using System;
using Xamarin.Forms;

namespace Covid19Radar.Common
{

    public class SecondsTimer
    {
        public delegate void TimeOutHandler(EventArgs e);

        public event TimeOutHandler TimeOutEvent;

		private bool _timerRunning;

        private int _timeDiffernce;

        public SecondsTimer(int timeDiffernce)
        {
            _timeDiffernce = timeDiffernce;
            this._timerRunning = false;
        }
        public void Start()
        {
            if (this._timerRunning == true)
                return;
            RegisterTimer(this.HandleFunc);
        }

        public void Stop()
        {
            this._timerRunning = false;
        }

        private bool HandleFunc()
        {
            if (this._timerRunning == true)
            {

                if (this.TimeOutEvent != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.TimeOutEvent(new EventArgs());
                    });
                }
                RegisterTimer(this.HandleFunc);
            }
            return false;
        }

        private void RegisterTimer(Func<bool> callback)
        {
            //this._startDateTime = DateTime.Now;
            //double spanSecond = 60 - this._startDateTime.Second+_timeDiffernce;

            this._timerRunning = true;
            Device.StartTimer(TimeSpan.FromSeconds(_timeDiffernce), callback);
        }
    }
}
