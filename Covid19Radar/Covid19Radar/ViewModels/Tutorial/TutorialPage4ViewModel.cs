using System;
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
		private readonly ILoggerService              _logger;
		private readonly INavigationService          _ns;
		private readonly ExposureNotificationService _ens;
		private readonly IUserDataService            _user_data_service;
		private readonly UserDataModel               _user_data;

		public Command OnClickEnable => new(async () => {
			_logger.StartMethod();
			await _ens.StartExposureNotification();
			await _ns.NavigateAsync(nameof(TutorialPage6));
			_logger.EndMethod();
		});

		public Command OnClickDisable => new(async () => {
			_logger.StartMethod();
			_user_data.IsExposureNotificationEnabled = false;
			await _user_data_service.SetAsync(_user_data);
			await _ns.NavigateAsync(nameof(TutorialPage6));
			_logger.EndMethod();
		});

		public TutorialPage4ViewModel(
			ILoggerService              logger,
			INavigationService          navigationService,
			ExposureNotificationService exposureNotificationService,
			IUserDataService            userDataService)
		{
			_logger            = logger                      ?? throw new ArgumentNullException(nameof(logger));
			_ns                = navigationService           ?? throw new ArgumentNullException(nameof(navigationService));
			_ens               = exposureNotificationService ?? throw new ArgumentNullException(nameof(exposureNotificationService));
			_user_data_service = userDataService             ?? throw new ArgumentNullException(nameof(userDataService));
			_user_data         = userDataService.Get();
		}
	}
}
