using GDPanelFramework.Panels;
using Godot;

namespace GDPanelFramework.Tests;

public partial class Test1_SiblingPanel : UIPanelArg<string, Empty>
{
    [Export] private Label _label;
    
    protected override void _OnPanelOpen(string openArg)
    {
        _label.Text = openArg;
        GD.Print($"Opened: {openArg}");
    }

    protected override void _OnPanelClose(Empty closeArg)
    {
        base._OnPanelClose(closeArg);
        GD.Print($"Closed: {OpenArg}");
    }

    public void ClosePanelExtern() => ClosePanel(Empty.Default);
}