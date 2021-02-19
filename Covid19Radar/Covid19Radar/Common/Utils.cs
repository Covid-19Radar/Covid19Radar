using System;

namespace Covid19Radar.Common
{
	public static class Utils
	{
		public static DateTime JstNow()
		{
			return TimeZoneInfo.ConvertTime(DateTime.Now, JstTimeZoneInfo());
		}

		public static DateTime[] JstDateTimes(int days)
		{
			if (days <= 0) {
				return new DateTime[0];
			}
			var result = new DateTime[days];
			for (int i = 0; i < days; ++i) {
				result[i] = TimeZoneInfo.ConvertTime(DateTime.Now.AddDays(-i), JstTimeZoneInfo());
			}
			return result;
		}

		private static TimeZoneInfo JstTimeZoneInfo()
		{
			try {
				return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
			} catch (TimeZoneNotFoundException) {
				try {
					return TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
				} catch (TimeZoneNotFoundException) {
					return TimeZoneInfo.CreateCustomTimeZone("JST", new TimeSpan(9, 0, 0), "(GMT+09:00) JST", "JST");
				}
			}
		}
	}
}
