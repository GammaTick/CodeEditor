using System.Runtime.CompilerServices;

namespace CodeEditor.Editor;

internal class CaretController : ObservableObject
{
    private readonly TextBuffer _textBuffer;

    private int _line = 0;
    private int _column = 0;
    private int _preferredColumn = 0;
    private int _absoluteIndex = 0;

    public int Line => _line;
    public int Column => _column;
    public int PreferredColumn => _preferredColumn;
    public int AbsoluteIndex => _absoluteIndex;

    public CaretController(TextBuffer textBuffer)
    {
        _textBuffer = textBuffer;
    }

    public void Move(MovementDirection movementDirection)
    {
        switch (movementDirection)
        {
            case MovementDirection.Up:
                MoveUp();
                return;

            case MovementDirection.Down:
                MoveDown();
                return;

            case MovementDirection.Left:
                MoveLeft();
                return;

            case MovementDirection.Right:
                MoveRight();
                return;
        }
    }

    public void MoveByCharCount(int count, MovementDirection movementDirection)
    {
        if (count < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count));
        }

        if (count == 0)
        {
            return;
        }

        if (movementDirection != MovementDirection.Left && movementDirection != MovementDirection.Right)
        {
            throw new System.Exception("Custom error");
        }

        if (movementDirection == MovementDirection.Left)
        {
            count *= -1;
        }

        int newAbsolutePosition = _absoluteIndex + count;

        (int line, int column) = _textBuffer.GetLineAndColumnFromAbsoluteIndex(newAbsolutePosition);

        _line = line;
        _column = column;
        _preferredColumn = column;
        _absoluteIndex = newAbsolutePosition;

        NotifyChanged();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MoveUp()
    {
        if (_line == 0)
        {
            return;
        }

        int previousLineLength = _textBuffer.GetLineLength(_line - 1);
        int previousLineLengthIncludingBreak = _textBuffer.GetLineLengthIncludingLineBreak(_line - 1);

        int oldColumn = _column;

        _line--;

        if (_column > previousLineLength)
        {
            _column = previousLineLength;
        }
        else
        {
            _column = _preferredColumn;
        }

        _absoluteIndex -= previousLineLengthIncludingBreak - _column + oldColumn;

        NotifyChanged();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MoveDown()
    {
        if (_line == _textBuffer.LineCount - 1)
        {
            return;
        }

        int currentLineLengthIncludingBreak = _textBuffer.GetLineLengthIncludingLineBreak(_line);
        int targetLineLength = _textBuffer.GetLineLength(_line + 1);

        int oldColumn = _column;

        _line++;

        if (_column > targetLineLength)
        {
            _column = targetLineLength;
        }
        else
        {
            _column = _preferredColumn;
        }

        _absoluteIndex += currentLineLengthIncludingBreak - oldColumn + _column;

        NotifyChanged();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MoveLeft()
    {
        if (_column == 0)
        {
            if (_line == 0)
            {
                return;
            }

            _line--;

            int targetLineLength = _textBuffer.GetLineLength(_line);
            int lineBreakLength = _textBuffer.GetLineLengthIncludingLineBreak(_line) - targetLineLength;

            _column = targetLineLength;
            _preferredColumn = targetLineLength;
            _absoluteIndex -= lineBreakLength;

            NotifyChanged();

            return;
        }

        _column--;
        _preferredColumn--;
        _absoluteIndex--;

        NotifyChanged();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MoveRight()
    {
        int currentLineLength = _textBuffer.GetLineLength(_line);

        if (_column == currentLineLength)
        {
            int lineBreakLength = _textBuffer.GetLineLengthIncludingLineBreak(_line) - currentLineLength;

            _line++;
            _column = 0;
            _preferredColumn = 0;
            _absoluteIndex += lineBreakLength;

            NotifyChanged();

            return;
        }

        _column++;
        _preferredColumn++;
        _absoluteIndex++;

        NotifyChanged();
    }

    public enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}