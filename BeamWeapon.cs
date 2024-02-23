using Godot;

public partial class BeamWeapon : RayCast2D
{

	[Signal]
	public delegate void HitAsteroidEventHandler();

	public Tween BeamTween;

	public bool IsCasting { get; set; } = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		GetNode<Line2D>("Line2D").Points[1] = Vector2.Zero;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetTree().Paused == true)
			return;
		if (IsColliding())
		{
			var body = GetCollider();
			OnBeamWeaponBodyEntered(body);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("secondary_fire"))
		{
			SetIsCasting(true);
		}
		if (@event.IsActionReleased("secondary_fire"))
		{
			SetIsCasting(false);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var castTo = new Vector2(0, 2000);
		var castPoint = castTo;
		ForceRaycastUpdate();

		GetNode<GpuParticles2D>("GPUParticles2D2").Emitting = IsColliding();

		if (IsColliding())
		{
			castPoint = ToLocal(GetCollisionPoint());
			GetNode<GpuParticles2D>("GPUParticles2D2").GlobalRotation = GetCollisionNormal().Angle();
			GetNode<GpuParticles2D>("GPUParticles2D2").Position = castPoint;
		}

		GetNode<Line2D>("Line2D").SetPointPosition(1, castPoint);
	}

	private void SetIsCasting(bool cast)
	{
		IsCasting = cast;
		GetNode<GpuParticles2D>("GPUParticles2D").Emitting = IsCasting;
		if (IsCasting)
		{
			Show();
			Appear();
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Show();
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
			GetNode<GpuParticles2D>("GPUParticles2D2").Emitting = true;
			Enabled = true;
		}
		else
		{
			Disappear();
			GetNode<GpuParticles2D>("GPUParticles2D2").Emitting = false;
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Stop();
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Hide();
			Enabled = false;
		}
		SetPhysicsProcess(IsCasting);
	}

	private void Appear()
	{
		var line = GetNode<Line2D>("Line2D");
		if (BeamTween != null && BeamTween.IsValid())
			BeamTween.Kill();
		
		BeamTween = CreateTween().BindNode(GetNode<Line2D>("Line2D"));
		BeamTween.TweenProperty(line, "width", 1.0, 0.4);

		// Need to adjust sound time and beam cooldown/firetime
		//GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
	}

	private void Disappear()
	{
		var line = GetNode<Line2D>("Line2D");
		if (BeamTween != null && BeamTween.IsValid())
			BeamTween.Kill();

		BeamTween = CreateTween().BindNode(GetNode<Line2D>("Line2D"));
		BeamTween.TweenProperty(line, "width", 0.0, 0.2);
	}

	private void OnBeamWeaponBodyEntered(GodotObject body)
	{
		if (body is mob mob)
		{
			HitMob(mob);
		}
		if (body is EnemyBoss boss)
		{
			boss.HitBoss("beamWeapon");
		}
	}

	private void HitMob(mob mob)
	{
		EmitSignal(SignalName.HitAsteroid);
		mob.LinearVelocity = Vector2.Zero;
		mob.GetChild<CollisionPolygon2D>(2).SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true);
		mob.GetChild<TextureRect>(0).Hide();
		var animatedBody = mob.GetChild<AnimatedSprite2D>(3);
		animatedBody.Show();
		animatedBody.Play();
		var explosionSound = mob.GetChild<AudioStreamPlayer>(4);
		explosionSound.Play();
		FreeBody(mob);
	}

	private async void FreeBody(Node2D body)
	{
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		body.Free();
	}
}
