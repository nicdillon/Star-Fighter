public class MissionDialog
{
    public string MissionTitle { get; set; }
    public Dialog[] Intro { get; set; }
    public Dialog[] Encounter { get; set; }
    public Dialog[] Outro { get; set; }

    public Dialog RetrieveMissionDialog(int index) 
    {
        return Intro[index];
    }

    public Dialog RetrieveEncounters(int index)
    {
        return Encounter[index];
    }

    public Dialog RetrieveOutro(int index)
    {
        return Outro[index];
    }
}