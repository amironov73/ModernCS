### Библиотека RecordParser

Довольно простая в использовании, при этом изощренная изнутри [библиотека RecordParser](https://github.com/leandromoh/recordparser), предназначенная для разбора (или наоборот — форматирования) текстовых записей. Поддерживает .NET Core 2.1, 3.1, 5.0 и .NET Standard 2.1. 

Среди достоинств библиотеки - то, что она создает минимальный трафик памяти, т. к. по возможности широко использует тип `Span<T>`. Кроме того, она использует генерацию кода с помощью `System.Linq.Expressions`, так что результирующий код получается довольно шустрый. Поддерживается парсинг и форматирование структур без боксинга (вот это замечательно!).


Вот пример разбора текста с фиксированной шириной колонок:

```c#
using System;
using RecordParser.Builders.Reader;

var reader = new FixedLengthReaderBuilder<(string Name, DateTime Birthday, decimal Money)>()
.Map(x => x.Name, startIndex: 0, length: 11)
.Map(x => x.Birthday, 12, 10)
.Map(x => x.Money, 23, 7)
.Build();

var result = reader.Parse("foo bar baz 2020/05/23 0123,45");
Console.WriteLine($"{result.Name}, {result.Birthday}, {result.Money}");
```

Пример разбора текста с переменной шириной колонок (т. е. с явно заданным разделителем колонок):

```c#
using System;
using RecordParser.Builders.Reader;

var reader = new VariableLengthReaderBuilder<(string Name, DateTime Birthday, decimal Money)>()
.Map(x => x.Name, indexColumn: 0)
.Map(x => x.Birthday, 1)
.Map(x => x.Money, 2)
.Build(";");

var result = reader.Parse("foo bar baz ; 2020/05/23 ; 0123,45");
Console.WriteLine($"{result.Name}, {result.Birthday}, {result.Money}");
```

Можно задать правила преобразования для определенных типов:

```c#
var reader = new VariableLengthReaderBuilder<...>()
.Map(x => x.Name, indexColumn: 0)
.Map(x => x.Birthday, 1)
.Map(x => x.Money, 2)
.DefaultTypeConvert(value => decimal.Parse(value) / 100)
.DefaultTypeConvert(value => DateTime.ParseExact(value, "ddMMyyyy", null))
.Build(";");
```

и правила преобразования для конкретных колонок:

```c#
var reader = new VariableLengthReaderBuilder<...>()
.Map(x => x.Name, indexColumn: 0)
.Map(x => x.Age, 1, value => int.Parse(value) + 3)
.Map(x => x.Money, 2)
.Build(";");
```

Разумеется, предусмотрено и обратное преобразование в строку. С постоянной шириной колонок:

```c#
using System;
using RecordParser.Builders.Writer;

var writer = new FixedLengthWriterBuilder<(string Name, DateTime Birthday, decimal Money)>()
.Map(x => x.Name, startIndex: 0, length: 12)
.Map(x => x.Birthday, 12, 11, "yyyy.MM.dd", paddingChar: ' ')
.Map(x => x.Money, 23, 8)
.Build();

var instance = (Name: "foo bar baz",
Birthday: new DateTime(2020, 05, 23),
Money: 01234.567M);

Span<char> destination = new char[100];
var success = writer.TryFormat(instance, destination, out var charsWritten);
Console.WriteLine(success);
var result = destination.Slice(0, charsWritten);
Console.WriteLine(result.ToString());
```

С переменной шириной колонок:

```c#
using System;
using RecordParser.Builders.Writer;

var writer = new VariableLengthWriterBuilder<(string Name, DateTime Birthday, decimal Money)>()
.Map(x => x.Name, indexColumn: 0)
.Map(x => x.Birthday, 1, "yyyy.MM.dd")
.Map(x => x.Money, 2)
.Build(" ; ");

var instance = ("foo bar baz", new DateTime(2020, 05, 23), 0123.45M);

Span<char> destination = new char[100];
var success = writer.TryFormat(instance, destination, out var charsWritten);
var result = destination.Slice(0, charsWritten);
Console.WriteLine(result.ToString());
```

Правила форматирования для типов:

```c#
using System;
using RecordParser.Builders.Writer;

var writer = new FixedLengthWriterBuilder<(decimal Balance, DateTime Date, decimal Debit)>()
.Map(x => x.Balance, 0, 12, padding: Padding.Left, paddingChar: '0')
.Map(x => x.Date, 13, 8)
.Map(x => x.Debit, 22, 6, padding: Padding.Left, paddingChar: '0')
.DefaultTypeConvert<decimal>((span, value) =>
(((long)(value * 100)).TryFormat(span, out var written), written))
.DefaultTypeConvert<DateTime>((span, value) =>
(value.TryFormat(span, out var written, "ddMMyyyy"), written))
.Build();

var instance = (Balance: 123456789.01M,
Date: new DateTime(2020, 05, 23),
Debit: 123.45M);

Span<char> destination = new char[50];
var success = writer.TryFormat(instance, destination, out var charsWritten);
Console.WriteLine(success);
var result = destination.Slice(0, charsWritten);
Console.WriteLine(result.ToString());
```

Правила форматирования для конкретных колонок:

```c#
using System;
using RecordParser.Builders.Writer;

var writer = new VariableLengthWriterBuilder<(int Age, int MotherAge, int FatherAge)>()
.Map(x => x.Age, 0)
.Map(x => x.MotherAge, 1,
(span, value) => ((value + 2).TryFormat(span, out var written), written))
.Map(x => x.FatherAge, 2)
.Build(" ; ");

var instance = (Age: 15,
MotherAge: 40,
FatherAge: 50);

Span<char> destination = new char[50];
var success = writer.TryFormat(instance, destination, out var charsWritten);
var result = destination.Slice(0, charsWritten);
Console.WriteLine(result.ToString());
```
