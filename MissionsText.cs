using System.Collections.Generic;

public class MissionsText 
{
    public List<MissionDialog> Missions { get; set; }

    public string RetrieveMissionDialog(int missionIndex, int index) 
    {
        return Missions[missionIndex].Dialog[index];
    }
}