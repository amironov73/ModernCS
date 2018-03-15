### ������������������� �������� ����� C#

� C# ���� ������ ������������������� �������� �����: `__arglist`, `__reftype`, `__makeref` � `__refvalue`. ����� ��� �����?

`__reftype` ������ ������ �� ��� ����������� ��� ���������. ������ ����� ��� System.TypedReference � ��� ������ ������������ ���������, ��� ������� ���������� ������������ ���� �������� ����� ToString(). �������, ���������� ������ ���� �� ���� �� �����, � �������� � �������� ��������� ������� ��������� ����� `__reftype`, ������� ������� ����� `System.Type`. ��� � ������� �������� ��������� ����� `__refvalue` ���������� �� ��������, ����������� � �������������� ������. �������, �������� �������� ����� ��������� ���������� � ����� ������������ ���������� ���������� ������������� ����.

����� ������ ���� � ���� ������ ����:
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
� �����, ������ ������ �������� �� �������������������� ��������� ������� �� ����������. ��, ��� ��� �� ����������� �� ��� ���, ����� ���������� �� ������������ ������.
