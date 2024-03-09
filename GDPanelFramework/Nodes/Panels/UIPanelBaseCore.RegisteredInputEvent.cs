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

        public void RegisterCall(Action<InputEvent> call, InputActionPhase inputActionPhase) => GetCall(inputActionPhase) += call;

        public void RemoveCall(Action<InputEvent> call, InputActionPhase inputActionPhase) => GetCall(inputActionPhase) -= call;

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
        public MappedInputAxis(string target)
        {
            _target = target;
            NegativeInputActionPressed = inputEvent =>
            {
                _isNegativePressed = true;
                
                if (inputEvent is InputEventJoypadMotion motion) _negativeAxisVector = motion.AxisValue;
                else _negativeAxisVector = 1;
                
                var currentValue = GetCurrentValue();
                if(!_isPositivePressed) InvokeStart(currentValue);
                InvokeUpdate(currentValue);
            };
            PositiveInputActionPressed = inputEvent =>
            {
                _isPositivePressed = true;
                
                if (inputEvent is InputEventJoypadMotion motion) _positiveAxisVector = motion.AxisValue;
                else _positiveAxisVector = 1;
                
                var currentValue = GetCurrentValue();
                if(!_isNegativePressed) InvokeStart(currentValue);
                InvokeUpdate(currentValue);
            };
            NegativeInputActionReleased = _ =>
            {
                _isNegativePressed = false;
                _negativeAxisVector = 0;
                
                var currentValue = GetCurrentValue();
                InvokeUpdate(currentValue);
                if(!_isPositivePressed) InvokeEnd(currentValue);
            };
            PositiveInputActionReleased = _ =>
            {
                _isPositivePressed = false;
                _positiveAxisVector = 0;
                
                var currentValue = GetCurrentValue();
                InvokeUpdate(currentValue);
                if(!_isNegativePressed) InvokeEnd(currentValue);
            };
        }

        public readonly Action<InputEvent> NegativeInputActionPressed;
        public readonly Action<InputEvent> PositiveInputActionPressed;
        public readonly Action<InputEvent> NegativeInputActionReleased;
        public readonly Action<InputEvent> PositiveInputActionReleased;
        
        public event Action<float>? OnStart;
        public event Action<float>? OnUpdate;
        public event Action<float>? OnEnd;

        public bool IsEmpty => OnStart == null && OnUpdate == null && OnEnd == null;

        private readonly string _target;
        private bool _isNegativePressed;
        private bool _isPositivePressed;
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
            if(Mathf.IsZeroApprox(currentValue - _cachedValue)) return;
            _cachedValue = currentValue;
            
            DelegateRunner.RunProtected(
                OnUpdate,
                _cachedValue,
                "Input Axis Composite Update",
                _target
            );
        }
    }

    private class MappedInputVector2
    {
        public MappedInputVector2(string target)
        {
            HorizontalAxis = new(target);
            VerticalAxis = new(target);

            HorizontalAxis.OnStart += horizontalAxisValue =>
            {
                _isHorizontalPressed = true;
                _horizontalAxisValue = horizontalAxisValue;
                
                var currentValue = GetCurrentValue();
                if(!_isVerticalPressed) InvokeStart(ref currentValue);
                InvokeUpdate(ref currentValue);
            };
            VerticalAxis.OnStart += verticalAxisValue =>
            {
                _isVerticalPressed = true;
                _verticalAxisValue = verticalAxisValue;
                
                var currentValue = GetCurrentValue();
                if(!_isHorizontalPressed) InvokeStart(ref currentValue);
                InvokeUpdate(ref currentValue);
            };    
            
            HorizontalAxis.OnUpdate += horizontalAxisValue =>
            {
                _horizontalAxisValue = horizontalAxisValue;
                var currentValue = GetCurrentValue();
                InvokeUpdate(ref currentValue);
            };
            
            VerticalAxis.OnUpdate += verticalAxisValue =>
            {
                _verticalAxisValue = verticalAxisValue;
                var currentValue = GetCurrentValue();
                InvokeUpdate(ref currentValue);
            };
            
            HorizontalAxis.OnEnd += horizontalAxisValue =>
            {
                _isHorizontalPressed = false;
                _horizontalAxisValue = horizontalAxisValue;
                
                var currentValue = GetCurrentValue();
                InvokeUpdate(ref currentValue);
                if(!_isVerticalPressed) InvokeEnd(ref currentValue);
            };
            
            VerticalAxis.OnEnd += horizontalAxisValue =>
            {
                _isVerticalPressed = false;
                _verticalAxisValue = horizontalAxisValue;
                
                var currentValue = GetCurrentValue();
                InvokeUpdate(ref currentValue);
                if(!_isHorizontalPressed) InvokeEnd(ref currentValue);
            };
            
            _target = target;
        }
        
        public readonly MappedInputAxis HorizontalAxis;
        public readonly MappedInputAxis VerticalAxis;

        private bool _isHorizontalPressed;
        private bool _isVerticalPressed;
        private float _horizontalAxisValue;
        private float _verticalAxisValue;
        private Vector2 _cachedValue = new(float.NaN, float.NaN);

        private readonly string _target;
                
        public event Action<Vector2>? OnStart;
        public event Action<Vector2>? OnUpdate;
        public event Action<Vector2>? OnEnd;
        
        public bool IsEmpty => OnStart == null && OnUpdate == null && OnEnd == null;

        private Vector2 GetCurrentValue() => new(_horizontalAxisValue, _verticalAxisValue);
        
        private void InvokeStart(ref readonly Vector2 currentValue)
        {
            DelegateRunner.RunProtected(
                OnStart,
                currentValue,
                "Input Vector Composite Start",
                _target
            );
        }

        private void InvokeEnd(ref readonly Vector2 currentValue)
        {
            DelegateRunner.RunProtected(
                OnEnd,
                currentValue,
                "Input Vector Composite End",
                _target
            );
        }

        private void InvokeUpdate(ref readonly Vector2 currentValue)
        {
            if(Mathf.IsZeroApprox(_cachedValue.X - currentValue.X) && Mathf.IsZeroApprox(_cachedValue.Y - currentValue.Y)) return;

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