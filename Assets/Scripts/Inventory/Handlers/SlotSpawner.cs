using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SlotSpawner 
{
    private static GameObject SlotPrefab
    {
        get
        {
            GameObject slot = Resources.Load<GameObject>("Prefabs/Slot");

            return slot;
        }
    }

    public static List<Slot> AllSlots = new ();

    public static Slot[] GenerateSlots(Transform grid, int size)
    {
        if (SlotPrefab == null)
        {
            Debug.Log("Trying to spawn slots with no scene handler");
            return null;
        }
        
        List<Slot> slots = new();

        for (int i = 0; i < size; i++)
        {
            Slot slot = SceneHandler.Instance.Spawn(SlotPrefab, grid).GetComponent<Slot>();
            
            slots.Add(slot);
            AllSlots.Add(slot);
        }

        return slots.ToArray();
    }
}
