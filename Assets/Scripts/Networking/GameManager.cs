using Fusion;
using UnityEngine;


[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkCallBackEvents))]
public class GameManager : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private Session sessionPrefab;
    [SerializeField] private Gameplay gameplayPrefab;

    
    private bool _componentsSetup;

    public void PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
        {
            return;
        }

        if (!_componentsSetup)
        {
            Runner.Spawn(sessionPrefab);
            Runner.Spawn(gameplayPrefab);
            _componentsSetup = true;
        }
    }
}
