using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Covid19Radar.Common;

namespace Covid19Radar.Services.Logs
{
	public interface ILogReaderWriter : IDisposable
	{
		public string[]               GetFiles();
		public void                   Write(string message, string method, string filePath, int lineNumber, LogLevel logLevel);
		public IReadOnlyList<LogData> Read();
		public IReadOnlyList<LogData> Read(string filename);
	}

	public sealed class LogReaderWriter : ILogReaderWriter
	{
		private const    string             _dt_format = "yyyy/MM/dd HH:mm:ss.fffffff";
		private readonly ILogPathService    _log_path;
		private readonly IEssentialsService _essentials;
		private readonly Encoding           _enc;
		private          string?            _path;
		private          FileStream?        _fs;
		private          StreamWriter?      _sw;
		private readonly string             _header;
		private readonly EnumerationOptions _eo;

		public LogReaderWriter(ILogPathService logPath, IEssentialsService essentials)
		{
			_log_path   = logPath;
			_essentials = essentials;
			_enc        = Encoding.UTF8;
			_header     = CreateLogHeaderRow();
			_eo         = new() {
				AttributesToSkip         = FileAttributes.Hidden | FileAttributes.System,
				BufferSize               = 0,
				IgnoreInaccessible       = true,
				MatchCasing              = MatchCasing.PlatformDefault,
				MatchType                = MatchType.Win32,
				RecurseSubdirectories    = false,
				ReturnSpecialDirectories = false
			};
		}

		public string[] GetFiles()
		{
			string dir = _log_path.LogsDirPath;
			if (Directory.Exists(dir)) {
				return Directory.GetFiles(dir, _log_path.LogFileWildcardName, _eo);
			} else {
				return Array.Empty<string>();
			}
		}

		public void Write(string message, string method, string filePath, int lineNumber, LogLevel logLevel)
		{
#if !DEBUG
			if (logLevel == LogLevel.Verbose || logLevel == LogLevel.Debug) {
				return;
			}
#endif
			try {
				var    jstNow = Utils.JstNow();
				string row    = CreateLogContentRow(message, method, filePath, lineNumber, logLevel, jstNow, _essentials);
				Debug.WriteLine(row);
				this.WriteLine(jstNow, row);
			} catch (Exception e) {
				Debug.WriteLine(e.ToString());
			}
		}

		private void WriteLine(DateTime jstNow, string line)
		{
			string filename = _log_path.LogFilePath(jstNow);
			lock (this) {
				if (_path != filename || _sw is null) {
					_path = filename;
					_sw?.Dispose();
					_fs?.Dispose();
					if (!Directory.Exists(_log_path.LogsDirPath)) {
						Directory.CreateDirectory(_log_path.LogsDirPath);
					}
					_fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Read);
					_sw = new StreamWriter(_fs, _enc);
					_sw.AutoFlush = true;
					_sw.WriteLine(_header);
				}
				_sw.WriteLine(line);
			}
		}

		public IReadOnlyList<LogData> Read()
		{
			string? path;
			lock (this) {
				path = _path;
			}
			if (path is null) {
				return Array.Empty<LogData>();
			} else {
				return this.Read(path);
			}
		}

		public IReadOnlyList<LogData> Read(string filename)
		{
			var result = new List<LogData>();
			using (var sr = new StreamReader(filename, _enc, true)) {
				string? line;
				while ((line = sr.ReadLine()) is not null) {
					result.Add(ParseRow(line));
				}
			}
			return result.AsReadOnly();
		}

		public void Dispose()
		{
			lock (this) {
				_path = null;
				_sw?.Dispose();
				_fs?.Dispose();
			}
		}

		private static string CreateLogHeaderRow()
		{
			return CreateLogRow(
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

		private static string CreateLogContentRow(
			string             message,
			string             method,
			string             filePath,
			int                lineNumber,
			LogLevel           logLevel,
			DateTime           jstDateTime,
			IEssentialsService essentials)
		{
			return CreateLogRow(
				jstDateTime.ToString(_dt_format),
				logLevel.ToString(),
				message,
				method,
				filePath,
				lineNumber.ToString(),
				essentials.Platform,
				essentials.PlatformVersion,
				essentials.Model,
				essentials.DeviceType,
				essentials.AppVersion,
				essentials.BuildNumber
			);
		}

		private static string CreateLogRow(params string[] cols)
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
					case '\0':
						break;
					case '\r':
						sb.Append("\\r");
						break;
					case '\n':
						sb.Append("\\n");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					case '\\':
					case '\"':
						sb.Append(c);
						sb.Append(c);
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

		private static LogData ParseRow(string line)
		{
			var  cols = new List<string>(12);
			var  sb   = new StringBuilder();
			bool span = false;
			bool dq   = false;
			for (int i = 0; i < line.Length; ++i) {
				char c = line[i];
				if (dq) {
					dq = false;
					if (c == '\"') {
						sb.Append(c);
						continue;
					} else {
						span = !span;
					}
				}
				switch (c) {
				case ' ':
				case '\t':
				case '\n': // 通常は出現しない
				case '\r': // 通常は出現しない
					if (span) {
						sb.Append(c);
					}
					break;
				case '\"':
					dq = true;
					break;
				case '\\':
					++i;
					if (i < line.Length) {
						sb.Append(line[i] switch {
							'\\' => '\\',
							't'  => '\t',
							'n'  => '\n',
							'r'  => '\r',
							_    => '\x7F' // (DEL)
						});;
					}
					break;
				case ',':
					if (span) {
						sb.Append(c);
					} else {
						cols.Add(sb.ToString());
						sb.Clear();
					}
					break;
				default:
					sb.Append(c);
					break;
				}
			}
			cols.Add(sb.ToString());
			return new(cols, _dt_format);
		}
	}
}
