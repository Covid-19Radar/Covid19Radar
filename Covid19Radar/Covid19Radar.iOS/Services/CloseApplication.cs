﻿using System.Threading;
using Covid19Radar.iOS.Services;
using Covid19Radar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]

namespace Covid19Radar.iOS.Services
{
	public class CloseApplication : ICloseApplication
	{
		void ICloseApplication.CloseApplication()
		{
			Thread.CurrentThread.Abort();
		}
	}
}
