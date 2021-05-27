### Встроенная проверка .NET SDK

To make it easier to track when new versions of the SDK and Runtimes are available, we’ve added a new command to the .NET 6 SDK: dotnet sdk check

This will tell you within each feature band what is the latest available version of the .NET SDK and .NET Runtime.

```
> dotnet sdk check
.NET SDKs:
Version                        Status
----------------------------------------------------------
3.1.409                        Up to date.
5.0.104                        Up to date.
5.0.203                        Up to date.
5.0.300-preview.21258.4        Patch 5.0.300 is available.
5.0.300                        Up to date.
6.0.100-preview.4.21255.9      Up to date.
 
.NET Runtimes:
Name                              Version                      Status
----------------------------------------------------------------------------------------
Microsoft.AspNetCore.All          2.1.28                       Up to date.
Microsoft.AspNetCore.App          2.1.28                       Up to date.
Microsoft.NETCore.App             2.1.28                       Up to date.
Microsoft.AspNetCore.App          3.1.15                       Up to date.
Microsoft.NETCore.App             3.1.15                       Up to date.
Microsoft.WindowsDesktop.App      3.1.15                       Up to date.
Microsoft.AspNetCore.App          5.0.1                        Patch 5.0.6 is available.
Microsoft.NETCore.App             5.0.1                        Patch 5.0.6 is available.
Microsoft.WindowsDesktop.App      5.0.1                        Patch 5.0.6 is available.
Microsoft.AspNetCore.App          5.0.3                        Patch 5.0.6 is available.
Microsoft.NETCore.App             5.0.3                        Patch 5.0.6 is available.
Microsoft.WindowsDesktop.App      5.0.3                        Patch 5.0.6 is available.
Microsoft.AspNetCore.App          5.0.4                        Patch 5.0.6 is available.
Microsoft.NETCore.App             5.0.4                        Patch 5.0.6 is available.
Microsoft.WindowsDesktop.App      5.0.4                        Patch 5.0.6 is available.
Microsoft.AspNetCore.App          5.0.5                        Patch 5.0.6 is available.
Microsoft.NETCore.App             5.0.5                        Patch 5.0.6 is available.
Microsoft.WindowsDesktop.App      5.0.5                        Patch 5.0.6 is available.
Microsoft.AspNetCore.App          5.0.6                        Up to date.
Microsoft.NETCore.App             5.0.6                        Up to date.
Microsoft.WindowsDesktop.App      5.0.6                        Up to date.
Microsoft.AspNetCore.App          6.0.0-preview.4.21253.5      Up to date.
Microsoft.NETCore.App             6.0.0-preview.4.21253.7      Up to date.
Microsoft.WindowsDesktop.App      6.0.0-preview.4.21254.5      Up to date.
```
