### DynamicMethod

```c#
using System;
using System.Reflection;
using System.Reflection.Emit;

var dynamicAdd = new DynamicMethod
    (
        "AddTwoIntegers",
        typeof(int),
        new[] { typeof(int), typeof(int) },
        typeof(Helper).Module
    );
var il = dynamicAdd.GetILGenerator(256);
il.Emit(OpCodes.Ldarg_0);
il.Emit(OpCodes.Ldarg_1);
il.Emit(OpCodes.Add);
il.Emit(OpCodes.Ret);
dynamicAdd.DefineParameter(1, ParameterAttributes.In, "left");
dynamicAdd.DefineParameter(2, ParameterAttributes.In, "right");

var plus = dynamicAdd.CreateDelegate<Helper.PlusDelegate>();
var left = 123;
var right = 456;
var summa = plus(left, right);
Console.WriteLine($"{left} + {right} = {summa}");

//==============================================================================

var dynamicGreeter = new DynamicMethod
    (
        "GreetSomebody",
        typeof(void),
        new[] { typeof(string) },
        typeof(Helper).Module
    );
var writeString = typeof(Console).GetMethod
    (
        "WriteLine",
        new [] { typeof(string), typeof(string) }
    );
il = dynamicGreeter.GetILGenerator(256);
il.Emit(OpCodes.Ldstr, "Hello, {0}!");
il.Emit(OpCodes.Ldarg_0);
il.EmitCall(OpCodes.Call, writeString!, null);
il.Emit(OpCodes.Ret);

var greet = dynamicGreeter.CreateDelegate<Helper.HelloDelegate>();
greet("World");

//==============================================================================

internal static class Helper
{
    public delegate int PlusDelegate(int left, int right);

    public delegate void HelloDelegate(string name);
}
```
