### iText

iText 7 (бывший iTextSharp) — полностью управляемый фреймворк для создания PDF-файлов. Имеется как бесплатная Community версия, так и коммерческая лицензция.

GitHub: https://github.com/itext, NuGet: https://www.nuget.org/packages/itext7/ (старая версия — https://www.nuget.org/packages/iTextSharp/), официальный сайт: http://itextpdf.com/.

Поддерживает .NET Framework 4.0 и .NET Standard 1.6.

Простейшая программа:

```csharp
using System;
using System.IO;

using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

class Program
{
    static void Main()
    {
        PdfWriter writer = new PdfWriter("Hello.pdf");
        PdfDocument pdf = new PdfDocument(writer);
        Document document = new Document(pdf);
        document.Add(new Paragraph("Hello, world!"));
        document.Close();
    }
}
```

Кириллические символы, к сожалению, так просто вывести не получится. Понадобятся дополнительные телодвижения. Пример таких телодвижений под Windows:

```csharp
using System;
using System.IO;

using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

class Program
{
    static void Main()
    {
        string arialPath = Path.Combine
            (
                Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "arial.ttf"
            );
        PdfFontFactory.Register(arialPath);
        PdfFont arialFont = PdfFontFactory.CreateRegisteredFont("Arial", "CP1251");
        string timesPath = Path.Combine
            (
                Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "times.ttf"
            );
        PdfFontFactory.Register(timesPath);
        PdfFont timesFont = PdfFontFactory.CreateRegisteredFont("Times New Roman", "CP1251");

        PdfWriter writer = new PdfWriter("Hello.pdf");
        PdfDocument pdf = new PdfDocument(writer);
        Document document = new Document(pdf);
        document.Add(new Paragraph("Привет, Arial!").SetFont(arialFont));
        document.Add(new Paragraph("Привет, Times New Roman!").SetFont(timesFont));
        document.Close();
    }
}
```

Создание списка:

```csharp
PdfWriter writer = new PdfWriter("Hello.pdf");
PdfDocument pdf = new PdfDocument(writer);
Document document = new Document(pdf);
document.Add(new Paragraph("iText is:"));
List list = new List()
    .SetSymbolIndent(12)
    .SetListSymbol("\u2022");
list.Add(new ListItem("Never gonna give you up"))
    .Add(new ListItem("Never gonna let you down"))
    .Add(new ListItem("Never gonna run around and desert you"))
    .Add(new ListItem("Never gonna make you cry"))
    .Add(new ListItem("Never gonna say goodbye"))
    .Add(new ListItem("Never gonna tell a lie and hurt you"));
document.Add(list);
document.Close();
```

Вставка картинок:

```csharp
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

class Program
{
    static void Main()
    {
        PdfWriter writer = new PdfWriter("Hello.pdf");
        PdfDocument pdf = new PdfDocument(writer);
        Document document = new Document(pdf);
        Image cat = new Image(ImageDataFactory.Create("cat.jpg"))
            .ScaleToFit(100f, 100f);
        document.Add(new Paragraph("Here is cat: ").Add(cat));
        Image dog = new Image(ImageDataFactory.Create("dog.jpg"))
            .ScaleToFit(100f, 100f);
        document.Add(new Paragraph("And here is dog: ").Add(dog));
        document.Close();
    }
}
```

Вот что получается:

![pdfImages](img/pdfImages.png)

Теперь попробуем создать таблицу. Пусть у нас есть файл `unites_states.csv` следующего содержания:

```
name;abbr;capital;most populous city;population;square miles;time zone 1;time zone 2;dst
ALABAMA;AL;Montgomery;Birmingham;4,708,708;52,423;CST (UTC-6);EST (UTC-5);YES
ALASKA;AK;Juneau;Anchorage;698,473;656,425;AKST (UTC-09) ;HST (UTC-10) ;YES
ARIZONA;AZ;Phoenix;Phoenix;6,595,778;114,006;MT (UTC-07); ;NO
ARKANSAS;AR;Little Rock;Little Rock;2,889,450;53,182;CST (UTC-6); ;YES
CALIFORNIA;CA;Sacramento;Los Angeles;36,961,664;163,707;PT (UTC-8); ;YES
COLORADO;CO;Denver;Denver;5,024,748;104,100;MT (UTC-07); ;YES
```

Вот как строится таблица:

```csharp
using System;
using System.IO;

using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

class Program
{
    public static void ProcessLine
        (
            Table table,
            string line,
            PdfFont font,
            bool isHeader
        )
    {
        string[] parts = line.Split(';');
        foreach (string part in parts)
        {
            Cell cell = new Cell().Add(new Paragraph(part).SetFont(font));
            if (isHeader)
            {
                table.AddHeaderCell(cell);
            }
            else
            {
                table.AddCell(cell);
            }
        }
    }

    static void Main()
    {
        PdfWriter writer = new PdfWriter("Hello.pdf");
        PdfDocument pdf = new PdfDocument(writer);
        Document document = new Document(pdf, PageSize.A4.Rotate());
        document.SetMargins(20, 20, 20, 20);
        PdfFont font = PdfFontFactory.CreateFont
            (
                iText.IO.Font.Constants.StandardFonts.HELVETICA
            );
        PdfFont bold = PdfFontFactory.CreateFont
            (
                iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD
            );
        Table table = new Table(new float[] { 4, 1, 3, 4, 3, 3, 3, 3, 1 });
        table.SetWidth(UnitValue.CreatePercentValue(100));
        TextReader reader = new StreamReader("united_states.csv");
        String line = reader.ReadLine();
        ProcessLine(table, line, bold, true);
        while ((line = reader.ReadLine()) != null)
        {
            ProcessLine(table, line, font, false);
        }
        reader.Close();
        document.Add(table);
        document.Close();
    }
}

```

Вот что получается:

![pdfTable](img/pdfTable.png)

