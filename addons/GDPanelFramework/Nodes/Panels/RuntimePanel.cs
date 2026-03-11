using System;
using System.Threading;
using Godot;

namespace GDPanelFramework.Panels;

internal sealed partial class RuntimePanel : UIPanel, IRuntimePanelHandle
{
    private Control? _root;
    private Action<IRuntimePanelHandle>? _onInitialized;
    private Action<IRuntimePanelHandle>? _onOpen;
    private Action<IRuntimePanelHandle>? _onClose;
    private Action<IRuntimePanelHandle>? _onExternalClose;
    private Action<IRuntimePanelHandle>? _onPredelete;
    private Action<IRuntimePanelHandle, int>? _onNotification;

    internal override bool SupportsCacheReuse => false;

    internal void Configure(
        Control root,
        Action<IRuntimePanelHandle>? onInitialized,
        Action<IRuntimePanelHandle>? onOpen,
        Action<IRuntimePanelHandle>? onClose,
        Action<IRuntimePanelHandle>? onExternalClose,
        Action<IRuntimePanelHandle>? onPredelete,
        Action<IRuntimePanelHandle, int>? onNotification)
    {
        _root = root;
        _onInitialized = onInitialized;
        _onOpen = onOpen;
        _onClose = onClose;
        _onExternalClose = onExternalClose;
        _onPredelete = onPredelete;
        _onNotification = onNotification;
    }

    void IRuntimePanelHandle.Close() => ClosePanel();
    CancellationToken IRuntimePanelHandle.PanelCloseToken => PanelCloseToken;
    CancellationToken IRuntimePanelHandle.PanelOpenTweenFinishToken => PanelOpenTweenFinishToken;
    CancellationToken IRuntimePanelHandle.PanelCloseTweenFinishToken => PanelCloseTweenFinishToken;
    void IRuntimePanelHandle.EnableCancelToClose(InputActionPhase? actionPhase) => EnableCloseWithCancelKey(actionPhase);
    void IRuntimePanelHandle.DisableCancelToClose() => DisableCloseWithCancelKey();
    void IRuntimePanelInputScope.RegisterAnyKeyInput(Action<InputEvent> callback, InputActionPhase? actionPhase) => base.RegisterAnyKeyInput(callback, actionPhase);
    void IRuntimePanelInputScope.RegisterInput(StringName inputName, Action<InputEvent> callback, InputActionPhase? actionPhase) => base.RegisterInput(inputName, callback, actionPhase);
    void IRuntimePanelInputScope.RemoveInput(StringName inputName, Action<InputEvent> callback, InputActionPhase? actionPhase) => base.RemoveInput(inputName, callback, actionPhase);
    void IRuntimePanelInputScope.RegisterInputCancel(Action callback, InputActionPhase? actionPhase) => base.RegisterInputCancel(callback, actionPhase);
    void IRuntimePanelInputScope.RemoveInputCancel(Action callback, InputActionPhase? actionPhase) => base.RemoveInputCancel(callback, actionPhase);
    void IRuntimePanelInputScope.RegisterEchoedInput(StringName inputName, Action callback) => base.RegisterEchoedInput(inputName, callback);
    void IRuntimePanelInputScope.RemoveEchoedInput(StringName inputName, Action callback) => base.RemoveEchoedInput(inputName, callback);
    void IRuntimePanelInputScope.RegisterInputAxis(StringName negativeInputName, StringName positiveInputName, Action<float> callback, CompositeInputActionState actionState) => base.RegisterInputAxis(negativeInputName, positiveInputName, callback, actionState);
    void IRuntimePanelInputScope.RemoveInputAxis(StringName negativeInputName, StringName positiveInputName, Action<float> callback, CompositeInputActionState actionState) => base.RemoveInputAxis(negativeInputName, positiveInputName, callback, actionState);
    void IRuntimePanelInputScope.RegisterInputVector(StringName upInputName, StringName downInputName, StringName leftInputName, StringName rightInputName, Action<Vector2> callback, CompositeInputActionState actionState) => base.RegisterInputVector(upInputName, downInputName, leftInputName, rightInputName, callback, actionState);
    void IRuntimePanelInputScope.RemoveInputVector(StringName upInputName, StringName downInputName, StringName leftInputName, StringName rightInputName, Action<Vector2> callback, CompositeInputActionState actionState) => base.RemoveInputVector(upInputName, downInputName, leftInputName, rightInputName, callback, actionState);

    protected override void _OnPanelInitialize()
    {
        AttachRoot();
        _onInitialized?.Invoke(this);
    }

    protected override void _OnPanelOpen() => _onOpen?.Invoke(this);
    protected override void _OnPanelClose() => _onClose?.Invoke(this);
    protected override void _OnPanelExternalClose() => _onExternalClose?.Invoke(this);
    protected override void _OnPanelPredelete() => _onPredelete?.Invoke(this);
    protected override void _OnPanelNotification(int what) => _onNotification?.Invoke(this, what);

    private void AttachRoot()
    {
        if (_root is null) throw new InvalidOperationException("Runtime panel root is not configured.");
        if (_root.GetParent() == this) return;
        AddChild(_root);
        _root.CallDeferred(Control.MethodName.SetAnchorsAndOffsetsPreset, Variant.From(Control.LayoutPreset.FullRect));
    }
}