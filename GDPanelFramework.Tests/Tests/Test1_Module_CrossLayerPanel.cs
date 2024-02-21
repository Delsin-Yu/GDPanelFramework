using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests;

public partial class Test1_Module_CrossLayerPanel : TestModule
{
    [Export] private PackedScene _panel;

    public override async GDTask Run()
    {
        await _panel.CreatePanel<Test1_Panel>()
            .OpenPanelAsync((0, _panel));
    }
}