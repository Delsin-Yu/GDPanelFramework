using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelBase<TOpenParam, TCloseParam> : UIPanelBaseCore
{
    private Action<TCloseParam> _onPanelCloseCallback;
    private CancellationTokenSource _panelCloseTokenSource;
    
    protected TOpenParam OpenParam { get; private set; }

    internal sealed override void InitializePanelInternal()
    {
        _panelCloseTokenSource = new CancellationTokenSource();
        CurrentPanelStatus = PanelStatus.Initialized;
        DelegateRunner.RunProtected(_OnPanelInitialize, "On Initialize Panel", Name);
    }
    
    internal void OpenPanelInternal(TOpenParam openParam, Action<TCloseParam> onPanelCloseCallback)
    {
        _onPanelCloseCallback = onPanelCloseCallback;
        CurrentPanelStatus = PanelStatus.Opened;
        OpenParam = openParam;
        DelegateRunner.RunProtected(_OnPanelOpen, openParam, "On Open Panel", Name);
    }

    internal void ClosePanelInternal(TCloseParam closeParam)
    {
        OpenParam = default;
        CurrentPanelStatus = PanelStatus.Closed;
        _panelCloseTokenSource.Cancel();
        _panelCloseTokenSource = new CancellationTokenSource();
        DelegateRunner.RunProtected(_OnPanelClose, closeParam, "On Close Panel", Name);
        var call = _onPanelCloseCallback;
        _onPanelCloseCallback = null;
        call(closeParam);
    }

    protected virtual void _OnPanelInitialize() { }
    protected abstract void _OnPanelOpen(TOpenParam openParam);
    protected virtual void _OnPanelClose(TCloseParam closeParam) { }
}