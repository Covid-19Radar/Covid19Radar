using Covid19Radar.Model;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
	/// <summary>
	///  Verification device information required for positive submissions
	/// </summary>
	/// <remarks>
	///  See deviceVerificationPayload
	///  https://github.com/google/exposure-notifications-server/blob/master/docs/server_functional_requirements.md
	/// </remarks>
	/// <returns>Device Verification Payload</returns>
	public interface IDeviceVerifier
	{
		public Task<string> VerifyAsync(DiagnosisSubmissionParameter submission);
	}
}
