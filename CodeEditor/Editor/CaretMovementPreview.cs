using System;

namespace CodeEditor.Editor;

internal static class CaretMovementPreview
{
    public static CaretMovementResult Backwards(TextBuffer textBuffer, CaretController caretController)
    {
        int caretLine = caretController.Line;

        if ((uint)caretLine >= (uint)textBuffer.LineCount)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(caretLine));
        }

        var lineSpan = textBuffer.GetLine(caretLine);

        int caretColumn = caretController.Column;

        if ((uint)caretColumn > (uint)lineSpan.Span.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(caretColumn));
        }

        // In the future make it use real grapheme 

        int caretAbsoluteIndex = caretController.AbsoluteIndex;

        if (caretColumn == 0)
        {
            if (caretLine == 0)
            {
                return default;
            }

            var aboveLine = textBuffer.GetLine(caretLine - 1);

            int chars = lineSpan.Start - aboveLine.End;

            return new CaretMovementResult(caretAbsoluteIndex - chars, caretLine - 1, aboveLine.Span.Length, true);
        }

        return new CaretMovementResult(caretAbsoluteIndex - 1, caretLine, caretColumn - 1, true);
    }

    public static CaretMovementResult Forwards(TextBuffer textBuffer, CaretController caretController)
    {
        int caretLine = caretController.Line;

        if ((uint)caretLine >= (uint)textBuffer.LineCount)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(caretLine));
        }

        var lineSpan = textBuffer.GetLine(caretLine);

        int caretColumn = caretController.Column;

        if ((uint)caretColumn > (uint)lineSpan.Span.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(caretColumn));
        }

        // In the future make it use real grapheme 

        int caretAbsoluteIndex = caretController.AbsoluteIndex;

        int currentLineLength = lineSpan.Span.Length;

        if (caretColumn == currentLineLength)
        {
            if (caretLine == textBuffer.LineCount)
            {
                return default;
            }

            var bellowLine = textBuffer.GetLine(caretLine + 1);

            int chars = bellowLine.Start - lineSpan.End;

            return new CaretMovementResult(caretAbsoluteIndex + chars, caretLine + 1, 0, true);
        }

        return new CaretMovementResult(caretAbsoluteIndex + 1, caretLine, caretColumn + 1, true);
    }

    internal readonly struct CaretMovementResult
    {
        public int AbsoluteIndex { get; }
        public int Line { get; }
        public int Column { get; }
        public bool HasMoved { get; }

        public CaretMovementResult(int absoluteIndex, int line, int column, bool hasMoved)
        {
            AbsoluteIndex = absoluteIndex;
            Line = line;
            Column = column;
            HasMoved = hasMoved;
        }
    }
}