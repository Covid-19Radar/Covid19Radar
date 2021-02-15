using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class TutorialPage6ViewModel : ViewModelBase
	{
		private readonly ILoggerService   _logger;
		private readonly IUserDataService _user_data_service;
		private          UserDataModel?   _user_data;

		public TutorialPage6ViewModel(INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService) : base(navigationService)
		{
			_logger            = loggerService;
			_user_data_service = userDataService;
			_user_data         = userDataService.Get();
		}

		public override async void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			if (_user_data is null) {
				_logger.Warning("The user data is null.");
			} else {
				_user_data.IsPolicyAccepted = true;
				await _user_data_service.SetAsync(_user_data);
				_logger.Info($"Is the policy accepted? {_user_data.IsPolicyAccepted}");
			}
			_logger.EndMethod();
		}
	}
}
