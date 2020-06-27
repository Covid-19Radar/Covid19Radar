using Covid19Radar.Common;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class NotContactPageViewModel : ViewModelBase
    {
        public NotContactPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = Resources.AppResources.TitleUserStatusSettings;
        }
        public Command OnClickShareApp => new Command(() =>
        {
            AppUtils.PopUpShare();
        });

    }
}
