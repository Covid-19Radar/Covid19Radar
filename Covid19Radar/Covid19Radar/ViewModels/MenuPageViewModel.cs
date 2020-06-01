﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using Covid19Radar.Model;
using Covid19Radar.Views;

namespace Covid19Radar.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {
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
            MenuItems = new ObservableCollection<MainMenuModel>();
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf965",
                PageName = nameof(HomePage),
                Title = Resources.AppResources.HomePageTitle
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf4fe",
                PageName = nameof(SettingsPage),
                Title = Resources.AppResources.SettingsPageTitle
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf2f1",
                PageName = nameof(UpdateInformationPage),
                Title = Resources.AppResources.TitleUpdateInformation
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf56c",
                PageName = nameof(LicenseAgreementPage),
                Title = Resources.AppResources.TitleLicenseAgreement
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(ContributorsPage),
                Title = Resources.AppResources.TitleContributorsPage
            });

#if DEBUG
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(DebugPage),
                Title = nameof(DebugPage)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage1),
                Title = nameof(TutorialPage1)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage2),
                Title = nameof(TutorialPage2)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage3),
                Title = nameof(TutorialPage3)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage4),
                Title = nameof(TutorialPage4)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage5),
                Title = nameof(TutorialPage5)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(TutorialPage6),
                Title = nameof(TutorialPage6)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage1),
                Title = nameof(HelpPage1)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage2),
                Title = nameof(HelpPage2)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage3),
                Title = nameof(HelpPage3)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage4),
                Title = nameof(HelpPage4)
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(HelpPage5),
                Title = nameof(HelpPage5)
            });

#endif

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            if (SelectedMenuItem.PageName == nameof(StartTutorialPage))
            {
                await NavigationService.NavigateAsync(nameof(StartTutorialPage), useModalNavigation: true);
                return;
            }
            await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
            return;
        }

    }
}
