using System;
using System.Threading.Tasks;
using Covid19Radar.iOS.Services;
using Covid19Radar.Model;
using Covid19Radar.Services;
using DeviceCheck;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceCheckService))]

namespace Covid19Radar.iOS.Services
{
	public class DeviceCheckService : IDeviceVerifier
	{
		public async Task<string> VerifyAsync(DiagnosisSubmissionParameter submission)
		{
			var token = await DCDevice.CurrentDevice.GenerateTokenAsync();
			return Convert.ToBase64String(token.ToArray());
		}
	}
}
