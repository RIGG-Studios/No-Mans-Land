using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public enum FadeTypes
{
    FadeIn,
    FadeOut,
    InstantFadeIn,
    InstantFadeOut
}

public class SceneUI : SceneComponent
{
    [SerializeField] private Image fadeImage;
    
    private UIComponent[] _uiComponents;
    private UIComponent _openedMenu;

    private void Awake()
    {
        _uiComponents = GetComponentsInChildren<UIComponent>();
        
    }

    private void Start()
    {
        Debug.Log(Context);
        for (int i = 0; i < _uiComponents.Length; i++)
        {
            _uiComponents[i].Init(Context);
        }
        CloseAllMenus();
    }

    public T GetService<T>() where T : UIComponent
    {
        for (int i = 0, count = _uiComponents.Length; i < count; i++)
        {
            if (_uiComponents[i] is T service)
                return service;
        }

        return null;
    }

    public void EnableMenu(string id)
    {
        if (_openedMenu)
        {
            _openedMenu.Disable();
            _openedMenu.gameObject.SetActive(false);
        }

        UIComponent nextMenu = FindUIComponent(id);
        
        Debug.Log(nextMenu);
        nextMenu.gameObject.SetActive(true);
        nextMenu.Enable();
        _openedMenu = nextMenu;
    }

    public void DisableMenu(string id)
    {
        UIComponent menu = FindUIComponent(id);

        menu.Disable();
        menu.gameObject.SetActive(false);
    }

    public void CloseAllMenus()
    {
        for (int i = 0; i < _uiComponents.Length; i++)
        {
            _uiComponents[i].Disable();
            _uiComponents[i].gameObject.SetActive(false);
        }
    }

    public void DisableCurrentMenu()
    {
        if (_openedMenu)
        {
            _openedMenu.Disable();
            _openedMenu.gameObject.SetActive(false);
        }

        _openedMenu = null;
    }

    private UIComponent FindUIComponent(string id)
    {
        for (int i = 0; i < _uiComponents.Length; i++)
        {
            if (id == _uiComponents[i].ID)
            {
                return _uiComponents[i];
            }
        }

        return null;
    }
    
    public void FadeImage(float targetAlpha, float time)
    {
        StartCoroutine(IE_Fade(time, targetAlpha));
    }
    
    public IEnumerator IE_Fade(float time, float targetAlpha)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.CrossFadeAlpha(targetAlpha, time, false);
        yield return new WaitForSeconds(time);

        if (targetAlpha == 0.0f)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
}


