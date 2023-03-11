using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public enum Vitals
{
    Head,
    Arm,
    Chest,
    Leg
}

public class VitalPoint : Floater
{
    [SerializeField] private Vitals vital;
    [SerializeField] private bool useBuoyancy;


    public override void FixedUpdateNetwork()
    {
        if (useBuoyancy && rigidBody.useGravity)
        {
            base.FixedUpdateNetwork();
        }
    }
    

    public float CalculateDamage(float baseDmg)
    {
        switch (vital)
        {
            case Vitals.Arm:
                return baseDmg;
            
            case Vitals.Chest:
                return baseDmg * 1.5f;
            
            case Vitals.Head:
                return baseDmg * 2.0f;
            
            case Vitals.Leg:
                return baseDmg;
        }

        return baseDmg;
    }
}
