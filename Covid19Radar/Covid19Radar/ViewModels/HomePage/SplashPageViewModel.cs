using System;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class SplashPageViewModel : ViewModelBase
	{
		private readonly ILoggerService      _logger;
		private readonly INavigationService  _ns;
		private readonly ITermsUpdateService _terms_update;

		public SplashPageViewModel(ILoggerService logger, INavigationService navigationService, ITermsUpdateService termsUpdate)
		{
			_logger       = logger            ?? throw new ArgumentNullException(nameof(logger));
			_ns           = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			_terms_update = termsUpdate       ?? throw new ArgumentNullException(nameof(termsUpdate));
		}

		public override async void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			var termsUpdateInfo = await _terms_update.GetTermsUpdateInfo();
			if (_terms_update.IsReAgree(TermsType.TermsOfService, termsUpdateInfo)) {
				await _ns.NavigateAsync(nameof(ReAgreeTermsOfServicePage), new NavigationParameters() {
					{ "updateInfo", termsUpdateInfo }
				});
			} else if (_terms_update.IsReAgree(TermsType.PrivacyPolicy, termsUpdateInfo)) {
				await _ns.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), new NavigationParameters() {
					{ "updatePrivacyPolicyInfo", termsUpdateInfo.PrivacyPolicy }
				});
			} else {
				await _ns.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
			}
			_logger.EndMethod();
		}
	}
}
