using System;
using System.Runtime.CompilerServices;
using Godot;

namespace GDPanelSystem.Core;

internal static class Utils
{
    internal static void LogError(string message) => GD.PushError(message);

    internal static void RunProtected<T>(Action<T> call, T arg, string actionName, string targetName, [CallerArgumentExpression(nameof(call))] string methodName = default)
    {
        try
        {
            call(arg);
        }
        catch (Exception e)
        {
            ReportException(e, actionName, targetName, methodName);
        }  
    }
    internal static void RunProtected(Action call, string actionName, string targetName, [CallerArgumentExpression(nameof(call))] string methodName = default)
    {
        try
        {
            call();
        }
        catch (Exception e)
        {
            ReportException(e, actionName, targetName, methodName);
        }
    }

    internal static void ReportException(Exception e, string actionName, string targetName, string methodName)
    {
        LogError(
            $"""
             ┌┈┈┈┈ {actionName} Error ┈┈┈┈
             │ {e.GetType().Name} on {targetName}.{methodName}
             │ Message:
             │   {e.Message}
             └┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈
             """
        );
    }
}


public struct EmptyUnit
{
    public static readonly EmptyUnit Default = new();
}