using System;
using Godot;
using GodotPanelFramework.Utils;

namespace GDPanelSystem.Core.Panels.Tweener;

public class NonePanelTweener : IPanelTweener
{
    public static NonePanelTweener Instance => Singleton<NonePanelTweener>.GetInstance(() => new());
    
    private NonePanelTweener() { }

    /// <inheritdoc/>
    public void Show(Control panel, Action? onFinish) => onFinish?.Invoke();

    /// <inheritdoc/>
    public void Hide(Control panel, Action? onFinish) => onFinish?.Invoke();
}