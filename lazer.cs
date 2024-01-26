using Godot;

public partial class lazer : Area2D
{	
	[Signal]
	public delegate void HitAsteroidEventHandler();

	public Callable OnLazerHitAteriod { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GetNode<CollisionPolygon2D>("hitbox").Disabled = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetTree().Paused == true)
			return;
		var velocity = new Vector2(1500.0f, 0.0f);
		Position += velocity * (float)delta;
	}
	
	private void _on_body_entered(Node2D body)
	{
		GetNode<CollisionPolygon2D>("HitBox").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
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
			OnLazerHitAteriod.Call();
			FreeBody(mob);
		}
	}

	private async void FreeBody(Node2D body)
	{
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		body.Free();
	}
}
