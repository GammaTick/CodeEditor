using System;
using System.Collections.Generic;

namespace CodeEditor.Editor;

internal class EditorUpdateCoordinator
{
    private readonly Dictionary<ObservableObject, List<Action>> _updates = [];

    public void Add(ObservableObject observableObject, params Action[] actions)
    {
        if (actions.Length == 0)
        {
            throw new ArgumentException("At least one action is required.", nameof(actions));
        }

        if (_updates.TryGetValue(observableObject, out List<Action>? existingActions))
        {
            existingActions.AddRange(actions);
            return;
        }

        _updates.Add(observableObject, [.. actions]);
        observableObject.Changed += OnObjectChanged;
    }

    public void Dispose()
    {
        foreach (ObservableObject observableObject in _updates.Keys)
        {
            observableObject.Changed -= OnObjectChanged;
        }

        _updates.Clear();
    }

    private void OnObjectChanged(object? sender, EventArgs e)
    {
        if (sender is not ObservableObject observableObject)
        {
            return;
        }

        if (!_updates.TryGetValue(observableObject, out List<Action>? actions))
        {
            return;
        }

        foreach (Action action in actions)
        {
            action();
        }
    }
}