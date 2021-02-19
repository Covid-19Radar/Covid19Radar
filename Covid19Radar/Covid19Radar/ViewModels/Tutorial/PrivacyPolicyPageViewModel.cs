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
		private readonly INavigationService  _ns;
		private readonly ITermsUpdateService _terms_update;
		private readonly IUserDataService    _user_data_service;
		private readonly UserDataModel       _user_data;
		private          string              _url;

		public string Url
		{
			get => _url;
			set => this.SetProperty(ref _url, value);
		}

		public Command OnClickAgree => new(async () => {
			_logger.StartMethod();
			_user_data.IsPolicyAccepted = true;
			await _user_data_service.SetAsync(_user_data);
			_logger.Info($"Is the policy accepted? {_user_data.IsPolicyAccepted}");
			await _terms_update.SaveLastUpdateDateAsync(TermsType.PrivacyPolicy, DateTime.Now);
			await _ns.NavigateAsync(nameof(TutorialPage4));
			_logger.EndMethod();
		});

		public PrivacyPolicyPageViewModel(
			ILoggerService      logger,
			INavigationService  navigationService,
			ITermsUpdateService termsUpdate,
			IUserDataService    userDataService)
		{
			_logger            = logger            ?? throw new ArgumentNullException(nameof(logger));
			_ns                = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			_terms_update      = termsUpdate       ?? throw new ArgumentNullException(nameof(termsUpdate));
			_user_data_service = userDataService   ?? throw new ArgumentNullException(nameof(userDataService));
			_user_data         = userDataService.Get();
			_url               = AppResources.UrlPrivacyPolicy;
			this.Title         = AppResources.PrivacyPolicyPageTitle;
			this.RaisePropertyChanged(nameof(this.Url));
		}
	}
}
