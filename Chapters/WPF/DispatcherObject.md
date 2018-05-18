### Класс DispatcherObject

```csharp
namespace System.Windows.Threading
{
    // Объект с ассоциированным диспетчером
    public abstract class DispatcherObject
    {
        // Ассоциированный объект-диспетчер
        // null означает, что доступ к объекту разрешен из любого потока
        public Dispatcher Dispatcher { get; }

        // Есть ли доступ к объекту из текущего потока
        public bool CheckAccess();
    }

    // Проверка UI-потока.
    public sealed class Dispatcher
    {
        public static Dispatcher CurrentDispatcher { get; }

        public Thread Thread { get; }

        public bool HasShutdownStarted { get; }

        public bool HasShutdownFinished { get; }

        public event EventHandler ShutdownStarted;

        public event EventHandler ShutdownFinished;

        public static Dispatcher FromThread(Thread thread);

        public bool CheckAccess()
        {
            return Thread == Thread.CurrentThread;
        }

        public void VerifyAccess()
        {
            if(!CheckAccess())
            {
                throw new InvalidOperationException("Error!");
            }
        }

        public void BeginInvokeShutdown(DispatcherPriority priority);

        public void InvokeShutdown();

        public static void Run();

        public static void PushFrame(DispatcherFrame frame);

        public static void ExitAllFrames();

        public static DispatcherPriorityAwaitable Yield();

        public static DispatcherPriorityAwaitable Yield(DispatcherPriority priority);

        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method);

        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg);

        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args);

        public DispatcherOperation BeginInvoke(Delegate method, params object[] args);

        public DispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority, params object[] args);

        public void Invoke(Action callback);

        public void Invoke(Action callback, DispatcherPriority priority);

        public void Invoke(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);

        public void Invoke(Action callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout);

        public TResult Invoke<TResult>(Func<TResult> callback);

        public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority);

        public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken);

        public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout);

        public DispatcherOperation InvokeAsync(Action callback);

        public DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority);

        public DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);

        public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback);

        public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority);

        public DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken);

        public object Invoke(DispatcherPriority priority, Delegate method);

        public object Invoke(DispatcherPriority priority, Delegate method, object arg);

        public object Invoke(DispatcherPriority priority, Delegate method, object arg, params object[] args);

        public object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method);

        public object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method, object arg);

        public object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method, object arg, params object[] args);

        public object Invoke(Delegate method, params object[] args);

        public object Invoke(Delegate method, DispatcherPriority priority, params object[] args);

        public object Invoke(Delegate method, TimeSpan timeout, params object[] args);

        public object Invoke(Delegate method, TimeSpan timeout, DispatcherPriority priority, params object[] args);

        public DispatcherProcessingDisabled DisableProcessing();

        public static void ValidatePriority(DispatcherPriority priority, string parameterName);

        public DispatcherHooks Hooks { get; }

        public event DispatcherUnhandledExceptionFilterEventHandler UnhandledExceptionFilter;

        public event DispatcherUnhandledExceptionEventHandler UnhandledException;
    }

    public enum DispatcherPriority
    {
        /// <summary>
        /// This is an invalid priority.
        /// </summary>
        Invalid = -1,
 
        /// <summary>
        /// Operations at this priority are not processed.
        /// </summary>
        Inactive = 0,
 
        /// <summary>
        /// Operations at this priority are processed when the system
        /// is idle.
        /// </summary>
        SystemIdle,
 
        /// <summary>
        /// Operations at this priority are processed when the application
        /// is idle.
        /// </summary>
        ApplicationIdle,
 
        /// <summary>
        /// Operations at this priority are processed when the context
        /// is idle.
        /// </summary>
        ContextIdle,
 
        /// <summary>
        /// Operations at this priority are processed after all other
        /// non-idle operations are done.
        /// </summary>
        Background,
 
        /// <summary>
        /// Operations at this priority are processed at the same
        /// priority as input.
        /// </summary>
        Input,
 
        /// <summary>
        /// Operations at this priority are processed when layout and render is
        /// done but just before items at input priority are serviced. Specifically
        ///     this is used while firing the Loaded event
        /// </summary>
        Loaded,
 
        /// <summary>
        /// Operations at this priority are processed at the same
        /// priority as rendering.
        /// </summary>
        Render,
 
        /// <summary>
        /// Operations at this priority are processed at the same
        /// priority as data binding.
        /// </summary>
        DataBind,
 
        /// <summary>
        /// Operations at this priority are processed at normal priority.
        /// </summary>
        Normal,
 
        /// <summary>
        /// Operations at this priority are processed before other
        /// asynchronous operations.
        /// </summary>
        Send
    }

    ublic enum DispatcherOperationStatus
    {
        /// <summary>
        /// The operation is still pending.
        /// </summary>
        Pending,
 
        /// <summary>
        /// The operation has been aborted.
        /// </summary>
        Aborted,
 
        /// <summary>
        /// The operation has been completed.
        /// </summary>
        Completed,
        
        /// <summary>
        /// The operation has started executing, but has not completed yet.
        /// </summary>
        Executing
    }

    public class DispatcherFrame : DispatcherObject
    {
        public DispatcherFrame();

        public DispatcherFrame(bool exitWhenRequested);

        public bool Continue { get; set; }
    }

    public class DispatcherOperation
    {
        public Dispatcher Dispatcher { get; }

        public DispatcherPriority Priority { get; set; }

        public DispatcherOperationStatus Status { get; }

        public Task Task { get; }

        public TaskAwaiter GetAwaiter();

        public DispatcherOperationStatus Wait();

        public DispatcherOperationStatus Wait(TimeSpan timeout);

        public bool Abort();

        public object Result { get; }

        public event EventHandler Aborted;

        public event EventHandler Completed;
    }
}
```
