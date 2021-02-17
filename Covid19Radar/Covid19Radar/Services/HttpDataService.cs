using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Covid19Radar.Services
{
	public class HttpDataService : IHttpDataService
	{
		private readonly ILoggerService _logger;
		private readonly HttpClient     _api;  // API key based client.
		private readonly HttpClient     _http; // Secret based client.
		private readonly HttpClient     _download;

		public HttpDataService(ILoggerService logger, IHttpClientService httpClientService)
		{
			_logger = logger;

			// Create API key based client.
			_api = httpClientService.Create();
			_api.BaseAddress = new Uri(AppSettings.Instance.ApiUrlBase);
			_api.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_api.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
			_api.DefaultRequestHeaders.Add("x-api-key", AppSettings.Instance.ApiKey);

			// Create Secret based client.
			_http = httpClientService.Create();
			_http.BaseAddress = new Uri(AppSettings.Instance.ApiUrlBase);
			_http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_http.DefaultRequestHeaders.Add("x-functions-key", AppSettings.Instance.ApiSecret);
			this.SetSecret();

			// Create download client.
			_download = httpClientService.Create();
		}

		private void SetSecret()
		{
			_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SecureStorage.GetAsync(AppConstants.StorageKey.Secret).Result);
		}

		// POST /api/Register - Register User
		public async ValueTask<bool> PostRegisterUserAsync(UserDataModel userData)
		{
			_logger.StartMethod();
			try {
				string  url     = AppSettings.Instance.ApiUrlBase + "/register";
				var     content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
				string? result  = await this.PostAsync(url, content);
				if (!string.IsNullOrEmpty(result)) {
					var registerResult = JsonConvert.DeserializeObject<RegisterResultModel>(result);
					userData.Secret             = registerResult.Secret;
					userData.UserUuid           = registerResult.UserUuid;
					userData.JumpConsistentSeed = registerResult.JumpConsistentSeed;
					userData.IsOptined          = true;
					await SecureStorage.SetAsync(AppConstants.StorageKey.Secret, registerResult.Secret);
					this.SetSecret();
					_logger.EndMethod();
					return true;
				}
			} catch (HttpRequestException e) {
				_logger.Exception("Failed to register an user.", e);
			}
			_logger.EndMethod();
			return false;
		}

		public async ValueTask<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
		{
			_logger.StartMethod();
			string url      = $"{AppSettings.Instance.ApiUrlBase.TrimEnd('/')}/diagnosis";
			var    content  = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
			var    response = await _http.PutAsync(url, content);
			await response.Content.ReadAsStringAsync();
			_logger.EndMethod();
			return response.StatusCode;
		}

		public async ValueTask<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
		{
			_logger.StartMethod();
			string  container = AppSettings.Instance.BlobStorageContainerName;
			string  url       = AppSettings.Instance.CdnUrlBase + $"{container}/{region}/list.json";
			string? result    = await this.GetCdnAsync(url, async c => await c.ReadAsStringAsync(), cancellationToken);
			if (result is null) {
				_logger.Error("Failed to download");
				_logger.EndMethod();
				return new List<TemporaryExposureKeyExportFileModel>();
			} else {
				_logger.Info("Success to download");
				_logger.EndMethod();
				return JsonConvert.DeserializeObject<List<TemporaryExposureKeyExportFileModel>>(result);
			}
		}

		public async ValueTask<Stream> GetTemporaryExposureKey(string url, CancellationToken cancellationToken)
		{
			var result = await this.GetCdnAsync(url, async c => await c.ReadAsStreamAsync(), cancellationToken);
			if (result is null) {
				throw new NullReferenceException("The download result was null.");
			} else {
				return result;
			}
		}

		public async ValueTask<ApiResponse<LogStorageSas?>> GetLogStorageSas()
		{
			_logger.StartMethod();
			HttpStatusCode statusCode;
			LogStorageSas? logStorageSas;
			try {
				string requestUrl = $"{AppSettings.Instance.ApiUrlBase.TrimEnd('/')}/inquirylog";
				var    response   = await _api.GetAsync(requestUrl);
				statusCode = response.StatusCode;
				_logger.Info($"Response status: {statusCode} ({(int)(statusCode)})");
				if (response.StatusCode == HttpStatusCode.OK) {
					logStorageSas = JsonConvert.DeserializeObject<LogStorageSas>(await response.Content.ReadAsStringAsync());
				} else {
					logStorageSas = null;
				}
			} catch (Exception e) {
				_logger.Exception("Failed get log storage SAS.", e);
				statusCode    = 0;
				logStorageSas = null;
			}
			_logger.EndMethod();
			return new ApiResponse<LogStorageSas?>(statusCode, logStorageSas);
		}

		private async ValueTask<T?> GetCdnAsync<T>(string url, Func<HttpContent, ValueTask<T?>> read, CancellationToken cancellationToken)
		{
			_logger.StartMethod();
			var response = await _download.GetAsync(url, cancellationToken);
			var result   = await read(response.Content);
			if (response.StatusCode == HttpStatusCode.OK) {
				_logger.Info("Succeed to download.");
				_logger.EndMethod();
				return result;
			} else {
				_logger.Error("Failed to download.");
				_logger.EndMethod();
				return default;
			}
		}

		private async ValueTask<string?> PostAsync(string url, HttpContent body)
		{
			_logger.StartMethod();
			var     response = await _http.PostAsync(url, body);
			string? result   = await response.Content.ReadAsStringAsync();
			if (response.StatusCode == HttpStatusCode.OK) {
				_logger.Info("Succeed to post data.");
				_logger.EndMethod();
				return result;
			} else {
				_logger.Error("Failed to post data.");
				return null;
			}
		}
	}
}
