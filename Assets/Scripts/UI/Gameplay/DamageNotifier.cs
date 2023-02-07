using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class DamageNotifier : MonoBehaviour
{
    public static DamageNotifier Instance;
    
    [SerializeField] private GameObject hitMarkerAnimator;

    private void Awake()
    {
        Instance = this;
    }

    public void OnDamageEntity(Vector3 target, Vector3 pos, float damage)
    {
        Vector3 normal = Vector3.Cross(target, pos);
        Vector3 dir = (pos - target).normalized;

        GameObject g = Instantiate(hitMarkerAnimator, pos + -dir, Quaternion.LookRotation(normal));
        g.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
        g.transform.LookAt(target);
        
        
        Destroy(g, 4.0f);
    }
}
