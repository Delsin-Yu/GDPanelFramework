using System;
using Godot;

namespace GDPanelFramework.Panels;

public abstract partial class UIPanelBaseCore
{
    private class RegisteredInputEvent
    {
        private Action<InputEvent>? _pressedCall;
        private Action<InputEvent>? _releasedCall;
        private Action<InputEvent>? _anyCall;

        public bool Empty => _pressedCall is null && _releasedCall is null && _anyCall is null;

        public void RegisterCall(Action<InputEvent> call, InputActionPhase? inputActionPhase) => GetCall(PanelManager.GetInputActionPhase(inputActionPhase)) += call;

        public void RemoveCall(Action<InputEvent> call, InputActionPhase? inputActionPhase) => GetCall(PanelManager.GetInputActionPhase(inputActionPhase)) -= call;

        public bool Call(InputEvent inputEvent, InputActionPhase inputActionPhase, string name)
        {
            var called = false;
            var call = GetCall(inputActionPhase);

            if (call != null)
            {
                called = true;
                DelegateRunner.RunProtected(call, inputEvent, "Input Call", name, inputActionPhase == InputActionPhase.Pressed ? "Pressed" : "Released");
            }

            if (_anyCall != null)
            {
                called = true;
                DelegateRunner.RunProtected(_anyCall, inputEvent, "Input Any Call", name, "Any");
            }

            return called;
        }

