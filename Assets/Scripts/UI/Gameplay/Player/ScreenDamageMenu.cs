using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDamageMenu : UIComponent
{
    [SerializeField] private float blurFadeSpeed;
    [SerializeField] private float blurDuration;
    [SerializeField] private Image blurImage;
    
    private bool _fadeInBlur;
    private bool _showingBlur;
    private bool _fadeOutBlur;
    
    public override void Enable()
    {
        base.Enable();
        _fadeInBlur = true;
    }
    
    
    public override void Disable()
    {
        base.Disable();
        
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsEnabled)
        {
            return;
        }

        if (_showingBlur)
        {
            return;
        }

        if (_fadeInBlur)
        {
            Color tempColor = blurImage.color;
            tempColor.a += Time.deltaTime * blurFadeSpeed;
            blurImage.color = tempColor;
            
            if (tempColor.a >= 1.9f)
            {
                _fadeInBlur = false;
                 StartCoroutine(ShowBlur());
            }
        }

        if (_fadeOutBlur)
        {
            Color tempColor = blurImage.color;
            tempColor.a -= Time.deltaTime * blurFadeSpeed;
            blurImage.color = tempColor;

            if (tempColor.a <= 0f) {
                
                _fadeOutBlur = false;
                blurImage.enabled = false;
                Disable();
            }
        }
    }

    private IEnumerator ShowBlur()
    {
        if (_showingBlur)
        {
            yield break;
        }

        _showingBlur = true;
        blurImage.enabled = true;
        
        yield return new WaitForSeconds(blurDuration);
        
        _showingBlur = false;
        _fadeOutBlur = true;
    }
}
