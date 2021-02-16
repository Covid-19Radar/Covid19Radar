using Covid19Radar.Droid.Services.Logs;
using Covid19Radar.Services.Logs;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogFileServiceAndroid))]

namespace Covid19Radar.Droid.Services.Logs
{
	public class LogFileServiceAndroid : ILogFileDependencyService
	{
		public void AddSkipBackupAttribute()
		{
			// Skip backup in `AndroidManifest.xml`
		}
	}
}
