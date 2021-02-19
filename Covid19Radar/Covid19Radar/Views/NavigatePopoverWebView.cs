using Xamarin.Forms;
using Xamarin.Essentials;

namespace Covid19Radar.Views
{
	public class NavigatePopoverWebView : WebView
	{
		private string? _first_url = null;

		public NavigatePopoverWebView()
		{
			this.Navigating += async (_, e) =>
			{
				if (_first_url != null && _first_url != e.Url) {
					e.Cancel = true;
					await Browser.OpenAsync(e.Url);
				}
			};
		}

		protected override void OnPropertyChanged(string propertyName)
		{
			if (propertyName == nameof(this.Source) && _first_url == null && this.Source is UrlWebViewSource urlSource) {
				_first_url = urlSource.Url;
			}
			base.OnPropertyChanged(propertyName);
		}
	}
}
