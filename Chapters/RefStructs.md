### Типы-значения с семантикой ссылочных типов

В C# 7.2 были значительно доработаны типы-значения (структуры).

Во-первых, при объявлении метода теперь можно использовать для параметров модификатор `in`, который указывает, что структура будет передана по ссылке, при этом изменять её значение запрещено. Был добавлен также парный модификатор `ref readonly`, который указывает, что значение будет возвращено по ссылке, однако вызвающей стороне запрещено менять его. Операция присвоения создаст копию, на которую этот запрет не распространяется.

```c#
public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    private static Point _zero = new Point { X = 0; Y = 0; };
    public static ref readonly Zero => ref _zero;
}
```

Во-вторых были введены модификаторы для структур `readonly` и `ref`.

#### readonly struct

Модификатор `readonly` при определении структуры создаёт т. наз. неизменяемую структуру, все свойства и поля которой доступны только для чтения. Компилятор выдаст ошибку при обнаружении изменяемых полей или свойств.

```c#
public readonly struct Vector
{
    public int X { get; }
    public int Y { get; }
 
    public Vector(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}
```

#### ref struct

Модификатор `ref` при определении структуры указывает, что структура может быть создана только в стеке и запрещает её создание в куче. Из этого следует, что она:

* не может быть подвержена боксингу;
* не может использоваться в качестве членов обычной структуры или класса;
* более того, такие структуры нельзя объединять в массивы (массивы хранятся в куче);
* не может объявляться в асинхронных методах (но может быть в методах возвращающих Task, Task<T> и обобщенные типы;
* не может применяться в итераторах;
* не может быть захвачена в лямбда-выражениях или локальных методах.

```c#
public ref struct Vector
{
    public int X, Y;

}
```

Можно сочетать `readonly` и `ref`:

```c#
public readonly ref struct Vector
{
    public int X { get; }
    public int Y { get; }

    public Vector(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

#### Как это работает

Вся магия таких структур работает на уровне компилятора C#, который добавляет к определению структуры атрибуты `[Obsolete]` и `[IsByRefLike]`. Сборки остаются совместимыми со старыми компиляторами, однако при попытке задействовать типы, помеченные этими атрибутами, старые компиляторы выдают ошибки.

```
.class public sequential ansi sealed beforefieldinit Demo.MyRefStruct
extends [System.Runtime]System.ValueType
{
.custom instance void [System.Runtime]System.Runtime.CompilerServices.IsByRefLikeAttribute::.ctor() = (
        01 00 00 00
    )
.custom instance void [System.Runtime]System.ObsoleteAttribute::.ctor(string, bool) = (
        01 00 52 54 79 70 65 73 20 77 69 74 68 20 65 6d
        62 65 64 64 65 64 20 72 65 66 65 72 65 6e 63 65
        73 20 61 72 65 20 6e 6f 74 20 73 75 70 70 6f 72
        74 65 64 20 69 6e 20 74 68 69 73 20 76 65 72 73
        69 6f 6e 20 6f 66 20 79 6f 75 72 20 63 6f 6d 70
        69 6c 65 72 2e 01 00 00
    )
.custom instance void [System.Runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = (
        01 00 00 00
    )
// Fields
.field public initonly int32 MyIntValue1
.field public initonly int32 MyIntValue2
// Methods
.method public hidebysig specialname rtspecialname
instance void .ctor (
int32 value1,
int32 value2
        ) cil managed
    {
// Method begins at RVA 0x2090
// Code size 16 (0x10)
.maxstack 8
// (no C# code)
        IL_0000: nop
// this.MyIntValue1 = value1;
        IL_0001: ldarg.0
        IL_0002: ldarg.1
        IL_0003: stfld int32 Demo.MyRefStruct::MyIntValue1
// this.MyIntValue2 = value2;
        IL_0008: ldarg.0
        IL_0009: ldarg.2
        IL_000a: stfld int32 Demo.MyRefStruct::MyIntValue2
// (no C# code)
        IL_000f: ret
    } // end of method MyRefStruct::.ctor
.method public hidebysig virtual
instance bool Equals (
object obj
        ) cil managed
    {
// Method begins at RVA 0x20a1
// Code size 6 (0x6)
.maxstack 8
// throw new NotSupportedException();
        IL_0000: newobj instance void [System.Runtime]System.NotSupportedException::.ctor()
// (no C# code)
        IL_0005: throw
    } // end of method MyRefStruct::Equals
.method public hidebysig virtual
instance int32 GetHashCode () cil managed
    {
.custom instance void [System.Runtime]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System.Runtime]System.ComponentModel.EditorBrowsableState) = (
            01 00 01 00 00 00 00 00
        )
// Method begins at RVA 0x20a8
// Code size 6 (0x6)
.maxstack 8
// throw new NotSupportedException();
        IL_0000: newobj instance void [System.Runtime]System.NotSupportedException::.ctor()
// (no C# code)
        IL_0005: throw
    } // end of method MyRefStruct::GetHashCode
.method public hidebysig virtual
instance string ToString () cil managed
    {
.custom instance void [System.Runtime]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System.Runtime]System.ComponentModel.EditorBrowsableState) = (
            01 00 01 00 00 00 00 00
        )
// Method begins at RVA 0x20af
// Code size 6 (0x6)
.maxstack 8
// throw new NotSupportedException();
        IL_0000: newobj instance void [System.Runtime]System.NotSupportedException::.ctor()
// (no C# code)
        IL_0005: throw
    } // end of method MyRefStruct::ToString
} // end of class Demo.MyRefStruct
```

#### Span<T>

Структура `Span` является примером ref struct. Она определена так (довольно много кода опущено):

```c#
public readonly ref struct Span<T>
{
    // A byref or a native ptr.
    internal readonly ByReference<T> _pointer;
    // The number of elements this Span contains.
    private readonly int _length;

    public int Length { get => _length; }
    public bool IsEmpty { get => 0 >= (uint)_length; }

        public Span(T[]? array)
        {
            if (array == null)
            {
                this = default;
                return;
            }

            _pointer = new ByReference<T>(ref Unsafe.As<byte, T>(ref array.GetRawSzArrayData()));
            _length = array.Length;
        }

    public ref T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_length)
                ThrowHelper.ThrowIndexOutOfRangeException();
            return ref Unsafe.Add(ref _pointer.Value, index);
        }
    }

    // This method is not supported as spans cannot be boxed.
    [Obsolete("Equals() on Span will always throw an exception. Use == instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) =>
        throw new NotSupportedException(SR.NotSupported_CannotCallEqualsOnSpan);

    // This method is not supported as spans cannot be boxed.
    [Obsolete("GetHashCode() on Span will always throw an exception.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() =>
        throw new NotSupportedException(SR.NotSupported_CannotCallGetHashCodeOnSpan);

    public static implicit operator Span<T>(T[]? array) => new Span<T>(array);

    public static implicit operator Span<T>(ArraySegment<T> segment) =>
        new Span<T>(segment.Array, segment.Offset, segment.Count);

    public static Span<T> Empty => default;
}
```
