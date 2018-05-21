### Microsoft.Recognizers.Text

Библиотека Microsoft.Recognizers.Text обеспечивает надежное распознавание в тексте таких объектов, как числа, единицы и дата/время. Поддерживается несколько языков. Полная поддержка китайского, английского, французского, испанского и португальского языков. Частичная поддержка немецкого и японского языков. Ведутся работы над поддержкой других языков.

GitHub: https://github.com/Microsoft/Recognizers-Text, NuGet: https://www.nuget.org/packages/Microsoft.Recognizers.Text/ (несколько пакетов с префиксом Microsoft.Recognizers.Text)

Библиотека поддерживает .NET Framework 4.5+ и .NET Standard 2.0.

Пример программы:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.NumberWithUnit;
using Microsoft.Recognizers.Text.Choice;
using Microsoft.Recognizers.Text.Sequence;

using Newtonsoft.Json;

class Program
{
    static void Main()
    {
        ProcessQuery("I have two apples");
        ProcessQuery("I live at eleventh floor");
        ProcessQuery("One hundred percents");
        ProcessQuery("Between 2 and 5");
        ProcessQuery("After ninety five years of age, perspectives change");
        ProcessQuery("Interest expense in the 1988 third quarter was $ 75.3 million");
        ProcessQuery("The six-mile trip to my airport hotel that had taken 20 minutes earlier in the day took more than three hours");
        ProcessQuery("Set the temperature to 30 degrees celsius");
        ProcessQuery("I'll go back 8pm today");
        ProcessQuery("My phone number is ( 19 ) 38294427");
        ProcessQuery("My Ip is 8.8.8.8");
        ProcessQuery("Like @Cicero");
        ProcessQuery("Done task #123");
        ProcessQuery("My e-mail is a@b.com");
        ProcessQuery("Search with bing.com");
        ProcessQuery("yup, I need that");
    }

    static void ProcessQuery(string text)
    {
        // Use English for the Recognizers culture
        string culture = Culture.English;

        Console.WriteLine(text);

        var results = MergeResults
        (
            NumberRecognizer.RecognizeNumber(text, culture),
            NumberRecognizer.RecognizeOrdinal(text, culture),
            NumberRecognizer.RecognizePercentage(text, culture),
            NumberRecognizer.RecognizeNumberRange(text, culture),
            NumberWithUnitRecognizer.RecognizeAge(text, culture),
            NumberWithUnitRecognizer.RecognizeCurrency(text, culture),
            NumberWithUnitRecognizer.RecognizeDimension(text, culture),
            NumberWithUnitRecognizer.RecognizeTemperature(text, culture),
            DateTimeRecognizer.RecognizeDateTime(text, culture),
            SequenceRecognizer.RecognizePhoneNumber(text, culture),
            SequenceRecognizer.RecognizeIpAddress(text, culture),
            SequenceRecognizer.RecognizeMention(text, culture),
            SequenceRecognizer.RecognizeHashtag(text, culture),
            SequenceRecognizer.RecognizeEmail(text, culture),
            SequenceRecognizer.RecognizeURL(text, culture),
            ChoiceRecognizer.RecognizeBoolean(text, culture)
        );

        foreach (ModelResult result in results)
        {
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }

        Console.WriteLine();
    }

    private static IEnumerable<ModelResult> MergeResults(params List<ModelResult>[] results)
    {
        return results.SelectMany(o => o);
    }
}
```

Выводит:

```json
I have two apples
{
  "Text": "two",
  "Start": 7,
  "End": 9,
  "TypeName": "number",
  "Resolution": {
    "value": "2"
  }
}

I live at eleventh floor
{
  "Text": "eleventh",
  "Start": 10,
  "End": 17,
  "TypeName": "ordinal",
  "Resolution": {
    "value": "11"
  }
}

