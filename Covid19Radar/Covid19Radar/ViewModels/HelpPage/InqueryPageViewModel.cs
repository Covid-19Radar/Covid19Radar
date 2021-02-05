using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ILoggerService loggerService;

        public Func<string, BrowserLaunchMode, Task> BrowserOpenAsync = Browser.OpenAsync;
        public Func<string, string, string[], Task> ComposeEmailAsync { get; set; } = Email.ComposeAsync;

        public InqueryPageViewModel(INavigationService navigationService, ILoggerService loggerService) : base(navigationService)
        {
            this.loggerService = loggerService;
        }

        public Command OnClickQuestionCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            //var uri = "https://github.com/Covid-19Radar/Covid19Radar";
            //await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/kenkou_iryou/covid19_qa_kanrenkigyou_00009.html";
            await BrowserOpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });

        public Command OnClickSite2 => new Command(async () =>
        {
            var uri = "https://github.com/Covid-19Radar/Covid19Radar";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });

        public Command OnClickSendLogCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            //var uri = "https://github.com/Covid-19Radar/Covid19Radar";
            //await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            _ = await NavigationService.NavigateAsync(nameof(SendLogConfirmationPage));

            loggerService.EndMethod();
        });

        public Command OnClickEmailCommand => new Command(async () =>
        {
#if true
            loggerService.StartMethod();

            // Device Model (SMG-950U, iPhone10,6)
            var device = DeviceInfo.Model;

            // Manufacturer (Samsung)
            var manufacturer = DeviceInfo.Manufacturer;

            // Operating System Version Number (7.0)
            var version = DeviceInfo.VersionString;

            // Platform (Android)
            var platform = DeviceInfo.Platform;

            var device_info = "DEVICE_INFO : " + AppSettings.Instance.AppVersion + "," + device + "("+ manufacturer + ")," + platform + "," + version;
            Debug.WriteLine("DEVICE_INFO : " + device_info);

            try
            {
                List<string> recipients = new List<string>();
                recipients.Add(AppSettings.Instance.SupportEmail);
                var message = new EmailMessage
                {
                    Subject = AppResources.InqueryMailSubject,
                    Body = device_info + "\r\n" + AppResources.InqueryMailBody.Replace("\\r\\n", "\r\n"),
                    To = recipients
                };
                await Email.ComposeAsync(message);
            } catch (Exception ex) {
                loggerService.Exception("Exception", ex);
            } finally {
                loggerService.EndMethod();
			}
#else
            // Device Model (SMG-950U, iPhone10,6)
            var device = DeviceInfo.Model;

            // Manufacturer (Samsung)
            var manufacturer = DeviceInfo.Manufacturer;

            // Operating System Version Number (7.0)
            var version = DeviceInfo.VersionString;

            // Platform (Android)
            var platform = DeviceInfo.Platform;

            var device_info = "DEVICE_INFO : " + AppSettings.Instance.AppVersion + "," + device + "("+ manufacturer + ")," + platform + "," + version;
            Debug.WriteLine("DEVICE_INFO : " + device_info);

            try
            {
                List<string> recipients = new List<string>();
                recipients.Add(AppSettings.Instance.SupportEmail);
                var message = new EmailMessage
                {
                    Subject = AppResources.InqueryMailSubject,
                    Body = device_info + "\r\n" + AppResources.InqueryMailBody.Replace("\\r\\n", "\r\n"),
                    To = recipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException)
            {
                // Email is not supported on this device
                // TODO: Add a code to handle the exception
            }
            catch (Exception) {
                // Some other exception occurred
                // TODO: Add a code to handle the exception
            }
#else
            loggerService.StartMethod();
            try
            {
                await ComposeEmailAsync(
                    AppResources.InquiryMailSubject,
                    AppResources.InquiryMailBody.Replace("\\r\\n", "\r\n"),
                    new string[] { AppSettings.Instance.SupportEmail });

                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                loggerService.Exception("Exception", ex);
                loggerService.EndMethod();
            }
#endif
        });

        public Command OnClickAboutAppCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/cocoa_00138.html";
            await BrowserOpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });
    }
}
