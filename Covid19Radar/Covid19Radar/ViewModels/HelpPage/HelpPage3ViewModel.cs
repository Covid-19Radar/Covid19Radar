using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class HelpPage3ViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;

		public Command OnClickNotifyOtherPage => new Command(async () => {
			_logger.StartMethod();
			var task = this.NavigationService?.NavigateAsync(nameof(SubmitConsentPage));
			if (!(task is null)) {
				await task;
			}
			_logger.EndMethod();
		});

		public HelpPage3ViewModel(INavigationService navigationService, ILoggerService logger) : base(navigationService)
		{
			_logger    = logger;
			this.Title = AppResources.HelpPage3Title;
		}
	}
}