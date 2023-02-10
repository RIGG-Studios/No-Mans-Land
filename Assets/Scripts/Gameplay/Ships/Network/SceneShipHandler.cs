using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SceneShipHandler : ContextBehaviour
{
    [SerializeField] private GameObject shipPrefab;

    [Networked, Capacity(16)] 
    public NetworkDictionary<int, NetworkObject> Ships { get; } = new();
    
    private List<SpawnRequest> _spawnRequests = new();

    
    public struct SpawnRequest
    {
        public int TeamID;
        public Ship OldShip;
        public int Tick;
    }

    protected override void Awake()
    {
        base.Awake();
        
        SceneShipHandlerInstance.InitShipHandler(this);
    }
    
    public void RequestShip(int teamID, PlayerRef inputAuthority, out NetworkObject ship)
    {
        if (!HasStateAuthority)
        {
            ship = null;
            return;
        }

        ship = SpawnNetworkShip(inputAuthority, teamID);
        Ships.Add(teamID, ship);
    }

    public void RequestShipRespawn(Ship ship, int teamID)
    {
        int delayTicks = Mathf.RoundToInt(Runner.Simulation.Config.TickRate * Context.Config.shipRespawnDelay);
        
        _spawnRequests.Add(new SpawnRequest()
        {
            TeamID =  teamID,
            OldShip = ship,
            Tick = Runner.Tick + delayTicks,
        });
    }
    
    
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        int currentTick = Runner.Tick;

        for (int i = _spawnRequests.Count - 1; i >= 0; i--)
        {
            var request = _spawnRequests[i];

            if (request.Tick > currentTick)
            {
                continue;
            }

            _spawnRequests.RemoveAt(i);
            SpawnNetworkShip(default, request.TeamID);
            Runner.Despawn(request.OldShip.Object);
        }
    }
    
    
    private NetworkObject SpawnNetworkShip(PlayerRef inputAuthority, int teamID)
    {
        Debug.Log(teamID);
        ISpawnPoint[] spawnPoints = NetworkSpawnHandler.Instance.GetTeamShipSpawns(teamID);

        ISpawnPoint spawnPoint = spawnPoints[0];
        
        NetworkObject ship = Runner.Spawn(shipPrefab, spawnPoint.Transform.position, spawnPoint.Transform.rotation, inputAuthority)
            .GetComponent<NetworkObject>();

        ship.GetComponent<Ship>().Init((byte)teamID);
        return ship;
    }
    
}
