using System;
using System.Runtime.CompilerServices;
using GDPanelSystem.Core.Core;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanel : UIPanelBase<Empty, Empty>
{
    protected void ClosePanel() => ClosePanelInternal(Empty.Default);

    protected sealed override void _OnPanelOpen(Empty openParam) => _OnPanelOpen();

    protected abstract void _OnPanelOpen();

    public readonly struct OpenArgsBuilder
    {
        private readonly UIPanel _panel;

        internal OpenArgsBuilder(UIPanel panel) => _panel = panel;

        public PanelCloseAwaitable InNewLayer(LayerVisual previousLayerVisual) => new(
            PanelManager.PushPanelToPanelStack(
                _panel,
                PanelLayer.NewLayer,
                previousLayerVisual
            ).OpenPanelInternal(Empty.Default)
        );

        public PanelCloseAwaitable InCurrentLayer() => new(
            PanelManager.PushPanelToPanelStack(
                _panel,
                PanelLayer.NewLayer,
                LayerVisual.Visible
            ).OpenPanelInternal(Empty.Default)
        );

        public struct PanelCloseAwaitable : INotifyCompletion
        {
            private UIPanelBase<Empty, Empty>.PanelCloseAwaitable _backing;

            internal PanelCloseAwaitable(UIPanelBase<Empty, Empty>.PanelCloseAwaitable backing) => _backing = backing;

            public PanelCloseAwaitable GetAwaiter() => this;

            public bool IsCompleted => _backing.IsCompleted;

            public void OnCompleted(Action continuation) => _backing.OnCompleted(continuation);

            public void GetResult() => _backing.GetResult();
        }
    }
}