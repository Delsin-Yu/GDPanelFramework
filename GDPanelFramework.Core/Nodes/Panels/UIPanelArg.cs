using GDPanelSystem.Utils.AsyncInterop;

namespace GDPanelSystem.Core.Panels;

/// <summary>
/// Inherit this panel to create a panel that requires opening / closing arguments, you can use <see cref="Empty"/> for a placeholder if you do not need either side.
/// </summary>
public abstract partial class UIPanelArg<TOpenArg, TCloseArg> : _UIPanelBase<TOpenArg, TCloseArg>
{
    /// <summary>
    /// Close this panel.
    /// </summary>
    protected void ClosePanel(TCloseArg closeArg)
    {
        this.ThrowIfNotOpened();
        ClosePanelInternal(closeArg);
    }

    /// <inheritdoc cref="UIPanel.OpenArgsBuilder"/> 
    public readonly struct OpenArgsBuilder
    {
        private readonly UIPanelArg<TOpenArg, TCloseArg> _panel;
        private readonly TOpenArg m_OpenArg;

        internal OpenArgsBuilder(UIPanelArg<TOpenArg, TCloseArg> panel, TOpenArg openArg)
        {
            _panel = panel;
            m_OpenArg = openArg;
        }

        private AsyncAwaitable<TCloseArg> OpenImpl(OpenLayer layer, LayerVisual previousLayerVisual, ClosePolicy closePolicy)
        {
            var panel = _panel;
            var openParam = m_OpenArg;
            PanelManager.PushPanelToPanelStack(_panel, layer, previousLayerVisual);
            return AsyncInterop.ToAsync<TCloseArg>(
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
        
        /// <inheritdoc cref="UIPanel.OpenArgsBuilder.InNewLayer"/> 
        public AsyncAwaitable<TCloseArg> InNewLayer(LayerVisual previousLayerVisual, ClosePolicy closePolicy = ClosePolicy.Cache) => 
            OpenImpl(OpenLayer.NewLayer, previousLayerVisual, closePolicy);

        /// <inheritdoc cref="UIPanel.OpenArgsBuilder.InCurrentLayer"/> 
        public AsyncAwaitable<TCloseArg> InCurrentLayer(ClosePolicy closePolicy = ClosePolicy.Cache) =>
            OpenImpl(OpenLayer.SameLayer, LayerVisual.Visible, closePolicy);
    }
}