using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
	internal class HttpDataServiceMock : IHttpDataService
	{
		private readonly ILoggerService _logger;

		public HttpDataServiceMock(ILoggerService logger)
		{
			_logger = logger;
		}

		public ValueTask<Stream> GetTemporaryExposureKey(string url, CancellationToken cancellationToken)
		{
			_logger.StartMethod();
			_logger.Debug($"called {nameof(HttpDataServiceMock)}::{nameof(this.GetTemporaryExposureKey)}");
			var result = new ValueTask<Stream>(new MemoryStream());
			_logger.EndMethod();
			return result;
		}

		public ValueTask<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
		{
			_logger.StartMethod();
			_logger.Debug($"called {nameof(HttpDataServiceMock)}::{nameof(this.GetTemporaryExposureKeyList)}");
			var result = new ValueTask<List<TemporaryExposureKeyExportFileModel>>(new List<TemporaryExposureKeyExportFileModel>());
			_logger.EndMethod();
			return result;
		}

		public async ValueTask<bool> PostRegisterUserAsync(UserDataModel userData)
		{
			_logger.StartMethod();
			_logger.Debug($"called {nameof(HttpDataServiceMock)}::{nameof(this.PostRegisterUserAsync)}");
			userData.Secret             = "dummy secret";
			userData.UserUuid           = "dummy uuid";
			userData.JumpConsistentSeed = 999;
			userData.IsOptined          = true;
			Application.Current.Properties[AppConstants.StorageKey.Secret] = userData.Secret;
			await Application.Current.SavePropertiesAsync();
			_logger.EndMethod();
			return true;
		}

		public ValueTask<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
		{
			_logger.StartMethod();
			_logger.Debug($"called {nameof(HttpDataServiceMock)}::{nameof(this.PutSelfExposureKeysAsync)}");
			var result = new ValueTask<HttpStatusCode>(HttpStatusCode.OK);
			_logger.EndMethod();
			return result;
		}

		public ValueTask<ApiResponse<LogStorageSas?>> GetLogStorageSas()
		{
			_logger.StartMethod();
			_logger.Debug($"called {nameof(HttpDataServiceMock)}::{nameof(this.PutSelfExposureKeysAsync)}");
			var result = new ValueTask<ApiResponse<LogStorageSas?>>(new ApiResponse<LogStorageSas?>(
				HttpStatusCode.OK,
				new() { SasToken = "sv=2012-02-12&se=2015-07-08T00%3A12%3A08Z&sr=c&sp=wl&sig=t%2BbzU9%2B7ry4okULN9S0wst%2F8MCUhTjrHyV9rDNLSe8g%3Dsss" }
			));
			_logger.EndMethod();
			return result;
		}
	}
}
