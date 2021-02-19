using Covid19Radar.Resources;

namespace Covid19Radar.ViewModels
{
	public class PrivacyPolicyPage2ViewModel : ViewModelBase
	{
		private string _url;

		public string Url
		{
			get => _url;
			set => this.SetProperty(ref _url, value);
		}

		public PrivacyPolicyPage2ViewModel()
		{
			_url       = AppResources.UrlPrivacyPolicy;
			this.Title = AppResources.PrivacyPolicyPageTitle;
			this.RaisePropertyChanged(nameof(this.Url));
		}
	}
}
