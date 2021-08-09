### Неочевидные места C#

По видео Дмитрия Дорогого ["Сложные задачи на языке C#"](https://youtu.be/imONgstP1Lg) (18.09.2018).

#### Захват переменной цикла

```c#
var actions = new List<Action>();
for (var i = 0; i < 3; i++)
{
    actions.Add (() => Console.Write($"{i};"));
}

foreach (var action in actions)
{
    action();
}
```

выведет

```
3;3;3;
```

т. к. происходит захват переменной `i` по <u>адресу</u>. Вот реальный код, сгенерированный компилятором:


```c#
[CompilerGenerated]
private sealed class <>c__DisplayClass0_0
{
    public int i;

    internal void <Main>b__0()
    {
        Console.Write(string.Format("{0};", i));
    }
}

private static void Main()
{
    List<Action> list = new List<Action>();
    <>c__DisplayClass0_0 <>c__DisplayClass0_ = new <>c__DisplayClass0_0();
    <>c__DisplayClass0_.i = 0;
    while (<>c__DisplayClass0_.i < 3)
    {
        list.Add(new Action(<>c__DisplayClass0_.<Main>b__0));
        <>c__DisplayClass0_.i++;
    }
    
    List<Action>.Enumerator enumerator = list.GetEnumerator();
    try
    {
        while (enumerator.MoveNext())
        {
            Action current = enumerator.Current;
            current();
        }
    }
    finally
    {
        ((IDisposable)enumerator).Dispose();
    }
}
```

К моменту выполнения `current` переменная (точнее, поле служебного класса) уже имеет значение `3`.  Исправить это довольно легко:

```c#
var actions = new List<Action>();
for (var i = 0; i < 3; i++)
{
    var temp = i;
    actions.Add (() => Console.Write($"{temp};"));
}

foreach (var action in actions)
{
    action();
}
```

Здесь происходит захват разных копий переменной `temp`, что эквивалентно захвату по значению. Вот код, сгенерированный компилятором:

```c#
[CompilerGenerated]
private sealed class <>c__DisplayClass0_0
{
    public int temp;

    internal void <Main>b__0()
    {
        Console.Write(string.Format("{0};", temp));
    }
}

private static void Main()
{
    List<Action> list = new List<Action>();
    int num = 0;
    while (num < 3)
    {
        <>c__DisplayClass0_0 <>c__DisplayClass0_ = new <>c__DisplayClass0_0();
        <>c__DisplayClass0_.temp = num;
        list.Add(new Action(<>c__DisplayClass0_.<Main>b__0));
        num++;
    }
    List<Action>.Enumerator enumerator = list.GetEnumerator();
    try
    {
        while (enumerator.MoveNext())
        {
            Action current = enumerator.Current;
            current();
        }
    }
    finally
    {
        ((IDisposable)enumerator).Dispose();
    }
}
```

#### Ленивое (отложенное) выполнение кода

LINQ-запросы не исполняются, пока значение не будет затребовано. Поэтому такой код

```c#
var list = new List<string> { "A", "B", "C" };
var filtered = list.Where (i => i.StartsWith("B"));
list.Remove("B");
Console.WriteLine(filtered.Count());
```

выведет `0`, ведь в момент реального исполнения `Where` строка `"B"` будет удалена из списка. Чтобы исправить код, достаточно "материлизовать" запрос, вызвав, например, `ToArray()`:

```c#
var list = new List<string> { "A", "B", "C" };
var filtered = list.Where (i => i.StartsWith("B")).ToArray();
list.Remove("B");
Console.WriteLine(filtered.Count());
```

Ленивое выполнение также срабатывает и в данном случае:

```c#
void Main()
{
    var data = GetData().Take(2).Select(i => i * 1);
    Console.WriteLine(data.FirstOrDefault());
}

public static IEnumerable<int> GetData()
{
    yield return 1;
    throw new Exception();
    yield return 2;
}
```

Исключение не выбрасывается, т. к. дело до него просто не доходит - из `GetData` берется первое число, и на этом все заканчивается.

#### Перечислитель-структура

Перечислитель у `List<int>` сделан [структурой](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs):

```c#
public struct Enumerator : IEnumerator<T>, IEnumerator
{
    private readonly List<T> _list;
    private int _index;
    private readonly int _version;
    private T? _current;

    internal Enumerator(List<T> list)
    {
        _list = list;
        _index = 0;
        _version = list._version;
        _current = default;
    }
    
    ...
}
```

Поэтому мы получаем все прелести передачи структуры по значению, например, следующий код зацикливается (и печатает нули):

```c#
var i = new 
{
    Items = new List<int> { 1, 2, 3 }.GetEnumerator()
};

while (i.Items.MoveNext())
{
    Console.WriteLine(i.Items.Current);
}
```

Исправить это довольно легко:

```c#
var i = new 
{
    Items = new List<int> { 1, 2, 3 }.GetEnumerator()
};

var enumerator = i.Items;
while (enumerator.MoveNext())
{
    Console.WriteLine(enumerator.Current);
}
```

#### finally не побеждает

Такой код

```c#
int Test()
{
    try
    {
        return 1;
    }
    finally
    {
        return 2;
    }
}

Console.WriteLine(Test());
```

просто не скомпилируется, т. к.  в C# запрещено покидать блок `finally` с помощью операторов `return`, `break`, `continue` и `goto`. В Java это был бы валидный код, `return 2` в `finally` победил бы.

#### Код после yield

Следующий код

```c#
IEnumerable<string> Foo()
{
    yield return "1";
    yield return "2";
    Console.WriteLine ("3");
}

foreach (var s in Foo())
{
    Console.WriteLine (s);
}
```

напечатает строки "1", "2" и "3", т. е. код после `yield return "2"` прекрасно выполняется. Чтобы он не выполнялся, нужно добавить строку `yield break` (компилятор выдаст предупреждение о недостижимом коде).

#### Инициализация enum

Грамматика C# такова, что проинициализировать переменную типа `enum` можно с помощью нуля с плавающей точкой. Это вполне допустимый код.

```c#
void Main()
{
    MyEnum f0 = 0.0f;
    Console.WriteLine(f0);
}

enum MyEnum { Hello, Hola };
```

Будет напечатано "Hello". Проинициализировать значением с плавающеё точкой, отличным от нуля, например, `0.5f`, нельзя - компилятор ругается.

#### Парсинг enum

Парсинг enum устроен довольно неожиданно:

```c#
void Main()
{
    MyEnum result;
    if (Enum.TryParse ("V1, V2, V3", out result))
    {
        Console.WriteLine (result);
    }
    else
    {
        Console.WriteLine ("Error");
    }

}

public enum MyEnum { V1, V2, V3, V4, V5, V6, V7 }
```

напечатает "V4". Что тут происходит:

1. Несмотря на то, что у `MyEnum` нет атрибута `Flags`, парсинг всегда происходит так, будто этот атрибут есть.
2. Запятая трактуется как признак перечисления флагов. Другие символы (например, точка с запятой или плюс) приводят к выдаче ошибки.
3. `V1 = 0`, `V2 = 1`, `V3 = 2` в сумме выдают `3`, что соответствует `V4`. Если удалить из `MyEnum` все члены, начиная с `V4`, будет просто напечатано `3`.


#### Конкатенация строк

Конкатенация строк в CLR прописана довольно [нетривиально](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/String.Manipulation.cs):

```c#
public static string Concat(string? str0, string? str1)
{
    if (IsNullOrEmpty(str0))
    {
        if (IsNullOrEmpty(str1))
        {
            return string.Empty;
        }
        return str1;
    }

    if (IsNullOrEmpty(str1))
    {
        return str0;
    }

    int str0Length = str0.Length;

    string result = FastAllocateString(str0Length + str1.Length);

    FillStringChecked(result, 0, str0);
    FillStringChecked(result, str0Length, str1);

    return result;
}
```

Поэтому такой код

```c#
var s = (string)null + null;
Console.WriteLine(s is null);
```

напечатает `False`.

Вообще, компилятор C# настолько разбирается в строках, что заменяет оператор `+` на вызов `String.Concat`, а если учесть наличие методов вроде

```c#
public static string Concat(object? arg0) =>
    arg0?.ToString() ?? Empty;

public static string Concat(object? arg0, object? arg1) =>
    Concat(arg0?.ToString(), arg1?.ToString());

public static string Concat(object? arg0, object? arg1, object? arg2) =>
    Concat(arg0?.ToString(), arg1?.ToString(), arg2?.ToString());

public static string Concat(params object?[] args)
{
    ...
}
```

то данный код

```c#
Console.WriteLine(1 + 2 + "A");
Console.WriteLine("A" + 1 + 2);
```

напечатает

```
3A
A12
```

Чтобы два раза не вставать: `char` без проблем молча конвертируется в `int`, так что этот код

```c#
Console.WriteLine(1 + 2 + 'A');
Console.WriteLine('A' + 1 + 2);
```

дважды напечатает "68".

#### Битовый сдвиг

Следующий код

```c#
int i = 1 << 1;
int j = 1 << 33;
Console.WriteLine($"{i}, {j}");
```

неожиданно напечатает "2, 2". Так происходит потому, что по факту компилятор C# использует лишь последние 5 бит от величины сдвига:

```c#
int i = 1 << 1;
int j = 1 << (33 & 0b11111);
Console.WriteLine($"{i}, {j}");
```
#### new IInterface

COM, как известно, есть шкатулка с сюрпризами (не всегда приятными). Часть из этих сюрпризов сказывается и на C#:

```c#
[ComImport, CoClass(typeof(Foo))]
[Guid("80C641A2-EEBA-47B4-A463-AF7BDD122694")]
interface IInterface
{
    void Message();
}

void Main()
{
    var foo = new IInterface();
    foo.Message();
}

class Foo: IInterface
{
    void IInterface.Message()
    {
    }
}
```

#### Множество значений True

Эта проблема присутствует не только в C#. В C/C++ логическая истина также может кодироваться несколькими способами, поэтому сравнивать логические значения друг с другом нужно с осторожностью (особенно, если они приходят из внешнего кода, например, из Interop).

```c#
void Main()
{
    bool bool1 = BoolStore.Bool1;
    bool bool2 = BoolStore.Bool2;
    
    Console.WriteLine (bool1);
    Console.WriteLine (bool2);
    Console.WriteLine (bool1 == bool2);
}
```

напечатает

```
True
True
False
```


```c#
[StructLayout (LayoutKind.Explicit)]
struct BoolStore
{
    [FieldOffset (0)]
    int IntegerValue;
    
    [FieldOffset (0)]
    bool BooleanValue;
    
    public bool GetBool (int i)
    {
        IntegerValue = i;
        return BooleanValue;
    }
    
    public static bool Bool1 => new BoolStore().GetBool(1);
    public static bool Bool2 => new BoolStore().GetBool(2);
}
```

