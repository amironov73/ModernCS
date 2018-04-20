### Magick.NET

http://magick.codeplex.com/

ImageMagick is a powerful image manipulation library that supports over 100 major file formats (not including sub-formats). With Magick.NET you can use ImageMagick in your C#/VB.NET application without having to install ImageMagick on your server or desktop.

```csharp
using System;
using ImageMagick;
 
namespace MagickExample
{
   class Program
   {
      static void Main(string[] args)
      {
         MagickNET.Initialize(@"C:\MyProgram\MyImageMagickXmlFiles");
 
         using (MagickImage image = new MagickImage("Snakeware.gif"))
         {
            image.Write("Snakeware.jpg");
         }
 
         // Read image that has no predefined dimensions.
         MagickReadSettings settings = new MagickReadSettings();
         settings.Width = 800;
         settings.Height = 600;
         using (MagickImage image = new MagickImage("xc:yellow", settings))
         {
         }
      }
   }
}
```

