using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace GDPanelFramework;

public static class Helpers
{
    public static T NotNull<T>([NotNull] this T? nullableObject)
    {
        ArgumentNullException.ThrowIfNull(nullableObject);
        return nullableObject;
    }

    public static void KeyPressed(Key key)
    {
        KeyPress(key);
        KeyRelease(key);
    }

    public static void KeyPress(Key key)
    {
        Input.ParseInputEvent(
            new InputEventKey
            {
                Pressed = true,
                Keycode = key,
                PhysicalKeycode = key
            }
        );
        Input.FlushBufferedEvents();
    }

    public static void KeyRelease(Key key)
    {
        Input.ParseInputEvent(
            new InputEventKey
            {
                Pressed = false,
                Keycode = key,
                PhysicalKeycode = key
            }
        );
        Input.FlushBufferedEvents();
    }
}