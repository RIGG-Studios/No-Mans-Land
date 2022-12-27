using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemController : MonoBehaviour
{
    [SerializeField] private AnimationClip equipAnimation;
    [SerializeField] private AnimationClip hideAnimation;

    public Item Item { get; private set; }

    private ItemListData _itemListData;
    
    public void Init(Item item, ref ItemListData itemListData)
    {
        Item = item;
        _itemListData = itemListData;
    }

    public abstract void Equip();
    public abstract void Hide();
    
    public float EquipTime => equipAnimation.length;
    public float HideTime => hideAnimation.length;
}
