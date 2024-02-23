using System;
using Godot;

public partial class player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void HitTimerTimeoutEventHandler();
	[Signal]
	public delegate void DodgeCooldownActivatedEventHandler();
	
	[Export]
	public int Speed { get; set; } = 400;
	
	public Vector2 ScreenSize;

	private bool dodgeCooldownIsActive = false;
	private bool beamWeaponCooldownIsActive = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if (GetTree().Paused == true)
			return;
		
		if (Input.IsActionPressed("move_right"))
			velocity.X +=1;
		if (Input.IsActionPressed("move_left"))
			velocity.X -=1;
		if (Input.IsActionPressed("move_up"))
			velocity.Y -=1;
		if (Input.IsActionPressed("move_down"))
			velocity.Y +=1;
		
		if (velocity.Length() > 0)
			velocity = velocity.Normalized() * Speed;
		
		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("shift"))
		{
			if (dodgeCooldownIsActive)
				return;
			Dodge();
		}
		// else if (@event.IsActionPressed("secondary_fire"))
		// {
		// 	if (beamWeaponCooldownIsActive)
		// 		return; 
			
		// 	ShootBeam();
		// }
	}

	// private void ShootBeam()
	// {
	// 	var playerPosition = Position;
	// 	var beamWeapon = GetNode<RayCast2D>("BeamWeapon");
	// 	beamWeapon.Show();
	// 	beamWeapon.TargetPosition = new Vector2(ScreenSize.X + 48.0f, playerPosition.Y);
	// 	beamWeapon.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");
	// }

	private void Dodge()
	{
		GetNode<CollisionPolygon2D>("hitbox").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("dodging");
		Speed = 600;
		dodgeCooldownIsActive = true;
		GetNode<AudioStreamPlayer2D>("DodgeSound").Play();
		GetNode<Timer>("DodgeTimer").Start();
		GetNode<Timer>("DodgeCooldownTimer").Start();
		EmitSignal(SignalName.DodgeCooldownActivated);
	}

	private void _on_body_entered(Node2D body)
	{
		if (body is mob)
			PlayerHit();
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionPolygon2D>("hitbox").Disabled = false;
	}

	private void OnTimerTimeout()
	{
		var animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		GetNode<CollisionPolygon2D>("hitbox").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, false);
		animation.Play("moving");
		EmitSignal(SignalName.HitTimerTimeout);
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is EnemyLazer) 
		{
			PlayerHit();
		}
	}

	private void PlayerHit()
	{
		EmitSignal(SignalName.Hit);
		GetNode<CollisionPolygon2D>("hitbox").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
		var animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Play("default");
		
		var hitTimer = GetNode<Timer>("Timer");
		hitTimer.Start();
	}

	private void OnDodgeTimerTimeout()
	{
		GetNode<CollisionPolygon2D>("hitbox").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, false);
		var animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Play("moving");
	}

	private void OnDodgeCooldownTimeout()
	{
		dodgeCooldownIsActive = false;
	}
}
