using System.Runtime.InteropServices;
using GDPanelSystem.Core;
using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests.Tests;

public partial class Test0__OpenClose : TestModule
{
    [Export(hintString: "*.tscn")] private PackedScene _nonParamPanel;
    [Export(hintString: "*.tscn")] private PackedScene _paramPanel;

    public override async GDTask Run()
    {
        await _nonParamPanel
            .GetOrCreatePanel<Test0_NonParamUIPanel>()
            .OpenPanel()
            .InCurrentLayer();

        var result = await _paramPanel
            .GetOrCreatePanel<Test0_ParamUIPanel>()
            .OpenPanel(5)
            .InCurrentLayer();
        
        GD.Print(result);
    }
}