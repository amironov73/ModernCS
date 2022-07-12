## Привет, мир!

MyApp.csproj:

```msbuild
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="0.10.14" />
    <PackageReference Include="Avalonia.Desktop" Version="0.10.14" />
   </ItemGroup>
</Project>
```

`Program.cs`:

```csharp
using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Styling;

namespace MyApp;

public class App
    : Application
{
    public override void Initialize()
    {
        Current!.Styles.Add (new StyleInclude (new Uri ("avares://ControlCatalog/Styles"))
        {
            Source = new Uri ("avares://Avalonia.Themes.Fluent/FluentLight.xaml")
        });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

public class MainWindow
    : Window
{
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Title = "Это окно Avalonia, и это круто!";
        Width = 300;
        Height = 100;

        Content = new Label
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Content = "Привет, мир!"
        };
    }
}

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime (args);
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}
```
