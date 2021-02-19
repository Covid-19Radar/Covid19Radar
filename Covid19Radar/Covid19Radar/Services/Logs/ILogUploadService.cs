using System.Threading.Tasks;

namespace Covid19Radar.Services.Logs
{
	public interface ILogUploadService
	{
		public ValueTask<bool> UploadAsync(string zipFileName);
	}
}
