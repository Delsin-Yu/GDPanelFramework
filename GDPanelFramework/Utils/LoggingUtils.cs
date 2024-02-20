using Godot;

namespace GDPanelFramework;

internal static class LoggingUtils
{
    internal static void LogError(string message) => GD.PushError(message);
}