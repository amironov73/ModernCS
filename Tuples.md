###  ортежи

„тобы использовать ValueTuple, нужно подключить NuGet: https://www.nuget.org/packages/System.ValueTuple/ (кроме FW 4.7 и .NET Core 2.0, где это происходит само).
```csharp
var tuple1 = (1, 2); // без присвоени€ имЄн
var tuple2 = (a: 3, b: 4); // с именами
 
// словарь с кортежами
var dictionary = new Dictionary<(int x, int y), (byte a, short b)>();
dictionary.Add(tuple1, (a: 3, b: 4));
if (dictionary.TryGetValue((1, 2), out var r))
{
    // при деконструкции игнорируем первый элемент
    var (_, b) = r;
         
    Console.WriteLine($"a: {r.a}, b: {r.Item2}");
}
```
ќбратите внимание, методы `GetHashCode` и `Equals` дл€ кортежей генерируютс€ автоматически!

ƒва кортежа с одинаковыми типами элементов, но с разными именами, совместимы: `(int a, int b) = (1, 2)`.

 ортежи имеют семантику значений: `(1,2) .Equals ((a: 1, b: 2))` и `(1,2) .GetHashCode () == (1,2) .GetHashCode ()` €вл€ютс€ истинными.

 ортежи не поддерживают == и !=.

 ортежи могут быть Ђдеконструированыї, но только в Ђобъ€вление переменнойї, но не в Ђout varї или в блок `case`:
```csharp
var (x, y) = (1,2) Ч OK, (var x, int y) = ( 1,2) Ч OK
dictionary.TryGetValue (key, out var (x, y)) Ч не OK, case var (x, y): break; Ч не ќ .
```
 ортежи измен€ютс€: `(int a, int b) x (1,2); x.a++;`.

Ёлементы кортежа можно получить по имени (если указано при объ€влении) или через общие имена, такие как `Item1`, `Item2` и т. д.
