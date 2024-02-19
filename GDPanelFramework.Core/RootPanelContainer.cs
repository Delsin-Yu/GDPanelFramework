using Godot;

namespace GDPanelSystem.Core;

internal partial class RootPanelContainer : CanvasLayer
{
    private static Control? _backing;

    internal static Control PanelRoot
    {
        get
        {
            if (_backing != null) return _backing;
            
            var panelContainer = new RootPanelContainer
            {
                FollowViewportEnabled = true, 
                Name = "RootPanelViewport",
            };
            
            var container = new Control
            {
                Name = "PanelRoot"
            };
            
            panelContainer.AddChild(container);
            
            var root = ((SceneTree)Engine.GetMainLoop()).Root;
            root.CallDeferred(Node.MethodName.AddChild, panelContainer, false, Variant.From(InternalMode.Front));
            container.CallDeferred(Control.MethodName.SetAnchorsAndOffsetsPreset, Variant.From(Control.LayoutPreset.FullRect));
            
            _backing = container;
            return _backing;
        }
    }
}