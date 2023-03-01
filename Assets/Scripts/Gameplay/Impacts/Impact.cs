using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.VFX;

public class Impact : NetworkBehaviour
{
    [SerializeField] private ParticleSystem vfx;
    
    
    [Networked] 
    private TickTimer life { get; set; }

    public override void Spawned()
    {
        vfx.Play();
    }
    
    public void Init(float time = 1.3f)
    {
        life  = TickTimer.CreateFromSeconds(Runner, time); ;
    }


    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
