using System;
using Godot;
using GodotPanelFramework.Utils;

namespace GDPanelFramework.Panels.Tweener;

/// <summary>
/// This is the default tweener that does not have any animated transition properties, it instantly hides and shows the panel.
/// </summary>
public class NonePanelTweener : IPanelTweener
{
    /// <summary>
    /// The unified instance of this <see cref="NonePanelTweener"/>.
    /// </summary>
    public static NonePanelTweener Instance => Singleton<NonePanelTweener>.GetInstance(() => new());
    
    private NonePanelTweener() { }

    /// <inheritdoc/>
    public void Init(Control panel) { }

    /// <inheritdoc/>
    public void Show(Control panel, Action? onFinish) => onFinish?.Invoke();

    /// <inheritdoc/>
    public void Hide(Control panel, Action? onFinish) => onFinish?.Invoke();
}