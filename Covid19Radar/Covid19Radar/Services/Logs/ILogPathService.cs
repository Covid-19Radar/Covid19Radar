using System;

namespace Covid19Radar.Services.Logs
{
	public interface ILogPathService
	{
		// Logger
		public string    LogsDirPath         { get; }
		public string    LogFileWildcardName { get; }
		public string    LogFilePath(DateTime jstDateTime);
		public DateTime? GetDate    (string   filename);

		// Log upload
		public string LogUploadingTmpPath          { get; }
		public string LogUploadingPublicPath       { get; }
		public string LogUploadingFileWildcardName { get; }
	}
}
