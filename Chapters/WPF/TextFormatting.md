### Форматирование текста

```csharp
class MyTextParagraphProperties : TextParagraphProperties
{
    public override FlowDirection FlowDirection
        => FlowDirection.LeftToRight;
    public override TextAlignment TextAlignment
        => TextAlignment.Left;
    public override double LineHeight => 20;
    public override bool FirstLineInParagraph => false;
    public override TextRunProperties DefaultTextRunProperties
        => new MyTextRunProperties(Colors.Black);

    public override TextWrapping TextWrapping => TextWrapping.Wrap;
    public override TextMarkerProperties TextMarkerProperties
        => new TextSimpleMarkerProperties(TextMarkerStyle.Box, 0, 1,
            new MyTextParagraphProperties());

    public override double Indent => 0;
}

class MyTextRunProperties: TextRunProperties
{
    private readonly Color _color;

    public MyTextRunProperties(Color color)
    {
        _color = color;
    }

    public override Typeface Typeface => new Typeface("Arial");

    public override double FontRenderingEmSize => 16;

    public override double FontHintingEmSize => 0;

    public override TextDecorationCollection TextDecorations => new TextDecorationCollection();

    public override Brush ForegroundBrush => new SolidColorBrush(_color);

    public override Brush BackgroundBrush => Brushes.Transparent;

    public override CultureInfo CultureInfo => CultureInfo.CurrentCulture;

    public override TextEffectCollection TextEffects => new TextEffectCollection();
}

class MyTextSource : TextSource
{
    private int index;
    private char[] chars;

    public MyTextSource
        (
            string text
        )
    {
        chars = text.ToCharArray();
    }

    public override TextRun GetTextRun(int textSourceCharacterIndex)
    {
        Color color = Colors.Blue;
        if (index % 2 == 1)
        {
            color = Colors.Red;
        }

        index++;

        int length = chars.Length - textSourceCharacterIndex;
        if (length > 3)
        {
            length = 3;
        }
        if (length <= 0)
        {
            return new TextEndOfParagraph(1);
        }
        TextCharacters result = new TextCharacters
            (
                chars,
                textSourceCharacterIndex,
                length,
                new MyTextRunProperties(color)
            );

        return result;
    }

    public override TextSpan<CultureSpecificCharacterBufferRange>
        GetPrecedingText(int textSourceCharacterIndexLimit)
    {
        throw new NotImplementedException();
    }

    public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex
        (int textSourceCharacterIndex)
    {
        throw new NotImplementedException();
    }
}

public class CustomControl1 : Control
{
  static CustomControl1()
  {
      DefaultStyleKeyProperty.OverrideMetadata
          (
              typeof(CustomControl1),
              new FrameworkPropertyMetadata(typeof(CustomControl1))
          );
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
      Brush brush = new SolidColorBrush(Colors.AliceBlue);
      Pen pen = new Pen(new SolidColorBrush(Colors.Blue), 2);
      Rect rect = new Rect(RenderSize);
      drawingContext.DrawRectangle(brush, pen, rect);

      TextFormatter formatter = TextFormatter.Create();
      MyTextParagraphProperties paragraphProperties = new MyTextParagraphProperties();
      string myText = "Жили у бабуси два веселых гуся, "
          + "один белый, другой серый, два веселых гуся. "
          + "Один в лес пролез, а другой не пролез! "
          + "У попа была собака, он ее любил, она съела "
          + "кусок мяса, он ее убил. В землю закопал и "
          + "надпись написал.";
      MyTextSource source = new MyTextSource(myText);
      Point point = new Point(4, 4);

      int position = 0;
      while (position < myText.Length)
      {
          TextLine line = formatter.FormatLine(source, position,
              rect.Width - 8, paragraphProperties, null);
          line.Draw(drawingContext, point, InvertAxes.None);
          position += line.Length;
          point.Y += line.Height;
          line.Dispose();
      }
  }
}
```

