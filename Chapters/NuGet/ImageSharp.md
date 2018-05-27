### Библиотека ImageSharp

[ImageSharp](https://sixlabors.com/projects/imagesharp/) - новая кросс-платформенная библиотека обработки изображений, ориентированная на .NET Core. Она содержит только чистый управляемый код. Производительность не так хороша, как у библиотек, зависящих от нативных компомнентов, но вполне приемлема.

GitHub: https://github.com/SixLabors/ImageSharp, NuGet: https://www.nuget.org/packages/SixLabors.ImageSharp.

Библиотека поддерживает .NET Core 2.0 и 2.1.

Пример примитивной программы, рисующей линии (надо подключить два пакета: SixLabors.ImageSharp и SixLabors.ImageSharp.Drawing):

```csharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing.Overlays;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.Primitives;

class Program
{
    static void Main()
    {
        using (Image<Rgba32> image = new Image<Rgba32>(500, 250))
        {
            image.Mutate
            (
                img => img.BackgroundColor(Rgba32.Blue)
                    .DrawLines(Rgba32.White, 5,
                        new[]
                            {
                                new PointF(10, 10),
                                new Point(200, 150),
                                new PointF(50, 200)
                            })
            );

            image.Save("imagesharp.png");
        }
    }
}
```

Результат:

![imagesharp](img/imagesharp.png)
