### WCF: исключения

К сожалению ли, к счастью ли, в WCF просто так швыряться исключениями нельзя, т. к. не всякое исключение, возникшее на сервере, будет передано клиенту. Бросать можно только FaultException<T>.

Вернёмся к ранее начатому проекту и добавим в сборку CommonClasses следующий класс:

```csharp
using System.Runtime.Serialization;
 
namespace CommonClasses
{
    [DataContract]
    public sealed class ReaderFault
    {
        [DataMember]
        public string CustomError;
 
        public ReaderFault()
        {
        }
 
        public ReaderFault(string customError)
        {
            CustomError = customError;
        }
    }
}
```

Придётся немного переделать контракт – добавить в него указание на тип, используемый для передачи информации об исключении.

```csharp
[ServiceContract]
public interface IReaderValidator
{
    [OperationContract]
    [FaultContract(typeof(ReaderFault))]
    bool ValidateReader(string readerName);
}
```

В реализацию метода ValidateReader добавляем следующий код:

```csharp
if (string.IsNullOrEmpty(readerName))
{
     throw new FaultException<ReaderFault>
        (
            new ReaderFault("Empty reader name")
        );
}
```

Осталось только обернуть удалённый вызов методов в try-catch:

```csharp
try
{
    IReaderValidator validator = factory.CreateChannel();
    validator.ValidateReader (null);
}
catch (FaultException<ReaderFault> readerFault)
{
    Console.WriteLine(readerFault.Detail.CustomError);
}
```
