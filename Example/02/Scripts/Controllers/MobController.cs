using Godot;
using System;
using GDPanelFramework;

public partial class MobController : RigidBody2D
{
	[Export] private AnimatedSprite2D? _animatedSprite;
	[Export] private VisibleOnScreenNotifier2D? _screenNotifier;
	
	public override void _Ready()
	{
		_screenNotifier.NotNull().ScreenExited += QueueFree;
		var mobTypes = _animatedSprite.NotNull().SpriteFrames.GetAnimationNames();
		_animatedSprite.Play(mobTypes[Random.Shared.Next(0, mobTypes.Length - 1)]);
	}
}
