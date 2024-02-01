using System.Dynamic;
using GDPanelSystem.Core;
using GDPanelSystem.Core.Core;
using GDPanelSystem.Core.Panels;
using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Test;

public class Test
{
    [Export] private PackedScene _test;
    
    public async GDTask Run()
    {
        await _test
            .CreateOrGetPanel<TestPanel>()
            .OpenPanel()
            .InNewLayer(LayerVisual.Visible);

        var closeParam = await _test
            .CreateOrGetPanel<TestPanel2>()
            .OpenPanel(Empty.Default)
            .InCurrentLayer();
    }
}

public partial class TestPanel : UIPanel
{
    protected override void _OnPanelInitialize()
    {
        base._OnPanelInitialize();
        GD.Print("_OnPanelInitialize");
    }

    protected override void _OnPanelOpen()
    {
        
    }
}
public partial class TestPanel2 : UIPanelParam<Empty, int>
{
    protected override void _OnPanelInitialize()
    {
        base._OnPanelInitialize();
        GD.Print("_OnPanelInitialize");
    }

    protected override void _OnPanelOpen(Empty openParam)
    {
        
    }
}