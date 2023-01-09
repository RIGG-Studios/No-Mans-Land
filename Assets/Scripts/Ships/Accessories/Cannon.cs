using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Networked] 
    public NetworkBool isOccupied { get; set; }

    [SerializeField] private Camera cannonCamera;

    public string LookAtID => "[F] INTERACT";
    public string ID => "Cannon";
    
    public bool ButtonInteract(NetworkPlayer player)
    {
        if (isOccupied)
        {
            return false;
        }
        
        RPC_RequestOccupyCannon(true);

        if (!isOccupied)
        {
            return false;
        }
        
        SetupCannon();
        return true;

    }

    public void StopButtonInteract()
    {
    }

    public void LookAtInteract() { }
    public void StopLookAtInteract() { }

    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestOccupyCannon(bool occupy)
    {
        isOccupied = occupy;
    }

    private void SetupCannon()
    {
        cannonCamera.gameObject.SetActive(true);
        
    }
}
