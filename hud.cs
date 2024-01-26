using Godot;

public partial class hud : CanvasLayer
{

	[Signal]
	public delegate void StartGameEventHandler();
	[Signal]
	public delegate void QuitEventHandler();
	
	private Color energyColor = new(0, 170, 208, 255);
	private Color hotEnergyColor = new(255, 0, 0, 255);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var energyProgressBar = GetNode<Control>("Container").GetNode<ProgressBar>("ProgressBar");
		energyProgressBar.Value -= 5 * delta;
		
		if (energyProgressBar.Value <= 75)
		{
			energyProgressBar.GetThemeStylebox("fill").Set("bg_color", new Color(0, 170, 208, 255));
			energyProgressBar.GetThemeStylebox("fill").Set("border_color", new Color(255, 255, 255, 255));
		}
		else 
		{
			energyProgressBar.GetThemeStylebox("fill").Set("bg_color", energyColor.Lerp(hotEnergyColor, (float)(4 * ((float)energyProgressBar.Value / 100.0f) - 0.75f)));
		}
	}

	public void ShowMessage(string text)
	{
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	async public void ShowGameOver() 
	{
		ShowMessage("Game Over");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		var message = GetNode<Label>("Message");
		message.Text = "Dodge the Asteriods!";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
	}

	public void UpdateScore(int score)
	{
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}
	
	private void _on_message_timer_timeout()
	{
		GetNode<Label>("Message").Hide();
	}
	
	private void _on_start_button_pressed()
	{
		GetNode<Button>("StartButton").Hide();
		GetNode<Button>("Quit").Hide();
		EmitSignal(SignalName.StartGame);
	}
	
	private void _on_quit_pressed()
	{
		GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
		GetTree().Quit();
	}

	public void UpdateLives(int livesLeft)
	{
		var healthContainer = GetNode<Control>("Container").GetNode<Control>("Control");
		switch (livesLeft) 
		{
			case 0: 
				healthContainer.GetNode<TextureRect>("TextureRect").Hide();
				return;
			case 1: 
				healthContainer.GetNode<TextureRect>("TextureRect2").Hide();
				return;
			case 2: 
				healthContainer.GetNode<TextureRect>("TextureRect3").Hide();
				return;
			case 3: 
				healthContainer.GetNode<TextureRect>("TextureRect").Show();
				healthContainer.GetNode<TextureRect>("TextureRect2").Show();
				healthContainer.GetNode<TextureRect>("TextureRect3").Show();
				return;
			default: 
				return;
		}
		
	}
}
