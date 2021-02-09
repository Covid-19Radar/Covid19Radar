using System;
using System.Runtime.CompilerServices;

namespace Covid19Radar.Services.Logs
{
	public interface ILoggerService
	{
		public void StartMethod(
			[CallerMemberName()] string method    = "",
			[CallerFilePath()]   string filePath  = "",
			[CallerLineNumber()] int   lineNumber = 0);

		public void EndMethod(
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0);

		public void Verbose(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0);

		public void Debug(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0);

		public void Info(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0);

		public void Warning(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0);

		public void Error(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0);

		public void Exception(
			string message, Exception exception,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0);
	}
}