        private ref Action<InputEvent>? GetCall(InputActionPhase inputActionPhase)
        {
            switch (inputActionPhase)
            {
                case InputActionPhase.Pressed:
                    return ref _pressedCall;
                case InputActionPhase.Released:
                    return ref _releasedCall;
                case InputActionPhase.Any:
                    return ref _anyCall;
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputActionPhase), inputActionPhase, null);
            }
        }

        public void Reset()
        {
            _pressedCall = null;
            _releasedCall = null;
            _anyCall = null;
        }
    }

    private class MappedInputAxis
    {
        public MappedInputAxis(string target, float deadZone)
        {
            _target = target;
            var deadZoneSquared = deadZone * deadZone;
            var negativeKeyPressed = false;
            var positiveKeyPressed = false;
            NegativeInputActionUpdate = inputEvent =>
            {
                var oldValue = GetCurrentValue();

                if (inputEvent is InputEventJoypadMotion motion)
                {
                    _negativeAxisVector = Mathf.Abs(motion.AxisValue);
                    _negativeAxisVector = Mathf.Max(_negativeAxisVector, deadZoneSquared);
                    ProcessUpdate(oldValue);
                }
                else
                {
                    var wasActive = negativeKeyPressed || positiveKeyPressed;
                    negativeKeyPressed = inputEvent.IsPressed();
                    _negativeAxisVector = negativeKeyPressed ? 1 : 0;
                    var isActive = negativeKeyPressed || positiveKeyPressed;
                    ProcessUpdate(oldValue, !wasActive && isActive && negativeKeyPressed, wasActive && !isActive && !negativeKeyPressed);
                }
            };
            PositiveInputActionUpdate = inputEvent =>
            {
                var oldValue = GetCurrentValue();

                if (inputEvent is InputEventJoypadMotion motion)
                {
                    _positiveAxisVector = Mathf.Abs(motion.AxisValue);
                    _positiveAxisVector = Mathf.Max(_negativeAxisVector, deadZoneSquared);
                    ProcessUpdate(oldValue);
                }
                else
                {
                    var wasActive = negativeKeyPressed || positiveKeyPressed;
                    positiveKeyPressed = inputEvent.IsPressed();
                    _positiveAxisVector = positiveKeyPressed ? 1 : 0;
                    var isActive = negativeKeyPressed || positiveKeyPressed;
                    ProcessUpdate(oldValue, !wasActive && isActive && positiveKeyPressed, wasActive && !isActive && !positiveKeyPressed);
                }
            };
        }

        private void ProcessUpdate(float oldValue)
        {
            var currentValue = GetCurrentValue();

            switch (Mathf.IsZeroApprox(oldValue), Mathf.IsZeroApprox(currentValue))
            {
                case (true, true):
                    return;
                case (true, false):
                    InvokeStart(currentValue);
                    InvokeUpdate(currentValue);
                    break;
                case (false, true):
                    InvokeEnd(currentValue);
                    InvokeUpdate(currentValue);
                    break;
                case (false, false):
                    InvokeUpdate(currentValue);
                    break;
            }
        }

        private void ProcessUpdate(float oldValue, bool invokeStart, bool invokeEnd)
        {
            var currentValue = GetCurrentValue();

            if (Mathf.IsZeroApprox(oldValue) && Mathf.IsZeroApprox(currentValue)) return;

            if (invokeStart) InvokeStart(currentValue);
            if (invokeEnd) InvokeEnd(currentValue);

            InvokeUpdate(currentValue);
        }

        public readonly Action<InputEvent> NegativeInputActionUpdate;
        public readonly Action<InputEvent> PositiveInputActionUpdate;

        public event Action<float>? OnStart;
        public event Action<float>? OnUpdate;
        public event Action<float>? OnEnd;

        public bool IsEmpty => OnStart == null && OnUpdate == null && OnEnd == null;

        private readonly string _target;
        private float _negativeAxisVector;
        private float _positiveAxisVector;
        private float _cachedValue = float.NaN;

        private float GetCurrentValue() => _positiveAxisVector - _negativeAxisVector;

        private void InvokeStart(float currentValue) => DelegateRunner.RunProtected(
            OnStart,
            currentValue,
            "Input Axis Composite Start",
            _target
        );

        private void InvokeEnd(float currentValue) => DelegateRunner.RunProtected(
            OnEnd,
            currentValue,
            "Input Axis Composite End",
            _target
        );

        private void InvokeUpdate(float currentValue)
        {
            if (Mathf.IsZeroApprox(currentValue - _cachedValue)) return;
            _cachedValue = currentValue;

            DelegateRunner.RunProtected(
                OnUpdate,
                _cachedValue,
                "Input Axis Composite Update",
                _target
            );
        }
    }

    private class MappedInputVector
    {
        public MappedInputVector(string target, float deadZone)
        {
            _deadZone = deadZone;
            _target = target;
            var horizontalNegativeKeyPressed = false;
            var horizontalPositiveKeyPressed = false;
            var verticalNegativeKeyPressed = false;
            var verticalPositiveKeyPressed = false;
            HorizontalNegativeInputActionUpdate = inputEvent =>
            {
                var oldValue = GetCurrentValue();

                if (inputEvent is InputEventJoypadMotion motion) _horizontalAxisVector = -Mathf.Abs(motion.AxisValue);
                else
                {
                    var wasActive = horizontalNegativeKeyPressed || horizontalPositiveKeyPressed || verticalNegativeKeyPressed || verticalPositiveKeyPressed;
                    horizontalNegativeKeyPressed = inputEvent.IsPressed();
                    _horizontalAxisVector = (horizontalNegativeKeyPressed, horizontalPositiveKeyPressed) switch
                    {
                        (false, false) => 0,
                        (true, true) => 0,
                        (true, false) => -1,
                        (false, true) => 1,
                    };

                    var isActive = horizontalNegativeKeyPressed || horizontalPositiveKeyPressed || verticalNegativeKeyPressed || verticalPositiveKeyPressed;
                    ProcessUpdate(oldValue, !wasActive && isActive && horizontalNegativeKeyPressed, wasActive && !isActive && !horizontalNegativeKeyPressed);
                    return;
                }

                ProcessUpdate(oldValue);
            };
            HorizontalPositiveInputActionUpdate = inputEvent =>
            {
                var oldValue = GetCurrentValue();

                if (inputEvent is InputEventJoypadMotion motion) _horizontalAxisVector = Mathf.Abs(motion.AxisValue);
                else
                {
                    var wasActive = horizontalNegativeKeyPressed || horizontalPositiveKeyPressed || verticalNegativeKeyPressed || verticalPositiveKeyPressed;
                    horizontalPositiveKeyPressed = inputEvent.IsPressed();
                    _horizontalAxisVector = (horizontalNegativeKeyPressed, horizontalPositiveKeyPressed) switch
                    {
                        (false, false) => 0,
                        (true, true) => 0,
                        (true, false) => -1,
                        (false, true) => 1,
                    };

                    var isActive = horizontalNegativeKeyPressed || horizontalPositiveKeyPressed || verticalNegativeKeyPressed || verticalPositiveKeyPressed;
                    ProcessUpdate(oldValue, !wasActive && isActive && horizontalPositiveKeyPressed, wasActive && !isActive && !horizontalPositiveKeyPressed);
                    return;
                }

                ProcessUpdate(oldValue);
            };
            VerticalNegativeInputActionUpdate = inputEvent =>
            {
                var oldValue = GetCurrentValue();

                if (inputEvent is InputEventJoypadMotion motion) _verticalAxisVector = -Mathf.Abs(motion.AxisValue);
                else
                {
                    var wasActive = horizontalNegativeKeyPressed || horizontalPositiveKeyPressed || verticalNegativeKeyPressed || verticalPositiveKeyPressed;
                    verticalNegativeKeyPressed = inputEvent.IsPressed();
                    _verticalAxisVector = (verticalNegativeKeyPressed, verticalPositiveKeyPressed) switch
                    {
                        (false, false) => 0,
                        (true, true) => 0,
                        (true, false) => -1,
                        (false, true) => 1,
                    };

                    var isActive = horizontalNegativeKeyPressed || horizontalPositiveKeyPressed || verticalNegativeKeyPressed || verticalPositiveKeyPressed;
                    ProcessUpdate(oldValue, !wasActive && isActive && verticalNegativeKeyPressed, wasActive && !isActive && !verticalNegativeKeyPressed);
                    return;
                }

                ProcessUpdate(oldValue);
            };
            VerticalPositiveInputActionUpdate = inputEvent =>
            {
                var oldValue = GetCurrentValue();

                if (inputEvent is InputEventJoypadMotion motion) _verticalAxisVector = Mathf.Abs(motion.AxisValue);
                else
                {
                    var wasActive = horizontalNegativeKeyPressed || horizontalPositiveKeyPressed || verticalNegativeKeyPressed || verticalPositiveKeyPressed;
                    verticalPositiveKeyPressed = inputEvent.IsPressed();
                    _verticalAxisVector = (verticalNegativeKeyPressed, verticalPositiveKeyPressed) switch
                    {
                        (false, false) => 0,
                        (true, true) => 0,
                        (true, false) => -1,
                        (false, true) => 1,
                    };

                    var isActive = horizontalNegativeKeyPressed || horizontalPositiveKeyPressed || verticalNegativeKeyPressed || verticalPositiveKeyPressed;
                    ProcessUpdate(oldValue, !wasActive && isActive && verticalPositiveKeyPressed, wasActive && !isActive && !verticalPositiveKeyPressed);
                    return;
                }

                ProcessUpdate(oldValue);
            };
        }

        private void ProcessUpdate(Vector2 oldValue)
        {
            var currentValue = GetCurrentValue();
            if (currentValue.LengthSquared() < _deadZone * _deadZone) currentValue = Vector2.Zero;

            switch (Mathf.IsZeroApprox(oldValue.LengthSquared()),
                Mathf.IsZeroApprox(currentValue.LengthSquared()))
            {
                case (true, true):
                    return;
                case (true, false):
                    InvokeStart(currentValue);
                    InvokeUpdate(currentValue);
                    break;
                case (false, true):
                    InvokeEnd(currentValue);
                    InvokeUpdate(currentValue);
                    break;
                case (false, false):
                    InvokeUpdate(currentValue);
                    break;
            }
        }

        private void ProcessUpdate(Vector2 oldValue, bool invokeStart, bool invokeEnd)
        {
            var currentValue = GetCurrentValue();
            if (currentValue.LengthSquared() < _deadZone * _deadZone) currentValue = Vector2.Zero;

            if (Mathf.IsZeroApprox(oldValue.LengthSquared()) && Mathf.IsZeroApprox(currentValue.LengthSquared())) return;

            if (invokeStart) InvokeStart(currentValue);
            if (invokeEnd) InvokeEnd(currentValue);

            InvokeUpdate(currentValue);
        }

        public readonly Action<InputEvent> HorizontalNegativeInputActionUpdate;
        public readonly Action<InputEvent> HorizontalPositiveInputActionUpdate;
        public readonly Action<InputEvent> VerticalNegativeInputActionUpdate;
        public readonly Action<InputEvent> VerticalPositiveInputActionUpdate;

        public event Action<Vector2>? OnStart;
        public event Action<Vector2>? OnUpdate;
        public event Action<Vector2>? OnEnd;

        public bool IsEmpty => OnStart == null && OnUpdate == null && OnEnd == null;

        private readonly string _target;
        private float _horizontalAxisVector;
        private float _verticalAxisVector;
        private Vector2 _cachedValue = new(float.NaN, float.NaN);
        private readonly float _deadZone;

        private Vector2 GetCurrentValue() =>
            new(
                _horizontalAxisVector,
                _verticalAxisVector
            );

        private void InvokeStart(Vector2 currentValue) => DelegateRunner.RunProtected(
            OnStart,
            currentValue,
            "Input Vector Composite Start",
            _target
        );

        private void InvokeEnd(Vector2 currentValue) => DelegateRunner.RunProtected(
            OnEnd,
            currentValue,
            "Input Vector Composite End",
            _target
        );

        private void InvokeUpdate(Vector2 currentValue)
        {
            if (Mathf.IsZeroApprox((currentValue - _cachedValue).LengthSquared())) return;
            _cachedValue = currentValue;

            DelegateRunner.RunProtected(
                OnUpdate,
                _cachedValue,
                "Input Vector Composite Update",
                _target
            );
        }
    }

    private record struct InputAxisBinding(string NegativeInputName, string PositiveInputName);

    private record struct InputVectorBinding(InputAxisBinding HorizontalInputAxis, InputAxisBinding VerticalInputAxis);
}