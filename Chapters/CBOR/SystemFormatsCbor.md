### Пакет System.Formats.Cbor

Microsoft реализовала поддержку формата CBOR в пакете [System.Formats.Cbor](https://www.nuget.org/packages/System.Formats.Cbor/).

Пакет поддерживает .NET 5/6, начиная с превью-версии 7 также поддерживается .NET Standard 2.0 и .NET Framework 4.6.2+.

Вот как им пользоваться:

```c#
using System;
using System.Diagnostics;
using System.Formats.Cbor;
using System.IO;
 
const string FileName = @"D:\Temp\First.cbor";
 
var first = new OurClass
{
    One = true,
    Two = 123_456,
    Three = "Hello"
};
first.SaveToFile (FileName);
Console.WriteLine (first);
  
var second = new OurClass();
second.RestoreFromFile (FileName);
Console.WriteLine (second);
  
Debug.Assert (first.One == second.One);
Debug.Assert (first.Two == second.Two);
Debug.Assert (first.Three == second.Three);
 
internal sealed class OurClass
{
    public bool One { get; set; }
    public int Two { get; set; }
    public string? Three { get; set; }
     
    public void RestoreFromFile (string fileName)
    {
        var encoded = File.ReadAllBytes (fileName);
        var reader = new CborReader (encoded);
        reader.ReadStartArray();
        One = reader.ReadBoolean();
        Two = reader.ReadInt32();
        Three = reader.ReadTextString();
        reader.ReadEndArray();
    }
      
    public void SaveToFile (string fileName)
    {
        var writer = new CborWriter();
        writer.WriteStartArray (3);
        writer.WriteBoolean (One);
        writer.WriteInt32 (Two);
        writer.WriteTextString (Three);
        writer.WriteEndArray();
  
        var encoded = writer.Encode();
        File.WriteAllBytes (fileName, encoded);
    }
  
    public override string ToString()
    {
        return $"{nameof (One)}: {One}, {nameof (Two)}: {Two}, {nameof (Three)}: {Three}";
    }
}
```
