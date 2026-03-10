using Godot;

namespace GDPanelFramework;

partial class RootPanelContainer : CanvasLayer
{
    public Control Container { get; }

    private readonly Window _root;

    public RootPanelContainer(PanelManager.ConfigurePanelContainerHandler? customHandler)
    {
        _root = ((SceneTree)Engine.GetMainLoop()).Root;

        FollowViewportEnabled = false;

        Name = "RootPanelViewport";

        if (customHandler is not null)
            Container = customHandler(this);
        else
        {
            Container = new();
            Container.Name = "PanelRoot";
            AddChild(Container);
        }

        _root.CallDeferred(Node.MethodName.AddChild, this, false, Variant.From(InternalMode.Front));
        Container.CallDeferred(Control.MethodName.SetAnchorsAndOffsetsPreset, Variant.From(Control.LayoutPreset.FullRect));
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsEcho())
        {
            if (IsInstanceValid(_root)) _root.SetInputAsHandled();
            inputEvent.Dispose();
            return;
        }

        var isSynthesisedEvent = _synthesisedEvent.Remove(inputEvent);

        if (!isSynthesisedEvent)
        {
            ProcessEchoEvents(inputEvent);
            var accept = PanelManager.ProcessInputEvent(inputEvent);
            if (accept && IsInstanceValid(_root))
                _root.SetInputAsHandled();
        }

        if (!IsInstanceValid(inputEvent)) return;

        inputEvent.Dispose();
    }
}