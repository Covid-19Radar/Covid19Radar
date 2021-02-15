using Covid19Radar.Services;
using Prism.Navigation;
using System;
using System.Linq;
using Xamarin.Forms;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Xamarin.ExposureNotifications;
using System.Threading.Tasks;

namespace Covid19Radar.ViewModels
{
	public class DebugPageViewModel : ViewModelBase
	{
		private readonly IUserDataService _user_data_service;
		private          UserDataModel?   _user_data;
		private          string           _en_message;

		public UserDataModel? UserData
		{
			get => _user_data;
			set => this.SetProperty(ref _user_data, value);
		}

		public string EnMessage
		{
			get => _en_message;
			set => this.SetProperty(ref _en_message, value);
		}

		public string NativeImplementationName => ExposureNotification.OverridesNativeImplementation ? "TEST" : "LIVE";

		public string CurrentBatchFileIndex
		{
			get
			{
				if (_user_data is null) {
					return string.Empty;
				} else {
					return string.Join(", ", _user_data.ServerBatchNumbers.Select(p => $"{p.Key}={p.Value}"));
				}
			}
		}

		public Command ResetSelfDiagnosis => new Command(async () => {
			if (!(_user_data is null)) {
				_user_data.ClearDiagnosis();
				await _user_data_service.SetAsync(_user_data);
				await UserDialogs.Instance.AlertAsync("Cleared self-diagnosis!");
			}
		});

		public Command ResetExposures => new Command(async () => {
			if (!(_user_data is null)) {
				await Device.InvokeOnMainThreadAsync(() => _user_data.ExposureInformation.Clear());
				_user_data.ExposureSummary = null;
				await _user_data_service.SetAsync(_user_data);
				await UserDialogs.Instance.AlertAsync("Cleared exposures!");
			}
		});

		public Command AddExposures => new Command(async () => {
			if (!(_user_data is null)) {
				await Device.InvokeOnMainThreadAsync(async () => {
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-14), TimeSpan.FromMinutes(5),  10, 6, RiskLevel.Lowest));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-13), TimeSpan.FromMinutes(10), 20, 6, RiskLevel.Low));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-12), TimeSpan.FromMinutes(15), 50, 6, RiskLevel.Medium));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-11), TimeSpan.FromMinutes(20), 50, 6, RiskLevel.MediumLow));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-10), TimeSpan.FromMinutes(30), 50, 6, RiskLevel.MediumHigh));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-9),  TimeSpan.FromMinutes(35), 70, 6, RiskLevel.High));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-8),  TimeSpan.FromMinutes(40), 70, 6, RiskLevel.Highest));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-7),  TimeSpan.FromMinutes(45), 80, 6, RiskLevel.VeryHigh));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-6),  TimeSpan.FromMinutes(50), 80, 6, RiskLevel.VeryHigh));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-5),  TimeSpan.FromMinutes(55), 70, 6, RiskLevel.MediumHigh));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-4),  TimeSpan.FromMinutes(0),  70, 6, RiskLevel.Medium));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-3),  TimeSpan.FromMinutes(5),  70, 6, RiskLevel.MediumLow));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-2),  TimeSpan.FromMinutes(3),  30, 6, RiskLevel.Low));
					_user_data.ExposureInformation.Add(new ExposureInfo(DateTime.UtcNow.AddDays(-1),  TimeSpan.FromMinutes(20), 70, 6, RiskLevel.MediumHigh));
					await _user_data_service.SetAsync(_user_data);
				});
			}
		});

		public Command UpdateStatus => new Command(async () => {
			_user_data = _user_data_service.Get();
			await this.FetchEnMessage();
		});

		public Command ToggleWelcome => new Command(async () => {
			if (!(_user_data is null)) {
				_user_data.IsOptined = !_user_data.IsOptined;
				await _user_data_service.SetAsync(_user_data);
			}
		});

		public Command ToggleEn => new Command(async () => {
			if (!(_user_data is null)) {
				_user_data.IsExposureNotificationEnabled = !_user_data.IsExposureNotificationEnabled;
				await _user_data_service.SetAsync(_user_data);
			}
		});

		public Command ResetEnabled => new Command(async () => {
			if (!(_user_data is null)) {
				using (UserDialogs.Instance.Loading(string.Empty)) {
					if (await ExposureNotification.IsEnabledAsync()) {
						await ExposureNotification.StopAsync();
					}
					await _user_data_service.SetAsync(_user_data);
				}
				await UserDialogs.Instance.AlertAsync("Cleared the enabled state!");
			}
		});

		public Command ResetBatchFileIndex => new Command(async () => {
			if (!(_user_data is null)) {
				_user_data.ServerBatchNumbers = AppSettings.Instance.GetDefaultBatch();
				await _user_data_service.SetAsync(_user_data);
				this.RaisePropertyChanged(nameof(this.CurrentBatchFileIndex));
				await UserDialogs.Instance.AlertAsync("Cleared batch file index!");
			}
		});

		public Command ManualTriggerKeyFetch => new Command(async () => {
			using (UserDialogs.Instance.Loading("Fetching...")) {
				var task = this.ExposureNotificationService?.FetchExposureKeyAsync();
				if (!(task is null)) {
					await task;
				}
				this.RaisePropertyChanged(nameof(this.CurrentBatchFileIndex));
			}
		});

		public DebugPageViewModel(INavigationService navigationService, ExposureNotificationService exposureNotificationService, IUserDataService userDataService)
			: base(navigationService, exposureNotificationService)
		{
			_user_data_service = userDataService;
			_user_data         = userDataService.Get();
			_en_message        = this.ExposureNotificationService?.CurrentStatusMessage ?? string.Empty;
			this.Title         = "Debug";
			_user_data_service.UserDataChanged += this.UserDataChanged;
		}

		private async void UserDataChanged(object sender, UserDataModel e)
		{
			_user_data = e;
			await this.FetchEnMessage();
		}

		private async ValueTask FetchEnMessage()
		{
			var task = this.ExposureNotificationService?.UpdateStatusMessageAsync();
			if (!(task is null)) {
				await task;
			}
			this.EnMessage = this.ExposureNotificationService?.CurrentStatusMessage ?? string.Empty;
		}
	}
}
