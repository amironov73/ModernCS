### Библиотека Polly

Polly — это библиотека, которая позволяет .NET-разработчикам добавить в свои приложения устойчивость к ошибкам и контроль за переходом из одного состояния в другое. Она предоставляет такие политики: Retry, Circuit Breaker, Timeout, Bulkhead Isolation и Fallback.

Сайт проекта: http://www.thepollyproject.org, GitHub: https://github.com/App-vNext/Polly, NuGet: https://www.nuget.org/packages/Polly/. Репозиторий с примерами: https://github.com/App-vNext/Polly-Samples.

Поддерживаются: .NET 4.5, .NET Standard 1.1 и 2.0. Версии 4.x поддерживали .NET 3.5 и 4.0.

```csharp
using System;
using System.Net;
 
using Polly;
 
class Program
{
    static void Main()
    {
        var policy = Policy.Handle<Exception>().Retry(3, (ex, attempt) =>
        {
            Console.WriteLine($"Got {ex.Message}");
            Console.WriteLine($"Attempt: {attempt}");
        });
 
        using (WebClient client = new WebClient())
        {
            policy.Execute
                (
                    () =>
                    {
                        string s = client.DownloadString("http://noogle.com");
                        Console.WriteLine($"Downloaded: {s}");
                    }
                );
        }
    }
}
```

выдаёт

```
Attempt: 1
Got An error occurred while sending the request. Не удается установить соединение с сервером
Attempt: 2
Got An error occurred while sending the request. Не удается установить соединение с сервером
Attempt: 3
 
Unhandled Exception: System.Net.WebException: An error occurred while sending the request.
Не удается установить соединение с сервером ---> System.Net.Http.HttpRequestException: An error occurred while sending the request. ---> System.Net.Http.WinHttpException: Не удается установить соединение с сервером
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Threading.Tasks.RendezvousAwaitable`1.GetResult()
   at System.Net.Http.WinHttpHandler.d__105.MoveNext()
   --- End of inner exception stack trace ---
C:\projects\polly\src\Polly.Shared\Retry\RetrySyntax.cs:line 97
   at Polly.Policy.ExecuteInternal(Action`2 action, Context context, CancellationToken cancellationToken) in C:\projects\polly\src\Polly.Shared\Policy.cs:line 43
   at Polly.Policy.Execute(Action`2 action, Context context, CancellationToken cancellationToken) in C:\projects\polly\src\Polly.Shared\Policy.ExecuteOverloads.cs:line 83
   at Program.Main() in D:\Projects\Misc\ConsoleApp40\ConsoleApp40\Program.cs:line 18
```

|Политика|Посылка|По-просту|Что можно сделать?|
|--------|-------|---------|------------------|
|Retry (семейство)|Сбой является временным и после короткой паузы может самостоятельно восстановиться.|«Возможно, это был случайный глюк»|был случайный глюк»	Можно настроить автоматическое повторение попыток.|
|Circuit Breaker(семейство)|Когда система слишком загружена, лучше сразу выдавать ошибку, чем заставлять потребителя напрасно ждать. Защита от перегрузки системы.|«Прекратить, если станет невыносимо», «Дать системе передышку»|Запрещает выполнение на некоторое время, если количество сбоев превысит заданный порог.|
|Timeout|При превышении тайм-аута дальше ждать не имеет смысла.|«Не ждать вечно»|Гарантирует, что потребитель будет ждать не более указанного периода.|
|Bulkhead Isolation|Когда происходит сбой, последовательные запросы могут «съесть» все доступные ресурсы хоста. Сбойная подсистема может привести к перерасходу ресурсов вышестоящей подсистемы. Сбой в одной подсистеме может распространиться и на другие.|«Одна ошибка не должна потопить весь корабль»|Ограничивает управляемую подсистему пулом ресурсов фиксированного размера, изолируя её от других.|
|Cache|Некоторые запросы могут быть однотипными.|«Вы уже спрашивали это»|Предоставляет ответ из кеша. Значения помещаются в кеш при первом успешном выполнении.|
|Fallback|Планируйте, что будете говорить, когда всё пойдёт наперекосяк.|«Вежливый отказ»|Задаёт значение, которое будет возвращено в случае сбоя.|
|PolicyWrap|Разные сбои требуют разных подходов; гибкость означает комбинирование стратегий.|«Эшелонированная защита»|Можно гибко комбинировать несколько политик.|
