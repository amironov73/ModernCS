﻿### Использование аннотаций JetBrains

Аннотации от JetBrains – простой и эффективный способ немного повысить качество своего кода за счёт специальной его разметки. На размеченном коде Resharper автоматически обнаруживает противоречия между реальным и задекларированным контрактом классов/методов. Вот простой пример:

```csharp
// Утверждаем, что функция не должна возвращать null
[NotNull]
public object Foo()
{
    // Много сложного кода
 
    // Противоречие с аннотацией: вернули null
    return null; // ReSharper выдаст предупреждение
}
```

Вот какие аннотации предусмотрены:

* `StringFormatMethod` помечает метод, аналогичный `string.Format` или `Console.WriteLine`. Аннотация помогает отловить несоответствие количества ожидаемых и переданных параметров;
* `CanBeNull` и `NotNull` утверждает, что в данном месте допустимо или недопустимо значение `null`;
* `ItemNotNull` – элементы массива/коллекции/списка не могут быть `null`;
* `ItemCanBeNull` – элементы массива/коллекции/списка могут быть `null`;
* `CannotApplyEqualityOperator` связана со сравнением с помощью оператора ==;
* `LocalizationRequired` – требуется локализация строк (перевод на другие языки);
* `PublicAPI` – помечает код как публичный API, который будет использоваться внешним кодом (чтобы Resharper не считал данный код неиспользуемым при отсутствии вызовов внутри проекта);
* `Pure` – помечает метод как чистый, т. е. не производящий глобальных изменений. Пример такого кода: `int Multiply ( int x, int y ) { return x * y; }`. Кроме прочего, Resharper будет предупреждать, если результаты вызова чистых функций отбрасываются (не используются);
* `MustUseReturnValue` – результат вычислений метода должен использоваться;
* `ContractAnnotation` используется для определение контракта функции. Например: `[ContractAnnotation("input:null => false")]` утверждает, что функция возвращает `false`, если ей передан `null`.

Полное описание аннотаций для C# см. https://www.jetbrains.com/help/resharper/10.0/Reference__Code_Annotation_Attributes.html

Аннотации в NuGet: https://www.nuget.org/packages/JetBrains.Annotations/

