### Библиотека AutoMapper

Как известно, довольно большую часть программы составляет перекладывание данных из одних структур в другие, конверсия из одного типа в другой. Это несложно, но довольно утомительно и требует от программиста внимания, которое он мог бы потратить на что-нибудь более полезное. Некоторую часть этих забот может принять на себя пакет AutoMapper, назначение которого — автоматизация задач по перекладыванию данных туда-сюда.

Сайт: https://automapper.org, GitHub: https://github.com/AutoMapper/AutoMapper, NuGet: https://www.nuget.org/packages/AutoMapper/.

Поддерживается классический .NET 4.6.1 и .NET Standard 2.0.

Пример:

```c#
using System;
using AutoMapper;
 
class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
 
class Reader
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}
 
class Program
{
    static void Main()
    {
        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, Reader>();
        })
           .CreateMapper();
 
        var user = new User
        {
            Id = 123,
            Name = "Alexey",
            Email = "amironov@fmail.com"
        };
 
        var reader = mapper.Map<Reader>(user);
 
        Console.WriteLine($"{reader.Id}: {reader.Name}");
    }
}
```

Обратите внимание, AutoMapper сам сконвертировал свойство `User.Id` из `int` в `string` для свойства `Reader.Id`.

Конечно же, AutoMapper легко справляется и с перекладыванием массивов или списков:

```c#
var users = new[]
{
    new User
    {
        Id = 123,
        Name = "Alexey",
        Email = "amironov@fmail.com"
    }
};
 
var readers = mapper.Map <List<Reader>>(users);
```

Авторы рекомендуют собирать маппинги в так называемые профили, чтобы всё, относящееся к маппингу, было в одном месте:

```c#
class LibraryProfile : Profile
{
    public LibraryProfile()
    {
        CreateMap<User, Reader>();
    }
}
 
// где-то в Startup.cs
 
var mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<LibraryProfile>();
    })
    .CreateMapper();
 
// лучше так:
 
var mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(Assembly.GetExecutingAssembly());
    })
    .CreateMapper();
 
// или даже так:
 
var mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps("Foo.Core", "Foo.UI");
    })
    .CreateMapper();
```

Для нужд ASP.NET Core предусмотрен пакет [AutoMapper.Extensions.Microsoft.DependencyInjection](https://www.nuget.org/packages/AutoMapper.Extensions.Microsoft.DependencyInjection/), позволяющий свести конфигурирование к волшебной строчке в `Startup.cs`:

после чего AutoMapper становится для «впрыскивания» в конструкторы:

```c#
public class EmployeesController {
    private readonly IMapper _mapper;
 
    public EmployeesController(IMapper mapper) => _mapper = mapper;
 
    // use _mapper.Map or _mapper.ProjectTo
}
```
