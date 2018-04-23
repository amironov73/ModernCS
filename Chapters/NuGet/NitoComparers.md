### Nito.Comparers

Стивен Клири не устаёт радовать нас полезными библиотеками. Знакомьтесь — Nito.Comparers! Этот пакет умеет автоматически генерировать методы для сравнения/упорядочения классов.

Поддерживает netstandard1.0 (.NET 4.5, .NET Core 1.0, Xamarin.iOS 10, Xamarin.Android 7, Mono 4.6, Universal Windows 10, Windows 8, Windows Phone Applications 8.1, Windows Phone Silverlight 8.0).

GitHub: https://github.com/StephenCleary/Comparers, NuGet: https://www.nuget.org/packages/Nito.Comparers

Простой пример:

```csharp
using System;
using System.Collections.Generic;
using Nito.Comparers;
 
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
 
class Program
{
    static void Main()
    {
        List<Person> persons = new List<Person>
        {
            new Person{Name = "Иван", Age=15},
            new Person{Name = "Катерина", Age = 16},
            new Person{Name = "Василий", Age = 12},
            new Person{Name = "Василий", Age = 15}
        };
 
        IComparer<Person> personComparer = ComparerBuilder.For<Person>()
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Age);
 
        persons.Sort(personComparer);
 
        foreach (Person person in persons)
        {
            Console.WriteLine($"{person.Name} {person.Age}");
        }
    }
}
```
