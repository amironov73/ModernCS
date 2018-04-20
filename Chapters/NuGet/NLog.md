### NLog

NLog – фреймворк для вывода диагностических, отладочных и прочих служебных сообщений из .NET-приложения. Сайт проекта: http://nlog-project.org/, пакет NuGet: https://www.nuget.org/packages/NLog, исходные тексты: https://github.com/NLog/NLog/.

В современных версиях Visual Studio достаточно выполнить команду NuGet

```
Install-Package NLog.Config
```

Есть также дополнительные пакеты:

* `NLog.Web` – компоненты, специфичные для ASP.Net и IIS.
* `NLog.Windows.Forms` – компоненты, специфичные для WinForms.
* `NLog.Extended` – логгирование в MSMQ.

Настройки NLog хранятся в файле NLog.config, который должен находиться рядом с приложением. Пример файла конфигурации:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
  xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
  autoReload="true"
  throwExceptions="false"
  internalLogLevel="Off"
  internalLogFile="c:\temp\nlog-internal.log" >
 
  <variable name="brief"
     value="${longdate} | ${level} | ${logger} | ${message}"/>
 
  <targets>
 
    <target xsi:type="Console" name="console"
      layout="${brief}" />
 
    <target xsi:type="File" name="file"
      fileName="log.txt" layout="${longdate} | ${level} | ${message}" />
     
  </targets>
 
  <rules>
 
    <logger name="SomeNamespace.Component.*" minlevel="Trace"
      writeTo="file" final="true" />
    <logger name="*" minlevel="Trace" writeTo="file" />
    <logger name="*" minlevel="Debug" writeTo="console" />
  </rules>
</nlog>
```

Из переменных можно изобразить довольно сложную конструкцию:

```xml
<variable name="day" value="${date:format=dddd}"/>
<variable name="month" value="${date:format=MMMM}"/>
<variable name="fmt"
  value="${longdate} | ${level} | ${logger} | ${day} | ${month} | ${message}"/>
 
<targets>
    <target name="console" xsi:type="ColoredConsole" layout="${fmt}" />
    <target name="file" xsi:type="File" layout="${verbose}"
      fileName="${basedir}/${day}.log" />
</targets>
```

Можно подключать дополнительные сборки с расширениями NLog:

```xml
<extensions>
    <add assembly="MyNLogExtensions"/>
</extensions>
```

Уровни логирования:

* **Trace** – очень детальный отчёт, например, содержимое передаваемых по сети пакетов.
* **Debug** – отладочные сообщения, как правило, используемые при отладке приложения.
* **Info** – информационные сообщения, как правило, не используемые при обычной работе приложения.
* **Warn** – предупреждения о некритичных ошибках, после которых возможно восстановление работоспособности приложения, например, временные сбои в работе сети.
* **Error** – сообщения об ошибках.
* **Fatal** – очень серьёзные ошибки, как правило, прекращающие выполнение программы.

Пример использования:

```csharp
using System;
 
using NLog;
 
class Program
{
    static void Main()
    {
        Logger logger = LogManager.GetCurrentClassLogger();
 
        logger.Trace("Some message");
    }
}
```

Лучше, конечно, прикапывать логгер в какое-нибудь статическое поле:

```csharp
class MyClass
{
    private static Logger _logger = LogManager.GetCurrentClassLogger();
 
...
 
