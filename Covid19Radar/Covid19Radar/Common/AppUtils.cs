using System;
using System.Net.Http;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.Common
{
	static class AppUtils
	{
		public static async ValueTask CheckPermission()
		{
			var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
			if (status != PermissionStatus.Granted) {
				await Permissions.RequestAsync<Permissions.LocationAlways>();
			}
		}

		public static async ValueTask PopUpShare()
		{
			if (Device.RuntimePlatform == Device.iOS) {
				await Share.RequestAsync(new ShareTextRequest() {
					Uri   = AppSettings.Instance.AppStoreUrl,
					Title = AppResources.AppName
				});
			} else if (Device.RuntimePlatform == Device.Android) {
				await Share.RequestAsync(new ShareTextRequest() {
					Uri   = AppSettings.Instance.GooglePlayUrl,
					Title = AppResources.AppName
				});
			}

		}

		public static async ValueTask CheckVersionAsync(ILoggerService? logger)
		{
			logger?.StartMethod();
			string uri = AppResources.UrlVersion;
			using (var client = new HttpClient()) {
				try {
					string json          = await client.GetStringAsync(uri);
					string versionString = JObject.Parse(json).Value<string>("version");
					if (new Version(versionString).CompareTo(new Version(AppInfo.VersionString)) > 0) {
						await UserDialogs.Instance.AlertAsync(
							AppResources.AppUtilsGetNewVersionDescription,
							AppResources.AppUtilsGetNewVersionTitle,
							AppResources.ButtonOk);
						if (Device.RuntimePlatform == Device.iOS) {
							await Browser.OpenAsync(AppSettings.Instance.AppStoreUrl, BrowserLaunchMode.External);
						} else if (Device.RuntimePlatform == Device.Android) {
							await Browser.OpenAsync(AppSettings.Instance.GooglePlayUrl, BrowserLaunchMode.External);
						}
					}
				} catch (Exception e) {
					logger?.Exception("Failed to check version.", e);
				} finally {
					logger?.EndMethod();
				}
			}
		}
	}
}
