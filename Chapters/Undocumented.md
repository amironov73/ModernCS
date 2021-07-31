### Недокументированные ключевые слова C#

В C# есть четыре недокументированных ключевых слова: `__arglist`, `__reftype`, `__makeref` и `__refvalue`. Зачем они нужны?

`__makeref` создаёт ссылку на тип переданного ему аргумента. Ссылка имеет тип `System.TypedReference` – это весьма своеобразная структура, для которой компилятор отказывается даже вызывать метод `ToString()`. Впрочем, полученная ссылка сама по себе не нужна, её передают в качестве аргумента второго ключевого слова `__reftype`, получая обычный класс `System.Type`. Или с помощью третьего ключевого слова `__refvalue` добираются до значения, хранящегося в типизированной ссылке. Наконец, четвёртое ключевое слово позволяет передавать в метод произвольное количество аргументов произвольного типа.

Лучше тысячи слов – один пример кода:

```csharp
using System;

namespace Undocumented
{
    class Program
    {
        static void Main()
        {
            var i = 21;
            var tr = __makeref(i);
            var t = __reftype (tr);
            Console.WriteLine (t.ToString());
            var rv = __refvalue (tr, int);
            Console.WriteLine (rv);
            __refvalue(tr, int) = 22; // *tr = 22

            SomeMethod (__arglist (1, 2, 3, 5, 6));
        }
        
        static void SomeMethod (__arglist)
        {
            var ai = new ArgIterator (__arglist);
            while (ai.GetRemainingCount() &gt; 0)
            {
                var tr = ai.GetNextArg();
                Console.WriteLine (TypedReference.ToObject (tr));

                // можно так
                // var i = __refvalue(tr, int);
                // Console.WriteLine(i);
            }
        }
    }
}
```

В общем, ничего такого «этакого» за недокументированными ключевыми словами не скрывается. Те, кто ими не пользовался до сих пор, могут продолжать не пользоваться дальше.