One hundred percents
{
  "Text": "One hundred",
  "Start": 0,
  "End": 10,
  "TypeName": "number",
  "Resolution": {
    "value": "100"
  }
}
{
  "Text": "One hundred percents",
  "Start": 0,
  "End": 19,
  "TypeName": "percentage",
  "Resolution": {
    "value": "100%"
  }
}

Between 2 and 5
{
  "Text": "2",
  "Start": 8,
  "End": 8,
  "TypeName": "number",
  "Resolution": {
    "value": "2"
  }
}
{
  "Text": "5",
  "Start": 14,
  "End": 14,
  "TypeName": "number",
  "Resolution": {
    "value": "5"
  }
}
{
  "Text": "Between 2 and 5",
  "Start": 0,
  "End": 14,
  "TypeName": "numberrange",
  "Resolution": {
    "value": "(2,5)"
  }
}

After ninety five years of age, perspectives change
{
  "Text": "ninety five",
  "Start": 6,
  "End": 16,
  "TypeName": "number",
  "Resolution": {
    "value": "95"
  }
}
{
  "Text": "ninety five years of age",
  "Start": 6,
  "End": 29,
  "TypeName": "age",
  "Resolution": {
    "unit": "Year",
    "value": "95"
  }
}
{
  "Text": "after ninety five years",
  "Start": 0,
  "End": 22,
  "TypeName": "datetimeV2.duration",
  "Resolution": {
    "values": [
      {
        "timex": "P95Y",
        "Mod": "after",
        "type": "duration",
        "value": "2995920000"
      }
    ]
  }
}

Interest expense in the 1988 third quarter was $ 75.3 million
{
  "Text": "1988",
  "Start": 24,
  "End": 27,
  "TypeName": "number",
  "Resolution": {
    "value": "1988"
  }
}
{
  "Text": "75.3 million",
  "Start": 49,
  "End": 60,
  "TypeName": "number",
  "Resolution": {
    "value": "75300000"
  }
}
{
  "Text": "third",
  "Start": 29,
  "End": 33,
  "TypeName": "ordinal",
  "Resolution": {
    "value": "3"
  }
}
{
  "Text": "$ 75.3 million",
  "Start": 47,
  "End": 60,
  "TypeName": "currency",
  "Resolution": {
    "unit": "Dollar",
    "value": "75300000"
  }
}
{
  "Text": "1988 third quarter",
  "Start": 24,
  "End": 41,
  "TypeName": "datetimeV2.daterange",
  "Resolution": {
    "values": [
      {
        "timex": "(1988-07-01,1988-10-01,P3M)",
        "type": "daterange",
        "start": "1988-07-01",
        "end": "1988-10-01"
      }
    ]
  }
}

The six-mile trip to my airport hotel that had taken 20 minutes earlier in the day took more than three hours
{
  "Text": "six",
  "Start": 4,
  "End": 6,
  "TypeName": "number",
  "Resolution": {
    "value": "6"
  }
}
{
  "Text": "20",
  "Start": 53,
  "End": 54,
  "TypeName": "number",
  "Resolution": {
    "value": "20"
  }
}
{
  "Text": "three",
  "Start": 98,
  "End": 102,
  "TypeName": "number",
  "Resolution": {
    "value": "3"
  }
}
{
  "Text": "more than three",
  "Start": 88,
  "End": 102,
  "TypeName": "numberrange",
  "Resolution": {
    "value": "(3,)"
  }
}
{
  "Text": "six-mile",
  "Start": 4,
  "End": 11,
  "TypeName": "dimension",
  "Resolution": {
    "unit": "Mile",
    "value": "6"
  }
}
{
  "Text": "20 minutes",
  "Start": 53,
  "End": 62,
  "TypeName": "datetimeV2.duration",
  "Resolution": {
    "values": [
      {
        "timex": "PT20M",
        "type": "duration",
        "value": "1200"
      }
    ]
  }
}
{
  "Text": "the day",
  "Start": 75,
  "End": 81,
  "TypeName": "datetimeV2.date",
  "Resolution": {
    "values": [
      {
        "timex": "2018-05-21",
        "type": "date",
        "value": "2018-05-21"
      }
    ]
  }
}
{
  "Text": "more than three hours",
  "Start": 88,
  "End": 108,
  "TypeName": "datetimeV2.duration",
  "Resolution": {
    "values": [
      {
        "timex": "PT3H",
        "Mod": "more",
        "type": "duration",
        "value": "10800"
      }
    ]
  }
}

