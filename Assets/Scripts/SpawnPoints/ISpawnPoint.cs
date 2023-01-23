using UnityEngine;

public enum SpawnPointTypes : byte
{
    Player,
    Ship
}

public interface ISpawnPoint 
{
    public int TeamID { get; set; }
    
    public SpawnPointTypes SpawnType { get;  }

    public Transform Transform { get; set; }

    void Init();
    void OverrideTeam(int teamID);
}
