using System;
using Avalonia.Input;

namespace CodeEditor.Editor;

internal class EditorInputHandler
{
    private readonly TextBuffer _textBuffer;
    private readonly CaretController _caretController;

    public EditorInputHandler(TextBuffer textBuffer, CaretController caretController)
    {
        _textBuffer = textBuffer;
        _caretController = caretController;
    }

    public bool HandleTextInput(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        _textBuffer.InsertText(_caretController.Line, _caretController.Column, text);
        _caretController.MoveByCharCount(text.Length, CaretController.MovementDirection.Right);

        return true;
    }

    public bool HandleKey(Key key)
    {
        switch (key)
        {
            case Key.Back:
                return HandleBackspace();

            case Key.Enter:
                HandleEnter();
                return true;
        }

        return false;
    }

    private bool HandleBackspace()
    {
        var result = CaretMovementPreview.Backwards(_textBuffer, _caretController);

        if (!result.HasMoved)
        {
            return false;
        }

        int currentCaretAbsPos = _caretController.AbsoluteIndex;
        int movedChars = currentCaretAbsPos - result.AbsoluteIndex;

        _caretController.MoveByCharCount(movedChars, CaretController.MovementDirection.Left);
        _textBuffer.DeleteText(result.Line, result.Column, movedChars);

        return true;
    }

    private void HandleEnter()
    {
        _textBuffer.InsertNewLine(_caretController.Line, _caretController.Column);

        var result = CaretMovementPreview.Forwards(_textBuffer, _caretController);

        int currentCaretAbsPos = _caretController.AbsoluteIndex;
        int movedChars = result.AbsoluteIndex - currentCaretAbsPos;

        _caretController.MoveByCharCount(movedChars, CaretController.MovementDirection.Right);
    }
}