using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using Behaviour = UnityEngine.Behaviour;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; set; }

    [SerializeField] private Behaviour[] remoteComponentsToDisable;
    [SerializeField] private GameObject[] remoteGameObjectsDisable;

    [Space]
    
    [SerializeField] private UnityEvent onLocalPlayerInit;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            
            SceneHandler.Instance.ToggleSceneCamera(false);
        }
        
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        //is this player local
        if (!Object.HasInputAuthority)
        {
            for (int i = 0; i < remoteComponentsToDisable.Length; i++)
            {
                remoteComponentsToDisable[i].enabled = false;
            }

            for (int i = 0; i < remoteGameObjectsDisable.Length; i++)
            {
                remoteGameObjectsDisable[i].SetActive(false);
            }
        }
        else
        {
            onLocalPlayerInit?.Invoke();
        }
    }
}
