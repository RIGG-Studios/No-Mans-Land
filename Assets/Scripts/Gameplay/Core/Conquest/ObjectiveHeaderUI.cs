using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveHeaderUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    

    public void UpdateProgress(float amount, Color color)
    {
    //    fillImage.fillAmount = amount;
        fillImage.color = color;
    }
}
