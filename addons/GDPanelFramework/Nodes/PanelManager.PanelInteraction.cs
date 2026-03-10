using System;
using GDPanelFramework.Panels;

namespace GDPanelFramework;

public static partial class PanelManager
{
    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <returns>An awaitable that completes when this panel has closed.</returns>
    /// <remarks>If <paramref name="cancellationToken"/> is canceled while the panel is open, awaiting this operation will throw <see cref="OperationCanceledException"/>.</remarks>
    public static PanelAwaitable OpenPanelAsync(
        this UIPanel panel,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null
    )
    {
        var completionSource = PanelAwaitableCompletionSource<Empty>.Create();
        OpenPanel(
            panel,
            result =>
            {
                if (result.HasValue) completionSource.TrySetResult(Empty.Default);
                else completionSource.TrySetCanceled(cancellationToken ?? System.Threading.CancellationToken.None);
            },
            previousPanelVisual,
            closePolicy,
            cancellationToken
        );
        return new PanelAwaitable(completionSource, completionSource.Version);
    }

    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="onPanelCloseCallback">Calls when this panel is closed.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it's closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <remarks>Use this overload when you need a close callback with cancellation awareness.</remarks>
    public static void OpenPanel(
        this UIPanel panel,
        Action onPanelCloseCallback,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null) =>
        OpenPanel(
            panel,
            _ => onPanelCloseCallback(),
            previousPanelVisual,
            closePolicy,
            cancellationToken
        );

    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="onPanelCloseCallback">Calls when this panel is closed.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it's closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <remarks>Use this overload when you need a close callback with cancellation awareness.</remarks>
    public static void OpenPanel(
        this UIPanel panel,
        Action<PanelResult<Empty>> onPanelCloseCallback,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(Empty.Default, new(previousPanelVisual, closePolicy, null, onPanelCloseCallback, cancellationToken));
    }

    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <remarks>Use this overload when you do not need any close callback.</remarks>
    public static void OpenPanel(
        this UIPanel panel,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(Empty.Default, new(previousPanelVisual, closePolicy, null, null, cancellationToken));
    }

    /// <summary>
    /// Asynchronously opens this panel with the specified argument and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <typeparam name="TOpenArg">The argument passed to the panel when opening.</typeparam>
    /// <returns>An awaitable that completes when this panel has closed.</returns>
    /// <remarks>If <paramref name="cancellationToken"/> is canceled while the panel is open, awaiting this operation will throw <see cref="OperationCanceledException"/>.</remarks>
    public static PanelAwaitable OpenPanelAsync<TOpenArg>(
        this UIPanelArg1<TOpenArg> panel,
        TOpenArg openArg,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        var completionSource = PanelAwaitableCompletionSource<Empty>.Create();
        OpenPanel(
            panel,
            openArg,
            result =>
            {
                if (result.HasValue) completionSource.TrySetResult(Empty.Default);
                else completionSource.TrySetCanceled(cancellationToken ?? System.Threading.CancellationToken.None);
            },
            previousPanelVisual,
            closePolicy,
            cancellationToken
        );
        return new PanelAwaitable(completionSource, completionSource.Version);
    }

    /// <summary>
    /// Opens this panel with the specified argument and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="onPanelCloseCallback">Calls when this panel is closed.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it's closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <remarks>Use this overload when you need a close callback with cancellation awareness.</remarks>
    public static void OpenPanel<TOpenArg>(
        this UIPanelArg1<TOpenArg> panel,
        TOpenArg openArg,
        Action<PanelResult<Empty>> onPanelCloseCallback,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(openArg, new(previousPanelVisual, closePolicy, null, onPanelCloseCallback, cancellationToken));
    }

    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <remarks>Use this overload when you do not need any close callback.</remarks>
    public static void OpenPanel<TOpenArg>(
        this UIPanelArg1<TOpenArg> panel,
        TOpenArg openArg,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(openArg, new(previousPanelVisual, closePolicy, null, null, cancellationToken));
    }

    /// <summary>
    /// Asynchronously opens this panel with the specified argument and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <typeparam name="TOpenArg">The argument passed to the panel when opening.</typeparam>
    /// <typeparam name="TCloseArg">The value returned by the panel after closing.</typeparam>
    /// <returns>An awaitable that completes and returns the <typeparamref name="TCloseArg"/> when this panel closes.</returns>
    /// <remarks>If <paramref name="cancellationToken"/> is canceled while the panel is open, awaiting this operation will throw <see cref="OperationCanceledException"/>.</remarks>
    public static PanelAwaitable<TCloseArg> OpenPanelAsync<TOpenArg, TCloseArg>(
        this UIPanelArg2<TOpenArg, TCloseArg> panel,
        TOpenArg openArg,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        var completionSource = PanelAwaitableCompletionSource<TCloseArg>.Create();
        OpenPanel(
            panel,
            openArg,
            result =>
            {
                if (result.TryGetValue(out var value)) completionSource.TrySetResult(value);
                else completionSource.TrySetCanceled(cancellationToken ?? System.Threading.CancellationToken.None);
            },
            previousPanelVisual,
            closePolicy,
            cancellationToken
        );
        return completionSource.Awaitable;
    }


    /// <summary>
    /// Opens this panel with the specified argument and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="onPanelCloseCallback">Calls when this panel is closed.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it's closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <remarks>Use this overload when you need a close callback with cancellation awareness.</remarks>
    public static void OpenPanel<TOpenArg, TCloseArg>(
        this UIPanelArg2<TOpenArg, TCloseArg> panel,
        TOpenArg openArg,
        Action<PanelResult<TCloseArg>> onPanelCloseCallback,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(openArg, new(previousPanelVisual, closePolicy, onPanelCloseCallback, null, cancellationToken));
    }
    
    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="onPanelCloseCallback">Calls when this panel is closed.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it's closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <remarks>Use this overload when you need a close callback with cancellation awareness.</remarks>
    public static void OpenPanel<TCloseArg>(
        this UIPanelArg2<Empty, TCloseArg> panel,
        Action<PanelResult<TCloseArg>> onPanelCloseCallback,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(Empty.Default, new(previousPanelVisual, closePolicy, onPanelCloseCallback, null, cancellationToken));
    }

    /// <summary>
    /// Opens this panel and stops the previous panel from receiving inputs until this panel closes.
    /// </summary>
    /// <param name="panel">The opening panel instance.</param>
    /// <param name="openArg">The argument passes to the panel.</param>
    /// <param name="previousPanelVisual">When setting to <see cref="PreviousPanelVisual.Hidden"/>, the previous panel will become invisible.</param>
    /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
    /// <param name="cancellationToken">An optional cancellation token to close the panel externally.</param>
    /// <remarks>Use this overload when you do not need any close callback.</remarks>
    public static void OpenPanel<TOpenArg, TCloseArg>(
        this UIPanelArg2<TOpenArg, TCloseArg> panel,
        TOpenArg openArg,
        PreviousPanelVisual previousPanelVisual = PreviousPanelVisual.Visible,
        ClosePolicy closePolicy = ClosePolicy.Cache,
        System.Threading.CancellationToken? cancellationToken = null)
    {
        panel.ThrowIfUninitialized();
        PushPanelToPanelStack(panel, previousPanelVisual);
        panel.OpenPanelInternal(openArg, new(previousPanelVisual, closePolicy, null, null, cancellationToken));
    }
}