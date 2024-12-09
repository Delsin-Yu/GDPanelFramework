using System;
using System.Threading;
using GDPanelFramework.Panels;
using Godot;
using GodotPanelFramework;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

/// <summary>
/// This panel serves as the game manager,
/// it handles player and mob spawning, control binding, score counting
/// and stops the game at an appropriate time.
/// </summary>
public partial class GamePanel : UIPanelArg<GamePanel.OpenContext, Empty>
{
    /// <summary>
    /// The open argument required by this panel.
    /// </summary>
    /// <param name="CenterText">The text for displaying the <see cref="GetReadyText"/>.</param>
    /// <param name="SceneObjectsModel">The model that holds scene object references.</param>
    public record struct OpenContext(Label CenterText, SceneObjectsModel SceneObjectsModel);

    /// <summary>
    /// An additional label at the top of the screen for displaying current score.
    /// </summary>
    [Export] private Label? _score;
    
    /// <summary>
    /// The delay in seconds before the mob starts to spawn.
    /// </summary>
    [Export] private double _startNewGameWaitTime;
    
    /// <summary>
    /// The delay in seconds before the next mob spawns.
    /// </summary>
    [Export] private double _mobSpawnTime;
    
    /// <summary>
    /// The prefab of a node that has a <see cref="MobController"/> attached.
    /// </summary>
    [Export] private PackedScene? _mobPrefab;
    
    /// <summary>
    /// The prefab of a node that has a <see cref="PlayerController"/> attached.
    /// </summary>
    [Export] private PackedScene? _playerPrefab;

    /// <summary>
    /// The cached size for the current window, <see cref="PlayerController"/> depends on this
    /// to determine the bounds the player is allowed to move.  
    /// </summary>
    private Vector2 _screenBound;

    private const string GetReadyText = "Get Ready!";
    
    /// <summary>
    /// Cache the size for the current viewport rect as the <see cref="_screenBound"/>.
    /// </summary>
    protected override void _OnPanelInitialize()
    {
        base._OnPanelInitialize();
        _screenBound = GetViewportRect().Size;
    }

    /// <summary>
    /// Calls the async function that manages the core game logic.
    /// </summary>
    /// <param name="openContext">The arguments required by this panel.</param>
    protected override void _OnPanelOpen(OpenContext openContext) => StartNewGameAsync(openContext).Forget();
    
    /// <summary>
    /// Manages the main game logic.
    /// </summary>
    /// <param name="openContext">The arguments required by this function.</param>
    private async GDTask StartNewGameAsync(OpenContext openContext)
    {
        // Extract the necessary variables from the open context.
        var (centerText, (mobContainer, playerContainer, mobSpawnLocation, playerSpawn)) = openContext;

        // Destroy every existing mob instance under the mob container.
        foreach (var child in mobContainer.GetChildren()) child.QueueFree();

        // Instantiate and initialize the player controller.
        var playerInstance = _playerPrefab.NotNull().Instantiate<PlayerController>();
        playerContainer.AddChild(playerInstance);
        playerInstance.Position = playerSpawn.Position;
        playerInstance.ScreenBound = _screenBound;
        
        // Setup input bindings for the player controller instance,
        // Player should able to move the character afterward
        SetupControls(true);

        // Displays the GetReadyText and set score text to zero.
        centerText.Text = GetReadyText;
        _score.NotNull().Text = "0";

        // Wait for some time before we start start spawning mobs.
        await GDTask.Delay(TimeSpan.FromSeconds(_startNewGameWaitTime));

        // Hide the large text in the center of the screen.
        centerText.Hide();

        // There two cancellation token sources are here for
        // terminating the two loops (score counting and mob spawning).
        using var mobSpawnerCts = new CancellationTokenSource();
        using var scoreTimerCts = new CancellationTokenSource();
        
        // Starts the score count and mob spawn loops,
        // since we don't need to wait for them (and they never returns)
        // we call Forget() here to suppress the compiler warning.
        StartScoreCount(mobSpawnerCts.Token).Forget();
        StartMobSpawn(scoreTimerCts.Token).Forget();

        // This line suspends the control flow and wait until player receives any collision.
        var colliders = await ToSignal(playerInstance, Area2D.SignalName.BodyEntered);

        GD.Print($"Collision to: {((Node)colliders[0]).Name}");
        
        // Deletes the player, and unbind inputs.
        SetupControls(false);
        playerInstance.QueueFree();

        // Calling cancel on these two sources will effectively
        // stops the score count and mob spawn loops. 
        mobSpawnerCts.Cancel();
        scoreTimerCts.Cancel();

        // Sets and displays the "Game Over" text.
        centerText.Text = "Game Over";
        centerText.Show();

        // Delay 2 seconds.
        await GDTask.Delay(TimeSpan.FromSeconds(2));

        // Close this panel and return the control flow back to the main panel.
        ClosePanel(Empty.Default);

        return;

        // Handles the input binding.
        void SetupControls(bool enable)
        {
            ToggleInputVector( // Associates a callback to a composite of input commands. 
                enable, // When equals to true, registers the delegate, other wise removes the registration.
                BuiltinInputNames.UIDown, // In Godot, -Y equals to move down, 
                BuiltinInputNames.UIUp,   // so that we flip the up/down bindings here.
                BuiltinInputNames.UILeft,
                BuiltinInputNames.UIRight,
                rawInput => // Gets called when input updates, for example, press and release UIRight will fire this delegate twice, with rawInput be (1, 0) and (0, 0)
                {
                    var normalized = rawInput.Normalized();
                    // Apply the last input direction to the player controller,
                    // note that this function does not move the player, only updates the cached input direction.
                    playerInstance.LastInputDirection = normalized;
                },
                CompositeInputActionState.Update
            );
        }

        // This methods handles scoring, which is a while loop that only terminate when the token is canceled.
        async GDTaskVoid StartScoreCount(CancellationToken cancellationToken)
        {
            var currentScore = 0;
            var oneSecond = TimeSpan.FromSeconds(1);
            while (!cancellationToken.IsCancellationRequested)
            {
                await GDTask
                    .Delay(oneSecond, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow(); // This line suppresses the TaskCanceledException if this delay gets cancelled.
                currentScore++;
                _score.Text = currentScore.ToString();
            }
        }

        // This methods handles mob spawning, which is a while loop that only terminate when the token is canceled.
        async GDTaskVoid StartMobSpawn(CancellationToken cancellationToken)
        {
            var mobSpawnTime = TimeSpan.FromSeconds(_mobSpawnTime);
            var mobCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                await GDTask
                    .Delay(mobSpawnTime, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();

                var mob = _mobPrefab.NotNull().Instantiate<MobController>();

                mob.Name = $"Mob#{mobCount++}";
                
                mobSpawnLocation.ProgressRatio = Random.Shared.NextSingle();

                var direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

                mob.Position = mobSpawnLocation.Position;

                direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
                mob.Rotation = direction;

                var velocity = new Vector2(Mathf.Lerp(150f, 250f, Random.Shared.NextSingle()), 0);
                mob.LinearVelocity = velocity.Rotated(direction); // Make the mob moves.

                mobContainer.AddChild(mob);
            }
        }
    }
}