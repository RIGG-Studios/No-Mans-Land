using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Weapon : NetworkBehaviour, IInputProccesor
{
    public bool IsSwitching { get; private set; }
    
    
    
    public ItemController CurrentWeapon => itemControllers[CurrentWeaponID];
    
    
    
    [Networked, HideInInspector]
    public int CurrentWeaponID { get; private set; }
    
    [Networked(OnChanged = nameof(OnItemListChanged)), Capacity(16)]
    public NetworkLinkedList<ItemController> itemControllers { get; }
    
    public void ProcessInput(NetworkInputData input)
    {
        if (CurrentWeapon == null)
        {
            return;
        }
        
        
    }

    public override void Spawned()
    {
        ItemController[] itemControllers = GetComponentsInChildren<ItemController>();


        for (int i = 0; i < itemControllers.Length; i++)
        {
            this.itemControllers.Add(itemControllers[i]);
        }
    }
    
    private static void OnItemListChanged(Changed<Weapon> changed)
    {
        
    }
}
