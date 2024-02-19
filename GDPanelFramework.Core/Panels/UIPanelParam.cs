using GDPanelSystem.Utils.AsyncInterop;

namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelParam<TOpenParam, TCloseParam> : _UIPanelBase<TOpenParam, TCloseParam>
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

        private AsyncAwaitable<TCloseParam> OpenImpl(OpenLayer layer, LayerVisual previousLayerVisual, ClosePolicy closePolicy)
        {
            var panel = _panel;
            var openParam = _openParam;
            PanelManager.PushPanelToPanelStack(_panel, layer, previousLayerVisual);
            return AsyncInterop.ToAsync<TCloseParam>(
                call => panel.OpenPanelInternal(
                    openParam,
                    result =>
                    {
                        PanelManager.HandlePanelClose(panel, layer, previousLayerVisual, closePolicy);
                        call(result);
                    }
                )
            );
        }
        
        public AsyncAwaitable<TCloseParam> InNewLayer(LayerVisual previousLayerVisual, ClosePolicy closePolicy = ClosePolicy.Cache) => 
            OpenImpl(OpenLayer.NewLayer, previousLayerVisual, closePolicy);

        public AsyncAwaitable<TCloseParam> InCurrentLayer(ClosePolicy closePolicy = ClosePolicy.Cache) =>
            OpenImpl(OpenLayer.SameLayer, LayerVisual.Visible, closePolicy);
    }
}