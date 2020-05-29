﻿using CoreBluetooth;
using CoreLocation;
using Covid19Radar.Common;
using Covid19Radar.iOS.Renderers;
using Covid19Radar.iOS.Services;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Foundation;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using ObjCRuntime;
using Prism;
using Prism.Ioc;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Covid19Radar.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public static AppDelegate Instance { get; private set; }
        public AppDelegate()
        {
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            global::FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration()
            {
                Logger = new Covid19Radar.Services.DebugLogger()
            });
            Distribute.DontCheckForUpdatesInDebug();

            Plugin.LocalNotification.NotificationCenter.AskPermission();

            LoadApplication(new App(new iOSInitializer()));

#if ENABLE_TEST_CLOUD
            // requires Xamarin Test Cloud Agent
            Xamarin.Calabash.Start();
#endif

            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            return base.FinishedLaunching(app, options);
        }

        /*
        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Check for new data, and display it
            BackgroundService();

            // Inform system of fetch results
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        private async void BackgroundService()
        {
            await Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        InvokeOnMainThread(delegate
                        {
                        });
                        System.Threading.Thread.Sleep(60000);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message + System.Environment.NewLine + ex.StackTrace);
                }
            }).ConfigureAwait(false);
        }
        */

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
        }
        /*
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, System.Action<UIBackgroundFetchResult> completionHandler)
        {
            var result = Push.DidReceiveRemoteNotification(userInfo);
            if (result)
            {
                completionHandler?.Invoke(UIBackgroundFetchResult.NewData);
            }
            else
            {
                completionHandler?.Invoke(UIBackgroundFetchResult.NoData);
            }
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Push.FailedToRegisterForRemoteNotifications(error);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Push.RegisteredForRemoteNotifications(deviceToken);
        }
        */
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISQLiteConnectionProvider, SQLiteConnectionProvider>();
            //containerRegistry.RegisterSingleton<UserDataService, UserDataService>();
        }
    }



}