Set the temperature to 30 degrees celsius
{
  "Text": "30",
  "Start": 23,
  "End": 24,
  "TypeName": "number",
  "Resolution": {
    "value": "30"
  }
}
{
  "Text": "30 degrees celsius",
  "Start": 23,
  "End": 40,
  "TypeName": "temperature",
  "Resolution": {
    "unit": "C",
    "value": "30"
  }
}

I'll go back 8pm today
{
  "Text": "8pm",
  "Start": 13,
  "End": 15,
  "TypeName": "dimension",
  "Resolution": {
    "unit": "Picometer",
    "value": "8"
  }
}
{
  "Text": "8pm today",
  "Start": 13,
  "End": 21,
  "TypeName": "datetimeV2.datetime",
  "Resolution": {
    "values": [
      {
        "timex": "2018-05-21T20",
        "type": "datetime",
        "value": "2018-05-21 20:00:00"
      }
    ]
  }
}

My phone number is ( 19 ) 38294427
{
  "Text": "19",
  "Start": 21,
  "End": 22,
  "TypeName": "number",
  "Resolution": {
    "value": "19"
  }
}
{
  "Text": "38294427",
  "Start": 26,
  "End": 33,
  "TypeName": "number",
  "Resolution": {
    "value": "38294427"
  }
}
{
  "Text": "( 19 ) 38294427",
  "Start": 19,
  "End": 33,
  "TypeName": "phonenumber",
  "Resolution": {
    "value": "( 19 ) 38294427"
  }
}

My Ip is 8.8.8.8
{
  "Text": "8",
  "Start": 9,
  "End": 9,
  "TypeName": "number",
  "Resolution": {
    "value": "8"
  }
}
{
  "Text": "8",
  "Start": 11,
  "End": 11,
  "TypeName": "number",
  "Resolution": {
    "value": "8"
  }
}
{
  "Text": "8",
  "Start": 13,
  "End": 13,
  "TypeName": "number",
  "Resolution": {
    "value": "8"
  }
}
{
  "Text": "8",
  "Start": 15,
  "End": 15,
  "TypeName": "number",
  "Resolution": {
    "value": "8"
  }
}
{
  "Text": "8.8.8.8",
  "Start": 9,
  "End": 15,
  "TypeName": "ip",
  "Resolution": {
    "type": "ipv4",
    "value": "8.8.8.8"
  }
}

Like @Cicero
{
  "Text": "@Cicero",
  "Start": 5,
  "End": 11,
  "TypeName": "mention",
  "Resolution": {
    "value": "@Cicero"
  }
}

Done task #123
{
  "Text": "123",
  "Start": 11,
  "End": 13,
  "TypeName": "number",
  "Resolution": {
    "value": "123"
  }
}
{
  "Text": "#123",
  "Start": 10,
  "End": 13,
  "TypeName": "hashtag",
  "Resolution": {
    "value": "#123"
  }
}

My e-mail is a@b.com
{
  "Text": "a@b.com",
  "Start": 13,
  "End": 19,
  "TypeName": "email",
  "Resolution": {
    "value": "a@b.com"
  }
}

Search with bing.com
{
  "Text": "bing.com",
  "Start": 12,
  "End": 19,
  "TypeName": "url",
  "Resolution": {
    "value": "bing.com"
  }
}

yup, I need that

Для продолжения нажмите любую клавишу . . .
```
