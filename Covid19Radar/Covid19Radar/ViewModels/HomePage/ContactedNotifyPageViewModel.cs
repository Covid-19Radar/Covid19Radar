using System;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class ContactedNotifyPageViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;
		private          string         _exposure_count;

		public string ExposureCount
		{
			get => _exposure_count;
			set => this.SetProperty(ref _exposure_count, value ?? string.Empty);
		}

		public Command OnClickByForm => new(async () => {
			_logger.StartMethod();
			await Browser.OpenAsync(AppResources.UrlContactedForm, BrowserLaunchMode.SystemPreferred);
			_logger.EndMethod();
		});

		public ContactedNotifyPageViewModel(ILoggerService logger, ExposureNotificationService exposureNotificationService)
		{
			_logger            =  logger                      ?? throw new ArgumentNullException(nameof(logger));
			_exposure_count    = (exposureNotificationService ?? throw new ArgumentNullException(nameof(exposureNotificationService)))
			                   .GetExposureCount().ToString();
			this.Title         = AppResources.TitleUserStatusSettings;
			this.RaisePropertyChanged(nameof(this.ExposureCount));
		}
	}
}
