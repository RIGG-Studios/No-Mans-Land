using System;
using UnityEngine;


public class SpawnPoint : MonoBehaviour, ISpawnPoint
{
    [SerializeField] private SpawnPointTypes spawnType;
    [SerializeField] private int teamID;
    [SerializeField] private bool refreshOnSpawn = false;

    public SpawnPointTypes SpawnType => spawnType;

    public virtual int TeamID { get; set; }
    
    public Transform Transform
    {
        get => transform;
        set => value = transform;
    }

    private void Awake()
    {
        TeamID = teamID;
    }

    public void Init()
    {
        if (refreshOnSpawn)
        {
            FindObjectOfType<NetworkSpawnHandler>().RefreshSpawnPoints();
        }
    }

    public virtual void OverrideTeam(int id)
    {
        TeamID = id;
    }
}
