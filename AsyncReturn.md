### Возвращаемые типы в async

Метод с модификатором async может возвращать:

* void (рекомендуется только для обработчиков событий);
* Task, если не требуется результат выполнения метода;
* Task&lt;T&gt;, если требуется результат (например, целое число или строка);
* Начиная с C# 7, можно возвращать объект любого класса, у которого есть метод GetAwaiter, который в свою очередь возвращает объект, реализующий интерфейс
[System.Runtime.CompilerServices.ICriticalNotifyCompletion](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.icriticalnotifycompletion).

Возврат значения Task&lt;T&gt; намеренно упрощён авторами компилятора:
```csharp
public async Task<string> GetGreeting()
{
    return "Hello";
}
```
т. е. не обязательно самостоятельно создавать объект типа Task&gt;T&gt; и присваивать ему результирующее значение.

Благодаря генерализованной обработке async-методов, можно
возвращать собственные типы, имеющие публичный метод GetAwaiter.
```csharp
public async ValueTask<int> GetDiceRoll()
{
    return 3; // Случайное число
}
```
