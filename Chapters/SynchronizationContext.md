### Класс SynchronizationContext

SynchronizationContext — инструмент маршалинга («проброса») кода из одного потока (thread) в другой. Чаще всего к нему прибегают, когда хотят выполнить обновление UI-контрола из фонового потока. Если позвать любой отрисовывающий метод контрола напрямую, скорее всего, прилетит исключение. Вот тут-то и пригождается SynchronizationContext, который сам разбирается с тем, как выполнить наш код в правильном потоке.

См. https://msdn.microsoft.com/en-us/library/system.threading.synchronizationcontext(v=vs.110).aspx. Статья: https://msdn.microsoft.com/magazine/gg598924.aspx. Ещё статья: https://www.codeproject.com/Articles/31971/Understanding-SynchronizationContext-Part-I.

```csharp
namespace System.Threading
{
  public class SynchronizationContext
  {
    public static SynchronizationContext Current { get; }
 
    public SynchronizationContext();
 
    public virtual SynchronizationContext CreateCopy();
    protected virtual void Finalize();
    public bool IsWaitNotificationRequired();
    public virtual void OperationCompleted();
    public virtual void OperationStarted();
    public virtual void Post
      (
        SendOrPostCallback d,
        object state
      );
    public virtual void Send
      (
        SendOrPostCallback d,
        object state
      );
    public static void SetSynchronizationContext
      (
        SynchronizationContext syncContext
      );
    protected void SetWaitNotificationRequired();
    public virtual int Wait
      (
        IntPtr[] waitHandles,
        bool waitAll,
        int millisecondsTimeout
      );
    protected static int WaitHelper
      (
        IntPtr[] waitHandles,
        bool waitAll,
        int millisecondsTimeout
      );
  }
}
```
Чаще всего SynchronizationContext используется так:
```csharp
// где-то в коде
TextBox editControl  = ...;
Button stopButton = ...;
 
...
 
SynchronizationContext uiContext = SynchronizationContext.Current;
 
...
 
uiContext.Post
  (
    () =>
    {
      editControl.Text = "Половина дела сделана!";
      stopButton.Enabled = false;
      // и так далее
    }
  );
```
Что это даёт по сравнению с Control.Invoke? Да почти ничего! 🙂 Но знать о SynchronizationContext не помешает!

Для приложений WinForms вместе с первым визуальным контролом создаётся и регистрируется как Current WindowsFormsSynchronizationContext.
