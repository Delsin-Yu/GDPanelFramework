using System;
using GDPanelSystem.Core.Panels;
using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests.Tests;

public partial class Test0_NonParamUIPanel : UIPanel
{
    protected override void _OnPanelInitialize()
    {
        GD.Print("NonParamPanel:Initialize");
    }

    protected override void _OnPanelOpen()
    {
        GD.Print("NonParamPanel:Open");
        GDTask
            .Delay(TimeSpan.FromSeconds(1))
            .ContinueWith(ClosePanel)
            .Forget();
    }

    protected override void _OnPanelClose()
    {
        GD.Print("NonParamPanel:Close");
    }
}