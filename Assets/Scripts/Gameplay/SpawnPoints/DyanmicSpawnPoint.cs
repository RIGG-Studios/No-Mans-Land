using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class DyanmicSpawnPoint : NetworkBehaviour, ISpawnPoint
{
    [SerializeField] private SpawnPointTypes spawnType;

    
    [Networked]
    public int NetworkTeamID { get; set; }
    
    [Networked]
    public bool UnderAttack { get; set; }
    
    public int TeamID
    {
        get => NetworkTeamID;
        set => value = NetworkTeamID;
    }

    public Transform Transform
    {
        get => transform;
        set => value = transform;
    }
    
    public SpawnPointTypes SpawnType => spawnType;


    public void Init()
    {
    //    FindObjectOfType<NetworkSpawnHandler>().RefreshSpawnPoints();
    }

    public void OnAttacked()
    {
        
    }

    public void OverrideTeam(int teamID)
    {
        NetworkTeamID = teamID;
    }

}
