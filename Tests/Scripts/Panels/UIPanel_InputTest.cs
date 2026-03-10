using System;
using GDPanelFramework.Panels;
using GdUnit4;
using Godot;
using GodotPanelFramework;
using GodotTask;

namespace GDPanelFramework.Tests;

public partial class UIPanel_InputTest : UIPanel
{
    public class TestMonitor
    {
        public bool UIAcceptPressed { get; set; }
        public bool UIAcceptReleased { get; set; }
        public bool UICancelPressed { get; set; }
        public bool UICancelReleased { get; set; }
    }

    public TestMonitor? Monitor { get; set; }
    public ISceneRunner? SceneRunner { get; set; }


    protected override void _OnPanelOpen() => OnPanelOpenAsync().Forget();
    
    protected async GDTask OnPanelOpenAsync()
    {
        Monitor.NotNull();
        
        Action<InputEvent> call1 = _ => Monitor.UIAcceptPressed = !Monitor.UIAcceptPressed;
        Action<InputEvent> call2 = _ => Monitor.UIAcceptReleased = !Monitor.UIAcceptReleased;
        Action call3 = () => Monitor.UICancelPressed = !Monitor.UICancelPressed;
        Action call4 = () => Monitor.UICancelReleased = !Monitor.UICancelReleased;

        RegisterInput(BuiltinInputNames.UIAccept, call1, InputActionPhase.Pressed);
        RegisterInput(BuiltinInputNames.UIAccept, call2, InputActionPhase.Released);

        await Helpers.KeyPressedAsync(Key.Enter);

        RemoveInput(BuiltinInputNames.UIAccept, call1, InputActionPhase.Pressed);
        RemoveInput(BuiltinInputNames.UIAccept, call2, InputActionPhase.Released);

        await Helpers.KeyPressedAsync(Key.Enter);

        RegisterInputCancel(call3, InputActionPhase.Pressed);
        RegisterInputCancel(call4, InputActionPhase.Released);

        await Helpers.KeyPressedAsync(Key.Escape);

        RegisterInputCancel(call3, InputActionPhase.Pressed);
        RegisterInputCancel(call4, InputActionPhase.Released);

        await Helpers.KeyPressedAsync(Key.Escape);

        ClosePanel();
    }
}