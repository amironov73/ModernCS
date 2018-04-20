### It’s Noda Time!

NodaTime – альтернативная библиотека для работы с датами и временем в .NET. Автор библиотеки – легендарный Джон Скит, которого справедливо сравнивают с Чаком Норрисом (или же Чака Норриса несправедливо сравнивают с Джоном Скитом?).

Изучив стандартный тип System.DateTime и его окружение, Джон пришёл к выводу, что поддержка дат и времени в .NET сделана неадекватно, и запилил собственный проект на эту тему.

Что же не понравилось Джону в стандартной структуре DateTime?

Взглянем на следующий код:

```csharp
DateTime utc = DateTime.UtcNow;
DateTime local = DateTime.Now;
Console.WriteLine (local == utc);
```

Что же будет выведено на консоль? Большинство разработчиков путаются и не могут ответить аргументированно. (Будет выведено False, т. к. с точки зрения Microsoft.NET в переменных хранятся разные моменты времени.)

Введение структуры DateTimeOffset (которая хранит локальную дату и время с указанным смещением от UTC) в .NET 3.5 решило часть проблем, но не все.

Пример кода с использованием NodaTime:

```csharp
// Время с начала эпохи
Instant now = SystemClock.Instance.Now;
 
// Получаем время в привязке к зоне
ZonedDateTime nowInIsoUtc = now.InUtc ();
 
// Отрезок времени заданной продолжительности
Duration duration = Duration.FromMinutes (3);
 
// Прибавляем отрезок к нашему времени
ZonedDateTime thenInIsoUtc = nowInIsoUtc + duration;
 
// Поддержка временных зон
var london = DateTimeZoneProviders.Tzdb["Europe/London"];
 
// Преобразования между временными зонами
var localDate = new LocalDateTime (2012, 3, 27, 0, 45, 0);
var before = london.AtStrictly (localDate);
```

Системные требования NodaTime: Microsoft .NET, начиная с версии 3.5, поддерживается Mono с некоторыми ограничениями. Сборка PCL поддерживает:

* .NET 4 и выше
* Silverlight 4 и выше
* Windows Phone 7 и выше
* .NET for Windows Store apps

Библиотека представляет собой сборку NodaTime.dll, которая, кроме прочего, включает в себя базу данных TZDB.
