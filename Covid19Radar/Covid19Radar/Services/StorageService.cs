using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
	public interface IStorageService
	{
		public ValueTask<bool> UploadAsync(string endpoint, string uploadPath, string accountName, string sasToken, string sourceFilePath);
	}

	public class StorageService : IStorageService
	{
		private readonly ILoggerService _logger;

		public StorageService(ILoggerService logger)
		{
			_logger = logger;
		}

		public async ValueTask<bool> UploadAsync(string endpoint, string uploadPath, string accountName, string sasToken, string sourceFilePath)
		{
			_logger.StartMethod();
			bool result = false;
			try {
				var uri = new UriBuilder(endpoint) {
					Path  = $"{uploadPath.Trim('/')}/{Path.GetFileName(sourceFilePath)}",
					Query = sasToken.TrimStart('?')
				}.Uri;
				var client = new BlobClient(uri);
				using (var fs = File.OpenRead(sourceFilePath)) {
					var rawResponse = (await client.UploadAsync(fs)).GetRawResponse();
					result = rawResponse.Status == ((int)(HttpStatusCode.Created));
				}
			} catch (Exception e) {
				_logger.Exception("Failed upload to the storage.", e);
			}
			_logger.EndMethod();
			return result;
		}
	}
}
