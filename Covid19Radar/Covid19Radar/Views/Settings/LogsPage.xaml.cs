using System;
using System.Threading;
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
		private CancellationTokenSource? _cts;

		public LogsPage()
		{
			this.InitializeComponent();
		}

		private async void filePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (filePicker.SelectedItem is LogsPageViewModel.LogFileInfo lfi) {
				lock (this) {
					if (_cts is not null) {
						_cts.Cancel();
					}
				}
				using (var cts = new CancellationTokenSource()) {
					lock (this) {
						_cts = cts;
					}
					var token = cts.Token;
					await Task.Run(async () => {
						token.ThrowIfCancellationRequested();
						var logs = lfi.GetLogData();
						if (logs is not null) {
							MainThread.BeginInvokeOnMainThread(() => {
								logDataViews.Children.Clear();
							});
							int count = logs.Count;
							for (int i = 0; i < count && !token.IsCancellationRequested; ++i) {
								var logData = logs[i];
								MainThread.BeginInvokeOnMainThread(() => {
									logDataViews.Children.Add(new LogDataView() { LogData = logData });
								});
								await Task.Delay(10);
							}
						}
					}, token);
					lock (this) {
						_cts = null;
					}
				}
			}
		}
	}
}
