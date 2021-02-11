using System;
using System.Text;
using System.Threading.Tasks;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class InqueryPageViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;

		public Func<string, BrowserLaunchMode, Task> OpenBrowserAsync  { get; set; }
		public Func<EmailMessage, Task>              ComposeEmailAsync { get; set; }

		public Command OnClickQuestionCommand => new Command(async () => {
			_logger.StartMethod();
			await this.OpenBrowserAsync(
				"https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/kenkou_iryou/covid19_qa_kanrenkigyou_00009.html",
				BrowserLaunchMode.SystemPreferred
			);
			_logger.EndMethod();
		});

		public Command OnClickSite2 => new Command(async () => {
			_logger.StartMethod();
			await this.OpenBrowserAsync("https://github.com/Covid-19Radar/Covid19Radar", BrowserLaunchMode.SystemPreferred);
			_logger.EndMethod();
		});

		public Command OnClickSendLogCommand => new Command(async () => {
			_logger.StartMethod();
			var task = this.NavigationService?.NavigateAsync(nameof(SendLogConfirmationPage));
			if (!(task is null)) {
				await task;
			}
			_logger.EndMethod();
		});

		public Command OnClickEmailCommand => new Command(async () => {
			try {
				_logger.StartMethod();
				var sb = new StringBuilder();
				sb.Append("DEVICE_INFO : ");
				sb.Append(AppSettings.Instance.AppVersion);
				sb.Append(", ");
				sb.Append(DeviceInfo.Model);
				sb.Append(" (");
				sb.Append(DeviceInfo.Manufacturer);
				sb.Append("), ");
				sb.Append(DeviceInfo.Platform);
				sb.Append(", ");
				sb.AppendLine(DeviceInfo.VersionString);
				sb.Append(AppResources.InquiryMailBody.Replace("\\r\\n", "\r\n"));
				await this.ComposeEmailAsync(new EmailMessage(
					AppResources.InquiryMailSubject,
					sb.ToString(),
					new[] { AppSettings.Instance.SupportEmail }
				));
			} catch (Exception e) {
				_logger.Exception("Failed to send an email.", e);
			} finally {
				_logger.EndMethod();
			}
		});

		public Command OnClickAboutAppCommand => new Command(async () => {
			_logger.StartMethod();
			await this.OpenBrowserAsync("https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/cocoa_00138.html", BrowserLaunchMode.SystemPreferred);
			_logger.EndMethod();
		});

		public InqueryPageViewModel(INavigationService navigationService, ILoggerService logger) : base(navigationService)
		{
			_logger                = logger;
			this.OpenBrowserAsync  = Browser.OpenAsync;
			this.ComposeEmailAsync = Email.ComposeAsync;
		}
	}
}
