namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanel : UIPanelBase<EmptyUnit, EmptyUnit>
{
    protected void ClosePanel() => ClosePanelInternal(EmptyUnit.Default);

    protected sealed override void _OnPanelOpen(EmptyUnit openParam) => _OnPanelOpen();

    protected abstract void _OnPanelOpen();
}