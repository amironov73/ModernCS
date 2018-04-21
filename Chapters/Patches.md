### Как узнать, какие патчи для .NET установлены

Сведения и код заимствованы из статьи MSDN https://msdn.microsoft.com/en-us/library/hh925567(v=vs.110).aspx.

Все установленные обновления для Microsoft .NET перечислены в виде подразделов в разделе реестра HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Updates.

Вот программа, которая перебирает вышеупомянутые подразделы:

```csharp
using System;
using Microsoft.Win32;
 
public class GetUpdateHistory
{
  public static void Main()
  {
    using (RegistryKey baseKey = RegistryKey
      .OpenBaseKey
        (
          RegistryHive.LocalMachine, 
          RegistryView.Registry32
        )
      .OpenSubKey(@"SOFTWARE\Microsoft\Updates"))
      {
        foreach (string baseKeyName in baseKey.GetSubKeyNames())
        {
          if (baseKeyName.Contains(".NET Framework") 
             || baseKeyName.StartsWith("KB") 
             || baseKeyName.Contains(".NETFramework"))
          {
            using (RegistryKey updateKey = baseKey
                    .OpenSubKey(baseKeyName))
            {
              string name = (string)updateKey
                             .GetValue("PackageName", "");
              Console.WriteLine(baseKeyName + "  " + name);
              foreach (string kbKeyName in updateKey.GetSubKeyNames())
              {
                using (RegistryKey kbKey = updateKey
                         .OpenSubKey(kbKeyName))
                {
                  name = (string)kbKey.GetValue("PackageName", "");
                  Console.WriteLine("  " + kbKeyName + "  " + name);
                  if (kbKey.SubKeyCount > 0)
                  {
                    foreach (string sbKeyName in
                               kbKey.GetSubKeyNames())
                    {
                      using (RegistryKey sbSubKey = kbKey
                               .OpenSubKey(sbKeyName))
                      {
                        name = (string)sbSubKey
                                .GetValue("PackageName", "");
                        if (name == "")
                          name = (string)sbSubKey
                                .GetValue("Description", "");
                        Console.WriteLine("    " + sbKeyName 
                            + "  " + name);
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
  }
}
```

Программа напечатает примерно следующее:

```
Microsoft .NET Framework 3.5 SP1
  KB953595  Hotfix for Microsoft .NET Framework 3.5 SP1 (KB953595)
  SP1
    KB2657424  Security Update for Microsoft .NET Framework 3.5 SP1 (KB2657424)
    KB958484  Hotfix for Microsoft .NET Framework 3.5 SP1 (KB958484)
    KB963707  Update for Microsoft .NET Framework 3.5 SP1 (KB963707)
Microsoft .NET Framework 4 Client Profile
  KB2160841  Security Update for Microsoft .NET Framework 4 Client Profile (KB2160841)
  KB2446708  Security Update for Microsoft .NET Framework 4 Client Profile (KB2446708)
  KB2468871  Update for Microsoft .NET Framework 4 Client Profile (KB2468871)
  KB2478663  Security Update for Microsoft .NET Framework 4 Client Profile (KB2478663)
  KB2518870  Security Update for Microsoft .NET Framework 4 Client Profile (KB2518870)
  KB2533523  Update for Microsoft .NET Framework 4 Client Profile (KB2533523)
  KB2539636  Security Update for Microsoft .NET Framework 4 Client Profile (KB2539636)
  KB2572078  Security Update for Microsoft .NET Framework 4 Client Profile (KB2572078)
  KB2633870  Security Update for Microsoft .NET Framework 4 Client Profile (KB2633870)
  KB2656351  Security Update for Microsoft .NET Framework 4 Client Profile (KB2656351)
Microsoft .NET Framework 4 Extended
  KB2416472  Security Update for Microsoft .NET Framework 4 Extended (KB2416472)
  KB2468871  Update for Microsoft .NET Framework 4 Extended (KB2468871)
  KB2487367  Security Update for Microsoft .NET Framework 4 Extended (KB2487367)
  KB2533523  Update for Microsoft .NET Framework 4 Extended (KB2533523)
  KB2656351  Security Update for Microsoft .NET Framework 4 Extended (KB2656351)
```

