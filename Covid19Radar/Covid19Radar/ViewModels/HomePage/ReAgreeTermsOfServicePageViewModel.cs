using System;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class ReAgreeTermsOfServicePageViewModel : ViewModelBase
	{
		private readonly ILoggerService        _logger;
		private readonly ITermsUpdateService   _terms_update;
		private          TermsUpdateInfoModel? _update_info;
		private          DateTime              _update_dt;
		private          string?               _update_text;

		public string? UpdateText
		{
			get => _update_text;
			set => this.SetProperty(ref _update_text, value);
		}

		public Func<string, BrowserLaunchMode, Task> OpenBrowserAsync { get; set; }

		public Command OpenWebView => new Command(async () => {
			_logger.StartMethod();
			await this.OpenBrowserAsync(AppResources.UrlTermOfUse, BrowserLaunchMode.SystemPreferred);
			_logger.EndMethod();
		});

		public Command OnClickReAgreeCommand => new Command(async () => {
			_logger.StartMethod();
			if (_update_info is null) {
				_logger.Warning("The view model is not initialized yet.");
			} else {
				await _terms_update.SaveLastUpdateDateAsync(TermsType.TermsOfService, _update_dt);
				Task? task;
				if (_terms_update.IsReAgree(TermsType.PrivacyPolicy, _update_info)) {
					task = this.NavigationService?.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), new NavigationParameters() {
						{ "updatePrivacyPolicyInfo", _update_info.PrivacyPolicy }
					});
				} else {
					task = this.NavigationService?.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
				}
				if (!(task is null)) {
					await task;
				}
			}
			_logger.EndMethod();
		});

		public ReAgreeTermsOfServicePageViewModel(INavigationService navigationService, ILoggerService logger, ITermsUpdateService termsUpdate) : base(navigationService)
		{
			_logger               = logger;
			_terms_update         = termsUpdate;
			this.OpenBrowserAsync = Browser.OpenAsync;
		}

		public override void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			_update_info    = ((TermsUpdateInfoModel)(parameters["updateInfo"]));
			_update_dt      = _update_info.TermsOfService?.UpdateDateTime ?? default;
			this.UpdateText = _update_info.TermsOfService?.Text;
			_logger.EndMethod();
		}
	}
}
