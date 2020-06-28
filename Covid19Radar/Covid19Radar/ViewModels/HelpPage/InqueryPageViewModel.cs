using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Covid19Radar.Common;
using Covid19Radar.Resources;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InqueryPageViewModel : ViewModelBase
    {

        public InqueryPageViewModel() : base()
        {
        }

        public ICommand OnClickSite1 => new AsyncDelegateCommand(async () =>
        {
            var uri = "https://corona.go.jp/";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            await Task.Delay(TimeSpan.FromMilliseconds(300));
        });

        public ICommand OnClickSite2 => new AsyncDelegateCommand(async () =>
        {
            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/cocoa_00138.html";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            await Task.Delay(TimeSpan.FromMilliseconds(300));
        });

        public ICommand OnClickSite3 => new AsyncDelegateCommand(async () =>
        {
            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/kenkou_iryou/covid19_qa_kanrenkigyou_00009.html";
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            await Task.Delay(TimeSpan.FromMilliseconds(300));
        });

        public ICommand OnClickEmail => new AsyncDelegateCommand(async () =>
        {

            try
            {
                List<string> recipients = new List<string>();
                recipients.Add(AppSettings.Instance.SupportEmail);
                var message = new EmailMessage
                {
                    Subject = AppResources.InqueryMailSubject,
                    Body = AppResources.InqueryMailBody.Replace("\\r\\n", "\r\n"),
                    To = recipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        });


    }
}
