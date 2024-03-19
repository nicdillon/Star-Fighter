public class MissionDialog
{
    public string MissionTitle { get; set; }
    public Dialog[] Dialog { get; set; }

    public Dialog RetrieveMissionDialog(int index) 
    {
        return Dialog[index];
    }
}