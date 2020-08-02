using Covid19Radar.Resources;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class WebViewerPageViewModel : ViewModelBase
	{
		public string Url => NewsPageViewModel.Url;

		public WebViewerPageViewModel(INavigationService navigationService) : base(navigationService)
		{
			this.Title = AppResources.NewsPageTitle;
		}
	}
}
