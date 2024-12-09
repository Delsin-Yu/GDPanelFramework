using GDPanelFramework.Panels;
using GodotTask;

namespace GDPanelFramework.Tests;

public partial class UIPanel_TokenTest : UIPanel
{
    public class TestMonitor
    {
        public bool PanelCloseTokenCanceled { get; set; }
        public bool PanelOpenTweenFinishTokenCanceled { get; set; }
        public bool PanelCloseTweenFinishTokenCanceled { get; set; }
    }
    
    public TestMonitor? Monitor { get; set; }

    protected override void _OnPanelInitialize()
    {
        base._OnPanelInitialize();
        PanelCloseToken.RegisterWithoutCaptureExecutionContext(
            monitor => ((TestMonitor)monitor).PanelCloseTokenCanceled = true,
            Monitor
        );
        PanelOpenTweenFinishToken.RegisterWithoutCaptureExecutionContext(
            monitor => ((TestMonitor)monitor).PanelOpenTweenFinishTokenCanceled = true,
            Monitor
        );
        PanelCloseTweenFinishToken.RegisterWithoutCaptureExecutionContext(
            monitor => ((TestMonitor)monitor).PanelCloseTweenFinishTokenCanceled = true,
            Monitor
        );
    }

    protected override void _OnPanelOpen()
    {
        GDTask.NextFrame().ContinueWith(ClosePanel);
    }
}