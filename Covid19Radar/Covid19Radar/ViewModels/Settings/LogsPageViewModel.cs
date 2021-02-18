using System;
using System.Collections.Generic;
using System.IO;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class LogsPageViewModel : ViewModelBase
	{
		private readonly ILoggerService   _logger;
		private readonly ILogPathService  _log_path;
		private readonly ILogReaderWriter _reader;
		private          string           _picker_title;
		private          LogFileInfo[]?   _files;

		public string PickerTitle
		{
			get => _picker_title;
			set => this.SetProperty(ref _picker_title, value ?? string.Empty);
		}

		public LogFileInfo[] Files
		{
			get => _files ?? Array.Empty<LogFileInfo>();
			set => this.SetProperty(ref _files, value);
		}

		public LogsPageViewModel(ILoggerService logger, ILogPathService logPath, ILogReaderWriter reader)
		{
			_logger       = logger  ?? throw new ArgumentNullException(nameof(logger));
			_log_path     = logPath ?? throw new ArgumentNullException(nameof(logPath));
			_reader       = reader  ?? throw new ArgumentNullException(nameof(reader));
			_picker_title = AppResources.LogsPage_Picker;
			this.Title    = AppResources.LogsPageTitle;
		}

		public override void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			this.RaisePropertyChanged(nameof(this.PickerTitle));

			// ファイル一覧読み込み
			string[] fileNames = _reader.GetFiles();
			var      files     = new LogFileInfo[fileNames.Length];
			for (int i = 0; i < fileNames.Length; ++i) {
				files[i] = new(this, fileNames[i]);
			}
			this.Files = files;

			_logger.EndMethod();
		}

		public readonly struct LogFileInfo
		{
			private readonly LogsPageViewModel _owner;
			private readonly string            _fname;

			internal LogFileInfo(LogsPageViewModel owner, string fname)
			{
				_owner = owner;
				_fname = fname;
			}

			public IReadOnlyList<LogData>? GetLogData()
			{
				_owner._logger.StartMethod();
				IReadOnlyList<LogData>? result = null;
				try {
					result = _owner._reader.Read(_fname);
				} catch (Exception e) {
					_owner._logger.Exception($"Failed to read the log file \"{_fname}\".", e);
				}
				_owner._logger.EndMethod();
				return result;
			}

			public override string ToString()
			{
				var dt = _owner._log_path.GetDate(_fname);
				if (dt.HasValue) {
					return $"{dt.Value.ToLongDateString()} - {dt.Value.Hour:D02}";
				} else {
					return string.Format(AppResources.LogsPage_UnknownFile, Path.GetFileName(_fname));
				}
			}
		}
	}
}
