namespace GDPanelSystem.Core.Panels;

public abstract partial class UIPanelExtern : UIPanel
{
    protected virtual void OnExitExtern() { }

    public void CloseExternPanel()
    {
        OnExitExtern();
        ClosePanel();
    }
}