# VaraniumSharp.WinUI

![Logo](https://github.com/NinetailLabs/VaraniumSharp.WinUI/blob/master/logo.png?raw=true)

[![Build status](https://ci.appveyor.com/api/projects/status/7ki0vvu6hecak8uw/branch/master?svg=true)](https://ci.appveyor.com/project/DeadlyEmbrace/varaniumsharp-winui/branch/master)
[![NuGet](https://img.shields.io/nuget/v/VaraniumSharp.WinUI.svg)](https://www.nuget.org/packages/VaraniumSharp.WinUI/)
[![Coverage Status](https://coveralls.io/repos/github/NinetailLabs/VaraniumSharp.WinUI/badge.svg?branch=main)](https://coveralls.io/github/NinetailLabs/VaraniumSharp.WinUI?branch=main)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=NinetailLabs_VaraniumSharp.WinUI&metric=bugs)](https://sonarcloud.io/dashboard?id=NinetailLabs_VaraniumSharp.WinUI)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=NinetailLabs_VaraniumSharp.WinUI&metric=code_smells)](https://sonarcloud.io/dashboard?id=NinetailLabs_VaraniumSharp.WinUI)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=NinetailLabs_VaraniumSharp.WinUI&metric=ncloc)](https://sonarcloud.io/dashboard?id=NinetailLabs_VaraniumSharp.WinUI)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=NinetailLabs_VaraniumSharp.WinUI&metric=alert_status)](https://sonarcloud.io/dashboard?id=NinetailLabs_VaraniumSharp.WinUI)

VaraniumSharp.WinUI is a [VarniumSharp](https://github.com/NinetailLabs/VaraniumSharp) library with WinUI 3 helper classes and components.
It can be used to implement certain features into a WinUI 3 project with minimal effort.

## Functionality
- Create a Window where the user can fully customize the layout and controls per tab
- Automatic saving/loading of layouts the user configured previously
- Multiple layout panes to assist with layout customization
- Setting pane that can be extended easily with custom seting panes
- Modules to assist with filtering, sorting and grouping a GridView with minimal setup
- Drag and drop helper classes to easy the use of drag and drop functionality
- Dialog wrappers to ease the use of WinUI dialogs
- Bind converters
- Sortable, filterable and groupable collections to back the sorting, filtering and grouping modules

## Requirements
VaraniumSharp.WinUI relies on the attribute based dependency injection system provided by VaraniumSharp to handle the loading of it's components and to handle the customized layouts.
As such a dependency injection package is required.
Currently there are dependency injection packages for [DryIoC](https://www.nuget.org/packages/VaraniumSharp.DryIoc) and the Microsoft [ServiceCollection](Microsoft.Extensions.DependencyInjection.Abstractions)
It is easy to create a custom wrapper for your preferred package as well, simply look at the implementation of one of the above packages as a basic guide.

## Basic setup
Simply create a new WinUI 3 project, add the VaraniumSharp.WinUI package and one of the above mentioned IoC packages then add the following code to your `App.xaml.cs` file's `OnLaunched` method to bootstrap the main window.
```csharp
// It is required to pre-load the assemblies that are auto-injected by VaraniumSharp otherwise their injections won't be picked up
AppDomain.CurrentDomain.Load(new AssemblyName("VaraniumSharp.WinUI"));

// Set up your IoC container and request that all classes are registered
var containerSetup = new ContainerSetup();
containerSetup.RetrieveClassesRequiringRegistration(true);
containerSetup.RetrieveConcretionClassesRequiringRegistration(true);

// Resolve a TabWindow instance from the container, configure it and then activate it to display it to the user
var tabWindow = containerSetup.Resolve<TabWindow>();
tabWindow.MinWidth = 750;
tabWindow.Backdrop = new AcrylicSystemBackdrop();
m_window = tabWindow;
m_window.Activate();
```

## Additional examples
VaraniumSharp.WinUI is designed to be easy to use, but some of the configurations are a bit counter-intuitive.
To see how to handle the basic configuration a test project is included in the GitHub repository.
See the test project [here](https://github.com/NinetailLabs/VaraniumSharp.WinUI/tree/master/TestHelper)

## Bugs, feedback, questions
Feel free to open an issue on [GitHub](https://github.com/NinetailLabs/VaraniumSharp.WinUI/issues) to request further assistance.


## Icon
[Sprout](https://thenounproject.com/term/sprout/607325/) by [parkjisun](https://thenounproject.com/naripuru/) from the Noun Project
