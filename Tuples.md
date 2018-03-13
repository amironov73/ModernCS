### Кортежи

Чтобы использовать ValueTuple, нужно подключить NuGet: https://www.nuget.org/packages/System.ValueTuple/ (кроме FW 4.7 и .NET Core 2.0, где это происходит само).
```csharp
var tuple1 = (1, 2); // без присвоения имён
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
Обратите внимание, методы `GetHashCode` и `Equals` для кортежей генерируются автоматически!

Два кортежа с одинаковыми типами элементов, но с разными именами, совместимы: `(int a, int b) = (1, 2)`.

Кортежи имеют семантику значений: `(1,2) .Equals ((a: 1, b: 2))` и `(1,2) .GetHashCode () == (1,2) .GetHashCode ()` являются истинными.

Кортежи не поддерживают == и !=.

Кортежи могут быть «деконструированы», но только в «объявление переменной», но не в «out var» или в блок `case`:
```csharp
var (x, y) = (1,2) — OK, (var x, int y) = ( 1,2) — OK
dictionary.TryGetValue (key, out var (x, y)) — не OK, case var (x, y): break; — не ОК.
```
Кортежи изменяются: `(int a, int b) x (1,2); x.a++;`.

Элементы кортежа можно получить по имени (если указано при объявлении) или через общие имена, такие как `Item1`, `Item2` и т. д.
