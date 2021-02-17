using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogDataView : ContentView
	{
		private LogData? _log_data;

		public static readonly BindableProperty LogDataProperty = BindableProperty.Create(
			nameof(LogData),
			typeof(LogData),
			typeof(LogDataView),
			null,
			propertyChanged: (bindable, oldValue, newValue) => {
				if (bindable is LogDataView ldv && newValue is LogData ld) {
					ldv.LogData = ld;
				}
			},
			defaultBindingMode: BindingMode.TwoWay
		);

		public LogData? LogData
		{
			get => _log_data;
			set
			{
				_log_data       = value;
				if (_log_data is not null) {
					if (_log_data.OutputDate.HasValue) {
						outputDate.Text = _log_data.OutputDate.Value.ToString("yyyy/MM/dd HH:mm:ss.fffffff");
					} else {
						outputDate.Text = AppResources.LogDataView_OutputDate_Unknown;
					}
					switch (_log_data.LogLevel) {
					case LogLevel.Verbose:
						logLevel.Text      = AppResources.LogDataView_LogLevel_Verbose;
						logLevel.TextColor = ((Color)(this.Resources["Verbose"]));
						break;
					case LogLevel.Debug:
						logLevel.Text      = AppResources.LogDataView_LogLevel_Debug;
						logLevel.TextColor = ((Color)(this.Resources["Debug"]));
						break;
					case LogLevel.Info:
						logLevel.Text      = AppResources.LogDataView_LogLevel_Info;
						logLevel.TextColor = ((Color)(this.Resources["Info"]));
						break;
					case LogLevel.Warning:
						logLevel.Text      = AppResources.LogDataView_LogLevel_Warning;
						logLevel.TextColor = ((Color)(this.Resources["Warning"]));
						break;
					case LogLevel.Error:
						logLevel.Text      = AppResources.LogDataView_LogLevel_Error;
						logLevel.TextColor = ((Color)(this.Resources["Error"]));
						break;
					default:
						logLevel.Text      = AppResources.LogDataView_LogLevel_Invalid;
						logLevel.TextColor = ((Color)(this.Resources["Invalid"]));
						break;
					}
					method    .Text = _log_data.Method;
					filePath  .Text = _log_data.FilePath;
					lineNumber.Text = _log_data.LineNumber.ToString();
					message   .Text = _log_data.Message;
				}
			}
		}

		public LogDataView()
		{
			this.InitializeComponent();
		}
	}
}
