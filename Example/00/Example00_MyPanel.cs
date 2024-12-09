using GDPanelFramework.Panels;
using Godot;

namespace GDPanelFramework.Examples;

/// <summary>
/// Attach this script to a Control to make it a "UIPanel".
/// </summary>
public partial class Example00_MyPanel : UIPanel
{
    // These three fields are assigned in Godot Editor, through inspector.
    [Export] private Label? _text;
    [Export] private Button? _updateButton;
    [Export] private Button? _closeButton;

    // Stores the click count.
    private int _clickCount = 0;

    /// <summary>
    /// Called by the framework when this instance of panel is created,
    /// an instance can only gets created once.
    /// </summary>
    protected override void _OnPanelInitialize()
    {
        _updateButton.NotNull().Pressed += OnClick; // Calls OnClick then the _updateButton gets pressed.
        _closeButton.NotNull().Pressed += ClosePanel; // Close this panel when the _closeButton gets pressed.
    }

    /// <summary>
    /// Registered to the <see cref="_updateButton"/>.
    /// </summary>
    private void OnClick()
    {
        _clickCount++;
        _text.NotNull().Text = $"Clicked {_clickCount} time(s).";
    }

    /// <summary>
    /// Called by the framework when this instance of panel is opened. 
    /// The framework supports automatic panel caching
    /// so you may reopen a panel after it's closed and cached.
    /// </summary>
    protected override void _OnPanelOpen()
    {
        _text.NotNull().Text = "Hello World";
        _updateButton.NotNull().GrabFocus();
    }
}