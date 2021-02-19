using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Covid19Radar.Droid.Services.Logs;
using Covid19Radar.Services.Logs;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using Prism;
using Prism.Ioc;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FFImageConfig = FFImageLoading.Config.Configuration;
using Platform = Xamarin.Essentials.Platform;

namespace Covid19Radar.Droid
{
	[Activity(
		Label                = "@string/app_name",
		Icon                 = "@mipmap/ic_launcher",
		Theme                = "@style/MainTheme.Splash",
		MainLauncher         = true,
		LaunchMode           = LaunchMode.SingleTop,
		ScreenOrientation    = ScreenOrientation.Portrait,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
	)]
	public class MainActivity : FormsAppCompatActivity
	{
		public static object dataLock = new object();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource   = Resource.Layout.Toolbar;
			base.SetTheme(Resource.Style.MainTheme);
			base.OnCreate(savedInstanceState);

			Forms.SetFlags("RadioButton_Experimental");
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			Forms.Init(this, savedInstanceState);
			FormsMaterial.Init(this, savedInstanceState);

			CachedImageRenderer.Init(enableFastRenderer: true);
			ImageService.Instance.Initialize(new FFImageConfig());

			UserDialogs.Init(this);
			this.LoadApplication(new App(new AndroidInitializer()));
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum()] Permission[] grantResults)
		{
			Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			ExposureNotification.OnActivityResult(requestCode, resultCode, data);
		}

		public class AndroidInitializer : IPlatformInitializer
		{
			public void RegisterTypes(IContainerRegistry containerRegistry)
			{
				// Services
				containerRegistry.RegisterSingleton<ILogPathDependencyService, LogPathServiceAndroid>();
			}
		}
	}
}
