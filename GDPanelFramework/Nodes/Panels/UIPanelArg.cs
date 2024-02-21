namespace GDPanelFramework.Panels;

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
}