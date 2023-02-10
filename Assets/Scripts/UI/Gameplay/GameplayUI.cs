using TMPro;
using UnityEngine;

public class GameplayUI : UIComponent
{
    [SerializeField] private TextMeshProUGUI gameplayTimeText;


    private void Update()
    {
        if (Context.Session == null)
        {
            return;
        }

        if (Context.Session.Object == null)
        {
            return;
        }

        float time = Context.Session.GameplayTimer;
        
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        
        gameplayTimeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
