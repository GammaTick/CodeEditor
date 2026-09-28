using Avalonia.Media;

namespace CodeEditor.Editor;

internal class EditorTextStyle : ObservableObject
{
    private const double LineVerticalMargins = 4;

    public FontFamily FontFamily { get; private set; } = new FontFamily("Consolas");
    public Typeface Typeface { get; private set; }
    public double FontSize { get; private set; } = 18;
    public double LineHeight { get; private set; }
    public IBrush Foreground { get; private set; } = Brushes.White;

    public EditorTextStyle()
    {
        Typeface = new Typeface(FontFamily);
        LineHeight = FontSize + LineVerticalMargins;
    }

    public void SetFontFamily(FontFamily fontFamily)
    {
        FontFamily = fontFamily;
        Typeface = new Typeface(fontFamily);

        NotifyChanged();
    }

    public void SetFontSize(double fontSize)
    {
        if (fontSize <= 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(fontSize));
        }

        if (FontSize == fontSize)
        {
            return;
        }

        FontSize = fontSize;
        LineHeight = FontSize + LineVerticalMargins;

        NotifyChanged();
    }

    public void SetForeground(IBrush foreground)
    {
        Foreground = foreground;

        NotifyChanged();
    }
}