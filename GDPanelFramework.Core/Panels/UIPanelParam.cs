namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelParam<TOpenParam, TCloseParam> : UIPanelBase<TOpenParam, TCloseParam>
{
    protected void ClosePanel(TCloseParam closeParam) => ClosePanelInternal(closeParam);
}