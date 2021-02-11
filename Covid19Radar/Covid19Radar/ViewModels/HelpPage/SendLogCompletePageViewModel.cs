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
		private readonly ILoggerService _logger;
		private          string?        _log_id;

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

		public SendLogCompletePageViewModel(INavigationService navigationService, ILoggerService logger) : base(navigationService)
		{
			_logger = logger;
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
			var task = this.NavigationService?.NavigateAsync($"/{nameof(MenuPage)}/{nameof(NavigationPage)}/{nameof(HomePage)}");
			if (!(task is null)) {
				await task;
			}
			_logger.EndMethod();
		}
	}
}
