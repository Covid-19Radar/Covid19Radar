using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Covid19Radar.ViewModels
{
	public class ExposuresPageViewModel : ViewModelBase
	{
		private readonly IUserDataService                      _user_data_service;
		private readonly UserDataModel?                        _user_data;
		public           ObservableCollection<ExposureSummary> _exposures;

		public ObservableCollection<ExposureSummary> Exposures
		{
			get => _exposures;
			set => this.SetProperty(ref _exposures, value);
		}

		public ExposuresPageViewModel(INavigationService navigationService, IUserDataService userDataService) : base(navigationService)
		{
			_user_data_service = userDataService;
			_user_data         = _user_data_service.Get();
			_exposures         = new ObservableCollection<ExposureSummary>();
			this.Title         = AppResources.MainExposures;

			if (!(_user_data is null)) {
				foreach (var item in from x in _user_data.ExposureInformation group x by x.Timestamp) {
					_exposures.Add(new ExposureSummary() {
						ExposureDate  = item.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture),
						ExposureCount = item.Count().ToString()
					});
				}
			}
		}
	}

	public class ExposureSummary
	{
		public string? ExposureDate  { get; set; }
		public string? ExposureCount { get; set; }
	}
}
