using Godot;
using System.Text.Json;

public partial class DialogueScene : CanvasLayer
{
	public int Index {get; set;} = 0;
	public int MissionIndex {get; set;} = 0;

	private static string JSONPATH = "text/missionDialog.json";
	private string JsonContent;
	private MissionsText MissionsText;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		JsonContent = System.IO.File.ReadAllText(JSONPATH);
		MissionsText = JsonSerializer.Deserialize<MissionsText>(JsonContent);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void SelectTextToDisplay() 
	{
		var currentDialog = MissionsText.RetrieveMissionDialog(MissionIndex, Index);
		DisplayText(currentDialog);
	}

	public void DisplayText(string text) 
	{
		GetNode<Label>("DialogLabel").Text = text;
	}
}
