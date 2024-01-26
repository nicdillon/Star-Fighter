using Godot;

public partial class settings_menu : CanvasLayer
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_resolution_selector_item_selected(long index)
	{
		if (index == 0)
			GetWindow().Size = new Vector2I(1920, 1080);
		if (index == 1)
			GetWindow().Size = new Vector2I(2560, 1080);
	}
}
