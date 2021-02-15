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
			string s = string.Join("|",
				submission.AppPackageName,
				submission.Keys   ?.GetKeyString(),
				submission.Regions?.GetRegionString(),
				submission.VerificationPayload);
			using (var sha = SHA256.Create()) {
				return sha.ComputeHash(Encoding.UTF8.GetBytes(s));
			}
		}

		private static string GetKeyString(this IEnumerable<DiagnosisSubmissionParameter.Key> keys)
		{
			return string.Join(",", keys.OrderBy(k => k.KeyData).Select(k => k.GetKeyString()));
		}

		private static string GetKeyString(this DiagnosisSubmissionParameter.Key k)
		{
			return string.Join(".", k.KeyData, k.RollingStartNumber, k.RollingPeriod, k.TransmissionRisk);
		}

		private static string GetRegionString(this IEnumerable<string> regions)
		{
			return string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));
		}
	}
}
