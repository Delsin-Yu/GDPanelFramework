using System;
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
        while (true)
        {
            await _nonParamPanel
                .CreatePanel<Test0_NonParamUIPanel>()
                .OpenPanel()
                .InCurrentLayer(CachingPolicy.Delete);

            var result = await _paramPanel
                .CreatePanel<Test0_ParamUIPanel>()
                .OpenPanel(5)
                .InCurrentLayer(CachingPolicy.Delete);
        
            GD.Print(result);
        }
    }
}