using System;
using System.Runtime.CompilerServices;
using GDPanelSystem.Core.Core;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelParam<TOpenParam, TCloseParam> : UIPanelBase<TOpenParam, TCloseParam>
{
    protected void ClosePanel(TCloseParam closeParam) => ClosePanelInternal(closeParam);

    public readonly struct OpenArgsBuilder
    {
        private readonly UIPanelParam<TOpenParam, TCloseParam> _panel;
        private readonly TOpenParam _openParam;

        public OpenArgsBuilder(UIPanelParam<TOpenParam, TCloseParam> panel, TOpenParam openParam)
        {
            _panel = panel;
            _openParam = openParam;
        }

        public PanelCloseAwaitable InNewLayer(LayerVisual previousLayerVisual) => new(
            PanelManager
                .PushPanelToPanelStack(_panel, PanelLayer.NewLayer, previousLayerVisual)
                .OpenPanelInternal(_openParam)
        );

        public PanelCloseAwaitable InCurrentLayer() => new(
            PanelManager
                .PushPanelToPanelStack(_panel, PanelLayer.NewLayer, LayerVisual.Visible)
                .OpenPanelInternal(_openParam)
        );


        public struct PanelCloseAwaitable : INotifyCompletion
        {
            private UIPanelBase<TOpenParam, TCloseParam>.PanelCloseAwaitable _backing;

            internal PanelCloseAwaitable(UIPanelBase<TOpenParam, TCloseParam>.PanelCloseAwaitable backing) => _backing = backing;

            public PanelCloseAwaitable GetAwaiter() => this;

            public bool IsCompleted => _backing.IsCompleted;

            public void OnCompleted(Action continuation) => _backing.OnCompleted(continuation);

            public TCloseParam GetResult() => _backing.GetResult();
        }
    }
}