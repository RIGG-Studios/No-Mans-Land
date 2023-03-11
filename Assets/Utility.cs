using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility 
{

    public static NetworkPlayer FindClosestPlayer(NetworkPlayer[] players, Transform transform)
    {
        NetworkPlayer foundPlayer = null;
        float minDist = Mathf.Infinity;

        foreach (NetworkPlayer player in players)
        {
            float dist = (player.transform.position - transform.position).magnitude;

            if (dist <= minDist)
            {
                minDist = dist;
                foundPlayer = player;
            }
        }

        return foundPlayer;
    }
}
