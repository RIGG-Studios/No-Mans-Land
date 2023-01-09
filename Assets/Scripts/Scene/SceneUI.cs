using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneUI : SceneComponent
{
    [SerializeField] private Text timeText;

    [SerializeField] private GameObject battleStartingWindow;
    [SerializeField] private Text battleStartingTimerText;

    
    public void ToggleTimeText(bool state) => timeText.gameObject.SetActive(state);
    public void ToggleBattleStartingWindow(bool state) => battleStartingWindow.gameObject.SetActive(state);

    public void SetTimeText(float timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.RoundToInt(timer % 60f);
        
        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
        }
 
        timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void SetBattleStartingTimerText(float timer)
    {
        battleStartingTimerText.text = ((int)timer).ToString();
    }
}
