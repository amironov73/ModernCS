### Класс DispatcherTimer

```csharp
namespace System.Windows.Threading 
{
    public class DispatcherTimer
    {
        public DispatcherTimer();

        public DispatcherTimer(DispatcherPriority priority);

        public DispatcherTimer(DispatcherPriority priority, Dispatcher dispatcher);

        public DispatcherTimer(TimeSpan interval, DispatcherPriority priority, EventHandler callback, Dispatcher dispatcher);

        public Dispatcher Dispatcher { get; }

        public bool IsEnabled { get; set; }

        public TimeSpan Interval { get; set; }

        public void Start();

        public void Stop();

        public event EventHandler Tick;

        public object Tag { get; set; }
    }
}
```


