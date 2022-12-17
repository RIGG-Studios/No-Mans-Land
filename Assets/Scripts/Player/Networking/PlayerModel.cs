using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private Renderer modelRenderer;
    
    public void DisableModel()
    {
        modelRenderer.enabled = false;
    }
}
