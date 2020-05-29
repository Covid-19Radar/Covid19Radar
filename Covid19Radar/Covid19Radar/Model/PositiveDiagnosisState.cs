﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
	public class PositiveDiagnosisState
	{
		public string DiagnosisUid { get; set; }

		public DateTimeOffset DiagnosisDate { get; set; }

		public bool Shared { get; set; }
	}
}
