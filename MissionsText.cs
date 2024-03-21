using System.Collections.Generic;

public class MissionsText 
{
    public List<MissionDialog> Missions { get; set; }

    public Dialog RetrieveMissionDialog(int missionIndex, int index) 
    {
        return Missions[missionIndex].Intro[index];
    }
}