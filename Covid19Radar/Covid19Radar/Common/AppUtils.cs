﻿#if !DEBUG
#define NDEBUG
#endif
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.Common
{
    static class AppUtils
    {
        public static async void CheckPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationAlways>();
            }
        }
        public static async void PopUpShare()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppSettings.Instance.AppStoreUrl,
                    Title = AppResources.AppName
                });
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = AppSettings.Instance.GooglePlayUrl,
                    Title = AppResources.AppName
                });
            }

        }

        [Conditional("NDEBUG")]
        public static void CheckVersion()
		{
			CheckVersion(null);
		}

        [Conditional("NDEBUG")]
        public static async void CheckVersion(ILoggerService? loggerService)
        {
            loggerService?.StartMethod();

            var uri = AppResources.UrlVersion;
            using (var client = new HttpClient())
            {
                try
                {
                    var json = await client.GetStringAsync(uri);
                    var versionString = JObject.Parse(json).Value<string>("version");

                    if (new Version(versionString).CompareTo(new Version(AppInfo.VersionString)) > 0)
                    {
                        await UserDialogs.Instance.AlertAsync(
                            AppResources.AppUtilsGetNewVersionDescription,
                            AppResources.AppUtilsGetNewVersionTitle,
                            AppResources.ButtonOk);
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            await Browser.OpenAsync(AppSettings.Instance.AppStoreUrl, BrowserLaunchMode.External);
                        }
                        else if (Device.RuntimePlatform == Device.Android)
                        {
                            await Browser.OpenAsync(AppSettings.Instance.GooglePlayUrl, BrowserLaunchMode.External);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    loggerService?.Exception("Failed to check version.", ex);
                }
                finally
                {
                    loggerService?.EndMethod();
                }
            }
        }
    }
}
