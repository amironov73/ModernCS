### �� ��� ��������������� await

Sinix �����:

> ��� ����
```csharp
Report("Before await");
var someValue = await GetSomeValueAsync();
Report("After await: {0}", someValue);
```
> ���������� ���������� � ����� �����
```csharp
Report("Before await");
var awaiter = GetSomeValueAsync().GetAwaiter();
awaiter.OnCompleted
  (
    () => Report("After await: {0}", awaiter.GetResult()
  );
```
