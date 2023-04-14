using UnityEngine;

public enum Team : byte
{
    RedGoo = 1,
    BlueSlime = 2,
}

public class TeamDefinitions : ScriptableObject
{
    public Team Team;
    public Color PrimaryColor;
    public Color SecondaryColor;
}