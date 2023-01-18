using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public string LookAtID { get; }
    public string ID { get; }
    public void LookAtInteract()
    {
        throw new System.NotImplementedException();
    }

    public void StopLookAtInteract()
    {
        throw new System.NotImplementedException();
    }

    public bool ButtonInteract(NetworkPlayer networkPlayer, out ButtonInteractionData interactData)
    {
        throw new System.NotImplementedException();
    }

    public void StopButtonInteract(out ButtonInteractionData interactionData)
    {
        throw new System.NotImplementedException();
    }
}
