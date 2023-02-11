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

    private ISpawnPoint _selectedSpawnPoint;

    
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    {
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
        
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if(spawnPoints[i].SpawnType == SpawnPointTypes.Player) PlayerSpawnPoints.Add(spawnPoints[i]);
            if(spawnPoints[i].SpawnType == SpawnPointTypes.Ship) ShipSpawnPoints.Add(spawnPoints[i]);
        }

        if (Context != null)
        {
            Context.UI.GetService<SpawnSelectionMenu>().RefreshSpawnPoints(spawnPoints.ToArray());
        }
    }


    public ISpawnPoint GetRandomPlayerSpawnPoint(int teamID)
    {
        ISpawnPoint[] spawnPoints = GetTeamPlayerSpawns(teamID);
        
        return  spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

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
            }
        }

        return spawnPoints.ToArray();
    }

    
    public void OnSpawnPointSelected(ISpawnPoint spawnPoint)
    {
        if (_selectedSpawnPoint != null)
        {
            Context.UI.GetService<SpawnSelectionMenu>().ResetSpawnUI(_selectedSpawnPoint);
        }
        
        _selectedSpawnPoint = spawnPoint;
    }

    public void OnDeploy()
    {
        if (_selectedSpawnPoint == null)
        {
            return;
        }
        
        Context.Camera.SmoothLerp(_selectedSpawnPoint.Transform,  5.0f);
        Context.UI.CloseAllMenus();
        
        StartCoroutine(SpawnPlayerDelay(3.5f));
    }

    private IEnumerator SpawnPlayerDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Context.Gameplay.RPC_RequestSpawn(Player.Local, _selectedSpawnPoint.Transform.position, _selectedSpawnPoint.Transform.rotation);
    }
}
