using System;
using Godot;

namespace GDPanelFramework;

/// <summary>
/// Provide an event to customize the behavior when an exception is thrown in protected methods.
/// </summary>
public static class GDPanelFrameworkExceptionHandler
{
    /// <summary>
    /// Occurs when an exception is thrown in protected methods, exceptions will output to the default Godot output via <see cref="GD.PushError(string)"/> if left unassigned.
    /// </summary>
    public static event Action<Exception>? OnProtectedException;

    internal static void PublishProtectedException(Exception e, string actionName, string targetName, string? methodName)
    {
        if (OnProtectedException != null)
        {
            OnProtectedException.Invoke(e);
        }
        else
        {
            GD.PushError(
                $"""

                 ┌┈┈┈┈ {actionName} Error ┈┈┈┈
                 │ {e.GetType().Name} on {targetName}.{methodName ?? "UnknownFunction"}
                 │ Message:
                 │   {e.Message}
                 └┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈
                 {e.StackTrace}
                 """
            );
        }
    }
}