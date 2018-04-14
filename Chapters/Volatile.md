### Класс System.Threading.Volatile

В .NET Framework 4.5 появился небольшой, но полезный статический класс System.Threading.Volatile (обитает в mscorlib). В нём, по сути, всего два метода – Read и Write (оба перегружены по всевозможным типам аргументов), которые соответственно считывают и записывают некие значения потокобезопасным образом.

```csharp
public static class Volatile
{
  public static bool    Read ( ref bool location    );
  public static byte    Read ( ref byte location    );
  public static double  Read ( ref double location  );
  public static short   Read ( ref short location   );
  public static int     Read ( ref int location     );
  public static long    Read ( ref long location    );
  public static IntPtr  Read ( ref IntPtr location  );
  public static sbyte   Read ( ref sbyte location   );
  public static float   Read ( ref float location   );
  public static ushort  Read ( ref ushort location  );
  public static uint    Read ( ref uint location    );
  public static ulong   Read ( ref ulong location   );
  public static UIntPtr Read ( ref UIntPtr location );
  public static T Read<T>    ( ref T location       );
 
  public static void Write ( ref bool    location, bool    value );
  public static void Write ( ref byte    location, byte    value );
  public static void Write ( ref double  location, double  value );
  public static void Write ( ref short   location, short   value );
  public static void Write ( ref int     location, int     value );
  public static void Write ( ref long    location, long    value );
  public static void Write ( ref IntPtr  location, IntPtr  value );
  public static void Write ( ref sbyte   location, sbyte   value );
  public static void Write ( ref float   location, float   value );
  public static void Write ( ref ushort  location, ushort  value );
  public static void Write ( ref uint    location, uint    value );
  public static void Write ( ref ulong   location, ulong   value );
  public static void Write ( ref UIntPtr location, UIntPtr value );
  public static void Write<T> ( ref T location, T value );
}
```

На мультипроцессорной системе такая запись гарантирует что записанное в память значение немедленно становится видимым всем процессорам, а чтение соответственно получает самое последнее (актуальное) значение, неважно которым из процессоров оно было выполнено. Такие операции могут сбрасывать процессорные кэши и приводить к падению производительности приложения.

На однопроцессорной системе операции с помощью класса Volatile гарантируют, что считываемое или записываемое значение не окажется кешированным (например, в процессорном регистре), поэтому их можно использовать для синхронизации доступа к полям класса (например, из другого потока или вообще аппаратного считывания/записи).

Класс Volatile пригождается тогда, когда обычная блокировка (например, с помощью ключевого слова lock) недоступна.
