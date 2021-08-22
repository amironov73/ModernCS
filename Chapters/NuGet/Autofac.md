### Autofac

Вообще-то, Autofac – это [автоматическая фабрика из одноименного рассказа Филиппа К. Дика](https://en.wikipedia.org/wiki/Autofac). В рассказе полностью автоматические фабрики, управляемые искусственным интеллектом, сами производили всё необходимое, включая запчасти для самих себя. Похоже, эта способность настолько восхитила некоторых программистов, что они решили назвать [свой IoC-фреймворк](https://autofac.org) Autofac.

NuGet: https://www.nuget.org/packages/Autofac/, GitHub: https://github.com/autofac/Autofac.

Подкупает интуитивность применения Autofac:

```c#
using System;
using Autofac;

var builder = new ContainerBuilder();
builder.RegisterType<ConsoleOutput>().As<IOutput>();

var container = builder.Build();
var output = container.Resolve<IOutput>();
output.Write("Hello, Autofac!");

interface IOutput
{
  void Write(string text);
}

sealed class ConsoleOutput : IOutput
{
  public void Write(string text)
  {
    Console.WriteLine(text);
  }
}
```

Согласитесь, всё просто как «1-2-3»: 1) зарегистрировали типы, 2) затребовали объект с нужным нам интерфейсом, 3) применили его по назначению.

По мере необходимости можно накручивать сложность. Например, нам потребовалось управлять временем жизни объекта, создаваемого контейнером. Вот как это делается:

```c#
using (var scope = container.BeginLifetimeScope())
{
  var output = scope.Resolve<IOutput>();
  output.Write("Hello, Autofac!");
}
```

Можно зарегистрировать тип с несколькими интерфейсами:

```c#
builder.RegisterType<CallLogger>()
  .As<ILogger>()
  .As<ICallInterceptor>();
```

Кроме интерфейсов, можно указать Autofac, «этот тип настолько замечательный, что зарегистрируй-ка его самого по себе» :)

```c#
builder.RegisterType<CallLogger>()
  .AsSelf()
  .As<ILogger>()
  .As<ICallInterceptor>();
```

Далее, допустим, у нашего класса нетривиальный конструктор. Autofac позаботится о том, чтобы создать и передать в него необходимые аргументы. Вот как это выглядит:

```c#
using System;
using Autofac;
 
interface IMath
{
    int Add(int left, int right);
}
 
interface ILogger
{
    void WriteLog(string text);
}
 
sealed class MyMath : IMath
{
    private readonly ILogger _logger;
 
    public MyMath(ILogger logger)
    {
        _logger = logger;
    }
 
    public int Add(int left, int right)
    {
        _logger.WriteLog("Doing math");
 
        return left + right;
    }
}
 
sealed class ConsoleLogger : ILogger
{
    public void WriteLog(string text)
    {
        Console.WriteLine(text);
    }
}
 
class Program
{
    static void Main()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<MyMath>().As<IMath>();
        builder.RegisterType<ConsoleLogger>().As<ILogger>();
 
        var container = builder.Build();
        var math = container.Resolve<IMath>();
        Console.WriteLine("Result is {0}", math.Add(1, 2));
    }
}
```

Можно указать конструктор, который должен использоваться:

```c#
builder.RegisterType<MyMath>()
  .UsingConstructor(typeof(ILogger), typeof(IConfigReader));
```

или вообще заранее создать объект-синглтон, который будет храниться в контейнере и выдаваться по требованию:

```c#
var output = new StringWriter();
builder.RegisterInstance(output).As<TextWriter>();
```

Можно указать, что наш объект не должен погибать при разрушении контейнера:

```c#
var output = new StringWriter();
builder.RegisterInstance(output)
  .As<TextWriter>()
  .ExternallyOwned();
```

Можно задать лямбду, ответственную за создание нужного экземпляра:

```c#
builder.Register(c => new A(c.Resolve<B>()));
```

Лямбда может быть довольно сложной, с дополнительными параметрами:

```c#
builder.Register<CreditCard>(
  (c, p) =>
    {
      var accountId = p.Named<string>("accountId");
      if (accountId.StartsWith("9"))
      {
        return new GoldCard(accountId);
      }
      else
      {
        return new StandardCard(accountId);
      }
    });
```

Вот как создают объекты с помощью такой лямбды:

```c#
var card = container.Resolve<CreditCard>
  (
    new NamedParameter("accountId", "12345")
  );
```
Поддерживаются generic-интерфейсы:

```c#
builder.RegisterGeneric(typeof(NHibernateRepository<>))
  .As(typeof(IRepository<>))
  .InstancePerLifetimeScope();
```

Как их использовать:

```c#
// Autofac will return an NHibernateRepository<Task>
var tasks = container.Resolve<IRepository<Task>>();
```

Для одного интерфейса можно зарегистрировать несколько реализаций, последняя из зарегистрированных становится реализацией по умолчанию:

```c#
builder.RegisterType<ConsoleLogger>().As<ILogger>();
builder.RegisterType<FileLogger>().As<ILogger>();
```

Чтобы такого не происходило, достаточно сказать `PreserveExistingDefaults`:

```c#
builder.RegisterType<ConsoleLogger>().As<ILogger>();
builder.RegisterType<FileLogger>().As<ILogger>().PreserveExistingDefaults();
```

Переходим к нетривиальному и вкусному. Вот так легко и просто можно зарегистрировать все типы в сборке:

```c#
builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
  .Where(t => t.Name.EndsWith("Impl"))
  .AsImplementedInterfaces();
```

Можно выбрать только публичные типы:

