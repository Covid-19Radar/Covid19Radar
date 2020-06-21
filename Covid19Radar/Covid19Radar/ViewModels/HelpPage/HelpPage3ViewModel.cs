﻿using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpPage3ViewModel : ViewModelBase
    {
        public HelpPage3ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage3Title;
        }

        public Command OnClickNotifyOtherPage => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(SubmitConsentPage));
        });

    }
}