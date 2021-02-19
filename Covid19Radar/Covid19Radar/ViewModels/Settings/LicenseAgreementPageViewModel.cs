using Covid19Radar.Resources;

namespace Covid19Radar.ViewModels
{
	public class LicenseAgreementPageViewModel : ViewModelBase
	{
		private string _url;

		public string Url
		{
			get => _url;
			set => this.SetProperty(ref _url, value);
		}

		public LicenseAgreementPageViewModel()
		{
			this.Title = AppResources.TitleLicenseAgreement;
			_url       = AppSettings.Instance.LicenseUrl;
			this.RaisePropertyChanged(nameof(this.Url));
		}
	}
}