```c#
builder.RegisterAssemblyTypes(asm)
  .PublicOnly();
```

или отфильтровать типы, которые не должны попасть в контейнер:

```c#
builder.RegisterAssemblyTypes(asm)
  .Except<MyUnwantedType>();
```

Autofac предлагает абстракцию программного модуля, содержащего несколько типов, которые регистрируются все сразу:

```c#
public class AModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.Register(c => new AComponent()).As<AComponent>();
  }
}
 
public class BModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.Register(c => new BComponent()).As<BComponent>();
  }
}
```

Модули регистрируются буквально в одну строку:

```c#
builder.RegisterAssemblyModules(assembly);
```

Autofac поддерживает конфигурирование с помощью `Microsoft.Extensions.Configuration`, для этого достаточно подключить NuGet-пакет `Autofac.Configuration`. Вот как выглядит конфигурационный JSON-файл:

```json
{
  "defaultAssembly": "Autofac.Example.Calculator",
  "components": [{
    "type": "Autofac.Example.Calculator.Addition.Add, Autofac.Example.Calculator.Addition",
    "services": [{
      "type": "Autofac.Example.Calculator.Api.IOperation"
    }],
    "injectProperties": true
  }, {
    "type": "Autofac.Example.Calculator.Division.Divide, Autofac.Example.Calculator.Division",
    "services": [{
      "type": "Autofac.Example.Calculator.Api.IOperation"
    }],
    "parameters": {
      "places": 4
    }
  }]
}
```

а так XML-файл:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<autofac defaultAssembly="Autofac.Example.Calculator">
    <components name="0">
        <type>Autofac.Example.Calculator.Addition.Add, Autofac.Example.Calculator.Addition</type>
        <services name="0" type="Autofac.Example.Calculator.Api.IOperation" />
        <injectProperties>true</injectProperties>
    </components>
    <components name="1">
        <type>Autofac.Example.Calculator.Division.Divide, Autofac.Example.Calculator.Division</type>
        <services name="0" type="Autofac.Example.Calculator.Api.IOperation" />
        <injectProperties>true</injectProperties>
        <parameters>
            <places>4</places>
        </parameters>
    </components>
</autofac>
```

Подключается эта конфигурация так:

```c#
var config = new ConfigurationBuilder();
config.AddJsonFile("autofac.json");
// или AddXmlFile("autofac.xml");

var module = new ConfigurationModule(config.Build());
var builder = new ContainerBuilder();
builder.RegisterModule(module);
```

Тут есть одна засада: со времен .NET Framework девелоперы приучены к тому, что рантайм ищет и загружает сборки из текущей папки (рядом с EXE-файлом, проще говоря), но в .NET Core это поведение поломали. Вот что об этом пишут авторы Autofac:

> THIS IS THE MAGIC!
.NET Core assembly loading is confusing. Things that happen to be in your bin folder don’t just suddenly
qualify with the assembly loader. If the assembly isn’t specifically referenced by your app, you need to
tell .NET Core where to get it EVEN IF IT’S IN YOUR BIN FOLDER.
https://stackoverflow.com/questions/43918837/net-core-1-1-type-gettype-from-external-assembly-returns-null
>
> The documentation says that any .dll in the application base folder should work, but that doesn’t seem
to be entirely true. You always have to set up additional handlers if you AREN’T referencing the plugin assembly.
https://github.com/dotnet/core-setup/blob/master/Documentation/design-docs/corehost.md

Поэтому необходимо добавить в код следующий резольвер:

```c#
AssemblyLoadContext.Default.Resolving += (context, asm) =>
{
var path = Path.Combine(AppContext.BaseDirectory, asm.Name + ".dll");

return context.LoadFromAssemblyPath(path);
};
```

#### Autofac с Microsoft.Extensions.DependencyInjection

Как использовать Autofac с MEDI? Очень просто: подключить пакет [Autofac.Extensions.DependencyInjection](https://www.nuget.org/packages/Autofac.Extensions.DependencyInjection/) и дописать пару магических строк. Вот пример:

```c#
using System;
using System.Threading;
using System.Threading.Tasks;
 
using Autofac;
using Autofac.Extensions.DependencyInjection;
 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
 
interface IUseful
{
    void DoSomething();
}
 
class UsefulThing : IUseful
{
    private readonly ILogger _logger;
 
    public UsefulThing(ILogger<UsefulThing> logger)
    {
        _logger = logger;
    }
 
    public void DoSomething()
    {
        _logger.LogInformation("Something useful");
    }
}
 
class MyService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IUseful _useful;
 
    public MyService
        (
            ILogger<MyService> logger, 
            IUseful useful
        )   
    {
        _logger = logger;
        _useful = useful;
    }
 
    protected override async Task ExecuteAsync
        (
            CancellationToken stoppingToken
        )
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _useful.DoSomething();
            _logger.LogInformation("Wasting time");
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}
 
class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer((ContainerBuilder builder) =>
            {
                builder.RegisterType<UsefulThing>().As<IUseful>();
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<MyService>();
            })
            .Build();
 
        using (host)
        {
            host.RunAsync();
            Console.ReadLine();
            host.StopAsync();
        }
    }
}
```

Магия здесь заключена в строках

```c#
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer((ContainerBuilder builder) =>
{
    builder.RegisterType<UsefulThing>().As<IUseful>();
})
```

Впрочем, можно регистрировать сервисы и "по старинке", так тоже работает:

```c#
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureServices(services =>
{
    services.AddTransient<IUseful, UsefulThing>();
    services.AddHostedService<MyService>();
})
```
