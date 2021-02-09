using System;
using System.IO;
using System.IO.Compression;
using Covid19Radar.Common;
using Xamarin.Forms;

namespace Covid19Radar.Services.Logs
{
	public class LogFileService : ILogFileService
	{
		private readonly ILoggerService  _logger;
		private readonly ILogPathService _log_path;

		public LogFileService(ILoggerService logger, ILogPathService logPath)
		{
			_logger   = logger;
			_log_path = logPath;
		}

		public string CreateLogId()
		{
			return Guid.NewGuid().ToString();
		}

		public string LogUploadingFileName(string logId)
		{
			return $"{LogPathService.PREFIX_UPLOADING}{logId}.{LogPathService.EXTENSION_UPLOADING}";
		}

		public bool CreateLogUploadingFileToTmpPath(string logUploadingFileName)
		{
			_logger.StartMethod();
			try {
				string   logsDirPath = _log_path.LogsDirPath;
				string[] logFiles    = Directory.GetFiles(logsDirPath, _log_path.LogFileWildcardName);
				if (logFiles.Length == 0) {
					_logger.EndMethod();
					return false;
				}
				ZipFile.CreateFromDirectory(logsDirPath, Path.Combine(_log_path.LogUploadingTmpPath, logUploadingFileName));
				_logger.EndMethod();
				return true;
			} catch (Exception e) {
				_logger.Exception("Failed to create the uploading file.", e);
				_logger.EndMethod();
				return false;
			}
		}

		public bool CopyLogUploadingFileToPublicPath(string logUploadingFileName)
		{
			_logger.StartMethod();
			try {
				string tmpPath    = _log_path.LogUploadingTmpPath;
				string publicPath = _log_path.LogUploadingPublicPath;
				if (string.IsNullOrEmpty(tmpPath) || string.IsNullOrEmpty(publicPath)) {
					_logger.EndMethod();
					return false;
				}
				File.Copy(Path.Combine(tmpPath, logUploadingFileName), Path.Combine(publicPath, logUploadingFileName), true);
				_logger.EndMethod();
				return true;
			} catch (Exception e) {
				_logger.Exception("Failed to copy the log file.", e);
				_logger.EndMethod();
				return false;
			}
		}

		public bool DeleteAllLogUploadingFiles()
		{
			_logger.StartMethod();
			try {
				string tmpPath = _log_path.LogUploadingTmpPath;
				if (string.IsNullOrEmpty(tmpPath)) {
					_logger.EndMethod();
					return false;
				}
				string[] uploadingFiles = Directory.GetFiles(tmpPath, _log_path.LogUploadingFileWildcardName);
				for (int i = 0; i < uploadingFiles.Length; ++i) {
					File.Delete(uploadingFiles[i]);
				}
				/* TODO: rewrite
				switch (uploadingFiles.Length) {
				case 1:
					_logger.Info($"Deleted one file.");
					break;
				case > 0:
					_logger.Info($"Deleted {uploadingFiles.Length} files.");
					break;
				}
				//*/
				if (uploadingFiles.Length == 1) {
					_logger.Info($"Deleted one file.");
				} else if (uploadingFiles.Length > 0) {
					_logger.Info($"Deleted {uploadingFiles.Length} files.");
				}
				_logger.EndMethod();
				return true;
			} catch (Exception e) {
				_logger.Exception("Failed to delete uploading files", e);
				_logger.EndMethod();
				return false;
			}
		}

		public void AddSkipBackupAttribute()
		{
			DependencyService.Get<ILogFileDependencyService>().AddSkipBackupAttribute();
		}

		public void Rotate()
		{
			_logger.StartMethod();
			try {
				var      dateTimes   = Utils.JstDateTimes(14);
				string   logsDirPath = _log_path.LogsDirPath;
				string[] logFiles    = Directory.GetFiles(logsDirPath, _log_path.LogFileWildcardName);
				for (int i = 0; i < logFiles.Length; ++i) {
					string fname = logFiles[i];
					if (this.ShouldDeleteFile(dateTimes, fname)) {
						File.Delete(fname);
						_logger.Info($"Deleted '{Path.GetFileName(fname)}'");
					}
				}
			} catch (Exception e) {
				_logger.Exception("Failed to rotate log files.", e);
			}
			_logger.EndMethod();
		}

		public bool DeleteLogsDir()
		{
			_logger.StartMethod();
			try {
				Directory.Delete(_log_path.LogsDirPath, true);
				_logger.Info("Deleted all log files.");
				_logger.EndMethod();
				return true;
			} catch (Exception e) {
				_logger.Exception("Failed to delete all log files.", e);
				_logger.EndMethod();
				return false;
			}
		}

		private bool ShouldDeleteFile(DateTime[] dateTimes, string fileName)
		{
			for (int i = 0; i < dateTimes.Length; ++i) {
				if (fileName.Contains(dateTimes[i].ToString("yyyyMMdd"))) {
					return false;
				}
			}
			return true;
		}
	}
}
