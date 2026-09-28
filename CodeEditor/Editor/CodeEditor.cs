using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace CodeEditor.Editor;

internal class CodeEditor : Control
{
    public static readonly StyledProperty<IBrush> BackgroundProperty =
        AvaloniaProperty.Register<CodeEditor, IBrush>(nameof(Background), new SolidColorBrush(new Color(255, 31, 31, 31)));

    static CodeEditor()
    {
        AffectsRender<CodeEditor>(
            BackgroundProperty
        );
    }

    public IBrush Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    private readonly EditorUpdateCoordinator _updateCoordinator = new();

    private readonly TextBuffer _textBuffer;
    private readonly CaretController _caretController;
    private readonly Viewport _viewport;
    private readonly EditorTextStyle _editorTextStyle;
    private readonly EditorInputHandler _editorInputHandler;

    public CodeEditor()
    {
        _textBuffer = new TextBuffer();
        _caretController = new CaretController(_textBuffer);
        _viewport = new Viewport();
        _editorTextStyle = new EditorTextStyle();
        _editorInputHandler = new EditorInputHandler(_textBuffer, _caretController);

        _updateCoordinator.Add(_textBuffer, InvalidateVisual);
        _updateCoordinator.Add(_caretController, InvalidateVisual);
        _updateCoordinator.Add(_editorTextStyle, InvalidateVisual);

        Focusable = true;
        Cursor = new Cursor(StandardCursorType.Ibeam);
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);

        e.Handled = _editorInputHandler.HandleTextInput(e.Text);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        e.Handled = _editorInputHandler.HandleKey(e.Key);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        _viewport.SetSize(e.NewSize.Width, e.NewSize.Height);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.FillRectangle(Background, new Rect(Bounds.Size));

        if (_textBuffer.LineCount == 0)
        {
            return;
        }

        var fontSize = _editorTextStyle.FontSize;

        int visibleLinesCount = _viewport.GetVisibleLinesCount(fontSize, _textBuffer.LineCount);

        if (visibleLinesCount == 0)
        {
            return;
        }

        var typeface = _editorTextStyle.Typeface;
        var lineHeight = _editorTextStyle.LineHeight;
        var foreground = _editorTextStyle.Foreground;

        for (int i = 0; i < visibleLinesCount; i++)
        {
            var line = _textBuffer.GetLine(i);

            using var textLayout = new TextLayout(line.ToString(), typeface, fontSize, foreground);

            textLayout.Draw(context, new Point(0, i * lineHeight));
        }
    }
}