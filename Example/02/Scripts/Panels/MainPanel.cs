using System;
using Godot;
using GDPanelFramework.Panels;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

/// <summary>
/// This panel serves as the main menu panel for the game,
/// it provides a start button for start a new game,
/// and a quit button to close this panel (hence terminating the game).  
/// </summary>
/// <remarks>
/// A UIPanelArg supports passing argument into the panel when it's opened,
/// and returning argument out from the panel when it's closed. 
/// int this example we don't need this returning argument,
/// so we utilize this <see cref="Empty"/> struct for a place holder.
/// This use case is also commonly found in 
/// </remarks>
public partial class MainPanel : UIPanelArg<SceneObjectsModel, Empty>
{
    /// <summary>
    /// The reference to start button.
    /// </summary>
    [Export] private Button? _startButton;
    
    /// <summary>
    /// The reference to quit button.
    /// </summary>
    [Export] private Button? _quitButton;
    
    /// <summary>
    /// The text in the center of the screen, it also get passed to <see cref="GamePanel"/> when the game starts/
    /// </summary>
    [Export] private Label? _text;
    
    /// <summary>
    /// The prefab of <see cref="GamePanel"/>.
    /// </summary>
    [Export] private PackedScene? _gamePanel;

    /// <summary>
    /// The game title in the form of string constant.
    /// </summary>
    private const string GreetingsMessage =
        """
        Dodge the
        Creeps!
        """;
    
    /// <summary>
    /// Connect necessary functions to <see cref="_startButton"/> and <see cref="_quitButton"/> when this panel is initializing.
    /// </summary>
    /// <remarks>
    /// This event function gets called once after the instantiation of this panel.<br/>
    /// The design for this function makes it suitable for handling variable initiation, signal connecting, etc, it's role is similar to what <see cref="Node._Ready"/> does.<br/>
    /// After this function call, the framework calls <see cref="_OnPanelOpen"/>.
    /// </remarks>
    protected override void _OnPanelInitialize()
    {
        // Registers the start new game function to the button.
        _startButton.NotNull().Pressed += () => StartGameAsync().Forget(); // Since we don't care when this function ends, Forget() is called for suppressing the compiler warning.
        
        // Since a UIPanelArg requires to close with an argument which we don't need, we 
        _quitButton.NotNull().Pressed += () => ClosePanel(Empty.Default);
    }
    
    /// <summary>
    /// Assign the <see cref="GreetingsMessage"/> to <see cref="_text"/>, and move the ui focus to <see cref="_startButton"/>.
    /// </summary>
    /// <remarks>
    /// This event function gets called when this panel gets opened every time.
    /// For this example where we chain CreatePanel() and OpenPanelAsync() calls,
    /// <see cref="_OnPanelInitialize"/> gets called first, and then <see cref="_OnPanelOpen"/> gets called afterward.
    /// </remarks>
    /// <param name="sceneObjectsModel">The <see cref="SceneObjectsModel"/> passed by the <see cref="GameMain"/> script,
    /// you may also access this value through <see cref="MainPanel.OpenArg"/>
    /// </param>
    protected override void _OnPanelOpen(SceneObjectsModel sceneObjectsModel)
    {
        _text.NotNull().Text = GreetingsMessage;
        _startButton.NotNull().GrabFocus();
    }

    /// <summary>
    /// Hide the buttons, create, open, and pass <see cref="_text"/> to the <see cref="GamePanel"/>,
    /// after the game panel closes, show the <see cref="GreetingsMessage"/> and, after 1 second,
    /// Show the buttons and move the ui focus to <see cref="_startButton"/>.
    /// </summary>
    private async GDTaskVoid StartGameAsync()
    {
        _startButton.NotNull().Hide();
        _quitButton.NotNull().Hide();

        // Open the game panel and wait for it to close.
        // The control flow moves to the game panel instance, see GamePanel._OnPanelInitialize. 
        await _gamePanel.NotNull()
            .CreatePanel<GamePanel>()
            .OpenPanelAsync(
                new GamePanel.OpenContext(_text.NotNull(), OpenArg.NotNull()), // Construct an instance of the open context and pass it to the game panel.
                PreviousPanelVisual.Visible, // Tells the framework not to hide the main panel when opening the game panel, so that we can keeps the _text visible.
                ClosePolicy.Cache // Tells the framework to cache this instance after it's closed, so that when the next time we are creating a game panel from _gamePanel prefab, we are reusing the same instance, this avoids unnecessary allocations.
            );
        
        _text.Text = GreetingsMessage;
        
        await GDTask.Delay(TimeSpan.FromSeconds(1));
        
        _startButton.Show();
        _quitButton.Show();
        _startButton.GrabFocus();
    }
}
