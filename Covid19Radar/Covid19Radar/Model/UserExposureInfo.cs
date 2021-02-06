using System;

namespace Covid19Radar.Model
{
	public class UserExposureInfo
	{
		/// <summary>
		///  When the contact occurred
		/// </summary>
		public DateTime Timestamp { get; }

		/// <summary>
		///  How long the contact lasted in 5 min increments
		/// </summary>
		public TimeSpan Duration { get; }

		public int           AttenuationValue      { get; }
		public int           TotalRiskScore        { get; }
		public UserRiskLevel TransmissionRiskLevel { get; }

		public UserExposureInfo(DateTime timestamp, TimeSpan duration, int attenuationValue, int totalRiskScore, UserRiskLevel riskLevel)
		{
			this.Timestamp             = timestamp;
			this.Duration              = duration;
			this.AttenuationValue      = attenuationValue;
			this.TotalRiskScore        = totalRiskScore;
			this.TransmissionRiskLevel = riskLevel;
		}
	}

	public enum UserRiskLevel
	{
		Invalid    = 0,
		Lowest     = 1,
		Low        = 2,
		MediumLow  = 3,
		Medium     = 4,
		MediumHigh = 5,
		High       = 6,
		VeryHigh   = 7,
		Highest    = 8
	}
}
