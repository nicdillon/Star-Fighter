using Godot;

public partial class EnemyLazer : Area2D
{
	[Signal]
	public delegate void HitPlayerEventHandler();
	[Signal]
	public delegate void HitAsteroidEventHandler();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetTree().Paused == true)
			return;
		var velocity = new Vector2(-1500.0f, 0.0f);
		Position += velocity * (float)delta;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		GetNode<CollisionShape2D>("HitBox").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		Hide();
		if (body is mob)
		{
			EmitSignal(SignalName.HitAsteroid);
			RigidBody2D mob = (RigidBody2D)body;
			mob.LinearVelocity = Vector2.Zero;
			mob.GetChild<CollisionPolygon2D>(2).SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);;
			mob.GetChild<TextureRect>(0).Hide();
			var animatedBody = mob.GetChild<AnimatedSprite2D>(3);
			animatedBody.Show();
			animatedBody.Play();
			var explosionSound = mob.GetChild<AudioStreamPlayer>(4);
			explosionSound.Play();
			FreeBody(mob);
		}
	}

	private async void FreeBody(Node2D body)
	{
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		body.Free();
	}
	
	private void OnPlayerHit(Area2D area)
	{
		if (area is player)
		{
			GetNode<CollisionShape2D>("HitBox").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			Hide();
			EmitSignal(SignalName.HitPlayer); 
		}
	}
}
