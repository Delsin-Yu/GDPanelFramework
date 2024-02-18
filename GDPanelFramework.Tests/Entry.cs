using GDPanelSystem.Core;
using GDPanelSystem.Core.Panels.Tweener;
using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests;

public partial class Entry : Control
{
    [Export] private TestModule[] _testModules;

    public override void _Ready() => RunTests().Forget();

    private async GDTaskVoid RunTests()
    {
        PanelManager.DefaultPanelTweener = new FadePanelTweener { FadeTime = 1f };

        foreach (var testModule in _testModules)
        {
            await testModule.Run();
        }
    }
}