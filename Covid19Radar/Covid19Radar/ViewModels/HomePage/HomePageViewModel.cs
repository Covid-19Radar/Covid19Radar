using System;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class HomePageViewModel : ViewModelBase
	{
		private readonly ILoggerService   _logger;
		private readonly IUserDataService _user_data_service;
		private          UserDataModel?   _user_data;
		private          string?          _start_date;
		private          string?          _past_date;

		public string? StartDate
		{
			get => _start_date;
			set => this.SetProperty(ref _start_date, value);
		}

		public string? PastDate
		{
			get => _past_date;
			set => this.SetProperty(ref _past_date, value);
		}

		public Command OnClickExposures => new Command(async () => {
			_logger.StartMethod();
			if (this.ExposureNotificationService is null) {
				_logger.Warning("Could not access to the exposure notification service.");
				_logger.EndMethod();
				return;
			}
			if (this.NavigationService is null) {
				_logger.Warning("Could not access to the navigation service.");
				_logger.EndMethod();
				return;
			}
			var count = this.ExposureNotificationService.GetExposureCount();
			_logger.Info($"The exposure count: {count}");
			if (count > 0) {
				await this.NavigationService.NavigateAsync(nameof(ContactedNotifyPage));
			} else {
				await this.NavigationService.NavigateAsync(nameof(NotContactPage));
			}
			_logger.EndMethod();
		});

		public Command OnClickShareApp => new Command(async () => {
			_logger.StartMethod();
			await AppUtils.PopUpShare();
			_logger.EndMethod();
		});

		public HomePageViewModel(
			INavigationService          navigationService,
			ExposureNotificationService exposureNotificationService,
			ILoggerService              logger,
			IUserDataService            userDataService)
			: base(navigationService, exposureNotificationService)
		{
			_logger            = logger;
			_user_data_service = userDataService;
			_user_data         = userDataService.Get();
			this.Title         = AppResources.HomePageTitle;

			if (_user_data is null) {
				_logger.Warning("The user data was null.");
			} else {
				this.StartDate = _user_data.GetLocalDateString();
				this.PastDate  = (DateTime.UtcNow - _user_data.StartDateTime).Days.ToString();
			}
		}

		public override async void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
#if !DEBUG
			await AppUtils.CheckVersionAsync(_logger);
#endif
			if (this.ExposureNotificationService is null) {
				_logger.Warning("Could not access to the exposure notification service.");
				_logger.EndMethod();
				return;
			}
			try {
				await this.ExposureNotificationService.StartExposureNotification();
				await this.ExposureNotificationService.FetchExposureKeyAsync();
				_logger.Info($"The exposure notification status: {await this.ExposureNotificationService.UpdateStatusMessageAsync()}");
				base.Initialize(parameters);
			} catch (Exception e) {
				_logger.Error("Could not get an exposure notification status.");
				_logger.Exception("Failed to initialize.", e);
			} finally {
				_logger.EndMethod();
			}
		}
	}
}
