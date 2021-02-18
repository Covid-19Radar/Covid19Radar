using System;
using System.Globalization;
using System.IO;

namespace Covid19Radar.Services.Logs
{
	public class LogPathService : ILogPathService
	{
		private  const    string                    PREFIX                       =  "cocoa_log_";
		private  const    string                    EXTENSION                    =  "csv";
		internal const    string                    PREFIX_UPLOADING             =  PREFIX;
		internal const    string                    EXTENSION_UPLOADING          =  "zip";
		public            string                    LogsDirPath                  => _log_path_dep.LogsDirPath;
		public            string                    LogFileWildcardName          => PREFIX + "*." + EXTENSION;
		public            string                    LogUploadingTmpPath          => _log_path_dep.LogUploadingTmpPath;
		public            string                    LogUploadingPublicPath       => _log_path_dep.LogUploadingPublicPath;
		public            string                    LogUploadingFileWildcardName => PREFIX_UPLOADING + "*." + EXTENSION_UPLOADING;
		private  readonly ILogPathDependencyService _log_path_dep;
		private  readonly string[]                  _dt_formats;
		private  readonly Range                     _dt_pos;

		public LogPathService(ILogPathDependencyService logPathDependency)
		{
			_log_path_dep = logPathDependency;
			_dt_formats   = new[] { "yyyyMMdd", "yyyyMMddHH" };
			_dt_pos       = PREFIX.Length..^(EXTENSION.Length + 1);
		}

		public string LogFilePath(DateTime jstDateTime)
		{
			return Path.Combine(this.LogsDirPath, $"{PREFIX}{jstDateTime:yyyyMMddHH}.{EXTENSION}");
		}

		public DateTime? GetDate(string filename)
		{
			filename = Path.GetFileName(filename);
			if (filename.StartsWith(PREFIX) && filename.EndsWith(EXTENSION) &&
				DateTime.TryParseExact(filename[_dt_pos], _dt_formats, null, DateTimeStyles.None, out var dt)) {
				return dt;
			} else {
				return null;
			}
		}
	}
}
