using System;
using GDPanelFramework.Panels;
using GdUnit4;
using Godot;
using GodotPanelFramework;
using GodotTask;

namespace GDPanelFramework.Tests;

public partial class UIPanel_CompositeInputTest : UIPanel
{
    public class TestMonitor
    {
        public bool Composite_Axis_Started { get; set; }
        public bool Composite_Axis_Negative_Updated { get; set; }
        public bool Composite_Axis_Positive_Updated { get; set; }
        public bool Composite_Axis_Ended { get; set; }

        public bool Composite_Vector_Started { get; set; }
        public bool Composite_Vector_Up_Updated { get; set; }
        public bool Composite_Vector_Down_Updated { get; set; }
        public bool Composite_Vector_Left_Updated { get; set; }
        public bool Composite_Vector_Right_Updated { get; set; }
        public bool Composite_Vector_Ended { get; set; }
    }

    public TestMonitor? Monitor { get; set; }
    public ISceneRunner? SceneRunner { get; set; }

    protected override void _OnPanelOpen() => _OnPanelOpenAsync().Forget();

    protected async GDTask _OnPanelOpenAsync()
    {
        Monitor.NotNull();

        Action<float> callAxisStarted = inputDirection =>
            Monitor.Composite_Axis_Started = !Monitor.Composite_Axis_Started;

        Action<float> callAxisUpdated = inputDirection =>
        {
            switch (inputDirection)
            {
                case < 0:
                    Monitor.Composite_Axis_Negative_Updated = !Monitor.Composite_Axis_Negative_Updated;
                    break;
                case > 0:
                    Monitor.Composite_Axis_Positive_Updated = !Monitor.Composite_Axis_Positive_Updated;
                    break;
            }
        };

        Action<float> callAxisEnded = inputDirection => Monitor.Composite_Axis_Ended = !Monitor.Composite_Axis_Ended;

        Action<Vector2> callVectorStarted = inputDirection =>
            Monitor.Composite_Vector_Started = !Monitor.Composite_Vector_Started;

        Action<Vector2> callVectorUpdated = inputDirection =>
        {
            if (inputDirection == Vector2.Up)
                Monitor.Composite_Vector_Up_Updated = !Monitor.Composite_Vector_Up_Updated;
            if (inputDirection == Vector2.Down)
                Monitor.Composite_Vector_Down_Updated = !Monitor.Composite_Vector_Down_Updated;
            if (inputDirection == Vector2.Left)
                Monitor.Composite_Vector_Left_Updated = !Monitor.Composite_Vector_Left_Updated;
            if (inputDirection == Vector2.Right)
                Monitor.Composite_Vector_Right_Updated = !Monitor.Composite_Vector_Right_Updated;
        };

        Action<Vector2> callVectorEnded = inputDirection =>
            Monitor.Composite_Vector_Ended = !Monitor.Composite_Vector_Ended;

        RegisterInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisStarted, CompositeInputActionState.Start);
        RegisterInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisUpdated, CompositeInputActionState.Update);
        RegisterInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisEnded, CompositeInputActionState.End);

        await Helpers.KeyPressAsync(Key.Left);
        await Helpers.KeyPressAsync(Key.Right);
        await Helpers.KeyReleaseAsync(Key.Left);
        await Helpers.KeyReleaseAsync(Key.Right);

        RemoveInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisStarted, CompositeInputActionState.Start);
        RemoveInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisUpdated, CompositeInputActionState.Update);
        RemoveInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisEnded, CompositeInputActionState.End);

        await Helpers.KeyPressAsync(Key.Left);
        await Helpers.KeyPressAsync(Key.Right);
        await Helpers.KeyReleaseAsync(Key.Left);
        await Helpers.KeyReleaseAsync(Key.Right);
        
        RegisterInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorStarted, CompositeInputActionState.Start);
        RegisterInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorUpdated, CompositeInputActionState.Update);
        RegisterInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorEnded, CompositeInputActionState.End);
        
        await Helpers.KeyPressAsync(Key.Up);
        await Helpers.KeyPressAsync(Key.Right);
        await Helpers.KeyReleaseAsync(Key.Up);
        await Helpers.KeyPressAsync(Key.Down);
        await Helpers.KeyReleaseAsync(Key.Right);
        await Helpers.KeyPressAsync(Key.Left);
        await Helpers.KeyReleaseAsync(Key.Down);
        await Helpers.KeyReleaseAsync(Key.Left);
        
        RemoveInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorStarted, CompositeInputActionState.Start);
        RemoveInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorUpdated, CompositeInputActionState.Update);
        RemoveInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorEnded, CompositeInputActionState.End);

        await Helpers.KeyPressAsync(Key.Up);
        await Helpers.KeyPressAsync(Key.Right);
        await Helpers.KeyReleaseAsync(Key.Up);
        await Helpers.KeyPressAsync(Key.Down);
        await Helpers.KeyReleaseAsync(Key.Right);
        await Helpers.KeyPressAsync(Key.Left);
        await Helpers.KeyReleaseAsync(Key.Down);
        await Helpers.KeyReleaseAsync(Key.Left);
        
        ClosePanel();
    }
}