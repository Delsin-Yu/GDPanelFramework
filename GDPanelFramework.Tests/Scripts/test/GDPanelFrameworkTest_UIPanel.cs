using System;
using System.Diagnostics;
using GdUnit4;
using System.Threading.Tasks;
using GdUnit4.Exceptions;
using Godot;
using GodotTask;

namespace GDPanelFramework.Tests;

[TestSuite]
public class GDPanelFrameworkTest_UIPanel
{
    [TestCase]
    public static async Task UIPanel_EventFunction()
    {
        await GDTask.NextFrame();
        
        var resource = GD.Load<PackedScene>("res://Prefabs/UIPanel_EventFunctionTest.tscn");

        var monitor = new UIPanel_EventFunctionTest.TestMonitor();

        await resource
            .CreatePanel<UIPanel_EventFunctionTest>(initializeCallback: panel => panel.Monitor = monitor)
            .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();
        
        Assertions.AssertThat(monitor.Initialized).IsTrue();
        Assertions.AssertThat(monitor.Opened).IsTrue();
        Assertions.AssertThat(monitor.Closed).IsTrue();
        Assertions.AssertThat(monitor.Predelete).IsTrue();
        Assertions.AssertThat(monitor.Notification).IsTrue();
    }
}