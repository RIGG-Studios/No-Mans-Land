using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIItem : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    
    public void Init(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void Reset()
    {
        playerNameText.text = null;
        Destroy(gameObject);
    }
}
