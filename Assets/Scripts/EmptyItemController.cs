using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyItemController : ItemController
{
    public override T GetService<T>()
    {
        return null;
    }

    public override void Equip()
    {
    }

    public override void Hide()
    {
    }

    public override float GetEquipTime()
    {
        return 0.0f;
    }

    public override float GetHideTime()
    {
        return 0.0f;
    }
}
