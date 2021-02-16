using System;
using System.IO;
using Android.Content;
using Covid19Radar.Services.Logs;
using AndroidApp = Android.App.Application;

namespace Covid19Radar.Droid.Services.Logs
{
	public class LogPathServiceAndroid : ILogPathDependencyService
	{
		private static readonly string  _logs_dir_name = "logs";
		private        readonly Context _context       = AndroidApp.Context;

		public string LogsDirPath            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _logs_dir_name);
		public string LogUploadingTmpPath    => _context.CacheDir.Path;
		public string LogUploadingPublicPath => _context.GetExternalFilesDir(string.Empty).Path;

		public LogPathServiceAndroid() { }
	}
}
