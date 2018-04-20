### Генерация IL с помощью Sigil

По материалам статьи http://www.infoq.com/news/2016/01/sigil-intermediate-language

Sigil – довольно простая в освоении, при том очень полезная библиотека для генерации динамических методов. GitHub: https://github.com/kevin-montrose/Sigil, NuGet: https://www.nuget.org/packages/Sigil/.

Лучше тысячи слов скажет один простой листинг:

```csharp
using System;
using Sigil;
 
class Program
{
    static void Main()
    {
        var emiter = Emit<Func<int, int, int>>.NewDynamicMethod("Adder");
        emiter.LoadArgument(0);
        emiter.LoadArgument(1);
        emiter.Add();
        emiter.Return();
        Func<int, int, int> adder = emiter.CreateDelegate();
 
        int result = adder(100, 23);
        Console.WriteLine(result);
    }
}
```

Очень просто и наглядно, не правда ли?

Рассмотрим другие возможности библиотеки.

Локальные переменные. Их можно ввести с помощью DeclareLocal<T>. Локальные переменные имеют ограниченную область видимости, управляемую с помощью using. Пример:

```csharp
var e1 = Emit<Func<int>>.NewDynamicMethod();
 
using (var a = e1.DeclareLocal<int>("a"))
{
    e1.LoadLocal(a);
    e1.LoadConstant(1);
    e1.Add();
}
 
// reuses the definition of "a", since it's available and the types match
using (var b = e1.DeclareLocal<int>("b"))
{
    e1.StoreLocal(b);
    e1.LoadLocal(b);
    e1.Return();
}
```

Ветвление:

```csharp
var emiter = Emit<Func<int>>.NewDynamicMethod("Unconditional");
 
var label1 = emiter.DefineLabel("label1");
var label2 = emiter.DefineLabel("label2");
var label3 = emiter.DefineLabel("label3");
 
emiter.LoadConstant(1);
emiter.Branch(label1);
 
emiter.MarkLabel(label2);
emiter.LoadConstant(2);
emiter.Branch(label3);
 
emiter.MarkLabel(label1);
emiter.Branch(label2);
 
emiter.MarkLabel(label3); // the top of the stack is the first element
emiter.Add();
emiter.Return();
 
var d = emiter.CreateDelegate();
d();  // returns 3
```

Работа с исключениями. В Sigil есть методы Sigil BeginExceptionBlock, EndExceptionBlock, BeginCatchBlock, BeginCatchAllBlock, EndCatchBlock, BeginFinallyBlock и EndFinallyBlock.

```csharp
MethodInfo mayFail = ...;
MethodInfo alwaysCall = ...;
var emiter = Emit<Func<string, bool>>.NewDynamicMethod("TryCatchFinally");
 
// names are purely for ease of debugging, and are optional
var inputIsNull = emiter.DefineLabel("ifNull");
var tryCall = emiter.DefineLabel("tryCall");
 
emiter.LoadArgument(0);
emiter.LoadNull();
emiter.BranchIfEqual(inputIsNull);
emiter.Branch(tryCall);
 
emiter.MarkLabel(inputIsNull, Type.EmptyTypes);
emiter.LoadConstant(false);
emiter.Return();
 
emiter.MarkLabel(tryCall);
 
var succeeded = emiter.DeclareLocal<bool>("succeeded");
var t = emiter.BeginExceptionBlock();
emiter.Call(mayFail);
emiter.LoadConstant(true);
emiter.StoreLocal(succeeded);
 
var c = emiter.BeginCatchAllBlock(t);
emiter.Pop();   // Remove exception
emiter.LoadConstant(false);
emiter.StoreLocal(succeeded);
emiter.EndCatchBlock(c);
 
var f = emiter.BeginFinallyBlock(t);
emiter.Call(alwaysCall);
emiter.EndFinallyBlock(f);
 
emiter.EndExceptionBlock(t);
 
emiter.LoadLocal(succeeded);
emiter.Return();
 
var del = emiter.CreateDelegate();
```

В результате создаётся следующий динамический метод:

```csharp
Func<string, bool> del = s =>
   {
      if(s == null) return false;
 
      bool succeeded;
      try
      {
         mayFail();
         succeeded = true;
      }
      catch
      {
         succeeded = false;
      }
      finally
      {
         alwaysCall();
      }
 
      return succeeded;
   };
```

Sigil пытается сделать отладку динамических методов менее болезненной. В частности, при помощи метода Instructions можно получить строковое представление IL-кода.

Имеется восхитительный метод Disassemble:

```csharp
Func<string, int> del =
    str =>
    {
        var i = int.Parse(str);
        return (int)Math.Pow(2, i);
    };
 
var ops = Sigil.Disassembler<Func<string, int>>
    .Disassemble(del);
 
var calls = ops.Where(o => o.IsOpCode && new[] 
  { 
    OpCodes.Call, 
    OpCodes.Callvirt 
  }
  .Contains(o.OpCode)).ToList();
var methods = calls.Select(c => c.Parameters.ElementAt(0))
  .Cast<MethodInfo>().ToList();
```
