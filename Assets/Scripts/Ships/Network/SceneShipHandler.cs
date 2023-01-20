using Fusion;
using UnityEngine;

public class SceneShipHandler : ContextBehaviour
{
    [SerializeField] private GameObject shipPrefab;

    [Networked, Capacity(16)] 
    public NetworkDictionary<int, NetworkObject> Ships { get; } = new();
    
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

    private NetworkObject SpawnNetworkShip(PlayerRef inputAuthority, int teamID)
    {
        ISpawnPoint[] spawnPoints = NetworkSpawnHandler.Instance.GetTeamShipSpawns(teamID);

        ISpawnPoint spawnPoint = spawnPoints[0];
        
        NetworkObject ship = Runner.Spawn(shipPrefab, spawnPoint.Transform.position, spawnPoint.Transform.rotation, inputAuthority)
            .GetComponent<NetworkObject>();

        ship.GetComponent<Ship>().Init((byte)teamID);
        return ship;
    }
}
