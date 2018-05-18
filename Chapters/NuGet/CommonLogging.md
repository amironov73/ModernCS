### Библиотека Common.Logging

Сайт: http://net-commons.github.io/common-logging/, GitHub: https://github.com/net-commons/common-logging, NuGet: https://www.nuget.org/packages/Common.Logging/.

Обёртка над следующими провайдерами:

* Log4Net (v1.2.9 - v1.2.15);
* NLog (v1.0 - v4.4.1);
* SeriLog (v1.5.14);
* Microsoft Enterprise Library Logging Application Block (v3.1 - v6.0);
* Microsoft AppInsights (2.4.0);
* Microsoft Event Tracing for Windows (ETW);
* Log to STDOUT;
* Log to DEBUG OUT.

Поддерживаются следующие фреймворки:

* .NET 2.0, 3.0, 3.5, 4.0, 4.5, 4.6, 4.7;
* Silverlight 5.0;
* Windows Phone 7.x;
* Windows Phone 8.x;
* WinRT 8.1 (for Windows 8.1 and Windows Phone 8.1);
* Universal Windows Platform 10.0+ (WinRT for Windows 10);
* .NET Core 1.0, 2.0.

В простейшем случае библиотека вполне может работать сама по себе:

```csharp
using Common.Logging;
using Common.Logging.Simple;

class Program
{
    static void Main()
    {
        LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter();

        ILog log = LogManager.GetLogger<Program>();

        log.Debug("Some Debug Log Output");
    }
}
```

В консоль будет выведено:

```
17.05.2018 15:57:28 [DEBUG] Program - Some Debug Log Output
```

Можно подключить логирование, например, посредством NLog с помощью пакета Common.Logging.NLog41 (здесь 41 означает версию NLog, с которой совместим пакет). Обязательно нужно сконфигурировать Common.Logging через app.config:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <sectionGroup name="common">
      <section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
    </sectionGroup>

    <section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog"/>
  </configSections>

  <common>
    <logging>
      <factoryAdapter type="Common.Logging.NLog.NLogLoggerFactoryAdapter, Common.Logging.NLog41">
        <arg key="configType" value="INLINE" />
      </factoryAdapter>
    </logging>
  </common>

  <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <targets>
      <target name="console" xsi:type="Console" layout="${date:format=HH\:MM\:ss} ${logger} ${message}" />
    </targets>
    <rules>
      <logger name="*" minlevel="Debug" writeTo="console" />
    </rules>
  </nlog>

  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>
</configuration>
```

Программа в этом случае выглядит тривиально:

```csharp
using Common.Logging;

class Program
{
    static void Main()
    {
        ILog log = LogManager.GetLogger<Program>();

        log.Debug("Some Debug Log Output");
    }
}
```

Хотя при желании можно всю настройку Common.Logging перенести в код, а настройку NLog — в файл NLog.config:

```csharp
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.NLog;

class Program
{
    static void Main()
    {
        NameValueCollection properties = new NameValueCollection
        {
            {"configType", "FILE"},
            {"configFile", "NLog.config"}
        };

        LogManager.Adapter = new NLogLoggerFactoryAdapter(properties);

        ILog log = LogManager.GetLogger<Program>();

        log.Debug("Some Debug Log Output");
    }
}
```
