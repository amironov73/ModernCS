### LinqAF

LinqAF — независимая реализация LINQ-to-Objects, использующая минимально возможное размещение объектов в куче. Там, где стандартная реализация содержит хотя бы один оператор new, LinqAF обходится вообще без new. В некоторых сценариях это может сильно сократить вмешательство GC в работу библиотеки.

GitHub: https://github.com/kevin-montrose/LinqAF, NuGet: https://github.com/kevin-montrose/LinqAF, серия статей автора, в которых раскрывается механизм работы LinqAF: https://kevinmontrose.com/2018/01/16/linqaf-a-series-of-questionable-ideas/. Поддерживается .NET FW 4.5.2 и .NET Standard 2.0.

Как этим пользоваться:

```csharp
using System;
 
using LinqAF;
 
class Program
{
    static void Main()
    {
        var sequence = Enumerable.Range(0, 100)
                .SelectMany(x => new [] { x, x * 2 })
                .Reverse()
                .Select(y => y.ToString());
  
        foreach(var str in sequence)
        {
            Console.WriteLine(str);
        }
    }
}
```

Обратите внимание, что для совместимости с обычным LINQ, важно использовать именно var!

Из недостатков библиотеки следует заметить её «недетский» размер: 23 Мб. Впрочем, по нынешним временам, не так уж много.
