### MultiValueDictionary

Microsoft опубликовала NuGet-пакет [Microsoft.Experimental.Collections](https://www.nuget.org/packages/Microsoft.Experimental.Collections), в котором, правда, всего одна коллекция — MultiValueDictionary. Этот словарь позволяет ассоциировать с одним ключом несколько значений. Пользоваться им чрезвычайно легко:

```csharp
using System;
using System.Collections.Generic;
 
class Program
{
    static void Main()
    {
        var dictionary = new MultiValueDictionary<int, string>
        {
            {1, "one"},
            {1, "two"},
            {1, "three"}
        };
 
        var values = dictionary[1]; // IReadOnlyCollection<string>
        Console.WriteLine(string.Join(", ", values));
    }
}
```

Пакет совместим с:

* .NET Framework 4.5
* Windows 8
* Windows Phone Silverlight 8
* Portable Class Libraries

