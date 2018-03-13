### Во что разворачивается await

Sinix пишет:

> Код вида
```csharp
Report("Before await");
var someValue = await GetSomeValueAsync();
Report("After await: {0}", someValue);
```
> компилятор превращает в нечто вроде
```csharp
Report("Before await");
var awaiter = GetSomeValueAsync().GetAwaiter();
awaiter.OnCompleted
  (
    () => Report("After await: {0}", awaiter.GetResult()
  );
```
