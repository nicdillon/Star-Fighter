using System.Reflection.Metadata.Ecma335;
using Godot;

public partial class main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	[Export]
	public PackedScene ProjectileScene { get; set; }
	[Export]
	public PackedScene EnemyProjectileScene { get; set; }

	private bool firingDisabled = true;
	private bool firingCooldownActive = true;
	private float firingCooldown = 0.25f;
	private string currentScreen = "main";
	private Vector2 screenSize = new(1920, 1080);
	private int livesLeft = 3;
	private bool bossEnemyIsActive = true;

	private int score;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// skip this method
		//NewGame();
		if (bossEnemyIsActive)
		{
			GetNode<Timer>("FiringCooldown").Start();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// MoveBoss((float)delta);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("pause"))
		{
			PauseButtonPressed();
		}
		if (@event.IsActionPressed("left_click"))
		{
			if (firingDisabled)
				return;
			ShootLazer();
		}
	}

	private void game_over()
	{
		firingDisabled = true;
		GetNode<Area2D>("Player").Hide();
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
		firingDisabled = true;

		GetNode<hud>("HUD").ShowGameOver();
	}

	public void NewGame()
	{
		score = 0;
		livesLeft = 3;
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		GetTree().CallGroup("lazers", Node.MethodName.QueueFree);
		GetTree().CallGroup("enemyLazers", Node.MethodName.QueueFree);

		GetNode<AudioStreamPlayer>("Music").Play();
		var hud = GetNode<hud>("HUD");
		hud.UpdateScore(score);
		hud.ShowMessage("Get Ready!");
		hud.UpdateLives(livesLeft);

		var player = GetNode<player>("Player");
		var startPosition = new Vector2(screenSize.X / 4, screenSize.Y / 2);
		player.Start(startPosition);

		GetNode<Timer>("StartTimer").Start();
		firingDisabled = true;
		var energyProgressBar = GetNode<CanvasLayer>("HUD").GetNode<Control>("Container").GetNode<ProgressBar>("ProgressBar");
		energyProgressBar.Show();
		energyProgressBar.Value = 0;
		firingCooldownActive = false;
		
	}

	private void _on_mob_timer_timeout()
	{
		mob mob = MobScene.Instantiate<mob>();

		var mobSpawnLocation = GetNode<PathFollow2D>("Control/MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		// Rotate the asteroid towards the players direction
		float direction = Mathf.Pi;

		mob.Position = new Vector2(GetViewport().GetVisibleRect().Size.X, mobSpawnLocation.Position.Y);

		direction += (float)GD.RandRange(-Mathf.Pi / 16, Mathf.Pi / 16);
		mob.Rotation += direction;

		var scaleFactor = (float)GD.RandRange(2, 8);

		mob.GetNode<TextureRect>("TextureRect").Scale = new Vector2(scaleFactor, scaleFactor);
		mob.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Scale = new Vector2(scaleFactor, scaleFactor);
		mob.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Scale = mob.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Scale * new Vector2(scaleFactor, scaleFactor);

		var velocity = new Vector2((float)GD.RandRange(400.0, 1000.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		AddChild(mob);
	}

	private void _on_score_timer_timeout()
	{
		// score++;

		// GetNode<hud>("HUD").UpdateScore(score);
	}

	private void _on_start_timer_timeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
		firingDisabled = false;
		firingCooldownActive = false;
	}

	private void _on_pause_menu_resume_game()
	{
		currentScreen = "main";
		GetNode<CanvasLayer>("PauseMenu").Hide();
		GetNode<CanvasLayer>("HUD").Show();
		var energyProgressBar = GetNode<CanvasLayer>("HUD").GetNode<Control>("Container").GetNode<ProgressBar>("ProgressBar");
		energyProgressBar.Show();
	}

	private void _on_pause_menu_restart()
	{
		currentScreen = "main";
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<CanvasLayer>("PauseMenu").Hide();
		GetNode<CanvasLayer>("HUD").Show();
		NewGame();
	}

	private void _on_pause_menu_settings()
	{
		currentScreen = "settings";
		GetNode<CanvasLayer>("PauseMenu").Hide();
		GetNode<CanvasLayer>("SettingsMenu").Show();
	}

	private void PauseButtonPressed()
	{
		var energyProgressBar = GetNode<CanvasLayer>("HUD").GetNode<Control>("Container").GetNode<ProgressBar>("ProgressBar");

		if (GetTree().Paused == false)
		{
			GetTree().Paused = true;
			energyProgressBar.Hide();
		}
			

		switch (currentScreen)
		{
			case "main":
				currentScreen = "pause";
				GetNode<CanvasLayer>("PauseMenu").Show();
				firingDisabled = true;
				return;
			case "pause":
				currentScreen = "main";
				GetNode<CanvasLayer>("HUD").Hide();
				GetNode<CanvasLayer>("PauseMenu").Hide();
				GetTree().Paused = false;
				firingDisabled = false;
				energyProgressBar.Show();
				return;
			case "settings":
				currentScreen = "pause";
				GetNode<CanvasLayer>("SettingsMenu").Hide();
				GetNode<CanvasLayer>("PauseMenu").Show();
				return;
			default:
				return;
		}
	}

	private void ShootLazer()
	{
		if (IsCooldownOngoing() == true)
			return;
		GetNode<AudioStreamPlayer>("LazerShotNoise").Play();
		lazer lazer = ProjectileScene.Instantiate<lazer>();

		var projectileSpawnLocation = GetNode<Area2D>("Player").Position;

		lazer.Position = projectileSpawnLocation;

		// Here we set the "OnLazerHitAsteriod" method to the callable property on 
		// the lazer object so that the lazer object can call it directly when it hits an asteroid 
		lazer.OnLazerHitAteriod = Callable.From(OnLazerHitAsteriod);

		AddChild(lazer);
		var energyProgressBar = GetNode<CanvasLayer>("HUD").GetNode<Control>("Container").GetNode<ProgressBar>("ProgressBar");
		energyProgressBar.Value += 10;
		if (energyProgressBar.Value == 100)
		{
			firingCooldownActive = true;
			energyProgressBar.GetThemeStylebox("fill").Set("border_color", new Color(1.0f, 0.25f, 0.25f));
		}
			
	}

	private void OnLazerHitAsteriod()
	{
		score++;
		GetNode<hud>("HUD").UpdateScore(score);
	}

	private bool IsCooldownOngoing()
	{
		var energyProgressBar = GetNode<CanvasLayer>("HUD").GetNode<Control>("Container").GetNode<ProgressBar>("ProgressBar");
		
		if (energyProgressBar.Value < 75)
		{
			firingCooldownActive = false;
		}
		
		return firingCooldownActive && energyProgressBar.Value > 75;
	}

	private void OnPlayerHit()
	{
		GetNode<hud>("HUD").UpdateLives(--livesLeft);
		firingDisabled = true;
		if (livesLeft == 0)
		{
			game_over();
		}
	}

	
	private void OnPlayerHitTimerTimeout()
	{
		firingDisabled = false;
	}

	private void MoveBoss(float delta) 
	{
		// var boss = GetNode<EnemyBoss>("EnemyBoss");
		// var bossMovementPath = GetNode<PathFollow2D>("BossPath/PathFollow2D");
		// bossMovementPath.ProgressRatio += delta * 0.025f;

		// // Rotate the asteroid towards the players direction
		// float direction = Mathf.Pi;

		// boss.Position = new Vector2(bossMovementPath.Position.X, bossMovementPath.Position.Y);

		//direction += (float)GD.RandRange(-Mathf.Pi / 16, Mathf.Pi / 16);
		//boss.Rotation += direction;
	}
	
	private void OnFiringCooldownTimeout()
	{
		EnemyLazer enemyLazer1 = EnemyProjectileScene.Instantiate<EnemyLazer>();
		EnemyLazer enemyLazer2 = EnemyProjectileScene.Instantiate<EnemyLazer>();

		var projectileSpawnLocation1 = GetNode<Area2D>("EnemyBoss").Position + new Vector2(-54, 28);
		var projectileSpawnLocation2 = GetNode<Area2D>("EnemyBoss").Position + new Vector2(-54, -28);

		enemyLazer1.Position = projectileSpawnLocation1;
		enemyLazer2.Position = projectileSpawnLocation2;

		AddChild(enemyLazer1);
		AddChild(enemyLazer2);

		GetNode<Area2D>("EnemyBoss").GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("shooting");
		GetNode<AudioStreamPlayer>("EnemyLazerShotNoise").Play();
	}

	private void OnBossDestroyed()
	{
		bossEnemyIsActive = false;
		GetNode<Timer>("FiringCooldown").Stop();
	}
}
