using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : UIComponent
{
    public TextMeshProUGUI welcomeText;


    public override void Enable()
    {
        base.Enable();

        welcomeText.text = ClientInfo.ClientName;
    }
}
