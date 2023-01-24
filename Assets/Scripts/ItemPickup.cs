using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ItemPickup : NetworkBehaviour, IInteractable
{
    private int _itemID;
    private int _stack;
    
    public string LookAtID =>  string.Format("<color={0}>[F]</color> INTERACT", "red");
    public string ID => "ItemPickup";


    public void Init(int itemID, int stack)
    {
        _itemID = itemID;
        _stack = stack;
    }
    
    
    public void LookAtInteract() { }

    public void StopLookAtInteract() { }
    

    public bool ButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactData)
    {
        networkPlayer.Inventory.AddItem(_itemID, -1, _stack);
        interactData = default;
        
        Runner.Despawn(Object);
        
        return true;
    }

    public void StopButtonInteract(out ButtonInteractionData interactionData)
    {
        interactionData = default;
    }
}
