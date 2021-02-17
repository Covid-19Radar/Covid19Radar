using System;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class ChatbotPageViewModel : ViewModelBase
	{
		private readonly ILoggerService   _logger;
		private readonly IUserDataService _user_data_service;
		private          UserDataModel    _user_data;

		public  UserDataModel UserData
		{
			get => _user_data;
			set => this.SetProperty(ref _user_data, value);
		}

		public ICommand OnChangeExposureNotificationState => new Command(this.SaveUserData);
		public ICommand OnChangeNotificationState         => new Command(this.SaveUserData);

		public ICommand OnChangeResetData => new Command(async () => {
			_logger.StartMethod();
			if (await UserDialogs.Instance.ConfirmAsync(
				AppResources.SettingsPageDialogResetText,
				AppResources.SettingsPageDialogResetTitle,
				AppResources.ButtonOk,
				AppResources.ButtonCancel
			)) {
				_logger.Info("The user accepted.");

				UserDialogs.Instance.ShowLoading(AppResources.LoadingTextDeleting);
				if (await ExposureNotification.IsEnabledAsync()) {
					await ExposureNotification.StopAsync();
				}

				// Reset all data and opt-out
				var userData = new UserDataModel();
				await _user_data_service.SetAsync(userData);

				UserDialogs.Instance.HideLoading();
				await UserDialogs.Instance.AlertAsync(AppResources.SettingsPageDialogResetCompletedText);
				Application.Current.Quit();

				// Close the application
				DependencyService.Get<ICloseApplication>().CloseApplication();
			} else {
				_logger.Info("The user did not accept.");
			}
			_logger.EndMethod();
		});

		public ChatbotPageViewModel(ILoggerService logger, IUserDataService userDataService)
		{
			_logger            = logger          ?? throw new ArgumentNullException(nameof(logger));
			_user_data_service = userDataService ?? throw new ArgumentNullException(nameof(userDataService));
			_user_data         = userDataService.Get();
			this.Title         = AppResources.SettingsPageTitle;
		}

		private async void SaveUserData()
		{
			_logger.StartMethod();
			await _user_data_service.SetAsync(_user_data);
			_logger.EndMethod();
		}
	}
}
