namespace GDPanelFramework.Panels;

/// <summary>
/// Inherit this panel to create a panel that requires opening / closing arguments, you can use <see cref="Empty"/> for a placeholder if you do not need either side.
/// </summary>
public abstract partial class UIPanelArg<TOpenArg, TCloseArg> : UIPanelBase<TOpenArg, TCloseArg>
{
    /// <summary>
    /// Close this panel.
    /// </summary>
    /// <param name="closeArg">The argument passes to the caller after the panel has closed.</param>
    /// <remarks>
    /// The call to this method is ignored if the current panel is not opened.
    /// </remarks>
    protected void ClosePanel(TCloseArg closeArg) => ClosePanelInternal(closeArg);
}