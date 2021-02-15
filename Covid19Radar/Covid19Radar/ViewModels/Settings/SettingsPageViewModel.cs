using System;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class SettingsPageViewModel : ViewModelBase
	{
		private readonly ILoggerService              _logger;
		private readonly ILogFileService             _log_file;
		private readonly ExposureNotificationService _ens;
		private readonly IUserDataService            _user_data_service;
		private          UserDataModel?              _user_data;
		private          string                      _app_version;

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
			} else {
				if (_user_data.IsExposureNotificationEnabled) {
					await _ens.StartExposureNotification();
				} else {
					await _ens.StopExposureNotification();
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
			ILoggerService              logger,
			ILogFileService             logFile,
			ExposureNotificationService exposureNotificationService,
			IUserDataService            userDataService)
		{
			_logger            = logger                      ?? throw new ArgumentNullException(nameof(logger));
			_log_file          = logFile                     ?? throw new ArgumentNullException(nameof(logFile));
			_ens               = exposureNotificationService ?? throw new ArgumentNullException(nameof(exposureNotificationService));
			_user_data_service = userDataService             ?? throw new ArgumentNullException(nameof(userDataService));
			_user_data         = userDataService.Get();
			_app_version       = AppInfo.VersionString;
			this.Title         = AppResources.SettingsPageTitle;
			this.RaisePropertyChanged(nameof(this.AppVer));
		}
	}
}
