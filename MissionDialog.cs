public class MissionDialog
{
    public string MissionTitle { get; set; }
    public string[] Dialog { get; set; }

    public string RetrieveMissionDialog(int index) 
    {
        return Dialog[index];
    }
}