using System;
using Godot;

namespace GDPanelSystem.Core.Panels.Tweener;

public class NonePanelTweener : IPanelTweener
{
    /// <inheritdoc/>
    public void Show(Control panel, Action? onFinish) => onFinish?.Invoke();

    /// <inheritdoc/>
    public void Hide(Control panel, Action? onFinish) => onFinish?.Invoke();
}