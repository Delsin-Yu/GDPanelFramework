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
    
    /// <summary>
    /// The argument passed to the panel when opening.
    /// </summary>
    protected TOpenArg? OpenParam { get; private set; }

    internal _UIPanelBase()
    {
        _onPanelInitialize = _OnPanelInitialize;
        _onPanelOpen = _OnPanelOpen;
        _onPanelClose = _OnPanelClose;
    }

    internal sealed override void InitializePanelInternal(PackedScene sourcePrefab)
    {
        base.InitializePanelInternal(sourcePrefab);
        CurrentPanelStatus = PanelStatus.Initialized;
        DelegateRunner.RunProtected(_onPanelInitialize, "On Initialize Panel", Name);
    }

    internal void OpenPanelInternal(TOpenArg openArg, Action<TCloseArg> onPanelCloseCallback)
    {
        _onPanelCloseCallback = onPanelCloseCallback;
        CurrentPanelStatus = PanelStatus.Opened;
        OpenParam = openArg;
		ShowPanel(() => FinishAndResetTokenSource(ref _panelOpenTweenFinishTokenSource));
        SetPanelChildAvailability(true);
        DelegateRunner.RunProtected(_onPanelOpen, openArg, "On Open Panel", Name);
	}

    internal void ClosePanelInternal(TCloseArg closeArg)
    {
        OpenParam = default;
        CurrentPanelStatus = PanelStatus.Closed;
        FinishAndResetTokenSource(ref _panelCloseTokenSource);
        HidePanel(() => FinishAndResetTokenSource(ref _panelCloseTweenFinishTokenSource));
        DelegateRunner.RunProtected(_onPanelClose, closeArg, "On Close Panel", Name);
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
    /// Called when the system is initializing the panel (<see cref="InitializePanelInternal"/>).
    /// </summary>
    protected virtual void _OnPanelInitialize()
    {
    }

    /// <summary>
    /// Called when the system is opening the panel (<see cref="OpenPanelInternal"/>).
    /// </summary>
    /// <param name="openArg">The argument passed to the panel when opening.</param>
    protected abstract void _OnPanelOpen(TOpenArg openArg);

    /// <summary>
    /// Called when the system is closing the panel (<see cref="OpenPanelInternal"/>).
    /// </summary>
    /// <param name="closeArg">The value returned by the panel after closing.</param>
    protected virtual void _OnPanelClose(TCloseArg closeArg)
    {
    }
}