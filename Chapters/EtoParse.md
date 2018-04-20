### Eto.Parse

Eto.Parse — библиотека для разбора текста с довольно удобным синтаксисом и высоким быстродействием.

Для сравнения: разбор довольно большого JSON-файла 1000 раз разными библиотеками:

**Скорость**

Test              | Parsing | Slower than best |  Warmup | Slower than best
----------------- | ------: | :--------------: | ------: | :--------------:
Eto.Parse         |  2,327s |     1,00x        |  0,008s |     1,00x
Newtonsoft Json   |  2,523s |     1,08x        |  0,068s |     8,08x
ServiceStack.Text |  2,854s |     1,23x        |  0,066s |     7,78x
Irony             | 25,401s |    10,92x        |  0,188s |    22,28x
bsn.GoldParser    | 11,186s |     4,81x        |  0,013s |     1,49x
NFX.JSON          | 11,847s |     5,09x        |  0,187s |    22,10x
SpracheJSON       | 92,774s |    39,88x        |  0,189s |    22,37x

**Расход кучи**

Framework        |  Allocated  | More than best | # Objects | More than best
---------------- | ----------: | :------------: | --------: | :------------:
Eto.Parse        |   553.99 MB |      1.00x     |  15268050 |    1.00x
Newtonsoft.Json  | 1,074.27 MB |      1.94x     |  21562432 |    1.41x
ServiceStack.Text| 2,540.91 MB |      4.59x     |  15738493 |    1.03x
Irony            | 4,351.44 MB |      7.85x     |  94831118 |    6.21x
bsn.GoldParser   | 2,012.16 MB |      3.63x     |  74387176 |    4.87x

GitHub: https://github.com/picoe/Eto.Parse, NuGet: https://www.nuget.org/packages/Eto.Parse/

Простейшая программа:

```csharp
using System;

using Eto.Parse;

class Program
{
    static void Main()
    {
        var ws = Terminals.WhiteSpace;
        var letter = Terminals.LetterOrDigit;

        var nameParser = ws.Repeat(0)
            & letter.Repeat().Named("first")
            & ws.Repeat()
            & letter.Repeat().Named("second")
            & ws.Repeat(0);

        var grammar = new Grammar(nameParser);
        var input = " Alexey Mironov ";
        var match = grammar.Match(input);

        var first = match["first"].Value;
        var second = match["second"].Value;
        Console.WriteLine($"{first} {second}");
    }
}

```

Более сложный пример из документации

```csharp
// optional repeating whitespace
var ws = Terminals.WhiteSpace.Repeat(0);

// parse a value with or without brackets
var valueParser = Terminals.Set('(')
    .Then(Terminals.AnyChar.Repeat().Until(ws.Then(')')).Named("value"))
    .Then(Terminals.Set(')'))
    .SeparatedBy(ws)
    .Or(Terminals.WhiteSpace.Inverse().Repeat().Named("value"));

// our grammar
var grammar = new Grammar(
    ws
    .Then(valueParser.Named("first"))
    .Then(valueParser.Named("second"))
    .Then(Terminals.End)
    .SeparatedBy(ws)
);
```

или более читабельная версия

```csharp
// optional repeating whitespace
var ws = -Terminals.WhiteSpace;

// parse a value with or without brackets
Parser valueParser = 
    ('(' & ws & (+Terminals.AnyChar ^ (ws & ')')).Named("value") & ws & ')')
    | (+!Terminals.WhiteSpace).Named("value");

// our grammar
var grammar = new Grammar(
    ws & valueParser.Named("first") & 
    ws & valueParser.Named("second") & 
    ws & Terminals.End
);
```

