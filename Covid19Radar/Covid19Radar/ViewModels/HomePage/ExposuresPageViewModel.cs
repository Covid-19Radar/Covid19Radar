using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;

namespace Covid19Radar.ViewModels
{
	public class ExposuresPageViewModel : ViewModelBase
	{
		private readonly IUserDataService                      _user_data_service;
		public           ObservableCollection<ExposureSummary> _exposures;

		public ObservableCollection<ExposureSummary> Exposures
		{
			get => _exposures;
			set => this.SetProperty(ref _exposures, value);
		}

		public ExposuresPageViewModel(IUserDataService userDataService)
		{
			_user_data_service = userDataService;
			_exposures         = new ObservableCollection<ExposureSummary>();
			this.Title         = AppResources.MainExposures;

			var userData = _user_data_service.Get();
			foreach (var item in from x in userData.ExposureInformation group x by x.Timestamp) {
				_exposures.Add(new() {
					ExposureDate  = item.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture),
					ExposureCount = item.Count().ToString()
				});
			}
		}
	}

	public class ExposureSummary
	{
		public string? ExposureDate  { get; set; }
		public string? ExposureCount { get; set; }
	}
}
