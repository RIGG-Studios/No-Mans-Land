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

   public GameObject Spawn(GameObject prefab, Transform parent, bool asChild = true)
   {
      GameObject spawnedPrefab = Instantiate(prefab, parent);

      if (!asChild)
      {
         spawnedPrefab.transform.SetParent(null);
      }

      return spawnedPrefab;
   }

   public void Spawn(GameObject prefab, Vector3 spawnPos, Quaternion spawnRot)
   {
      
   }
}
