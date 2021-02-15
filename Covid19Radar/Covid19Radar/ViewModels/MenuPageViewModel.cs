using System;
using System.Collections.ObjectModel;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
	public class MenuPageViewModel : ViewModelBase
	{
		private readonly ILoggerService     _logger;
		private readonly INavigationService _ns;
		private          MainMenuModel      _selected_menu_item;

		public ObservableCollection<MainMenuModel> MenuItems { get; set; }

		public MainMenuModel SelectedMenuItem
		{
			get => _selected_menu_item;
			set => this.SetProperty(ref _selected_menu_item, value);
		}

		public DelegateCommand NavigateCommand { get; private set; }

		public MenuPageViewModel(ILoggerService logger, INavigationService navigationService)
		{
			_logger = logger            ?? throw new ArgumentNullException(nameof(logger));
			_ns     = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			_logger.StartMethod();
			_logger.Info("Loading menu items...");
			_selected_menu_item = new MainMenuModel() {
				Icon     = "\uF965",
				PageName = nameof(HomePage),
				Title    = AppResources.HomePageTitle
			};
			this.MenuItems = new ObservableCollection<MainMenuModel>();
			this.MenuItems.Add(_selected_menu_item);
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(NewsPage),
				Title    = AppResources.NewsPageTitle
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF013",
				PageName = nameof(SettingsPage),
				Title    = AppResources.SettingsPageTitle
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0E0",
				PageName = nameof(InqueryPage),
				Title    = AppResources.InqueryPageTitle
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF70E",
				PageName = nameof(TermsofservicePage),
				Title    = AppResources.TermsofservicePageTitle
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF70E",
				PageName = nameof(PrivacyPolicyPage2),
				Title    = AppResources.PrivacyPolicyPageTitle
			});
#if DEBUG
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(DebugPage),
				Title    = nameof(DebugPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF70E",
				PageName = nameof(LicenseAgreementPage),
				Title    = AppResources.TitleLicenseAgreement
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(ThankYouNotifyOtherPage),
				Title    = nameof(ThankYouNotifyOtherPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(NotContactPage),
				Title    = nameof(NotContactPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(ContactedNotifyPage),
				Title    = nameof(ContactedNotifyPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF70E",
				PageName = nameof(PrivacyPolicyPage),
				Title    = nameof(PrivacyPolicyPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF70E",
				PageName = nameof(PrivacyPolicyPage2),
				Title    = nameof(PrivacyPolicyPage2)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage1),
				Title    = nameof(TutorialPage1)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage2),
				Title    = nameof(TutorialPage2)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage3),
				Title    = nameof(TutorialPage3)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage4),
				Title    = nameof(TutorialPage4)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage5),
				Title    = nameof(TutorialPage5)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage6),
				Title    = nameof(TutorialPage6)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(HelpMenuPage),
				Title    = nameof(HelpMenuPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(HelpPage1),
				Title    = nameof(HelpPage1)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(HelpPage2),
				Title    = nameof(HelpPage2)
			});
			this.MenuItems.Add(new MainMenuModel()  {
				Icon     = "\uF0C0",
				PageName = nameof(HelpPage3),
				Title    = nameof(HelpPage3)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(HelpPage4),
				Title    = nameof(HelpPage4)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(ChatbotPage),
				Title    = nameof(ChatbotPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(NotifyOtherPage),
				Title    = nameof(NotifyOtherPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(SubmitConsentPage),
				Title    = nameof(SubmitConsentPage)
			});
			this.MenuItems.Add(new MainMenuModel() {
				Icon     = "\uF0C0",
				PageName = nameof(WebViewerPage),
				Title    = nameof(WebViewerPage)
			});
#endif
			_logger.Info("Loaded menu items");
			this.ClearMenuItemsColors();
			this.NavigateCommand = new DelegateCommand(this.Navigate);
			_logger.StartMethod();
		}

		private async void Navigate()
		{
			_logger.StartMethod();
			this.ClearMenuItemsColors();
			_selected_menu_item.IconColor = "#FFFFFF";
			_selected_menu_item.TextColor = "#FFFFFF";
			await _ns.NavigateAsync(nameof(NavigationPage) + "/" + _selected_menu_item.PageName);
			_logger.EndMethod();
		}

		private void ClearMenuItemsColors()
		{
			_logger.StartMethod();
			int count = this.MenuItems.Count;
			for (int i = 0; i < count; ++i) {
				var item = this.MenuItems[i];
				item.IconColor = "#019AE8";
				item.TextColor = "#000000";
			}
			_logger.EndMethod();
		}
	}
}
