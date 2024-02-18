using System;
using Godot;

namespace GDPanelSystem.Core.Panels.Tweener;

public class FadePanelTweener : IPanelTweener
{
    private static readonly NodePath _modulate = new(Control.PropertyName.Modulate);
    
    public float FadeTime { get; set; }

    /// <inheritdoc/>
    public void Show(Control panel, Action? onFinish)
    {
        panel
            .CreateTween()
            .TweenProperty(
                panel,
                _modulate,
                Colors.White,
                FadeTime
            );
    }

    /// <inheritdoc/>
    public void Hide(Control panel, Action? onFinish)
    {
        panel
            .CreateTween()
            .TweenProperty(
                panel,
                _modulate,
                Colors.Transparent,
                FadeTime
            );
    }
}