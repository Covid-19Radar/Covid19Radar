using Covid19Radar.iOS.Services.Logs;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Covid19Radar.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		public static AppDelegate Instance { get; private set; }

		public AppDelegate() { }

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			NSUrlCache.SharedCache.RemoveAllCachedResponses();

			Forms.SetFlags("RadioButton_Experimental");

			Forms.Init();
			FormsMaterial.Init();

			CachedImageRenderer.Init();
			ImageService.Instance.Initialize(new() {
				Logger = new DebugLogger()
			});

			this.LoadApplication(new App(new iOSInitializer()));

			UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
			return base.FinishedLaunching(app, options);
		}
	}

	public class iOSInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Services
			containerRegistry.RegisterSingleton<ILogPathDependencyService, LogPathServiceIos>();
		}
	}
}
