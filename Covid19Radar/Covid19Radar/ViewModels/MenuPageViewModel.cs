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
			_selected_menu_item = new() {
				Icon     = "\uF965",
				PageName = nameof(HomePage),
				Title    = AppResources.HomePageTitle
			};
			this.MenuItems = new();
			this.MenuItems.Add(_selected_menu_item);
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(NewsPage),
				Title    = AppResources.NewsPageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF013",
				PageName = nameof(SettingsPage),
				Title    = AppResources.SettingsPageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0E0",
				PageName = nameof(InqueryPage),
				Title    = AppResources.InqueryPageTitle_Menu
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF70E",
				PageName = nameof(TermsofservicePage),
				Title    = AppResources.TermsofservicePageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF70E",
				PageName = nameof(PrivacyPolicyPage2),
				Title    = AppResources.PrivacyPolicyPageTitle
			});
#if DEBUG
			this.MenuItems.Add(new()); // 区切り用
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(DebugPage),
				Title    = nameof(DebugPage)
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF70E",
				PageName = nameof(LicenseAgreementPage),
				Title    = AppResources.TitleLicenseAgreement
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(ThankYouNotifyOtherPage),
				Title    = nameof(ThankYouNotifyOtherPage)
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(NotContactPage),
				Title    = AppResources.NotContactPageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(ContactedNotifyPage),
				Title    = AppResources.ContactedNotifyPageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF70E",
				PageName = nameof(PrivacyPolicyPage),
				Title    = AppResources.PrivacyPolicyPageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF70E",
				PageName = nameof(PrivacyPolicyPage2),
				Title    = AppResources.PrivacyPolicyPageTitle + " 2"
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage1),
				Title    = AppResources.TutorialPage1Title1
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage2),
				Title    = AppResources.TutorialPage2Title
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage3),
				Title    = AppResources.TutorialPage3Title
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage4),
				Title    = AppResources.TutorialPage4Title1
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage5),
				Title    = AppResources.TutorialPage5Title
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(TutorialPage6),
				Title    = nameof(TutorialPage6)
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(HelpMenuPage),
				Title    = nameof(HelpMenuPage)
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(HelpPage1),
				Title    = AppResources.HelpPage1Title
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(HelpPage2),
				Title    = AppResources.HelpPage2Title
			});
			this.MenuItems.Add(new()  {
				Icon     = "\uF0C0",
				PageName = nameof(HelpPage3),
				Title    = AppResources.HelpPage3Title
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(HelpPage4),
				Title    = AppResources.HelpPage4Title
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(ChatbotPage),
				Title    = nameof(ChatbotPage)
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(NotifyOtherPage),
				Title    = AppResources.NotifyOtherPageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(SubmitConsentPage),
				Title    = AppResources.SubmitConsentPageTitle1
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(WebViewerPage),
				Title    = nameof(WebViewerPage)
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(LogsPage),
				Title    = AppResources.LogsPageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(SendLogConfirmationPage),
				Title    = AppResources.SendLogConfirmationPageTitle
			});
			this.MenuItems.Add(new() {
				Icon     = "\uF0C0",
				PageName = nameof(SendLogCompletePage),
				Title    = nameof(SendLogCompletePage)
			});
#endif
			_logger.Info("Loaded menu items");
			this.ClearMenuItemsColors();
			this.NavigateCommand = new(this.Navigate);
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
