### Атрибут CallerFilePath

Давным-давно, ещё во времена классического .NET Framework 4.5, появился атрибут [CallerFilePathAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerfilepathattribute?view=net-5.0), который всё это время успешно избегал моего внимания. А ведь полезнейший атрибут!

```c#
using System;
using System.IO;
using System.Runtime.CompilerServices;
 
DoSomething();
 
void DoSomething
    (
        [CallerFilePath] string name = null,
        [CallerLineNumber] int line = 0
    )
{
    var shortName = Path.GetFileName(name);
    Console.WriteLine($"Called from {shortName}, line {line}");
}
```