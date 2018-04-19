### Пакет Docker.DotNet

Docker.DotNet — (сравнительно) официальный клиент для Docker Remote API от Microsoft. Поддерживает:

* .NET Framework 4.5 и выше;
* .NET Standard 1.6 и выше.

GitHub: https://github.com/Microsoft/Docker.DotNet, NuGet: https://www.nuget.org/packages/Docker.DotNet/

Использование:

```csharp
using Docker.DotNet;

// Создание клиента
Uri uri = new Uri("http://ubuntu-docker.cloudapp.net:4243");
DockerClient client = new DockerClientConfiguration(uri)
     .CreateClient();

// К локальному серверу на Windows можно подключиться
// с помощью именованных каналов
Uri uri = new Uri("npipe://./pipe/docker_engine");
DockerClient client = new DockerClientConfiguration(uri)
     .CreateClient();

// Получение списка контейнеров
IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync
    (
        new ContainersListParameters { Limit = 10 }
    );

// Создание образа путём выкачивания с хаба
// Вместо AuthConfig можно указать null
Stream stream  = await client.Images.CreateImageAsync
    (
        new ImagesCreateParameters
        {
            Parent = "fedora/memcached",
            Tag = "alpha",
        },
        new AuthConfig
        {
            Email = "ahmetb@microsoft.com",
            Username = "ahmetalpbalkan",
            Password = "pa$$w0rd"
        }
    );

// Запуск образа
await client.Containers.StartContainerAsync
    (
        "39e3317fd258",
        new HostConfig
        {
            DNS = new[] { "8.8.8.8", "8.8.4.4" }
        }
    );

// Остановка контейнера
var stopped = await client.Containers.StopContainerAsync
    (
        "39e3317fd258",
        new ContainerStopParameters
        {
            WaitBeforeKillSeconds = 30
        },
        CancellationToken.None
    );
```
