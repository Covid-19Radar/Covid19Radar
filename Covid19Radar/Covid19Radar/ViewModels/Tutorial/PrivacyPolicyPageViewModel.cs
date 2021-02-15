using System;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class PrivacyPolicyPageViewModel : ViewModelBase
	{
		private readonly ILoggerService      _logger;
		private readonly ITermsUpdateService _terms_update;
		private readonly IUserDataService    _user_data_service;
		private          UserDataModel?      _user_data;
		private          string              _url;

		public string Url
		{
			get => _url;
			set => this.SetProperty(ref _url, value);
		}

		public Command OnClickAgree => new Command(async () => {
			_logger.StartMethod();
			if (_user_data is null) {
				_logger.Warning("The user data is null.");
			} else {
				_user_data.IsPolicyAccepted = true;
				await _user_data_service.SetAsync(_user_data);
				_logger.Info($"Is the policy accepted? {_user_data.IsPolicyAccepted}");
				await _terms_update.SaveLastUpdateDateAsync(TermsType.PrivacyPolicy, DateTime.Now);
				var task = this.NavigationService?.NavigateAsync(nameof(TutorialPage4));
				if (!(task is null)) {
					await task;
				}
			}
			_logger.EndMethod();
		});

		public PrivacyPolicyPageViewModel(
			INavigationService  navigationService,
			ILoggerService      logger,
			IUserDataService    userDataService,
			ITermsUpdateService termsUpdate)
			: base(navigationService)
		{
			_logger            = logger;
			_terms_update      = termsUpdate;
			_user_data_service = userDataService;
			_user_data         = userDataService.Get();
			_url               = AppResources.UrlPrivacyPolicy;
			this.Title         = AppResources.PrivacyPolicyPageTitle;
			this.RaisePropertyChanged(nameof(this.Url));
		}
	}
}
