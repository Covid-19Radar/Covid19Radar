using System.Collections.ObjectModel;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Commands;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
	public class HelpMenuPageViewModel : ViewModelBase
	{
		private readonly ILoggerService _logger;
		private          MainMenuModel? _selected_menuitem;

		public ObservableCollection<MainMenuModel> MenuItems { get; set; }

		public  MainMenuModel? SelectedMenuItem
		{
			get => _selected_menuitem;
			set => this.SetProperty(ref _selected_menuitem, value);
		}

		public DelegateCommand NavigateCommand { get; private set; }

		public HelpMenuPageViewModel(INavigationService navigationService, ILoggerService logger) : base(navigationService)
		{
			_logger = logger;
			_logger.StartMethod();
			_logger.Info("Loading menu items...");
			this.Title = AppResources.HelpMenuPageTitle;
			this.MenuItems = new ObservableCollection<MainMenuModel>();
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF105",
				PageName = nameof(HelpPage1),
				Title    = AppResources.HelpMenuPageLabel1
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF105",
				PageName = nameof(HelpPage2),
				Title    = AppResources.HelpMenuPageLabel2
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF105",
				PageName = nameof(HelpPage3),
				Title    = AppResources.HelpMenuPageLabel3
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF105",
				PageName = nameof(HelpPage4),
				Title    = AppResources.HelpMenuPageLabel4
			});
			_logger.Info("Loaded menu items");
			this.NavigateCommand = new DelegateCommand(this.Navigate);
			_logger.EndMethod();
		}

		private async void Navigate()
		{
			_logger.StartMethod();
			if (this.NavigationService is null) {
				_logger.Warning("Could not access to the navigation service.");
			} else {
				await this.NavigationService.NavigateAsync(this.SelectedMenuItem?.PageName);
			}
			this.SelectedMenuItem = null;
			_logger.EndMethod();
		}
	}
}
