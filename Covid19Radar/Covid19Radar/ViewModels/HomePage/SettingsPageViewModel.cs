﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private string _AppVersion;

        public string AppVer
        {
            get { return _AppVersion; }
            set { SetProperty(ref _AppVersion, value); }
        }

        private bool _EnableExposureNotification;

        public bool EnableExposureNotification
        {
            get { return _EnableExposureNotification; }
            set
            {
                SetProperty(ref _EnableExposureNotification, value);
                RaisePropertyChanged(nameof(EnableExposureNotification));
            }
        }

        private bool _EnableLocalNotification;

        public bool EnableLocalNotification
        {
            get { return _EnableLocalNotification; }
            set
            {
                SetProperty(ref _EnableLocalNotification, value);
                RaisePropertyChanged(nameof(EnableLocalNotification));
            }
        }

        private bool _ResetData;

        public bool ResetData
        {
            get { return _ResetData; }
            set
            {
                SetProperty(ref _ResetData, value);
                RaisePropertyChanged(nameof(ResetData));
            }
        }

        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public SettingsPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = AppResources.SettingsPageTitle;
            AppVer = AppConstants.AppVersion;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            this.userDataService.UserDataChanged += _userDataChanged;

            EnableExposureNotification = userData.IsExposureNotificationEnabled;
            EnableLocalNotification = userData.IsNotificationEnabled;
        }

        private void _userDataChanged(object sender, UserDataModel e)
        {
            userData = this.userDataService.Get();
            EnableExposureNotification = userData.IsExposureNotificationEnabled;
            EnableLocalNotification = userData.IsNotificationEnabled;
        }

        public ICommand OnChangeEnableExposureNotification => new Command(async () =>
        {
            userData.IsExposureNotificationEnabled = !EnableExposureNotification;
            await userDataService.SetAsync(userData);
        });

        public ICommand OnChangeEnableNotification => new Command(async () =>
        {
            userData.IsNotificationEnabled = !EnableLocalNotification;
            await userDataService.SetAsync(userData);
        });

        public ICommand OnChangeResetData => new Command(async () =>
        {

            var check = await UserDialogs.Instance.ConfirmAsync(
                Resources.AppResources.SettingsPageDialogResetText,
                Resources.AppResources.SettingsPageDialogResetTitle,
                Resources.AppResources.ButtonOk,
                Resources.AppResources.ButtonCancel
            );

            if (check)
            {
                UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextDeleting);

                if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();
                }

                // Reset All Data and Optout
                UserDataModel userData = new UserDataModel();
                await userDataService.SetAsync(userData);

                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync("全設定とデータを削除しました。アプリの再起動をしてください。");
                Application.Current.Quit();

                // Application close
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().closeApplication();
                return;

            }
        });
    }
}
