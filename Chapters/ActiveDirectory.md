### Аутентификация с Active Directory

Проверить логин-пароль в Active Directory довольно просто. Надо добавить ссылку на сборку System.DirectoryServices.AccountManagement.dll и позвать на помощь класс PrincipalContext:

```csharp
using System;
using System.DirectoryServices.AccountManagement;
 
class Program
{
    static void Main()
    {
        string domain = Console.ReadLine();
        string userName = Console.ReadLine();
        string password = Console.ReadLine();
 
        using (PrincipalContext context
            = new PrincipalContext(ContextType.Domain, domain))
        {
            bool result = context.ValidateCredentials(userName, password);
            Console.WriteLine(result);
        }
    }
}
```
