using System;
using Android.App;
using Android.Content;
using Covid19Radar.Droid.Services.Logs;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogPeriodicDeleteServiceAndroid))]

namespace Covid19Radar.Droid.Services.Logs
{
	public class LogPeriodicDeleteServiceAndroid : ILogPeriodicDeleteService
	{
		private static readonly int            _request_code              = 1000;
		private static readonly long           _execution_interval_millis = 60 * 60 * 24 * 1000; // 24hours
		private        readonly ILoggerService _logger;

		public LogPeriodicDeleteServiceAndroid()
		{
			_logger = DependencyService.Resolve<ILoggerService>();
		}

		public void Init()
		{
			long nextScheduledTime = SetNextSchedule();
			_logger.Info($"Next scheduled time: {DateTimeOffset.FromUnixTimeMilliseconds(nextScheduledTime).ToOffset(new TimeSpan(9, 0, 0))}");
		}

		public static long SetNextSchedule()
		{
			long nextScheduledTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _execution_interval_millis;
			var  context           = Platform.AppContext;
			var  intent            = new Intent(context, typeof(LogPeriodicDeleteReceiver));
			var  pendingIntent     = PendingIntent.GetBroadcast(context, _request_code, intent, PendingIntentFlags.CancelCurrent);
			if (context.GetSystemService(Context.AlarmService) is AlarmManager alermService) {
				alermService.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, nextScheduledTime, pendingIntent);
			}
			return nextScheduledTime;
		}
	}

	[BroadcastReceiver()]
	[IntentFilter(new[] { Intent.ActionBootCompleted })]
	public class LogPeriodicDeleteReceiver : BroadcastReceiver
	{
		private readonly ILoggerService  _logger;
		private readonly ILogFileService _log_file;

		public LogPeriodicDeleteReceiver()
		{
			var essentials = new EssentialsService();
			var logPath    = new LogPathService(new LogPathServiceAndroid());
			var writer     = new LogReaderWriter(logPath, essentials);
			_logger        = new LoggerService(writer);
			_log_file      = new LogFileService(_logger, logPath);
		}

		public override void OnReceive(Context context, Intent intent)
		{
			_logger.StartMethod();
			try {
				_logger.Info($"Action: {intent.Action}");
				_log_file.Rotate();
				_logger.Info("Periodic deletion of old logs.");
				long nextScheduledTime = LogPeriodicDeleteServiceAndroid.SetNextSchedule();
				_logger.Info($"Next scheduled time: {DateTimeOffset.FromUnixTimeMilliseconds(nextScheduledTime).ToOffset(new TimeSpan(9, 0, 0))}");
			} catch (Exception e) {
				_logger.Exception("Failed.", e);
			} finally {
				_logger.EndMethod();
			}
		}
	}
}
