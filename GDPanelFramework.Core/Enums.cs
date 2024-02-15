using Godot;

namespace GDPanelSystem.Core;

/// <summary>
/// Defines the behaviour when opening a new panel.
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