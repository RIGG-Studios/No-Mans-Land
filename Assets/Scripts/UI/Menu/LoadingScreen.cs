using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject[] disableOnLoad;
    
    public void Enable()
    {
        gameObject.SetActive(true);

        foreach (GameObject g in disableOnLoad)
        {
            g.SetActive(false);
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
