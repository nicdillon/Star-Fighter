using System.Threading.Tasks;
using Godot;

public partial class EnemyBoss : Area2D
{
	[Signal]
	public delegate void BossDestroyedEventHandler();

	public float PathHeight = 400;
	public float PathWidth = 150;
	public bool BossIsActive = false;

	private float health = 100;
	private float speed = 0.3f;
	private float time = 0.0f;
	private Vector2 screenSize;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");
		screenSize = GetViewportRect().Size;
		Position = new Vector2(screenSize.X * 0.8f, screenSize.Y * 0.5f);
		PathHeight = screenSize.Y * 0.4f;
		PathWidth = screenSize.X * 0.1f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetTree().Paused == true)
			return;
		if (BossIsActive) {
			time += speed * (float)delta;
			Position = new Vector2(screenSize.X * 0.8f + PathWidth * Mathf.Sin(time), screenSize.Y * 0.5f + PathHeight * Mathf.Sin(2 * time));
		}
	}

	public void HitBoss(string projectileType)
	{
		if (projectileType == "basicLazer")
		{
			health -= 10;
			GetNode<ProgressBar>("HealthBar").Value = health;
			if (health <= 0)
			{
				Explode();
			}
		}
		if (projectileType == "beamWeapon")
		{
			health -= 0.1f;
			GetNode<ProgressBar>("HealthBar").Value = health;
			if (health <= 0)
			{
				Explode();
			}
		}
	}

	private void Explode()
	{
		PlayDeathNoise();
		GetNode<ProgressBar>("HealthBar").Hide();
		GetNode<CollisionPolygon2D>("CollisionPolygon2D").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
		var animatedBody = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedBody.Show();
		animatedBody.Play("destroyed");
		BossIsActive = false;
		EmitSignal(SignalName.BossDestroyed);
	}
	
	private async Task PlayDeathNoise()
	{
		await ToSignal(GetTree().CreateTimer(0.75f), "timeout");

		GetNode<AudioStreamPlayer>("DeathExplosion").Play();
	}

	private void OnAnimationFinished()
	{
		if (GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "destroyed")
		{
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Stop();
			Hide();
			return;
		}

		if (GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "shooting")
		{
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "default";
			return;
		}
	}

	private void OnAnimationLooped()
	{
		if (GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "destroyed")
		{
			Hide();
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Stop();
			return;
		}

		if (GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "shooting")
		{
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "default";
			return;
		}
	}

	private void OnTeleportAnimationLooped()
	{
		GetNode<AnimatedSprite2D>("Teleport").Stop();
		GetNode<AnimatedSprite2D>("Teleport").Hide();
	}

	private void OnBossActivationTimeout()
	{
		BossIsActive = true;
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Show();
		GetNode<ProgressBar>("HealthBar").Show();
		health = 100;
		GetNode<ProgressBar>("HealthBar").Value = health;
		GetNode<CollisionPolygon2D>("CollisionPolygon2D").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, false);
	}
}

