using System;
using System.Threading;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage4ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;
        private AsyncDelegateCommand onClickEnable;
        private AsyncDelegateCommand onClickDisable;

        private bool isRunning;
        private bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                onClickEnable?.RaiseCanExecuteChanged();
                onClickDisable?.RaiseCanExecuteChanged();
            }
        }

        public TutorialPage4ViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
            userData = this.userDataService.Get();
        }

        public ICommand OnClickEnable => onClickEnable ??= new AsyncDelegateCommand(async () =>
        {
            IsRunning = true;
            await ExposureNotificationService.StartExposureNotification();
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
            IsRunning = false;
        }, () => !IsRunning);
        public ICommand OnClickDisable => onClickDisable ??= new AsyncDelegateCommand(async () =>
        {
            IsRunning = true;
            userData.IsExposureNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
            IsRunning = false;
        }, () => !IsRunning);
    }
}