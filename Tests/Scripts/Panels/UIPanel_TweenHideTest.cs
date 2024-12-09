using GDPanelFramework.Panels;

namespace GDPanelFramework.Tests;

public partial class UIPanel_TweenHideTest : UIPanel
{
    public void CloseExtern() => ClosePanel();
    
    protected override void _OnPanelOpen() { }
}