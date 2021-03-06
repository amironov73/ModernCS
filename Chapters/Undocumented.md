﻿### Недокументированные ключевые слова C#

В C# есть четыре недокументированных ключевых слова: `__arglist`, `__reftype`, `__makeref` и `__refvalue`. Зачем они нужны?

`__reftype` создаёт ссылку на тип переданного ему аргумента. Ссылка имеет тип System.TypedReference – это весьма своеобразная структура, для которой компилятор отказывается даже вызывать метод ToString(). Впрочем, полученная ссылка сама по себе не нужна, её передают в качестве аргумента второго ключевого слова `__reftype`, получая обычный класс `System.Type`. Или с помощью третьего ключевого слова `__refvalue` добираются до значения, хранящегося в типизированной ссылке. Наконец, четвёртое ключевое слово позволяет передавать в метод произвольное количество аргументов произвольного типа.

Лучше тысячи слов – один пример кода:
```csharp
int i = 21;
TypedReference tr = __makeref(i);
Type t = __reftype (tr);
Console.WriteLine (t.ToString());
int rv = __refvalue (tr, int);
Console.WriteLine (rv);
SomeMethod (__arglist (1, 2, 3, 5, 6));
 
...
 
SomeMethod (__arglist)
{
    ArgIterator ai = new ArgIterator (__arglist);
    while (ai.GetRemainingCount() &gt; 0)
    {
        TypedReference tr = ai.GetNextArg();
        Console.WriteLine (TypedReference.ToObject (tr));
    }
}
```
В общем, ничего такого «этакого» за недокументированными ключевыми словами не скрывается. Те, кто ими не пользовался до сих пор, могут продолжать не пользоваться дальше.
