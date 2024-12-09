using Godot;
using GodotTask;

namespace GDPanelFramework.Examples;

/// <summary>
/// The bootstrap script that creates and opens the panel.
/// </summary>
public partial class Example01_Main : Node
{
    /// <summary>
    /// The packed panel.
    /// </summary>
    [Export] private PackedScene? _panelPrefab;

    /// <summary>
    /// Executes the main logic after one frame since the game starts. 
    /// This is required by the GDPanelFramework for adding its panel root into the scene tree.
    /// </summary>
    public override void _Ready() =>
        GDTask.NextFrame().ContinueWith(OnReady);

    private void OnReady()
    {
        _panelPrefab.NotNull()
            .CreatePanel<Example01_MyPanel>() // This extension method tells the framework to create or reuse an instance of this panel.
            .OpenPanel( // This method tells the framework to opens the panel.
                "Hello World!", // Passes the argument to the panel.
                onPanelCloseCallback: // This delegate gets called when this panel gets closed when the panel itself calls ClosePanel().
                result => // Prints the result and terminate the application when this panel gets closed.
                {
                    GD.Print($"Clicked {result} time(s) before closed.");
                    GetTree().Quit();
                }
            );
    }
}