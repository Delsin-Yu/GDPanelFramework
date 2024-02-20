using GDPanelSystem.Core.Panels;
using Godot;

namespace GDPanelFramework.Tests.Tests;

public partial class Test0_NonParamUIPanel : UIPanel
{
    [Export] private Button _closeButton;
    
    protected override void _OnPanelInitialize()
    {
        GD.Print("NonParamPanel:Initialize");
        _closeButton.Pressed += ClosePanel;
        EnableCloseWithCancelKey();
    }

    protected override void _OnPanelOpen()
    {
        GD.Print("NonParamPanel:_OnPanelOpen");
        _closeButton.GrabFocus();
    }

    protected override void _OnPanelClose()
    {
        GD.Print("NonParamPanel:Close");
    }
}