using Godot;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

/// <summary>
/// This script serves as the entry point of the game,
/// its sole purpose is to create and open the <see cref="MainPanel"/>,
/// and terminate the game after the panel closes.
/// </summary>
public partial class GameMain : Node
{
    /// <summary>
    /// The prefab of <see cref="MainPanel"/>.
    /// </summary>
    [Export] private PackedScene? _mainPanel;
    
    /// <summary>
    /// Holds references to certain scene objects and gets passed between panels.
    /// </summary>
    [Export] private SceneObjectsModel? _sceneObjectsModel;

    /// <summary>
    /// The entry point function called by the engine, which fires the async main function.
    /// </summary>
    public override void _Ready() => ReadyAsync().Forget();
    
    /// <summary>
    /// Opens the <see cref="MainPanel"/>, after it's closed, terminates the game.
    /// </summary>
    public async GDTaskVoid ReadyAsync()
    {
        // Delay one frame for all Godot Subsystems to initialize.
        await GDTask.NextFrame();
        
        // Open the main panel and wait for it to close.
        // The control flow moves to the main panel instance, see MainPanel._OnPanelInitialize. 
        await _mainPanel.NotNull()
            .CreatePanel<MainPanel>() // Create an instance of main panel from the _mainPanel prefab.
            .OpenPanelAsync( // This async method continues after the opened main panel has closed.
                _sceneObjectsModel.NotNull(), // The argument passes to the main panel.
                closePolicy: ClosePolicy.Delete // Tells the framework to free/delete this instance (instead of caching it) after it's closed. 
            );

        // Terminates the game.
        GetTree().Quit();
    }
}