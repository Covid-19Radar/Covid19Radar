using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Covid19Radar.Services.Logs
{
	public record LogData
	{
		public DateTime? OutputDate      { get; }
		public LogLevel  LogLevel        { get; }
		public string    Message         { get; }
		public string    Method          { get; }
		public string    FilePath        { get; }
		public int       LineNumber      { get; }
		public string    Platform        { get; }
		public string    PlatformVersion { get; }
		public string    Model           { get; }
		public string    DeviceType      { get; }
		public string    AppVersion      { get; }
		public string    BuildNumber     { get; }

		public LogData(List<string> cols, string dtFormat)
		{
			if (cols.Count < 12) {
				cols.AddRange(new EmptyCollection(12 - cols.Count));
			}
			this.OutputDate      = DateTime.TryParseExact     (cols[0], dtFormat, null, DateTimeStyles.None, out var dt)  ? dt  : null;
			this.LogLevel        = Enum    .TryParse<LogLevel>(cols[1],                                      out var lvl) ? lvl : LogLevel.Invalid;
			this.LineNumber      = int     .TryParse          (cols[5],                                      out int num) ? num : -1;
			this.Message         = cols[ 2];
			this.Method          = cols[ 3];
			this.FilePath        = cols[ 4];
			this.Platform        = cols[ 6];
			this.PlatformVersion = cols[ 7];
			this.Model           = cols[ 8];
			this.DeviceType      = cols[ 9];
			this.AppVersion      = cols[10];
			this.BuildNumber     = cols[11];
		}

		private sealed class EmptyCollection : ICollection<string>
		{
			public int  Count      { get; }
			public bool IsReadOnly => true;

			public EmptyCollection(int count)
			{
				this.Count = count;
			}

			public void Add(string item) { }

			public bool Remove(string item)
			{
				return false;
			}

			public void Clear() { }

			public bool Contains(string item)
			{
				return string.IsNullOrEmpty(item);
			}

			public void CopyTo(string[] array, int arrayIndex)
			{
				// do nothing, fast copy
			}

			public IEnumerator<string> GetEnumerator()
			{
				int count = this.Count;
				for (int i = 0; i < count; ++i) {
					yield return string.Empty;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}
	}
}
