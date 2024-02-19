using System;
using Godot;

namespace GDPanelSystem.Core.Panels.Tweener;

public interface IPanelTweener
{
    void Init(Control panel);
    void Show(Control panel, Action? onFinish);
    void Hide(Control panel, Action? onFinish);
}