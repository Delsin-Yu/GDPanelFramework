using System;
using System.Runtime.CompilerServices;

namespace GDPanelFramework;

internal static class DelegateRunner
{
    internal static bool RunProtected<T>(Action<T>? call, in T arg, string actionName, string targetName, [CallerArgumentExpression(nameof(call))] string? methodName = null)
    {
        try
        {
            call?.Invoke(arg);
            return true;
        }
        catch (Exception e)
        {
            GDPanelFrameworkExceptionHandler.PublishProtectedException(e, actionName, targetName, methodName);
            return false;
        }
    }

    internal static bool RunProtected(Action? call, string actionName, string targetName, [CallerArgumentExpression(nameof(call))] string? methodName = null)
    {
        try
        {
            call?.Invoke();
            return true;
        }
        catch (Exception e)
        {
            GDPanelFrameworkExceptionHandler.PublishProtectedException(e, actionName, targetName, methodName);
            return false;
        }
    }
}