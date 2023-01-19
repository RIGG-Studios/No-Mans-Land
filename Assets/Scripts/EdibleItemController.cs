using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class EdibleItemController : BaseWeapon
{
    [SerializeField] private float healAmount;
    [SerializeField] private WeaponAnimationData eatAnimationData;

    [Networked]
    public NetworkBool IsEating { get; set; }

    public override void ProcessInput(WeaponContext context)
    {
        bool hasItem = Player.Inventory.FindItem(item.itemID, out ItemListData itemData);
        bool firePressed = context.Input.Buttons.IsSet(PlayerButtons.Fire);

        if (!firePressed || IsEating || !hasItem)
        {
            return;
        }
        
        
        if (Object.HasInputAuthority)
        {
            Animator.SetTrigger(eatAnimationData.name);
        }

        Player.Inventory.UpdateItemStack(itemData, 1);
        if (Object.HasStateAuthority)
        {
            StartCoroutine(IE_DelayHeal());
        }
    }

    private IEnumerator IE_DelayHeal()
    {
        IsEating = true;
        yield return new WaitForSeconds(eatAnimationData.ClipLength);

        Heal();
        IsEating = false;
    }


    private void Heal()
    {
        Player.Health.Heal(healAmount);
    }
}
