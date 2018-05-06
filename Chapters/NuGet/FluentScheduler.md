### FluentScheduler

GitHub: https://github.com/fluentscheduler/FluentScheduler, NuGet: https://www.nuget.org/packages/FluentScheduler

Поддерживает .NET Framework 4.0 и .NET Standard 1.4.

```csharp
using System;

using FluentScheduler;

class Program
{
    static void Main()
    {
        JobManager.AddJob
            (
                () => Console.WriteLine("Hello from Job"),
                s => s.ToRunEvery(1).Seconds()
            );

        Console.ReadKey();

        // stop and wait for the running jobs to finish
        JobManager.StopAndBlock();
    }
}
```

