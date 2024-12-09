using Godot;

namespace GDPanelFramework.DodgeTheCreeps;

/// <summary>
/// This class holds references to certain scene objects and gets passed between panels.
/// </summary>
[GlobalClass]
public partial class SceneObjectsModel : Node
{
    /// <summary>
    /// Mobs are spawned under this node.
    /// </summary>
    [Export] private Node? _mobContainer;
    /// <summary>
    /// The player gets spawned under this node.
    /// </summary>
    [Export] private Node? _playerContainer;
    /// <summary>
    /// The path that indicates mob spawn locations.
    /// </summary>
    [Export] private PathFollow2D? _mobSpawnLocation;
    /// <summary>
    /// The spawn point for the player.
    /// </summary>
    [Export] private Marker2D? _playerSpawn;

    /// <summary>
    /// Extract all data with in this object model.
    /// </summary>
    /// <param name="mobContainer">Mobs are spawned under this node.</param>
    /// <param name="playerContainer">The player gets spawned under this node.</param>
    /// <param name="mobSpawnLocation">The path that indicates mob spawn locations.</param>
    /// <param name="playerSpawn">The spawn point for the player.</param>
    public void Deconstruct(out Node mobContainer, out Node playerContainer, out PathFollow2D mobSpawnLocation, out Marker2D playerSpawn)
    {
        mobContainer = _mobContainer.NotNull();
        playerContainer = _playerContainer.NotNull();
        mobSpawnLocation = _mobSpawnLocation.NotNull();
        playerSpawn = _playerSpawn.NotNull();
    }
}