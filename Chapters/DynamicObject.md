### DynamicObject и ExpandoObject

В .NET Framework 4 появилась возможность динамической типизации при помощи ключевого слова dynamic:
```csharp
dynamic someObject = SomeMethod ();
```
К свойствам и методам dynamic-объекта можно обращаться без оглядки, описаны они статически или нет:
```csharp
someObject.ExistentMethod (); // Такой метод есть
someObject.NotExistentMethod (); // А такого нет
```
Если во время исполнения программы среда не найдёт соответствующего метода, будет вызвано исключение `RuntimeBinderException` с сообщением ‘SomeClass’ does not contain a definition for ‘NonExistentMethod’.

Чтобы сделать эту возможность обращения к произвольному методу класса более полезной, в Framework был добавлен класс System.Dynamic.DynamicObject, содержащий метод для перехвата обращения к несуществующим членам:
```csharp
bool TryInvokeMember
    (
        InvokeMemberBinder binder, 
        object[] args, 
        out object result
    );
```
Для ещё большего удобства в Framework предусмотрен класс `System.Dynamic.ExpandoObject`, который позволяет динамически добавлять свойства и методы по мере необходимости.
```csharp
dynamic eo = new ExpandoObject ();
eo.Name = "Alexey Mironov";
eo.Age = 2010 - 1973 + 1;
eo.City = "Irkutsk";
eo.PrintInfo = new Action ( () => Console.WriteLine 
      ( "{0} is {1} years old", eo.Name, eo.Age) );
...
eo.PrintInfo ();
```
ExpandoObject можно сериализовать в XML с помощью `System.Runtime.Serialization.DataContractSerializer`:
```csharp
var s = new DataContractSerializer
    (
        typeof (IDictionary<string, object>)
    );
var sb = new StringBuilder();
using (var xw = XmlWriter.Create(sb))
{
  s.WriteObject(xw, eo);
}
Console.WriteLine(sb.ToString());
```