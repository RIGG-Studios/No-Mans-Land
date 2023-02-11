using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnterUsername : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TextMeshProUGUI welcomeText;



    public void ConfirmUsername()
    {
        string userName = usernameInput.text;
        ClientInfo.ClientName = userName;


        welcomeText.text = "Welcome, " + ClientInfo.ClientName;
    }
}
