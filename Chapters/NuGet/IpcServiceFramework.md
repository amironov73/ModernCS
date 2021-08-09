### IpcServiceFramework

IpcServiceFramework - простой и легковесный фреймворк для межпроцессной коммуникации, ориентированный на .NET Core 3.1 и современный .NET 5/6. Поддерживаются именованные каналы и TCP/IP.

GitHub: https://github.com/jacqueskang/IpcServiceFramework, NuGet: [JKang.IpcServiceFramework.(Client|Hosting).(NamedPipe|Tcp)](https://www.nuget.org/profiles/kylinwater).

Вот простой пример.

Сначала создаем библиотеку классов с единственным определением интерфейса:

```c#
namespace CommonInterfaces
{
    public interface IStringReverser
    {
        string ReverseString(string input);
    }
}
```

Теперь создаем консольное серверное приложение. Вот реализация сервиса:

```c#
using System;

using CommonInterfaces;

namespace ServerApp
{
    public sealed class ReverserService : IStringReverser
    {
        public string ReverseString(string input)
        {
            var characters = input.ToCharArray();
            Array.Reverse(characters);
            
            return new string(characters);
        }
    }
}
```

А вот его публикация (не забудьте добавить ссылку на `Microsoft.Extensions.Hosting`!):

```c#
using System.Net;

using CommonInterfaces;

using JKang.IpcServiceFramework.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices(services =>
            {
                services.AddScoped<IStringReverser, ReverserService>();
            })
            .ConfigureIpcHost(builder =>
            {
                builder.AddTcpEndpoint<IStringReverser>(IPAddress.Loopback, 8888);
            })
            .ConfigureLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
            });
    }
}
```

Наконец, клиентское приложение:

```c#
using System;
using System.Net;

using CommonInterfaces;

using JKang.IpcServiceFramework.Client;

using Microsoft.Extensions.DependencyInjection;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddTcpIpcClient<IStringReverser>("SimpleClient", IPAddress.Loopback, 8888)
                .BuildServiceProvider();

            var factory = services.GetRequiredService<IIpcClientFactory<IStringReverser>>();
            var client = factory.CreateClient("SimpleClient");

            var input = "HELLO";
            var output = client.InvokeAsync(x => x.ReverseString(input))
                .GetAwaiter().GetResult();
            Console.WriteLine($"{input} => {output}");
        }
    }
}
```

Вот и всё! Можно запускать и пользоваться.
