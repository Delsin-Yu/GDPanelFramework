using Godot;

namespace GDPanelSystem.Core;

public partial class RootPanelContainer : Control
{
    private static Control _backing;

    public static Control PanelRoot
    {
        get
        {
            if (_backing != null) return _backing;
            
            var newInstance = new RootPanelContainer
            {
                Name = "RootPanelContainer"
            };
            
            var canvasLayer = new CanvasLayer
            {
                FollowViewportEnabled = true, 
                Name = "RootPanelViewport",
            };
            
            var container = new Control
            {
                Name = "PanelRoot"
            };
            
            newInstance.AddChild(canvasLayer);
            canvasLayer.AddChild(container);
            
            var root = ((SceneTree)Engine.GetMainLoop()).Root;
            
            root.CallDeferred(Node.MethodName.AddChild, newInstance, false, Variant.From(InternalMode.Front));
            newInstance.CallDeferred(Control.MethodName.SetAnchorsAndOffsetsPreset, Variant.From(LayoutPreset.FullRect));
            container.CallDeferred(Control.MethodName.SetAnchorsAndOffsetsPreset, Variant.From(LayoutPreset.FullRect));
            
            _backing = container;
            return _backing;
        }
    }
}