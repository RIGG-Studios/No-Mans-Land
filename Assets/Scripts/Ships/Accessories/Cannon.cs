using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, IInteractable
{
    public enum CannonState
    {
        Occupied,
        Empty
    }


    public CannonState State { get; private set; } = CannonState.Empty;

    [SerializeField] private Camera cannonCamera;

    public string LookAtID => "[F] INTERACT";
    public string ID => "Cannon";
    
    public bool ButtonInteract()
    {
        return true;
    }

    public void StopButtonInteract()
    {
    }
    
    public void LookAtInteract() { }
    public void StopLookAtInteract() { }

}
