using Godot;

namespace GDPanelFramework;

internal partial class RootPanelContainer : CanvasLayer
{
    private static Control? _backing;

    private readonly Window _root;
    private readonly Control _container;

    internal static Control PanelRoot
    {
        get
        {
            if (_backing != null) return _backing;

            var panelContainer = new RootPanelContainer();
            
            _backing = panelContainer._container;
            return _backing;
        }
    }

    public RootPanelContainer()
    {
        _root = ((SceneTree)Engine.GetMainLoop()).Root;
        
        FollowViewportEnabled = false;
        
        Name = "RootPanelViewport";
        _container = new() { Name = "PanelRoot" };
        
        AddChild(_container);

        _root.CallDeferred(Node.MethodName.AddChild, this, false, Variant.From(InternalMode.Front));
        _container.CallDeferred(Control.MethodName.SetAnchorsAndOffsetsPreset, Variant.From(Control.LayoutPreset.FullRect));
    }

    public override void _Input(InputEvent inputEvent)
    {
        var accept = PanelManager.ProcessInputEvent(inputEvent);
        if (accept && IsInstanceValid(_root)) _root.SetInputAsHandled();
    }
}