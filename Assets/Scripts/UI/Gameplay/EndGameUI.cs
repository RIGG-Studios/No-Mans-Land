using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameUI : UIComponent
{
    [SerializeField] private GameObject gameWon;
    [SerializeField] private GameObject gameLost;


    public void OnGameOver(int winnerID)
    {
        if (Player.Local == null)
        {
            return;
        }

        if (winnerID == Player.Local.Stats.TeamID)
        {
            gameWon.SetActive(true);
            gameLost.SetActive(false);
        }
        else
        {
            gameLost.SetActive(true);
            gameWon.SetActive(false);
        }
    }
}
