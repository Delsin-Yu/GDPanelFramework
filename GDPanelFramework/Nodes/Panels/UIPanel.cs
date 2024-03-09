namespace GDPanelFramework.Panels;

/// <summary>
/// Inherit this panel to create a panel that does not require opening / closing arguments
/// </summary>
public abstract partial class UIPanel : UIPanelBase<Empty, Empty>
{
    private InputActionPhase? _registeredInputActionPhase;

    /// <inheritdoc cref="UIPanelBase{TOpenArg,TCloseArg}()"/>

    /// <summary>
    /// Close this panel.
    /// </summary>
    /// <remarks>
    /// The call to this method is ignored if the current panel is not opened.
    /// </remarks>
    protected void ClosePanel() => ClosePanelInternal(Empty.Default);

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
        RegisterInputCancel(ClosePanel, actionPhase);
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
        RemoveInputCancel(ClosePanel, _registeredInputActionPhase!.Value);
        _registeredInputActionPhase = null;
    }
}