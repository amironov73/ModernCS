### Castle.DynamicProxy

[Castle Project](http://www.castleproject.org/) – открытый проект, давший миру такие замечательные инструменты как ActiveRecord, DynamicProxy, MicroKernel, MonoRail и Windsor. Сегодня мы поговорим о DynamicProxy – замечательном инструменте, применяемом во множестве проектов, в том числе Moq.

Если кратко, то DynamicProxy представляет собой лёгкий генератор прокси-объектов для интерфейсов и классов .NET (lightweight proxy generator for interfaces and concrete classes). Прокси-объекты генерируются во время исполнения программы (отсюда название). Проще всего продемонстрировать мощь DynamicProxy на конкретном примере. Допустим, у нас есть (возможно, не нами написанный) класс, выполняющий некие полезные действия. Мы хотели бы добавить ко всем его методам обработку транзакций, но – вот беда! – у нас нет исходных текстов этого класса. Конечно же, можно вручную написать обёртку для каждого метода, но можно поступить проще. Сначала мы создаём перехватчик вызовов методов:

```csharp
public class TransactionHandler : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        using (var transaction = new TransactionScope())
        {
            invocation.Proceed();
            transaction.Complete();                
        }
    }
}
```

Потом подключаем перехватчик к нужному объекту:

```csharp
var generator = new ProxyGenerator();
var ourObject = new OurClass();
var wrappedObject = generator.CreateInterfaceProxyWithTarget
    (
        foo,
        typeof(TransactionHandler)
    );
wrappedObject.SomeMethod(); // вызов метода будет обёрнут в транзакцию
```

Вообще, перехватчик в общем случае выглядит так (пример от автора Castle):

```csharp
public class MyInterceptor : IInterceptor
{
    public object Intercept(IInvocation invocation, params object[] args)
    {
        DoSomeWorkBefore(invocation, args);
 
        object retValue = invocation.Proceed( args );
 
        DoSomeWorkAfter(invocation, retValue, args);
 
        return retValue;
    }
}
```

Интерфейс IInvocation:

```csharp
public interface IInvocation
{
    object Proxy { get; }
 
    object InvocationTarget { get; set; }
 
    MethodInfo Method { get; }
 
    object Proceed(params object[] args);
}
```

Ещё несколько примеров от автора Castle. Первый:

```csharp
public interface IMyInterface
{
    int Calc(int x, int y);
    int Calc(int x, int y, int z, Single k);
}
 
public class MyInterfaceImpl : IMyInterface
{
    public virtual int Calc(int x, int y)
    {
        return x + y;
    }
 
    public virtual int Calc(int x, int y, int z, Single k)
    {
        return x + y + z + (int)k;
    }
}
 
ProxyGenerator generator = new ProxyGenerator();
 
IMyInterface proxy = (IMyInterface) generator.CreateProxy
    ( 
        typeof(IMyInterface),
        new StandardInterceptor(),
        new MyInterfaceImpl()
    );
```

Второй:

```csharp
ProxyGenerator generator = new ProxyGenerator();
 
Hashtable proxy = (Hashtable) generator.CreateClassProxy
    ( 
        typeof(Hashtable),
        new HashtableInterceptor()
    );
 
object value = proxy["key"]; // == "default"
 
public class HashtableInterceptor
    : StandardInterceptor
{
    public override object Intercept
        (
            IInvocation invocation,
            params object[] args
        )
    {
        if (invocation.Method.Name.Equals("get_Item"))
        {
            object item = base.Intercept(invocation, args);
            return (item == null) ? "default" : item;
        }
        return base.Intercept(invocation, args);
    }
}
```

Сгенерированный прокси-объект выглядит примерно так (псевдо-C#):

```csharp
public class ProxyGenerated : YourClass
{
  // Exposes all constructors
   
  // Overrides all methods
   
  public override int DoSomething(int x)
  {
    MethodInfo m = ldtoken currentMethod;
    IInvocation invocation = ObtainInvocationFor
        (
            deletegateForDoSomething, 
            callableForDoSomething, 
            m
        );
    return (int) _interceptor.Intercept(invocation, x);
  }
 
  private int BaseCallDoSomething(int x)
  {
    return base.DoSomething(x);
  }
}
```

GitHub: https://github.com/castleproject/Core, NuGet: https://www.nuget.org/packages/Castle.Core. Поддерживает .NET 3.5, 4.0 и 4.5, а также Silverlight 4 и 5.

Полный пример:

```csharp
using System;
 
using Castle.DynamicProxy;
 
public class Calculator
{
    public virtual int Add(int left, int right)
    {
        return left + right;
    }
 
    public virtual int Sub(int left, int right)
    {
        return left - right;
    }
}
 
class LoggingInterceptor
    : IInterceptor
{
    public void Intercept
        (
            IInvocation invocation
        )
    {
        Console.Write
            (
                "Logging interceptor: Calling method {0}:",
                invocation.Method.Name
            );
        foreach (object argument in invocation.Arguments)
        {
            Console.Write(" {0}", argument);
        }
        Console.WriteLine();
 
        invocation.Proceed();
 
        Console.WriteLine
            (
                "Logging interceptor: Result is {0}",
                invocation.ReturnValue
            );
    }
}
 
class ExceptionInterceptor
    : IInterceptor
{
    public void Intercept
        (
            IInvocation invocation
        )
    {
        Console.Write
            (
                "Exception interceptor: Calling method {0}:",
                invocation.Method.Name
            );
        foreach (object argument in invocation.Arguments)
        {
            Console.Write(" {0}", argument);
        }
        Console.WriteLine();
 
        try
        {
            invocation.Proceed();
        }
        catch (Exception ex)
        {
            Console.WriteLine
                (
                    "Exception occurs: {0}",
                    ex
                );
        }
 
        Console.WriteLine
            (
                "Exception interceptor: Result is {0}",
                invocation.ReturnValue
            );
    }
}
 
class Program
{
    static void Main()
    {
        ProxyGenerator generator = new ProxyGenerator();
        Calculator proxy = generator.CreateClassProxy<Calculator>
            (
                new LoggingInterceptor(),
                new ExceptionInterceptor()
            );
        proxy.Add(1, 2);
        proxy.Sub(2, 1);
 
        Console.ReadLine();
    }
}
```

