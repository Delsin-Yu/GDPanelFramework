using System;
using Godot;

namespace GDPanelSystem.Core.Panels.Tweener;

public class NonePanelTweener : IPanelTweener
{
    public void Show(Control panel, Action? onFinish)
    {
        panel.Visible = true;
        onFinish?.Invoke();
    }

    public void Hide(Control panel, Action? onFinish)
    {
        panel.Visible = false;
        onFinish?.Invoke();
    }
}