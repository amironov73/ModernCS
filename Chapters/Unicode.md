### Unicode и его ловушки

Программирование на C приучило нас к тому, что вызов `isalpha(c)` по факту означает `((c >= ‘A’) && (c <= ‘Z’)) || ((c >= ‘a’) && (c <= ‘z’))`. Все мы знали о замечательной функции `setlocale`, и использовали её так:
```c
setlocale(LC_ALL, "");
```
что в конечном итоге приводило к вышеописанному: буквами считаются только A-Z и a-z. Конечно же, существует ещё и кириллица, но она традиционно оказывается за бортом.

В .NET не так. Метод `Char.IsLetter` не ориентируется на текущую локаль, т. к. используется Unicode. В результате охваченными оказываются все символы, а не только избранные. Вот что говорится в MSDN о `Char.IsLetter`:

>This method determines whether a Char is a member of any category of Unicode letter. Unicode letters include the following:
>* Uppercase letters, such as U+0041 (LATIN CAPITAL LETTER A) through U+005A (LATIN CAPITAL LETTER Z), or U+0400 (CYRILLIC CAPITAL LETTER IE WITH GRAVE) through U+042F (CYRILLIC CAPITAL LETTER YA). These characters are members of theUnicodeCategory.UppercaseLetter category.
>* Lowercase letters, such as U+0061 (LATIN SMALL LETTER A) through U+007A (LATIN SMALL LETTER Z), or U+03AC (GREEK SMALL LETTER ALPHA WITH TONOS) through U+03CE (GREEK SMALL LETTER OMEGA WITH TONOS). These characters are members of theUnicodeCategory.LowercaseLetter category.
>* Title case letters, such as U+01C5 (LATIN CAPITAL LETTER D WITH SMALL LETTER Z WITH CARON) or U+1FFC (GREEK CAPITAL LETTER OMEGA WITH PROSGEGRAMMENI). These characters are members of the UnicodeCategory.TitlecaseLetter category.
>* Modifiers, such as U+02B0 (MODIFIER LETTER SMALL H) through U+02C1 (MODIFIER LETTER REVERSED GLOTTAL STOP), or U+1D2C (MODIFIER LETTER CAPITAL A) through U+1D61 (MODIFIER LETTER SMALL CHI). These characters are members of theUnicodeCategory.ModifierLetter category.
>* Other letters, such as U+05D0 (HEBREW LETTER ALEF) through U+05EA (HEBREW LETTER TAV), U+0621 (ARABIC LETTER HAMZA) through U+063A (ARABIC LETTER GHAIN), or U+4E00 (<CJK Ideograph, First>) through U+9FC3 (<CJK Ideograph, Last>). These characters are members of the UnicodeCategory.OtherLetter category.

Как видим, букв оказывается гораздо больше, чем мы ожидали.

Кроме `Char.IsLetter`, сюрприз нам преподносит и `Char.IsDigit`:
```csharp
Console.WriteLine (Char.IsDigit('0'));
Console.WriteLine (Char.IsDigit('9'));
Console.WriteLine (Char.IsDigit('\x0661')); // ١
Console.WriteLine (Char.IsDigit('\x0662')); // ٢
Console.WriteLine (Char.IsDigit('\x0663')); // ٣
```
Во всех случаях будет напечатано «True». Вот так-то! Цифр в природе существует намного больше, чем мы подозреваем. Само собой, «помесь» двух перечисленных методов, `Char.IsLetterOrDigit`, тоже поддерживает расширенный по сравнению с нашими ожиданиями диапазон символов.

Чтобы не впутывать в свои программы логику обработки неожиданных цифр и букв, можно использовать следующие методы:
```csharp
static bool IsArabicDigit (char c)
{
    return (c >= '0') && (c <= '9');
}
 
static bool IsLatinLetter (char c)
{
    return ((c >= 'A') && (c <= 'Z')) || ((c >= 'a') && (c <= 'z'));
}
 
static bool IsRussianLetter (char c)
{
    return ((c >= 'А') && (c <= 'я'))
        || (c == 'Ё') || (c == 'ё');
}
```

