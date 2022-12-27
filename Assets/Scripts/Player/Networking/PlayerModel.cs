using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private Renderer[] modelRenderers;
    
    public void DisableModel()
    {

        for (int i = 0; i < modelRenderers.Length; i++)
        {
            modelRenderers[i].enabled = false;
        }
    }
}
