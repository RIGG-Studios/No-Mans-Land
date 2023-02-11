using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Object = System.Object;

public class PlayerChatExample : NetworkBehaviour, IInputProccesor
{

    public void ProcessInput(NetworkInputData input)
    {
        if (input.Buttons.IsSet(PlayerButtons.Enter))
        {
            if (Object.HasInputAuthority)
            {
                //show ui
            }

            if (Object.HasStateAuthority)
            {
                //stop movement
            }
        }
    }
}
