using System.Collections.Generic;
using GDPanelSystem.Core;
using GDPanelSystem.Core.Panels;
using Godot;

namespace GDPanelFramework.Tests.Tests;

public partial class Test1_PrimaryPanel : UIPanel
{
    [Export] private Button _closeButton;
    [Export] private Control _container;
    [Export] private PackedScene _siblingPanelPrefab;

    private readonly Stack<Test1_SiblingPanel> _sameLayerPanels = new();

    protected override void _OnPanelInitialize()
    {
        base._OnPanelInitialize();
        _closeButton.Pressed += ClosePanel;
    }

    protected override void _OnPanelOpen()
    {
        _closeButton.GrabFocus();
        PanelManager.PushPanelParent(this, _container);
        for (var i = 0; i < 20; i++)
        {
            var instance = _siblingPanelPrefab.CreatePanel<Test1_SiblingPanel>();
            
            instance
                .OpenPanel(i.ToString())
                .InCurrentLayer(ClosePolicy.Delete)
                .Start();
            
            _sameLayerPanels.Push(instance);
        }
    }

    protected override void _OnPanelClose()
    {
        base._OnPanelClose();
        while (_sameLayerPanels.TryPop(out var siblingPanel))
        {
            siblingPanel.ClosePanelExtern();
        }
        PanelManager.PopPanelParent(this);
    }
}