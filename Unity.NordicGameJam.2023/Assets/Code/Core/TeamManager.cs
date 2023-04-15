using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeamManager : MonoBehaviour
{
    public int playersPerTeam = 2;

    public TeamDefinitions Team1Definitions;
    [Space] public TeamDefinitions Team2Definitions;

    private List<PlayerCharacter> Team1 = new();
    private List<PlayerCharacter> Team2 = new();

    private bool IsMatchMakingComplete => Team1.Count == Team2.Count && Team1.Count == playersPerTeam;

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

        if (IsMatchMakingComplete) GameController.Instance.GetReady();
    }

    public void RemovePlayer(PlayerCharacter player)
    {
        if (Team1.Contains(player))
            Team1.Remove(player);
        else if (Team2.Contains(player))
            Team2.Remove(player);
    }

    private void ResetTeams()
    {
        Team1.Clear();
        Team2.Clear();
    }

}
