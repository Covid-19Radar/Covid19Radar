using System.Windows.Input;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
//using Plugin.LocalNotification;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage5ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public TutorialPage5ViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            this.userDataService = userDataService;
            userData = this.userDataService.Get();

        }

        public ICommand OnClickEnable => new AsyncDelegateCommand(async () =>
        {
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = AppResources.LocalNotificationPermittedTitle,
                Description = AppResources.LocalNotificationPermittedDescription,
                Android =
                {
                    IconName = "logo_notification"
                }
            };
            NotificationCenter.Current.Show(notification);
            */
            //var notification = new NotificationRequest
            //{
            //    NotificationId = 100,
            //    Title = AppResources.LocalNotificationPermittedTitle,
            //    Description = AppResources.LocalNotificationPermittedDescription
            //};
            //NotificationCenter.Current.Show(notification);
            userData.IsNotificationEnabled = true;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
        });
        public ICommand OnClickDisable => new AsyncDelegateCommand(async () =>
        {
            userData.IsNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
        });

    }
}