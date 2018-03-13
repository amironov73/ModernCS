### ������������ ���� � async

����� � ������������� async ����� ����������:

* void (������������� ������ ��� ������������ �������);
* Task, ���� �� ��������� ��������� ���������� ������;
* Task&lt;T&gt;, ���� ��������� ��������� (��������, ����� ����� ��� ������);
* ������� � C# 7, ����� ���������� ������ ������ ������, � �������� ���� ����� GetAwaiter, ������� � ���� ������� ���������� ������, ����������� ���������
[System.Runtime.CompilerServices.ICriticalNotifyCompletion](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.icriticalnotifycompletion).

������� �������� Task&lt;T&gt; ��������� ������� �������� �����������:
```csharp
public async Task<string> GetGreeting()
{
    return "Hello";
}
```
�. �. �� ����������� �������������� ��������� ������ ���� Task&gt;T&gt; � ����������� ��� �������������� ��������.

��������� ���������������� ��������� async-�������, �����
���������� ����������� ����, ������� ��������� ����� GetAwaiter.
```csharp
public async ValueTask<int> GetDiceRoll()
{
    return 3; // ��������� �����
}
```
