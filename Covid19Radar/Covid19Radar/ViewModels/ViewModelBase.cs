using Covid19Radar.Services;
using Prism.Mvvm;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
	{
		private string _title;

		public string Title
		{
			get => _title;
			set => this.SetProperty(ref _title, value ?? string.Empty);
		}

		public ViewModelBase()
		{
			_title = string.Empty;
		}

		public virtual void Initialize     (INavigationParameters parameters) { }
		public virtual void OnNavigatedFrom(INavigationParameters parameters) { }
		public virtual void OnNavigatedTo  (INavigationParameters parameters) { }
		public virtual void Destroy        ()                                 { }
	}
}
