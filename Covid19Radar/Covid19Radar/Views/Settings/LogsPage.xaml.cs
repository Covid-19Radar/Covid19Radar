using System;
using System.Threading.Tasks;
using Covid19Radar.Controls;
using Covid19Radar.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogsPage : ContentPage
	{
		public LogsPage()
		{
			this.InitializeComponent();
		}

		private async void filePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (filePicker.SelectedItem is LogsPageViewModel.LogFileInfo lfi) {
				await Task.Run(() => {
					var logs = lfi.GetLogData();
					if (logs is not null) {
						int count = logs.Count;
						for (int i = 0; i < count; ++i) {
							var logData = logs[i];
							MainThread.BeginInvokeOnMainThread(() => {
								logDataViews.Children.Add(new LogDataView() { LogData = logData });
							});
						}
					}
				});
			}
		}
	}
}
