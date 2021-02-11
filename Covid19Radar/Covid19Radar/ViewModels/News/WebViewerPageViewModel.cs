using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class WebViewerPageViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;
		private          string?        _url;

		public string? Url
		{
			get => _url;
			set => this.SetProperty(ref _url, value);
		}

		public WebViewerPageViewModel(ILoggerService logger)
		{
			_logger    = logger;
			this.Title = AppResources.NewsPageTitle;
		}

		public override void Initialize(INavigationParameters parameters)
		{
			_logger.StartMethod();
			base.Initialize(parameters);
			this.Url = parameters["url"].ToString();
			_logger.Info($"The URL: {_url}");
			_logger.EndMethod();
		}
	}
}
