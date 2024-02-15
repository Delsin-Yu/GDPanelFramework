using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GDPanelSystem.Core.Panels;

namespace GDPanelSystem.Core;

internal static class ExceptionUtils
{
    private const string PanelClosingOrderNotification = "When closing panels, it is mandatory to ensure the order of closing these panels is symmetrical to how they are opened.";

    internal static void ThrowIfUninitialized(this UIPanelBaseCore panel)
    {
        if (panel.CurrentPanelStatus != UIPanelBaseCore.PanelStatus.Uninitialized) return;
        throw new InvalidOperationException("Attempting to open an uninitialized panel, this is not supported, please use CreateOrGetPanel to properly get an initialized panel.");
    }

    internal static void ThrowIfNotOpened(this UIPanelBaseCore panel)
    {
        if (panel.CurrentPanelStatus != UIPanelBaseCore.PanelStatus.Opened) return;
        throw new InvalidOperationException("Attempting to close a not opened panel, this is not supported.");
    }

    public static void ThrowIfClosingPanelIsNotTopPanel<TPanel>(TPanel closingPanel, UIPanelBaseCore topPanel) where TPanel : UIPanelBaseCore
    {
        if (ReferenceEquals(closingPanel, topPanel)) return;
        throw new InvalidOperationException($"Attempting to close a panel that is not on top of the current panel layer, this is not supported. {PanelClosingOrderNotification}");
    }

    internal static void ThrowIfPanelLayerIsGreaterThanOne(IReadOnlyCollection<UIPanelBaseCore> panelLayer)
    {
        if (panelLayer.Count == 1) return;
        throw new InvalidOperationException($"Attempting to close a panel layer while there are other active panels inside the current layer, this is not supported. {PanelClosingOrderNotification}");
    }
}

internal static class DelegateRunner
{
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
        LoggingUtils.LogError(
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