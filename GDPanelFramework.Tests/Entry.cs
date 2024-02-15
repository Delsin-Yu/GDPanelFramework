using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests;

public partial class Entry : Control
{
	[Export] private TestModule[] _testModules;
	
	public override void _Ready()
	{
		RunTests().Forget();
	}

	private async GDTaskVoid RunTests()
	{
		foreach (var testModule in _testModules)
		{
			await testModule.Run();
		}
	}
}