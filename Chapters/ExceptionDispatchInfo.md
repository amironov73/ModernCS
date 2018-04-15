### ExceptionDispatchInfo

Иногда очень хочется сохранить как можно больше информации об исключении, чтобы потом перевыбрость его в другом месте кода (возможно даже,  в другом методе). Теперь (в .NET 4.5) это стало возможным с классом System.Runtime.ExceptionServices.ExceptionDispatchInfo. Делается это примерно так:

```csharp
// может быть статической переменной
ExceptionDispatchInfo possibleException = null;
 
try
{
    // Делаем что-нибудь ужасное, чтобы вызвать исключение
    int.Parse ("wrong!");
}
catch (Exception ex)
{
    // Сохранили контекст исключения
    possibleException = ExceptionDispatchInfo.Capture (ex);
 
    // Можно сделать ещё что-нибудь полезное, 
    // главное не вызвать ещё одного исключения
}
 
...
 
// где-нибудь, возможно, в другом методе
if (possibleException != null)
{
    possibleException.Throw ();
}
```

Ограничение: класс `ExceptionDispatchInfo` не сериализуется и не может быть передан за пределы домена (app domain).

Главное здесь – соблюдать меру, а то можно запутаться в перехваченных исключениях.

