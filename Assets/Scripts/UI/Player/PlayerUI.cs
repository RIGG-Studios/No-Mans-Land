using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private UIComponent[] _uiComponents;
    private UIComponent _openedMenu;

    private void Awake()
    {
        _uiComponents = GetComponentsInChildren<UIComponent>(true);
    }

    private void Start()
    {
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
        
        nextMenu.gameObject.SetActive(true);
        nextMenu.Enable();
        _openedMenu = nextMenu;
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
}
