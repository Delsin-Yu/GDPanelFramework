using GDPanelFramework.Panels;
using Godot;

namespace GDPanelFramework.Tests;

public partial class Test2_Panel : UIPanelArg<(int, PackedScene), Empty>
{
    [Export] private Button _openNewButton;
    [Export] private Button _closeSelfButton;
    [Export] private Label _label;
    [Export] private Vector2 _newPanelOffset;

    protected override void _OnPanelInitialize()
    {
        base._OnPanelInitialize();
        _closeSelfButton.Pressed += Close;
        _openNewButton.Pressed += () =>
        {
            OpenArg.Item2
                .CreatePanel<Test2_Panel>(initializeCallback: x => x.Position = Position + _newPanelOffset)
                .OpenPanel((OpenArg.Item1 + 1, OpenArg.Item2))
                .InNewLayer(LayerVisual.Visible, ClosePolicy.Delete)
                .Start();
        };
    }

    private void Close() => ClosePanel(Empty.Default);

    protected override void _OnPanelOpen((int, PackedScene) openArg)
    {
        if (openArg.Item1 != 0) RegisterCancelInput(Close);
        _openNewButton.GrabFocus();
        _label.Text = openArg.Item1.ToString("D2");
    }
}