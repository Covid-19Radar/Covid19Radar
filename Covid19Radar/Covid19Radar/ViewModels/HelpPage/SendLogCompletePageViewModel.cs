using System;
using System.Threading.Tasks;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class SendLogCompletePageViewModel : ViewModelBase
	{
		private readonly ILoggerService     _logger;
		private readonly INavigationService _ns;
		private          string?            _log_id;

		public Func<EmailMessage, Task> ComposeEmailAsync { get; set; } = Email.ComposeAsync;

		public Command OnClickSendMailCommand => new Command(async () => {
			_logger.StartMethod();
			try {
				await this.ComposeEmailAsync(new EmailMessage(
					AppResources.SendIdMailSubject,
					AppResources.SendIdMailBody1 + _log_id + AppResources.SendIdMailBody2.Replace("\\r\\n", "\r\n"),
					new string[] { AppSettings.Instance.SupportEmail }
				));
			} catch (Exception e) {
				_logger.Exception("Failed to send an email.", e);
			} finally {
				_logger.EndMethod();
			}
		});

		public Command OnClickHomeCommand         => new Command(this.NavigateToHomeAsync);
		public Command OnBackButtonPressedCommand => new Command(this.NavigateToHomeAsync);

		public SendLogCompletePageViewModel(ILoggerService logger, INavigationService navigationService)
		{
			_logger    = logger            ?? throw new ArgumentNullException(nameof(logger));
			_ns        = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
		}

		public override void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			_log_id = parameters["logId"] as string;
			_logger.EndMethod();
		}

		private async void NavigateToHomeAsync()
		{
			_logger.StartMethod();
			await _ns.NavigateAsync($"/{nameof(MenuPage)}/{nameof(NavigationPage)}/{nameof(HomePage)}");
			_logger.EndMethod();
		}
	}
}
