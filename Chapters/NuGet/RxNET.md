### Rx.NET - реактивные расширения

«Реактивный» означает «работающий аналогично традиционным .NET-событиям, только по-другому». Обычно мы подписываемся на событие так:
```csharp
theObject.SomeEvent += OurHandler;
```
и терпеливо ждём, когда theObject вызовет наш обработчик события. Реактивная подписка происходит аналогично, но с важными отличиями:
```csharp
public interface IObserver<T>
{
    // появился новый элемент для обработки
    void OnNext ( T value );
 
    // возникла ошибка
    void OnError ( Exception error );
 
    // обработка завершена, больше элементов не будет
    void OnComplete ();
}
 
public interface IObservable<T>
{
    // подписываемся на извещения о элементах, ошибках
    // и окончании обработки
    IDisposable Subscribe (IObserver<T> observer);
}
```
Сами по себе эти интерфейсы погоды не делают, нужна инфраструктура. Её предлагает библиотека Reactive Extensions (RX): в ней множество способов организации обработки данных в соответствии с парадигмой «Observable + Observer». Домашняя страница проекта: https://msdn.microsoft.com/en-us/data/gg577609, исходный код: https://github.com/Reactive-Extensions/Rx.NET, пакет NuGet: https://www.nuget.org/packages/Rx-Main/.

Для иллюстрации идеологии RX часто рисуют следующую табличку:

| | Единственное значение | Множество значений |
|-|-----------------------|--------------------|
| Pull/синхронный/интерактивный | T | IEnumerable&lt;T&gt; |
| Push/асинхронный/реактивный | Task&lt;T&gt; | IObservable&lt;T&gt; |

Простейший пример (здесь и далее версия RX-Main 1.0.11226 на десктопном .NET 4.0):
```csharp
using System;
using System.Reactive.Linq;
 
class Program
{
    static void Main()
    {
        var items = new[] { 2, 12, 85, 0, 6 }.ToObservable();
        items.Subscribe
            (
                item => Console.WriteLine("Элемент {0}", item),
                ex => Console.WriteLine("Ошибка: {0}", ex.Message),
                () => Console.WriteLine("Обработка завершена")
            );
    }
}
```
<table>
<tr>
  <th>OnNext</th>
  <th>OnError</th>
  <th>OnComplete</th>
</tr>
<tr>
  <td>Вызывается 0 или более раз</td>
  <td colspan="2">Однократно вызывается один из двух методов</td>
</tr>
</table>

Обратите внимание, будет вызван либо метод OnComplete, либо OnError (после чего OnNext для данной подписки вызываться больше не будет). Можно применить метод-расширение Finally, чтобы отреагировать на прекращение работы подписки по любому поводу:
```csharp
items
.Finally
    (
        () => Console.WriteLine("Обработка завершена по любой причине")
    )
.Subscribe
    (
        item => Console.WriteLine("Элемент {0}", item),
        ex => Console.WriteLine("Обработка завершена из-за ошибки"),
        () => Console.WriteLine("Обработка завершена нормально")
    );
```
Подписку можно прервать в любой момент, вызвав Dispose:
```csharp
var subscription = items.Subscribe
    (
        item => Console.WriteLine("Элемент {0}", item),
        ex => Console.WriteLine("Ошибка: {0}", ex.Message),
        () => Console.WriteLine("Обработка завершена")
    );
subscription.Dispose();
```
К потоку обозреваемых элементов можно применять методы-расширения, знакомые нам по LINQ, например, Select, Where, Group и т. д.:
```csharp
items
    .Select(x => x * 2)
    .Subscribe
    (
        item => Console.WriteLine("Элемент {0}", item),
        ex => Console.WriteLine("Ошибка: {0}", ex.Message),
        () => Console.WriteLine("Обработка завершена")
    );
```
Для создания observable-потока можно использовать метод-расширение ToObservable (как в примере выше) или методы класса Observable:
```csharp
// будет вызван лишь OnComplete
Observable.Empty<T>();
 
// будет вызван лишь OnError
Observable.Throw<T>(new Exception("Oops"));
 
// будет однократно вызван OnNext, затем OnComplete
Observable.Return(42);
 
// будут сгенерированы элементы: 5, 6 и 7
Observable.Range(5, 3);
 
// вообще ничего не будет вызвано
Observable.Never<int>();
 
// повторить значение 5 десять раз
Observable.Repeat(5, 10);
 
// генерируем последовательность и повторяем её 10 раз
var first = Observable.Range(1, 10);
var second = first.Repeat(10);
 
// генерация последовательности в функциональном стиле
var items = Observable.Generate
    (
        1,             // начальный элемент
        x => x < 1024, // условие
        x => x*2,      // итератор
        x => x         // селектор
    );
```
Если при генерации последовательности произойдёт ошибка, приложение может быть закрыто. Чтобы избежать этого, можно использовать метод-расширение Catch. При вызове указывается значение, которое нужно будет вернуть вместо того, на котором возникла ошибка (генерация на этом будет прервана):
```csharp
items
    .Catch(Observable.Empty<int>())
    .Subscribe (...);
```
