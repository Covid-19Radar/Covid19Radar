using System;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class TutorialPage3ViewModel : ViewModelBase
	{
		private readonly ILoggerService      _logger;
		private readonly IUserDataService    _user_data_service;
		private          UserDataModel?      _user_data;
		private          ITermsUpdateService _terms_update;
		private          string?             _url;

		public string? Url
		{
			get => _url;
			set => this.SetProperty(ref _url, value);
		}

		public Command OnClickAgree => new Command(async () => {
			_logger.StartMethod();
			UserDialogs.Instance.ShowLoading(AppResources.LoadingTextRegistering);
			if (_user_data is null) {
				_logger.Warning("The user data is null. Registering user...");
				_user_data = await _user_data_service.RegisterUserAsync();
				if (_user_data is null) {
					_logger.Warning("The user data is still null.");
					UserDialogs.Instance.HideLoading();
					await UserDialogs.Instance.AlertAsync(
						AppResources.DialogNetworkConnectionError,
						AppResources.DialogNetworkConnectionErrorTitle,
						AppResources.ButtonOk
					);
					_logger.EndMethod();
					return;
				}
			}
			_logger.Info("The user data is not null.");
			_user_data.IsOptined = true;
			await _user_data_service.SetAsync(_user_data);
			_logger.Info($"The user data property \'{nameof(_user_data.IsOptined)}\' is set to \'{_user_data.IsOptined}\'.");
			await _terms_update.SaveLastUpdateDateAsync(TermsType.TermsOfService, DateTime.Now);
			UserDialogs.Instance.HideLoading();
			if (this.NavigationService is null) {
				_logger.Warning("The navigation service is null.");
			} else {
				await this.NavigationService.NavigateAsync(nameof(PrivacyPolicyPage));
			}
			_logger.EndMethod();
		});

		public TutorialPage3ViewModel(
			INavigationService  navigationService,
			ILoggerService      logger,
			IUserDataService    userDataService,
			ITermsUpdateService termsUpdate)
			: base(navigationService)
		{
			_logger            = logger;
			_user_data_service = userDataService;
			_user_data         = userDataService.Get();
			_terms_update      = termsUpdate;
		}
	}
}
