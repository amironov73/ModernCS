### Новые возможности C# 6.0

#### using для статических типов
```csharp
using System.Math;
 
...
 
double oneHalf = Sin ( 30.0 * PI / 180.0 );
```
#### Интерполяция строк
```csharp
string name = "Alexey Mironov";
string city = "Irkutsk";
int age = 41;
 
Console.WriteLine($"{name} lives in {city}, he's {age:D3} years old");
```
Интерполируемая строка описывается следующим форматом:
```
$ " <text> { <interpolation-expression> <optional-comma-field-width> 
  <optional-colon-format> } <text> ... "
```
Внутри фигурных скобок можно помещать не просто ссылки на имена переменных или свойств, но и довольно сложные конструкции:
```csharp
$"{person.Name, 20} is {person.Age:D3} year {(p.Age == 1 ? "" : "s")} old."
```
#### Инициализация свойств
```csharp
class Employee
{
  public string Name { get; set; } // без инициализации
 
  public string Job { get; set; } = "Worker"; // с инициализацией
}
```
Инициализировать подобным образом можно и свойства «только для чтения».
#### Инициализация словарей
Почему-то считается, что следующий синтаксис
```csharp
var dictionary = new Dictionary<string, string> ()
{
  ["name"] = "Alexey Mironov",
  ["job"] = "Librarian",
  ["city"] = "Irkutsk"
};
```
более удобный, чем старый
```csharp
var dictionary = new Dictionary<string, string> ()
{
  { "name", "Alexey Mironov" },
  { "job", "Librarian" },
  { "city", "Irkutsk" }
};
```
Новый синтаксис по задумке авторов C# должен напоминать цепочку присваиваний. По-моему, сильно удобнее не стало. Радует то, что новым синтаксисом пользоваться необязательно.
#### nameof
Крайне полезный компаньон к `typeof`.
```csharp
private string _department;
 
public string Department
{ 
  get
  {
      return _department;
  }
  set
  {
      _department = value;
      OnPropertyChanged ( nameof (Department) );
  }
}
 
...
 
void SomeFunction ( string argument )
{
  LogMethodEntry ( nameof (SomeFunction) );
  ...
  if (argument == null)
    throw new ArgumentNullException ( nameof (argument) );
  ...
}
```
Resharper до некоторой степени снимал остроту проблемы переименования переменных/свойств, однако как решение «из коробки» `nameof`, несомненно, круче.
#### Упрощённая проверка на null
```csharp
Console.WriteLine ( superHero?.SuperPower ?? "Not superhero" );
```
Допускается произвольная вложенность конструкций «?»:
```csharp
Console.WriteLine ( customer?.Orders?[5] );
```
Есть возможность совместить проверку на `null` с вызовом делегата:
```csharp
PropertyChanged?.Invoke(e);
```
Совмещённый вызов хорош тем, что является потокобезопасным – компилятор вычисляет делегат лишь однажды, помещая вычисленное значение во временную переменную. `Invoke` необходимо вызывать явно, т. к. сократить подобным образом не получится:
```csharp
PropertyChanged? (e);
```
#### Лямбды в свойствах
```csharp
class Employee
{
    public string FirsName { get; set; }
 
    public string LastName { get; set; }
 
    public string FullName => FirstName + " " + LastName;
}
```
#### Фильтры исключений
По-моему, довольно дурацкое нововведение, без которого совершенно спокойно можно было обойтись.
```csharp
try
{
    // Some code here
}
catch (Exception ex) if (ex.Message.Equals ("400"))
{
    Console.WriteLine ( "Bad request" );
}
catch (Exception ex) if (ex.Message.Equals ("401"))
{
    Console.WriteLine ( "Unauthorized" );
}
```
Можно сделать так:
```csharp
private static bool _MyFilter 
 ( 
    string originalMessage, 
    out string customMessage
  )
{
    customMessage = originalMessage;
 
    if ( originalMessage == "400" )
    {
        customMessage = "Bad request";
        return true;
    }
    if ( originalMessage == "401" )
    {
        customMessage = "Unauthorized";
        return true;
    }
 
    return false;
}
 
...
 
try
{
    // Some code here
}
catch (Exception ex) if (_MyFilter (ex.Message, out customMessage))
{
    Console.WriteLine (customMessage);
}
```
Подводя итог: на прорыв, аналогичный LINQ, dynamic или async не тянет, но жить можно.

На всякий случай напоминаю, что перечисленные возможности относятся к компилятору, а не к рантайму, поэтому будут работать не только в .Net Framework 4.6, но и в 4.0, 4.5 и 4.5.1.
