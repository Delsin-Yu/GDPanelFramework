using System;
using System.Collections.Generic;
using Godot;

namespace GDPanelFramework.Panels.Tweener;

/// <summary>
/// Fades panels between solid and transparent for transitions.
/// </summary>
public class FadePanelTweener : IPanelTweener
{
    private readonly Dictionary<Control, Tween> _activeTween = new();

    private static readonly NodePath ModulatePath = new(Control.PropertyName.Modulate);

    /// <summary>
    /// The duration for fading.
    /// </summary>
    public float FadeTime { get; set; } = 0.1f;

    private void KillAndCreateNewTween(Control panel, in Color color, Action? onFinish, string methodName)
    {
        if (_activeTween.TryGetValue(panel, out var runningTween))
        {
            runningTween.Kill();
            runningTween.Dispose();
        }

        runningTween = panel.CreateTween();
        _activeTween[panel] = runningTween;

        runningTween
            .TweenProperty(panel, ModulatePath, color, FadeTime)
            .Dispose();
        runningTween
            .TweenCallback(
                Callable.From(
                    () =>
                    {
                        if (onFinish != null) DelegateRunner.RunProtected(onFinish, "OnFinish", panel.Name, methodName);
                        if (!_activeTween.Remove(panel, out var tween)) return;
                        tween.Kill();
                    }
                )
            )
            .Dispose();
    }

    /// <inheritdoc/>
    public void Init(Control panel) => 
        panel.Modulate = Colors.Transparent;

    /// <inheritdoc/>
    public void Show(Control panel, Action? onFinish) => 
        KillAndCreateNewTween(panel, Colors.White, onFinish, "Show");

    /// <inheritdoc/>
    public void Hide(Control panel, Action? onFinish) => 
        KillAndCreateNewTween(panel, Colors.Transparent, onFinish, "Hide");
}