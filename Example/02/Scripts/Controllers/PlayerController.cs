using Godot;
using GodotTask;

namespace GDPanelFramework.DodgeTheCreeps;

/// <summary>
/// This class handles the visual and movement behavior of a player,
/// it also manages collection detection. 
/// </summary>
public partial class PlayerController : Area2D
{
    [Export] private int _speed = 400; // The speed for the player
    [Export] private AnimatedSprite2D? _animatedSprite; // The sprite component for the player

    /// <summary>
    /// 
    /// </summary>
    public Vector2 ScreenBound { private get; set; }

    /// <summary>
    /// The last input direction for this controller,
    /// this property is updated by the binding method inside game panel. 
    /// Setting this to <see cref="Vector2.Zero"/> effectively stops the controller.
    /// </summary>
    public Vector2 LastInputDirection { private get; set; }
    
    /// <summary>
    /// Calls by the engine, updates the visual appearance and position of this controller.
    /// </summary>
    public override void _Process(double delta)
    {
        if (!Mathf.IsZeroApprox(LastInputDirection.Length())) // Only do visual calculation if the input direction is not zero.
        {
            _animatedSprite.NotNull().Play();
            _animatedSprite.Animation = LastInputDirection.Y != 0 ? "up" : "walk";
            _animatedSprite.FlipH = LastInputDirection.X < 0;
            _animatedSprite.FlipV = LastInputDirection.Y > 0;
        }
        else _animatedSprite.NotNull().Stop(); // Otherwise stops the animation.

        Position += LastInputDirection * _speed * (float)delta;
        Position = new(Mathf.Clamp(Position.X, 0, ScreenBound.X), Mathf.Clamp(Position.Y, 0, ScreenBound.Y));
    }
}