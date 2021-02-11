using Covid19Radar.Resources;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class HelpPage1ViewModel : ViewModelBase
	{
		public HelpPage1ViewModel(INavigationService navigationService) : base(navigationService)
		{
			this.Title = AppResources.HelpPage1Title;
		}
	}
}
