using Godot;

namespace GDPanelFramework;

/// <summary>
/// Represents a type that is capable of listening to global input events.
/// </summary>
public interface IGlobalInputListener
{
    /// <summary>
    /// Called when a global input event occurs.
    /// </summary>
    void OnGlobalInput(InputEvent inputEvent);
}