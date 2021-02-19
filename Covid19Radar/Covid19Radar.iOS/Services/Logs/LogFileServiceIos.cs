using Covid19Radar.iOS.Services.Logs;
using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogFileServiceIos))]

namespace Covid19Radar.iOS.Services.Logs
{
	public class LogFileServiceIos : ILogFileDependencyService
	{
		public void AddSkipBackupAttribute()
		{
			string logsDirPath = DependencyService.Resolve<ILogPathService>().LogsDirPath;
			var    url         = NSUrl.FromFilename(logsDirPath);
			url.SetResource(NSUrl.IsExcludedFromBackupKey, NSNumber.FromBoolean(true));
		}
	}
}
