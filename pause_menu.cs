using Godot;

public partial class pause_menu : CanvasLayer
{
	[Signal]
	public delegate void ResumeGameEventHandler();
	[Signal]
	public delegate void RestartEventHandler();
	[Signal]
	public delegate void QuitEventHandler();
	[Signal]
	public delegate void SettingsEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_quit_pressed()
	{
		GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
		GetTree().Quit();
	}
	
	private void _on_resume_pressed()
	{
		GetTree().Paused = false;
		EmitSignal(SignalName.ResumeGame);
	}
	
	private void _on_restart_pressed()
	{
		GetTree().Paused = false;
		EmitSignal(SignalName.Restart);
	}
	
	private void _on_settings_pressed()
	{
		EmitSignal(SignalName.Settings);
	}
}
