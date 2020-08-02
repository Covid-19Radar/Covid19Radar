using Covid19Radar.Resources;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class NewsPageViewModel : ViewModelBase
	{
		public static string Url { get; private set; }

		private readonly INavigationService _navigationService;

		public Command OnClick_ShowPage => new Command(() => {
			Url = AppResources.NewsPageUrl;
			_navigationService.NavigateAsync(nameof(WebViewerPage));
		});

		public Command OnClick_ShowStopCOVID19JP => new Command(() => {
			Url = AppResources.StopCOVID19JPUrl;
			_navigationService.NavigateAsync(nameof(WebViewerPage));
		});

		public NewsPageViewModel(INavigationService navigationService) : base(navigationService)
		{
			_navigationService = navigationService;
			this.Title = AppResources.NewsPageTitle;
		}
	}
}
