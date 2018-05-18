### Класс Application

```csharp
namespace System.Windows
{
    public class Application
    {
        public Application();

        public int Run();

        public int Run(Window window);

        public void Shutdown();

        public void Shutdown(int exitCode);

        public object FindResource(object resourceKey);

        public object TryFindResource(object resourceKey);

        public static void LoadComponent(Object component, Uri resourceLocator);

        public static object LoadComponent(Uri resourceLocator);

        public static StreamResourceInfo GetResourceStream(Uri uriResource);

        public static StreamResourceInfo GetContentStream(Uri uriContent);

        public static StreamResourceInfo GetRemoteStream(Uri uriRemote);

        public static string GetCookie(Uri uri);

        public static void SetCookie(Uri uri, string value);

        static public Application Current{ get; }

        public WindowCollection Windows { get; }

        public Window MainWindow { get; set; }

        public ShutdownMode ShutdownMode { get; set; }

        public ResourceDictionary Resources { get; set; }

        public Uri StartupUri { get; set; }

        public IDictionary Properties { get; }

        public static Assembly ResourceAssembly { get; set; }

        public event StartupEventHandler Startup;

        public event ExitEventHandler Exit; 

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        public event SessionEndingCancelEventHandler SessionEnding;

        public event DispatcherUnhandledExceptionEventHandler DispatcherUnhandledException;

        public event NavigatingCancelEventHandler Navigating;

        public event NavigatedEventHandler Navigated;

        public event NavigationProgressEventHandler NavigationProgress;

        public event NavigationFailedEventHandler NavigationFailed;

        public event LoadCompletedEventHandler LoadCompleted;

        public event NavigationStoppedEventHandler NavigationStopped;

        public event FragmentNavigationEventHandler FragmentNavigation;

        protected virtual void OnStartup(StartupEventArgs e);

        protected virtual void OnExit(ExitEventArgs e);

        protected virtual void OnActivated(EventArgs e);

        protected virtual void OnDeactivated(EventArgs e);

        protected virtual void OnSessionEnding(SessionEndingCancelEventArgs e);

        protected virtual void OnNavigating(NavigatingCancelEventArgs e);

        protected virtual void OnNavigated(NavigationEventArgs e);

        protected virtual void OnNavigationProgress(NavigationProgressEventArgs e);

        protected virtual void OnNavigationFailed(NavigationFailedEventArgs e);

        protected virtual void OnLoadCompleted(NavigationEventArgs e);

        protected virtual void OnNavigationStopped(NavigationEventArgs e);

        protected virtual void OnFragmentNavigation(FragmentNavigationEventArgs e);
    }

    public enum ShutdownMode : byte
    {
        OnLastWindowClose = 0,
 
        OnMainWindowClose = 1,
 
        OnExplicitShutdown
    }
}
```
