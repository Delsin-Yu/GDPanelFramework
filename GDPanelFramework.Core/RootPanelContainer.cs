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

            var canvasLayer = new CanvasLayer
            {
                FollowViewportEnabled = true, 
                Name = "RootPanelViewport",
            };
            
            var newInstance = new RootPanelContainer();
            
            var root = ((SceneTree)Engine.GetMainLoop()).Root;
            root.CallDeferred(Node.MethodName.AddChild, newInstance, false, Variant.From(InternalMode.Front));
            newInstance.Name = "RootPanelContainer";
            _backing = newInstance;
            newInstance.CallDeferred(MethodName.InitializeBackingSize);
            return _backing;
        }
    }


    private void InitializeBackingSize()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        OffsetBottom = 0f;
        OffsetTop = 0f;
        OffsetLeft = 0f;
        OffsetRight = 0f;
    }
}