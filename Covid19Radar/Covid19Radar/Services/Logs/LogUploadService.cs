using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Covid19Radar.Services.Logs
{
	public class LogUploadService : ILogUploadService
	{
		private readonly ILoggerService   _logger;
		private readonly IHttpDataService _http_data;
		private readonly ILogPathService  _log_path;
		private readonly IStorageService  _storage;

		public LogUploadService(ILoggerService logger, IHttpDataService httpData, ILogPathService logPath, IStorageService storage)
		{
			_logger    = logger;
			_http_data = httpData;
			_log_path  = logPath;
			_storage   = storage;
		}

		public async ValueTask<bool> UploadAsync(string zipFileName)
		{
			_logger.StartMethod();
			try {
				// Get the storage SAS Token for upload.
				var response = await _http_data.GetLogStorageSas();
				if (response.StatusCode != HttpStatusCode.OK) {
					_logger.Error("Bad response.");
					_logger.EndMethod();
					return false;
				}
				if (string.IsNullOrEmpty(response.Result?.SasToken)) {
					_logger.Error("The storage SAS token is null or empty.");
					_logger.EndMethod();
					return false;
				}

				// Upload to storage.
				var setting = AppSettings.Instance;
				return await _storage.UploadAsync(
					setting.LogStorageEndpoint,
					setting.LogStorageContainerName,
					setting.LogStorageAccountName,
					response.Result.SasToken,
					Path.Combine(_log_path.LogUploadingTmpPath, zipFileName)
				);
			} catch (Exception e) {
				_logger.Exception("Failed to upload.", e);
				return false;
			} finally {
				_logger.EndMethod();
			}
		}
	}
}
