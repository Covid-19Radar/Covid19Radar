using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using ImTools;
using Newtonsoft.Json;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
	public class ExposureNotificationService
	{
		private readonly ILoggerService     _logger;
		private readonly IHttpClientService _http_client;
		private readonly IUserDataService   _user_data_service;
		private          UserDataModel      _user_data;

		public string CurrentStatusMessage       { get; set; }
		public Status ExposureNotificationStatus { get; set; }

		/// <summary>
		///  Date of diagnosis or onset (Local time)
		/// </summary>
		public DateTime? DiagnosisDate { get; set; }

		public ExposureNotificationService(ILoggerService logger, IHttpClientService httpClient, IUserDataService userData)
		{
			_logger                   = logger;
			_http_client              = httpClient;
			_user_data_service        = userData;
			this.CurrentStatusMessage = AppResources.ExposureNotificationStatusMessageUnknown;

			_ = this.GetExposureNotificationConfig();
			_user_data = userData.Get();
			userData.UserDataChanged += this.OnUserDataChanged;
		}

		public async Task GetExposureNotificationConfig()
		{
			_logger.StartMethod();

			string container = AppSettings.Instance.BlobStorageContainerName;
			string url       = AppSettings.Instance.CdnUrlBase + $"{container}/Configration.json";
			var httpClient   = _http_client.Create();
			var result       = await httpClient.GetAsync(url);
			if (result.StatusCode == HttpStatusCode.OK) {
				_logger.Info("Success to download configuration");
				Application.Current.Properties[AppConstants.StorageKey.ExNConfig] = await result.Content.ReadAsStringAsync();
				await Application.Current.SavePropertiesAsync();
			} else {
				_logger.Error("Failed to download configuration");
			}

			_logger.EndMethod();
		}

		private async void OnUserDataChanged(object sender, UserDataModel? userData)
		{
			_logger.StartMethod();

			_user_data = _user_data_service.Get();
			_logger.Info("The user data has changed.");
			_logger.Debug("The current user data: " + JsonConvert.SerializeObject(_user_data));
			if (_user_data != userData) {
				_logger.Warning("The specified user data is invalid: " + JsonConvert.SerializeObject(userData));
			}
			await this.UpdateStatusMessageAsync();

			_logger.EndMethod();
		}

		public async Task FetchExposureKeyAsync()
		{
			_logger.StartMethod();
			await ExposureNotification.UpdateKeysFromServer();
			_logger.EndMethod();
		}

		public int GetExposureCount()
		{
			_logger.StartMethod();
			int result = _user_data?.ExposureInformation?.Count() ?? -1;
			_logger.EndMethod();
			return result;
		}

		public async Task<string> UpdateStatusMessageAsync()
		{
			_logger.StartMethod();
			this.ExposureNotificationStatus = await ExposureNotification.GetStatusAsync();
			string result = await this.GetStatusMessageAsync();
			_logger.EndMethod();
			return result;
		}

		private async ValueTask DisabledAsync()
		{
			_user_data.IsExposureNotificationEnabled = false;
			await _user_data_service.SetAsync(_user_data);
		}

		private async ValueTask EnabledAsync()
		{
			_user_data.IsExposureNotificationEnabled = true;
			await _user_data_service.SetAsync(_user_data);
		}

		public async Task<bool> StartExposureNotification()
		{
			_logger.StartMethod();
			try {
				if (!await ExposureNotification.IsEnabledAsync()) {
					await ExposureNotification.StartAsync();
				}
				await this.EnabledAsync();
				_logger.EndMethod();
				return true;
			} catch (Exception e) {
				await this.DisabledAsync();
				_logger.Exception("Error occurred during enabling notifications.", e);
				_logger.EndMethod();
				return false;
			}
		}

		public async Task<bool> StopExposureNotification()
		{
			_logger.StartMethod();
			try {
				if (await ExposureNotification.IsEnabledAsync()) {
					await ExposureNotification.StopAsync();
				}
				_logger.EndMethod();
				return true;
			} catch (Exception e) {
				_logger.Exception("Error occurred during disabling notifications.", e);
				_logger.EndMethod();
				return false;
			} finally {
				await this.DisabledAsync();
			}
		}

		private async Task<string> GetStatusMessageAsync()
		{
			string message = string.Empty;
			switch (this.ExposureNotificationStatus) {
			case Status.Unknown:
				await UserDialogs.Instance.AlertAsync(AppResources.ExposureNotificationStatusMessageUnknown, string.Empty, AppResources.ButtonOk);
				message = AppResources.ExposureNotificationStatusMessageUnknown;
				break;
			case Status.Disabled:
				await UserDialogs.Instance.AlertAsync(AppResources.ExposureNotificationStatusMessageDisabled, string.Empty, AppResources.ButtonOk);
				message = AppResources.ExposureNotificationStatusMessageDisabled;
				break;
			case Status.Active:
				message = AppResources.ExposureNotificationStatusMessageActive;
				break;
			case Status.BluetoothOff:
				// call out settings in each os
				await UserDialogs.Instance.AlertAsync(AppResources.ExposureNotificationStatusMessageBluetoothOff, string.Empty, AppResources.ButtonOk);
				message = AppResources.ExposureNotificationStatusMessageBluetoothOff;
				break;
			case Status.Restricted:
				// call out settings in each os
				await UserDialogs.Instance.AlertAsync(AppResources.ExposureNotificationStatusMessageRestricted, string.Empty, AppResources.ButtonOk);
				message = AppResources.ExposureNotificationStatusMessageRestricted;
				break;
			default:
				break;
			}
			
			if (!_user_data.IsOptined) {
				message.Append(ExposureNotificationStatusMessageNotOptined());
			}

			this.CurrentStatusMessage = message;
			return message;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string ExposureNotificationStatusMessageNotOptined()
		{
			// 互換性のため
			return AppResources.ExposureNotificationStatusMessageIsOptined;
		}

		public IEnumerable<TemporaryExposureKey> FliterTemporaryExposureKeys(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			_logger.StartMethod();

			IEnumerable<TemporaryExposureKey> newTemporaryExposureKeys;
			try {
				if (this.DiagnosisDate is DateTime diagnosisDate) {
					var fromDateTimeOffset = new DateTimeOffset(diagnosisDate.AddDays(AppConstants.DaysToSendTek));
					_logger.Info($"Filter: After {fromDateTimeOffset}");
					newTemporaryExposureKeys = temporaryExposureKeys.Where(x => x.RollingStart >= fromDateTimeOffset);
					_logger.Info($"Count: {newTemporaryExposureKeys.Count()}");
				} else {
					throw new InvalidOperationException("No diagnosis date has been set.");
				}
			} catch (Exception e) {
				_logger.Exception("Failed to filter temporary exposure keys.", e);
				throw;
			} finally {
				this.DiagnosisDate = null;
				_logger.EndMethod();
			}
			return newTemporaryExposureKeys;
		}
	}
}
