### �������

����� ������������ ValueTuple, ����� ���������� NuGet: https://www.nuget.org/packages/System.ValueTuple/ (����� FW 4.7 � .NET Core 2.0, ��� ��� ���������� ����).
```csharp
var tuple1 = (1, 2); // ��� ���������� ���
var tuple2 = (a: 3, b: 4); // � �������
 
// ������� � ���������
var dictionary = new Dictionary<(int x, int y), (byte a, short b)>();
dictionary.Add(tuple1, (a: 3, b: 4));
if (dictionary.TryGetValue((1, 2), out var r))
{
    // ��� ������������� ���������� ������ �������
    var (_, b) = r;
         
    Console.WriteLine($"a: {r.a}, b: {r.Item2}");
}
```
�������� ��������, ������ `GetHashCode` � `Equals` ��� �������� ������������ �������������!

��� ������� � ����������� ������ ���������, �� � ������� �������, ����������: `(int a, int b) = (1, 2)`.

������� ����� ��������� ��������: `(1,2) .Equals ((a: 1, b: 2))` � `(1,2) .GetHashCode () == (1,2) .GetHashCode ()` �������� ���������.

������� �� ������������ == � !=.

������� ����� ���� ������������������, �� ������ � ����������� ����������, �� �� � �out var� ��� � ���� `case`:
```csharp
var (x, y) = (1,2) � OK, (var x, int y) = ( 1,2) � OK
dictionary.TryGetValue (key, out var (x, y)) � �� OK, case var (x, y): break; � �� ��.
```
������� ����������: `(int a, int b) x (1,2); x.a++;`.

�������� ������� ����� �������� �� ����� (���� ������� ��� ����������) ��� ����� ����� �����, ����� ��� `Item1`, `Item2` � �. �.
