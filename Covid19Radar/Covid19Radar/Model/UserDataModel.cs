using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Covid19Radar.Common;

namespace Covid19Radar.Model
{
	public class UserDataModel : IEquatable<UserDataModel>
	{
		/// <summary>
		/// User UUID / take care misunderstand Becon ID
		/// </summary>
		/// <value>User UUID</value>
		public string? UserUuid { get; set; }

		/// <summary>
		/// Secret key
		/// </summary>
		/// <value>Secret Key</value>
		public string? Secret { get; set; }

		/// <summary>
		/// Jump Consistent Seed
		/// </summary>
		/// <value>Jump Consistent Seed</value>
		public ulong JumpConsistentSeed { get; set; }

		/// <summary>
		/// StartDate
		/// </summary>
		public DateTime StartDateTime { get; set; }

		/// <summary>
		/// Last notification date and time
		/// </summary>
		public DateTime LastNotificationTime { get; set; }

		public bool Equals(UserDataModel other)
		{
			return this.UserUuid                      == other?.UserUuid
				&& this.LastNotificationTime          == other?.LastNotificationTime
				&& this.IsExposureNotificationEnabled == other.IsExposureNotificationEnabled;
				//&& IsNotificationEnabled == other.IsNotificationEnabled;
		}

		/// <summary>
		/// User Unique ID format UserUUID.(Padding Zero Major).(Padding Zero Minor)
		/// </summary>
		/// <value>User Minor</value>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string? GetId()
		{
			return this.UserUuid;
		}

		public int GetJumpHashTime()
		{
			return JumpHash.JumpConsistentHash(this.JumpConsistentSeed, 86400);
		}

		public string GetLocalDateString()
		{
			if (this.StartDateTime == DateTime.MinValue)
			{
				this.StartDateTime = DateTime.UtcNow;
			}

			string cultureName = CultureInfo.CurrentUICulture.ToString();
			if (cultureName.Contains("ar"))
			{
				return this.StartDateTime.ToLocalTime().ToString("D", new CultureInfo("ar-AE"));
			}
			else
			{
				return this.StartDateTime.ToLocalTime().ToString("D");
			}
		}

		public bool IsOptined { get; set; } = false;

		public bool IsExposureNotificationEnabled { get; set; } = false;

		public bool IsNotificationEnabled { get; set; } = false;

		public bool IsPositived { get; set; } = false;

		public bool IsPolicyAccepted { get; set; } = false;

		public Dictionary<string, long> LastProcessTekTimestamp { get; set; } = new Dictionary<string, long>();

		public Dictionary<string, ulong> ServerBatchNumbers { get; set; } = AppSettings.Instance.GetDefaultBatch();

		public ObservableCollection<UserExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<UserExposureInfo>();

		public UserExposureSummary? ExposureSummary { get; set; }

		// for mock
		public List<PositiveDiagnosisState> PositiveDiagnoses { get; set; } = new List<PositiveDiagnosisState>();

		public void AddDiagnosis(string diagnosisUid, DateTimeOffset submissionDate)
		{
			if (diagnosisUid is null) {
				throw new ArgumentNullException(nameof(diagnosisUid));
			}
			if (diagnosisUid.Length == 0) {
				throw new ArgumentException($"The parameter \'{nameof(diagnosisUid)}\' cannot be an empty string.", nameof(diagnosisUid));
			}
			if (this.PositiveDiagnoses.Any(d => d.DiagnosisUid.Equals(diagnosisUid, StringComparison.OrdinalIgnoreCase))) {
				return;
			}

			// Remove ones that were not submitted as the new one is better
			this.PositiveDiagnoses.Clear();
			this.PositiveDiagnoses.Add(new PositiveDiagnosisState
			{
				DiagnosisDate = submissionDate,
				DiagnosisUid  = diagnosisUid,
			});
		}

		public void ClearDiagnosis()
		{
			this.PositiveDiagnoses.Clear();
		}

		public PositiveDiagnosisState LatestDiagnosis => this.PositiveDiagnoses.OrderByDescending(p => p.DiagnosisDate).FirstOrDefault();
	}
}
