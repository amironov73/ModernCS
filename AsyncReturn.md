### ������������ ���� � async

����� � ������������� async ����� ����������:

* `void` (������������� ������ ��� ������������ �������);
* `Task`, ���� �� ��������� ��������� ���������� ������;
* `Task<T>`, ���� ��������� ��������� (��������, ����� ����� ��� ������);
* ������� � C# 7, ����� ���������� ������ ������ ������, � �������� ���� ��������� ����� `GetAwaiter`, ������� � ���� ������� ���������� ������, ����������� ���������
[System.Runtime.CompilerServices.ICriticalNotifyCompletion](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.icriticalnotifycompletion).

������� �������� `Task<T>` ��������� ������� �������� �����������:
```csharp
public async Task<string> GetGreeting()
{
    return "Hello";
}
```
�. �. �� ����������� �������������� ��������� ������ ���� `Task<T>` � ����������� ��� �������������� ��������.

��������� ���������������� ��������� async-�������, �����
���������� ����������� ����, ������� ��������� ����� `GetAwaiter`.
```csharp
public async ValueTask<int> GetDiceRoll()
{
    return 3; // ��������� �����
}
```
