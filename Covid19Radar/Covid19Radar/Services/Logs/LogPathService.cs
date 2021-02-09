using System;
using System.IO;

namespace Covid19Radar.Services.Logs
{
	public class LogPathService : ILogPathService
	{
		private  const    string                    PREFIX                       = "cocoa_log_";
		private  const    string                    EXTENSION                    = "csv";
		internal const    string                    PREFIX_UPLOADING             = PREFIX;
		internal const    string                    EXTENSION_UPLOADING          = "zip";
		public            string                    LogsDirPath                  => _log_path_dep.LogsDirPath;
		public            string                    LogFileWildcardName          => PREFIX + "*." + EXTENSION;
		public            string                    LogUploadingTmpPath          => _log_path_dep.LogUploadingTmpPath;
		public            string                    LogUploadingPublicPath       => _log_path_dep.LogUploadingPublicPath;
		public            string                    LogUploadingFileWildcardName => PREFIX_UPLOADING + "*." + EXTENSION_UPLOADING;
		private  readonly ILogPathDependencyService _log_path_dep;

		public LogPathService(ILogPathDependencyService logPathDependency)
		{
			_log_path_dep = logPathDependency;
		}

		public string LogFilePath(DateTime jstDateTime)
		{
			return Path.Combine(this.LogsDirPath, $"{PREFIX}{jstDateTime:yyyyMMdd}.{EXTENSION}");
		}
	}
}
