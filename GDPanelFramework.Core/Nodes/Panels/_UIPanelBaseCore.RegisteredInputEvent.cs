using System;
using Godot;

namespace GDPanelSystem.Core.Panels;

public abstract partial class _UIPanelBaseCore
{
    private class RegisteredInputEvent
    {
        private Action<InputEvent>? m_PressedCall;
        private Action<InputEvent>? m_ReleasedCall;
        private Action<InputEvent>? _anyCall;

        public bool Empty => m_PressedCall is null && m_ReleasedCall is null && _anyCall is null;

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
                    return ref m_PressedCall;
                case InputActionPhase.Released:
                    return ref m_ReleasedCall;
                case InputActionPhase.Any:
                    return ref _anyCall;
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputActionPhase), inputActionPhase, null);
            }
        }
    }
}