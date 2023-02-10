using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnterUsername : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private DisconnectionHandler disconnectionHandler;
    [SerializeField] private GameObject mainMenu;



    public void ConfirmUsername()
    {
        string userName = usernameInput.text;

        if (userName == "")
        {
            disconnectionHandler.ShutdownCustomMessage("Invalid Username", "Please enter a valid username");
            return;
        }
        
        ClientInfo.ClientName = userName;
        welcomeText.text = "Welcome, " + ClientInfo.ClientName;
        
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
