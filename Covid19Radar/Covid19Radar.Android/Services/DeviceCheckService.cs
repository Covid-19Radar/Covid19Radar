using System.Threading.Tasks;
using Android.Gms.SafetyNet;
using Covid19Radar.Droid.Services;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(DeviceCheckService))]

namespace Covid19Radar.Droid.Services
{
	public class DeviceCheckService : IDeviceVerifier
	{
		public Task<string> VerifyAsync(DiagnosisSubmissionParameter submission)
		{
			byte[] nonce = submission.GetAndroidNonce();
			return this.GetSafetyNetAttestationAsync(nonce);
		}

		/// <summary>
		///  Verification device information required for positive submissions
		/// </summary>
		/// <returns>Device Verification Payload</returns>
		async Task<string> GetSafetyNetAttestationAsync(byte[] nonce)
		{
			using var client   = SafetyNetClass.GetClient(AndroidApp.Context);
			using var response = await client.AttestAsync(nonce, AppSettings.Instance.AndroidSafetyNetApiKey);
			return response.JwsResult;
		}
	}
}
