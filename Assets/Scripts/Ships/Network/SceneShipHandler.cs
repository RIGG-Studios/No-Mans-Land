using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SceneShipHandler : NetworkBehaviour
{
    [SerializeField] private GameObject shipPrefab;

    [Networked, Capacity(16)] 
    public NetworkLinkedList<int> Ships { get; } = new();



    public void Activate()
    {
        
    }

    public void DeActivate()
    {
        
    }
}
