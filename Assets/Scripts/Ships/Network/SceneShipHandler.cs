using Fusion;
using UnityEngine;

public class SceneShipHandler : ContextBehaviour
{
    [SerializeField] private GameObject shipPrefab;

    [Networked, Capacity(16)] 
    public NetworkDictionary<int, NetworkObject> Ships { get; } = new();
    
    public override void Spawned()
    {
        Context.Ships = this;
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

    private NetworkObject SpawnNetworkShip(PlayerRef inputAuthority, int teamID)
    {
        SpawnPoint[] spawnPoints = NetworkSpawnHandler.Instance.GetTeamShipSpawns(teamID);

        Debug.Log(spawnPoints.Length);
        SpawnPoint spawnPoint = spawnPoints[0];
        
        NetworkObject ship = Runner.Spawn(shipPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation, inputAuthority)
            .GetComponent<NetworkObject>();

        return ship;
    }
}
