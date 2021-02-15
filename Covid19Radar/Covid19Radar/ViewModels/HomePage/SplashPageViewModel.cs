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
		private readonly ITermsUpdateService _terms_update;

		public SplashPageViewModel(INavigationService navigationService, ILoggerService logger, ITermsUpdateService termsUpdate) : base(navigationService)
		{
			_logger       = logger;
			_terms_update = termsUpdate;
		}

		public override async void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			var termsUpdateInfo = await _terms_update.GetTermsUpdateInfo();
			Task? task;
			if (_terms_update.IsReAgree(TermsType.TermsOfService, termsUpdateInfo)) {
				task = this.NavigationService?.NavigateAsync(nameof(ReAgreeTermsOfServicePage), new NavigationParameters() {
					{ "updateInfo", termsUpdateInfo }
				});
			} else if (_terms_update.IsReAgree(TermsType.PrivacyPolicy, termsUpdateInfo)) {
				task = this.NavigationService?.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), new NavigationParameters() {
					{ "updatePrivacyPolicyInfo", termsUpdateInfo.PrivacyPolicy }
				});
			} else {
				task = this.NavigationService?.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
			}
			_logger.EndMethod();
		}
	}
}
