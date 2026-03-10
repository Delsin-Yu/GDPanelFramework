namespace GDPanelFramework.Panels;

/// <summary>
/// Inherit this panel to create a panel that requires opening argument.
/// </summary>
public abstract partial class UIPanelArg1<TOpenArg> : UIPanelBase<TOpenArg, Empty>
{
    private InputActionPhase? _registeredInputActionPhase;

    /// <summary>
    /// Close this panel.
    /// </summary>
    /// <remarks>
    /// The call to this method is ignored if the current panel is not opened.
    /// </remarks>
    protected void ClosePanel() => ClosePanelInternal(Empty.Default);

    /// <inheritdoc cref="_OnPanelClose(Empty)"/>
    protected virtual void _OnPanelClose() { }
    
    /// <inheritdoc/>
    protected sealed override void _OnPanelClose(Empty closeArg) => _OnPanelClose();

    /// <summary>
    /// Enable this panel to be closed with the <see cref="PanelManager.UICancelActionName"/>.
    /// </summary>
    /// <param name="actionPhase">The action phase focuses on.</param>
    /// <remarks>
    /// Repeated calls to this method will not function, only the first call actually registers the <see cref="ClosePanel"/> method.
    /// </remarks>
    protected void EnableCloseWithCancelKey(InputActionPhase? actionPhase = null)
    {
        if (_registeredInputActionPhase != null) return;
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
        if (_registeredInputActionPhase == null) return;
        RemoveInputCancel(ClosePanel, _registeredInputActionPhase!.Value);
        _registeredInputActionPhase = null;
    }
}