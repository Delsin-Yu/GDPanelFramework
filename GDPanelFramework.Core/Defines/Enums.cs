using Godot;

namespace GDPanelSystem.Core;

/// <summary>
/// Defines the behavior when creating a panel.
/// </summary>
public enum CreatePolicy
{
    /// <summary>
    /// The framework will reuse a panel if there is cache available, otherwise creating a new instance.
    /// </summary>
    TryReuse,
    /// <summary>
    /// The framework will creating a new instance of panel regardless the cache status.
    /// </summary>
    ForceCreate
}

/// <summary>
/// Defines the subsequent behavior after the panel has closed.
/// </summary>
public enum ClosePolicy
{
    /// <summary>
    /// The framework will cache this instance of the panel, and, when specifying createPolicy as <see cref="CreatePolicy.TryReuse"/>, reuse it in the next <see cref="PanelManager.CreatePanel{T}"/> call.
    /// </summary>
    Cache,
    /// <summary>
    /// The framework will calls the <see cref="Node.QueueFree"/> on this panel for deletion.
    /// </summary>
    Delete
}

/// <summary>
/// Defines the behavior when opening a new panel.
/// </summary>
public enum OpenLayer
{
    /// <summary>
    /// Opens the panel in new panel layer, which means every panel inside the previous layer will no longer focusable or react to pointer click.
    /// </summary>
    NewLayer,

    /// <summary>
    /// Opens the panel in current layer, which means every panel inside this layer will remains focusable and react to pointer click if specified via <see cref="Godot.Control.MouseFilter"/>.
    /// </summary>
    SameLayer
}

/// <summary>
/// When opening a panel in <see cref="OpenLayer.NewLayer"/> mode, controls the visual status of panels inside the previous layer.
/// </summary>
public enum LayerVisual
{
    /// <summary>
    /// When opening a panel in <see cref="OpenLayer.NewLayer"/> mode, every panel inside the previous layer remains visible.
    /// </summary>
    Visible,

    /// <summary>
    /// When opening a panel in <see cref="OpenLayer.NewLayer"/> mode, every panel become hidden.
    /// </summary>
    Hidden
}

/// <summary>
/// Internal enum for indicating the selection cache result when opening panel in <see cref="OpenLayer.NewLayer"/> 
/// </summary>
internal enum SelectionCachingResult
{
    /// <summary>
    /// Nothing is currently selected system wise, the caching enumeration should stop
    /// </summary>
    NoSelections,
    /// <summary>
    /// Currently focusing control is not a child of the specified panel, the caching enumeration should continues
    /// </summary>
    NotAChild,
    /// <summary>
    /// Currently focusing control is a child of the specified panel, and is cached successfully, the caching enumeration should stop
    /// </summary>
    Successful
}

/// <summary>
/// Define the input phase of a specific <see cref="Godot.InputEvent"/>
/// </summary>
public enum InputActionPhase
{
    /// <summary>
    /// Triggers when the <see cref="InputEvent.IsPressed"/> method of the <see cref="InputEvent"/> returns true 
    /// </summary>
    Pressed,
    /// <summary>
    /// Triggers when the <see cref="InputEvent.IsReleased"/> method of the <see cref="InputEvent"/> returns true 
    /// </summary>
    Released,
    /// <summary>
    /// Triggers when the <see cref="InputEvent.IsPressed"/> or the <see cref="InputEvent.IsReleased"/> method of the <see cref="InputEvent"/> returns true
    /// </summary>
    Any
}