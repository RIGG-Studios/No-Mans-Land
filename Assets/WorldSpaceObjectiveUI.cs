using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class WorldSpaceObjectiveUI : MonoBehaviour
{
    [SerializeField] private float scalar;
    [SerializeField] private TextMeshProUGUI distanceText;
    
    private void Update()
    {
        if (NetworkPlayer.Local == null)
        {
            return;
        }
        
        float dist = (NetworkPlayer.Local.transform.position - transform.position).magnitude;
        distanceText.text = (int)dist + "m";
        transform.localScale = Vector3.one * (dist * scalar);
        transform.LookAt(NetworkPlayer.Local.Camera.transform);
    }
}
