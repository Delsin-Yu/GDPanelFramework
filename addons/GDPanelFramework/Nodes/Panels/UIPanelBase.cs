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
        Action<TCloseArg>? OnPanelCloseCallback,
        Action? UntypedOnPanelCloseCallback);

    
    private PanelOpeningMetadata? _metadata;

    
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
        CurrentPanelStatus = PanelStatus.Opened;
        OpenArg = openArg;
		ShowPanel(() => FinishAndResetTokenSource(ref PanelOpenTweenFinishTokenSource));
        SetPanelChildAvailability(true);
        DelegateRunner.RunProtected(_OnPanelOpen, openArg, "Open Panel", LocalName);
	}

    internal void ClosePanelInternal(TCloseArg closeArg)
    {
        if(CurrentPanelStatus != PanelStatus.Opened) return;
        CurrentPanelStatus = PanelStatus.Closed;
        FinishAndResetTokenSource(ref PanelCloseTokenSource);
        DelegateRunner.RunProtected(_OnPanelClose, closeArg, "Close Panel", LocalName);
        OpenArg = default;
        SetPanelChildAvailability(false);

        var metadataValue = _metadata!.Value;
        _metadata = null;
        
        PanelManager.HandlePanelClose(this, metadataValue.PreviousPanelVisual, metadataValue.ClosePolicy);
        HidePanel(() => FinishAndResetTokenSource(ref PanelCloseTweenFinishTokenSource));

        metadataValue.UntypedOnPanelCloseCallback?.Invoke();
        metadataValue.OnPanelCloseCallback?.Invoke(closeArg);
	}

    private static void FinishAndResetTokenSource(ref CancellationTokenSource? cancellationTokenSource)
    {
        if(cancellationTokenSource == null) return;
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
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

        if (what == NotificationWMWindowFocusOut)
        {
            CancelPressedInput();
        }
    }

    /// <summary>
    /// Called when the system is initializing the panel (<see cref="InitializePanelInternal"/>).
    /// </summary>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelInitialize()
    {
    }

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
    protected virtual void _OnPanelClose(TCloseArg closeArg)
    {
    }

    /// <summary>
    /// Called when Godot is deleting the panel (<see cref="GodotObject.NotificationPredelete"/>).
    /// </summary>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelPredelete()
    {
    }

    /// <inheritdoc cref="GodotObject._Notification"/>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelNotification(int what)
    {
    }
}