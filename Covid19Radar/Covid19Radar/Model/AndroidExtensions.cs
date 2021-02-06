using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Covid19Radar.Model
{
	public static class AndroidExtensions
	{
		public static byte[] GetAndroidNonce(this DiagnosisSubmissionParameter submission)
		{
			return GetSha256(GetAndroidNonceClearText(submission));
		}

		static string GetAndroidNonceClearText(this DiagnosisSubmissionParameter submission)
		{
			return string.Join("|", submission.AppPackageName, GetKeyString(submission.Keys), GetRegionString(submission.Regions), submission.VerificationPayload);
		}

		static string GetKeyString(IEnumerable<DiagnosisSubmissionParameter.Key> keys)
		{
			return string.Join(",", keys.OrderBy(k => k.KeyData).Select(k => GetKeyString(k)));
		}

		static string GetKeyString(DiagnosisSubmissionParameter.Key k)
		{
			return string.Join(".", k.KeyData, k.RollingStartNumber, k.RollingPeriod, k.TransmissionRisk);
		}

		static string GetRegionString(IEnumerable<string> regions)
		{
			return string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));
		}

		static byte[] GetSha256(string text)
		{
			using var sha = SHA256.Create();
			return sha.ComputeHash(Encoding.UTF8.GetBytes(text));
		}
	}
}
