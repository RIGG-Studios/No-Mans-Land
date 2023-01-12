using UnityEngine;

public enum SpawnPointTypes : byte
{
    Player,
    Ship
}

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private SpawnPointTypes spawnType;
    [SerializeField] private int teamID;

    public SpawnPointTypes SpawnType => spawnType;
    public int TeamID => teamID;
}
