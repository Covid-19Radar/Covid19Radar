using System;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class DebugPageViewModel : ViewModelBase
	{
		private readonly ILoggerService              _logger;
		private readonly ExposureNotificationService _ens;
		private readonly IUserDataService            _user_data_service;
		private          UserDataModel               _user_data;
		private          string                      _en_message;

		public UserDataModel UserData
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

		public string CurrentBatchFileIndex => string.Join(", ", _user_data.ServerBatchNumbers.Select(p => $"{p.Key}={p.Value}"));

		public Command ResetSelfDiagnosis => new(async () => {
			_logger.StartMethod();
			_user_data.ClearDiagnosis();
			await _user_data_service.SetAsync(_user_data);
			await UserDialogs.Instance.AlertAsync("Cleared self-diagnosis!");
			_logger.EndMethod();
		});

		public Command ResetExposures => new(async () => {
			_logger.StartMethod();
			await Device.InvokeOnMainThreadAsync(() => _user_data.ExposureInformation.Clear());
			_user_data.ExposureSummary = null;
			await _user_data_service.SetAsync(_user_data);
			await UserDialogs.Instance.AlertAsync("Cleared exposures!");
			_logger.EndMethod();
		});

		public Command AddExposures => new(async () => {
			_logger.StartMethod();
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
			_logger.EndMethod();
		});

		public Command UpdateStatus => new(async () => {
			_logger.StartMethod();
			_user_data = _user_data_service.Get();
			await this.FetchEnMessage();
			_logger.EndMethod();
		});

		public Command ToggleWelcome => new(async () => {
			_logger.StartMethod();
			_user_data.IsOptined = !_user_data.IsOptined;
			await _user_data_service.SetAsync(_user_data);
			_logger.EndMethod();
		});

		public Command ToggleEn => new(async () => {
			_logger.StartMethod();
			_user_data.IsExposureNotificationEnabled = !_user_data.IsExposureNotificationEnabled;
			await _user_data_service.SetAsync(_user_data);
			_logger.EndMethod();
		});

		public Command ResetEnabled => new(async () => {
			_logger.StartMethod();
			using (UserDialogs.Instance.Loading(string.Empty)) {
				if (await ExposureNotification.IsEnabledAsync()) {
					await ExposureNotification.StopAsync();
				}
				await _user_data_service.SetAsync(_user_data);
			}
			await UserDialogs.Instance.AlertAsync("Cleared the enabled state!");
			_logger.EndMethod();
		});

		public Command ResetBatchFileIndex => new(async () => {
			_logger.StartMethod();
			_user_data.ServerBatchNumbers = AppSettings.Instance.GetDefaultBatch();
			await _user_data_service.SetAsync(_user_data);
			this.RaisePropertyChanged(nameof(this.CurrentBatchFileIndex));
			await UserDialogs.Instance.AlertAsync("Cleared batch file index!");
			_logger.EndMethod();
		});

		public Command ManualTriggerKeyFetch => new(async () => {
			_logger.StartMethod();
			using (UserDialogs.Instance.Loading("Fetching...")) {
				await _ens.FetchExposureKeyAsync();
				this.RaisePropertyChanged(nameof(this.CurrentBatchFileIndex));
			}
			_logger.EndMethod();
		});

		public DebugPageViewModel(
			ILoggerService              logger,
			ExposureNotificationService exposureNotificationService,
			IUserDataService            userDataService)
		{
			_logger            = logger                      ?? throw new ArgumentNullException(nameof(logger));
			_ens               = exposureNotificationService ?? throw new ArgumentNullException(nameof(exposureNotificationService));
			_user_data_service = userDataService             ?? throw new ArgumentNullException(nameof(userDataService));
			_user_data         = userDataService.Get();
			_en_message        = exposureNotificationService.CurrentStatusMessage;
			this.Title         = "Debug";
			_user_data_service.UserDataChanged += this.UserDataChanged;
		}

		private async void UserDataChanged(object sender, UserDataModel e)
		{
			_logger.StartMethod();
			_user_data = e;
			await this.FetchEnMessage();
			_logger.EndMethod();
		}

		private async ValueTask FetchEnMessage()
		{
			await _ens.UpdateStatusMessageAsync();
			this.EnMessage = _ens.CurrentStatusMessage;
		}
	}
}
