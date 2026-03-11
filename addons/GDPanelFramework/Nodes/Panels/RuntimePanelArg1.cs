using System;
using System.Threading;
using Godot;

namespace GDPanelFramework.Panels;

internal sealed partial class RuntimePanelArg1<TOpenArg> : UIPanelArg1<TOpenArg>, IRuntimePanelArg1Handle<TOpenArg>
{
    private Control? _root;
    private Action<IRuntimePanelArg1Handle<TOpenArg>>? _onInitialized;
    private Action<IRuntimePanelArg1Handle<TOpenArg>>? _onOpen;
    private Action<IRuntimePanelArg1Handle<TOpenArg>>? _onClose;
    private Action<IRuntimePanelArg1Handle<TOpenArg>>? _onExternalClose;
    private Action<IRuntimePanelArg1Handle<TOpenArg>>? _onPredelete;
    private Action<IRuntimePanelArg1Handle<TOpenArg>, int>? _onNotification;

    internal override bool SupportsCacheReuse => false;

    internal void Configure(
        Control root,
        Action<IRuntimePanelArg1Handle<TOpenArg>>? onInitialized,
        Action<IRuntimePanelArg1Handle<TOpenArg>>? onOpen,
        Action<IRuntimePanelArg1Handle<TOpenArg>>? onClose,
        Action<IRuntimePanelArg1Handle<TOpenArg>>? onExternalClose,
        Action<IRuntimePanelArg1Handle<TOpenArg>>? onPredelete,
        Action<IRuntimePanelArg1Handle<TOpenArg>, int>? onNotification)
    {
        _root = root;
        _onInitialized = onInitialized;
        _onOpen = onOpen;
        _onClose = onClose;
        _onExternalClose = onExternalClose;
        _onPredelete = onPredelete;
        _onNotification = onNotification;
    }

    TOpenArg? IRuntimePanelArg1Handle<TOpenArg>.CurrentOpenArg => OpenArg;
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

    protected override void _OnPanelOpen(TOpenArg openArg) => _onOpen?.Invoke(this);
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