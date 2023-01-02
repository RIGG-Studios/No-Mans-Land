using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class TeamUIItem : MonoBehaviour
{
    [SerializeField] private Transform grid;
    [SerializeField] private PlayerUIItem playerItemUIPrefab;

    private Dictionary<Player, PlayerUIItem> _players = new(4);

    
}
