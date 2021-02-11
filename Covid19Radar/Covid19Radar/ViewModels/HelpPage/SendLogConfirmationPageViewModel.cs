using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class SendLogConfirmationPageViewModel : ViewModelBase
	{
		private readonly ILoggerService    _logger;
		private readonly ILogFileService   _log_file;
		private readonly ILogUploadService _log_upload;
		private readonly ILogPathService   _log_path;
		private          string?           _log_id;
		private          string?           _zip_filename;

		public Action<Action>     BeginInvokeOnMainThread { get; set; } = MainThread.BeginInvokeOnMainThread;
		public Func<Action, Task> TaskRun                 { get; set; } = Task.Run;

		public Command OnClickConfirmLogCommand => new Command(() => {
			_logger.StartMethod();
			this.CopyZipFileToPublicPath();
			_logger.EndMethod();
		});

		public Command OnClickSendLogCommand => new Command(async () => {
			_logger.StartMethod();
			try {
				UserDialogs.Instance.ShowLoading(AppResources.Sending);
				bool uploadResult = await _log_upload.UploadAsync(_zip_filename!);
				UserDialogs.Instance.HideLoading();
				if (!uploadResult) {
					_logger.Warning("Failed to upload the ZIP file.");
					await UserDialogs.Instance.AlertAsync(
						AppResources.FailedMessageToSendOperatingInformation,
						AppResources.SendingError,
						AppResources.ButtonOk
					);
					return;
				}
				if (!_log_file.DeleteAllLogUploadingFiles()) {
					_logger.Warning("Failed to delete the ZIP file.");
				}
				var task = this.NavigationService?.NavigateAsync(
					$"{nameof(SendLogCompletePage)}?useModalNavigation=true/",
					new NavigationParameters() { { "logId", _log_id } }
				);
				if (!(task is null)) {
					await task;
				}
			} finally {
				_logger.EndMethod();
			}
		});

		public SendLogConfirmationPageViewModel(
			INavigationService navigationService,
			ILoggerService     logger,
			ILogFileService    logFile,
			ILogUploadService  logUpload,
			ILogPathService    logPath)
			: base(navigationService)
		{
			_logger     = logger;
			_log_file   = logFile;
			_log_upload = logUpload;
			_log_path   = logPath;
		}

		public override void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			this.CreateZipFile();
			_logger.EndMethod();
		}

		public override void Destroy()
		{
			_logger.StartMethod();
			base.Destroy();
			_log_file.DeleteAllLogUploadingFiles();
			_logger.EndMethod();
		}

		private void CreateZipFile()
		{
			_log_id       = _log_file.CreateLogId();
			_zip_filename = _log_file.LogUploadingFileName(_log_id);
			UserDialogs.Instance.ShowLoading(AppResources.Processing);
			this.TaskRun(() => {
				_log_file.Rotate();
				bool failed = !_log_file.CreateLogUploadingFileToTmpPath(_zip_filename);
				this.BeginInvokeOnMainThread(async () => {
					UserDialogs.Instance.HideLoading();
					if (failed) {
						await UserDialogs.Instance.AlertAsync(
							AppResources.FailedMessageToGetOperatingInformation,
							AppResources.Error,
							AppResources.ButtonOk
						);
						this.NavigationService?.GoBackAsync();
					}
				});
			});
		}

		private void CopyZipFileToPublicPath()
		{
			UserDialogs.Instance.ShowLoading(AppResources.Saving);
			this.TaskRun(() => {
				bool failed = !_log_file.CopyLogUploadingFileToPublicPath(_zip_filename!);
				this.BeginInvokeOnMainThread(async () => {
					UserDialogs.Instance.HideLoading();
					if (failed) {
						await UserDialogs.Instance.AlertAsync(
							AppResources.FailedMessageToSaveOperatingInformation,
							AppResources.Error,
							AppResources.ButtonOk
						);
					} else {
						string message = string.Empty;
						switch (Device.RuntimePlatform) {
							case Device.Android:
								message = AppResources.SuccessMessageToSaveOperatingInformationForAndroid
								        + _log_path.LogUploadingPublicPath
								        + AppResources.SuccessMessageToSaveOperatingInformationForAndroid2;
								break;
							case Device.iOS:
								message = AppResources.SuccessMessageToSaveOperatingInformationForIOS;
								break;
						}
						await UserDialogs.Instance.AlertAsync(
							message,
							AppResources.SaveCompleted,
							AppResources.ButtonOk
						);
					}
				});
			});
		}
	}
}
