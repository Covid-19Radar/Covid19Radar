using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class SettingsPageViewModel : ViewModelBase
	{
		private readonly ILoggerService   _logger;
		private readonly ILogFileService  _log_file;
		private          string           _app_version;
		private readonly IUserDataService _user_data_service;
		private          UserDataModel?   _user_data;

		public string AppVer
		{
			get => _app_version;
			set => this.SetProperty(ref _app_version, value);
		}

		public UserDataModel? UserData
		{
			get => _user_data;
			set => this.SetProperty(ref _user_data, value);
		}

		public ICommand OnChangeExposureNotificationState => new Command(async () => {
			_logger.StartMethod();
			if (_user_data is null) {
				_logger.Warning("The user data is null.");
			} else if (this.ExposureNotificationService is null) {
				_logger.Warning("The exposure notification service is not available.");
			} else {
				if (_user_data.IsExposureNotificationEnabled) {
					await this.ExposureNotificationService.StartExposureNotification();
				} else {
					await this.ExposureNotificationService.StopExposureNotification();
				}
			}
			_logger.EndMethod();
		});

		public ICommand OnChangeNotificationState => new Command(async () =>
		{
			_logger.StartMethod();
			if (_user_data is null) {
				_logger.Warning("The user data is null.");
			} else {
				await _user_data_service.SetAsync(_user_data);
			}
			_logger.EndMethod();
		});

		public ICommand OnChangeResetData => new Command(async () =>
		{
			_logger.StartMethod();
			if (await UserDialogs.Instance.ConfirmAsync(
				AppResources.SettingsPageDialogResetText,
				AppResources.SettingsPageDialogResetTitle,
				AppResources.ButtonOk,
				AppResources.ButtonCancel))
			{
				UserDialogs.Instance.ShowLoading(AppResources.LoadingTextDeleting);
				if (await ExposureNotification.IsEnabledAsync()) {
					await ExposureNotification.StopAsync();
				}

				// Reset all data and Optout
				await _user_data_service.ResetAllDataAsync();
				_log_file.DeleteLogsDir();

				UserDialogs.Instance.HideLoading();
				await UserDialogs.Instance.AlertAsync(AppResources.SettingsPageDialogResetCompletedText);
				Application.Current.Quit();

				// Close the application
				DependencyService.Get<ICloseApplication>().CloseApplication();
			}
			_logger.EndMethod();
		});

		public SettingsPageViewModel(
			INavigationService          navigationService,
			ExposureNotificationService exposureNotificationService,
			ILoggerService              logger,
			ILogFileService             logFile,
			IUserDataService            userDataService)
			: base(navigationService, exposureNotificationService)
		{
			_logger            = logger;
			_log_file          = logFile;
			_app_version       = AppInfo.VersionString;
			_user_data_service = userDataService;
			_user_data         = userDataService.Get();
			this.Title         = AppResources.SettingsPageTitle;
			this.RaisePropertyChanged(nameof(this.AppVer));
		}
	}
}
