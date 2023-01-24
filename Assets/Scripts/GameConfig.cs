using UnityEngine;


[CreateAssetMenu]
public class GameConfig : ScriptableObject
{
    public string gameVersion;
    public int maxTeamSize = 4;
    public int maxPlayersPerGame = 16;
    public float shipRespawnDelay = 30f;
}
