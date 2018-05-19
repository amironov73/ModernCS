### Библиотека Microsoft.VisualStudio.Threading

Чтобы Visual Studio не слишком тормозила, она должна использовать многопоточность и асинхронность. Но тогда она начинает глючить. Чтобы уменьшить глюки от многопоточности и асинхронности, в Microsoft придумали библиотеку Microsoft.VisualStudio.Threading и комплект розлиновских анализаторов. Несмотря на название, библиотека отлично работает в любых других приложениях. 

GitHub: https://github.com/Microsoft/vs-threading, NuGet: https://www.nuget.org/packages/Microsoft.VisualStudio.Threading и https://www.nuget.org/packages/Microsoft.VisualStudio.Threading.Analyzers. Поддерживаются: .NET Framework 4.5, Windows 8, Windows Phone 8.1, .NET Portable (Profile111 или .NET Standard 1.1).

Библиотека предоставляет асинхронные версии многих примитивов синхронизации: AsyncAutoResetEvent, AsyncManualResetEvent, AsyncBarrier, AsyncCountdownEvent, AsyncSemaphore, AsyncReaderWriterLock и др., асинхронные версии распространённых типов: AsyncLazy<T>, AsyncLocal<T>, AsyncQueue<T>, AsyncEventHandler и др. Также имеются асинхронные методы расширения:

* Await на TaskScheduler для переключения на него;
* Переключение в фоновый поток с помощью await на TaskScheduler.Default;
* Await на Task с тайм-аутом;
* Await на Task с отменой;
* JoinableTaskFactory, позволяющая запланировать асинхронное или синхронное действие, не блокирующее UI-поток.

#### Как переключиться на фоновый поток

```csharp
await TaskScheduler.Default;
```

#### Как переключиться на UI-поток

В async-методе:

```csharp
await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
```

В обычном синхронном методе:

```csharp
ThreadHelper.JoinableTaskFactory.Run(async delegate
{
    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
    // You're now on the UI thread.
});
```

#### Как использовать UI-поток с фоновым приоритетом

Используйте метод-расширение StartOnIdle:

```csharp
await ThreadHelper.JoinableTaskFactory.StartOnIdle(
    async delegate
    {
        for (int i = 0; i < 10; i++)
        {
            DoSomeWorkOn(i);
            
            // Ensure we frequently yield the UI thread in case user input is waiting.
            await Task.Yield();
        }
    });
```

#### Как вызвать асинхронный код из синхронного метода (и дождаться результата)


```csharp
ThreadHelper.JoinableTaskFactory.Run(async delegate
{
    await SomeOperationAsync(...);
});
```

#### Как писать методы "запустил-и-забыл"

Предупреждение: избегайте методов "запустил-и-забыл", возвращающих async void, т. к. они могут уронить всё приложение!

```csharp
void StartOperation()
{
  this.JoinableTaskFactory.RunAsync(async delegate
  {
    await Task.Yield(); // get off the caller's callstack.
    DoWork();
    this.DisposalToken.ThrowIfCancellationRequested();
    DoMoreWork();
  }).FileAndForget("vs/YOUR-FEATURE/YOUR-ACTION");
}
```

Здесь this.JoinableTaskFactory определён в вашем AsyncPackage. Если у вас нет такого, можете воспроизвести паттерн в своём классе:

```csharp
class MyResponsibleType : IDisposable
{
  private readonly CancellationTokenSource disposeCancellationTokenSource = new CancellationTokenSource();
 
  internal MyResponsibleType()
  {
    this.JoinableTaskCollection = ThreadHelper.JoinableTaskContext.CreateCollection();
    this.JoinableTaskFactory = ThreadHelper.JoinableTaskContext.CreateFactory(this.JoinableTaskCollection);
  }

  JoinableTaskFactory JoinableTaskFactory { get; }
  JoinableTaskCollection JoinableTaskCollection { get; }
  
  /// <summary>
  /// Gets a <see cref="CancellationToken"/> that can be used to check if the package has been disposed.
  /// </summary>
  CancellationToken DisposalToken => this.disposeCancellationTokenSource.Token;

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize(this);
  }
  
  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      this.disposeCancellationTokenSource.Cancel();

      try
      {
        // Block Dispose until all async work has completed.
        ThreadHelper.JoinableTaskFactory.Run(this.JoinableTaskCollection.JoinTillEmptyAsync);
      }
      catch (OperationCanceledException)
      {
        // this exception is expected because we signaled the cancellation token
      }
      catch (AggregateException ex)
      {
        // ignore AggregateException containing only OperationCanceledException
        ex.Handle(inner => (inner is OperationCanceledException));
      }
    }
  }
}
```
Если метод возвращает `Task` или `Task<T>`, можно применить метод-расширение `Forget`:

```csharp
DoSomeWork();
DoSomeWorkAsync().Forget(); // no await here
DoABitMore();
```

Рекомендуется вместо `Forget` применять метод `FileAndForget(string)`, который не только обработает засбоившую задачу, но и запишет телеметрию в лог Visual Studio.

```csharp
DoSomeWork();
DoSomeWorkAsync().FileAndForget("vs/YOUR-FEATURE/YOUR-ACTION"); // no await here
DoABitMore();
```
