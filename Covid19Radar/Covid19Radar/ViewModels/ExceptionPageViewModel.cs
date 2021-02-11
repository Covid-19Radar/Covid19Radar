using Prism.Mvvm;

namespace Covid19Radar.ViewModels
{
	public class ExceptionPageViewModel : BindableBase
	{
		private string _msg;

		public string Message
		{
			get => _msg;
			set => this.SetProperty(ref _msg, value ?? string.Empty);
		}

		public ExceptionPageViewModel()
		{
			_msg = string.Empty;
		}
	}
}
