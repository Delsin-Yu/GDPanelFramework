using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Godot;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelBase<TOpenParam, TCloseParam> : UIPanelBaseCore
{
    private Action<TCloseParam> _onPanelCloseCallback;
    
    private CancellationTokenSource _panelCloseTokenSource  = new();
    private CancellationTokenSource _panelOpenTransitionTokenSource  = new();
    private CancellationTokenSource _panelCloseTransitionTokenSource  = new();

    private readonly Action _onPanelInitialize;
    private readonly Action<TOpenParam> _onPanelOpen;
    private readonly Action<TCloseParam> _onPanelClose;
    
    protected TOpenParam OpenParam { get; private set; }
    
    protected CancellationToken? PanelCloseToken => _panelCloseTokenSource?.Token;
    protected CancellationToken? PanelOpenTransitionToken => _panelOpenTransitionTokenSource?.Token;
    protected CancellationToken? PanelCloseTransitionToken => _panelCloseTransitionTokenSource?.Token;

    internal UIPanelBase()
    {
        _onPanelInitialize = _OnPanelInitialize;
        _onPanelOpen = _OnPanelOpen;
        _onPanelClose = _OnPanelClose;
    }

    internal sealed override void InitializePanelInternal()
    {
        CurrentPanelStatus = PanelStatus.Initialized;
        DelegateRunner.RunProtected(_onPanelInitialize, "On Initialize Panel", Name);
    }

    internal void OpenPanelInternal(TOpenParam openParam, Action<TCloseParam> onPanelCloseCallback)
    {
        _onPanelCloseCallback = onPanelCloseCallback;
        CurrentPanelStatus = PanelStatus.Opened;
        OpenParam = openParam;
        DelegateRunner.RunProtected(_onPanelOpen, openParam, "On Open Panel", Name);

		ShowPanel(() => FinishAndResetTokenSource(ref _panelOpenTransitionTokenSource));
	}

    internal void ClosePanelInternal(TCloseParam closeParam)
    {
        OpenParam = default;
        CurrentPanelStatus = PanelStatus.Closed;
        FinishAndResetTokenSource(ref _panelCloseTokenSource);
        DelegateRunner.RunProtected(_onPanelClose, closeParam, "On Close Panel", Name);

        var call = _onPanelCloseCallback;
        _onPanelCloseCallback = null;
        call(closeParam);

		HidePanel(() => FinishAndResetTokenSource(ref _panelCloseTransitionTokenSource));
	}

    private static void FinishAndResetTokenSource(ref CancellationTokenSource cancellationTokenSource)
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    protected virtual void _OnPanelInitialize()
    {
    }

    protected abstract void _OnPanelOpen(TOpenParam openParam);

    protected virtual void _OnPanelClose(TCloseParam closeParam)
    {
    }
}