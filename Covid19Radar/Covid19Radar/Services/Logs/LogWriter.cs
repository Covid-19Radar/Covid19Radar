using System;
using System.IO;
using System.Text;

namespace Covid19Radar.Services.Logs
{
	public interface ILogWriter : IDisposable
	{
		public void WriteLine(string filename, string line);
	}

	public class LogWriter : ILogWriter
	{
		private readonly Encoding      _enc;
		private          string?       _path;
		private          StreamWriter? _sw;

		public LogWriter()
		{
			_enc = Encoding.UTF8;
		}

		public void WriteLine(string filename, string line)
		{
			if (filename is null) {
				throw new ArgumentNullException(nameof(filename));
			}
			if (_path != filename || _sw is null) {
				_path = filename;
				_sw?.Dispose();
				_sw = new StreamWriter(filename, true, _enc);
			}
			_sw.WriteLine(line);
		}

		public void Dispose()
		{
			_path = null;
			_sw?.Dispose();
		}
	}
}
