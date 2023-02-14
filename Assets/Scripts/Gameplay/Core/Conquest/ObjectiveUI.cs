using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI : SpawnPointUI
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI objectiveText;



    public void UpdateProgress(float amount, Color color)
    {
        fillImage.fillAmount = amount;
        objectiveText.color = color;
        fillImage.color = color;
    }
}
