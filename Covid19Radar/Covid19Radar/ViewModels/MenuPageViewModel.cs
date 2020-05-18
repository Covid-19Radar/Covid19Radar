using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using Covid19Radar.Model;
using Covid19Radar.Views;
using Covid19Radar.Resources;

namespace Covid19Radar.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {
        //private INavigationService _navigationService;

        public ObservableCollection<MainMenuModel> MenuItems { get; set; }

        private MainMenuModel selectedMenuItem;
        public MainMenuModel SelectedMenuItem
        {
            get => selectedMenuItem;
            set => SetProperty(ref selectedMenuItem, value);
        }

        public DelegateCommand NavigateCommand { get; private set; }

        public MenuPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            MenuItems = new ObservableCollection<MainMenuModel>
            {
                new MainMenuModel()
                {
                    Icon = "\uf059",
                    PageName = nameof(StartTutorialPage),
                    Title = AppResources.TitleAppDescription
                },
                new MainMenuModel()
                {
                    Icon = "\uf015",
                    PageName = nameof(HomePage),
                    Title = AppResources.ButtonHome
                },
                new MainMenuModel()
                {
                    Icon = "\uf0f3",
                    PageName = nameof(SettingsPage),
                    Title = AppResources.TitleStatusSettings
                },
                new MainMenuModel()
                {
                    Icon = "\uf2f1",
                    PageName = nameof(UpdateInfomationPage),
                    Title = AppResources.TitleUpdateInformation
                },
                new MainMenuModel()
                {
                    Icon = "\uf56c",
                    PageName = nameof(LicenseAgreementPage),
                    Title = AppResources.TitleLicenseAgreement
                },
                new MainMenuModel()
                {
                    Icon = "\uf0c0",
                    PageName = nameof(ContributorsPage),
                    Title = AppResources.MainContributors
                }
            };

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
        }

    }
}
