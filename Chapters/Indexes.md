### Индексы и диапазоны

В C# 8 были добавлены два типа данных: `System.Index` и `System.Range`. 

Вот как определена структура `Index` (для простоты тела методов опущены):

```c#
public readonly struct Index : IEquatable<Index>
{
    // Конструктор
    public Index(int value, bool fromEnd = false) {}
    
    // Простые вспомогательные методы
    public static Index Start => new Index(0);
    public static Index End => new Index(~0);

    public static Index FromStart(int value) {}
    public static Index FromEnd(int value) {}

    public int Value { get; }

    public bool IsFromEnd => _value < 0;
    
    public int GetOffset(int length) {}
    
    public bool Equals(Index other) => _value == other._value;
    public override int GetHashCode() => _value;

    public static implicit operator Index(int value) => FromStart(value);

    public override string ToString() {}
}
```

Для поддержки индексов "от конца массива" в C# 8 был введён специальный синтаксис:

```c#
Index i1 = 2;  // это обычный индекс от начала массива
Index i2 = ^2; // это индекс, отсчитываемый от конца массива
// он означает "предпоследний элемент"
```

Соответственно были доработаны индексаторы у стандартных коллекций и массивов, чтобы они принимали `System.Index` в качестве аргумента:

```c#
string[] fruits = { "яблоко", "груша", "лимон", "апельсин" };
string chosen = fruits[^2]; // "лимон"
```

Сами по себе индексы были бы слишком мелкой инновацией, но тут к ним на помощь спешат диапазоны, и вместе они серьёзно повышают привлекательность C#.

Диапазон определён как следующая структура (для простоты тела методов опущены):

```c#
public readonly struct Range : IEquatable<Range>
{
    // Начало и конец диапазона
    public Index Start { get; }
    public Index End { get; }

    // Конструктор
    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }

    // Простые вспомогательные методы
    public static Range StartAt(Index start) => new Range(start, Index.End);
    public static Range EndAt(Index end) => new Range(Index.Start, end);
    public static Range All => new Range(Index.Start, Index.End);

    public (int Offset, int Length) GetOffsetAndLength(int length) {}

    public bool Equals(Range other) => other.Start.Equals(Start) 
        && other.End.Equals(End);

    public override int GetHashCode() {}

    public override string ToString() {}
}
```

Для диапазонов введён специальный синтаксис (две точки), серьёзно улучшающий читаемость:

```c#
int start = 1;
int end = 2;
Range r1 = 1..2;
Range r2 = start..end;
```

Диапазон представляет часть последовательности, которая ограничена двумя индексами. Начальный индекс включается в диапазон, а конечный индекс НЕ входит в диапазон.

Синтаксис для задания диапазона довольно гибкий: можно задать:

1. одновременно начало и конец диапазона;
2. только начало диапазона (конец будет определён автоматически);
3. только конец диапазона (начало будет установлено в 0).

```c#
Range r1 = 1..2;  // получается всего один элемент
Range r2 = 1..;   // все элементы, кроме нулевого
Range r3 = ..2;   // первые два элемента
Range r4 = ..^1;  // все элементы, кроме после последнего
```

Соответственно были доработаны индексаторы у стандартных коллекций и массивов, чтобы они принимали `System.Range` в качестве аргумента:

```c#
string[] fruits = { "яблоко", "груша", "лимон", "апельсин" };
Range range = 1..^2;
foreach (var fruit in fruits[range])
    Console.WriteLine(fruit);
// будет напечатано "груша"
```
