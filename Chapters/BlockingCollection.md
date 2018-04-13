### BlockingCollection

В .NET 4.0 появилось пространство имён `System.Collections.Concurrent`, подарившее прикладным программистом специальные типы коллекций, предназначенные для многопоточных приложений. Теперь, вместо того, чтобы заниматься велосипедостроением, можно кататься на заводском велосипеде. 🙂

Класс `BlockingCollection` реализует шаблон «Производитель-Потребитель» («Producer-Consumer»). Это означает, что один (или несколько) потоков помещает элементы в коллекцию, а другой (или несколько других) поток забирает их оттуда, забота о блокировках и потокобезопасности перекладывается на программистов Microsoft.

Элементы в коллекцию добавляются методом `Add` (или `TryAdd`), забираются методом `Take` (лучше `TryTake`). Производитель может вызвать метод `CompleteAdding`, чтобы указать, что больше элементов в коллекцию добавляться не будет. Тогда свойство `IsCompleted` укажет потребителю, что коллекция опустела не просто так, и обработку можно завершать. Удобно забирать элементы из коллекции с помощью `GetConsumingEnumerable()`.

Для особо оголтелых программистов 🙂 предусмотрены методы `AddToAny` и `TakeFromAny`:

```csharp
public static int AddToAny
(
  BlockingCollection<T>[] collections,
  T item
);
 
public static int TakeFromAny
(
  BlockingCollection<T>[] collections,
  out T item
);
```
а также парные к ним `TryAddToAny` и `TryTakeFromAny`.

Важно: `BlockingCollection` – `IDisposable`, так что надо не забывать вызывать `Dispose`!

Небольшой пример:

```csharp
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
 
class Program
{
    static void Main()
    {
        BlockingCollection<int> collection = new BlockingCollection<int>();
 
        Task producer1 = Task.Factory.StartNew
            (
                () =>
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        collection.Add(i);
                        Thread.Sleep(10);
                    }
                }
            );
        Task producer2 = Task.Factory.StartNew
            (
                () =>
                {
                    for (int i = 2001; i >= 1900; i--)
                    {
                        collection.Add(i);
                        Thread.Sleep(10);
                    }
                }
            );
        Task consumer = Task.Factory.StartNew
            (
                () =>
                {
                    while (!collection.IsCompleted)
                    {
                        int number;
                        if (collection.TryTake(out number))
                        {
                            Console.Write(" {0}", number);
                        }
                    }
                }
            );
 
        Task.WaitAll(producer1, producer2);
        collection.CompleteAdding();
 
        consumer.Wait();
 
        Console.WriteLine();
        Console.WriteLine("All done!");
    }
}
```
Печатает примерно такое:
```
 1 2001 2 2000 3 1999 1998 4 1997 5 1996 6 7 1995 1994 8 1993 9 1992 10 1991 11
12 1990 13 1989 1988 14 15 1987 16 1986 17 1985 1984 18 1983 19 1982 20 1981 21
1980 22 1979 23 1978 24 1977 25 26 1976 1975 27 1974 28 1973 29 1972 30 1971 31
1970 32 1969 33 1968 34 1967 35 1966 36 1965 37 1964 38 1963 39 1962 40 1961 41
1960 42 1959 43 1958 44 1957 45 1956 46 1955 47 1954 48 1953 49 1952 50 1951 51
1950 52 1949 53 1948 54 1947 55 1946 56 1945 57 1944 58 1943 59 1942 60 1941 61
1940 62 1939 63 1938 64 1937 65 1936 66 1935 67 1934 68 1933 69 1932 70 1931 71
1930 72 1929 73 1928 74 1927 75 1926 76 1925 77 1924 78 1923 79 1922 80 1921 81
1920 82 1919 83 1918 84 1917 85 1916 86 1915 87 1914 88 1913 89 1912 90 1911 91
1910 92 1909 93 1908 94 1907 95 1906 96 1905 97 1904 98 1903 99 1902 100 1901
1900
All done!
```
