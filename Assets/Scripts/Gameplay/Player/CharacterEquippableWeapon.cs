using UnityEngine;

public class CharacterEquippableWeapon : CharacterEquippableItem
{
    [SerializeField] private ParticleSystem[] muzzleFlash;

    public override void OnAttack()
    {
        foreach (ParticleSystem particle in muzzleFlash)
        {
            particle.Play();
        }
    }
}
