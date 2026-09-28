using System;

namespace CodeEditor.Editor;

internal class Viewport
{
    public double Width { get; private set; }
    public double Height { get; private set; }

    public double HorizontalOffset { get; private set; }
    public double VerticalOffset { get; private set; }

    public void SetSize(double width, double height)
    {
        if (width <= 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(height));
        }

        Width = width;
        Height = height;
    }

    public int GetVisibleLinesCount(double lineHeight, int totalLines)
    {
        int visible = (int)Math.Floor(Height / lineHeight);
        return Math.Min(visible, totalLines);
    }
}