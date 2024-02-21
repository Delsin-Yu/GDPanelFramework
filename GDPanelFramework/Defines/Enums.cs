using Godot;

namespace GDPanelFramework;

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
/// Controls the visual status of the previous panel when opening a panel 
/// </summary>
public enum PreviousPanelVisual
{
    /// <summary>
    /// When opening a panel, the previous panel remains visible.
    /// </summary>
    Visible,

    /// <summary>
    /// When opening a panel, the previous panel become hidden.
    /// </summary>
    Hidden
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
    /// Triggers when the <see cref="InputEvent.IsPressed"/> method of the <see cref="InputEvent"/> returns false 
    /// </summary>
    Released,
    /// <summary>
    /// Triggers regardless the return value of the <see cref="InputEvent.IsPressed"/>.
    /// </summary>
    Any
}