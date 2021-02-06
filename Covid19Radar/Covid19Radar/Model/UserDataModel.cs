using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Covid19Radar.Common;
using Xamarin.ExposureNotifications;

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

		public bool                               IsOptined                     { get; set; }
		public bool                               IsExposureNotificationEnabled { get; set; }
		public bool                               IsNotificationEnabled         { get; set; }
		public bool                               IsPositived                   { get; set; }
		public bool                               IsPolicyAccepted              { get; set; }
		public Dictionary<string, long>           LastProcessTekTimestamp       { get; set; }
		public Dictionary<string, ulong>          ServerBatchNumbers            { get; set; }
		public ObservableCollection<ExposureInfo> ExposureInformation           { get; set; }
		public ExposureDetectionSummary?          ExposureSummary               { get; set; }
		public List<PositiveDiagnosisState>       PositiveDiagnoses             { get; set; }

		public PositiveDiagnosisState LatestDiagnosis => this.PositiveDiagnoses.OrderByDescending(p => p.DiagnosisDate).FirstOrDefault();

		public UserDataModel()
		{
			this.IsOptined                     = false;
			this.IsExposureNotificationEnabled = false;
			this.IsNotificationEnabled         = false;
			this.IsPositived                   = false;
			this.IsPolicyAccepted              = false;
			this.LastProcessTekTimestamp       = new Dictionary<string, long>();
			this.ServerBatchNumbers            = AppSettings.Instance.GetDefaultBatch();
			this.ExposureInformation           = new ObservableCollection<ExposureInfo>();
			this.PositiveDiagnoses             = new List<PositiveDiagnosisState>(); // for mock
		}

		public bool Equals(UserDataModel other)
		{
			return this.UserUuid                      == other?.UserUuid
				&& this.LastNotificationTime          == other?.LastNotificationTime
				&& this.IsExposureNotificationEnabled == other.IsExposureNotificationEnabled;
				// && this.IsNotificationEnabled         == other.IsNotificationEnabled;
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
			if (this.StartDateTime == DateTime.MinValue) {
				this.StartDateTime = DateTime.UtcNow;
			}
			return this.StartDateTime.ToLocalTime().ToString("D", CultureInfo.CurrentUICulture);
			/*
			if (CultureInfo.CurrentUICulture.Name.Contains("ar")) {
				return this.StartDateTime.ToLocalTime().ToString("D", new CultureInfo("ar-AE"));
			} else {
				return this.StartDateTime.ToLocalTime().ToString("D");
			}
			//*/
		}

		public void AddDiagnosis(string diagnosisUid, DateTimeOffset submissionDate)
		{
			if (diagnosisUid is null) {
				throw new ArgumentNullException(nameof(diagnosisUid));
			}
			if (diagnosisUid.Length == 0) {
				throw new ArgumentException($"The parameter \'{nameof(diagnosisUid)}\' cannot be an empty string.", nameof(diagnosisUid));
			}
			if (this.PositiveDiagnoses.Any(d => d.DiagnosisUid?.Equals(diagnosisUid, StringComparison.OrdinalIgnoreCase) ?? false)) {
				return;
			}

			// Remove ones that were not submitted as the new one is better
			this.PositiveDiagnoses.Clear();
			this.PositiveDiagnoses.Add(new PositiveDiagnosisState
			{
				DiagnosisUid  = diagnosisUid,
				DiagnosisDate = submissionDate,
			});
		}

		public void ClearDiagnosis()
		{
			this.PositiveDiagnoses.Clear();
		}
	}
}
