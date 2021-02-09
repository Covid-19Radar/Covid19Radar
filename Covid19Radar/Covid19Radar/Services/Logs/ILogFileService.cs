namespace Covid19Radar.Services.Logs
{
	public interface ILogFileService
	{
		// Log upload
		public string CreateLogId();
		public string LogUploadingFileName(string logId);
		public bool   CreateLogUploadingFileToTmpPath(string logUploadingFileName);
		public bool   CopyLogUploadingFileToPublicPath(string logUploadingFileName);
		public bool   DeleteAllLogUploadingFiles();

		// Log rotate
		public void AddSkipBackupAttribute();
		public void Rotate();
		public bool DeleteLogsDir();
	}
}
