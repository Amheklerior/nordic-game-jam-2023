using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public TeamDefinitions Team1Definitions;
    [Space] public TeamDefinitions Team2Definitions;
    
    private List<PlayerCharacter> Team1 = new();
    private List<PlayerCharacter> Team2 = new();
    
    public void AddPlayer(PlayerCharacter player)
    {
        if (Team1.Count <= Team2.Count)
        {
            Team1.Add(player);
            player.PlayerTeam = Team1Definitions;
        }
        else
        {
            Team2.Add(player);
            player.PlayerTeam = Team2Definitions;
        }
    }

    public void RemovePlayer(PlayerCharacter player)
    {
        if (Team1.Contains(player))
            Team1.Remove(player);
        else if (Team2.Contains(player))
            Team2.Remove(player);
    }
}
