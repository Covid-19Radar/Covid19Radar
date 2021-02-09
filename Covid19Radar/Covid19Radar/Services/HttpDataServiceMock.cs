using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
	internal class HttpDataServiceMock : IHttpDataService
	{
		public ValueTask<Stream?> GetTemporaryExposureKey(string url, CancellationToken cancellationToken)
		{
			Debug.WriteLine($"called {nameof(HttpDataServiceMock)}::{nameof(this.GetTemporaryExposureKey)}");
			return new ValueTask<Stream?>(new MemoryStream());
		}

		public ValueTask<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
		{
			Debug.WriteLine($"called {nameof(HttpDataServiceMock)}::{nameof(this.GetTemporaryExposureKeyList)}");
			return new ValueTask<List<TemporaryExposureKeyExportFileModel>>(new List<TemporaryExposureKeyExportFileModel>());
		}

		public async ValueTask<UserDataModel?> PostRegisterUserAsync()
		{
			Debug.WriteLine($"called {nameof(HttpDataServiceMock)}::{nameof(this.PostRegisterUserAsync)}");
			var userData = new UserDataModel();
			userData.Secret             = "dummy secret";
			userData.UserUuid           = "dummy uuid";
			userData.JumpConsistentSeed = 999;
			userData.IsOptined          = true;
			Application.Current.Properties[AppConstants.StorageKey.Secret] = userData.Secret;
			await Application.Current.SavePropertiesAsync();
			return userData;
		}

		public ValueTask<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
		{
			Debug.WriteLine($"called {nameof(HttpDataServiceMock)}::{nameof(this.PutSelfExposureKeysAsync)}");
			return new ValueTask<HttpStatusCode>(HttpStatusCode.OK);
		}

		public ValueTask<ApiResponse<LogStorageSas?>> GetLogStorageSas()
		{
			Debug.WriteLine($"called {nameof(HttpDataServiceMock)}::{nameof(this.PutSelfExposureKeysAsync)}");
			return new ValueTask<ApiResponse<LogStorageSas?>>(new ApiResponse<LogStorageSas?>(
				((int)(HttpStatusCode.OK)),
				new LogStorageSas { SasToken = "sv=2012-02-12&se=2015-07-08T00%3A12%3A08Z&sr=c&sp=wl&sig=t%2BbzU9%2B7ry4okULN9S0wst%2F8MCUhTjrHyV9rDNLSe8g%3Dsss" }
			));
		}
	}
}
