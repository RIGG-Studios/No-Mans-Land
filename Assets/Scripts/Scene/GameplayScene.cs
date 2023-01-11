using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using NoMansLand.Scene;
using UnityEngine;

public class GameplayScene : Scene
{
    [SerializeField] private ItemDatabase itemDatabase;
    
    protected override void OnInitialize()
    {
        Context.ItemDatabase = itemDatabase;
        
        var contextBehaviours = FindObjectsOfType<ContextBehaviour>(true);

        foreach (ContextBehaviour behaviour in contextBehaviours)
        {
            behaviour.Context = Context;
        }
        
        base.OnInitialize();
    }
    
    protected override void OnTick()
    {
        ValidateSimulationContext();
        
        base.OnTick();
    }
    
    private void ValidateSimulationContext()
    {
        var runner = Context.Runner;
        if (runner == null || runner.IsRunning == false)
        {
            Context.ObservedPlayer = null;
            return;
        }

        var observedPlayer = Context.Runner.GetPlayerObject(Context.ObservedPlayerRef);
        Context.ObservedPlayer = observedPlayer != null ? observedPlayer.GetComponent<Player>().ActivePlayer : null;
    }
}
