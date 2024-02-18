using System;
using System.Runtime.CompilerServices;
using GDPanelSystem.Utils.AsyncInterop;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanel : UIPanelBase<Empty, Empty>
{
    protected void ClosePanel()
    {
        this.ThrowIfNotOpened();
        ClosePanelInternal(Empty.Default);
    }

    protected sealed override void _OnPanelClose(Empty closeParam) => _OnPanelClose();

    protected sealed override void _OnPanelOpen(Empty openParam) => _OnPanelOpen();

    protected abstract void _OnPanelOpen();

    protected abstract void _OnPanelClose();
    
    public readonly struct OpenArgsBuilder
    {
        private readonly UIPanel _panel;

        internal OpenArgsBuilder(UIPanel panel) => _panel = panel;
        
        private AsyncAwaitable OpenImpl(OpenLayer layer, LayerVisual previousLayerVisual, CachingPolicy cachingPolicy)
        {
            var panel = _panel;
            PanelManager.PushPanelToPanelStack(_panel, layer, previousLayerVisual);
            return AsyncInterop.ToAsync(
                call => panel.OpenPanelInternal(
                    Empty.Default,
                    _ =>
                    {
                        PanelManager.HandlePanelClose(panel, layer, previousLayerVisual, cachingPolicy);
                        call();
                    }
                )
            );
        }
        
        public AsyncAwaitable InNewLayer(LayerVisual previousLayerVisual, CachingPolicy cachingPolicy = CachingPolicy.Cache) => 
            OpenImpl(OpenLayer.NewLayer, previousLayerVisual, cachingPolicy);

        public AsyncAwaitable InCurrentLayer(CachingPolicy cachingPolicy = CachingPolicy.Cache) =>
            OpenImpl(OpenLayer.SameLayer, LayerVisual.Visible, cachingPolicy);
    }
}