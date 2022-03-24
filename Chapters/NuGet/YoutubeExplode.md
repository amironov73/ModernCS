### Библиотека YoutubeExplode

Отличная библиотека [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) позволяет скачивать видео с YouTube прямо из своего приложения. Поддерживает .NET Framework, начиная с 4.6.1, .NET Core, начиная с 2.0, Mono, Tizen.

Базовый сценарий использования прост и интуитивно понятен: достаточно подключить NuGet-пакет [YoutubeExplode](https://www.nuget.org/packages/YoutubeExplode).

```c#
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
 
const string videoUrl = "https://www.youtube.com/watch?v=gSFXMtdqSdU";
 
var youtube = new YoutubeClient();
var videoId = VideoId.Parse (videoUrl);
 
var streamManifest = await youtube.Videos.Streams.GetManifestAsync (videoId);
var streamInfo = streamManifest.GetMuxedStreams()
  .TryGetWithHighestVideoQuality();
if (streamInfo is null)
{
    Console.Error.WriteLine ("Ошибка!");
    return 1;
}
 
Console.WriteLine ($"Video: {streamInfo.VideoQuality.Label}");
 
var fileName = $"{videoId}.{streamInfo.Container.Name}";
using (var progress = new InlineProgress())
{
    await youtube.Videos.Streams.DownloadAsync (streamInfo,
        fileName, progress);
}
 
Console.WriteLine ("ALL DONE");
 
return 0;
 
internal sealed class InlineProgress
    : IProgress<double>, IDisposable
{
    private readonly int _posX;
    private readonly int _posY;
 
    public InlineProgress()
    {
        _posX = Console.CursorLeft;
        _posY = Console.CursorTop;
    }
 
    public void Report (double progress)
    {
        Console.SetCursorPosition (_posX, _posY);
        Console.WriteLine ($"{progress:P1}");
    }
 
    public void Dispose()
    {
        Console.SetCursorPosition (_posX, _posY);
        Console.WriteLine ("Completed ✓");
    }
}
```

Довольно часто ролики в высоком разрешении на YouTube разделены – видео в одном потоке, аудио в другом (для экономии места на серверах). Если хочется видео со звуком в одном файле, придется подключать пакет `YoutubeExplode.Converter`, интегрирующий для этих целей FFmpeg.
