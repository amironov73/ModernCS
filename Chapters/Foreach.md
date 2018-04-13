### Как работает foreach

Во что реально превращает цикл `foreach` компилятор C#, написано в спецификации языка, в разделе 8.8.4. Строчка
```csharp
foreach (ElementType element in collection) statement
```
превращается в один из двух вариантов. Если `collection` честно реализует интерфейс `IEnumerable`, будет сгенерировано следующее:
```csharp
IEnumerator enumerator = 
        ((System.Collections.IEnumerable)(collection)).GetEnumerator();
try
{
   while (enumerator.MoveNext()) 
   {
      ElementType element = (ElementType)enumerator.Current;
      statement;
   }
}
finally
{
   IDisposable disposable = enumerator as System.IDisposable;
   if (disposable != null) disposable.Dispose();
}
```
Иначе (если `collection` не реализует `IEnumerable`), компилятор смотрит, нет ли у неё публичного метода `GetEnumerator()`. Если он есть и возвращает что угодно, что имеет свойство Current и метод `MoveNext`, будет сгененирован похожий, но всё-таки отличающийся код:
```csharp
E enumerator = (collection).GetEnumerator();
try
{
   while (enumerator.MoveNext()) 
   {
      ElementType element = (ElementType)enumerator.Current;
      statement;
   }
}
finally
{
   IDisposable disposable = enumerator as System.IDisposable;
   if (disposable != null) disposable.Dispose();
}
```
Это так называемая «утиная типизация». Если некий класс плавает подобно `IEnumerable` и крякает подобно `IEnumerable`, то фиг с ним, будем использовать его как `IEnumerable`.
