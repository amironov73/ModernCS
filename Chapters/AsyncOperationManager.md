### Класс AsyncOperationManager

Водится в .NET с незапамятных времён в сборке System.dll, в пространстве имён System.ComponentModel: https://msdn.microsoft.com/en-us/library/system.componentmodel.asyncoperationmanager(v=vs.90).aspx.

Класс небольшой, но довольно полезный:
```csharp
public static class AsyncOperationManager
{
    public static AsyncOperation CreateOperation
       (
           object userSuppliedState
       );
    public static SynchronizationContext SynchronizationContext { get; set; }
}
 
public sealed class AsyncOperation
{
    public SynchronizationContext SynchronizationContext { get; }
 
    public object UserSuppliedState { get; }
 
    public void Post
       (
          SendOrPostCallback d,
          object arg
       );
 
    public void PostOperationCompleted
       (
          SendOrPostCallback d,
          object arg
       );
}
```