    public void SomeMethod ()
    {
       _logger.Debug ("Some debug message");
    }
}
```

Стандартные таргеты в NLog.dll:

* [AspResponse](https://github.com/NLog/NLog/wiki/AspResponse-target) — выводит сообщение как объект ASP Response.
* [Chainsaw](https://github.com/NLog/NLog/wiki/Chainsaw-target) — отсылает сообщения в удалённое приложение Chainsaw.
* [ColoredConsole](https://github.com/NLog/NLog/wiki/ColoredConsole-target) — вывод в консоль с настраиваемой расцветкой.
* [Console](https://github.com/NLog/NLog/wiki/Console-target) — вывод в стандартную консоль.
* [Database](https://github.com/NLog/NLog/wiki/Database-target) — сохранение в базу данных через стандартный провайдер ADO.NET.
* [Debug](https://github.com/NLog/NLog/wiki/Debug-target) — мок-таргет, для тестирования.
* [Debugger](https://github.com/NLog/NLog/wiki/Debugger-target) — вывод в подключенный отладчик managed-кода.
* [EventLog](https://github.com/NLog/NLog/wiki/EventLog-target) — вывод в Event Log.
* [File](https://github.com/NLog/NLog/wiki/File-target) — вывод в один или более файлов.
* [LogReceiverService](https://github.com/NLog/NLog/wiki/LogReceiverService-target) — отсылка сообщений в NLog Receiver Service (с использованием WCF или Web Services).
* [Mail](https://github.com/NLog/NLog/wiki/Mail-target) — отсылка сообщений по email с использованием протокола SMTP.
* [Memory](https://github.com/NLog/NLog/wiki/Memory-target) — сохранение в памяти приложения в ArrayList (для программного извлечения).
* [MethodCall](https://github.com/NLog/NLog/wiki/MethodCall-target) — вызов указанного статического метода.
* [Network](https://github.com/NLog/NLog/wiki/Network-target) — отсылка сообщений по сети.
* [NLogViewer](https://github.com/NLog/NLog/wiki/NLogViewer-target) — отсылка сообщений удалённому приложению NLog Viewer.
* [Null](https://github.com/NLog/NLog/wiki/Null-target) — сообщения отбрасываются. Используется для отладки и проверки производительности.
* [OutputDebugString](https://github.com/NLog/NLog/wiki/OutputDebugString-target) — сообщения отсылаются через OutputDebugString() Win32 API.
* [PerfCounter](https://github.com/NLog/NLog/wiki/PerfCounter-target) — увеличение указанного счётчика производительности.
* [Trace](https://github.com/NLog/NLog/wiki/Trace-target) — Sends log messages through System.Diagnostics.Trace.
* [WebService](https://github.com/NLog/NLog/wiki/WebService-target) — Calls the specified web service on each log message.

Предусмотрены также обёртки вокруг таргетов, привносящие определённую дополнительную функциональность:

* [AsyncWrapper](https://github.com/NLog/NLog/wiki/AsyncWrapper-target) – Асинхронная буферизованная запись.
* [AutoFlushWrapper](https://github.com/NLog/NLog/wiki/AutoFlushWrapper-target) – Вызывает Flush после каждой операции записи.
* [BufferingWrapper](https://github.com/NLog/NLog/wiki/BufferingWrapper-target) – Буферизация записей и отсылка их «пачками». Полезно в сочетании с таргетом Mail.
* [FallbackGroup](https://github.com/NLog/NLog/wiki/FallbackGroup-target) – Резервные таргеты на случай сбоя основного.
* [FilteringWrapper](https://github.com/NLog/NLog/wiki/FilteringWrapper-target) – Фильтрация записей согласно указанным условиям.
* [ImpersonatingWrapper](https://github.com/NLog/NLog/wiki/ImpersonatingWrapper-target) – Имперсонация.
* [PostFilteringWrapper](https://github.com/NLog/NLog/wiki/PostFilteringWrapper-target) – Фильтрация записей на основе набора условий, вычисляемых по группе записей.
* [RandomizeGroup](https://github.com/NLog/NLog/wiki/RandomizeGroup-target) – Отсылка сообщений случайному таргету из группы.
* [RepeatingWrapper](https://github.com/NLog/NLog/wiki/RepeatingWrapper-target) – Повторение отсылки сообщения указанное число раз.
* [RetryingWrapper](https://github.com/NLog/NLog/wiki/RetryingWrapper-target) – Повторение отсылки сообщения в случае ошибки.
* [RoundRobinGroup](https://github.com/NLog/NLog/wiki/RoundRobinGroup-target) – Распределение нагрузки на несколько таргетов.
* [SplitGroup](https://github.com/NLog/NLog/wiki/SplitGroup-target) – Отсылка сообщения на все таргеты группы.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
 
    <targets>
        <target name="asyncFile" xsi:type="AsyncWrapper">
            <target name="logfile" xsi:type="File" fileName="file.txt" />
        </target>
    </targets>
 
    <rules>
        <logger name="*" minlevel="Info" writeTo="asyncFile" />
    </rules>
</nlog>
```

