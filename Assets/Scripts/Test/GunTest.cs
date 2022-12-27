using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GunTest : MonoBehaviour
{
    public int shots = 6;
    public DamageNotifier damage;


    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }


    void Shoot()
    {
        for (int i = 0; i < shots; i++)
        {
            Vector3 dir = Camera.main.transform.forward +
                          new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0.0f);
            
            
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit, 5000f))
            {
                damage.OnDamageEntity(hit.point, Random.Range(0, 100));
            }
        }
    }
}
