### Где взять самый распоследний компилятор C#

На [Azure DevOps](https://dev.azure.com/dnceng/public/_packaging?_a=feed&feed=dotnet-tools) Microsoft выкладывает прорву NuGet-пакетов ночных сборок всякого, в том числе свежесобранный Roslyn. Пакет называется Microsoft.Net.Compilers.Toolset. Внутри два набора: для классического .NET Framework 4.7 и для .NET Core 3.1. NuGet спокойно можно распаковать как ZIP-файл и выбрать нужное.

Кроме того, можно подключить NuGet-фид https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json. Вот как это делается:

```
mkdir Newest
cd Newest
dotnet new console
dotnet new nugetconfig
dotnet nuget add source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json --name azure --configfile nuget.config
dotnet add package Microsoft.Net.Compilers.Toolset --prerelease
```
