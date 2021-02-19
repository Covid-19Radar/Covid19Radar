using System;
using BackgroundTasks;
using Covid19Radar.iOS.Services.Logs;
using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogPeriodicDeleteServiceIos))]

namespace Covid19Radar.iOS.Services.Logs
{
	public class LogPeriodicDeleteServiceIos : ILogPeriodicDeleteService
	{
		private static readonly string          _identifier = AppInfo.PackageName + ".delete-old-logs";
		private        readonly ILoggerService  _logger;
		private        readonly ILogFileService _log_file;

		public LogPeriodicDeleteServiceIos()
		{
			_logger   = DependencyService.Resolve<ILoggerService>();
			_log_file = DependencyService.Resolve<ILogFileService>();
		}

		public void Init()
		{
			_logger.StartMethod();
			BGTaskScheduler.Shared.Register(_identifier, null, task => {
				this.HandleAppRefresh((BGAppRefreshTask)(task));
			});
			this.ScheduleAppRefresh();
			_logger.EndMethod();
		}

		private void HandleAppRefresh(BGAppRefreshTask task)
		{
			_logger.StartMethod();
			try {
				this.ScheduleAppRefresh();
				var queue = new NSOperationQueue();
				queue.MaxConcurrentOperationCount = 1;
				task.ExpirationHandler = () => {
					_logger.Info("The task was expired.");
					queue.CancelAllOperations();
				};
				var operation = new DeleteOldLogsOperation(_logger, _log_file);
				operation.CompletionBlock = () => {
					_logger.Info($"Operation completed. Is cancelled? {operation.IsCancelled}");
					task.SetTaskCompleted(!operation.IsCancelled);
				};
				queue.AddOperation(operation);
			} catch (Exception e) {
				_logger.Exception("Failed.", e);
			} finally {
				_logger.EndMethod();
			}
		}

		private void ScheduleAppRefresh()
		{
			_logger.StartMethod();
			int oneDay = 1 * 24 * 60 * 60;
			var request = new BGAppRefreshTaskRequest(_identifier);
			request.EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(oneDay); // Fetch no earlier than 1 day from now
			_logger.Info($"The request earliest begin date: {request.EarliestBeginDate}");
			BGTaskScheduler.Shared.Submit(request, out var error);
			if (error != null) {
				_logger.Error($"Could not schedule app refresh. Error: {error}");
			}
			_logger.EndMethod();
		}
	}

	internal class DeleteOldLogsOperation : NSOperation
	{
		private readonly ILoggerService  _logger;
		private readonly ILogFileService _log_file;

		public DeleteOldLogsOperation(ILoggerService logger, ILogFileService logFile)
		{
			_logger   = logger;
			_log_file = logFile;
		}

		public override void Main()
		{
			_logger.StartMethod();
			base.Main();
			try {
				_log_file.Rotate();
				_logger.Info("Periodic deletion of old logs.");
			} catch (Exception e) {
				_logger.Exception("Failed.", e);
			} finally {
				_logger.EndMethod();
			}
		}
	}
}
