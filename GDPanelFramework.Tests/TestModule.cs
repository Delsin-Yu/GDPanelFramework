using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests;

[GlobalClass]
public abstract partial class TestModule : Node
{
    public abstract GDTask Run();
}