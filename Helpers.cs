using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using GodotTask;

namespace GDPanelFramework;

public static class Helpers
{
    public static T NotNull<T>([NotNull] this T? nullableObject)
    {
        ArgumentNullException.ThrowIfNull(nullableObject);
        return nullableObject;
    }

    public static async GDTask KeyPressedAsync(Key key)
    {
        await KeyPressAsync(key);
        await KeyReleaseAsync(key);
    }

    public static async GDTask KeyPressAsync(Key key)
    {
        Input.ParseInputEvent(
            new InputEventKey
            {
                Pressed = true,
                Keycode = key,
                PhysicalKeycode = key
            }
        );
        await GDTask.NextFrame();
    }

    public static async GDTask KeyReleaseAsync(Key key)
    {
        Input.ParseInputEvent(
            new InputEventKey
            {
                Pressed = false,
                Keycode = key,
                PhysicalKeycode = key
            }
        );
        await GDTask.NextFrame();
    }
}