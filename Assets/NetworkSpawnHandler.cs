using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using Behaviour = UnityEngine.Behaviour;
using Random = UnityEngine.Random;

public class NetworkSpawnHandler : ContextBehaviour
{
    [SerializeField] private NetworkSpawnerUI _spawnerUI;
    
    public static NetworkSpawnHandler Instance;
    
  //  private List<SpawnPoint> _spawnPoints = new();

    public List<ISpawnPoint> PlayerSpawnPoints = new();
    public List<ISpawnPoint> ShipSpawnPoints = new();

    private void Awake()
    {
        Instance = this;
        
        RefreshSpawnPoints();
    }

    public void RefreshSpawnPoints()
    {
        PlayerSpawnPoints.Clear();
        ShipSpawnPoints.Clear();

        List<ISpawnPoint> spawnPoints = new();
        var spawnPoint = FindObjectsOfType<Behaviour>().OfType<ISpawnPoint>();
        
        foreach (ISpawnPoint s in spawnPoint)
        {
            spawnPoints.Add(s);
        }
        
        Debug.Log(spawnPoints.Count);

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if(spawnPoints[i].SpawnType == SpawnPointTypes.Player) PlayerSpawnPoints.Add(spawnPoints[i]);
            if(spawnPoints[i].SpawnType == SpawnPointTypes.Ship) ShipSpawnPoints.Add(spawnPoints[i]);
        }

        Context.UI.GetService<SpawnSelectionMenu>().RefreshSpawnPoints(spawnPoints.ToArray());
    }


    public ISpawnPoint GetRandomPlayerSpawnPoint() => PlayerSpawnPoints[Random.Range(0, PlayerSpawnPoints.Count)];
    public ISpawnPoint GetRandomShipSpawnPoint() => ShipSpawnPoints[Random.Range(0, ShipSpawnPoints.Count)];

    public ISpawnPoint[] GetTeamPlayerSpawns(int teamID)
    {
        List<ISpawnPoint> spawnPoints = new();

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
    
    public ISpawnPoint[] GetTeamShipSpawns(int teamID)
    {
        List<ISpawnPoint> spawnPoints = new();

        for (int i = 0; i < ShipSpawnPoints.Count; i++)
        {

            if (teamID == ShipSpawnPoints[i].TeamID)
            {
                spawnPoints.Add(ShipSpawnPoints[i]);
                Debug.Log(spawnPoints.Count);
            }
        }

        return spawnPoints.ToArray();
    }


    private ISpawnPoint _selectedSpawnPoint;
    public void OnSpawnPointSelected(ISpawnPoint spawnPoint)
    {
        _selectedSpawnPoint = spawnPoint;
    }

    public void OnDeploy()
    {
        if (_selectedSpawnPoint == null)
        {
            return;
        }
        
        Context.Camera.SmoothLerp(_selectedSpawnPoint.Transform,  5.0f);
        StartCoroutine(SpawnPlayerDelay());
    }

    private IEnumerator SpawnPlayerDelay()
    {
        yield return new WaitForSeconds(2.0f);
        Context.Gameplay.RPC_RequestSpawn(Player.Local, _selectedSpawnPoint.Transform.position, _selectedSpawnPoint.Transform.rotation);
    }
}
