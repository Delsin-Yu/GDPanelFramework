using GDPanelSystem.Core;
using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests.Tests;

public partial class Test2__CrossLayerPanel : TestModule
{
    [Export] private PackedScene _panel;

    public override async GDTask Run()
    {
        await _panel.CreatePanel<Test2_Panel>()
            .OpenPanel((0, _panel))
            .InCurrentLayer();
    }
}