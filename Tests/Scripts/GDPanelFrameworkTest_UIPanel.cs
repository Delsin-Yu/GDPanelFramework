using GdUnit4;
using System.Threading.Tasks;
using Godot;
using GodotTask;

namespace GDPanelFramework.Tests;

[TestSuite]
public class GDPanelFrameworkTest_UIPanel
{
    // [TestCase]
    // public static async Task WaitForDebugger()
    // {
    //     await GDTask.WaitUntil(() => System.Diagnostics.Debugger.IsAttached);
    // }

    private ISceneRunner? _sceneRunner;
    
    [BeforeTest]
    public void BeforeTest()
    {
        _sceneRunner = ISceneRunner.Load("res://Tests/test_entry.tscn", true).NotNull();
    }

    [TestCase]
    public static async Task UIPanel_Test_EventFunction()
    {
        await GDTask.NextFrame();

        var resource = GD.Load<PackedScene>("res://Tests/Prefabs/UIPanel_EventFunctionTest.tscn");

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

    [TestCase]
    public static async Task UIPanelArg_Test_EventFunction()
    {
        var resource = GD.Load<PackedScene>("res://Tests/Prefabs/UIPanelArg_EventFunctionTest.tscn");

        var monitor = new UIPanelArg_EventFunctionTest.TestMonitor();

        const int openArg = 10;

        var closeValue = await resource
            .CreatePanel<UIPanelArg_EventFunctionTest>(initializeCallback: panel => panel.Monitor = monitor)
            .OpenPanelAsync(openArg, closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.Initialized).IsTrue();
        Assertions.AssertThat(monitor.Opened).IsTrue();
        Assertions.AssertThat(monitor.Closed).IsTrue();
        Assertions.AssertThat(monitor.Predelete).IsTrue();
        Assertions.AssertThat(monitor.Notification).IsTrue();

        Assertions.AssertThat(monitor.OpenValue).IsEqual(openArg);
        Assertions.AssertThat(monitor.CloseValue).IsEqual(openArg * 2);
        Assertions.AssertThat(monitor.CloseValue).IsEqual(closeValue);
    }

    [TestCase]
    public static async Task UIPanel_Test_Token()
    {
        await GDTask.NextFrame();

        var resource = GD.Load<PackedScene>("res://Tests/Prefabs/UIPanel_TokenTest.tscn");

        var monitor = new UIPanel_TokenTest.TestMonitor();

        await resource
            .CreatePanel<UIPanel_TokenTest>(initializeCallback: panel => panel.Monitor = monitor)
            .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.PanelCloseTokenCanceled).IsTrue();
        Assertions.AssertThat(monitor.PanelCloseTweenFinishTokenCanceled).IsTrue();
        Assertions.AssertThat(monitor.PanelOpenTweenFinishTokenCanceled).IsTrue();
    }
    
    [TestCase]
    public async Task UIPanel_Test_Input()
    {
        await GDTask.NextFrame();

        var resource = GD.Load<PackedScene>("res://Tests/Prefabs/UIPanel_InputTest.tscn");

        var monitor = new UIPanel_InputTest.TestMonitor();

        await resource
            .CreatePanel<UIPanel_InputTest>(initializeCallback: panel =>
            {
                panel.Monitor = monitor;
                panel.SceneRunner = _sceneRunner;
            })
            .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.UIAcceptPressed).IsTrue();
        Assertions.AssertThat(monitor.UIAcceptReleased).IsTrue();
        Assertions.AssertThat(monitor.UICancelPressed).IsTrue();
        Assertions.AssertThat(monitor.UICancelReleased).IsTrue();
    }
    
    [TestCase]
    public async Task UIPanel_Test_Input_Composite()
    {
        await GDTask.NextFrame();

        var resource = GD.Load<PackedScene>("res://Tests/Prefabs/UIPanel_CompositeInputTest.tscn");

        var monitor = new UIPanel_CompositeInputTest.TestMonitor();

        await resource
            .CreatePanel<UIPanel_CompositeInputTest>(initializeCallback: panel =>
            {
                panel.Monitor = monitor;
                panel.SceneRunner = _sceneRunner;
            })
            .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.Composite_Axis_Started).IsTrue();
        Assertions.AssertThat(monitor.Composite_Axis_Negative_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Axis_Positive_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Axis_Ended).IsTrue();
        
        Assertions.AssertThat(monitor.Composite_Vector_Started).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Up_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Down_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Left_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Right_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Ended).IsTrue();
    }

    [TestCase]
    public async Task UIPanel_Test_TweenHide()
    {
        await GDTask.NextFrame();

        var resource = GD.Load<PackedScene>("res://Tests/Prefabs/UIPanel_TweenHideTest.tscn");

        var panelA = resource.CreatePanel<UIPanel_TweenHideTest>();
        panelA.OpenPanel(closePolicy: ClosePolicy.Delete);

        var panelB = resource.CreatePanel<UIPanel_TweenHideTest>();
        panelB.OpenPanel(PreviousPanelVisual.Hidden, ClosePolicy.Delete);
        
        Assertions.AssertThat(panelA.Visible).IsFalse();
        panelB.CloseExtern();
        Assertions.AssertThat(panelA.Visible).IsTrue();
        panelA.CloseExtern();
    }

    [TestCase]
    public async Task UIPanel_Test_Delete_Buffer()
    {
        await GDTask.NextFrame();
        var resource = GD.Load<PackedScene>("res://Tests/Prefabs/UIPanel_TweenHideTest.tscn");
        var panel = resource.CreatePanel<UIPanel_TweenHideTest>(CreatePolicy.ForceCreate);
        panel.OpenPanel(closePolicy: ClosePolicy.Cache);
        panel.CloseExtern();
        panel.QueueFree();
        panel = resource.CreatePanel<UIPanel_TweenHideTest>();
        panel.OpenPanel(closePolicy: ClosePolicy.Cache);
        panel.CloseExtern();
    }
}