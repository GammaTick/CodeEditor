using System;

namespace CodeEditor.Editor;

internal abstract class ObservableObject
{
    public event EventHandler? Changed;

    protected void NotifyChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }
}