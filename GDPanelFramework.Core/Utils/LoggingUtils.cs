using Godot;

namespace GDPanelSystem.Core;

internal static class LoggingUtils
{
    internal static void LogError(string message) => GD.PushError(message);
}