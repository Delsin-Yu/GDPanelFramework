using System;
using System.Threading;
using Godot;

namespace GDPanelFramework.Panels;

internal sealed partial class RuntimePanelArg2<TOpenArg, TCloseArg> : UIPanelArg2<TOpenArg, TCloseArg>, IRuntimePanelArg2Handle<TOpenArg, TCloseArg>
{
    private Control? _root;
    private Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? _onInitialized;
    private Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? _onOpen;
    private Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? _onClose;
    private Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? _onExternalClose;
    private Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? _onPredelete;
    private Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>, int>? _onNotification;

    internal override bool SupportsCacheReuse => false;

    internal void Configure(
        Control root,
        Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? onInitialized,
        Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? onOpen,
        Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? onClose,
        Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? onExternalClose,
        Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? onPredelete,
        Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>, int>? onNotification)
    {
        _root = root;
        _onInitialized = onInitialized;
        _onOpen = onOpen;
        _onClose = onClose;
        _onExternalClose = onExternalClose;
        _onPredelete = onPredelete;
        _onNotification = onNotification;
    }

    TOpenArg? IRuntimePanelArg2Handle<TOpenArg, TCloseArg>.CurrentOpenArg => OpenArg;
    CancellationToken IRuntimePanelArg2Handle<TOpenArg, TCloseArg>.PanelCloseToken => PanelCloseToken;
    CancellationToken IRuntimePanelArg2Handle<TOpenArg, TCloseArg>.PanelOpenTweenFinishToken => PanelOpenTweenFinishToken;
    CancellationToken IRuntimePanelArg2Handle<TOpenArg, TCloseArg>.PanelCloseTweenFinishToken => PanelCloseTweenFinishToken;
    void IRuntimePanelArg2Handle<TOpenArg, TCloseArg>.Close(TCloseArg closeArg) => ClosePanel(closeArg);
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
    protected override void _OnPanelClose(TCloseArg closeArg) => _onClose?.Invoke(this);
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