using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class TutorialPage4ViewModel : ViewModelBase
	{
		private readonly ILoggerService   _logger;
		private readonly IUserDataService _user_data_service;
		private          UserDataModel?   _user_data;

		public Command OnClickEnable => new Command(async () => {
			_logger.StartMethod();
			if (this.ExposureNotificationService is null) {
				_logger.Warning("The exposure notification service is null.");
			} else {
				await this.ExposureNotificationService.StartExposureNotification();
			}
			if (this.NavigationService is null) {
			} else {
				await this.NavigationService.NavigateAsync(nameof(TutorialPage6));
			}
			_logger.EndMethod();
		});

		public Command OnClickDisable => new Command(async () => {
			_logger.StartMethod();
			if (_user_data is null) {
				_logger.Warning("The user data is null.");
			} else {
				_user_data.IsExposureNotificationEnabled = false;
				await _user_data_service.SetAsync(_user_data);
				if (this.NavigationService is null) {
					_logger.Warning("The navigation service is null.");
				} else {
					await this.NavigationService.NavigateAsync(nameof(TutorialPage6));
				}
			}
			_logger.EndMethod();
		});

		public TutorialPage4ViewModel(
			INavigationService          navigationService,
			ExposureNotificationService exposureNotificationService,
			ILoggerService              logger,
			IUserDataService            userDataService)
			: base(navigationService, exposureNotificationService)
		{
			_logger            = logger;
			_user_data_service = userDataService;
			_user_data         = userDataService.Get();
		}
	}
}