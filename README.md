# Naylah Toolkit for X .net

<img src="https://raw.githubusercontent.com/NaylahProject/Naylah.Toolkit.UWP/master/NaylahLogo.png" width="48">

[![TFSBuild](https://softincloud.visualstudio.com/_apis/public/build/definitions/5b360ddf-7ff3-4c1d-93f6-2e82ed850c7e/47/badge)](https://softincloud.visualstudio.com/DefaultCollection/Naylah%20Services)

Naylah.Core [![NuGet](https://img.shields.io/nuget/v/Naylah.Core.svg?style=flat-square)](https://www.nuget.org/packages/Naylah.Core/)

Naylah.Xamarin [![NuGet](https://img.shields.io/nuget/v/Naylah.Xamarin.svg?style=flat-square)](https://www.nuget.org/packages/Naylah.Xamarin/)

[![forthebadge](http://forthebadge.com/images/badges/built-with-love.svg)](http://forthebadge.com)
[![forthebadge](http://forthebadge.com/images/badges/contains-cat-gifs.svg)](http://forthebadge.com)
[![forthebadge](http://forthebadge.com/images/badges/designed-in-ms-paint.svg)](http://forthebadge.com)
[![forthebadge](http://forthebadge.com/images/badges/fuck-it-ship-it.svg)](http://forthebadge.com)

Contains a set of useful libraries, controls, helpers, architecture, etc. The intention is to create a community of developers who contribute to standardization of user interfaces, model and data flow.

**You** are welcome to join the Naylah Community or make PRs. (Please do :smile:)

It's just to simple to use.

Installation
-------------

Naylah.Xamarin is available as a NuGet package. You can install it via NuGetPackage Manager or using the NuGet Package Console window:

```
PM> Install-Package Naylah.Xamarin
```

After installation, just replace Xamarin default Application class by bootstrapper and start a NavigationServiceFactory with a MasterDetail page or a NavigationPage.

```csharp
public class App : BootStrapper
{
  public static App CurrentApp { get; private set; }
  public App()
  {
      CurrentApp = this;
      NavigationServiceFactory(new NavigationPage(new SplashPage()));
  }
}
```

# What's in Naylah?

Controls
------
###ImageChooser
![Android ImageChooser](https://github.com/NaylahProject/Naylah/blob/master/Screenshots/Android%20ImageChooser.png)
![iOS ImageChooser](https://github.com/NaylahProject/Naylah/blob/master/Screenshots/iOS%20ImageChooser.png)

Behaviors
------
###NumericEntryBehavior

Services
------
###NavigationService

```csharp
NavigationService.NavigateAsync(var new Page, stringparam, true); //
NavigationService.NavigateModalAsync(var new Page, stringparam, true); //
...
```
