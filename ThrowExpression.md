### Throw-���������

� C#, ������� � ������� ������, ����� ������������ �������� throw �:

* ��������� ���������;
* null-coalescing ���������;
* ������-���������;
* ���� ���������.
#### ��������� ��������
```csharp
var customerInfo = HasPermission()
    ? ReadCustomer()
    : throw new SecurityException("permission denied");
```
#### Null-coalescing ��������
```csharp
var age = user.Age ?? 
    throw new InvalidOperationException("user age must be initialized");
```
#### ��������� � ���� �������
```csharp
class ReadStream : Stream
{
  ...
  override void Write(byte[] buffer, int offset, int count) =>
      throw new NotSupportedException("read only");
  ...
}
```
#### LINQ
```csharp
var awardRecipients = customers.Where(c => c.ShouldReceiveAward)
    .Select(c => c.Status == Status.None
       ? throw new InvalidDataException($"Customer {c.Id} has no status")
       : c)
    .ToList();
 
var customer = dbContext.Orders.Where(o => o.Address == address)
    .Select(o => o.Customer)
    .Distinct()
    .SingleOrDefault()
    ?? throw new InvalidDataException("Could not find an order");
```
���, ��������!
