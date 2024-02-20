using System;
using System.Threading;
using Godot;

namespace GDPanelSystem.Core.Panels;
/// <summary>
/// The fundamental type for all panels, do not inherit this type.
/// </summary>
/// <typeparam name="TOpenArg">The argument passed to the panel when opening.</typeparam>
/// <typeparam name="TCloseArg">The value returned by the panel after closing.</typeparam>
public abstract partial class _UIPanelBase<TOpenArg, TCloseArg> : _UIPanelBaseCore
{
    private Action<TCloseArg>? _onPanelCloseCallback;

    private readonly Action _onPanelInitialize;
    private readonly Action<TOpenArg> _onPanelOpen;
    private readonly Action<TCloseArg> _onPanelClose;
    private readonly Action _onPanelPredelete;
    private readonly Action<int> _onPanelNotification;
    
    /// <summary>
    /// The argument passed to the panel when opening.
    /// </summary>
    protected TOpenArg? OpenParam { get; private set; }

    /// <summary>
    /// Construct an instance of this panel and make necessary caching on certain event functions. 
    /// </summary>
    internal _UIPanelBase()
    {
        _onPanelInitialize = _OnPanelInitialize;
        _onPanelOpen = _OnPanelOpen;
        _onPanelClose = _OnPanelClose;
        _onPanelPredelete = _OnPanelPredelete;
        _onPanelNotification = _OnPanelNotification;
    }

    internal sealed override void InitializePanelInternal(PackedScene sourcePrefab)
    {
        base.InitializePanelInternal(sourcePrefab);
        CurrentPanelStatus = PanelStatus.Initialized;
        DelegateRunner.RunProtected(_onPanelInitialize, "Initialize Panel", Name);
    }

    internal void OpenPanelInternal(TOpenArg openArg, Action<TCloseArg> onPanelCloseCallback)
    {
        _onPanelCloseCallback = onPanelCloseCallback;
        CurrentPanelStatus = PanelStatus.Opened;
        OpenParam = openArg;
		ShowPanel(() => FinishAndResetTokenSource(ref _panelOpenTweenFinishTokenSource));
        SetPanelChildAvailability(true);
        DelegateRunner.RunProtected(_onPanelOpen, openArg, "Open Panel", Name);
	}

    internal void ClosePanelInternal(TCloseArg closeArg)
    {
        OpenParam = default;
        CurrentPanelStatus = PanelStatus.Closed;
        FinishAndResetTokenSource(ref _panelCloseTokenSource);
        HidePanel(() => FinishAndResetTokenSource(ref _panelCloseTweenFinishTokenSource));
        DelegateRunner.RunProtected(_onPanelClose, closeArg, "Close Panel", Name);
        SetPanelChildAvailability(false);
        var call = _onPanelCloseCallback!;
        _onPanelCloseCallback = null;
        call(closeArg);
	}

    private static void FinishAndResetTokenSource(ref CancellationTokenSource cancellationTokenSource)
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }
    
    /// <summary>
    /// Override <see cref="_OnPanelNotification"/> instead, this method is used for raising <see cref="_OnPanelPredelete"/> at the appropriate time.
    /// </summary>
    public sealed override void _Notification(int what)
    {
        base._Notification(what);
        if(what == NotificationPredelete) DelegateRunner.RunProtected(_onPanelPredelete, "Delete Panel", Name);
        DelegateRunner.RunProtected(_onPanelNotification, what, "Panel Notification", Name);
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