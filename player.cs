using Godot;
using System;

public partial class player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void HitTimerTimeoutEventHandler();
	
	[Export]
	public int Speed { get; set; } = 400;
	
	public Vector2 ScreenSize;
	
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
	}

	private async void _on_body_entered(Node2D body)
	{
		var animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Show();
		animation.Play();
		EmitSignal(SignalName.Hit);
		GetNode<CollisionPolygon2D>("hitbox").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
		var hitTimer = GetNode<Timer>("Timer");
		hitTimer.Start();
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
		GetNode<CollisionPolygon2D>("hitbox").Disabled = false;
		animation.Stop();
		animation.Frame = 0;
		animation.Hide();
		EmitSignal(SignalName.HitTimerTimeout);
	}
}
