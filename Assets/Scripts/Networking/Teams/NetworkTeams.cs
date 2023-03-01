using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


[System.Serializable]
public struct NetworkTeam : INetworkStruct
{
    public byte TeamID;
    public int PlayerCount;
    public int TeamScore;
    public int ShipIndex;
    public int ObjectivesOwned;


    public NetworkBool CanRespawn;
    public NetworkBool IsFull => PlayerCount >= 2;

    public NetworkTeam(byte teamID, int playerCount)
    {
        TeamID = teamID;
        PlayerCount = playerCount;
        ShipIndex = 0;
        CanRespawn = true;
        TeamScore = 0;
        ObjectivesOwned = 0;
    }
}

/// <summary>
/// this class handles team functionality like setting up, adding/removing players.
/// </summary>
public class NetworkTeams : ContextBehaviour
{
    [SerializeField] private int teamAmount = 4;
    [SerializeField] private bool autoFillTeams = true;

    [Networked, Capacity(16)] [HideInInspector] public NetworkLinkedList<NetworkTeam> Teams { get; } = new();

    public void OnEnable()
    {
        Context.Teams = this;
    }

    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        for (int i = 0; i < teamAmount; i++)
        {
            int id = i + 1;

            Teams.Add(new NetworkTeam((byte)id, 0));
        }

        for (int i = 0; i < Teams.Count; i++)
        {
            GetComponent<SceneShipHandler>()
                .RequestShip(Teams[i].TeamID, Object.StateAuthority, out NetworkObject ship);

            if (ship != null)
            {
                NetworkTeam team = Teams[i];
                team.ShipIndex = i;
                Teams.Set(i, team);
            }
        }
    }

    public void SelectTeam(int i)
    {
        RPC_RequestTeamJoin(i, Runner.LocalPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestTeamJoin(int teamIndex, PlayerRef requestedPlayer)
    {
        Context.Gameplay.TryFindPlayer(requestedPlayer, out Player player);

        if (player == null)
        {
            return;
        }

        AddToTeam(player, teamIndex);
    }

    public void AddToTeam(Player player, int teamIndex = -1)
    {
        byte teamID = 0;

        if (autoFillTeams)
        {
            teamID = GetBestTeam();
        }
        else
        {
            teamID = Teams[teamIndex].TeamID;
            UpdateTeamPlayerCount(teamIndex);
        }

        player.SetStat(StatTypes.TeamID, teamID);
    }

    public void AddToTeam(Player player, out ISpawnPoint spawnPoint, int teamIndex = -1)
    {
        byte teamID = 0;

        if (autoFillTeams)
        {
            teamID = GetBestTeam();
        }
        else
        {
            teamID = Teams[teamIndex].TeamID;
            UpdateTeamPlayerCount(teamIndex);
        }

        spawnPoint = FindObjectOfType<NetworkSpawnHandler>().GetRandomPlayerSpawnPoint(teamID);
        player.SetStat(StatTypes.TeamID, teamID);
    }


    public void RemoveFromTeam(Player player)
    {
        UpdateTeamPlayerCount(player.Stats.TeamID, -1);
    }

    private byte GetBestTeam()
    {
        for (int i = 0; i < Teams.Count; i++)
        {
            if (Teams[i].IsFull)
            {
                continue;
            }

            NetworkTeam team = Teams[i];
            team.PlayerCount++;
            Teams.Set(i, team);
            return Teams[i].TeamID;
        }

        return 0;
    }

    private void UpdateTeamPlayerCount(int teamIndex, int count = 1)
    {
        NetworkTeam team = Teams[teamIndex];
        team.PlayerCount += count;
        Teams.Set(teamIndex, team);
    }

    private NetworkTeam GetTeam(int index)
    {
        for (int i = 0; i < Teams.Count; i++)
        {
            if (index == Teams[i].TeamID)
            {
                return Teams[i];
            }
        }

        return default;
    }


    public void UpdateScore(float scoreAmt, int teamID)
    {
        NetworkTeam team = GetTeam(teamID);

        if (team.TeamScore < 0)
        {
            return;
        }
        
        team.TeamScore = (int)scoreAmt;
        Teams.Set(teamID - 1, team);
    }

    public void UpdateObjectives(int amt, int teamID)
    {
        NetworkTeam team = GetTeam(teamID);

        if (team.ObjectivesOwned <= 0 && amt <= 0)
        {
            return;
        }
        
        team.ObjectivesOwned += amt;
        Teams.Set(teamID - 1, team);
    }
}
