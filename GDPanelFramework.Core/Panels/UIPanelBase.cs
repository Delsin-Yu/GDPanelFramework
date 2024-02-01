using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Godot;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelBase<TOpenParam, TCloseParam> : Control
{
    private enum PanelOpenStatus
    {
        Closed,
        Opened
    }

    private bool _isInitialized = false;
    private PanelOpenStatus _panelOpenStatus = PanelOpenStatus.Closed;
    private PanelCloseAwaitable? _awaitable;
    private CancellationTokenSource _panelCloseTokenSource;
    private TCloseParam? _closeParam;

    internal void InitializePanelInternal()
    {
        _panelCloseTokenSource = new CancellationTokenSource();
        _isInitialized = true;
        Utils.RunProtected(_OnPanelInitialize, "On Initialize Panel", Name);
    }
    
    internal PanelCloseAwaitable OpenPanelInternal(TOpenParam openParam)
    {
        _awaitable = new PanelCloseAwaitable(this);
        _panelOpenStatus = PanelOpenStatus.Opened;
        Utils.RunProtected(_OnPanelOpen, openParam, "On Open Panel", Name);
        return _awaitable.Value;
    }

    internal void ClosePanelInternal(TCloseParam closeParam)
    {
        if (_panelOpenStatus != PanelOpenStatus.Closed)
        {
            var exception = new InvalidOperationException();
            Utils.ReportException(exception, "Close Panel", Name, nameof(ClosePanelInternal));
            throw exception;
        }

        _closeParam = closeParam;
        _panelOpenStatus = PanelOpenStatus.Closed;
        _panelCloseTokenSource.Cancel();
        _panelCloseTokenSource = new CancellationTokenSource();
        Utils.RunProtected(_OnPanelClose, _closeParam, "On Close Panel", Name);
    }

    protected virtual void _OnPanelInitialize() { }
    protected abstract void _OnPanelOpen(TOpenParam openParam);
    protected virtual void _OnPanelClose(TCloseParam closeParam) { }

    public struct PanelCloseAwaitable : ICriticalNotifyCompletion
    {
        private readonly UIPanelBase<TOpenParam, TCloseParam> _panel;
        private Action _continuation;

        internal PanelCloseAwaitable(UIPanelBase<TOpenParam, TCloseParam> panel)
        {
            _panel = panel;
        }

        public PanelCloseAwaitable GetAwaiter() => this;

        public bool IsCompleted => _panel._panelOpenStatus == PanelOpenStatus.Closed;

        public TCloseParam GetResult()
        {
            var closeParam = _panel._closeParam;
            _panel._closeParam = default;
            return closeParam;
        }

        public void OnCompleted(Action continuation) => 
            UnsafeOnCompleted(continuation);

        public void UnsafeOnCompleted(Action continuation) => 
            _continuation = continuation;

        internal void Complete()
        {
            _continuation?.Invoke();
        }
    }
}