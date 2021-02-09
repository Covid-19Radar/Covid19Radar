using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Covid19Radar.Common;
using D = System.Diagnostics.Debug;

namespace Covid19Radar.Services.Logs
{
	internal enum LogLevel
	{
		Verbose,
		Debug,
		Info,
		Warning,
		Error
	}

	public class LoggerService : ILoggerService
	{
		private readonly ILogPathService    _log_path;
		private readonly IEssentialsService _essentials;

		public LoggerService(ILogPathService logPath, IEssentialsService essentials)
		{
			_log_path = logPath;
			_essentials = essentials;
		}
		
		public void StartMethod(
			[CallerMemberName()] string method    = "",
			[CallerFilePath()]   string filePath  = "",
			[CallerLineNumber()] int   lineNumber = 0)
		{
			this.Output("Start", method, filePath, lineNumber, LogLevel.Info);
		}
		
		public void EndMethod(
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			this.Output("End", method, filePath, lineNumber, LogLevel.Info);
		}
		
		public void Verbose(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			this.Output(message, method, filePath, lineNumber, LogLevel.Verbose);
		}
		
		public void Debug(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			this.Output(message, method, filePath, lineNumber, LogLevel.Debug);
		}
		
		public void Info(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			this.Output(message, method, filePath, lineNumber, LogLevel.Info);
		}
		
		public void Warning(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			this.Output(message, method, filePath, lineNumber, LogLevel.Warning);
		}
		
		public void Error(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			this.Output(message, method, filePath, lineNumber, LogLevel.Error);
		}
		
		public void Exception(
			string message, Exception exception,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			this.Output(message + ", Exception: " + exception.ToString(), method, filePath, lineNumber, LogLevel.Error);
		}

		private void Output(string message, string method, string filePath, int lineNumber, LogLevel logLevel)
		{
#if !DEBUG
			if (logLevel == LogLevel.Verbose || logLevel == LogLevel.Debug) {
				return;
			}
#endif
			try {
				lock (this) {
					var    jstNow      = Utils.JstNow();
					string logFilePath = _log_path.LogFilePath(jstNow);
					this.CreateLogsDirIfNotExists();
					this.CreateLogFileIfNotExists(logFilePath);
					string row = this.CreateLogContentRow(message, method, filePath, lineNumber, logLevel, jstNow);
					D.WriteLine(row);

					// TODO: stream pool
					using (var sw = new StreamWriter(logFilePath, true, Encoding.UTF8)) {
						sw.WriteLine(row);
					}
				}
			} catch (Exception e) {
				D.WriteLine(e.ToString());
			}
		}

		private void CreateLogsDirIfNotExists()
		{
			if (!Directory.Exists(_log_path.LogsDirPath)) {
				Directory.CreateDirectory(_log_path.LogsDirPath);
			}
		}

		private void CreateLogFileIfNotExists(string logFilePath)
		{
			if (!File.Exists(logFilePath)) {
				// TODO: stream pool
				using (var fs = File.Create(logFilePath))
				using (var sw = new StreamWriter(fs, Encoding.UTF8)) {
					sw.WriteLine(this.CreateLogHeaderRow());
				}
			}
		}

		private string CreateLogHeaderRow()
		{
			return this.CreateLogRow(
				"output_date",
				"log_level",
				"message",
				"method",
				"file_path",
				"line_number",
				"platform",
				"platform_version",
				"model",
				"device_type",
				"app_version",
				"build_number"
			);
		}

		private string CreateLogContentRow(string message, string method, string filePath, int lineNumber, LogLevel logLevel, DateTime jstDateTime)
		{
			return this.CreateLogRow(
				jstDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
				logLevel.ToString(),
				message,
				method,
				filePath,
				lineNumber.ToString(),
				_essentials.Platform,
				_essentials.PlatformVersion,
				_essentials.Model,
				_essentials.DeviceType,
				_essentials.AppVersion,
				_essentials.BuildNumber
			);
		}

		private string CreateLogRow(params string[] cols)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < cols.Length; ++i) {
				if (i != 0) {
					sb.Append(", ");
				}
				string s = cols[i];
				sb.Append('\"');
				for (int j = 0; j < s.Length; ++j) {
					char c = s[j];
					switch (c) {
					case '\r':
					case '\n':
						break;
					case '\"':
						sb.Append("\"\"");
						break;
					default:
						sb.Append(c);
						break;
					}
				}
				sb.Append('\"');
			}
			return sb.ToString();
		}
	}
}
