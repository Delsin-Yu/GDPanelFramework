using System;
using GDPanelFramework.Panels;
using GDPanelFramework.Utils.AsyncInterop;

namespace GDPanelFramework;

public static partial class PanelManager
{
    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <returns>An awaitable that, when awaited, will asynchronously continues after this panel has closed.</returns>
    /// <remarks>This function provides an <see cref="AsyncAwaitable"/> for async/await styled programing, await this awaitable will suspend the control flow and resumes it when the panel closes, if you don't care when the panel closes, you can use the overload without this awaitable <see cref="OpenPanel(UIPanel,PreviousPanelVisual,ClosePolicy)"/>.</remarks>
    public static AsyncAwaitable OpenPanelAsync(
        this UIPanel panel,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache
    )
    {
        return AsyncInterop.ToAsync(continuation => OpenPanel(panel, continuation, previousPanelVisual, closePolicy));
    }

    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="onPanelCloseCallback">Calls when this panel is closed.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it's closed.</param>
    /// <remarks>This function provides an <paramref name="onPanelCloseCallback"/> to listen for panel close, if you don't care when the panel closes, you can use the overload without this callback <see cref="OpenPanel(UIPanel,PreviousPanelVisual,ClosePolicy)"/>.</remarks>
    public static void OpenPanel(
        this UIPanel panel,
        Action onPanelCloseCallback,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(Empty.Default, new(previousPanelVisual, closePolicy, null, onPanelCloseCallback));
    }

    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <remarks>This function doesn't allocate/accept a callback to listen for panel close, if you do need this callback, use the async overload <see cref="OpenPanelAsync"/> or the overload with a callback <see cref="OpenPanel(UIPanel,Action,PreviousPanelVisual,ClosePolicy)"/>.</remarks>
    public static void OpenPanel(
        this UIPanel panel,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(Empty.Default, new(previousPanelVisual, closePolicy, null, null));
    }


    /// <summary>
    /// Asynchronously opens this panel with the specified argument and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <typeparam name="TOpenArg">The argument passed to the panel when opening.</typeparam>
    /// <typeparam name="TCloseArg">The value returned by the panel after closing.</typeparam>
    /// <returns>An awaitable that, when awaited, will asynchronously continues and returns the <typeparamref name="TCloseArg"/> after this panel has closed.</returns>
    /// <remarks>This function provides an <see cref="AsyncAwaitable{TCloseArg}"/> for async/await styled programing, await this awaitable will suspend the control flow and resumes it when the panel closes, if you don't care when the panel closes, you can use the overload without this awaitable <see cref="OpenPanel(UIPanel,PreviousPanelVisual,ClosePolicy)"/>.</remarks>
    public static AsyncAwaitable<TCloseArg> OpenPanelAsync<TOpenArg, TCloseArg>(
        this UIPanelArg<TOpenArg, TCloseArg> panel,
        TOpenArg openArg,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache)
    {
        return AsyncInterop.ToAsync<TCloseArg>(continuation => OpenPanel(panel, openArg, continuation, previousPanelVisual, closePolicy));
    }

    
    /// <summary>
    /// Opens this panel with the specified argument and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="onPanelCloseCallback">Calls when this panel is closed.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it's closed.</param>
    /// <remarks>This function provides an <paramref name="onPanelCloseCallback"/> to listen for panel close, if you don't care when the panel closes, you can use the overload without this callback <see cref="OpenPanel{TOpenArg,TCloseArg}(UIPanelArg{TOpenArg,TCloseArg},TOpenArg,PreviousPanelVisual,ClosePolicy)"/>.</remarks>
    public static void OpenPanel<TOpenArg, TCloseArg>(
        this UIPanelArg<TOpenArg, TCloseArg> panel,
        TOpenArg openArg,
        Action<TCloseArg> onPanelCloseCallback,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(openArg, new(previousPanelVisual, closePolicy, onPanelCloseCallback, null));
    }

    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <remarks>This function doesn't allocate/accept a callback to listen for panel close, if you do need this callback, use the async overload <see cref="OpenPanelAsync"/> or the overload with a callback <see cref="OpenPanel{TOpenArg,TCloseArg}(UIPanelArg{TOpenArg,TCloseArg},TOpenArg,Action{TCloseArg},PreviousPanelVisual,ClosePolicy)"/>.</remarks>
    public static void OpenPanel<TOpenArg, TCloseArg>(
        this UIPanelArg<TOpenArg, TCloseArg> panel,
        TOpenArg openArg,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(openArg, new(previousPanelVisual, closePolicy, null, null));
    }
}