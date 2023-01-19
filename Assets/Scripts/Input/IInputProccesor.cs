using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputProccesor
{
    void ProcessInput(NetworkInputData input);
}
