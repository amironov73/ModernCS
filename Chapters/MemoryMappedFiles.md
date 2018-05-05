### MemoryMappedFiles

Начиная с .NET 4.0 стало доступно пространство имен `System.IO.MemoryMappedFiles`, поддерживающее замечательный механизм ввода-вывода.

Алгоритм работы таков:

1. Открываем нужный нам файл с помощью `FileStream`.
2. С помощью `MemoryMappedFile.CreateFromFile` создаем объект MemoryMappedFile.
3. Теперь нужно сделать одно из двух: создать либо поток `MemoryMappedViewStream` либо акцессор ``.

Пример создания потока:

```csharp
using System;
using System.IO;
using System.IO.MemoryMappedFiles;

class Program
{
    static void Main()
    {
        using (FileStream fs = new FileStream
            (
                path: "Mastering Git.pdf",
                mode: FileMode.Open,
                access: FileAccess.Read
            ))
        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile
            (
                fileStream: fs,
                mapName: null,
                capacity: 0,
                access: MemoryMappedFileAccess.Read,
                inheritability: HandleInheritability.None,
                leaveOpen: false
            ))
        using (MemoryMappedViewStream mmvs = mmf.CreateViewStream
            (
                offset: 0,
                size: 0,
                access: MemoryMappedFileAccess.Read
            ))
        {
            byte[] bytes = new byte[8];
            mmvs.Read(bytes, 0, bytes.Length);
            foreach (byte b in bytes)
            {
                Console.Write($"{b:X2} ");
            }

            Console.WriteLine();
        }
    }

    // Напечатает
    // 25 50 44 46 2D 31 2E 36
}
```

Более удобным представляется создание так называемого акцессора, позволяющего считывать значения с произвольной позиции:

```csharp
using System;
using System.IO;
using System.IO.MemoryMappedFiles;

class Program
{
    static void Main()
    {
        ...
        using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor
            (
                offset: 0,
                size: 0,
                access: MemoryMappedFileAccess.Read
            ))
        {
            long value = accessor.ReadInt64(0);
            Console.WriteLine(value.ToString("X8"));
        }
    }

    // Напечатает
    // 362E312D46445025
}
```

С помощью акцессора можно считывать целые структуры:

```csharp
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
struct MyStruct
{
    public int FirstValue;
    public int SecondValue;
}

class Program
{
    static void Main()
    {
        ...
            accessor.Read(0, out MyStruct myStruct);
            Console.WriteLine($"{myStruct.FirstValue:X4} {myStruct.SecondValue:X4}");
    }

    // Напечатает
    // 46445025 362E312D
}
```

С помощью `Span<T>` можно обращаться к акцессору как к обычному массиву в памяти:

```csharp
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using Microsoft.Win32.SafeHandles;

class Program
{
    static void Main()
    {
        ...
     int length = (int)fs.Length;
     SafeMemoryMappedViewHandle handle = accessor.SafeMemoryMappedViewHandle;
     unsafe
     {
         byte* pointer = null;
         handle.AcquirePointer(ref pointer);

         var span = new ReadOnlySpan<byte>(pointer, length);
         for (int i = 0; i < 8; i++)
         {
             char c = (char)span[i];
             Console.Write(c);
         }

         handle.ReleasePointer();
     }

     Console.WriteLine();

    // Напечатает
    // %PDF-1.6
    }
}
```

Обратите внимание, что к одному `MemoryMappedFile` одновременно может быть подключено произвольное количество потоков и акцессоров — главное, чтобы запрошенные ими права доступа были совместимы с режимом доступа к самому файлу.

При помощи режима доступа `CopyOnWrite` каждый поток/акцессор получает свою приватную копию, в которую можно вносить изменения незаметно для остальных потоков/акцессоров.
