using System;
using System.Threading;
using Godot;

namespace GDPanelFramework.Panels;

/// <summary>
/// The fundamental type for all panels, do not inherit this type.
/// </summary>
/// <typeparam name="TOpenArg">The argument passed to the panel when opening.</typeparam>
/// <typeparam name="TCloseArg">The value returned by the panel after closing.</typeparam>
public abstract partial class UIPanelBase<TOpenArg, TCloseArg> : UIPanelBaseCore
{
    internal record struct PanelOpeningMetadata(
        PreviousPanelVisual PreviousPanelVisual,
        ClosePolicy ClosePolicy,
        Action<PanelResult<TCloseArg>>? OnPanelCloseCallback,
        Action<PanelResult<Empty>>? UntypedOnPanelCloseCallback,
        CancellationToken? Token
    );


    private PanelOpeningMetadata? _metadata;

    /// <summary>
    /// The cancellation token associated with the panel, this panel will be closed when the token is canceled.
    /// </summary>
    protected CancellationToken PanelCancellationToken => _metadata?.Token ?? CancellationToken.None;
    private CancellationTokenRegistration? _registration;

    /// <summary>
    /// The argument passed to the panel when opening.
    /// </summary>
    protected TOpenArg? OpenArg { get; private set; }

    internal sealed override void InitializePanelInternal(PackedScene sourcePrefab)
    {
        base.InitializePanelInternal(sourcePrefab);
        CurrentPanelStatus = PanelStatus.Initialized;
        DelegateRunner.RunProtected(_OnPanelInitialize, "Initialize Panel", LocalName);
        PanelTweener.Init(this);
    }

    internal void OpenPanelInternal(TOpenArg openArg, PanelOpeningMetadata panelOpeningMetadata)
    {
        this.ThrowIfAlreadyOpened();
        _metadata = panelOpeningMetadata;
        _registration = panelOpeningMetadata.Token?.Register(state => ((UIPanelBase<TOpenArg, TCloseArg>)state!).ClosePanelInternal(default!), this);
        CurrentPanelStatus = PanelStatus.Opened;
        OpenArg = openArg;
        ShowPanel(() => FinishAndResetTokenSource(ref PanelOpenTweenFinishTokenSource));
        DelegateRunner.RunProtected(_OnPanelOpen, openArg, "Open Panel", LocalName);
    }

    internal void ClosePanelInternal(TCloseArg closeArg)
    {
        if (CurrentPanelStatus != PanelStatus.Opened) return;
        CurrentPanelStatus = PanelStatus.Closed;
        FinishAndResetTokenSource(ref PanelCloseTokenSource);
        
        var metadataValue = _metadata!.Value;
        var isCanceled = metadataValue.Token?.IsCancellationRequested == true;
        _registration?.Dispose();
        _registration = null;
        
        if(!isCanceled) DelegateRunner.RunProtected(_OnPanelClose, closeArg, "Close Panel", LocalName);
        else DelegateRunner.RunProtected(_OnPanelExternalClose, "External Close Panel", LocalName);
        
        DelegateRunner.RunProtected(InvokePanelClosed, "Panel Closed Event", LocalName);
        OpenArg = default;
        _metadata = null;

        PanelManager.HandlePanelClose(this, metadataValue.PreviousPanelVisual, metadataValue.ClosePolicy);
        HidePanel(() => FinishAndResetTokenSource(ref PanelCloseTweenFinishTokenSource));

        if (isCanceled)
        {
            metadataValue.UntypedOnPanelCloseCallback?.Invoke(PanelResult<Empty>.None);
            metadataValue.OnPanelCloseCallback?.Invoke(PanelResult<TCloseArg>.None);
        }
        else
        {
            metadataValue.UntypedOnPanelCloseCallback?.Invoke(Empty.Default);
            metadataValue.OnPanelCloseCallback?.Invoke(closeArg);
        }
    }

    private static void FinishAndResetTokenSource(ref CancellationTokenSource? cancellationTokenSource)
    {
        if (cancellationTokenSource == null) return;
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
    }

    internal sealed override void Cleanup()
    {
        base.Cleanup();
        _metadata = null;
        OpenArg = default;
    }

    /// <summary>
    /// Override <see cref="_OnPanelNotification"/> instead, this method is used for raising <see cref="_OnPanelPredelete"/> at the appropriate time.
    /// </summary>
    public sealed override void _Notification(int what)
    {
        base._Notification(what);

        DelegateRunner.RunProtected(_OnPanelNotification, what, "Panel Notification", LocalName);

        if (what == NotificationPredelete)
        {
            DelegateRunner.RunProtected(_OnPanelPredelete, "Delete Panel", LocalName);
            Cleanup();
        }

        if (what == NotificationWMWindowFocusOut) CancelPressedInput();
    }

    /// <summary>
    /// Called when the system is initializing the panel (<see cref="InitializePanelInternal"/>).
    /// </summary>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelInitialize() { }

    /// <summary>
    /// Called when the system is opening the panel (<see cref="OpenPanelInternal"/>).
    /// </summary>
    /// <param name="openArg">The argument passed to the panel when opening.</param>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected abstract void _OnPanelOpen(TOpenArg openArg);

    /// <summary>
    /// Called when the system is closing the panel (<see cref="OpenPanelInternal"/>).
    /// </summary>
    /// <param name="closeArg">The value returned by the panel after closing.</param>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelClose(TCloseArg closeArg) { }
    
    /// <summary>
    /// Called when the panel is closed by the cancellation token.
    /// </summary>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelExternalClose() { }

    /// <summary>
    /// Called when Godot is deleting the panel (<see cref="GodotObject.NotificationPredelete"/>).
    /// </summary>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelPredelete() { }

    /// <inheritdoc cref="GodotObject._Notification"/>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelNotification(int what) { }
}