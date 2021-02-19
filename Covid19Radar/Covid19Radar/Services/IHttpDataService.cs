using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Model;

namespace Covid19Radar.Services
{
	public interface IHttpDataService
	{
		public ValueTask<bool>                                      PostRegisterUserAsync      (UserDataModel userData);
		public ValueTask<HttpStatusCode>                            PutSelfExposureKeysAsync   (DiagnosisSubmissionParameter request);
		public ValueTask<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken);
		public ValueTask<Stream>                                    GetTemporaryExposureKey    (string url,    CancellationToken cancellationToken);
		public ValueTask<ApiResponse<LogStorageSas?>>               GetLogStorageSas           ();
	}
}
