### System.Collections.Immutable

Для Microsoft.NET 4.5 доступно новое пространство имён System.Collections.Immutable, разработанное Microsoft: https://www.nuget.org/packages/System.Collections.Immutable. Что называется, дождались!

* ImmutableArray
* ImmutableDictionary
* ImmutableHashSet
* ImmutableList
* ImmutableQueue
* ImmutableSortedDictionary
* ImmutableSortedSet
* ImmutableStack

Пользоваться легко и просто:

```csharp
using System;
using System.Collections.Immutable;
 
class Program
{
    static void Main()
    {
        int[] ordinaryArray = {1, 2, 3, 4, 5};
        ImmutableArray<int> immutableArray 
            = ImmutableArray.Create(ordinaryArray);
        Console.WriteLine("Length={0}", immutableArray.Length);
        Console.WriteLine("Item[1]={0}",immutableArray[1]);
 
        immutableArray[1] = 2; // не выйдет!
    }
}
```

Документация здесь: https://msdn.microsoft.com/en-us/library/system.collections.immutable(v=vs.111).aspx
