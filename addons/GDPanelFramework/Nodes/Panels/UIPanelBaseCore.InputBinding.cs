using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GDPanelFramework.Utils.Pooling;
using Godot;
using GodotPanelFramework;

namespace GDPanelFramework.Panels;

public abstract partial class UIPanelBaseCore
{
    private readonly List<StringName> _registeredInputEventNames = [];
    private readonly Dictionary<StringName, RegisteredInputEvent> _registeredInputEvent = new();
    private readonly RegisteredInputEvent _registeredAnyKeyInputEvent = new();
    private readonly Dictionary<Action, Action<InputEvent>> _mappedCancelEvent = new();
    private readonly Dictionary<InputAxisBinding, MappedInputAxis> _mappedInputAxis = new();
    private readonly Dictionary<InputVectorBinding, MappedInputVector> _mappedInputVector = new();
    private readonly HashSet<RegisteredInputEvent> _pressedInputEvents = new();

    private protected void CancelPressedInput()
    {
        var name = LocalName;
        var inputEventAction = new InputEventAction();
        foreach (var inputEvent in _pressedInputEvents)
            inputEvent.Call(inputEventAction, InputActionPhase.Released, name);
    }

    internal bool ProcessPanelInput(ref readonly PanelManager.CachedInputEvent inputEvent)
    {
        var name = LocalName;
        var executionQueue = Pool.Get<Queue<RegisteredInputEvent>>(() => new());

        try
        {
            if (!_registeredAnyKeyInputEvent.Empty)
                if (inputEvent.Event is InputEventJoypadButton or InputEventKey or InputEventMouseButton or InputEventScreenTouch)
                    executionQueue.Enqueue(_registeredAnyKeyInputEvent);

            foreach (var inputEventName in CollectionsMarshal.AsSpan(_registeredInputEventNames))
            {
                if (!inputEvent.ActionHasEventCached(inputEventName)) continue;
                executionQueue.Enqueue(_registeredInputEvent[inputEventName]);
            }

            if (executionQueue.Count == 0) return false;

            var currentPhase = inputEvent.Phase;

            var called = false;

            switch (currentPhase)
            {
                case InputActionPhase.Pressed:
                    while (executionQueue.TryDequeue(out var call))
                    {
                        var localCalled = call.Call(inputEvent.Event, currentPhase, name);
                        if (!localCalled) continue;
                        called = true;
                        _pressedInputEvents.Add(call);
                    }

                    break;
                case InputActionPhase.Released:
                    while (executionQueue.TryDequeue(out var call))
                    {
                        var localCalled = call.Call(inputEvent.Event, currentPhase, name);
                        if (!localCalled) continue;
                        called = true;
                        _pressedInputEvents.Remove(call);
                    }

                    break;
                default:
                    throw new InvalidOperationException();
            }

            return called;
        }
        finally { Pool.Collect(executionQueue); }
    }

