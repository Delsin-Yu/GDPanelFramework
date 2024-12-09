using Godot;

namespace GDPanelFramework;

internal partial class RootPanelContainer : CanvasLayer
{
    private static Control? PanelRootInstance;

    private readonly Window _root;
    private readonly Control _container;

    internal static Control PanelRoot
    {
        get
        {
            if (PanelRootInstance != null) return PanelRootInstance;

            var panelContainer = new RootPanelContainer();
            
            PanelRootInstance = panelContainer._container;
            return PanelRootInstance;
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
        if(IsInstanceValid(inputEvent)) inputEvent.Dispose();
    }
}