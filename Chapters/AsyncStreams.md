### Асинхронные потоки

В C# 8 добавлена возможность асинхронного перебора элементов последовательности. Почему-то это принято называть Async Streams, хотя к обычным потокам `System.IO.Stream` эта функциональность не имеет никакого отношения (там своя асинхронность).

Для реализации асинхронного перебора добавлены интерфейсы `IAsyncEnumerable<T>`, `IAsyncEnumerator<T>` и `IAsyncDisposable`, которые определены так:

```c#
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    // Получение перечислителя
    public interface IAsyncEnumerable<out T>
    {
        IAsyncEnumerator<T> GetAsyncEnumerator
            (
                CancellationToken cancellationToken = default
            );   
    }
    
    // Асинхронный перечислитель
    public interface IAsyncEnumerator<out T> : IAsyncDisposable
    {
        // Переход к следующему элементу последовательности
        ValueTask<bool> MoveNextAsync();
        
        // Текущий элемент
        T Current { get; }
    }
}

namespace System
{
    // Асинхронное освобождение занятых ресурсов
    public interface IAsyncDisposable
    {
        ValueTask DisposeAsync();
    }
}
```

Пример асинхронного потока целых чисел:

```c#
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    // асинхронно выдаём десяток целых чисел
    static async IAsyncEnumerable<int> GetSomeNumbersAsync()
    {
        for (int i = 0; i < 10; i++)
        {
            // имитируем некую длительную операцию
            await Task.Delay(100);
            yield return i;
        }
    }

    static async Task Main()
    {
        // асинхронно перечисляем числа из последовательности
        await foreach (var number in GetSomeNumbersAsync())
        {
            Console.WriteLine(number);
        }
    }
}
```

Здесь важно обратить внимание на конструкцию `async foreach`, которая заставляет компилятор генерировать машину состояний.
