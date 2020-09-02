using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;
using Covid19Radar.Resources;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;
using ImTools;
using Acr.UserDialogs;
using System.Diagnostics;
using Covid19Radar.Common;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        private string _exposureCount;
        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }

        private readonly ExposureNotificationService exposureNotificationService;


        public ContactedNotifyPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = Resources.AppResources.TitleUserStatusSettings;
            this.exposureNotificationService = exposureNotificationService;
            ExposureCount = exposureNotificationService.GetExposureCount().ToString();
        }
        public ICommand OnClickByForm => new AsyncDelegateCommand(async () =>
        {
            var uri = AppResources.UrlContactedForm;
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            await Task.Delay(TimeSpan.FromMilliseconds(300));
        });
        public ICommand OnClickByPhone => new AsyncDelegateCommand(async () =>
        {
            var uri = AppResources.UrlContactedPhone;
            using (var client = new HttpClient())
            {
                UserDialogs.Instance.ShowLoading();
                try
                {
                    var json = await client.GetStringAsync(uri);
                    var phoneNumber = JObject.Parse(json).Value<string>("phone");
                    Debug.WriteLine($"Contacted phone call number = {phoneNumber}");
                    PhoneDialer.Open(phoneNumber);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
        });

    }
}
