using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace GDPanelFramework;

partial class RootPanelContainer
{
    private static readonly HashSet<JoyButton> EchoedButtons =
    [
        JoyButton.DpadUp,
        JoyButton.DpadDown,
        JoyButton.DpadLeft,
        JoyButton.DpadRight,
    ];
    private static readonly HashSet<JoyAxis> EchoedAxes =
    [
        JoyAxis.LeftY,
        JoyAxis.RightY,
        JoyAxis.LeftX,
        JoyAxis.RightX,
    ];
    private static readonly HashSet<Key> NotEchoedKeys =
    [
        Key.Shift,
        Key.Ctrl,
        Key.Alt,
        Key.Meta,
    ];

    private void ProcessEchoEvents(InputEvent inputEvent)
    {
        switch (inputEvent)
        {
            case InputEventKey keyEvent:
            {
                if (keyEvent.GetModifiersMask() != 0) break;
                var key = keyEvent.PhysicalKeycode;
                if (NotEchoedKeys.Contains(key)) break;
                var pressed = keyEvent.Pressed;

                _pendingRepeatInputs.Remove(key);
                _activeRepeatInputs.Remove(key);
                if (pressed) _pendingRepeatInputs[key] = Time.GetTicksMsec() + InputEchoing.InitialDelay;

                break;
            }
            case InputEventJoypadButton joyButtonEvent:
            {
                if (!EchoedButtons.Contains(joyButtonEvent.ButtonIndex))
                    break;

                var pressed = joyButtonEvent.Pressed;
                var buttonIndex = joyButtonEvent.ButtonIndex;

                _pendingRepeatInputs.Remove(buttonIndex);
                _activeRepeatInputs.Remove(buttonIndex);
                if (pressed) _pendingRepeatInputs[buttonIndex] = Time.GetTicksMsec() + InputEchoing.InitialDelay;

                break;
            }
            case InputEventJoypadMotion joyMotionEvent:
            {
                if (!EchoedAxes.Contains(joyMotionEvent.Axis))
                    break;

                var pressed = joyMotionEvent.IsPressed();
                var axis = joyMotionEvent.Axis;
                var positive = joyMotionEvent.AxisValue > 0;
                var axisData = new JoyAxisData(axis, positive);

                var currentState = pressed
                    ? positive ? AxisState.Positive : AxisState.Negative
                    : AxisState.Neutral;

                if (_axisStates.TryGetValue(axis, out var lastPressedState)
                    && lastPressedState == currentState)
                    break;

                _axisStates[axis] = currentState;

                _pendingRepeatInputs.Remove(axisData);
                _activeRepeatInputs.Remove(axisData);
                var invertedAxisData = new JoyAxisData(axis, !positive);
                _pendingRepeatInputs.Remove(invertedAxisData);
                _activeRepeatInputs.Remove(invertedAxisData);

                if (pressed) _pendingRepeatInputs[axisData] = Time.GetTicksMsec() + InputEchoing.InitialDelay;

                break;
            }
        }
    }

    private enum AxisState
    {
        Negative,
        Neutral,
        Positive,
    }

    private record struct JoyAxisData(JoyAxis Axis, bool Positive);

    private readonly Dictionary<JoyAxis, AxisState> _axisStates = [];
    private readonly Dictionary<OneOf<Key, JoyButton, JoyAxisData>, ulong> _pendingRepeatInputs = [];
    private readonly Dictionary<OneOf<Key, JoyButton, JoyAxisData>, ulong> _activeRepeatInputs = [];
    private readonly Queue<OneOf<Key, JoyButton, JoyAxisData>> _executionQueue = new();
    private readonly HashSet<InputEvent> _synthesisedEvent = [];

    public override void _Process(double delta)
    {
        var currentTime = Time.GetTicksMsec();

        foreach (var (key, triggerTime) in _pendingRepeatInputs)
            if (currentTime >= triggerTime)
                _executionQueue.Enqueue(key);

        while (_executionQueue.TryDequeue(out var key))
        {
            _pendingRepeatInputs.Remove(key);
            _activeRepeatInputs.Add(key, currentTime + InputEchoing.RepeatInterval);
            SynthesizeEvent(key);
        }


        foreach (var (key, triggerTime) in _activeRepeatInputs)
            if (currentTime >= triggerTime)
                _executionQueue.Enqueue(key);

        while (_executionQueue.TryDequeue(out var key))
        {
            _activeRepeatInputs[key] = currentTime + InputEchoing.RepeatInterval;
            SynthesizeEvent(key);
        }

        return;

        void SynthesizeEvent(OneOf<Key, JoyButton, JoyAxisData> key)
        {
            ReadOnlySpan<InputEvent> inputEvents =
                key.Match(out var keyboardKey, out var joyButton, out var joyAxisData) switch
                {
                    OneOfThree.First =>
                    [
                        new InputEventKey
                        {
                            Keycode = keyboardKey,
                            PhysicalKeycode = keyboardKey,
                            Pressed = true,
                        },
                    ],
                    OneOfThree.Second =>
                    [
                        new InputEventJoypadButton
                        {
                            ButtonIndex = joyButton,
                            Pressed = true,
                        },
                    ],
                    OneOfThree.Third =>
                    [
                        new InputEventJoypadMotion
                        {
                            Axis = joyAxisData.Axis,
                            AxisValue = 0f,
                        },
                        new InputEventJoypadMotion
                        {
                            Axis = joyAxisData.Axis,
                            AxisValue = joyAxisData.Positive ? 1.0f : -1.0f,
                        },
                    ],
                    _ => throw new UnreachableException(),
                };

            foreach (var inputEvent in inputEvents)
                _synthesisedEvent.Add(inputEvent);

            foreach (var inputEvent in inputEvents)
                Input.ParseInputEvent(inputEvent);
        }
    }

    private enum OneOfThree
    {
        First,
        Second,
        Third,
    }

    private readonly struct OneOf<T1, T2, T3>(T1 item1, T2 item2, T3 item3, OneOfThree type)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
    {
        public OneOfThree Match(out T1 first, out T2 second, out T3 third)
        {
            first = item1;
            second = item2;
            third = item3;
            return type;
        }

        public static implicit operator OneOf<T1, T2, T3>(T1 item) => new(item, default!, default!, OneOfThree.First);
        public static implicit operator OneOf<T1, T2, T3>(T2 item) => new(default!, item, default!, OneOfThree.Second);
        public static implicit operator OneOf<T1, T2, T3>(T3 item) => new(default!, default!, item, OneOfThree.Third);

        public override string? ToString() =>
            type switch
            {
                OneOfThree.First => item1.ToString(),
                OneOfThree.Second => item2.ToString(),
                OneOfThree.Third => item3.ToString(),
                _ => throw new UnreachableException(),
            };
    }
}