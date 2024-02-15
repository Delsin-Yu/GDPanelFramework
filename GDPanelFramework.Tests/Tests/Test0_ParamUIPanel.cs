using System;
using GDPanelSystem.Core.Panels;
using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests.Tests;

public partial class Test0_ParamUIPanel : UIPanelParam<int, string>
{
    protected override void _OnPanelInitialize()
    {
        GD.Print("ParamPanel:Initialize");
    }

    protected override void _OnPanelOpen(int param)
    {
        GD.Print($"ParamPanel:Open({param})");
        GDTask
            .Delay(TimeSpan.FromSeconds(1))
            .ContinueWith(() => ClosePanel(param.ToString()))
            .Forget();
    }

    protected override void _OnPanelClose(string param)
    {
        GD.Print($"ParamPanel:Close({param})");
    }
}