using GDPanelSystem.Core.Panels;
using Godot;

namespace GDPanelFramework.Tests.Tests;

public partial class Test0_ParamUIPanel : UIPanelArg<int, string>
{
    [Export] private Button _closeButton;
    
    protected override void _OnPanelInitialize()
    {
        GD.Print("ParamPanel:Initialize");
        _closeButton.Pressed += () => ClosePanel(OpenArg.ToString("D3"));
        RegisterCancelInput(() => ClosePanel("D4"));
    }

    protected override void _OnPanelOpen(int param)
    {
        GD.Print($"ParamPanel:Open({param})");
        _closeButton.GrabFocus();
    }

    protected override void _OnPanelClose(string param)
    {
        GD.Print($"ParamPanel:Close({param})");
    }
}