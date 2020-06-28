using System.Windows.Input;
using Covid19Radar.Common;
using Covid19Radar.Views;
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

        public ICommand OnClickNotifyOtherPage => new AsyncDelegateCommand(async () =>
        {
            await NavigationService.NavigateAsync(nameof(SubmitConsentPage));
        });

    }
}