using System;

namespace Covid19Radar.Model
{
	public class UserExposureSummary
	{
		public int        DaysSinceLastExposure { get; }
		public ulong      MatchedKeyCount       { get; }
		public int        HighestRiskScore      { get; }
		public TimeSpan[] AttenuationDurations  { get; }
		public int        SummationRiskScore    { get; }

		public UserExposureSummary(int daysSinceLastExposure, ulong matchedKeyCount, int highestRiskScore, TimeSpan[] attenuationDurations, int summationRiskScore)
		{
			this.DaysSinceLastExposure = daysSinceLastExposure;
			this.MatchedKeyCount       = matchedKeyCount;
			this.HighestRiskScore      = highestRiskScore;
			this.AttenuationDurations  = attenuationDurations;
			this.SummationRiskScore    = summationRiskScore;
		}
	}
}
