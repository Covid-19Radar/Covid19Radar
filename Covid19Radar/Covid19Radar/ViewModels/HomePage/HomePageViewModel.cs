﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private string _exposureCount;
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;


        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }

        public HomePageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = AppResources.HomePageTitle;
            ExposureCount = String.Format("{0}{1}", 1, "件の接触がありました");
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;

            _ = exposureNotificationService.StartExposureNotification();
            userData = this.userDataService.Get();
        }

        public Command OnClickNotifyOther => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(NotifyOtherPage));
        });

        public Command OnClickExposures => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(ExposuresPage));
        });

        public Command OnClickShareApp => new Command(async () =>
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppConstants.AppStoreUrl,
                    Title = AppConstants.AppName
                });
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppConstants.GooglePlayUrl,
                    Title = AppConstants.AppName
                });
            }
        });
    }
}
