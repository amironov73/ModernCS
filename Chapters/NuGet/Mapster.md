### Библиотека MapsterMapper

Библиотека для простого перекладывания данных из одного POCO-класса в другой. GitHub: https://github.com/MapsterMapper/Mapster, NuGet: https://www.nuget.org/packages/Mapster/. Поддерживается .NET Framework (начиная с 4.0) и .NET Standard 1.3.

```csharp
using System;
 
using Mapster;
 
class SourceClass
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Title { get; set; }
    public int Age { get; set; }
    public int Price { get; set; }
}
 
class DestinationClass
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Place { get; set; }
}
 
class Program
{
    static void Main()
    {
        SourceClass source = new SourceClass
        {
            Name = "Alexey",
            Address = "Irkutsk",
            Title = "Librarian",
            Age = 45,
            Price = 0
        };
 
        DestinationClass dest = source.Adapt<DestinationClass>();
        Console.WriteLine($"{dest.Name} {dest.Age} {dest.Place}");
    }
}
```

Можно спроецировать на уже существующий объект:

```csharp
TDestination destObject = new TDestination();
destObject = sourceObject.Adapt(destObject);
```

Кроме «поштучного» преобразования, доступно также «пакетное»:

```csharp
using (MyDbContext context = new MyDbContext())
{
    // Build a Select Expression from DTO
    var dest = context.Sources.ProjectToType<Destination>().ToList();
 
    // Versus creating by hand:
    var destinations = context.Sources.Select(c => new Destination(){
        Id = p.Id,
        Name = p.Name,
        Surname = p.Surname,
        ....
    })
    .ToList();
}
```

Производительность вполне на уровне, об этом можно не беспокоиться.