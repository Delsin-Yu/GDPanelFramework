using Godot;
using GodotTask.Tasks;

namespace GDPanelFramework.Tests;

public abstract partial class TestModule : Control
{
    public abstract GDTask Run();
}