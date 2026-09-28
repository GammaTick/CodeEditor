using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace CodeEditor.Editor;

internal class TextBuffer : ObservableObject
{
    private SourceText _sourceText = SourceText.From(string.Empty, Encoding.UTF8);

    public int LineCount => _sourceText.Lines.Count;

    public TextChange InsertText(int line, int column, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            ThrowHelper.ThrowArgumentNullException(nameof(text));
        }

        if ((uint)line >= (uint)LineCount)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(line));
        }

        var lineSpan = _sourceText.Lines[line];

        if ((uint)column > (uint)lineSpan.Span.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(column));
        }

        int absoluteInsertionPos = lineSpan.Start + column;

        var textChange = new TextChange(new TextSpan(absoluteInsertionPos, 0), text);

        _sourceText = _sourceText.WithChanges(textChange);

        NotifyChanged();

        return textChange;
    }

    public (int line, int column) GetLineAndColumnFromAbsoluteIndex(int absoluteIndex)
    {
        if (absoluteIndex < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(absoluteIndex));
        }

        var textLine = _sourceText.Lines.GetLineFromPosition(absoluteIndex);

        int line = textLine.LineNumber;
        int column = absoluteIndex - textLine.Start;

        return (line, column);
    }

    public int GetLineLength(int line)
    {
        if ((uint)line >= LineCount)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(line));
        }

        return _sourceText.Lines[line].Span.Length;
    }

    public int GetLineLengthIncludingLineBreak(int line)
    {
        if ((uint)line >= LineCount)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(line));
        }

        return _sourceText.Lines[line].SpanIncludingLineBreak.Length;
    }

    public TextLine GetLine(int line)
    {
        if ((uint)line >= LineCount)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(line));
        }

        return _sourceText.Lines[line];
    }

    public TextChange DeleteText(int line, int column, int length)
    {
        if (length < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(length));
        }

        if ((uint)line >= (uint)LineCount)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(line));
        }

        var lineSpan = _sourceText.Lines[line];

        if ((uint)column > (uint)lineSpan.Span.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(column));
        }

        int absoluteDeletionPos = lineSpan.Start + column;

        var textChange = new TextChange(new TextSpan(absoluteDeletionPos, length), string.Empty);

        _sourceText = _sourceText.WithChanges(textChange);

        NotifyChanged();

        return textChange;
    }

    public TextChange InsertNewLine(int line, int column)
    {
        return InsertText(line, column, Environment.NewLine);
    }
}