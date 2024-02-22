using System;
using System.Runtime.CompilerServices;

namespace GDPanelFramework;

internal static class DelegateRunner
{
    internal static void RunProtected<T>(Action<T>? call, T arg, string actionName, string targetName, [CallerArgumentExpression(nameof(call))] string? methodName = null)
    {
        try
        {
            call?.Invoke(arg);
        }
        catch (Exception e)
        {
            ReportException(e, actionName, targetName, methodName);
        }
    }

    internal static void RunProtected(Action? call, string actionName, string targetName, [CallerArgumentExpression(nameof(call))] string? methodName = null)
    {
        try
        {
            call?.Invoke();
        }
        catch (Exception e)
        {
            ReportException(e, actionName, targetName, methodName);
        }
    }

    internal static void ReportException(Exception e, string actionName, string targetName, string? methodName)
    {
        LoggingUtils.LogError(
            $"""
             
             ┌┈┈┈┈ {actionName} Error ┈┈┈┈
             │ {e.GetType().Name} on {targetName}.{methodName ?? "UnknownFunction"}
             │ Message:
             │   {e.Message}
             └┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈┈
             """
        );
    }
}