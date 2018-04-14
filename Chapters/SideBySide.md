### Использование новой CLR бок о бок со старой

Новый .NET допускает применение [бок о бок со старым](https://msdn.microsoft.com/en-us/library/ee518876(VS.100).aspx) даже в рамках одного процесса. Это означает, что приложение, скомпилированное для .NET 2.0, может пользоваться «плюшками» нового .NET без перекомпилирования! Для этого достаточно «доработать напильником» файл App.config:

```xml
<configuration>
...
  <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0" />
    <supportedRuntime version="v2.0.50727" />
  </startup>
...
</configuration>
```

Если очень хочется, то можно уговорить систему использовать для Вашего приложения самую последнюю версию CLR из установленных на машине. Для этого достаточно установить в App.config недокументированный параметр rollForward:

```xml
<configuration>
...
  <startup>
    <process>
      <rollForward enabled="true" />
    </process>
  </startup>
...
</configuration>
```
