using System;
using GDPanelSystem.Utils.AsyncInterop;
using Godot;

namespace GDPanelSystem.Core.Panels;

/// <summary>
/// Inherit this panel to create a panel that does not require opening / closing arguments
/// </summary>
public abstract partial class UIPanel : _UIPanelBase<Empty, Empty>
{
    private readonly Action _closePanel;
    private InputActionPhase? _registeredInputActionPhase;

    /// <inheritdoc cref="_UIPanelBase{T1, T2}()"/>
    protected UIPanel()
    {
        _closePanel = ClosePanel;
    }
    
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
    protected sealed override void _OnPanelClose(Empty closeArg) => _OnPanelClose();

    /// <summary>
    /// Override the parameterless version <see cref="_OnPanelOpen()"/> instead.
    /// </summary>
    protected sealed override void _OnPanelOpen(Empty openArg) => _OnPanelOpen();

    /// <summary>
    /// Called when the system is opening the panel.
    /// </summary>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected abstract void _OnPanelOpen();

    /// <summary>
    /// Called when the system is closing the panel.
    /// </summary>
    /// <remarks>
    /// This method is considered "Protected", that is, throwing an exception inside the override of this method will not cause the framework to malfunction.
    /// </remarks>
    protected virtual void _OnPanelClose()
    {
    }

    /// <summary>
    /// Enable this panel to be closed with the <see cref="PanelManager.UICancelActionName"/>.
    /// </summary>
    /// <param name="actionPhase">The action phase focuses on.</param>
    /// <remarks>
    /// Repeated calls to this method will not function, only the first call actually registers the <see cref="ClosePanel"/> method.
    /// </remarks>
    protected void EnableCloseWithCancelKey(InputActionPhase actionPhase = InputActionPhase.Released)
    {
        if(_registeredInputActionPhase != null) return;
        _registeredInputActionPhase = actionPhase;
        RegisterCancelInput(_closePanel, actionPhase);
    }

    /// <summary>
    /// Disable this panel to be closed with the <see cref="PanelManager.UICancelActionName"/>.
    /// </summary>
    /// <remarks>
    /// Repeated calls to this method will not functions, only the first call actually de-registers the <see cref="ClosePanel"/> method.
    /// </remarks>
    protected void DisableCloseWithCancelKey()
    {
        if(_registeredInputActionPhase == null) return;
        RemoveCancelInput(_closePanel, _registeredInputActionPhase!.Value);
        _registeredInputActionPhase = null;
    }

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