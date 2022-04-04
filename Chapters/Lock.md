### Во что разворачивается оператор lock

https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock

Конструкция

```c#
lock (x)
{
    // какой-то код
}
```

разворачивается в

```c#
object __lockObj = x;
bool __lockWasTaken = false;
try
{
    System.Threading.Monitor.Enter (__lockObj, ref __lockWasTaken);
    // какой-то код
}
finally
{
    if (__lockWasTaken) 
    {
      System.Threading.Monitor.Exit (__lockObj);
    }
}
```

Документация к `Monitor.Enter` утверждает:

> It is legal for the same thread to invoke `Enter` more than once without it blocking; however, an equal number of `Exit` calls must be invoked before other threads waiting on the object will unblock.

Соответственно, `lock` может быть вложенным.
