using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{
   public static SceneHandler Instance;
   
   [SerializeField] private Camera sceneCamera;


   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
   }

   public void ToggleSceneCamera(bool state)
   {
      sceneCamera.gameObject.SetActive(state);
   }
}
