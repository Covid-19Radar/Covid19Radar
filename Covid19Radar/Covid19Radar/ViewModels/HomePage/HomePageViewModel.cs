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
		private readonly ILoggerService              _logger;
		private readonly INavigationService          _ns;
		private readonly ExposureNotificationService _ens;
		private readonly IUserDataService            _user_data_service;
		private readonly UserDataModel               _user_data;
		private          string?                     _start_date;
		private          string?                     _past_date;

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

		public Command OnClickExposures => new(async () => {
			_logger.StartMethod();
			int count = _ens.GetExposureCount();
			_logger.Info($"The exposure count: {count}");
			if (count > 0) {
				await _ns.NavigateAsync(nameof(ContactedNotifyPage));
			} else {
				await _ns.NavigateAsync(nameof(NotContactPage));
			}
			_logger.EndMethod();
		});

		public Command OnClickShareApp => new(async () => {
			_logger.StartMethod();
			await AppUtils.PopUpShare();
			_logger.EndMethod();
		});

		public HomePageViewModel(
			ILoggerService              logger,
			INavigationService          navigationService,
			ExposureNotificationService exposureNotificationService,
			IUserDataService            userDataService)
		{
			_logger            = logger                      ?? throw new ArgumentNullException(nameof(logger));
			_ns                = navigationService           ?? throw new ArgumentNullException(nameof(navigationService));
			_ens               = exposureNotificationService ?? throw new ArgumentNullException(nameof(exposureNotificationService));
			_user_data_service = userDataService             ?? throw new ArgumentNullException(nameof(userDataService));
			_user_data         = userDataService.Get();
			this.Title         = AppResources.HomePageTitle;
			this.StartDate     = _user_data.GetLocalDateString();
			this.PastDate      = (DateTime.UtcNow - _user_data.StartDateTime).Days.ToString();
		}

		public override async void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
#if !DEBUG
			await AppUtils.CheckVersionAsync(_logger);
#endif
			try {
				await _ens.StartExposureNotification();
				await _ens.FetchExposureKeyAsync();
				_logger.Info($"The exposure notification status: {await _ens.UpdateStatusMessageAsync()}");
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
