### Как работает using

Допустим, у нас есть такой код:
```csharp
using (SomeClass some = GetSomeValue())
{
    some.DoAnything ();
}
```
В какой конкретно код он разворачивается компилятором C#? Вот в такой:
```csharp
{ // Ограничиваем область видимости
    SomeClass some = GetSomeValue ();
    try
    {
        some.DoAnything ();
    }
    finally
    {
        if (some != null)
        {
            some.Dispose ();
        }
    }
}
```
Но если написать так:
```csharp
using (GetFoo())
{
    // Nothing here
}
```
то компилятор внезапно выполнит дополнительную проверку на `null`:
```csharp
{ // Ограничиваем область видимости
    IDisposable temp = GetFoo();
    try
    {
        if (temp != null)
        {
            // Nothing here
        }
    }
    finally
    {
        if (temp != null)
        {
            temp.Dispose ();
        }
    }
}
```
Кстати, следующая конструкция вполне допустима, и при выполнении программы не вызывает исключений. Почему – см. предыдущий листинг:
```csharp
using ((IDisposable)null)
{
    Console.WriteLine ("Ha-ha!");
}
```

