### Клиент и сервер DNS для .NET

Библиотека DNS представляет функционал клиента и сервера DNS для .NET Standard 2.0. GitHub: https://github.com/kapetan/dns, NuGet: https://www.nuget.org/packages/DNS/.

Клиентская часть очень простая:

```csharp
using System;
using System.Linq;
using System.Net;
 
using DNS.Client;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
 
class Program
{
    static void Main()
    {
        DnsClient client = new DnsClient("8.8.8.8");
        // Запрашиваем IPv4-адрес для foo.com
        IResponse response  = client.Resolve("foo.com", RecordType.A).Result;
        IPAddress[] addresses = response.AnswerRecords
            .Cast<IPAddressResourceRecord>()
            .Select(r => r.IPAddress)
            .ToArray();
        foreach (IPAddress address in addresses)
        {
            Console.WriteLine(address);
        }
    }
}

// Напечатает: 
// 23.23.86.44
```

Аналогично можно запросить IPv6-адрес:

```csharp
IResponse response  = client.Resolve("google.com", RecordType.AAAA).Result;
// Остальные строки прежние.

// Напечатает:
// 2a00:1450:4010:c05::64
```

Обратная операция (определение домена по адресу) тоже очень проста:

```csharp
DnsClient client = new DnsClient("8.8.8.8");
// По идее, это один из адресов google.com
string domain = client.Reverse("173.194.222.100").Result;
Console.WriteLine(domain);

// Напечатает:
// lo-in-f100.1e100.net
// Нам ответили: это Гугл
```

Прокси сервер тоже не очень сложен:

```csharp
using System;
 
using DNS.Server;
 
class Program
{
    static void Main()
    {
        // Proxy to google's DNS
        MasterFile masterFile = new MasterFile();
        DnsServer server = new DnsServer(masterFile, "8.8.8.8");
 
        // Resolve these domain to localhost
        masterFile.AddIPAddressResourceRecord("google.com", "127.0.0.1");
        masterFile.AddIPAddressResourceRecord("github.com", "127.0.0.1");
 
        // Log every request
        server.Requested += (request) => Console.WriteLine(request);
        // On every successful request log the request and the response
        server.Responded += (request, response) =>
        {
            Console.WriteLine("{0} => {1}", request, response);
        };
        // Log errors
        server.Errored += (e) => Console.WriteLine(e.Message);
 
        // Start the server (by default it listens on port 53)
        // One can specify different port number, e. g. 1111
        server.Listen().Wait();
    }
}
```
