using System.Windows.Input;
using Covid19Radar.Common;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpPage2ViewModel : ViewModelBase
    {
        public HelpPage2ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage2Title;
        }

        public ICommand OnClickNext => new AsyncDelegateCommand(async () =>
        {
            await NavigationService.NavigateAsync(nameof(HelpPage4));
        });
    }
}