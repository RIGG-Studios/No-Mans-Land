using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Item : ScriptableObject
{
    public int itemID;
    public string itemName;
    [TextArea]
    public string itemDescription;
    public Sprite itemIcon;

    [Space]
    public int maxStack = 1;

    public bool isEquippable;
    public GameObject fpPrefab;
    public GameObject tpPrefab;
    public GameObject pickupPrefab;


    public bool IsStackable => maxStack > 1;
}
