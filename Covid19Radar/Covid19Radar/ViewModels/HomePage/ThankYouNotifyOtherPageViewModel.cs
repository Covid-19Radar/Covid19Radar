using System;
using Covid19Radar.Common;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class ThankYouNotifyOtherPageViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;

		public Command OnClickShareApp => new Command(async () => {
			_logger.StartMethod();
			await AppUtils.PopUpShare();
			_logger.EndMethod();
		});

		public ThankYouNotifyOtherPageViewModel(ILoggerService logger) : base()
		{
			_logger    = logger ?? throw new ArgumentNullException(nameof(logger));
			this.Title = AppResources.TitleUserStatusSettings;
		}
	}
}
