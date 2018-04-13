### BigInteger

Теперь в .NET есть не только строки неограниченной длины, но и целые числа неограниченной длины. Встречайте – структура System.Numerics.BigInteger, которая при (не)правильном подходе может вызвать OutOfMemoryException в процессе вычислений.  🙂

BigInteger – это структура, так что семантика присваивания у неё такая же, как и у обычных целых чисел в .NET, и это хорошо, т. к. не возникает неожиданностей при целочисленных вычислениях, неважно с какими целыми они производятся.

Для начала, у BigInteger масса конструкторов на все случаи жизни:

* BigInteger (Byte[]);
* BigInteger (Decimal);
* BigInteger (Double);
* BigInteger (Int32);
* BigInteger (Int64);
* BigInteger (Single);
* BigInteger (UInt32);
* BigInteger (UInt64);

так что соорудить «большое целое» можно практически из чего угодно, было бы желание. Тем более, что для него определён ворох неявных (implicit) операторов преобразования, так что инициализация переменной типа BigInteger и последующие манипуляции с нею выглядят довольно интуитивно:
```csharp
BigInteger two = 2, three = 3;
BigInteger sum = two + three;
Console.WriteLine("{0} + {1} = {2}", two, three, sum );
```
Есть несколько перегрузок методов Parse и TryParse, позволяющие тонко настроить преобразование строки в «длинное целое»:

* Parse (String);
* Parse (String, NumberStyles);
* Parse (String, IFormatProvider);
* Parse (String, NumberStyles, IFormatProvider);

```csharp
string positiveString = "91389681247993671255432112000000";
string negativeString = "-90315837410896312071002088037140000";
BigInteger posBigInt 
    = BigInteger.Parse (positiveString); // если что, будет исключение
BigInteger negBigInt = 0;
if (!BigInteger.TryParse (negativeString, out negBitInt))
{
  ConsoleWriteLine ("Что-то не так с числом {0}", negativeString );
}
```

Для озабоченных быстродействием заведены специальные константы с говорящими именами:

* One;
* MinusOne;
* Zero;

и специальные методы (без которых можно было бы обойтись, если бы не забота о быстродействии), назначение которых понятно из названия:

* IsEven;
* IsOne;
* IsPowerOfTwo;
* IsZero;
* Sign – знак: -1, 0 или 1.

Само собой, определены операторы сложения, вычитания, умножения и деления, битовые операции «И», «ИЛИ», «ИСКЛЮЧАЮЩЕЕ ИЛИ», а также функции

* Abs – модуль (абсолютная величина) «длинного целого»;
* Compare – сравнивает два «длинных целых»;
* DivRem – возвращает частное и остаток от деления двух «длинных целых»;
* GreatestCommonDivisor – наибольший общий делитель;
* Log(BigInteger) – логарифм по основанию e;
* Log(BigInteger, Double) – логарифм по произвольному основанию;
* Log10(BigInteger) – логарифм по основанию 10;
* Max – возвращает большее из двух «длинных целых»;
* Min – возвращает меньшее из двух «длинных целых»;
* ModPow – деление по модулю числа, возведённого в указанную степень;
* Negate – меняет знак числа;
* Pow – возведение в указанную степень;
* Remainder – остаток от деления.

Определены операторы сравнения ==, !=, >, >=, <, <=, так что можно писать интуитивно понятный код:
```csharp
BigInteger ten = 10, five = 5;
if (five > ten)
{
   Console.WriteLine ("Что-то не так с этим миром!");
}
```
Конечно же, не обошлось без методов экспорта и форматирования строк:

* ToByteArray – возвращает внутреннее представление «длинного целого», которое можно записать, например, в файл;
* ToString();
* ToString(IFormatProvider);
* ToString(String);
*ToString(String, IFormatProvider).

Для «длинных целых» работают все те же форматы, что и для обычных целых в .NET:
```csharp
bigNumber.ToString ("X"); // шестнадцатеричное представление
bigNumber.ToString ("00000000000"); // явно заданный формат
```
«Длинные целые» можно сортировать стандартными методами, т. к. они реализуют интерфейс IComparable:
```csharp
BigInteger[] array = new BigInteger[3];
array[0] = 5;
array[1] = 0;
array[2] = 10;
Array.Sort(array);
```
Замечание по реализации: внутри «длинные целые» хранятся в том же формате, что и обычные целые (Int32, Int64), т. е. вполне допустимо следующие преобразования:
```csharp
int smallNumber = 100500;
BigInteger bigNumber 
    = new BigInteger (BitConverter.GetBytes (smallNumber));
 
...
 
// Осторожно! Значение bigNumber не должно превышать Int32.MaxValue
// или быть меньше Int32.MinValue, иначе возможно усечение!
byte[] bytes = bigNumber.ToByteArray();
Array.Resize ( ref bytes, 4 );
int smallAgain = BitConverter.ToInt32(bytes,0);
```
Отрицательные числа хранятся, как обычно у Intel, в инверсном представлении с дополнением до двойки (two’s complement representation).
