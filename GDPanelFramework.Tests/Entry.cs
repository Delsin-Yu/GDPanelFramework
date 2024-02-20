using GDPanelFramework;
using GDPanelFramework.Panels.Tweener;
using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests;

public partial class Entry : Control
{
    [Export] private TestModule[] _testModules;

    public override void _Ready() => RunTests().Forget();

    private async GDTaskVoid RunTests()
    {
        await GDTask.NextFrame();
        
        PanelManager.DefaultPanelTweener = new FadePanelTweener { FadeTime = 0.1f };

        foreach (var testModule in _testModules)
        {
            await testModule.Run();
        }

        ((SceneTree)Engine.GetMainLoop()).Quit();
    }
}