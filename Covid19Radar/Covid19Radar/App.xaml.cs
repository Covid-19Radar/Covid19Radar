using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using DryIoc;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*
 * Our mission...is
 * Empower every person and every organization on the planet achieve more.
 * Put an end to Covid 19
 */

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Covid19Radar
{
	public partial class App : PrismApplication
	{
		private ILoggerService?  _logger;
		private ILogFileService? _log_file;

		/*
		 * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
		 * This imposes a limitation in which the App class must have a default constructor.
		 * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
		 */
		public App() : this(EmptyInitializer.Instance) { }

		public App(IPlatformInitializer initializer) : base(initializer, setFormsDependencyResolver: true) { }

		protected override async void OnInitialized()
		{
			this.InitializeComponent();

			_logger = this.Container.Resolve<ILoggerService>();
			_logger.StartMethod();
			_log_file = this.Container.Resolve<ILogFileService>();
			_log_file.AddSkipBackupAttribute();

#if USE_MOCK
			// For debug mode, set the mock api provider to interact
			// with some fake data
			ExposureNotification.OverrideNativeImplementation(new TestNativeImplementation());
#endif
			ExposureNotification.Init();

			// Migrate userData
			await UserDataMigrationService.Migrate();

			// ignore backup
			DependencyService.Get<ISkipBackup>().SkipBackup(AppConstants.PropertyStore);

			INavigationResult result;

			// Check user data and skip tutorial
			var userData = this.Container.Resolve<IUserDataService>().Get();
			if (userData.SkipTutorial) {
				_logger.Info("The user data was found. Skip the tutorial.");
				_logger.Info($"Is optined: {userData.IsOptined}");
				_logger.Info($"Is policy accepted: {userData.IsPolicyAccepted}");
				if (userData.IsOptined && userData.IsPolicyAccepted) {
					_logger.Info("Navigating to the splash page...");
					result = await this.NavigationService.NavigateAsync("/" + nameof(SplashPage));
				} else {
					_logger.Info("Navigating to the tutorial page...");
					result = await this.NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
				}
			} else {
				_logger.Info("The user data has been created, or the user selected to show the tutorial page.");
				_logger.Info("Navigating to the tutorial page...");
				result = await this.NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
			}
			if (!result.Success) {
				_logger.Warning($"Failed to navigate.");
				this.MainPage = new ExceptionPage() {
					BindingContext = new ExceptionPageViewModel() {
						Message = result.Exception.Message
					}
				};
			}
			this.InitializeBackgroundTasks();
			_logger.EndMethod();
		}

		private async void InitializeBackgroundTasks()
		{
			if (await ExposureNotification.IsEnabledAsync()) {
				await ExposureNotification.ScheduleFetchAsync();
			}
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Base and Navigation
			containerRegistry.RegisterForNavigation<NavigationPage>();
			containerRegistry.RegisterForNavigation<MenuPage>();
			containerRegistry.RegisterForNavigation<HomePage>();

			// Settings
			containerRegistry.RegisterForNavigation<SettingsPage>();
			containerRegistry.RegisterForNavigation<LicenseAgreementPage>();
			containerRegistry.RegisterForNavigation<DebugPage>();

			// Tutorial
			containerRegistry.RegisterForNavigation<TutorialPage1>();
			containerRegistry.RegisterForNavigation<TutorialPage2>();
			containerRegistry.RegisterForNavigation<TutorialPage3>();
			containerRegistry.RegisterForNavigation<PrivacyPolicyPage>();
			containerRegistry.RegisterForNavigation<TutorialPage4>();
			containerRegistry.RegisterForNavigation<TutorialPage5>();
			containerRegistry.RegisterForNavigation<TutorialPage6>();

			// Help
			containerRegistry.RegisterForNavigation<HelpMenuPage>();
			containerRegistry.RegisterForNavigation<HelpPage1>();
			containerRegistry.RegisterForNavigation<HelpPage2>();
			containerRegistry.RegisterForNavigation<HelpPage3>();
			containerRegistry.RegisterForNavigation<HelpPage4>();
			containerRegistry.RegisterForNavigation<SendLogConfirmationPage>();
			containerRegistry.RegisterForNavigation<SendLogCompletePage>();

			// Pages
			containerRegistry.RegisterForNavigation<PrivacyPolicyPage2>();
			containerRegistry.RegisterForNavigation<InqueryPage>();
			containerRegistry.RegisterForNavigation<ChatbotPage>();
			containerRegistry.RegisterForNavigation<TermsofservicePage>();
			containerRegistry.RegisterForNavigation<ThankYouNotifyOtherPage>();
			containerRegistry.RegisterForNavigation<NotifyOtherPage>();
			containerRegistry.RegisterForNavigation<NotContactPage>();
			containerRegistry.RegisterForNavigation<ContactedNotifyPage>();
			containerRegistry.RegisterForNavigation<SubmitConsentPage>();
			containerRegistry.RegisterForNavigation<ExposuresPage>();
			containerRegistry.RegisterForNavigation<ReAgreePrivacyPolicyPage>();
			containerRegistry.RegisterForNavigation<ReAgreeTermsOfServicePage>();
			containerRegistry.RegisterForNavigation<SplashPage>();

			// News Page
			containerRegistry.RegisterForNavigation<NewsPage>();
			containerRegistry.RegisterForNavigation<WebViewerPage>();

			// Services
			containerRegistry.RegisterSingleton<ILoggerService,            LoggerService>();
			containerRegistry.RegisterSingleton<ILogReaderWriter,          LogReaderWriter>();
			containerRegistry.RegisterSingleton<ILogFileService,           LogFileService>();
			containerRegistry.RegisterSingleton<ILogPathService,           LogPathService>();
			containerRegistry.RegisterSingleton<ILogPeriodicDeleteService, LogPeriodicDeleteService>();
			containerRegistry.RegisterSingleton<ILogUploadService,         LogUploadService>();
			containerRegistry.RegisterSingleton<IEssentialsService,        EssentialsService>();
			containerRegistry.RegisterSingleton<IUserDataService,          UserDataService>();
			containerRegistry.RegisterSingleton<ExposureNotificationService>();
			containerRegistry.RegisterSingleton<ITermsUpdateService,         TermsUpdateService>();
			containerRegistry.RegisterSingleton<IApplicationPropertyService, ApplicationPropertyService>();
			containerRegistry.RegisterSingleton<IHttpClientService,          HttpClientService>();
#if USE_MOCK
			containerRegistry.RegisterSingleton<IHttpDataService, HttpDataServiceMock>();
			containerRegistry.RegisterSingleton<IStorageService,  StorageServiceMock>();
#else
			containerRegistry.RegisterSingleton<IHttpDataService, HttpDataService>();
			containerRegistry.RegisterSingleton<IStorageService,  StorageService>();
#endif
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (VersionTracking.IsFirstLaunchEver) {
				SecureStorage.Remove(AppConstants.StorageKey.UserData);
				SecureStorage.Remove(AppConstants.StorageKey.Secret);
			}
			this.Container.Resolve<ILogPeriodicDeleteService>().Init();
			_log_file?.Rotate();
		}

		protected override void OnResume()
		{
			base.OnResume();
			_log_file?.Rotate();
		}

		private sealed class EmptyInitializer : IPlatformInitializer
		{
			internal static readonly EmptyInitializer Instance = new EmptyInitializer();

			private EmptyInitializer() { }

			public void RegisterTypes(IContainerRegistry containerRegistry) { }
		}
	}
}
