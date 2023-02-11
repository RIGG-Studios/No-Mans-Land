using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IAttacker 
{
    event Action onAttack;

    void Attack();
}
