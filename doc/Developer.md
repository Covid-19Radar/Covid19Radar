# Overview

This document describe how to set up the development environment.

# Android

**Requirments**

On the Windows

- Windows 10
- Visual Studio 2019
  - Xamarin.Android
  - [Multilingual App Toolkit v4.0](https://marketplace.visualstudio.com/items?itemName=MultilingualAppToolkit.MultilingualAppToolkit-18308)
  - Hyper-V
	- If you want to get a significantly improved experience of Android Emulator
- Android Studio v4.0
  - JDK (Xamarin)
  - Android SDK
	- Build Tools 29
	- Platform 28, 29

On the macOS

- macOS Catalina
- [Visual Studio for Mac](https://visualstudio.microsoft.com/ja/vs/mac/xamarin/) v8.6.4
  - Xamarin.Android
  - .NET Core SDK v3.x.xxx (maybe need)
- Android Studio v4.0
  - JDK (Xamarin)
  - Android SDK
	- Build Tools 29
	- Platform 28, 29

More info

- See [Client Side Project Note](Developer-Node.md).

# iOS

**Requirments**

On the Windows

- At writing, i didn't get the build method. Thus, I won't pen it.

On the macOS

- macOS Catalina
- [Visual Studio for Mac](https://visualstudio.microsoft.com/ja/vs/mac/xamarin/) v8.6.4
  - Xamarin.iOS
- Xcode v11.5

More info

- Use Exposure Notification APIs. Use Exposure Notification API. Thus, provisioning profile required `com.apple.developer.exposure-notification entitlement` in it.
- See [Client Side Project Note](Developer-Node.md).

# Server

TBD

# References

- [Client Side Project Note](Developer-Node.md)
- [Installing Xamarin in Visual Studio 2019](https://docs.microsoft.com/en-us/xamarin/get-started/installation/windows)

