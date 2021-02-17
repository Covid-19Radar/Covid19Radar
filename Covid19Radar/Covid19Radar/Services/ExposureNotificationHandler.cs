using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
	[Xamarin.Forms.Internals.Preserve()] // Ensure this is not linked out
	public class ExposureNotificationHandler : IExposureNotificationHandler
	{
		private readonly ILoggerService              _logger;
		private readonly IHttpDataService            _http_data;
		private readonly IUserDataService            _user_data_service;
		private readonly ExposureNotificationService _ens;
		private          UserDataModel?              _user_data;

		// this string should be localized
		public string UserExplanation => AppResources.LocalNotificationDescription;

		public ExposureNotificationHandler()
		{
			_logger            = DependencyService.Resolve<ILoggerService>();
			_http_data         = DependencyService.Resolve<IHttpDataService>();
			_user_data_service = DependencyService.Resolve<IUserDataService>();
			_ens               = DependencyService.Resolve<ExposureNotificationService>();

			_user_data = _user_data_service.Get();
			_user_data_service.UserDataChanged += (s, e) => {
				_user_data = _user_data_service.Get();
				_logger.Info("Updated the user data.");
			};
			_logger.Info($"the user data is {(_user_data is null ? "null" : "set")}.");
		}

		// this configuration should be obtained from a server and it should be cached locally/in memory as it may be called multiple times
		public Task<Configuration> GetConfigurationAsync()
		{
			_logger.StartMethod();

			if (Application.Current.Properties.TryGetValue(AppConstants.StorageKey.ExNConfig, out object jsonObj)) {
				string json = jsonObj.ToString();
				_logger.Info($"Got the configuration: {json}");
				_logger.EndMethod();
				return Task.FromResult(JsonConvert.DeserializeObject<Configuration>(json));
			}

			var config = new Configuration() {
				MinimumRiskScore                = 21,
				AttenuationWeight               = 50,
				TransmissionWeight              = 50,
				DurationWeight                  = 50,
				DaysSinceLastExposureWeight     = 50,
				TransmissionRiskScores          = new int[] { 7, 7, 7, 7, 7, 7, 7, 7 },
				AttenuationScores               = new[] { 1, 2, 3, 4, 5, 6, 7, 8 },
				DurationScores                  = new[] { 0, 0, 0, 0, 1, 1, 1, 1 },
				DaysSinceLastExposureScores     = new[] { 1, 1, 1, 1, 1, 1, 1, 1 },
				DurationAtAttenuationThresholds = new[] { 50, 70 }
			};

			_logger.Info($"Created the default configuration: {JsonConvert.SerializeObject(config)}");
			_logger.EndMethod();
			return Task.FromResult(config);
		}

		// this will be called when a potential exposure has been detected
		public async Task ExposureDetectedAsync(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo)
		{
			_logger.StartMethod();

			if (_user_data is null) {
				_logger.Warning("the user data was null!");
				_logger.EndMethod();
				return;
			}

			_user_data.ExposureSummary = summary;
			_logger.Info($"{nameof(_user_data.ExposureSummary)}.{nameof(summary.MatchedKeyCount      )}: {summary.MatchedKeyCount}");
			_logger.Info($"{nameof(_user_data.ExposureSummary)}.{nameof(summary.DaysSinceLastExposure)}: {summary.DaysSinceLastExposure}");
			_logger.Info($"{nameof(_user_data.ExposureSummary)}.{nameof(summary.HighestRiskScore     )}: {summary.HighestRiskScore}");
			_logger.Info($"{nameof(_user_data.ExposureSummary)}.{nameof(summary.AttenuationDurations )}: {string.Join(",", summary.AttenuationDurations)}");
			_logger.Info($"{nameof(_user_data.ExposureSummary)}.{nameof(summary.SummationRiskScore   )}: {summary.SummationRiskScore}");

			var config = await this.GetConfigurationAsync();
			if (summary.HighestRiskScore >= config.MinimumRiskScore) {
				var info = await getExposureInfo();
				_logger.Info($"{nameof(ExposureInfo)}: {info.Count()}");

				// Add these on main thread in case the UI is visible so it can update
				await Device.InvokeOnMainThreadAsync(() => {
					foreach (var item in info) {
						_logger.Info($"{nameof(item)}.{nameof(item.Timestamp            )}: {item.Timestamp}");
						_logger.Info($"{nameof(item)}.{nameof(item.Duration             )}: {item.Duration}");
						_logger.Info($"{nameof(item)}.{nameof(item.AttenuationValue     )}: {item.AttenuationValue}");
						_logger.Info($"{nameof(item)}.{nameof(item.TotalRiskScore       )}: {item.TotalRiskScore}");
						_logger.Info($"{nameof(item)}.{nameof(item.TransmissionRiskLevel)}: {item.TransmissionRiskLevel}");
						_logger.Info("========");
						_user_data.ExposureInformation.Add(item);
					}
				});
			}

			_logger.Info($"Saving mached key count: {_user_data.ExposureSummary.MatchedKeyCount}");
			_logger.Info($"Saving exposure information count: {_user_data.ExposureInformation.Count}");
			await _user_data_service.SetAsync(_user_data);

			_logger.EndMethod();
		}

		// this will be called when they keys need to be collected from the server
		public async Task FetchExposureKeyBatchFilesFromServerAsync(Func<IEnumerable<string>, Task> submitBatches, CancellationToken cancellationToken)
		{
			_logger.StartMethod();
			try {
				foreach (string serverRegion in AppSettings.Instance.SupportedRegions) {
					cancellationToken.ThrowIfCancellationRequested();

					_logger.Info("Start to download files");
					var (batchNumber, downloadedFiles) = await this.DownloadBatchAsync(serverRegion, cancellationToken);
					_logger.Info("End to download files");
					_logger.Info($"Batch number: {batchNumber}, Downloaded files: {downloadedFiles.Count}");

					if (batchNumber != 0 && downloadedFiles.Count > 0) {
						_logger.Info("C19R Submit Batches");
						await submitBatches(downloadedFiles);

						// delete all temporary files
						foreach (string file in downloadedFiles) {
							try {
								File.Delete(file);
							} catch (Exception e) {
								// no-op
								_logger.Exception("Fail to delete downloaded files", e);
							}
						}
					}
				}
			} catch (Exception e) {
				// any expections, bail out and wait for the next time
				_logger.Exception("Fail to download files", e);
			}
			_logger.EndMethod();
		}

		private async Task<(int, List<string>)> DownloadBatchAsync(string region, CancellationToken cancellationToken)
		{
			_logger.StartMethod();

			int    batchNumber     = 0;
			var    downloadedFiles = new List<string>();
			string tmpDir          = Path.Combine(FileSystem.CacheDirectory, region);

			if (_user_data is null) {
				_logger.Warning("The user data was null.");
				_logger.EndMethod();
				return (batchNumber, downloadedFiles);
			}

			try {
				if (!Directory.Exists(tmpDir)) {
					Directory.CreateDirectory(tmpDir);
				}
			} catch (Exception ex) {
				_logger.Exception("Failed to create directory.", ex);
				_logger.EndMethod();
				return (batchNumber, downloadedFiles);
			}

			var tekList = await _http_data.GetTemporaryExposureKeyList(region, cancellationToken);
			if (tekList.Count == 0) {
				_logger.Warning("No items in tek list.");
				_logger.EndMethod();
				return (batchNumber, downloadedFiles);
			}
			Debug.WriteLine("C19R Fetch Exposure Key");

			var lastTekTimestamp = _user_data.LastProcessTekTimestamp;

			foreach (var tekItem in tekList)
			{
				long lastCreated = 0;
				if (lastTekTimestamp.TryGetValue(region, out long value)) {
					lastCreated = value;
				} else {
					lastTekTimestamp.Add(region, 0);
				}
				_logger.Info($"{nameof(tekItem)}.{nameof(tekItem.Created)}: {tekItem.Created}");
				if (tekItem.Created > lastCreated || lastCreated == 0) {
					string tmpFile = Path.Combine(tmpDir, Guid.NewGuid().ToString() + ".zip");
					Debug.WriteLine(JsonConvert.SerializeObject(tekItem));
					Debug.WriteLine(tmpFile);
					if (string.IsNullOrEmpty(tekItem.Url)) {
						_logger.Warning("The URL for a TEK item was empty.");
						continue;
					}
					_logger.Info($"Downloading TEK file from: {tekItem.Url}");
					try {
						await using (var rs = await _http_data.GetTemporaryExposureKey(tekItem.Url, cancellationToken))
						await using (var fs = File.Create(tmpFile)) {
							await rs.CopyToAsync(fs, cancellationToken);
							fs.Flush();
						}
					} catch (Exception e) {
						_logger.Exception("Failed to copy", e);
					}
					lastTekTimestamp[region] = tekItem.Created;
					downloadedFiles.Add(tmpFile);
					Debug.WriteLine($"C19R FETCH DIAGKEY {tmpFile}");
					++batchNumber;
				}
			}

			_logger.Info($"The batch number: {batchNumber}, Downloaded files: {downloadedFiles.Count}");
			_user_data.LastProcessTekTimestamp = lastTekTimestamp;
			await _user_data_service.SetAsync(_user_data);
			_logger.Info($"The region: {region}, the last process TEK timestamp: {_user_data.LastProcessTekTimestamp[region]}");

			_logger.EndMethod();
			return (batchNumber, downloadedFiles);
		}

		// this will be called when the user is submitting a diagnosis and the local keys need to go to the server
		public async Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			_logger.StartMethod();

			_logger.Info($"The count of temporary exposure keys: {temporaryExposureKeys.Count()}");
			foreach (var item in temporaryExposureKeys) {
				_logger.Info($"{nameof(item.RollingStart         )}: {item.RollingStart} ({item.RollingStart.ToUnixTimeSeconds()})");
				_logger.Info($"{nameof(item.RollingDuration      )}: {item.RollingDuration}");
				_logger.Info($"{nameof(item.TransmissionRiskLevel)}: {item.TransmissionRiskLevel}");
				_logger.Info("========");
			}

			if (_user_data is null) {
				_logger.Warning("The user data was null.");
				_logger.EndMethod();
				return;
			}

			var latestDiagnosis = _user_data.LatestDiagnosis;
			if (latestDiagnosis is null || string.IsNullOrEmpty(latestDiagnosis.DiagnosisUid))
			{
				_logger.Error("The diagnostic number is null or empty.");
				_logger.EndMethod();
				throw new InvalidOperationException("The diagnostic number is null or empty.");
			}

			var selfDiag       = await this.CreateSubmissionAsync(temporaryExposureKeys, latestDiagnosis);
			var httpStatusCode = await _http_data.PutSelfExposureKeysAsync(selfDiag);
			_logger.Info($"The HTTP status is {httpStatusCode} ({((int)(httpStatusCode))}).");

			if (httpStatusCode == HttpStatusCode.NotAcceptable) {
				await UserDialogs.Instance.AlertAsync(
					string.Empty,
					AppResources.ExposureNotificationHandler1ErrorMessage,
					AppResources.ButtonOk
				);
				_logger.Error("The diagnostic number is incorrect.");
				_logger.EndMethod();
				throw new InvalidOperationException("The diagnostic number is incorrect.");
			} else if (httpStatusCode == HttpStatusCode.ServiceUnavailable || httpStatusCode == HttpStatusCode.InternalServerError) {
				await UserDialogs.Instance.AlertAsync(
					string.Empty,
					AppResources.ExposureNotificationHandler2ErrorMessage,
					AppResources.ButtonOk
				);
				_logger.Error("Cannot connect to the center.");
				_logger.EndMethod();
				throw new InvalidOperationException("Cannot connect to the center.");
			} else if (httpStatusCode == HttpStatusCode.BadRequest) {
				await UserDialogs.Instance.AlertAsync(
					string.Empty,
					AppResources.ExposureNotificationHandler3ErrorMessage,
					AppResources.ButtonOk
				);
				_logger.Error("There is a problem with the record data.");
				_logger.EndMethod();
				throw new InvalidOperationException("There is a problem with the record data.");
			}

			await _user_data_service.SetAsync(_user_data);
			_logger.EndMethod();
		}


		private async Task<DiagnosisSubmissionParameter> CreateSubmissionAsync(
			IEnumerable<TemporaryExposureKey> temporaryExposureKeys, PositiveDiagnosisState pendingDiagnosis)
		{
			_logger.StartMethod();

			// Filter Temporary exposure keys
			var filteredTemporaryExposureKeys = _ens.FliterTemporaryExposureKeys(temporaryExposureKeys);

			// Create the network keys
			var keys = filteredTemporaryExposureKeys.Select(k => new DiagnosisSubmissionParameter.Key() {
				KeyData             = Convert.ToBase64String(k.Key),
				RollingStartNumber  = ((uint)((k.RollingStart - DateTime.UnixEpoch).TotalMinutes / 10)),
				RollingPeriod       = ((uint)(k.RollingDuration.TotalMinutes / 10)),
				TransmissionRisk    = ((int)(k.TransmissionRiskLevel))
			});

			string beforeKey = JsonConvert.SerializeObject(temporaryExposureKeys.ToList());
			string afterKey  = JsonConvert.SerializeObject(keys.ToList());
			Debug.WriteLine($"C19R {beforeKey}");
			Debug.WriteLine($"C19R {afterKey}");

			if (keys.Count() == 0) {
				_logger.Error("Temporary exposure keys are empty.");
				_logger.EndMethod();
				throw new InvalidDataException("Temporary exposure keys are empty.");
			}

			// Generate Padding
			string padding = this.GetPadding();

			_logger.Info($"The user data is {(_user_data is null ? "null" : "set")}.");

			// Create the submission
			var submission = new DiagnosisSubmissionParameter() {
				UserUuid                  = _user_data?.UserUuid,
				Keys                      = keys.ToArray(),
				Regions                   = AppSettings.Instance.SupportedRegions,
				Platform                  = DeviceInfo.Platform.ToString().ToLowerInvariant(),
				DeviceVerificationPayload = null,
				AppPackageName            = AppInfo.PackageName,
				VerificationPayload       = pendingDiagnosis.DiagnosisUid,
				Padding                   = padding
			};

			// See if we can add the device verification
			if (DependencyService.Get<IDeviceVerifier>() is IDeviceVerifier verifier) {
				submission.DeviceVerificationPayload = await verifier.VerifyAsync(submission);
			}

			_logger.Info($"The user uuid is {(string.IsNullOrEmpty(submission.UserUuid) ? "null or empty" : "set")}.");
			_logger.Info($"The device verification payload is {(string.IsNullOrEmpty(submission.DeviceVerificationPayload) ? "null or empty" : "set")}.");
			_logger.Info($"The verification payload is {(string.IsNullOrEmpty(submission.VerificationPayload) ? "null or empty" : "set")}.");

			_logger.EndMethod();
			return submission;
		}

		private string GetPadding()
		{
			// Approximate the base64 blowup.
			int    size    = ((int)(RandomNumberGenerator.GetInt32(1024, 2048) * 0.75D));
			byte[] padding = new byte[size];
			using (var rng = RandomNumberGenerator.Create()) {
				rng.GetBytes(padding);
			}
			return Convert.ToBase64String(padding);
		}
	}
}
