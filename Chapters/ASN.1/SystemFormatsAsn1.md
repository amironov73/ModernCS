# Пространство имен System.Formats.Asn1

В .NET 5 появилось замечательное пространство имен System.Formats.Asn1. В .NET 5/6 оно входит в стандартную поставку, для .NET Framework 4.6.2 его можно подключить [в виде NuGet-пакета](https://www.nuget.org/packages/System.Formats.Asn1).

Вот как его предполагается использовать:

```c#
using System.Diagnostics;
using System.Formats.Asn1;
 
const string FileName = @"D:\Temp\First.ber";
 
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
     
    private Asn1Tag OurTag => new Asn1Tag (TagClass.Application, 1);
 
    public void RestoreFromFile (string fileName)
    {
        var encoded = File.ReadAllBytes (fileName);
        var reader = new AsnReader (encoded, AsnEncodingRules.BER);
        var sequence = reader.ReadSequence (OurTag);
        reader.ThrowIfNotEmpty();
        One = sequence.ReadBoolean();
        if (!sequence.TryReadInt32 (out var two))
        {
            throw new Exception();
        }
        Two = two;
        Three = sequence.ReadCharacterString (UniversalTagNumber.UTF8String);
    }
     
    public void SaveToFile (string fileName)
    {
        var writer = new AsnWriter (AsnEncodingRules.BER);
        using (writer.PushSequence (OurTag))
        {
            writer.WriteBoolean (One);
            writer.WriteInteger (Two);
            writer.WriteCharacterString (UniversalTagNumber.UTF8String, Three);
        }
 
        var encoded = writer.Encode();
        File.WriteAllBytes (fileName, encoded);
    }
 
    public override string ToString()
    {
        return $"{nameof (One)}: {One}, {nameof (Two)}: {Two}, {nameof (Three)}: {Three}";
    }
}
```
