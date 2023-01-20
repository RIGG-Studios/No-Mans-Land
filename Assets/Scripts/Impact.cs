using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Impact : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }

    
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
