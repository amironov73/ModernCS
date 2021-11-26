### HttpClient

https://docs.microsoft.com/en-us/dotnet/core/extensions/http-client

Тип `HttpClient` впервые появился в .NET Framework 4.5 в 2012 году, по нынешним временам, чуть ли не при динозаврах. 

`HttpClient` используется для выполнения HTTP-запросов и обработки ответов HTTP из веб-ресурсов, определенных `Uri`. При передаче интернет-трафика в большинстве случаев используется протокол HTTP.

Хотя `HttpClient` можно использовать независимо, интерфейс `IHttpClientFactory` выступает в качестве уровня абстракции для фабрики, который может создавать экземпляры `HttpClient` с настраиваемыми конфигурациями. `IHttpClientFactory` впервые появился в .NET Core 2.1.

### Основной сценарий

Подключаем NuGet-пакеты:

* Microsoft.Extensions.DependencyInjection;
* Microsoft.Extensions.Hosting;
* Microsoft.Extensions.Http;
* Microsoft.Extensions.Logging.

Простейший пример:

```c#
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#nullable enable

class Program
{
    public static async Task Main (string[] args)
    {
        using var host = Host.CreateDefaultBuilder (args)
            .ConfigureServices (services =>
            {
                services.AddHttpClient ();
                services.AddTransient <TodoService> ();
            })
            .Build ();

        var service = host.Services.GetRequiredService<TodoService>();
        var todo = await service.Fetch(1);
        Console.WriteLine(todo);
        
    } // method Main
    
} // class Program

class Todo
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("completed")]
    public bool Completed { get; set; }

    public override string ToString() => $"{Id}: {Title}";
    
} // class Todo

class TodoService
{
    private readonly IHttpClientFactory _factory;
    
    public TodoService (IHttpClientFactory factory) => 
        _factory = factory;

    public async Task<Todo?> Fetch (int id)
    {
        var url = $"https://jsonplaceholder.typicode.com/todos/{id}";
        var client = _factory.CreateClient();
        var result = await client.GetFromJsonAsync<Todo> (url);
        
        return result;
    }
    
} // class TodoService
```

### Именованные клиенты

Именованных клиентов может быть сколько угодно, каждый настраивается независимо от других.

Конфигурируем так:

```c#
services.AddHttpClient
    (
        "todo",
        client =>
        {
            client.BaseAddress = new Uri ("https://jsonplaceholder.typicode.com");
        }
    );
```

используем так:

```c#
var url = $"todos/{id}";
var client = _factory.CreateClient("todo");
var result = await client.GetFromJsonAsync<Todo> (url);
```

### Типизированные клиенты

Отличаются от именованных тем, что привязываются к типу сервиса, а не к имени.

Конфигурируем так:

```c#
services.AddHttpClient<TodoService>
    (
        client =>
        {
            client.BaseAddress = new Uri ("https://jsonplaceholder.typicode.com");
        }
    );
```

используем так:

```c#
class TodoService
{
    private readonly HttpClient _client;
    
    public TodoService
        (
            HttpClient client
        )
    {
        _client = client;
    }

    public async Task<Todo?> Fetch (int id)
    {
        var url = $"https://jsonplaceholder.typicode.com/todos/{id}";
        var result = await _client.GetFromJsonAsync<Todo>
            (
                url
            );
        
        return result;
    }
    
} // class TodoService
```

### Жизненный цикл HttpClient

При каждом вызове `CreateClient` в `IHttpClientFactory` возвращается новый экземпляр `HttpClient`. Для каждого клиента создается один экземпляр `HttpClientHandler`. Фабрика обеспечивает управление временем существования экземпляров `HttpClientHandler`.

`IHttpClientFactory` объединяет в пул все экземпляры `HttpClientHandler`, созданные фабрикой, чтобы уменьшить потребление ресурсов. Экземпляр `HttpClientHandler` можно использовать повторно из пула при создании экземпляра `HttpClient`, если его время существования еще не истекло.

Создавать пулы обработчиков желательно, так как каждый обработчик обычно управляет собственным базовым HTTP-подключением. Создание лишних обработчиков может привести к задержке подключения. Некоторые обработчики поддерживают подключения открытыми в течение неопределенного периода, что может помешать обработчику отреагировать на изменения DNS.

Время существования обработчика по умолчанию — две минуты. Чтобы переопределить значение по умолчанию, вызовите для каждого клиента `SetHandlerLifetime` в `IServiceCollection`:

```c#
services.AddHttpClient("Named.Client")
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));
```

Как правило, экземпляры `HttpClient` можно рассматривать как объекты, которые не требуют удаления. Высвобождение отменяет исходящие запросы и гарантирует, что указанный экземпляр `HttpClient` не может использоваться после вызова `Dispose`. `IHttpClientFactory` отслеживает и высвобождает ресурсы, используемые экземплярами `HttpClient`.

До появления `IHttpClientFactory` один экземпляр `HttpClient` часто сохраняли в активном состоянии в течение длительного времени. После перехода на `IHttpClientFactory` это уже не нужно.

### Настройка HttpMessageHandler

```c#
services.AddHttpClient("Named.Client")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseDefaultCredentials = true
        };
    });
```
