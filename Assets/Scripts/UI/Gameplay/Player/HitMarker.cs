using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private AnimationClip hitAnimation;
    [SerializeField] private Image[] hitMarkerImages;

    private Animator _animator;


    private void Awake()
    {
        _animator = hitMarker.GetComponent<Animator>();
    }

    public void ShowHitMarker(bool isKill)
    {
        if (isKill)
        {
            for (int i = 0; i < hitMarkerImages.Length; i++)
            {
                hitMarkerImages[i].color = Color.red;
            }
        }
        else
        {
            for (int i = 0; i < hitMarkerImages.Length; i++)
            {
                hitMarkerImages[i].color = Color.white;
            }
        }


        StartCoroutine(IE_ShowHitMarker());
    }

    private IEnumerator IE_ShowHitMarker()
    {
        hitMarker.SetActive(true);
        _animator.SetTrigger("Show");

        yield return new WaitForSeconds(hitAnimation.length + delay);
        
        hitMarker.SetActive(false);
    }
}
