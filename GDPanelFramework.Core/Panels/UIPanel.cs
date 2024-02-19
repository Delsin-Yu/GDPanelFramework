using GDPanelSystem.Utils.AsyncInterop;

namespace GDPanelSystem.Core.Panels;

/// <summary>
/// Inherit this panel to create a panel that does not require opening / closing arguments
/// </summary>
public abstract partial class UIPanel : _UIPanelBase<Empty, Empty>
{
    /// <summary>
    /// Close this panel.
    /// </summary>
    protected void ClosePanel()
    {
        this.ThrowIfNotOpened();
        ClosePanelInternal(Empty.Default);
    }

    /// <summary>
    /// Override the parameterless version <see cref="_OnPanelClose()"/> instead.
    /// </summary>
    protected sealed override void _OnPanelClose(Empty closeParam) => _OnPanelClose();

    /// <summary>
    /// Override the parameterless version <see cref="_OnPanelOpen()"/> instead.
    /// </summary>
    protected sealed override void _OnPanelOpen(Empty openParam) => _OnPanelOpen();

    /// <summary>
    /// Called when the system is opening the panel.
    /// </summary>
    protected abstract void _OnPanelOpen();

    /// <summary>
    /// Called when the system is closing the panel.
    /// </summary>
    protected abstract void _OnPanelClose();
    
    /// <summary>
    /// A builder that handles the subsequent procedures for opening this panel.
    /// </summary>
    public readonly struct OpenArgsBuilder
    {
        private readonly UIPanel _panel;

        internal OpenArgsBuilder(UIPanel panel) => _panel = panel;
        
        private AsyncAwaitable OpenImpl(OpenLayer layer, LayerVisual previousLayerVisual, ClosePolicy closePolicy)
        {
            var panel = _panel;
            PanelManager.PushPanelToPanelStack(_panel, layer, previousLayerVisual);
            return AsyncInterop.ToAsync(
                call => panel.OpenPanelInternal(
                    Empty.Default,
                    _ =>
                    {
                        PanelManager.HandlePanelClose(panel, layer, previousLayerVisual, closePolicy);
                        call();
                    }
                )
            );
        }
        
        /// <summary>
        /// Opens this panel in a <see cref="OpenLayer.NewLayer"/>, every panel inside the previous layer will no longer receive inputs.
        /// </summary>
        /// <param name="previousLayerVisual">When setting to <see cref="LayerVisual.Hidden"/>, every panel inside the previous layer will become invisible.</param>
        /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
        /// <returns>An awaitable that, when awaited, will asynchronously continues after this panel has closed.</returns>
        public AsyncAwaitable InNewLayer(LayerVisual previousLayerVisual, ClosePolicy closePolicy = ClosePolicy.Cache) => 
            OpenImpl(OpenLayer.NewLayer, previousLayerVisual, closePolicy);

        /// <summary>
        /// Opens this panel in the same <see cref="OpenLayer.SameLayer"/>, that is, every panel inside this layer will stay active after opening this panel.
        /// </summary>
        /// <param name="closePolicy">When setting to <see cref="ClosePolicy.Delete"/>, the system will delete this panel after it is closed.</param>
        /// <returns>An awaitable that, when awaited, will asynchronously continues after this panel has closed.</returns>
        public AsyncAwaitable InCurrentLayer(ClosePolicy closePolicy = ClosePolicy.Cache) =>
            OpenImpl(OpenLayer.SameLayer, LayerVisual.Visible, closePolicy);
    }
}