using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetworkSpawnHandler : MonoBehaviour
{
    public static NetworkSpawnHandler Instance;
    
  //  private List<SpawnPoint> _spawnPoints = new();

    public List<SpawnPoint> PlayerSpawnPoints = new();
    public List<SpawnPoint> ShipSpawnPoints = new();

    private void Awake()
    {
        Instance = this;
        
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        Debug.Log(spawnPoints.Length);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if(spawnPoints[i].SpawnType == SpawnPointTypes.Player) PlayerSpawnPoints.Add(spawnPoints[i]);
            if(spawnPoints[i].SpawnType == SpawnPointTypes.Ship) ShipSpawnPoints.Add(spawnPoints[i]);
        }
    }

    public SpawnPoint GetRandomPlayerSpawnPoint() => PlayerSpawnPoints[Random.Range(0, PlayerSpawnPoints.Count)];
    public SpawnPoint GetRandomShipSpawnPoint() => ShipSpawnPoints[Random.Range(0, ShipSpawnPoints.Count)];

    public SpawnPoint[] GetTeamPlayerSpawns(int teamID)
    {
        List<SpawnPoint> spawnPoints = new();

        for (int i = 0; i < PlayerSpawnPoints.Count; i++)
        {
            if (teamID != PlayerSpawnPoints[i].TeamID)
            {
                continue;
            }
            
            spawnPoints.Add(PlayerSpawnPoints[i]);
        }

        return spawnPoints.ToArray();
    }
    
    public SpawnPoint[] GetTeamShipSpawns(int teamID)
    {
        List<SpawnPoint> spawnPoints = new();

        for (int i = 0; i < ShipSpawnPoints.Count; i++)
        {

            Debug.Log(teamID == ShipSpawnPoints[i].TeamID);
            if (teamID == ShipSpawnPoints[i].TeamID)
            {
                spawnPoints.Add(ShipSpawnPoints[i]);
                Debug.Log(spawnPoints.Count);
            }
        }

        return spawnPoints.ToArray();
    }
    

    public void RegisterSpawnPoint(SpawnPoint point)
    {
      
    }
}
