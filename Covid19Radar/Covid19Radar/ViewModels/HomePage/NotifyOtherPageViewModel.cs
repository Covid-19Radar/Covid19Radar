﻿using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using Acr.UserDialogs;
using Covid19Radar.Views;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        public bool IsEnabled { get; set; } = true;
        public string DiagnosisUid { get; set; }

        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public NotifyOtherPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();

        }

        public Command OnClickRegister => (new Command(async () =>
        {
            if (string.IsNullOrEmpty(DiagnosisUid))
            {
                // Check gov's positive api check here!!
                await UserDialogs.Instance.AlertAsync(
                    Resources.AppResources.NotifyOtherPageDialogSubmittedText,
                    Resources.AppResources.ButtonComplete,
                    Resources.AppResources.ButtonOk
                );
                return;
            }



            // Submit the UID
            using var dialog = UserDialogs.Instance.Loading(Resources.AppResources.LoadingTextSubmittingDiagnosis);
            IsEnabled = false;
            try
            {
                // EN Enabled Check
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();

                if (!enabled)
                {
                    dialog.Hide();
                    await UserDialogs.Instance.AlertAsync(
                        Resources.AppResources.NotifyOtherPageDialogSubmittedText,
                        Resources.AppResources.ButtonComplete,
                        Resources.AppResources.ButtonOk
                    );
                    await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
                    return;
                }

                // Set the submitted UID
                userData.AddDiagnosis(DiagnosisUid, new DateTimeOffset(DateTime.Now));
                await userDataService.SetAsync(userData);

                // Submit our diagnosis
                await Xamarin.ExposureNotifications.ExposureNotification.SubmitSelfDiagnosisAsync();
                dialog.Hide();

                await UserDialogs.Instance.AlertAsync(
                    Resources.AppResources.NotifyOtherPageDialogSubmittedText,
                    Resources.AppResources.ButtonComplete,
                    Resources.AppResources.ButtonOk
                );

                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                dialog.Hide();
                UserDialogs.Instance.Alert(
                    Resources.AppResources.NotifyOtherPageDialogExceptionText,
                    Resources.AppResources.ButtonFailed,
                    Resources.AppResources.ButtonOk
                );
            }
            finally
            {
                IsEnabled = true;
            }


        }));

        public Command OnClickAfter => (new Command(async () =>
        {
            var check = await UserDialogs.Instance.ConfirmAsync(
                Resources.AppResources.PositiveRegistrationConfirmText,
                Resources.AppResources.PositiveRegistrationText,
                Resources.AppResources.ButtonNotNow,
                Resources.AppResources.ButtonReturnToRegistration
            );
            if (check)
            {
                await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(HomePage));
            }

        }));
    }
}
