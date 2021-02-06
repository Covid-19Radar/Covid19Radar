using System;

namespace Covid19Radar.Model
{
	public class PositiveDiagnosisState
	{
		public string         DiagnosisUid  { get; set; }
		public DateTimeOffset DiagnosisDate { get; set; }

		public PositiveDiagnosisState()
		{
			this.DiagnosisUid = string.Empty;
		}
	}
}
