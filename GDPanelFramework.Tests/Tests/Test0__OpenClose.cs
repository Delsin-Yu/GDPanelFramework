using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests;

public partial class Test0__OpenClose : TestModule
{
    [Export(hintString: "*.tscn")] private PackedScene _nonParamPanel;
    [Export(hintString: "*.tscn")] private PackedScene _paramPanel;

    public override async GDTask Run()
    {
        await _nonParamPanel
            .CreatePanel<Test0_NonParamUIPanel>(CreatePolicy.ForceCreate)
            .OpenPanelAsync();

        var result = await _paramPanel
            .CreatePanel<Test0_ParamUIPanel>()
            .OpenPanelAsync(5);

        GD.Print(result);
    }
}