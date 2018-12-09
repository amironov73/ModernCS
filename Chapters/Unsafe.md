### Небезопасная мощь

Можно объявить `unsafe` весь класс или структуру целиком

```c#
public unsafe struct Node
{
    public int Value;
    public Node* Left;
    public Node* Right;
}
```

Можно объявить `unsafe` весь метод целиком

```c#
unsafe static void FastCopy(byte[] src, byte[] dst, int count)
{
    // Unsafe-контекст: здесь можно получить указатели на src и dst,
    // а затем использовать их
}

// или так

unsafe static void FastCopy(byte* ps, byte* pd, int count) 
{
    // Здесь можно непосредственно использовать ps и pd
}
```

Можно ввести отдельный unsafe-блок

```c#
unsafe
{
    // Unsafe-контекст: здесь можно использовать указатели
}
```

MSDN приводит следующий пример:

```c#
using System;

class Program
{
    unsafe static void Square(int* p)
    {
        *p *= *p;
    }

    unsafe static void Main()
    {
        int i = 5;
        Square(&i);
        Console.WriteLine($"{i}");
    }
}
```

#### fixed

Можно использовать буферы фиксированного размера:

```c#
internal unsafe struct MyBuffer
{
    public fixed char fixedBuffer[128];
}

internal unsafe class MyClass
{
    public MyBuffer myBuffer = default;
}

private static void AccessEmbeddedArray()
{
    MyClass myC = new MyClass();

    unsafe
    {
        // Pin the buffer to a fixed location in memory.
        fixed (char* charPtr = myC.myBuffer.fixedBuffer)
        {
            *charPtr = 'A';
        }
        // Access safely through the index:
        char c = myC.myBuffer.fixedBuffer[0];
        Console.WriteLine(c);
        // modify through the index:
        myC.myBuffer.fixedBuffer[0] = 'B';
        Console.WriteLine(myC.myBuffer.fixedBuffer[0]);
    }
}
```

Модификатором `fixed` могут быть помечены массивы элементов `bool`, `byte`, `char`, `short`, `int`, `long`, `sbyte`, `ushort`, `uint`, `ulong`, `float`, или `double`.

Модификатор `fixed` можно употреблять лишь при соблюдении следующих ограничений:

* Только в unsafe-контексте.
* Только одномерные массивы.
* Длина массива должна быть задана явно, например: `char id[8]`.
* Только поля экземпляров unsafe-структур.

#### stackalloc

Внутри unsafe-метода можно получить массив на стеке:

```c#
int *block = stackalloc int[100];
```

Разделять объявление переменной и размещение массива нельзя. Нельзя также перераспределить массив.

Начиная с C# 7.3, можно проинициализировать массив при создании:

```c#
int* first = stackalloc int[3] { 1, 2, 3 };
int* second = stackalloc int[] { 1, 2, 3 };
int* third = stackalloc[] { 1, 2, 3 };
```

Использование stackalloc автоматически активирует проверку на переполнение буфера (buffer overrun) в CLR. Если переполнение будет обнаружено, программа немедленно завершится. 

#### Что можно делать с указателями в C#

* Разыменование с помощью `*`.
* Обращение к полям и методам с помощью `->`.
* Обращение к элементам массива по индексу `[]`.
* Получение адреса переменной с помощью `&`.
* Адресная арифметика: `++`, `--`, `+`, `-`.
* Сравнение с помощью `==`, `!=`, `<`, `>`, `<=`, `>=`.
* Размещение массива на стеке с помощью `stackalloc`.
* Фиксация адреса с помощью `fixed`.
* Преобразование указателей из/к типу `void*` и любому другому типу указателя.
* Преобразование указателей из/в `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long` или `ulong`.
* Литерал `null` совместим с любым типом указателя.

#### Работа с неуправляемой кучей

```c#
using System;
using System.Runtime.InteropServices;

public unsafe class Memory
{
    // Handle for the process heap. This handle is used in all calls to the
    // HeapXXX APIs in the methods below.
    static int ph = GetProcessHeap();

    // Private instance constructor to prevent instantiation.
    private Memory() {}

    // Allocates a memory block of the given size. The allocated memory is
    // automatically initialized to zero.
    public static void* Alloc(int size) {
        void* result = HeapAlloc(ph, HEAP_ZERO_MEMORY, size);
        if (result == null) throw new OutOfMemoryException();
        return result;
    }

    // Copies count bytes from src to dst. The source and destination
    // blocks are permitted to overlap.
    public static void Copy(void* src, void* dst, int count) {
        byte* ps = (byte*)src;
        byte* pd = (byte*)dst;
        if (ps > pd) {
            for (; count != 0; count--) *pd++ = *ps++;
        }
        else if (ps < pd) {
            for (ps += count, pd += count; count != 0; count--) *--pd = *--ps;
        }
    }

    // Frees a memory block.
    public static void Free(void* block) {
        if (!HeapFree(ph, 0, block)) throw new InvalidOperationException();
    }

    // Re-allocates a memory block. If the reallocation request is for a
    // larger size, the additional region of memory is automatically
    // initialized to zero.
    public static void* ReAlloc(void* block, int size) {
        void* result = HeapReAlloc(ph, HEAP_ZERO_MEMORY, block, size);
        if (result == null) throw new OutOfMemoryException();
        return result;
    }

    // Returns the size of a memory block.
    public static int SizeOf(void* block) {
        int result = HeapSize(ph, 0, block);
        if (result == -1) throw new InvalidOperationException();
        return result;
    }

    // Heap API flags
    const int HEAP_ZERO_MEMORY = 0x00000008;

    // Heap API functions
    [DllImport("kernel32")]
    static extern int GetProcessHeap();

    [DllImport("kernel32")]
    static extern void* HeapAlloc(int hHeap, int flags, int size);

    [DllImport("kernel32")]
    static extern bool HeapFree(int hHeap, int flags, void* block);

    [DllImport("kernel32")]
    static extern void* HeapReAlloc(int hHeap, int flags, void* block, int size);

    [DllImport("kernel32")]
    static extern int HeapSize(int hHeap, int flags, void* block);
}
```

#### unsafe в .NET Framework

Конструкторы класса `System.String`

```c#
public String (char* value);
public String (sbyte* value);
public String (char* value, int startIndex, int length);
public String (sbyte* value, int startIndex, int length);
public String (sbyte* value, int startIndex, int length, Encoding enc);
```

Конструктор, методы и операторы класса `System.IntPtr`

```c#
public IntPtr (void* value);
public void* ToPointer ();
public static explicit operator IntPtr (void* value);
public static explicit operator void* (IntPtr value);
```
