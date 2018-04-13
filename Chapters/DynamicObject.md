### DynamicObject � ExpandoObject

� .NET Framework 4 ��������� ����������� ������������ ��������� ��� ������ ��������� ����� dynamic:
```csharp
dynamic someObject = SomeMethod ();
```
� ��������� � ������� dynamic-������� ����� ���������� ��� �������, ������� ��� ���������� ��� ���:
```csharp
someObject.ExistentMethod (); // ����� ����� ����
someObject.NotExistentMethod (); // � ������ ���
```
���� �� ����� ���������� ��������� ����� �� ����� ���������������� ������, ����� ������� ���������� `RuntimeBinderException` � ���������� �SomeClass� does not contain a definition for �NonExistentMethod�.

����� ������� ��� ����������� ��������� � ������������� ������ ������ ����� ��������, � Framework ��� �������� ����� System.Dynamic.DynamicObject, ���������� ����� ��� ��������� ��������� � �������������� ������:
```csharp
bool TryInvokeMember
    (
        InvokeMemberBinder binder, 
        object[] args, 
        out object result
    );
```
��� ��� �������� �������� � Framework ������������ ����� `System.Dynamic.ExpandoObject`, ������� ��������� ����������� ��������� �������� � ������ �� ���� �������������.
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
ExpandoObject ����� ������������� � XML � ������� `System.Runtime.Serialization.DataContractSerializer`:
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