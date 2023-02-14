using UnityEngine;


[CreateAssetMenu]
public class GameConfig : ScriptableObject
{
    public int maxScorePoints = 1000;
    public int maxPlayersPerGame = 16;
    public float shipRespawnDelay = 30f;
    public float backpackLife = 120f;
    public float startingGameTimer = 10f;
    public Color redTeamColor;
    public Color blueTeamColor;
}
