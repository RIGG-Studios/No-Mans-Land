using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdibleItemController : BaseWeapon
{
    [SerializeField] private float healAmount;
    [SerializeField] private WeaponAnimationData eatAnimationData;



    public override void Attack()
    {
        Animator.SetTrigger(eatAnimationData.name);

        StartCoroutine(IE_DelayHeal());
    }

    private IEnumerator IE_DelayHeal()
    {
        yield return new WaitForSeconds(eatAnimationData.ClipLength);

        Heal();
    }


    private void Heal()
    {
        NetworkPlayer.Local.Health.Heal(healAmount);
    }
}
