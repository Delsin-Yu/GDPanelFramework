using GDPanelSystem.Core;
using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests.Tests;

public partial class Test1__SameLayerPanel : TestModule
{
    [Export] private PackedScene _primaryPanel;

    public override async GDTask Run()
    {
        await _primaryPanel
            .CreatePanel<Test1_PrimaryPanel>()
            .OpenPanel()
            .InCurrentLayer();
        
        await _primaryPanel
            .CreatePanel<Test1_PrimaryPanel>()
            .OpenPanel()
            .InCurrentLayer(ClosePolicy.Delete);
    }
}