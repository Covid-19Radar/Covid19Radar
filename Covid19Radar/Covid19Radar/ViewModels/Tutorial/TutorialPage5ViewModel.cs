using System;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class TutorialPage5ViewModel : ViewModelBase
	{
		private readonly ILoggerService     _logger;
		private readonly INavigationService _ns;
		private readonly IUserDataService   _user_data_service;
		private readonly UserDataModel      _user_data;

		public Command OnClickEnable  => new(async () => await this.SetEnabledAndNavigate(true));
		public Command OnClickDisable => new(async () => await this.SetEnabledAndNavigate(false));

		public TutorialPage5ViewModel(ILoggerService logger, INavigationService navigationService, IUserDataService userDataService)
		{
			_logger            = logger            ?? throw new ArgumentNullException(nameof(logger));
			_ns                = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			_user_data_service = userDataService   ?? throw new ArgumentNullException(nameof(userDataService));
			_user_data         = userDataService.Get();
		}

		private async ValueTask SetEnabledAndNavigate(bool enabled)
		{
			_logger.StartMethod();
			_logger.Info($"The user selected the notification is {(enabled ? "enabled" : "disabled")}.");
			_user_data.IsNotificationEnabled = enabled;
			await _user_data_service.SetAsync(_user_data);
			await _ns.NavigateAsync(nameof(TutorialPage6));
			_logger.EndMethod();
		}
	}
}
