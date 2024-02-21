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
    }
}