### Sprache 

[Sprache](https://github.com/sprache/Sprache) - простая легковесная библиотека для конструирования парсеров прямо в C#-коде. Ее нельзя сравнивать с промышленными решениями вроде ANTLR, ориентированными на "перемалывание" больших объемов информации. Однако, сделать с помощью Sprache небольшой калькулятор или простой DSL довольно легко, и это та ниша, в которой Sprache пользуется заслуженной популярностью.

В основу Sprache заложена идеология комбинирования парсеров. Мы стартуем с простейшего парсера, умеющего разбирать, например, только десятичные цифры, комбинируем его с другими такими же простейшими парсерами и получаем в итоге довольно удобный инструмент для разбора, например, DSL.

Важным достоинством Sprache является то, что с ее помощью парсер можно построить динамически, в зависимости от текущей конфигурации системы. С ANTLR такой фокус не пройдет.

Итак, вот простейший парсер:

```c#
// Считываем строку, состоящую из символов 'A'
var parseA = Parse.Char('A').AtLeastOnce();
```

Комбинирование парсеров организовано довольно удобно - с помощью LINQ:

```c#
Parser<string> identifier =
    from leading in Parse.WhiteSpace.Many()
    from first in Parse.Letter.Once().Text()
    from rest in Parse.LetterOrDigit.Many().Text()
    from trailing in Parse.WhiteSpace.Many()
    select first + rest;

var id = identifier.Parse(" abc123  ");

Assert.AreEqual("abc123", id);
```

Парсер `identifier` умеет довольно много: во-первых, он пропускает произвольное количество начальных пробельных символов, затем считывает идентификатор (в котором первый символ должен быть буквой, остальные - либо буквой либо цифрой), а затем поглощает произвольное количество конечных пробельных символов. В качестве результата он возвращает идентификатор в виде строки. На-все-про-все ушло 6 строк, включая "наведение красоты".

Подробное описание Sprache с кучей примеров: https://justinpealing.me.uk/post/2020-03-11-sprache1-chars/

Итак, начиная с азов, опишем основные возможности Sprache.

#### Char

Это самый простой парсер, какой только можно себе представить. Он умеет считывать ровно один символ, и ничего кроме.

```c#
Parser<char> Char (char c);
Parser<char> Char (Predicate<char> predicate, string description);
```

Метод `Parse` возвращает прочитанный символ либо выбрасывает исключение.

#### Chars

Умеет считывать любой из перечисленных символов:

```c#
Parser<char> Chars (params char[] c);
Parser<char> Chars (string c);
```
#### CharExcept

Умеет считывать любой символ, кроме указанных.

```c#
Parser<char> CharExcept (char c);
Parser<char> CharExcept (IEnumerable<char> c);
Parser<char> CharExcept (string c);
Parser<char> CharExcept (Predicate<char> predicate, string description);
```

Это были простые, я бы даже сказал тривиальные парсеры. Далее начинаются гораздо более интересные.

#### Whitespace

Считывает любой пробельный символ (в т. ч. перевод строки). По факту представляет собой конструкцию вроде

```c#
var whitespaceParser = Parser.Char (char.IsWhitespace, "whitespace");
```

```c#
Assert.Equal ('\t', Parse.WhiteSpace.Parse("\t"));
```

#### Digit

Считывает любой символ, для которого `char.IsDigit` возвращает `true`.

#### Numeric

Считывает любой символ, для которого `char.IsNumber` возвращает `true`.

#### Letter

Считывает любой символ, для которого `char.IsLetter` возвращает `true`.

#### LetterOrDigit

Считывает любой символ, для которого `char.IsLetterOrDigit` возвращает `true`.

#### Lower

Считывает любой символ, для которого `char.IsLower` возвращает `true`.

#### Upper

Считывает любой символ, для которого `char.IsUpper` возвращает `true`.

#### AnyChar

Считывает вообще любой символ.

#### String

Считывает указанную последовательность символов.

```c#
Parser<IEnumerable<char>> String (string s);
```

#### Text

Иметь дело с `IEnumerable` не всегда удобно, поэтому введен модификатор `Text`, который конвертирует последовательность символов в строку:

```c#
Parser<string> Text (this Parser<IEnumerable<char>> characters);
```

Пример использования:

```c#
Parser<string> keywordReturn = Parse.String ("return").Text();
Console.WriteLine (keywordReturn.Parse ("return"));
```

#### IgnoreCase

Игнорирование регистра символов:

```c#
Parser<char> IgnoreCase (char c);
Parser<IEnumerable<char>> IgnoreCase (string s);
```

#### Number

Считывание целого числа, состоящего только из цифр (знак "минус" не допускается).

```c#
Parser<string> Number;
```

#### Decimal

Считывание десятичного числа, состоящего только из цифр и разделителя целой и дробной части (в разных культурах может различаться). Знак "минус" не допускается.

```c#
Parser<string> Decimal;
```

#### DecimalInvariant

Считывание десятичного числа, состоящего только из цифр и разделителя целой и дробной части в инвариантной культуре - точки. Знак "минус" не допускается.

#### LineEnd

Считывание перевода строки (`"\n"` либо `"\r\n"`).

#### Token

Игнорировать возможные пробелы вокруг элемента. Пробелы оцениваются с помощью `char.IsWhitespace`.

```c#
Parser<T> Token<T> (this Parser<T> parser);
```

Пример:

```c#
Parser<int> expression =
      from left in Parse.Number.Token()
      from plus in Parse.Char ('+').Token()
      from right in Parse.Number.Token()
      select int.Parse (left) + int.Parse (right);
```

#### End

Пытается обнаружить конец входного потока. Если остались нераспознанные символы, выбрасывает исключение.

```c#
Parser<T> End<T> (this Parser<T> parser).
```

Пример:

```c#
Assert.Equal ("12", Parse.Number.End().Parse ("12"));

// unexpected '_'; expected end of input
Assert.Throws<ParseException> (() => Parse.Number.End().Parse ("12_"));
```

#### Many

Модификатор, указывающий, что предшествующий парсер может считывать от 0 до бесконечности повторений.

```c#
Parser<IEnumerable<T>> Many<T> (this Parser<T> parser);
```

Пример - парсер для закавыченной строки:

```c#
Parser<string> quotedString =
    from open in Parse.Char ('"')
    from value in Parse.CharExcept ('"').Many().Text()
    from close in Parse.Char ('"')
    select value;
```

#### XMany

Аналог `Many`, отличающийся тем, что все элементы должны быть считаны полностью (т. е. частичное считывание запрещено). Пример:

```c#
Parser<string> record =
    from lparem in Parse.Char('(')
    from name in Parse.Letter.Many().Text()
    from rparem in Parse.Char(')')
    select name;

string input = "(monday)(tuesday0(wednesday)(thursday)";

Assert.Equal(new[] { "monday" }, record.Many().Parse(input));

// unexpected '('; expected end of input
Assert.Throws<ParseException>(() => record.Many().End().Parse(input));

// unexpected '0'; expected )
Assert.Throws<ParseException>(() => record.XMany().Parse(input));
```

#### AtLeastOnce

Модификатор, указывающий, что предшествующий парсер должен считать не менее одного повторения.

```c#
Parser<IEnumerable<T>> AtLeastOnce<T> (this Parser<T> parser);
```

#### XAtLeastOnce

Аналогично `XMany`, но для `AtLeastOnce`.

#### Until

Разбирает последовательность до тех пор, пока не достигнут указанный терминатор.

```c#
Parser<IEnumerable<T>> Until<T, U> (this Parser<T> parser, Parser<U> until);
```

Пример - считывание комментария:

```c#
Parser<string> parser =
    from first in Parse.String ("/*")
    from comment in Parse.AnyChar.Until (Parse.String("*/")).Text()
    select comment;
```

#### Repeat

Модификатор, явно задающий необходимое количество повторений:

```c#
Parser<IEnumerable<T>> Repeat<T> (this Parser<T> parser, int count);
Parser<IEnumerable<T>> Repeat<T> (this Parser<T> parser, int? minimumCount, int? maximumCount);
```

Пример:

```c#
Parser<string> parser = Parse.Digit.Repeat (3, 6).Text();
```

#### Once

Модификатор, требующий, чтобы предшествующий парсер считал ровно одно повторение.

```c#
Parser<IEnumerable<T>> Once<T> (this Parser<T> parser);
```

Пример:

```c#
Parser<string> identifier = Parse.Identifier (Parse.Letter, Parse.LetterOrDigit);

Parser<IEnumerable<string>> memberAccess =
    from first in identifier.Once()
    from subs in Parse.Char ('.').Then (_ => identifier).Many()
    select first.Concat (subs);

Assert.Equal(new [] { "foo", "bar", "baz"}, memberAccess.Parse ("foo.bar.baz"));
```

#### Or  

Альтернативные варианты парсеров.

```c#
Parser<T> Or<T> (this Parser<T> first, Parser<T> second);
```

Пример:

```c#
Parser<string> keyword = Parse.String ("return")
    .Or (Parse.String ("function"))
    .Or (Parse.String ("switch"))
    .Or (Parse.String ("if"))
    .Text();
```

Порядок альтернатив важен -- сработает первая из подходящих альтернатив.

#### XOr

```c#
Parser<T> XOr<T> (this Parser<T> first, Parser<T> second);
```

Отличается от `Or` тем, что не запускает второй парсер, если первый сматчил хотя бы один символ. Пример:

```c#
var parser = Parse.String ("foo")
    .XOr (Parse.Identifier (Parse.Letter, Parse.LetterOrDigit));

//  unexpected 'a'; expected o
Assert.Throws<ParseException> (() => parser.Parse ("far"));
```

#### Select

Проецирует результат работы парсера на другой домен, выполняя заданную функцию над результатом парсинга:

```c#
Parser<U> Select<T, U> (this Parser<T> parser, Func<T, U> convert);
```

Пример - разбор целого числа:

```c#
Parser<int> number = Parse.Number.Select (int.Parse);
Assert.Equal (12, number.Parse ("12"));
```

#### Return

```c#
Parser<T> Return<T> (T value);
Parser<U> Return<T, U> (this Parser<T> parser, U value);
```

Аналогично `Select`, но игнорирует результат работы парсера. Пример:

```c#
Parser<OperatorType> add = Parse.String ("+").Return (OperatorType.Add);

Assert.Equal (OperatorType.Add, add.Parse ("+"));
```

#### Regex

Конструирует парсер из заданного регулярного выражения.

```c#
Parser<string> Regex (string pattern, string description = null);
Parser<string> Regex (Regex regex, string description = null);
```

Пример:

```c#
Parser<string> digits = Parse.Regex (@"\d(\d*)");

Assert.Equal("123", digits.Parse ("123d"));
Assert.Throws<ParseException> (() => digits.Parse ("d123"));
```

#### RegexMatch

Аналогично `Regex`, только возвращает объект типа `Match`:

```c#
Parser<Match> RegexMatch (string pattern, string description = null);
Parser<Match> RegexMatch (Regex regex, string description = null);
```

Пример:

```c#
Parser<Match> digits = Parse.RegexMatch (@"\d(\d*)");

Assert.Equal ("23", digits.Parse ("123d").Groups[1].Value);
```

#### DelimitedBy

Разбирает элементы, сматченные парсером `parser`, разделенные элементами, сматченными парсером `delimiter`:

```c#
Parser<IEnumerable<T>> DelimitedBy<T, U> (this Parser<T> parser, Parser<U> delimiter);
```

Пример - разбор списка типов:

```c#
Parser<string> typeReference = Parse.Identifier (Parse.Letter, Parse.LetterOrDigit);

Parser<IEnumerable<string>> typeParameters =
    from open in Parse.Char ('<')
    from elements in typeReference.DelimitedBy (Parse.Char (',').Token())
    from close in Parse.Char ('>')
    select elements;

Assert.Equal(new[] { "string", "int" }, typeParameters.Parse ("<string, int>"));

// unexpected ','; expected >
Assert.Throws<ParseException> (() => typeParameters.Parse ("<string,>"));

// unexpected '>'; expected letter
Assert.Throws<ParseException> (() => typeParameters.Parse ("<>"));
```

Обратите внимание, что в отсутствие разделителей, парсер `parser` должен сматчить хотя бы один элемент. Если нужно обрабатывать в том числе и пустые списки, применяйте `Optional`:

```c#
Parser<IEnumerable<string>> array =
    from open in Parse.Char('[')
    from elements in Parse.Number.DelimitedBy (Parse.Char (',').Token()).Optional()
    from trailing in Parse.Char (',').Token().Optional()
    from close in Parse.Char (']')
    select elements.GetOrElse (new string[] { });

Assert.Equal (new[] { "1", "2", "3" }, array.Parse ("[1, 2, 3]"));
Assert.Equal (new[] { "1", "2" }, array.Parse ("[1, 2, ]"));
Assert.Equal (new string[] { }, array.Parse ("[]"));
```

#### XDelimitedBy

Аналогично `DelimitedBy`, только `parser` должен сматчить все элементы полностью.

#### ChainOperator

Организует разбор цепочки вызовов лево-ассоциативного оператора `op` с операндами `operand`:

```c#
Parser<T> ChainOperator<T, TOp>(Parser<TOp> op, Parser<T> operand, Func<TOp, T, T, T> apply)
```

Пример - сложение целых чисел:

```c#
 var number = Parse.DecimalInvariant.Select (int.Parse);
var plus = Parse.Char ('+').Token();
var chain = Parse.ChainOperator (plus, number, (op, left, right) => left + right);

Console.WriteLine (chain.Parse ("1+2+3+4"));
```

#### XChainOperator

Аналогично `ChainOperator`, только все элементы должны быть полностью распознаны.


#### ChainRightOperator

Аналогично `ChainOperator`, только для право-ассоциативных операторов (например, возведение в степень).

#### XChainRightOperator

Аналогично `ChainRightOperator`, только все элементы должны быть полностью распознаны.

#### Contained

Разбираемый элемент расположен между двумя другими:

```c#
Parser<T> Contained<T, U, V> (this Parser<T> parser, Parser<U> open, Parser<V> close);
```

Пример:

```c#
Parser<string> parser = Parse.Letter.Many().Text().Contained (Parse.Char ('('), Parse.Char (')'));

Assert.Equal ("foo", parser.Parse ("(foo)"));
```

#### Identifier

Вспомогательный метод для формирования парсера идентификаторов:

```c#
Parser<string> Identifier (Parser<char> firstLetterParser, Parser<char> tailLetterParser);
```

Пример:

```c#
Parser<string> identifier = Parse.Identifier (Parse.Letter, Parse.LetterOrDigit);

Assert.Equal ("d1", identifier.Parse ("d1"));
```

#### LineTerminator

Конец строки либо конец текста.

```c#
Parser<string> parser = Parse.LineTerminator;

Assert.Equal ("", parser.Parse (""));
Assert.Equal ("\n", parser.Parse ("\n foo"));
Assert.Equal ("\r\n", parser.Parse ("\r\n foo"));
```

#### Optional

Модификатор, указывающий, что данный элемент опциональный, т. е. отсутствие совпадения не должно останавливать разбор текста.

```c#
Parser<IOption<T>> Optional<T> (this Parser<T> parser);
```

Пример - разбор ассемблерной инструкции с опциональной меткой:

```c#
Parser<string> identifier = Parse.Identifier (Parse.Letter, Parse.LetterOrDigit);

Parser<string> label =
    from labelName in identifier.Token()
    from colon in Parse.Char (':').Token()
    select labelName;

Parser<Tuple<string, string[]>> instruction =
    from instructionName in Parse.LetterOrDigit.Many().Text().Token()
    from operands in identifier.Token().XDelimitedBy (Parse.Char (','))
    select Tuple.Create (instructionName, operands.ToArray());

// Example of returning anonymous type from a sprache parser
var assemblyLine =
    from l in label.Optional()
    from i in instruction.Optional()
    select new { Label = l, Instruction = i };

Assert.Equal ("test", assemblyLine.Parse ("test: mov ax, bx").Label.Get());
Assert.False (assemblyLine.Parse ("mov ax, bx").Label.IsDefined);
```

#### XOptional

Аналогично `Optional`, но требует от предшествующего парсера точного совпадения.

#### CommentParser

`CommentParser` - вспомогательный класс для разбора комментариев. Содержит три члена:

* `SingleLineComment`,
* `MultiLineComment`,
* `AnyComment`.

Пример:

```c#
var comment = new CommentParser ("<!--", "-->", "\r\n");

Assert.Equal(" Commented text ", comment.AnyComment.Parse ("<!-- Commented text -->"));

// No single-line comment header was defined, so this throws an exception
Assert.Throws<ParseException> (() => comment.SingleLineComment);
```

Конструктор по умолчанию эквивалентен

```c#
new CommentParser ("//", "/*", "*/", "\n");
```

что позволяет разбирать комментарии в стиле C/C++.

#### Commented

Конструирует парсер, принимающий пробелы и комментарии и выдающий поток, в котором эти пробелы и комментарии убраны.

```c#
Parser<ICommented<T>> Commented<T> (this Parser<T> parser, IComment commentParser = null);
```

Пример:

```c#
var commented = Parse.Identifier (Parse.Letter, Parse.LetterOrDigit)
    .Token().Commented (new CommentParser {NewLine = Environment.NewLine});

var parsed = commented.Parse(@"
/* Test comment */
// B
foo // C
// D
");

Assert.Equal (new[] {" Test comment ", " B"}, parsed.LeadingComments);
Assert.Equal ("foo", parsed.Value);
Assert.Equal (new[] {" C"}, parsed.TrailingComments.ToArray());
```

#### Named

Присваивает имя парсеру:

```c#
Parser<T> Named<T> (this Parser<T> parser, string name);
```

Пример:

```c#
Parser<string> quotedText =
    (
        from open in Parse.Char ('"')
        from content in Parse.CharExcept ('"').Many().Text()
        from close in Parse.Char ('"')
        select content
    )
    .Named("quoted text");

// This throws:
//   unexpected 'f'; expected quoted text
// instead of:
//   unexpected 'f'; expected "
Assert.Throws<ParseException> (() => quotedText.Parse ("foo"));
```

#### Then

Запускает второй парсер, если первый успешно сматчил текст. Возвращается результат второго парсера:

```c#
Parser<U> Then<T, U> (this Parser<T> first, Func<T, Parser<U>> second);
```

Пример:

```c#
Parser<string> identifier = Parse.Identifier (Parse.Letter, Parse.LetterOrDigit);

Parser<string[]> memberAccess =
    from first in identifier.Once()
    from subs in Parse.Char ('.').Then (_ => identifier).Many()
    select first.Concat (subs).ToArray();

Assert.Equal(new [] { "foo", "bar", "baz"}, memberAccess.Parse ("foo.bar.baz"));
```

#### Not

Конструирует отрицание, т. е. парсер, выбрасывающий исключение, если предшествующий парсер успешно матчит текст.

```c#
Parser<object> Not<T> (this Parser<T> parser);
```

Пример:

```c#
Parser<string> Keyword (string text) =>
    Parse.IgnoreCase (text)
    .Then (n => Parse.Not (Parse.LetterOrDigit.Or (Parse.Char ('_')))).Return (text);

Parser<string> returnKeyword = Keyword ("return");

Assert.Equal ("return", returnKeyword.Parse ("return"));
Assert.Throws<ParseException> (() => returnKeyword.Parse ("return_"));
Assert.Throws<ParseException> (() => returnKeyword.Parse ("returna"));
```

#### Where

Дополнительная проверка: кроме парсера должно также сработать условие, заданное предикатом:

```c#
Parser<T> Where<T> (this Parser<T> parser, Func<T, bool> predicate);
```

Пример:

```c#
Parser<int> parser = Parse.Number.Select (int.Parse).Where (n => n >= 100 && n < 200);

Assert.Equal (151, parser.Parse ("151"));

// Unexpected 201.;
Assert.Throws<ParseException> (() => parser.Parse ("201"));
```

Можно также использовать в Linq-выражениях:

```c#
var keywords = new[]
{
    "return",
    "var",
    "function"
};

Parser<string> identifier =
    from id in Parse.Identifier (Parse.Letter, Parse.LetterOrDigit.Or (Parse.Char ('_')))
    where !keywords.Contains (id)
    select id;

// Unexpected return.;
Assert.Throws<ParseException> (() => identifier.Parse ("return"));
```

#### Concat

Склеивание двух потоков элементов:

```c#
Parser<IEnumerable<T>> Concat<T> (this Parser<IEnumerable<T>> first, Parser<IEnumerable<T>> second);
```

Пример:

```c#
Parser<IEnumerable<char>> IgnoreCase (string s)
{
    return s
        .Select (Parse.IgnoreCase)
        .Aggregate (Parse.Return (Enumerable.Empty<char>()),
            (a, p) => a.Concat (p.Once()))
        .Named (s);
}

Assert.Equal ("SPRACHE", IgnoreCase ("sprache").Parse ("SPRACHE"));
```

#### Span

Конструирует парсер, возвращающий `ITextSpan`.

```c#
Parser<ITextSpan<T>> Span<T> (this Parser<T> parser);
```

Пример:

```c#
Parser<string> sample =
    from a in Parse.Char ('a').Many().Text().Token()
    from b in Parse.Char ('b').Many().Text().Token().Span()
    where b.Start.Pos <= 10
    select a + b.Value;

Assert.Equal ("aaabbb", sample.Parse (" aaa bbb "));
Assert.Throws<ParseException>(() => sample.Parse(" aaaaaaa      bbbbbb "));
```

#### Positioned

Если нужна информация о позиции разбираемых символов в тексте, необходимо применить метод `Positioned`. Значения, возвращаемые парсером, должны реализовывать интерфейс `IPositionAware`:

```c#
enum BinaryOperator
{
    Add,
    Subtract
}

class Node : IPositionAware<Node>
{
    public int Length { get; protected set; }
    public Position StartPos { get; protected set; }

    public Node SetPos (Position startPos, int length)
    {
        Length = length;
        StartPos = startPos;
        return this;
    }
}

class Literal : Node, IPositionAware<Literal>
{
    public int Value { get; }

    public Literal (int value)
    {
        Value = value;
    }

    public static Literal Create (string value) => 
        new Literal (int.Parse (value));

    public new Literal SetPos (Position startPos, int length) => 
        (Literal) base.SetPos (startPos, length);
}

class BinaryExpression : Node, IPositionAware<BinaryExpression>
{
    public BinaryOperator Op { get; }
    public Node Left { get; }
    public Node Right { get; }

    public BinaryExpression (BinaryOperator op, Node left, Node right)
    {
        Op = op;
        Left = left;
        Right = right;
    }

    public static Node Create (BinaryOperator op, Node left, Node right) => 
        new BinaryExpression (op, left, right);

    public new BinaryExpression SetPos(Position startPos, int length) => 
        (BinaryExpression) base.SetPos (startPos, length);
}

static readonly Parser<BinaryOperator> Add = 
    Parse.Char ('+').Token().Return (BinaryOperator.Add);
    
static readonly Parser<BinaryOperator> Subtract = 
    Parse.Char ('-').Token().Return (BinaryOperator.Subtract);
static readonly Parser<Node> Number = 
    Parse.Number.Token().Select (Literal.Create).Positioned();

static readonly Parser<Node> Expr = Parse.ChainOperator(
    Add.Or (Subtract), Number, BinaryExpression.Create).Positioned();

[Fact]
public void PositionTest()
{
    var aaa = (BinaryExpression) Expr.Parse ("1 + 2");

    Assert.Equal (4, aaa.Right.StartPos.Pos);
}
```

#### Ref

Учитывая, что разрабатываемая с помощью Sprache грамматики могут (и, скорее всего, будут) содержать рекурсивные правила, необходимо как-то разрешать кольцевые ссылки. Рекомендуемый способ - метод `Ref`:

```c#
Parser<T> Ref<T> (Func<Parser<T>> reference);
```

Пример:

```c#
public static readonly Parser<float> Integer =
    Parse.Number.Token().Select (float.Parse);

public static readonly Parser<float> PrimaryExpression =
    Integer.Or (Parse.Ref (() => AdditiveExpression).Contained (Parse.Char('('), Parse.Char(')')));

public static readonly Parser<float> MultiplicativeExpression =
    Parse.ChainOperator (Parse.Char ('*'), PrimaryExpression, (c, left, right) => left * right);

public static readonly Parser<float> AdditiveExpression =
    Parse.ChainOperator (Parse.Char ('+'), MultiplicativeExpression, (c, left, right) => left + right);

[Fact]
public void bbb()
{
    Assert.Equal (2, AdditiveExpression.End().Parse ("1+1"));
}
```

#### Except

Парсинг продолжается только при условии, что парсер `except` не срабатывает:

```c#
Parser<T> Except<T, U> (this Parser<T> parser, Parser<U> except);
```

Пример:

```c#
const char Quote = '\'';
const char OpenCurly = '{';
const char CloseCurly = '}';
const char Comma = ',';
const char EqualSign = '=';

Parser<char> validChars = Parse.AnyChar.Except (Parse.Chars (Quote, OpenCurly, CloseCurly, EqualSign, Comma)
.Or (Parse.WhiteSpace));

Assert.Equal ('t', validChars.Parse("t"));
Assert.Throws<ParseException>(() => validChars.Parse (" "));
```

#### Preview

Указывает, что данный парсер является опциональным и не-потребляющим. Результирующий парсер выдает успех независимо от того, сработал ли Preview-парсер.

```c#
Parser<IOption<T>> Preview<T> (this Parser<T> parser);
```

Еще статьи:

* https://thomaslevesque.com/2017/02/23/easy-text-parsing-in-c-with-sprache/
* https://www.dabrowski.space/posts/simple-and-powerful-parsing-in-csharp-with-sprache-library/
* https://nblumhardt.com/2010/01/building-an-external-dsl-in-c/
* https://habr.com/ru/post/127642/
* https://habr.com/ru/post/228037/
