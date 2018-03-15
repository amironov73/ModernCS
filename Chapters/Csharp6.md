### ����� ����������� C# 6.0

#### using ��� ����������� �����
```csharp
using System.Math;
 
...
 
double oneHalf = Sin ( 30.0 * PI / 180.0 );
```
#### ������������ �����
```csharp
string name = "Alexey Mironov";
string city = "Irkutsk";
int age = 41;
 
Console.WriteLine($"{name} lives in {city}, he's {age:D3} years old");
```
��������������� ������ ����������� ��������� ��������:
```
$ " <text> { <interpolation-expression> <optional-comma-field-width> 
  <optional-colon-format> } <text> ... "
```
������ �������� ������ ����� �������� �� ������ ������ �� ����� ���������� ��� �������, �� � �������� ������� �����������:
```csharp
$"{person.Name, 20} is {person.Age:D3} year {(p.Age == 1 ? "" : "s")} old."
```
#### ������������� �������
```csharp
class Employee
{
  public string Name { get; set; } // ��� �������������
 
  public string Job { get; set; } = "Worker"; // � ��������������
}
```
���������������� �������� ������� ����� � �������� ������� ��� �������.
#### ������������� ��������
������-�� ���������, ��� ��������� ���������
```csharp
var dictionary = new Dictionary<string, string> ()
{
  ["name"] = "Alexey Mironov",
  ["job"] = "Librarian",
  ["city"] = "Irkutsk"
};
```
����� �������, ��� ������
```csharp
var dictionary = new Dictionary<string, string> ()
{
  { "name", "Alexey Mironov" },
  { "job", "Librarian" },
  { "city", "Irkutsk" }
};
```
����� ��������� �� ������� ������� C# ������ ���������� ������� ������������. ��-�����, ������ ������� �� �����. ������ ��, ��� ����� ����������� ������������ �������������.
#### nameof
������ �������� ��������� � `typeof`.
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
Resharper �� ��������� ������� ������ ������� �������� �������������� ����������/�������, ������ ��� ������� ��� ������� `nameof`, ����������, �����.
#### ���������� �������� �� null
```csharp
Console.WriteLine ( superHero?.SuperPower ?? "Not superhero" );
```
����������� ������������ ����������� ����������� �?�:
```csharp
Console.WriteLine ( customer?.Orders?[5] );
```
���� ����������� ���������� �������� �� `null` � ������� ��������:
```csharp
PropertyChanged?.Invoke(e);
```
����������� ����� ����� ���, ��� �������� ���������������� � ���������� ��������� ������� ���� �������, ������� ����������� �������� �� ��������� ����������. `Invoke` ���������� �������� ����, �. �. ��������� �������� ������� �� ���������:
```csharp
PropertyChanged? (e);
```
#### ������ � ���������
```csharp
class Employee
{
    public string FirsName { get; set; }
 
    public string LastName { get; set; }
 
    public string FullName => FirstName + " " + LastName;
}
```
#### ������� ����������
��-�����, �������� �������� ������������, ��� �������� ���������� �������� ����� ���� ��������.
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
����� ������� ���:
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
������� ����: �� ������, ����������� LINQ, dynamic ��� async �� �����, �� ���� �����.

�� ������ ������ ���������, ��� ������������� ����������� ��������� � �����������, � �� � ��������, ������� ����� �������� �� ������ � .Net Framework 4.6, �� � � 4.0, 4.5 � 4.5.1.
