### Пакет OneOf

Пакет OneOf – поразительно полезный инструмент, привносящий в программирование на C# привкус F#. 🙂

GitHub: https://github.com/mcintyre321/OneOf/, NuGet: https://www.nuget.org/packages/OneOf/, поддерживается классический .NET Framework 4.5 и .NET Standard 1.3.

OneOf позволяет легко создавать нечто напоминающее размеченные объединения из F#. Каждый экземпляр такого типа содержит ровно одно значение одного из заданных при конструировании типов, причём экземпляр знает, значение какого типа он содержит.

Кроме обычных типов, в пакете предусмотрен набор «типовых» типов (в пространстве имен `OneOf.Types`): `Yes`, `No`, `Maybe`, `Unknown`, `True`, `False`, `All`, `Some`, `None`, `Error`, `Error<T>`, `NotFound`, `Result<T>`, `Success`, `Success<T>`. Кроме того, предусмотрены наиболее востребованные сочетания вроде `TrueFalseOrNull`, `YesNoOrMaybe`.

Простейший пример:

```c#
using OneOf;
using OneOf.Types;
 
using static System.Console;
 
class Program
{
    static OneOf<string, Error> GetSomething(int index)
        => index switch
        {
            1 => "One",
            _ => new Error()
        };
 
    static void Main()
    {
        var r = GetSomething(1);
 
        r.Switch
            (
                text => WriteLine(text),
                error => WriteLine("Something wrong!")
            );
    }
}
```

Вывод программы:

```
1 => System.String: One
2 => System.Int32: 2
```

Пример вычисления значения по элементам OneOf:

```c#
var r = GetSomething(1);
 
var t = r.Match
   (
       title => $"Title is: {title}",
       error => "No title"
   );
WriteLine(t);
```

К сожалению, написать что-нибудь вроде

```c#
class HttpContent : OneOf<string, Error> {}
```

не выйдет, класс объявлен sealed. Надо писать так:

```c#
class HttpContent : OneOfBase<string, Error> {}
```