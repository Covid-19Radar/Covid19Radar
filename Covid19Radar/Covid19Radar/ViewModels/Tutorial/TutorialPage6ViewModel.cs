﻿using Covid19Radar.Model;
using Prism.Navigation;
using Covid19Radar.Services;
using System.Threading.Tasks;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage6ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public TutorialPage6ViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            await TutorialCompleteAsync();
            base.Initialize(parameters);
        }
        private async Task TutorialCompleteAsync()
        {
            userData.IsPolicyAccepted = true;
            await userDataService.SetAsync(userData);
        }
    }
}