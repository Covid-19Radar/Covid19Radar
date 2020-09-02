using System.Windows.Input;
using Covid19Radar.Common;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpPage4ViewModel : ViewModelBase
    {
        public HelpPage4ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage4Title;
        }

        public ICommand OnClickSetting => new AsyncDelegateCommand(async () =>
        {
            await NavigationService.NavigateAsync(nameof(SettingsPage));
        });
    }
}