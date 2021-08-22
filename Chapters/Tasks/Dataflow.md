### System.Threading.Tasks.Dataflow

Когда-то это была библиотека, [выложенная на NuGet](https://www.nuget.org/packages/Microsoft.Tpl.Dataflow/) (она и сейчас там есть), а теперь это часть .NET 5 (т. е. ничего подключать не нужно, всё работает само «магически»). Старый код спокойно переносится «один-в-один»:

```c#
using System;
using System.Threading.Tasks.Dataflow;

var linkOptions = new DataflowLinkOptions
{
    PropagateCompletion = true
};
var executionOptions = new ExecutionDataflowBlockOptions()
{
    EnsureOrdered = true
};

var numberDoubler = new TransformBlock<int, int>(n => n * 2, executionOptions);
var batchBlock = new BatchBlock<int>(10);
var numberPrinter = new ActionBlock<int[]>(array => Console.WriteLine(string.Join(", ", array)));
numberDoubler.LinkTo(batchBlock, linkOptions);
batchBlock.LinkTo(numberPrinter, linkOptions);

for (var i = 0; i < 100; i++)
{
    numberDoubler.Post(i);
}

numberDoubler.Complete();
await numberPrinter.Completion;
Console.WriteLine("THAT'S ALL FOLKS!");
```

выведет:

```
0,2,4,6,8,10,12,14,16,18
20,22,24,26,28,30,32,34,36,38
40,42,44,46,48,50,52,54,56,58
60,62,64,66,68,70,72,74,76,78
80,82,84,86,88,90,92,94,96,98
100,102,104,106,108,110,112,114,116,118
120,122,124,126,128,130,132,134,136,138
140,142,144,146,148,150,152,154,156,158
160,162,164,166,168,170,172,174,176,178
180,182,184,186,188,190,192,194,196,198
THAT'S ALL FOLKS!
```

"Прокрутка вручную":

```c#
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
 
class Program
{
    static async Task<int> Consume(ISourceBlock<string> source)
    {
        var result = 0;
        while (await source.OutputAvailableAsync())
        {
            var data = source.Receive();
            result += data.Length;
        }
 
        return result;
    }
 
    static void Produce(ITargetBlock<string> target)
    {
        for (var i = 0; i < 100000000; i++)
        {
            var text = "abc";
            target.Post(text);
        }
 
        target.Complete();
    }
 
    static void Main()
    {
        var buffer = new BufferBlock<string>();
        var consumer = Consume(buffer);
        Produce(buffer);
        consumer.Wait();
        Console.WriteLine(consumer.Result);
    }
}
```

Пример с `async`:

```c#
var block = new ActionBlock<string>(async hostName =>
{
    var ipAddresses = await Dns.GetHostAddressesAsync(hostName);
    Console.WriteLine(ipAddresses[0]);
});
 
block.Post("google.com");
block.Post("reddit.com");
block.Post("stackoverflow.com");
 
block.Complete();
await block.Completion;
```

Документация теперь [интегрирована в официальный сайт](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.dataflow?view=net-5.0).

Существует независимая реализация, поддерживающая .Net, начиная с 4.0: https://github.com/cuteant/dotnet-tpl-dataflow
