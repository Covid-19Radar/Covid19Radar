using System;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class HelpPage2ViewModel : ViewModelBase
	{
		private readonly ILoggerService     _logger;
		private readonly INavigationService _ns;

		public Command OnClickNext => new Command(async () => {
			_logger.StartMethod();
			await _ns.NavigateAsync(nameof(HelpPage4));
			_logger.EndMethod();
		});

		public HelpPage2ViewModel(ILoggerService logger, INavigationService navigationService)
		{
			_logger    = logger            ?? throw new ArgumentNullException(nameof(logger));
			_ns        = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			this.Title = AppResources.HelpPage2Title;
		}
	}
}
