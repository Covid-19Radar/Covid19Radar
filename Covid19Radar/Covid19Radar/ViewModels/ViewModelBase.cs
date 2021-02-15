using Covid19Radar.Services;
using Prism.Mvvm;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
	{
		private string _title;

		protected INavigationService?          NavigationService           { get; }
		protected ExposureNotificationService? ExposureNotificationService { get; }

		public string Title
		{
			get => _title;
			set => this.SetProperty(ref _title, value ?? string.Empty);
		}

		public ViewModelBase()
		{
			_title = string.Empty;
		}

		public ViewModelBase(INavigationService? navigationService)
		{
			_title                 = string.Empty;
			this.NavigationService = navigationService;
		}

		public ViewModelBase(ExposureNotificationService? exposureNotificationService)
		{
			_title                           = string.Empty;
			this.ExposureNotificationService = exposureNotificationService;
		}

		public ViewModelBase(INavigationService? navigationService, ExposureNotificationService? exposureNotificationService)
		{
			_title                           = string.Empty;
			this.NavigationService           = navigationService;
			this.ExposureNotificationService = exposureNotificationService;
		}

		public virtual void Initialize     (INavigationParameters parameters) { }
		public virtual void OnNavigatedFrom(INavigationParameters parameters) { }
		public virtual void OnNavigatedTo  (INavigationParameters parameters) { }
		public virtual void Destroy        ()                                 { }
	}
}
