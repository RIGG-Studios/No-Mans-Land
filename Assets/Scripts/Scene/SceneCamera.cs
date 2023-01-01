using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : SceneComponent
{
    public Camera Camera => sceneCamera;
    
    [SerializeField] private Camera sceneCamera;
    
    protected override void OnInit()
    {
        base.OnInit();
        
        Scene.Context.Camera = this;
    }
    
    public void SetActive(bool state)
    {
        Camera.gameObject.SetActive(state);
    }
}