    /// <summary>
    /// Register a <paramref name="callback"/> to any key input for this panel when it's active.
    /// </summary>
    /// <param name="callback">The callback for receiving input command.</param>
    /// <param name="actionPhase">The action phase this callback registers to.</param>
    protected void RegisterAnyKeyInput(Action<InputEvent> callback, InputActionPhase actionPhase = InputActionPhase.Released)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _registeredAnyKeyInputEvent.RegisterCall(callback, actionPhase);
    }

    /// <summary>
    /// Register a <paramref name="callback"/> that receives toggle state (pressed/released) for the associated <paramref name="inputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="inputName">The input name to associate to.</param>
    /// <param name="callback">The callback for receiving the toggle state (true when pressed, false when released).</param>
    protected void RegisterInputToggle(StringName inputName, Action<bool> callback)
    {
        RegisterInput(inputName, _ => callback(true), InputActionPhase.Pressed);
        RegisterInput(inputName, _ => callback(false));
    }

    /// <summary>
    /// Register a <paramref name="callback"/> that receives toggle state (pressed/released) for any of the associated <paramref name="inputNames"/> for this panel when it's active.
    /// The callback is invoked with true if any of the inputs is pressed, and false when all inputs are released.
    /// </summary>
    /// <param name="inputNames">The input names to associate to.</param>
    /// <param name="callback">The callback for receiving the combined toggle state.</param>
    protected void RegisterInputToggle(ReadOnlySpan<StringName> inputNames, Action<bool> callback)
    {
        var pressedArray = new bool[inputNames.Length];

        for (var index = 0; index < inputNames.Length; index++)
        {
            var inputName = inputNames[index];
            var localIndex = index;
            RegisterInputToggle(
                inputName,
                pressed =>
                {
                    pressedArray[localIndex] = pressed;
                    callback(pressedArray.Contains(true));
                }
            );
        }
    }

    /// <summary>
    /// Register a <paramref name="callback"/> to the associated <paramref name="inputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="inputName">The input name to associate to.</param>
    /// <param name="callback">The callback for receiving input command.</param>
    /// <param name="actionPhase">The action phase this callback registers to.</param>
    protected void RegisterInput(StringName inputName, Action<InputEvent> callback, InputActionPhase actionPhase = InputActionPhase.Released)
    {
        ArgumentNullException.ThrowIfNull(inputName);
        ArgumentNullException.ThrowIfNull(callback);

        if (!_registeredInputEvent.TryGetValue(inputName, out var registeredInputEvent))
        {
            registeredInputEvent = Pool.Get<RegisteredInputEvent>(() => new());
            _registeredInputEvent.Add(inputName, registeredInputEvent);
            if (!_registeredInputEventNames.Contains(inputName)) _registeredInputEventNames.Add(inputName);
        }

        registeredInputEvent.RegisterCall(callback, actionPhase);
    }

    /// <summary>
    /// Register a <paramref name="callback"/> to multiple associated <paramref name="inputNames"/> for this panel when it's active.
    /// </summary>
    /// <param name="inputNames">The input names to associate to.</param>
    /// <param name="callback">The callback for receiving input command.</param>
    /// <param name="actionPhase">The action phase this callback registers to.</param>
    protected void RegisterInput(ReadOnlySpan<StringName> inputNames, Action<InputEvent> callback, InputActionPhase actionPhase = InputActionPhase.Released)
    {
        foreach (var inputName in inputNames)
            RegisterInput(inputName, callback, actionPhase);
    }

    /// <summary>
    /// Remove a <paramref name="callback"/> registration from the associated <paramref name="inputName"/> for this panel.
    /// </summary>
    /// <param name="inputName">The input name to remove from.</param>
    /// <param name="callback">The callback to remove.</param>
    /// <param name="actionPhase">The action phase this callback registered to.</param>
    protected void RemoveInput(StringName inputName, Action<InputEvent> callback, InputActionPhase actionPhase = InputActionPhase.Released)
    {
        ArgumentNullException.ThrowIfNull(inputName);
        ArgumentNullException.ThrowIfNull(callback);
        if (!_registeredInputEvent.TryGetValue(inputName, out var registeredInputEvent)) return;
        registeredInputEvent.RemoveCall(callback, actionPhase);
        if (!registeredInputEvent.Empty) return;
        _registeredInputEvent.Remove(inputName);
        _registeredInputEventNames.Remove(inputName);
        Pool.Collect(registeredInputEvent);
    }

    /// <summary>
    /// Register or remove a <paramref name="callback"/> registration from the associated <paramref name="inputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="enable">When setting to true, calls <see cref="RegisterInput(StringName,Action{InputEvent},InputActionPhase)"/>, otherwise calls <see cref="RemoveInput"/></param>
    /// <param name="inputName">The input name to associate to or remove from.</param>
    /// <param name="callback">The callback for receiving or stops receiving input command.</param>
    /// <param name="actionPhase">The action phase this callback registers to.</param>
    protected void ToggleInput(bool enable, StringName inputName, Action<InputEvent> callback, InputActionPhase actionPhase = InputActionPhase.Released)
    {
        if (enable) RegisterInput(inputName, callback, actionPhase);
        else RemoveInput(inputName, callback, actionPhase);
    }

    /// <summary>
    /// Register a <paramref name="callback"/> to the associated <see cref="PanelManager.UICancelActionName"/> for this panel when it's active.
    /// </summary>
    /// <param name="callback">The callback for receiving input command.</param>
    /// <param name="actionPhase">The action phase this callback registers to.</param>
    protected void RegisterInputCancel(Action callback, InputActionPhase actionPhase = InputActionPhase.Released)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (!_mappedCancelEvent.TryGetValue(callback, out var mappedCallback))
        {
            mappedCallback = _ => callback();
            _mappedCancelEvent.Add(callback, mappedCallback);
        }

        RegisterInput(PanelManager.UICancelActionName, mappedCallback, actionPhase);
    }

    /// <summary>
    /// Remove a <paramref name="callback"/> registration from the associated <see cref="PanelManager.UICancelActionName"/> for this panel.
    /// </summary>
    /// <param name="callback">The callback to remove.</param>
    /// <param name="actionPhase">The action phase this callback registered to.</param>
    protected void RemoveInputCancel(Action callback, InputActionPhase actionPhase = InputActionPhase.Released)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (!_mappedCancelEvent.Remove(callback, out var mappedCallback)) return;
        RemoveInput(PanelManager.UICancelActionName, mappedCallback, actionPhase);
    }

    /// <summary>
    /// Register or remove a <paramref name="callback"/> registration from the associated <see cref="PanelManager.UICancelActionName"/> for this panel when it's active.
    /// </summary>
    /// <param name="enable">When setting to true, calls <see cref="RegisterInputCancel"/>, otherwise calls <see cref="RemoveInputCancel"/></param>
    /// <param name="callback">The callback for receiving or stops receiving input command.</param>
    /// <param name="actionPhase">The action phase this callback registers to.</param>
    protected void ToggleInputCancel(bool enable, Action callback, InputActionPhase actionPhase = InputActionPhase.Released)
    {
        if (enable) RegisterInputCancel(callback, actionPhase);
        else RemoveInputCancel(callback, actionPhase);
    }

    /// <summary>
    /// Register a <paramref name="callback"/> to the composite axis of the <paramref name="negativeInputName"/> and <paramref name="positiveInputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="negativeInputName">The input name that represents the negative axis to associate to.</param>
    /// <param name="positiveInputName">The input name that represents the positive axis to associate to.</param>
    /// <param name="callback">The callback for receiving the composite input command.</param>
    /// <param name="actionState">The action state this callback registers to.</param>
    protected void RegisterInputAxis(StringName negativeInputName, StringName positiveInputName, Action<float> callback, CompositeInputActionState actionState)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var binding = new InputAxisBinding(negativeInputName, positiveInputName);

        if (!_mappedInputAxis.TryGetValue(binding, out var mappedInputAxis))
        {
            var negativeDeadZone = InputMap.ActionGetDeadzone(negativeInputName);
            var positiveDeadZone = InputMap.ActionGetDeadzone(positiveInputName);
            mappedInputAxis = new(LocalName, (negativeDeadZone + positiveDeadZone) / 2f);
            _mappedInputAxis.Add(binding, mappedInputAxis);

            RegisterInput(negativeInputName, mappedInputAxis.NegativeInputActionUpdate, InputActionPhase.Any);
            RegisterInput(positiveInputName, mappedInputAxis.PositiveInputActionUpdate, InputActionPhase.Any);
        }

        switch (actionState)
        {
            case CompositeInputActionState.Start:
                mappedInputAxis.OnStart += callback;
                break;
            case CompositeInputActionState.Update:
                mappedInputAxis.OnUpdate += callback;
                break;
            case CompositeInputActionState.End:
                mappedInputAxis.OnEnd += callback;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionState), actionState, null);
        }
    }

    /// <summary>
    /// Removes a <paramref name="callback"/> registration from the composite axis of the <paramref name="negativeInputName"/> and <paramref name="positiveInputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="negativeInputName">The input name that represents the negative axis to remove from.</param>
    /// <param name="positiveInputName">The input name that represents the positive axis to remove from.</param>
    /// <param name="callback">The callback to remove.</param>
    /// <param name="actionState">The action state this callback registered to.</param>
    protected void RemoveInputAxis(StringName negativeInputName, StringName positiveInputName, Action<float> callback, CompositeInputActionState actionState)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var binding = new InputAxisBinding(negativeInputName, positiveInputName);

        if (!_mappedInputAxis.TryGetValue(binding, out var mappedInputAxis)) return;

        switch (actionState)
        {
            case CompositeInputActionState.Start:
                mappedInputAxis.OnStart -= callback;
                break;
            case CompositeInputActionState.Update:
                mappedInputAxis.OnUpdate -= callback;
                break;
            case CompositeInputActionState.End:
                mappedInputAxis.OnEnd -= callback;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionState), actionState, null);
        }

        if (mappedInputAxis.IsEmpty) _mappedInputAxis.Remove(binding);

        RemoveInput(negativeInputName, mappedInputAxis.NegativeInputActionUpdate, InputActionPhase.Any);
        RemoveInput(positiveInputName, mappedInputAxis.PositiveInputActionUpdate, InputActionPhase.Any);
    }

    /// <summary>
    /// Register or removes a <paramref name="callback"/> registration from the composite axis of the <paramref name="negativeInputName"/> and <paramref name="positiveInputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="enable">When setting to true, calls <see cref="RegisterInputAxis"/>, otherwise calls <see cref="RemoveInputAxis"/></param>
    /// <param name="negativeInputName">The input name that represents the negative axis to associate to or remove from.</param>
    /// <param name="positiveInputName">The input name that represents the positive axis to associate to or remove from.</param>
    /// <param name="callback">The callback for receiving or stops receiving the composite input command.</param>
    /// <param name="actionState">The action state this callback registers to.</param>
    protected void ToggleInputAxis(bool enable, StringName negativeInputName, StringName positiveInputName, Action<float> callback, CompositeInputActionState actionState)
    {
        if (enable) RegisterInputAxis(negativeInputName, positiveInputName, callback, actionState);
        else RemoveInputAxis(negativeInputName, positiveInputName, callback, actionState);
    }

    /// <summary>
    /// Register a <paramref name="callback"/> to the composite vector (2 axis) of the <paramref name="upInputName"/>, <paramref name="downInputName"/>, <paramref name="leftInputName"/>, and <paramref name="rightInputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="upInputName">The input name that represents the positive vertical axis (Y+) to associate to.</param>
    /// <param name="downInputName">The input name that represents the negative vertical axis (Y-) to associate to.</param>
    /// <param name="rightInputName">The input name that represents the positive horizontal axis (X+) to associate to.</param>
    /// <param name="leftInputName">The input name that represents the negative horizontal axis (X-) to associate to.</param>
    /// <param name="callback">The callback for receiving input command.</param>
    /// <param name="actionState">The action state this callback registers to.</param>
    protected void RegisterInputVector(StringName upInputName, StringName downInputName, StringName leftInputName, StringName rightInputName, Action<Vector2> callback, CompositeInputActionState actionState)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var binding = new InputVectorBinding(new(downInputName, upInputName), new(leftInputName, rightInputName));

        if (!_mappedInputVector.TryGetValue(binding, out var mappedInputVector2))
        {
            var upDeadZone = InputMap.ActionGetDeadzone(upInputName);
            var downDeadZone = InputMap.ActionGetDeadzone(downInputName);
            var leftDeadZone = InputMap.ActionGetDeadzone(leftInputName);
            var rightDeadZone = InputMap.ActionGetDeadzone(rightInputName);
            mappedInputVector2 = new(LocalName, (upDeadZone + downDeadZone + leftDeadZone + rightDeadZone) / 4f);
            _mappedInputVector.Add(binding, mappedInputVector2);

            RegisterInput(upInputName, mappedInputVector2.VerticalPositiveInputActionUpdate, InputActionPhase.Any);
            RegisterInput(downInputName, mappedInputVector2.VerticalNegativeInputActionUpdate, InputActionPhase.Any);
            RegisterInput(rightInputName, mappedInputVector2.HorizontalPositiveInputActionUpdate, InputActionPhase.Any);
            RegisterInput(leftInputName, mappedInputVector2.HorizontalNegativeInputActionUpdate, InputActionPhase.Any);
        }

        switch (actionState)
        {
            case CompositeInputActionState.Start:
                mappedInputVector2.OnStart += callback;
                break;
            case CompositeInputActionState.Update:
                mappedInputVector2.OnUpdate += callback;
                break;
            case CompositeInputActionState.End:
                mappedInputVector2.OnEnd += callback;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionState), actionState, null);
        }
    }

    /// <summary>
    /// Remove a <paramref name="callback"/> registration from the composite vector (2 axis) of the <paramref name="upInputName"/>, <paramref name="downInputName"/>, <paramref name="leftInputName"/>, and <paramref name="rightInputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="upInputName">The input name that represents the positive vertical axis (Y+) to remove from.</param>
    /// <param name="downInputName">The input name that represents the negative vertical axis (Y-) to remove from.</param>
    /// <param name="rightInputName">The input name that represents the positive horizontal axis (X+) to remove from.</param>
    /// <param name="leftInputName">The input name that represents the negative horizontal axis (X-) to remove from.</param>
    /// <param name="callback">The callback to remove.</param>
    /// <param name="actionState">The action state this callback registered to.</param>
    protected void RemoveInputVector(StringName upInputName, StringName downInputName, StringName leftInputName, StringName rightInputName, Action<Vector2> callback, CompositeInputActionState actionState)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var binding = new InputVectorBinding(new(downInputName, upInputName), new(leftInputName, rightInputName));

        if (!_mappedInputVector.TryGetValue(binding, out var mappedInputVector2)) return;

        switch (actionState)
        {
            case CompositeInputActionState.Start:
                mappedInputVector2.OnStart -= callback;
                break;
            case CompositeInputActionState.Update:
                mappedInputVector2.OnUpdate -= callback;
                break;
            case CompositeInputActionState.End:
                mappedInputVector2.OnEnd -= callback;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionState), actionState, null);
        }

        if (mappedInputVector2.IsEmpty) _mappedInputVector.Remove(binding);

        RemoveInput(upInputName, mappedInputVector2.VerticalPositiveInputActionUpdate, InputActionPhase.Any);
        RemoveInput(downInputName, mappedInputVector2.VerticalNegativeInputActionUpdate, InputActionPhase.Any);
        RemoveInput(rightInputName, mappedInputVector2.HorizontalPositiveInputActionUpdate, InputActionPhase.Any);
        RemoveInput(leftInputName, mappedInputVector2.HorizontalNegativeInputActionUpdate, InputActionPhase.Any);
    }

    /// <summary>
    /// Register or removes a <paramref name="callback"/> registration from the composite vector (2 axis) of the <paramref name="upInputName"/>, <paramref name="downInputName"/>, <paramref name="leftInputName"/>, and <paramref name="rightInputName"/> for this panel when it's active.
    /// </summary>
    /// <param name="enable">When setting to true, calls <see cref="RegisterInputVector"/>, otherwise calls <see cref="RemoveInputVector"/></param>
    /// <param name="upInputName">The input name that represents the positive vertical axis (Y+) to associate to or remove from.</param>
    /// <param name="downInputName">The input name that represents the negative vertical axis (Y-) to associate to or remove from.</param>
    /// <param name="rightInputName">The input name that represents the positive horizontal axis (X+) to associate to or remove from.</param>
    /// <param name="leftInputName">The input name that represents the negative horizontal axis (X-) to associate to or remove from.</param>
    /// <param name="callback">The callback for receiving or stops receiving the composite input command.</param>
    /// <param name="actionState">The action state this callback registers to.</param>
    protected void ToggleInputVector(bool enable, StringName upInputName, StringName downInputName, StringName leftInputName, StringName rightInputName, Action<Vector2> callback, CompositeInputActionState actionState)
    {
        if (enable) RegisterInputVector(upInputName, downInputName, leftInputName, rightInputName, callback, actionState);
        else RemoveInputVector(upInputName, downInputName, leftInputName, rightInputName, callback, actionState);
    }

    /// <summary>
    /// Register the mouse wheel's up and down scroll actions to the UI's horizontal navigation when this panel is at the top layer.
    /// </summary>
    /// <param name="scrollUpInputName">The input name for scrolling up (maps to UI Left navigation).</param>
    /// <param name="scrollDownInputName">The input name for scrolling down (maps to UI Right navigation).</param>
    protected void RegisterHorizontalScrollNavigation(StringName scrollUpInputName, StringName scrollDownInputName)
    {
        RegisterInput(scrollUpInputName, _ => Input.ParseInputEvent(new InputEventAction { Action = BuiltinInputNames.UILeft, Pressed = true }), InputActionPhase.Pressed);
        RegisterInput(scrollDownInputName, _ => Input.ParseInputEvent(new InputEventAction { Action = BuiltinInputNames.UIRight, Pressed = true }), InputActionPhase.Pressed);
    }


    /// <summary>
    /// Register the mouse wheel's up and down scroll actions to the UI's vertical navigation when this panel is at the top layer.
    /// </summary>
    /// <param name="scrollUpInputName">The input name for scrolling up (maps to UI Up navigation).</param>
    /// <param name="scrollDownInputName">The input name for scrolling down (maps to UI Down navigation).</param>
    protected void RegisterVerticalScrollNavigation(StringName scrollUpInputName, StringName scrollDownInputName)
    {
        RegisterInput(scrollUpInputName, _ => Input.ParseInputEvent(new InputEventAction { Action = BuiltinInputNames.UIUp, Pressed = true }), InputActionPhase.Pressed);
        RegisterInput(scrollDownInputName, _ => Input.ParseInputEvent(new InputEventAction { Action = BuiltinInputNames.UIDown, Pressed = true }), InputActionPhase.Pressed);
    }
}