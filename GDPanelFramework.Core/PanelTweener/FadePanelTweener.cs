using System;
using System.Collections.Generic;
using Godot;
using GodotPanelFramework.Utils;

namespace GDPanelSystem.Core.Panels.Tweener;

public class FadePanelTweener : IPanelTweener
{
    private readonly Dictionary<Control, Tween> _activeTween = new();

    private static readonly NodePath _modulatePath = new(Control.PropertyName.Modulate);

    public float FadeTime { get; set; }

    private Tween KillAndCreateNewTween(Control panel, in Color color, Action? onFinish)
    {
        if (_activeTween.TryGetValue(panel, out var runningTween))
        {
            runningTween.Kill();
        }

        runningTween = panel.CreateTween();
        runningTween.TweenProperty(panel, _modulatePath, color, FadeTime);
        runningTween.TweenCallback(
            Callable.From(
                () =>
                {
                    onFinish?.Invoke();
                    if (!_activeTween.Remove(panel, out var tween)) return;
                    tween.Kill();
                }
            )
        );
        return runningTween;
    }
    
    /// <inheritdoc/>
    public void Show(Control panel, Action? onFinish)
    {
        panel.Modulate = Colors.Transparent;
        KillAndCreateNewTween(panel, Colors.White, onFinish);
    }

    /// <inheritdoc/>
    public void Hide(Control panel, Action? onFinish)
    {
        panel.Modulate = Colors.White;
        KillAndCreateNewTween(panel, Colors.Transparent, onFinish);
    }
}