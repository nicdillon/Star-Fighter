using Godot;

public partial class mob : RigidBody2D
{

	private float rotationSpeed = (float)GD.RandRange(-1f, 3f);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetTree().Paused == true)
			return;
		GetNode<CollisionPolygon2D>("CollisionPolygon2D").Rotation += rotationSpeed * (float)delta;
		GetNode<TextureRect>("TextureRect").Rotation += rotationSpeed * (float)delta;
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Rotation += rotationSpeed * (float)delta;
	}
	
	private void _on_visible_on_screen_enabler_2d_screen_exited()
	{
		QueueFree();
	}

	private void HitAsteroid(string projectileType)
	{
		if (projectileType == "basicLazer")
		{
			QueueFree();
		}
	}
}
