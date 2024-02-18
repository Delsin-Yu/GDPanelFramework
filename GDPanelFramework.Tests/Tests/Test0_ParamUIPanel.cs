using GDPanelSystem.Core.Panels;
using Godot;

namespace GDPanelFramework.Tests.Tests;

public partial class Test0_ParamUIPanel : UIPanelParam<int, string>
{
    [Export] private Button _closeButton;
    
    protected override void _OnPanelInitialize()
    {
        GD.Print("ParamPanel:Initialize");
        _closeButton.Pressed += () => ClosePanel(OpenParam.ToString("D3"));
    }

    protected override void _OnPanelOpen(int param)
    {
        GD.Print($"ParamPanel:Open({param})");
    }

    protected override void _OnPanelClose(string param)
    {
        GD.Print($"ParamPanel:Close({param})");
    }
}