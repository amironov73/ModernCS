### FFmpeg в .NET

Наверное, нет такого человека, который не сталкивался с набором библиотек FFmpeg, просто не все знают, что это такое. Это набор библиотек и утилит с открытым кодом, которые умеют читать, записывать, конвертировать и передавать аудио- и видеозаписи. Производители железа и софта суют FFmpeg куда только могут. Вот и я решил поинтересоваться, нельзя ли как-нибудь и мне воспользоваться этим замечательным продуктом из .NET-программы. Оказывается, можно, и это совсем нетрудно.

Добрые люди разработали множество .NET-обёрток над FFmpeg разной степени полноты и удобства. В этом посте Я рассмотрю одну из самых популярных – MediaToolkit. Этот проект с открытым кодом обитает на ГитХабе: https://github.com/AydinAdn/MediaToolkit и опубликован в NuGet: https://www.nuget.org/packages/MediaToolkit/. По факту в проект добавляется сборка MediaToolkit.dll размером около 10 Мб, больше ничего не требуется, всё необходимое включено в сборку, что не может не радовать.

Что может MediaToolkit:

* Извлекать кадры из видео;
* Обрезать и разделять видео;
* Конвертировать видео в разные форматы;
* Изменять битрейт, фреймрейт, разрешение и размеры видео;
* Конвертировать аудио;
* Поддерживает DCD, DV, DV50, VCD, SVCD.

Пример извлечения кадра из видео:

```csharp
using System;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
 
class Program
{
    static void Main(string[] args)
    {
        var inputFile = new MediaFile 
          { Filename = @"C:\Temp10\Лого.mp4" };
        var outputFile = new MediaFile 
          { Filename = @"C:\Temp10\SavedImage.jpg" };
 
        using (var engine = new Engine())
        {
            engine.GetMetadata(inputFile);
 
            var options = new ConversionOptions 
              { Seek = TimeSpan.FromSeconds(15) };
            engine.GetThumbnail(inputFile, outputFile, options);
        }
    }
}
```

Извлекается такой кадр:

![SavedImage](img/SavedImage.jpg)

Извлечение метаданных:

```csharp
var inputFile = new MediaFile { Filename = @"C:\Temp10\Лого.mp4" };
 
using (var engine = new Engine())
{
   engine.GetMetadata(inputFile);
   Console.WriteLine(inputFile.Metadata.Duration);
   Console.ReadLine();
   // Выведет: 00:00:59.9900000
}
```

Простое преобразование формата:

```csharp
var inputFile = new MediaFile { Filename = @"C:\Temp10\Лого.mp4" };
var outputFile = new MediaFile { Filename = @"C:\Temp10\Logo.wmv" };
 
using (var engine = new Engine())
{
   engine.Convert(inputFile, outputFile);
}
```

Преобразование с опциями:

```csharp
var inputFile = new MediaFile { Filename = @"C:\Temp10\Лого.mp4" };
var outputFile = new MediaFile { Filename = @"C:\Temp\Лого.wmv" };
 
var conversionOptions = new ConversionOptions
{
    MaxVideoDuration = TimeSpan.FromSeconds(30),
    VideoAspectRatio = VideoAspectRatio.R16_9,
    VideoSize = VideoSize.Hd1080,
    AudioSampleRate = AudioSampleRate.Hz44100
};
 
using (var engine = new Engine())
{
    engine.Convert(inputFile, outputFile, conversionOptions);
}
```

Обрезка видео до меньшей длины:

```csharp
var inputFile = new MediaFile { Filename = @"C:\Temp10\Лого.mp4" };
var outputFile = new MediaFile { Filename = @"C:\Temp\Лого.wmv" };
 
using (var engine = new Engine())
{
    engine.GetMetadata(inputFile);
 
    var options = new ConversionOptions();
 
    // This example will create a 25 second video, starting from the 
    // 30th second of the original video.
    //// First parameter requests the starting frame to cut the media from.
    //// Second parameter requests how long to cut the video.
    options.CutMedia(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(25));
 
    engine.Convert(inputFile, outputFile, options);
}
```

Подписка на события:

```csharp
public void StartConverting()
{
    var inputFile = new MediaFile { Filename = @"C:\Temp10\Лого.mp4" };
    var outputFile = new MediaFile { Filename = @"C:\Temp\Лого.wmv" };
    using (var engine = new Engine())
    {
        engine.ConvertProgressEvent += ConvertProgressEvent;
        engine.ConversionCompleteEvent += engine_ConversionCompleteEvent;
        engine.Convert(inputFile, outputFile);
    }
}
 
private void ConvertProgressEvent(object sender, 
  ConvertProgressEventArgs e)
{
    Console.WriteLine("\n------------\nConverting...\n------------");
    Console.WriteLine("Bitrate: {0}", e.Bitrate);
    Console.WriteLine("Fps: {0}", e.Fps);
    Console.WriteLine("Frame: {0}", e.Frame);
    Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
    Console.WriteLine("SizeKb: {0}", e.SizeKb);
    Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
}
 
private void engine_ConversionCompleteEvent(object sender, 
  ConversionCompleteEventArgs e)
{
    Console.WriteLine("\n------------\nConversion complete!\n------------");
    Console.WriteLine("Bitrate: {0}", e.Bitrate);
    Console.WriteLine("Fps: {0}", e.Fps);
    Console.WriteLine("Frame: {0}", e.Frame);
    Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
    Console.WriteLine("SizeKb: {0}", e.SizeKb);
    Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
}
```


