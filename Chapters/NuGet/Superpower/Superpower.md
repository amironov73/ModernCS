Где взять суперсилу? Дотнетчики отлично знают ответ на этот вопрос: конечно же, скачать с NuGet! 🙂

[Superpower](https://github.com/datalust/superpower) — библиотека-наследница Sprache, написанная тем же автором. Базовые сценарии у этих библиотек очень похожи, см. нижеследующий пример. Пусть нам нужно разбирать некоторые инвентарные номера, устроенные так: сначала четыре восьмеричных цифры, дефис, затем шесть шестнадцатеричных, слеш и наконец — суффикс, состоящий из единственной десятеричной цифры. Вот как это делается на Superpower:

```c#
using System;

using Superpower;
using Superpower.Parsers;

class Program
{
    // восьмеричные цифры
    static readonly TextParser<char> Oct =
    Character.In ('0', '1', '2', '3', '4', '5', '6', '7')
    .Named ("octal digit");

    // шестнадцатеричные цифры
    static readonly TextParser<char> Hex =
        Character.In ('0', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', 'A', 'B', 'C', 'D', 'E', 'F')
        .Named ("hex digit");
 
    // знак минуса/дефиса (просто для единообразия)
    static readonly TextParser<char> Minus = Character.EqualTo ('-');
     
    // знак слеша
    static readonly TextParser<char> Slash = Character.EqualTo ('/');
 
    // класс для хранения инвентарного номера
    class Inventory
    {
        public string Prefix { get; }
        public string Body { get; }
        public char Suffix { get; }
 
        public Inventory(string prefix, string body, char suffix)
        {
            Prefix = prefix;
            Body = body;
            Suffix = suffix;
        }
    }
 
    // собственно парсер инвентарного номера
    private static readonly TextParser<Inventory> InventoryParser =
        from prefix in Oct.Repeat (4)
        from minus in Minus
        from body in Hex.Repeat (6)
        from slash in Slash
        from suffix in Character.Digit
        select new Inventory 
            (
                new string (prefix), 
                new string (body),
                suffix
            );
 
    static void ParseInventory (string input)
    {
        try
        {
            var inventory = InventoryParser.AtEnd().Parse (input);
            Console.WriteLine ($"{input}: prefix={inventory.Prefix}, "
                + $"body={inventory.Body}, suffix={inventory.Suffix}");
        }
        catch (Exception exception)
        {
            Console.WriteLine ($"{input}: {exception.Message}");
        }
    }
 
    public static void Main ()
    {
        ParseInventory ("0122-48ABC2/3");
        ParseInventory ("0122/48ABC2+3"); // должно выдать ошибку
    }
}
```

Согласитесь, выглядит знакомо, так что сильно переучиваться не придется.
