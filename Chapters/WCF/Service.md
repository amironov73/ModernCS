### WCF: хостинг в сервисе

Допустим, у нас есть простой WCF-сервис, например, с таким интерфейсом:

```csharp
[ServiceContract]
public interface ISimpleCalculator
{
   [OperationContract]
   int Add(int num1, int num2);
 
   [OperationContract]
   int Subtract(int num1, int num2);
 
   [OperationContract]
   int Multiply(int num1,int num2);
 
   [OperationContract]
   double Divide(int num1, int num2);
}
```

и мы хотим захостить его в сервисном процессе Windows. Как это сделать?

Оказывается, ничего сложного нет. Создаём в Visual Studio проект типа «Windows Service» и в новеньком сервисе переопределяем методы OnStart и OnStop:

```csharp
public partial class CalculatorService 
    : ServiceBase
{
    public CalculatorService()
    {
        InitializeComponent();
    }
 
    private ServiceHost _host;
 
    protected override void OnStart(string[] args)
    {
        if (_host != null)
        {
            _host.Close();
        }
 
        Type serviceType = typeof (SimpleCalculatorImplementation);
        Uri serviceUri = new Uri("http://localhost:8080/CalculatorService");
        _host = new ServiceHost(serviceType, serviceUri);
        BasicHttpBinding binding = new BasicHttpBinding
        {
            HostNameComparisonMode = HostNameComparisonMode.Exact,
            Security = null
        };
        ServiceMetadataBehavior behavior = new ServiceMetadataBehavior
        {
            HttpGetEnabled = true
        };
        _host.Description.Behaviors.Add(behavior);
 
        _host.Open();
    }
 
    protected override void OnStop()
    {
        if (_host != null)
        {
            _host.Close();
            _host = null;
        }
    }
}
```

Хотя этим сервисом можно пользоваться и так, добавим в него для большего комфорта инсталлятор:

```csharp
[RunInstaller(true)]
public class WinServiceInstaller : Installer
{
    private ServiceProcessInstaller process;
    private ServiceInstaller service;
 
    public WinServiceInstaller()
    {
        process = new ServiceProcessInstaller();
        process.Account = ServiceAccount.NetworkService;
        service = new ServiceInstaller();
        service.ServiceName = "CalculatorService";
        service.DisplayName = "CalculatorService";
        service.Description = "Host process for WCF service";
        service.StartType = ServiceStartMode.Automatic;
        Installers.Add(process);
        Installers.Add(service);
    }
}
```

Кроме того, можно предусмотреть установку и управление сервисом из командной строки:

```csharp
class ServiceInstallerUtility
{
    private static readonly string exePath 
        = Assembly.GetExecutingAssembly().Location;
 
    // ================================================================
 
    public static bool Install()
    {
        try
        {
            ManagedInstallerClass.InstallHelper
                (
                    new[] {exePath}
                );
        }
        catch
        {
            return false;
        }
        return true;
    }
 
    // ================================================================
 
    public static bool Uninstall()
    {
        try
        {
            ManagedInstallerClass.InstallHelper
                (
                    new[] {"/u", exePath}
                );
        }
        catch
        {
            return false;
        }
        return true;
    }
}
 
// ====================================================================
 
static class Program
{
    const string ServiceName = "CalculatorService";
 
    static void RunService ()
    {
        ServiceBase[] ServicesToRun = new ServiceBase[] 
                                          { 
                                              new CalculatorService() 
                                          };
        ServiceBase.Run(ServicesToRun);
    }
 
    // ================================================================
 
    static void InstallService ( )
    {
        ServiceInstallerUtility.Install ();
    }
 
    // ================================================================
 
    static void UninstallService ( )
    {
        ServiceInstallerUtility.Uninstall ();
    }
 
    // ================================================================
 
    static void StartService ( )
    {
        try
        {
            ServiceController controller
                = new ServiceController (ServiceName);
            controller.Start ();
        }
        catch ( Exception ex )
        {
            Console.WriteLine ( ex );
        }
    }
 
    // ================================================================
 
    static void StopService ( )
    {
        try
        {
            ServiceController controller
                = new ServiceController(ServiceName);
            controller.Stop ();
        }
        catch ( Exception ex )
        {
            Console.WriteLine ( ex );
        }
    }
 
    // ================================================================
 
    static void RunAsConsoleApplication ( )
    {
      // Run as console application
    }
 
    // ================================================================
 
    static void ShowVersion()
    {
        Console.WriteLine
            (
                "Version info"
            );
    }
 
    // ================================================================
 
    static void ShowHelp ()
    {
        Console.WriteLine 
            (
                "Help"
            );
    }
 
    // ================================================================
 
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main
        (
            string[] args
        )
    {
        if ( args.Length == 0 )
        {
            if ( Environment.UserInteractive )
            {
                RunAsConsoleApplication ();
            }
            else
            {
                RunService ();
            }
        }
        else if ( args.Length == 1 )
        {
            switch ( args [ 0 ].ToLowerInvariant() )
            {
                case "-install":
                case "/install":
                case "-i":
                case "/i":
                    InstallService ();
                    break;
 
                case "-uninstall":
                case "/uninstall":
                case "-u":
                case "/u":
                    UninstallService ();
                    break;
 
                case "-start":
                case "/start":
                case "-run":
                case "/run":
                case "-r":
                case "/r":
                case "/1":
                case "-1":
                    StartService ();
                    break;
 
                case "-stop":
                case "/stop":
                case "-s":
                case "/s":
                case "-0":
                case "/0":
                    StopService ();
                    break;
 
                case "-console":
                case "/console":
                case "-c":
                case "/c":
                    RunAsConsoleApplication ();
                    break;
 
                case "-version":
                case "/version":
                case "-v":
                case "/v":
                    ShowVersion();
                    break;
 
                default:
                    ShowHelp ();
                    break;
            }
        }
        else
        {
            ShowHelp ();
        }
    }
}
```

