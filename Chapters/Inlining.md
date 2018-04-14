### Инлайнинг методов в .NET

Для начала, какие методы не инлайнятся:

 * Помеченные как MethodImplOptions.NoInlining;
* Занимающие больше 32 байт MSIL;
* Виртуальные;
* Принимающие большую (массивную) структуру в качестве параметра;
* Методы классов MarshalByRef;
* Имеющие сложный граф управления (flowgraph);
* Некоторые другие, экзотические 🙂

Один и тот же метод, в зависимости от контекста его вызова, может быть как заинлайнен, так и нет. И, к сожалению, нет простой мнемоники, чтобы запомнить, когда метод будет инлайнен, а когда – нет. Есть только табличка с эмпирическими данными пытливых исследователей:

| Контекст вызова | x32 JIT | x64 JIT |
|-----------------|---------|---------|
| Вне циклов | Нет | Нет |
| Цикл for | Да | Да |
| Цикл while | Да | Да |
| Бесконечный цикл for (;;) | Нет | Нет |
| Цикл foreach | Да | Да |
| Метод ForEach | Нет | Нет |

Возможно, при обновлении .NET Framework всё поменяется, вплоть до полной противоположности.

Источник информации: http://blogs.microsoft.co.il/sasha/2012/01/20/aggressive-inlining-in-the-clr-45-jit/


Для совместимости с .NET 4.0 и ниже можно использовать такой трюк:
```csharp
using System.Runtime.CompilerServices;
 
class MyClass
{
 
// Для .NET 4.5+ необходимо определить символ FW45
 
#if FW45
 
        private const MethodImplOptions Aggressive
            = MethodImplOptions.AggressiveInlining;
 
#else
 
        private const MethodImplOptions Aggressive
            = (MethodImplOptions)0;
 
#endif
 
  // Будет компилироваться во всех версиях .NET
  [MethodImpl(Aggressive)]
  static int AddTwoNumbers ( int first, int second )
  {
    return first + second;
  }
}
```
