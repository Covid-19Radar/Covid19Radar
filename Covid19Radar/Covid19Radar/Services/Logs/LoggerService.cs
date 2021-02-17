using System;
using System.Runtime.CompilerServices;

namespace Covid19Radar.Services.Logs
{
	public enum LogLevel
	{
		Verbose,
		Debug,
		Info,
		Warning,
		Error,
		Invalid
	}

	public class LoggerService : ILoggerService
	{
		private readonly ILogReaderWriter _writer;

		public LoggerService(ILogReaderWriter writer)
		{
			_writer = writer;
		}
		
		public void StartMethod(
			[CallerMemberName()] string method    = "",
			[CallerFilePath()]   string filePath  = "",
			[CallerLineNumber()] int   lineNumber = 0)
		{
			_writer.Write("Start", method, filePath, lineNumber, LogLevel.Info);
		}
		
		public void EndMethod(
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			_writer.Write("End", method, filePath, lineNumber, LogLevel.Info);
		}
		
		public void Verbose(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			_writer.Write(message, method, filePath, lineNumber, LogLevel.Verbose);
		}
		
		public void Debug(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			_writer.Write(message, method, filePath, lineNumber, LogLevel.Debug);
		}
		
		public void Info(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			_writer.Write(message, method, filePath, lineNumber, LogLevel.Info);
		}
		
		public void Warning(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			_writer.Write(message, method, filePath, lineNumber, LogLevel.Warning);
		}
		
		public void Error(
			string message,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			_writer.Write(message, method, filePath, lineNumber, LogLevel.Error);
		}
		
		public void Exception(
			string message, Exception exception,
			[CallerMemberName()] string method     = "",
			[CallerFilePath()]   string filePath   = "",
			[CallerLineNumber()] int    lineNumber = 0)
		{
			_writer.Write(message + ", Exception: " + exception.ToString(), method, filePath, lineNumber, LogLevel.Error);
		}
	}
}
