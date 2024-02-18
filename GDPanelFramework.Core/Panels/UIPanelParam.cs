using System;
using System.Runtime.CompilerServices;
using GDPanelSystem.Utils.AsyncInterop;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelParam<TOpenParam, TCloseParam> : UIPanelBase<TOpenParam, TCloseParam>
{
    protected void ClosePanel(TCloseParam closeParam)
    {
        this.ThrowIfNotOpened();
        ClosePanelInternal(closeParam);
    }

    public readonly struct OpenArgsBuilder
    {
        private readonly UIPanelParam<TOpenParam, TCloseParam> _panel;
        private readonly TOpenParam _openParam;

        internal OpenArgsBuilder(UIPanelParam<TOpenParam, TCloseParam> panel, TOpenParam openParam)
        {
            _panel = panel;
            _openParam = openParam;
        }

        private AsyncAwaitable<TCloseParam> OpenImpl(OpenLayer layer, LayerVisual previousLayerVisual, CachingPolicy cachingPolicy)
        {
            var panel = _panel;
            var openParam = _openParam;
            PanelManager.PushPanelToPanelStack(_panel, layer, previousLayerVisual);
            return AsyncInterop.ToAsync<TCloseParam>(
                call => panel.OpenPanelInternal(
                    openParam,
                    result =>
                    {
                        PanelManager.HandlePanelClose(panel, layer, previousLayerVisual, cachingPolicy);
                        call(result);
                    }
                )
            );
        }
        
        public AsyncAwaitable<TCloseParam> InNewLayer(LayerVisual previousLayerVisual, CachingPolicy cachingPolicy = CachingPolicy.Cache) => 
            OpenImpl(OpenLayer.NewLayer, previousLayerVisual, cachingPolicy);

        public AsyncAwaitable<TCloseParam> InCurrentLayer(CachingPolicy cachingPolicy = CachingPolicy.Cache) =>
            OpenImpl(OpenLayer.SameLayer, LayerVisual.Visible, cachingPolicy);
    }
}