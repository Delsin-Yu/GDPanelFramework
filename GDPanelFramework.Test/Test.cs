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
        
        _test
            .CreateOrGetPanel<TestPanel>()
            .OpenPanel()
            .InCurrentLayer();
        
        
        TestPanel instance = null!;
        var result = await instance .OpenPanelInternal();
        GD.Print(result.ToString());
    }
}

public partial class TestPanel : UIPanelBase<TestPanel>
{
    protected override void _OnPanelInitialize()
    {
        base._OnPanelInitialize();
        GD.Print("_OnPanelInitialize");
    }

    protected override void _OnPanelOpen()
    {
        base._OnPanelOpen();
        GD.Print("_OnPanelOpen");
    }

    protected override void _OnPanelClose()
    {
        base._OnPanelClose();
        GD.Print("_OnPanelClose");
    }
}