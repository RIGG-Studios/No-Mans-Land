using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveCapturingUI : UIComponent
{
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private Image objectiveProgressImage;

    
    private Objective _objective;
    
    public void SetObjective(Objective objective)
    {
        _objective = objective;

        if (_objective != null)
        {
            objectiveText.text = _objective.objectiveName;
        }
    }


    private void Update()
    {
        if (_objective == null)
        {
            return;
        }
        
        (float, Color) fillData = _objective.GetObjectiveStatus();

        objectiveText.color = fillData.Item2;
        objectiveProgressImage.fillAmount = fillData.Item1;
        objectiveProgressImage.color = fillData.Item2;
    }
}
