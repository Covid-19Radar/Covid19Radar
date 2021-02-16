using System;
using System.IO;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.iOS.Services.Logs
{
	public class LogPathServiceIos : ILogPathDependencyService
	{
		private static readonly string _logs_dir_name = "Logs";
		private        readonly string _documents_path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public string LogsDirPath
		{
			get
			{
				string libraryPath            = Path.Combine(_documents_path, "..", "Library");
				string applicationSupportPath = Path.Combine(libraryPath, "Application Support");
				return Path.Combine(applicationSupportPath, _logs_dir_name);
			}
		}

		public string LogUploadingTmpPath    => Path.Combine(_documents_path, "..", "tmp");
		public string LogUploadingPublicPath => _documents_path;

		public LogPathServiceIos() { }
	}
}
