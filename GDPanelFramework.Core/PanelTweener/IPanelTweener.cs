using System;
using Godot;

namespace GDPanelSystem.Core.Panels.Tweener;

public interface IPanelTweener
{
    void Show(Control panel, Action onFinish);
    void Hide(Control panel, Action onFinish);
}

public class FadePanelTweener : IPanelTweener
{
    public float FadeTime { get; set; }


    public void Show(Control panel, Action onFinish)
    {
        
    }

    public void Hide(Control panel, Action onFinish)
    {
        
    }
}

public class NonePanelTweener : IPanelTweener
{
    public void Show(Control panel, Action onFinish)
    {
        panel.Modulate = Colors.White;
        onFinish?.Invoke();
    }

    public void Hide(Control panel, Action onFinish)
    {
        panel.Modulate = Colors.Transparent;
        onFinish?.Invoke();
    }
}