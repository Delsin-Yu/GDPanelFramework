using System;
using Godot;

namespace GDPanelFramework.Panels.Tweener;

/// <summary>
/// Defines the behavior for panel transitions.
/// </summary>
public interface IPanelTweener
{
    /// <summary>
    /// This sets the default visual appearance for a panel.
    /// </summary>
    /// <param name="panel">The target panel.</param>
    void Init(Control panel);
    
    /// <summary>
    /// This async method manages the behavior when the panel is showing up.
    /// </summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="onFinish">Called by the method when the behavior is considered finished, or not be called at all if the behaviour is interrupted</param>
    void Show(Control panel, Action? onFinish);
    
    /// <summary>
    /// This async method manages the behavior when the panel is hiding out.
    /// </summary>
    /// <param name="panel">The target panel.</param>
    /// <param name="onFinish">Called by the method when the behavior is considered finished, or not be called at all if the behaviour is interrupted</param>
    void Hide(Control panel, Action? onFinish);
}