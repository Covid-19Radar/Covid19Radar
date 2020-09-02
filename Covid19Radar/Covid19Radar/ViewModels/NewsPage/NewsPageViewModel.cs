using Covid19Radar.Resources;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class NewsPageViewModel : ViewModelBase
	{
		public static string Url     { get; set; }
		public static bool   GSearch { get; set; }

		private readonly INavigationService _navigationService;

		public Command OnClick_ShowGoogle => new Command(() => {
			if (GSearch) {
				GSearch = false;
			} else {
				Url = AppResources.GoogleSearchUrl;
			}
			_navigationService.NavigateAsync(nameof(WebViewerPage));
		});

		public Command OnClick_ShowCoronaGoJP => new Command(() => {
			Url = AppResources.CoronaGoJPUrl;
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
