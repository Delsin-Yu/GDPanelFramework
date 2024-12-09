using GDPanelFramework.Panels;
using GodotTask;

namespace GDPanelFramework.Tests;

public partial class UIPanel_EventFunctionTest : UIPanel
{
    public class TestMonitor
    {
        public bool Initialized { get; set; }
        public bool Opened { get; set; }
        public bool Closed { get; set; }
        public bool Predelete { get; set; }
        public bool Notification { get; set; }
    }
    
    public TestMonitor? Monitor { get; set; }
    
    protected override void _OnPanelInitialize() => Monitor.NotNull().Initialized = true;

    protected override void _OnPanelOpen()
    {
        Monitor.NotNull().Opened = true;
        GDTask.NextFrame().ContinueWith(ClosePanel);
    }

    protected override void _OnPanelClose() => Monitor.NotNull().Closed = true;

    protected override void _OnPanelPredelete() => Monitor.NotNull().Predelete = true;

    protected override void _OnPanelNotification(int what)
    {
        if (Monitor != null) Monitor.Notification = true;
    }

}