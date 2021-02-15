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
		private readonly ILoggerService   _logger;
		private readonly IUserDataService _user_data_service;
		private          UserDataModel?   _user_data;

		public Command OnClickEnable  => new Command(async () => await this.SetEnabledAndNavigate(true));
		public Command OnClickDisable => new Command(async () => await this.SetEnabledAndNavigate(false));

		public TutorialPage5ViewModel(INavigationService navigationService, ILoggerService logger, IUserDataService userDataService) : base(navigationService)
		{
			_logger            = logger;
			_user_data_service = userDataService;
			_user_data         = _user_data_service.Get();
		}

		private async ValueTask SetEnabledAndNavigate(bool enabled)
		{
			_logger.StartMethod();
			_logger.Info($"The user selected the notification is {(enabled ? "enabled" : "disabled")}.");
			if (_user_data is null) {
				_logger.Warning("The user data is null.");
			} else {
				_user_data.IsNotificationEnabled = enabled;
				await _user_data_service.SetAsync(_user_data);
				if (this.NavigationService is null) {
					_logger.Warning("The navigation service is null.");
				} else {
					await this.NavigationService.NavigateAsync(nameof(TutorialPage6));
				}
			}
			_logger.EndMethod();
		}
	}
}
