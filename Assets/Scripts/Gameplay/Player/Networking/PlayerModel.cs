using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private Renderer[] modelRenderers;
    
    public void DisableModel()
    {

        for (int i = 0; i < modelRenderers.Length; i++)
        {
            modelRenderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }
}
