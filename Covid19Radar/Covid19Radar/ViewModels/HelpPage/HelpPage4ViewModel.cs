using System;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class HelpPage4ViewModel : ViewModelBase
	{
		private readonly ILoggerService     _logger;
		private readonly INavigationService _ns;

		public Command OnClickNotifyOtherPage => new Command(async () => {
			_logger.StartMethod();
			await _ns.NavigateAsync(nameof(SettingsPage));
			_logger.EndMethod();
		});

		public HelpPage4ViewModel(ILoggerService logger, INavigationService navigationService)
		{
			_logger    = logger            ?? throw new ArgumentNullException(nameof(logger));
			_ns        = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			this.Title = AppResources.HelpPage4Title;
		}
	}
}
