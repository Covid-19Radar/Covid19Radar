using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Controls;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogsPage : ContentPage
	{
		private readonly ILoggerService _logger;

		public LogsPage(ILoggerService logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.InitializeComponent();
		}

		private async void filePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			_logger.StartMethod();
			if (filePicker.SelectedItem is LogsPageViewModel.LogFileInfo lfi) {
				_logger.Info("The selected item is a log file.");
				var logs = await Task.Run(() => lfi.GetLogData());
				if (logs is not null) {
					await this.ShowLogData(logs);
				}
			} else {
				_logger.Warning("The selected item is not a log file.");
			}
			_logger.EndMethod();
		}

		private async ValueTask ShowLogData(IReadOnlyList<LogData> logs)
		{
			_logger.StartMethod();
			UserDialogs.Instance.ShowLoading(AppResources.LogsPage_Loading);
			logDataViews.Children.Clear();
			int count = logs.Count;
			for (int i = 0; i < count; ++i) {
				var logData = logs[i];
				logDataViews.Children.Add(await Task.Run(() => new LogDataView() { LogData = logData }));
				_logger.Verbose($"{i}/{count}...");
			}
			UserDialogs.Instance.HideLoading();
			_logger.EndMethod();
		}
	}
}
