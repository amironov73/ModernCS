### Сопоставление с образцом

#### Сопоставление с образцом в is-выражениях
```csharp
public static void IsExpressions(object o)
{
    // Альтернативный способ проверки на null
    if (o is null) Console.WriteLine("o is null");
 
    // Числовая константа
    const double value = 2.0;
    if (o is value) Console.WriteLine("o is value");
 
    // Строковый литерал
    if (o is "o") Console.WriteLine("o is \"o\"");
 
    // Проверка совпадение с типом
    if (o is int n) Console.WriteLine(n);
 
    // Совпадение с типом и составное выражение
    if (o is string s && s.Trim() != string.Empty)
        Console.WriteLine("o is not blank");
}
```

#### Сопоставление с образцом блоках switch
```csharp
public static int Count<T>(IEnumerable<T> e)
{
    switch (e)
    {
        case T[] a:
            return a.Length;
 
        case ICollection<T> c:
            return c.Count;
 
        case IReadOnlyCollection<T> c:
            return c.Count;
 
        case IProducerConsumerCollection<T> pc:
            return pc.Count;
 
        case IEnumerable<T> _:
            return e.Count();
 
        default:
            return 0;
    }
}
```
Все case-блоки содержат неявную проверку на `null`. В предыдущем примере последний `case`-блок правилен, поскольку он будет срабатывать только тогда, когда аргумент не равен `null`. Имя `_` является специальным и сообщает компилятору, что переменная не нужна. Шаблон типа в предложении `case` требует имени переменной, и если вы не собираетесь ее использовать, то вы можете ее проигнорировать с помощью `_`.

С использованием предикатов:

?
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
public static void FizzBuzz(object o)
{
    switch (o)
    {
        case string s when s.Contains("Fizz") || s.Contains("Buzz"):
            Console.WriteLine(s);
            break;
 
        case int n when n % 5 == 0 && n % 3 == 0:
            Console.WriteLine("FizzBuzz");
            break;
 
        case int n when n % 5 == 0:
            Console.WriteLine("Fizz");
            break;
 
        case int n when n % 3 == 0:
            Console.WriteLine("Buzz");
            break;
 
        case int n:
            Console.WriteLine(n);
            break;
    }
}
